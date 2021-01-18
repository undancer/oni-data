using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Delaunay.Geo;
using Klei;
using KSerialization;
using LibNoiseDotNet.Graphics.Tools.Noise.Builder;
using ProcGen;
using ProcGen.Map;
using ProcGen.Noise;
using STRINGS;
using UnityEngine;
using VoronoiTree;

namespace ProcGenGame
{
	[Serializable]
	public class WorldGen
	{
		public delegate bool OfflineCallbackFunction(StringKey stringKeyRoot, float completePercent, WorldGenProgressStages.Stages stage);

		public enum GenerateSection
		{
			SolarSystem,
			WorldNoise,
			WorldLayout,
			RenderToMap,
			CollectSpawners
		}

		[Flags]
		public enum DebugFlags
		{
			SitePoly = 0x4
		}

		private const string _SIM_SAVE_FILENAME = "WorldGenSimSave.dat";

		private const string _WORLDGEN_SAVE_FILENAME = "WorldGenDataSave.dat";

		private const int heatScale = 2;

		private const int UNPASSABLE_EDGE_COUNT = 4;

		private const string heat_noise_name = "noise/Heat";

		private const string base_noise_name = "noise/Default";

		private const string cave_noise_name = "noise/DefaultCave";

		private const string density_noise_name = "noise/DefaultDensity";

		public const int WORLDGEN_SAVE_MAJOR_VERSION = 1;

		public const int WORLDGEN_SAVE_MINOR_VERSION = 1;

		public static Element voidElement;

		public static Element vacuumElement;

		public static Element katairiteElement;

		public static Element unobtaniumElement;

		public static List<string> diseaseIds = new List<string>
		{
			"FoodPoisoning",
			"SlimeLung"
		};

		public bool isRunningDebugGen;

		private bool generateNoiseData = true;

		private Data data;

		private OfflineCallbackFunction successCallbackFn;

		private bool running = true;

		private Thread generateThread;

		private Thread renderThread;

		private Action<OfflineWorldGen.ErrorInfo> errorCallback;

		private SeededRandom myRandom;

		private NoiseMapBuilderPlane heatSource;

		private bool wasLoaded;

		public int polyIndex = -1;

		[EnumFlags]
		public DebugFlags drawOptions;

		public static string SIM_SAVE_FILENAME => System.IO.Path.Combine(Util.RootFolder(), "WorldGenSimSave.dat");

		public static string WORLDGEN_SAVE_FILENAME => System.IO.Path.Combine(Util.RootFolder(), "WorldGenDataSave.dat");

		public int BaseLeft => Settings.GetBaseLocation().left;

		public int BaseRight => Settings.GetBaseLocation().right;

		public int BaseTop => Settings.GetBaseLocation().top;

		public int BaseBot => Settings.GetBaseLocation().bottom;

		public Dictionary<string, object> stats
		{
			get;
			private set;
		}

		public bool HasData => data != null;

		public bool HasNoiseData
		{
			get
			{
				if (HasData)
				{
					return data.world != null;
				}
				return false;
			}
		}

		public float[] DensityMap => data.world.density;

		public float[] HeatMap => data.world.heatOffset;

		public float[] OverrideMap => data.world.overrides;

		public float[] BaseNoiseMap => data.world.data;

		public float[] DefaultTendMap => data.world.defaultTemp;

		public Chunk World => data.world;

		public Vector2I WorldSize => data.world.size;

		public Vector2I SubWorldSize => data.subWorldSize;

		public WorldLayout WorldLayout => data.worldLayout;

		public List<TerrainCell> OverworldCells => data.overworldCells;

		public List<TerrainCell> TerrainCells => data.terrainCells;

		public List<ProcGen.River> Rivers => data.rivers;

		public GameSpawnData SpawnData => data.gameSpawnData;

		public int ChunkEdgeSize => data.chunkEdgeSize;

		public WorldGenSettings Settings
		{
			get;
			private set;
		}

		public WorldGen(string worldName, List<string> chosenTraits, bool assertMissingTraits)
		{
			LoadSettings();
			Settings = new WorldGenSettings(worldName, chosenTraits, assertMissingTraits);
			data = new Data();
			data.chunkEdgeSize = Settings.GetIntSetting("ChunkEdgeSize");
			data.subWorldSize = new Vector2I(Settings.GetIntSetting("SubWorldWidth"), Settings.GetIntSetting("SubWorldHeight"));
			stats = new Dictionary<string, object>();
		}

		public WorldGen(string worldName, List<string> chosenTraits, Data data, Dictionary<string, object> stats, bool assertMissingTraits)
		{
			LoadSettings();
			Settings = new WorldGenSettings(worldName, chosenTraits, assertMissingTraits);
			this.data = data;
			this.stats = stats;
		}

		public static void SetupDefaultElements()
		{
			voidElement = ElementLoader.FindElementByHash(SimHashes.Void);
			vacuumElement = ElementLoader.FindElementByHash(SimHashes.Vacuum);
			katairiteElement = ElementLoader.FindElementByHash(SimHashes.Katairite);
			unobtaniumElement = ElementLoader.FindElementByHash(SimHashes.Unobtanium);
		}

		public void Reset()
		{
			wasLoaded = false;
		}

		public static void LoadSettings()
		{
			ListPool<YamlIO.Error, WorldGen>.PooledList pooledList = ListPool<YamlIO.Error, WorldGen>.Allocate();
			if (SettingsCache.LoadFiles(pooledList))
			{
				TemplateCache.Init();
			}
			_ = CustomGameSettings.Instance != null;
			if (Application.isPlaying)
			{
				Global.Instance.modManager.HandleErrors(pooledList);
			}
			else
			{
				foreach (YamlIO.Error item in pooledList)
				{
					YamlIO.LogError(item, force_log_as_warning: false);
				}
			}
			pooledList.Recycle();
		}

		public void SaveSettings(string newpath = null)
		{
			SettingsCache.Save((newpath == null) ? SettingsCache.GetPath() : newpath);
		}

		public void InitRandom(int worldSeed, int layoutSeed, int terrainSeed, int noiseSeed)
		{
			data.globalWorldSeed = worldSeed;
			data.globalWorldLayoutSeed = layoutSeed;
			data.globalTerrainSeed = terrainSeed;
			data.globalNoiseSeed = noiseSeed;
			Console.WriteLine($"Seeds are [{worldSeed}/{layoutSeed}/{terrainSeed}/{noiseSeed}]");
			myRandom = new SeededRandom(worldSeed);
		}

		public void Initialise(OfflineCallbackFunction callbackFn, Action<OfflineWorldGen.ErrorInfo> error_cb, int worldSeed = -1, int layoutSeed = -1, int terrainSeed = -1, int noiseSeed = -1)
		{
			if (wasLoaded)
			{
				Debug.LogError("Initialise called after load");
				return;
			}
			successCallbackFn = callbackFn;
			errorCallback = error_cb;
			Debug.Assert(successCallbackFn != null);
			isRunningDebugGen = false;
			running = false;
			int num = UnityEngine.Random.Range(0, int.MaxValue);
			if (worldSeed == -1)
			{
				worldSeed = num;
			}
			if (layoutSeed == -1)
			{
				layoutSeed = num;
			}
			if (terrainSeed == -1)
			{
				terrainSeed = num;
			}
			if (noiseSeed == -1)
			{
				noiseSeed = num;
			}
			data.gameSpawnData = new GameSpawnData();
			DebugUtil.LogArgs($"World seeds: [{worldSeed}/{layoutSeed}/{terrainSeed}/{noiseSeed}]");
			InitRandom(worldSeed, layoutSeed, terrainSeed, noiseSeed);
			TerrainCell.ClearClaimedCells();
			successCallbackFn(UI.WORLDGEN.COMPLETE.key, 0f, WorldGenProgressStages.Stages.Failure);
			stats["GenerateTime"] = 0;
			stats["GenerateNoiseTime"] = 0;
			stats["GenerateLayoutTime"] = 0;
			stats["ConvertVoroToMapTime"] = 0;
			WorldLayout.SetLayerGradient(SettingsCache.layers.LevelLayers);
		}

		public void DontGenerateNoiseData()
		{
			generateNoiseData = false;
		}

		public void GenerateOfflineThreaded()
		{
			if (wasLoaded)
			{
				Debug.LogError("GenerateOfflineThreaded called after load");
				return;
			}
			Debug.Assert(Settings.world != null, "You need to set a world");
			if (Settings.world != null)
			{
				running = true;
				generateThread = new Thread(GenerateOffline);
				Util.ApplyInvariantCultureToThread(generateThread);
				generateThread.Start();
			}
		}

		public void RenderWorldThreaded()
		{
			if (wasLoaded)
			{
				Debug.LogError("RenderWorldThreaded called after load");
				return;
			}
			running = true;
			renderThread = new Thread(RenderOfflineThreadFn);
			Util.ApplyInvariantCultureToThread(renderThread);
			renderThread.Start();
		}

		public void Quit()
		{
			if (generateThread != null && generateThread.IsAlive)
			{
				generateThread.Abort();
			}
			if (renderThread != null && renderThread.IsAlive)
			{
				renderThread.Abort();
			}
			running = false;
		}

		public bool IsGenerateComplete()
		{
			if (generateThread != null)
			{
				return !generateThread.IsAlive;
			}
			return false;
		}

		public bool IsRenderComplete()
		{
			if (renderThread != null)
			{
				return !renderThread.IsAlive;
			}
			return false;
		}

		public void GenerateOffline()
		{
			int num = 1;
			for (int i = 0; i < num; i++)
			{
				if (GenerateWorldData())
				{
					break;
				}
				DebugUtil.DevLogError("Failed worldgen");
				successCallbackFn(UI.WORLDGEN.RETRYCOUNT.key, i, WorldGenProgressStages.Stages.Failure);
			}
		}

		private void PlaceTemplateSpawners(Vector2I position, TemplateContainer template)
		{
			data.gameSpawnData.AddTemplate(template, position);
		}

		private void RenderOfflineThreadFn()
		{
			Sim.DiseaseCell[] dc = null;
			RenderOffline(doSettle: true, ref dc);
		}

		public static int GetDiseaseIdx(string disease)
		{
			for (int i = 0; i < diseaseIds.Count; i++)
			{
				if (disease == diseaseIds[i])
				{
					return i;
				}
			}
			return 255;
		}

		public Sim.Cell[] RenderOffline(bool doSettle, ref Sim.DiseaseCell[] dc)
		{
			Sim.Cell[] cells = null;
			float[] bgTemp = null;
			dc = null;
			HashSet<int> borderCells = new HashSet<int>();
			WriteOverWorldNoise(successCallbackFn);
			if (!RenderToMap(successCallbackFn, ref cells, ref bgTemp, ref dc, ref borderCells))
			{
				successCallbackFn(UI.WORLDGEN.FAILED.key, -100f, WorldGenProgressStages.Stages.Failure);
				return null;
			}
			EnsureEnoughAlgaeInStartingBiome(cells);
			List<KeyValuePair<Vector2I, TemplateContainer>> list = new List<KeyValuePair<Vector2I, TemplateContainer>>();
			foreach (TerrainCell item5 in GetTerrainCellsForTag(WorldGenTags.StartLocation))
			{
				TemplateContainer startingBaseTemplate = TemplateCache.GetStartingBaseTemplate(Settings.world.startingBaseTemplate);
				list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)item5.poly.Centroid().x, (int)item5.poly.Centroid().y), startingBaseTemplate));
			}
			List<TemplateContainer> list2 = TemplateCache.CollectBaseTemplateAssets("poi");
			foreach (WeightedName subworldFile in Settings.world.subworldFiles)
			{
				SubWorld subWorld = Settings.GetSubWorld(subworldFile.name);
				if (subWorld.pointsOfInterest == null)
				{
					continue;
				}
				foreach (KeyValuePair<string, string[]> item6 in subWorld.pointsOfInterest)
				{
					List<TerrainCell> terrainCellsForTag = GetTerrainCellsForTag(subWorld.name.ToTag());
					for (int num = terrainCellsForTag.Count - 1; num >= 0; num--)
					{
						if (!terrainCellsForTag[num].IsSafeToSpawnPOI(data.terrainCells))
						{
							terrainCellsForTag.Remove(terrainCellsForTag[num]);
						}
					}
					if (terrainCellsForTag.Count <= 0)
					{
						continue;
					}
					string template3 = null;
					TemplateContainer templateContainer = null;
					int num2 = 0;
					while (templateContainer == null && num2 < item6.Value.Length)
					{
						template3 = item6.Value[myRandom.RandomRange(0, item6.Value.Length)];
						templateContainer = list2.Find((TemplateContainer value) => value.name == template3);
						num2++;
					}
					if (templateContainer == null)
					{
						continue;
					}
					list2.Remove(templateContainer);
					for (int i = 0; i < terrainCellsForTag.Count; i++)
					{
						TerrainCell terrainCell = terrainCellsForTag[myRandom.RandomRange(0, terrainCellsForTag.Count)];
						if (!terrainCell.node.tags.Contains(WorldGenTags.POI))
						{
							if (!(templateContainer.info.size.Y > terrainCell.poly.MaxY - terrainCell.poly.MinY))
							{
								list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell.poly.Centroid().x, (int)terrainCell.poly.Centroid().y), templateContainer));
								terrainCell.node.tags.Add(template3.ToTag());
								terrainCell.node.tags.Add(WorldGenTags.POI);
								break;
							}
							float num3 = templateContainer.info.size.Y - (terrainCell.poly.MaxY - terrainCell.poly.MinY);
							float num4 = templateContainer.info.size.X - (terrainCell.poly.MaxX - terrainCell.poly.MinX);
							if (terrainCell.poly.MaxY + num3 < (float)Grid.HeightInCells && terrainCell.poly.MinY - num3 > 0f && terrainCell.poly.MaxX + num4 < (float)Grid.WidthInCells && terrainCell.poly.MinX - num4 > 0f)
							{
								list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell.poly.Centroid().x, (int)terrainCell.poly.Centroid().y), templateContainer));
								terrainCell.node.tags.Add(template3.ToTag());
								terrainCell.node.tags.Add(WorldGenTags.POI);
								break;
							}
						}
					}
				}
			}
			List<TemplateContainer> list3 = TemplateCache.CollectBaseTemplateAssets("features");
			foreach (WeightedName subworldFile2 in Settings.world.subworldFiles)
			{
				SubWorld subWorld2 = Settings.GetSubWorld(subworldFile2.name);
				if (subWorld2.featureTemplates == null || subWorld2.featureTemplates.Count <= 0)
				{
					continue;
				}
				List<string> list4 = new List<string>();
				foreach (KeyValuePair<string, int> featureTemplate in subWorld2.featureTemplates)
				{
					for (int j = 0; j < featureTemplate.Value; j++)
					{
						list4.Add(featureTemplate.Key);
					}
				}
				list4.ShuffleSeeded(myRandom.RandomSource());
				List<TerrainCell> terrainCellsForTag2 = GetTerrainCellsForTag(subWorld2.name.ToTag());
				terrainCellsForTag2.ShuffleSeeded(myRandom.RandomSource());
				foreach (TerrainCell item7 in terrainCellsForTag2)
				{
					if (list4.Count == 0)
					{
						break;
					}
					if (item7.IsSafeToSpawnFeatureTemplate())
					{
						string template2 = list4[list4.Count - 1];
						list4.RemoveAt(list4.Count - 1);
						TemplateContainer templateContainer2 = list3.Find((TemplateContainer value) => value.name == template2);
						if (templateContainer2 != null)
						{
							list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)item7.poly.Centroid().x, (int)item7.poly.Centroid().y), templateContainer2));
							item7.node.tags.Add(template2.ToTag());
							item7.node.tags.Add(WorldGenTags.POI);
						}
					}
				}
			}
			if (Settings.world.globalFeatureTemplates != null && Settings.world.globalFeatureTemplates.Count > 0)
			{
				List<TerrainCell> list5 = new List<TerrainCell>();
				foreach (TerrainCell terrainCell3 in data.terrainCells)
				{
					if (terrainCell3.IsSafeToSpawnFeatureTemplate(WorldGenTags.NoGlobalFeatureSpawning))
					{
						list5.Add(terrainCell3);
					}
				}
				list5.ShuffleSeeded(myRandom.RandomSource());
				List<string> list6 = new List<string>();
				foreach (KeyValuePair<string, int> globalFeatureTemplate in Settings.world.globalFeatureTemplates)
				{
					for (int k = 0; k < globalFeatureTemplate.Value; k++)
					{
						list6.Add(globalFeatureTemplate.Key);
					}
				}
				list6.ShuffleSeeded(myRandom.RandomSource());
				foreach (string template in list6)
				{
					if (list5.Count == 0)
					{
						break;
					}
					TerrainCell terrainCell2 = list5[list5.Count - 1];
					list5.RemoveAt(list5.Count - 1);
					TemplateContainer templateContainer3 = list3.Find((TemplateContainer value) => value.name == template);
					if (templateContainer3 != null)
					{
						list.Add(new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int)terrainCell2.poly.Centroid().x, (int)terrainCell2.poly.Centroid().y), templateContainer3));
						terrainCell2.node.tags.Add(template.ToTag());
						terrainCell2.node.tags.Add(WorldGenTags.Feature);
					}
				}
			}
			List<TerrainCell> terrainCellsForTag3 = GetTerrainCellsForTag(WorldGenTags.StartWorld);
			foreach (TerrainCell overworldCell in OverworldCells)
			{
				foreach (TerrainCell item8 in terrainCellsForTag3)
				{
					if (overworldCell.poly.PointInPolygon(item8.poly.Centroid()))
					{
						overworldCell.node.tags.Add(WorldGenTags.StartWorld);
						break;
					}
				}
			}
			foreach (int item9 in borderCells)
			{
				cells[item9].SetValues(unobtaniumElement, ElementLoader.elements);
			}
			if (doSettle)
			{
				running = WorldGenSimUtil.DoSettleSim(Settings, cells, bgTemp, dc, successCallbackFn, data, list, errorCallback, delegate(Sim.Cell[] updatedCells, float[] updatedBGTemp, Sim.DiseaseCell[] updatedDisease)
				{
					SpawnMobsAndTemplates(updatedCells, updatedBGTemp, updatedDisease, borderCells);
				});
			}
			foreach (KeyValuePair<Vector2I, TemplateContainer> item10 in list)
			{
				PlaceTemplateSpawners(item10.Key, item10.Value);
			}
			for (int num5 = data.gameSpawnData.buildings.Count - 1; num5 >= 0; num5--)
			{
				int item = Grid.XYToCell(data.gameSpawnData.buildings[num5].location_x, data.gameSpawnData.buildings[num5].location_y);
				if (borderCells.Contains(item))
				{
					data.gameSpawnData.buildings.RemoveAt(num5);
				}
			}
			for (int num6 = data.gameSpawnData.elementalOres.Count - 1; num6 >= 0; num6--)
			{
				int item2 = Grid.XYToCell(data.gameSpawnData.elementalOres[num6].location_x, data.gameSpawnData.elementalOres[num6].location_y);
				if (borderCells.Contains(item2))
				{
					data.gameSpawnData.elementalOres.RemoveAt(num6);
				}
			}
			for (int num7 = data.gameSpawnData.otherEntities.Count - 1; num7 >= 0; num7--)
			{
				int item3 = Grid.XYToCell(data.gameSpawnData.otherEntities[num7].location_x, data.gameSpawnData.otherEntities[num7].location_y);
				if (borderCells.Contains(item3))
				{
					data.gameSpawnData.otherEntities.RemoveAt(num7);
				}
			}
			for (int num8 = data.gameSpawnData.pickupables.Count - 1; num8 >= 0; num8--)
			{
				int item4 = Grid.XYToCell(data.gameSpawnData.pickupables[num8].location_x, data.gameSpawnData.pickupables[num8].location_y);
				if (borderCells.Contains(item4))
				{
					data.gameSpawnData.pickupables.RemoveAt(num8);
				}
			}
			SaveWorldGen();
			successCallbackFn(UI.WORLDGEN.COMPLETE.key, 101f, WorldGenProgressStages.Stages.Complete);
			running = false;
			return cells;
		}

		private void SpawnMobsAndTemplates(Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> borderCells)
		{
			MobSpawning.DetectNaturalCavities(TerrainCells, successCallbackFn);
			SeededRandom rnd = new SeededRandom(data.globalTerrainSeed);
			for (int i = 0; i < TerrainCells.Count; i++)
			{
				float completePercent = (float)i / (float)TerrainCells.Count * 100f;
				successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, completePercent, WorldGenProgressStages.Stages.PlacingCreatures);
				TerrainCell tc = TerrainCells[i];
				Dictionary<int, string> dictionary = MobSpawning.PlaceFeatureAmbientMobs(Settings, tc, rnd, cells, bgTemp, dc, borderCells, isRunningDebugGen);
				if (dictionary != null)
				{
					data.gameSpawnData.AddRange(dictionary);
				}
				dictionary = MobSpawning.PlaceBiomeAmbientMobs(Settings, tc, rnd, cells, bgTemp, dc, borderCells, isRunningDebugGen);
				if (dictionary != null)
				{
					data.gameSpawnData.AddRange(dictionary);
				}
			}
			successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, 100f, WorldGenProgressStages.Stages.PlacingCreatures);
		}

		private void ReportWorldGenError(Exception e)
		{
			string settingsCoordinate = CustomGameSettings.Instance.GetSettingsCoordinate();
			errorCallback(new OfflineWorldGen.ErrorInfo
			{
				errorDesc = string.Format(UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FAILURE, settingsCoordinate),
				exception = e
			});
			KCrashReporter.ReportErrorDevNotification("WorldgenFailure", e.StackTrace, settingsCoordinate);
		}

		public void SetWorldSize(int width, int height)
		{
			data.world = new Chunk(0, 0, width, height);
		}

		public bool GenerateNoiseData(OfflineCallbackFunction updateProgressFn)
		{
			stats["GenerateNoiseTime"] = System.DateTime.Now.Ticks;
			try
			{
				running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
				if (!running)
				{
					stats["GenerateNoiseTime"] = 0;
					return false;
				}
				SetupNoise(updateProgressFn);
				running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 100f, WorldGenProgressStages.Stages.SetupNoise);
				if (!running)
				{
					stats["GenerateNoiseTime"] = 0;
					return false;
				}
				GenerateUnChunkedNoise(updateProgressFn);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				ReportWorldGenError(ex);
				WorldGenLogger.LogException(message, stackTrace);
				running = successCallbackFn(new StringKey("Exception in GenerateNoiseData"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			stats["GenerateNoiseTime"] = System.DateTime.Now.Ticks - (long)stats["GenerateNoiseTime"];
			return true;
		}

		public bool GenerateLayout(OfflineCallbackFunction updateProgressFn)
		{
			stats["GenerateLayoutTime"] = System.DateTime.Now.Ticks;
			try
			{
				running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 0f, WorldGenProgressStages.Stages.WorldLayout);
				if (!running)
				{
					return false;
				}
				Debug.Assert(data.world.size.x != 0 && data.world.size.y != 0, "Map size has not been set");
				data.worldLayout = new WorldLayout(this, data.world.size.x, data.world.size.y, data.globalWorldLayoutSeed);
				running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 5f, WorldGenProgressStages.Stages.WorldLayout);
				data.voronoiTree = null;
				try
				{
					data.voronoiTree = WorldLayout.GenerateOverworld(Settings.world.layoutMethod == ProcGen.World.LayoutMethod.PowerTree);
					WorldLayout.PopulateSubworlds();
					CompleteLayout(updateProgressFn);
				}
				catch (Exception ex)
				{
					string message = ex.Message;
					string stackTrace = ex.StackTrace;
					WorldGenLogger.LogException(message, stackTrace);
					ReportWorldGenError(ex);
					running = updateProgressFn(new StringKey("Exception in InitVoronoiTree"), -1f, WorldGenProgressStages.Stages.Failure);
					return false;
				}
				data.overworldCells = new List<TerrainCell>(40);
				for (int i = 0; i < data.voronoiTree.ChildCount(); i++)
				{
					VoronoiTree.Tree tree = data.voronoiTree.GetChild(i) as VoronoiTree.Tree;
					ProcGen.Node node = data.worldLayout.overworldGraph.FindNodeByID(tree.site.id);
					data.overworldCells.Add(new TerrainCellLogged(node, tree.site));
				}
				running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 100f, WorldGenProgressStages.Stages.WorldLayout);
			}
			catch (Exception ex2)
			{
				string message2 = ex2.Message;
				string stackTrace2 = ex2.StackTrace;
				WorldGenLogger.LogException(message2, stackTrace2);
				ReportWorldGenError(ex2);
				successCallbackFn(new StringKey("Exception in GenerateLayout"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			stats["GenerateLayoutTime"] = System.DateTime.Now.Ticks - (long)stats["GenerateLayoutTime"];
			return true;
		}

		public bool CompleteLayout(OfflineCallbackFunction updateProgressFn)
		{
			long ticks = System.DateTime.Now.Ticks;
			try
			{
				running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!running)
				{
					return false;
				}
				data.terrainCells = null;
				running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 65f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!running)
				{
					return false;
				}
				running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 75f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!running)
				{
					return false;
				}
				data.terrainCells = new List<TerrainCell>(4000);
				List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
				data.voronoiTree.ForceLowestToLeaf();
				ApplyStartNode();
				data.voronoiTree.VisitAll(UpdateVoronoiNodeTags);
				data.voronoiTree.GetLeafNodes(list);
				for (int i = 0; i < list.Count; i++)
				{
					VoronoiTree.Node node = list[i];
					ProcGen.Node tn = data.worldLayout.localGraph.FindNodeByID(node.site.id);
					if (tn != null)
					{
						TerrainCell terrainCell = data.terrainCells.Find((TerrainCell c) => c.node == tn);
						if (terrainCell == null)
						{
							TerrainCell item = new TerrainCellLogged(tn, node.site);
							data.terrainCells.Add(item);
						}
						else
						{
							Debug.LogWarning("Duplicate cell found" + terrainCell.node.node.Id);
						}
					}
				}
				for (int j = 0; j < data.terrainCells.Count; j++)
				{
					TerrainCell terrainCell2 = data.terrainCells[j];
					foreach (KeyValuePair<uint, int> neighbour in terrainCell2.site.neighbours)
					{
						for (int k = 0; k < data.terrainCells.Count; k++)
						{
							if (data.terrainCells[k].site.id == neighbour.Key)
							{
								terrainCell2.terrain_neighbors_idx.Add(neighbour.Key);
							}
						}
					}
				}
				running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 100f, WorldGenProgressStages.Stages.CompleteLayout);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				successCallbackFn(new StringKey("Exception in CompleteLayout"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			ticks = System.DateTime.Now.Ticks - ticks;
			stats["GenerateLayoutTime"] = (long)stats["GenerateLayoutTime"] + ticks;
			return true;
		}

		public void UpdateVoronoiNodeTags(VoronoiTree.Node node)
		{
			ProcGen.Node node2 = null;
			((!node.tags.Contains(WorldGenTags.Overworld)) ? WorldLayout.localGraph.FindNodeByID(node.site.id) : WorldLayout.overworldGraph.FindNodeByID(node.site.id))?.tags.Union(node.tags);
		}

		public bool GenerateWorldData()
		{
			stats["GenerateDataTime"] = System.DateTime.Now.Ticks;
			Vector2I worldsize = Settings.world.worldsize;
			SetWorldSize(worldsize.x, worldsize.y);
			if (generateNoiseData && !GenerateNoiseData(successCallbackFn))
			{
				return false;
			}
			if (!GenerateLayout(successCallbackFn))
			{
				return false;
			}
			stats["GenerateDataTime"] = System.DateTime.Now.Ticks - (long)stats["GenerateDataTime"];
			return true;
		}

		public void EnsureEnoughAlgaeInStartingBiome(Sim.Cell[] cells)
		{
			List<TerrainCell> terrainCellsForTag = GetTerrainCellsForTag(WorldGenTags.StartWorld);
			float num = 8200f;
			float num2 = 0f;
			int num3 = 0;
			foreach (TerrainCell item in terrainCellsForTag)
			{
				foreach (int allCell in item.GetAllCells())
				{
					if (ElementLoader.GetElementIndex(SimHashes.Algae) == cells[allCell].elementIdx)
					{
						num3++;
						num2 += cells[allCell].mass;
					}
				}
			}
			if (!(num2 < num))
			{
				return;
			}
			float num4 = (num - num2) / (float)num3;
			foreach (TerrainCell item2 in terrainCellsForTag)
			{
				foreach (int allCell2 in item2.GetAllCells())
				{
					if (ElementLoader.GetElementIndex(SimHashes.Algae) == cells[allCell2].elementIdx)
					{
						cells[allCell2].mass += num4;
					}
				}
			}
		}

		public bool RenderToMap(OfflineCallbackFunction updateProgressFn, ref Sim.Cell[] cells, ref float[] bgTemp, ref Sim.DiseaseCell[] dcs, ref HashSet<int> borderCells)
		{
			Debug.Assert(Grid.WidthInCells == Settings.world.worldsize.x);
			Debug.Assert(Grid.HeightInCells == Settings.world.worldsize.y);
			Debug.Assert(Grid.CellCount == Grid.WidthInCells * Grid.HeightInCells);
			Debug.Assert(Grid.CellSizeInMeters != 0f);
			borderCells = new HashSet<int>();
			cells = new Sim.Cell[Grid.CellCount];
			bgTemp = new float[Grid.CellCount];
			dcs = new Sim.DiseaseCell[Grid.CellCount];
			running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 0f, WorldGenProgressStages.Stages.ClearingLevel);
			if (!running)
			{
				return false;
			}
			for (int i = 0; i < cells.Length; i++)
			{
				cells[i].SetValues(katairiteElement, ElementLoader.elements);
				bgTemp[i] = -1f;
				dcs[i] = default(Sim.DiseaseCell);
				dcs[i].diseaseIdx = byte.MaxValue;
				running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 100f * ((float)i / (float)Grid.CellCount), WorldGenProgressStages.Stages.ClearingLevel);
				if (!running)
				{
					return false;
				}
			}
			updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 100f, WorldGenProgressStages.Stages.ClearingLevel);
			try
			{
				ProcessByTerrainCell(cells, bgTemp, dcs, updateProgressFn);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				WorldGenLogger.LogException(message, stackTrace);
				running = updateProgressFn(new StringKey("Exception in ProcessByTerrainCell"), -1f, WorldGenProgressStages.Stages.Failure);
				return false;
			}
			if (Settings.GetBoolSetting("DrawWorldBorder"))
			{
				SeededRandom rnd = new SeededRandom(0);
				DrawWorldBorder(cells, data.world, rnd, borderCells, updateProgressFn);
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 100f, WorldGenProgressStages.Stages.DrawWorldBorder);
			}
			data.gameSpawnData.baseStartPos = data.worldLayout.GetStartLocation();
			return true;
		}

		public SubWorld GetSubWorldForNode(VoronoiTree.Tree node)
		{
			ProcGen.Node node2 = WorldLayout.overworldGraph.FindNodeByID(node.site.id);
			if (node2 == null)
			{
				return null;
			}
			if (!Settings.HasSubworld(node2.type))
			{
				return null;
			}
			return Settings.GetSubWorld(node2.type);
		}

		public VoronoiTree.Tree GetOverworldForNode(Leaf leaf)
		{
			if (leaf == null)
			{
				return null;
			}
			return data.worldLayout.GetVoronoiTree().GetChildContainingLeaf(leaf);
		}

		public Leaf GetLeafForTerrainCell(TerrainCell cell)
		{
			if (cell == null)
			{
				return null;
			}
			return data.worldLayout.GetVoronoiTree().GetNodeForSite(cell.site) as Leaf;
		}

		public List<TerrainCell> GetTerrainCellsForTag(Tag tag)
		{
			List<TerrainCell> list = new List<TerrainCell>();
			List<VoronoiTree.Node> leafNodesWithTag = WorldLayout.GetLeafNodesWithTag(tag);
			for (int i = 0; i < leafNodesWithTag.Count; i++)
			{
				VoronoiTree.Node node = leafNodesWithTag[i];
				TerrainCell terrainCell = data.terrainCells.Find((TerrainCell cell) => cell.site.id == node.site.id);
				if (terrainCell != null)
				{
					list.Add(terrainCell);
				}
			}
			return list;
		}

		private void GetStartCells(out int baseX, out int baseY)
		{
			Vector2I vector2I = new Vector2I(data.world.size.x / 2, (int)((float)data.world.size.y * 0.7f));
			if (data.worldLayout != null)
			{
				vector2I = data.worldLayout.GetStartLocation();
			}
			baseX = vector2I.x;
			baseY = vector2I.y;
		}

		public void ChooseBaseLocation(VoronoiTree.Node startNode)
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(WorldGenTags.StartLocation);
			List<VoronoiTree.Node> startNodes = WorldLayout.GetStartNodes();
			for (int i = 0; i < startNodes.Count; i++)
			{
				if (startNodes[i] != startNode)
				{
					startNodes[i].tags.Remove(tagSet);
				}
			}
		}

		private void SwitchNodes(VoronoiTree.Node n1, VoronoiTree.Node n2)
		{
			if (n1 is VoronoiTree.Tree || n2 is VoronoiTree.Tree)
			{
				Debug.Log("WorldGen::SwitchNodes() Skipping tree node");
				return;
			}
			ProcGen.Node node = data.worldLayout.localGraph.FindNodeByID(n1.site.id);
			ProcGen.Node node2 = data.worldLayout.localGraph.FindNodeByID(n2.site.id);
			Diagram.Site site = n1.site;
			n1.site = n2.site;
			n2.site = site;
			string type = node.type;
			node.SetType(node2.type);
			node2.SetType(type);
		}

		public void ApplyStartNode()
		{
			if (Settings.world.noStart)
			{
				return;
			}
			VoronoiTree.Node node2 = data.worldLayout.GetLeafNodesWithTag(WorldGenTags.StartLocation)[0];
			VoronoiTree.Tree parent = node2.parent;
			node2.parent.AddTagToChildren(WorldGenTags.IgnoreCaveOverride);
			node2.parent.tags.Remove(WorldGenTags.StartLocation);
			List<VoronoiTree.Node> siblings = node2.GetSiblings();
			List<VoronoiTree.Node> neighbors = node2.GetNeighbors();
			List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
			for (int i = 0; i < parent.ChildCount(); i++)
			{
				VoronoiTree.Node child = parent.GetChild(i);
				if (child != node2)
				{
					list.Add(child);
				}
			}
			siblings.RemoveAll((VoronoiTree.Node node) => neighbors.Contains(node));
			if (list.Count <= 0)
			{
				return;
			}
			list.ShuffleSeeded(myRandom.RandomSource());
			List<VoronoiTree.Node> list2 = new List<VoronoiTree.Node>();
			List<VoronoiTree.Node> list3 = new List<VoronoiTree.Node>();
			for (int j = 0; j < list.Count; j++)
			{
				VoronoiTree.Node node3 = list[j];
				bool flag = !node3.tags.Contains(WorldGenTags.Wet);
				bool flag2 = node3.site.poly.Centroid().y > node2.site.poly.Centroid().y;
				if (!flag && flag2)
				{
					if (list3.Count > 0)
					{
						SwitchNodes(node3, list3[0]);
						list3.RemoveAt(0);
					}
					else
					{
						list2.Add(node3);
					}
				}
				else if (flag && !flag2)
				{
					if (list2.Count > 0)
					{
						SwitchNodes(node3, list2[0]);
						list2.RemoveAt(0);
					}
					else
					{
						list3.Add(node3);
					}
				}
			}
			if (list3.Count <= 0)
			{
				return;
			}
			for (int k = 0; k < list2.Count; k++)
			{
				if (list3.Count <= 0)
				{
					break;
				}
				SwitchNodes(list2[k], list3[0]);
				list3.RemoveAt(0);
			}
		}

		public void GetElementForBiomePoint(Chunk chunk, ElementBandConfiguration elementBands, Vector2I pos, out Element element, out Sim.PhysicsData pd, out Sim.DiseaseCell dc, float erode)
		{
			TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(voidElement.tag.ToString(), null);
			elementOverride = GetElementFromBiomeElementTable(chunk, pos, elementBands, erode);
			element = elementOverride.element;
			pd = elementOverride.pdelement;
			dc = elementOverride.dc;
		}

		private bool ConvertTerrainCellsToEdges(OfflineCallbackFunction updateProgress)
		{
			for (int i = 0; i < data.overworldCells.Count; i++)
			{
				running = updateProgress(UI.WORLDGEN.CONVERTTERRAINCELLSTOEDGES.key, (float)i / (float)data.overworldCells.Count, WorldGenProgressStages.Stages.ConvertCellsToEdges);
				if (!running)
				{
					return running;
				}
				List<Vector2> vertices = data.overworldCells[i].poly.Vertices;
				for (int j = 0; j < vertices.Count; j++)
				{
					if (j < vertices.Count - 1)
					{
						ConvertIntersectingCellsToType(new MathUtil.Pair<Vector2, Vector2>(vertices[j], vertices[j + 1]), "EDGE");
					}
					else
					{
						ConvertIntersectingCellsToType(new MathUtil.Pair<Vector2, Vector2>(vertices[j], vertices[0]), (vertices.Count > 4) ? "UNPASSABLE" : "EDGE");
					}
				}
			}
			return true;
		}

		public void ConvertIntersectingCellsToType(MathUtil.Pair<Vector2, Vector2> segment, string type)
		{
			List<Vector2I> line = ProcGen.Util.GetLine(segment.First, segment.Second);
			for (int i = 0; i < data.terrainCells.Count; i++)
			{
				if (!(data.terrainCells[i].node.type != type))
				{
					continue;
				}
				for (int j = 0; j < line.Count; j++)
				{
					if (data.terrainCells[i].poly.Contains(line[j]))
					{
						data.terrainCells[i].node.SetType(type);
					}
				}
			}
		}

		public string GetSubWorldType(Vector2I pos)
		{
			for (int i = 0; i < data.overworldCells.Count; i++)
			{
				if (data.overworldCells[i].poly.Contains(pos))
				{
					return data.overworldCells[i].node.type;
				}
			}
			return null;
		}

		private List<Polygon> GetOverworldPolygons()
		{
			List<Polygon> list = new List<Polygon>();
			for (int i = 0; i < data.overworldCells.Count; i++)
			{
				list.Add(data.overworldCells[i].poly);
			}
			return list;
		}

		private List<Border> GetBorders(List<TerrainCell> cells)
		{
			List<Border> result = new List<Border>();
			HashSet<TerrainCell> hashSet = new HashSet<TerrainCell>();
			for (int i = 0; i < cells.Count; i++)
			{
				TerrainCell terrainCell = cells[i];
				hashSet.Add(terrainCell);
				HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = terrainCell.site.neighbours.GetEnumerator();
				int num = 0;
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, int> neighborId = enumerator.Current;
					TerrainCell terrainCell2 = cells.Find((TerrainCell n) => n.site.id == neighborId.Key);
					if (terrainCell2 != null)
					{
						hashSet.Contains(terrainCell2);
					}
					num++;
				}
			}
			return result;
		}

		private void ProcessByTerrainCell(Sim.Cell[] map_cells, float[] bgTemp, Sim.DiseaseCell[] dcs, OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.PROCESSING.key, 0f, WorldGenProgressStages.Stages.Processing);
			SeededRandom seededRandom = new SeededRandom(data.globalTerrainSeed);
			try
			{
				for (int i = 0; i < data.terrainCells.Count; i++)
				{
					updateProgressFn(UI.WORLDGEN.PROCESSING.key, 100f * ((float)i / (float)data.terrainCells.Count), WorldGenProgressStages.Stages.Processing);
					data.terrainCells[i].Process(this, map_cells, bgTemp, dcs, data.world, seededRandom);
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				updateProgressFn(new StringKey("Exception in TerrainCell.Process"), -1f, WorldGenProgressStages.Stages.Failure);
				Debug.LogError("Error:" + message + "\n" + stackTrace);
			}
			List<Border> list = new List<Border>();
			updateProgressFn(UI.WORLDGEN.BORDERS.key, 0f, WorldGenProgressStages.Stages.Borders);
			try
			{
				List<Edge> edgesWithTag = data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeUnpassable);
				for (int j = 0; j < edgesWithTag.Count; j++)
				{
					Edge edge2 = edgesWithTag[j];
					if (edge2.site0 != edge2.site1)
					{
						TerrainCell terrainCell = data.overworldCells.Find((TerrainCell c) => c.node.node == edge2.site0.node);
						TerrainCell terrainCell2 = data.overworldCells.Find((TerrainCell c) => c.node.node == edge2.site1.node);
						Debug.Assert(terrainCell != null && terrainCell2 != null, "NULL Terrainell nodes with EdgeUnpassable");
						Border border = new Border(new Neighbors(terrainCell, terrainCell2), edge2.corner0.position, edge2.corner1.position);
						border.element = SettingsCache.borders["impenetrable"];
						border.width = seededRandom.RandomRange(2, 3);
						list.Add(border);
					}
				}
				List<Edge> edgesWithTag2 = data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeClosed);
				for (int k = 0; k < edgesWithTag2.Count; k++)
				{
					Edge edge = edgesWithTag2[k];
					if (edge.site0 != edge.site1 && !edgesWithTag.Contains(edge))
					{
						TerrainCell terrainCell3 = data.overworldCells.Find((TerrainCell c) => c.node.node == edge.site0.node);
						TerrainCell terrainCell4 = data.overworldCells.Find((TerrainCell c) => c.node.node == edge.site1.node);
						Debug.Assert(terrainCell3 != null && terrainCell4 != null, "NULL Terraincell nodes with EdgeClosed");
						string borderOverride = Settings.GetSubWorld(terrainCell3.node.type).borderOverride;
						string borderOverride2 = Settings.GetSubWorld(terrainCell4.node.type).borderOverride;
						string key = ((!string.IsNullOrEmpty(borderOverride2) && !string.IsNullOrEmpty(borderOverride)) ? ((seededRandom.RandomValue() > 0.5f) ? borderOverride2 : borderOverride) : ((!string.IsNullOrEmpty(borderOverride2) || !string.IsNullOrEmpty(borderOverride)) ? ((!string.IsNullOrEmpty(borderOverride2)) ? borderOverride2 : borderOverride) : "hardToDig"));
						Border border2 = new Border(new Neighbors(terrainCell3, terrainCell4), edge.corner0.position, edge.corner1.position);
						border2.element = SettingsCache.borders[key];
						border2.width = seededRandom.RandomRange(1f, 2.5f);
						list.Add(border2);
					}
				}
			}
			catch (Exception ex2)
			{
				string message2 = ex2.Message;
				string stackTrace2 = ex2.StackTrace;
				updateProgressFn(new StringKey("Exception in Border creation"), -1f, WorldGenProgressStages.Stages.Failure);
				Debug.LogError("Error:" + message2 + " " + stackTrace2);
			}
			try
			{
				if (data.world.defaultTemp == null)
				{
					data.world.defaultTemp = new float[data.world.density.Length];
				}
				for (int l = 0; l < data.world.defaultTemp.Length; l++)
				{
					data.world.defaultTemp[l] = bgTemp[l];
				}
			}
			catch (Exception ex3)
			{
				string message3 = ex3.Message;
				string stackTrace3 = ex3.StackTrace;
				updateProgressFn(new StringKey("Exception in border.defaultTemp"), -1f, WorldGenProgressStages.Stages.Failure);
				Debug.LogError("Error:" + message3 + " " + stackTrace3);
			}
			try
			{
				TerrainCell.SetValuesFunction setValues = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
				{
					if (Grid.IsValidCell(index))
					{
						if (!TerrainCell.GetHighPriorityClaimCells().Contains(index))
						{
							if ((elem as Element).HasTag(GameTags.Special))
							{
								pd = (elem as Element).defaultValues;
							}
							map_cells[index].SetValues(elem as Element, pd, ElementLoader.elements);
							dcs[index] = dc;
						}
					}
					else
					{
						Debug.LogError("Process::SetValuesFunction Index [" + index + "] is not valid. cells.Length [" + map_cells.Length + "]");
					}
				};
				for (int m = 0; m < list.Count; m++)
				{
					Border border3 = list[m];
					SubWorld subWorld = Settings.GetSubWorld(border3.neighbors.n0.node.type);
					SubWorld subWorld2 = Settings.GetSubWorld(border3.neighbors.n1.node.type);
					float num = Mathf.Min(SettingsCache.temperatures[subWorld.temperatureRange].min, SettingsCache.temperatures[subWorld2.temperatureRange].min);
					float temperatureRange = Mathf.Max(SettingsCache.temperatures[subWorld.temperatureRange].max, SettingsCache.temperatures[subWorld2.temperatureRange].max) - num;
					border3.Stagger(seededRandom, seededRandom.RandomRange(8, 13), seededRandom.RandomRange(2, 5));
					border3.ConvertToMap(data.world, setValues, num, temperatureRange, seededRandom);
				}
			}
			catch (Exception ex4)
			{
				string message4 = ex4.Message;
				string stackTrace4 = ex4.StackTrace;
				updateProgressFn(new StringKey("Exception in border.ConvertToMap"), -1f, WorldGenProgressStages.Stages.Failure);
				Debug.LogError("Error:" + message4 + " " + stackTrace4);
			}
		}

		private void DrawBorder(Chunk chunk, int thickness, int range, SeededRandom rnd)
		{
			if (Mathf.Abs(chunk.offset.x % SubWorldSize.x) == 0)
			{
				int num = 0;
				for (int num2 = ChunkEdgeSize - 1; num2 >= 0; num2--)
				{
					num = Mathf.Max(-range, Mathf.Min(num + rnd.RandomRange(-2, 2), range));
					for (int i = 0; i < thickness + num; i++)
					{
						chunk.overrides[i + ChunkEdgeSize * num2] = 100f;
					}
				}
			}
			if (Mathf.Abs((chunk.offset.x + ChunkEdgeSize) % SubWorldSize.x) == 0)
			{
				int num3 = 0;
				for (int num4 = ChunkEdgeSize - 1; num4 >= 0; num4--)
				{
					num3 = Mathf.Max(-range, Mathf.Min(num3 + rnd.RandomRange(-2, 2), range));
					for (int j = 0; j < thickness + num3; j++)
					{
						chunk.overrides[ChunkEdgeSize - 1 - j + ChunkEdgeSize * num4] = 100f;
					}
				}
			}
			if (Mathf.Abs(chunk.offset.y % SubWorldSize.y) == 0)
			{
				int num5 = 0;
				for (int k = 0; k < ChunkEdgeSize; k++)
				{
					num5 = Mathf.Max(-range, Mathf.Min(num5 + rnd.RandomRange(-2, 2), range));
					for (int l = 0; l < thickness + num5; l++)
					{
						chunk.overrides[k + ChunkEdgeSize * l] = 100f;
					}
				}
			}
			if (Mathf.Abs((chunk.offset.y + ChunkEdgeSize) % SubWorldSize.y) != 0)
			{
				return;
			}
			int num6 = 0;
			for (int m = 0; m < ChunkEdgeSize; m++)
			{
				num6 = Mathf.Max(-range, Mathf.Min(num6 + rnd.RandomRange(-2, 2), range));
				for (int n = 0; n < thickness + num6; n++)
				{
					chunk.overrides[m + ChunkEdgeSize * (ChunkEdgeSize - 1 - n)] = 100f;
				}
			}
		}

		private void DrawWorldBorder(Sim.Cell[] cells, Chunk world, SeededRandom rnd, HashSet<int> borderCells, OfflineCallbackFunction updateProgressFn)
		{
			bool boolSetting = Settings.GetBoolSetting("DrawWorldBorderTop");
			int intSetting = Settings.GetIntSetting("WorldBorderThickness");
			int intSetting2 = Settings.GetIntSetting("WorldBorderRange");
			byte new_elem_idx = (byte)ElementLoader.elements.IndexOf(unobtaniumElement);
			float temperature = unobtaniumElement.defaultValues.temperature;
			float mass = unobtaniumElement.defaultValues.mass;
			int num = 0;
			int num2 = 0;
			updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 0f, WorldGenProgressStages.Stages.DrawWorldBorder);
			int num3 = world.size.y - 1;
			if (!boolSetting)
			{
				num3 = Math.Max(0, num3 - intSetting - 2 * intSetting2);
				num = -intSetting2;
				num2 = -intSetting2;
				num3 = world.size.y - 32;
			}
			for (int num4 = num3; num4 >= 0; num4--)
			{
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, (float)num4 / (float)num3 * 0.33f, WorldGenProgressStages.Stages.DrawWorldBorder);
				num = Mathf.Max(-intSetting2, Mathf.Min(num + rnd.RandomRange(-2, 2), intSetting2));
				for (int i = 0; i < intSetting + num; i++)
				{
					int num5 = Grid.XYToCell(i, num4);
					borderCells.Add(num5);
					cells[num5].SetValues(new_elem_idx, temperature, mass);
				}
				num2 = Mathf.Max(-intSetting2, Mathf.Min(num2 + rnd.RandomRange(-2, 2), intSetting2));
				for (int j = 0; j < intSetting + num2; j++)
				{
					int num6 = Grid.XYToCell(world.size.x - 1 - j, num4);
					borderCells.Add(num6);
					cells[num6].SetValues(new_elem_idx, temperature, mass);
				}
			}
			int num7 = 0;
			int num8 = 0;
			for (int k = 0; k < world.size.x; k++)
			{
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, (float)k / (float)world.size.x * 0.66f + 0.33f, WorldGenProgressStages.Stages.DrawWorldBorder);
				num7 = Mathf.Max(-intSetting2, Mathf.Min(num7 + rnd.RandomRange(-2, 2), intSetting2));
				for (int l = 0; l < intSetting + num7; l++)
				{
					int num9 = Grid.XYToCell(k, l);
					borderCells.Add(num9);
					cells[num9].SetValues(new_elem_idx, temperature, mass);
				}
				if (boolSetting)
				{
					num8 = Mathf.Max(-intSetting2, Mathf.Min(num8 + rnd.RandomRange(-2, 2), intSetting2));
					for (int m = 0; m < intSetting + num8; m++)
					{
						int num10 = Grid.XYToCell(k, world.size.y - 1 - m);
						borderCells.Add(num10);
						cells[num10].SetValues(new_elem_idx, temperature, mass);
					}
				}
			}
		}

		private void SetupNoise(OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
			heatSource = BuildNoiseSource(data.world.size.x, data.world.size.y, "noise/Heat");
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 100f, WorldGenProgressStages.Stages.SetupNoise);
		}

		public NoiseMapBuilderPlane BuildNoiseSource(int width, int height, string name)
		{
			ProcGen.Noise.Tree tree = SettingsCache.noise.GetTree(name, SettingsCache.GetPath());
			Debug.Assert(tree != null, name);
			return BuildNoiseSource(width, height, tree);
		}

		public NoiseMapBuilderPlane BuildNoiseSource(int width, int height, ProcGen.Noise.Tree tree)
		{
			Vector2f lowerBound = tree.settings.lowerBound;
			Vector2f upperBound = tree.settings.upperBound;
			Debug.Assert(lowerBound.x < upperBound.x, "BuildNoiseSource X range broken [l: " + lowerBound.x + " h: " + upperBound.x + "]");
			Debug.Assert(lowerBound.y < upperBound.y, "BuildNoiseSource Y range broken [l: " + lowerBound.y + " h: " + upperBound.y + "]");
			Debug.Assert(width > 0, "BuildNoiseSource width <=0: [" + width + "]");
			Debug.Assert(height > 0, "BuildNoiseSource height <=0: [" + height + "]");
			NoiseMapBuilderPlane noiseMapBuilderPlane = new NoiseMapBuilderPlane(lowerBound.x, upperBound.x, lowerBound.y, upperBound.y, seamless: false);
			noiseMapBuilderPlane.SetSize(width, height);
			noiseMapBuilderPlane.SourceModule = tree.BuildFinalModule(data.globalNoiseSeed);
			return noiseMapBuilderPlane;
		}

		private void GetMinMaxDataValues(float[] data, int width, int height)
		{
		}

		public static NoiseMap BuildNoiseMap(Vector2 offset, float zoom, NoiseMapBuilderPlane nmbp, int width, int height, NoiseMapBuilderCallback cb = null)
		{
			double num = offset.x;
			double num2 = offset.y;
			if (zoom == 0f)
			{
				zoom = 0.01f;
			}
			double num3 = num * (double)zoom;
			double num4 = (num + (double)width) * (double)zoom;
			double num5 = num2 * (double)zoom;
			double num6 = (num2 + (double)height) * (double)zoom;
			NoiseMap result = (NoiseMap)(nmbp.NoiseMap = new NoiseMap(width, height));
			nmbp.SetBounds((float)num3, (float)num4, (float)num5, (float)num6);
			nmbp.CallBack = cb;
			nmbp.Build();
			return result;
		}

		public static float[] GenerateNoise(Vector2 offset, float zoom, NoiseMapBuilderPlane nmbp, int width, int height, NoiseMapBuilderCallback cb = null)
		{
			NoiseMap noiseMap = BuildNoiseMap(offset, zoom, nmbp, width, height, cb);
			float[] buffer = new float[noiseMap.Width * noiseMap.Height];
			noiseMap.CopyTo(ref buffer);
			return buffer;
		}

		public static void Normalise(float[] data)
		{
			Debug.Assert(data != null && data.Length != 0, "MISSING DATA FOR NORMALIZE");
			float num = float.MaxValue;
			float num2 = float.MinValue;
			for (int i = 0; i < data.Length; i++)
			{
				num = Mathf.Min(data[i], num);
				num2 = Mathf.Max(data[i], num2);
			}
			float num3 = num2 - num;
			for (int j = 0; j < data.Length; j++)
			{
				data[j] = (data[j] - num) / num3;
			}
		}

		private void GenerateUnChunkedNoise(OfflineCallbackFunction updateProgressFn)
		{
			Vector2 offset = new Vector2(0f, 0f);
			updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, 0f, WorldGenProgressStages.Stages.GenerateNoise);
			NoiseMapBuilderCallback noiseMapBuilderCallback = delegate(int line)
			{
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (int)(0.0 + 25.0 * (double)((float)line / (float)data.world.size.y)), WorldGenProgressStages.Stages.GenerateNoise);
			};
			noiseMapBuilderCallback = delegate(int line)
			{
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (int)(25.0 + 25.0 * (double)((float)line / (float)data.world.size.y)), WorldGenProgressStages.Stages.GenerateNoise);
			};
			if (noiseMapBuilderCallback == null)
			{
				Debug.LogError("nupd is null");
			}
			data.world.heatOffset = GenerateNoise(offset, SettingsCache.noise.GetZoomForTree("noise/Heat"), heatSource, data.world.size.x, data.world.size.y, noiseMapBuilderCallback);
			data.world.data = new float[data.world.heatOffset.Length];
			data.world.density = new float[data.world.heatOffset.Length];
			data.world.overrides = new float[data.world.heatOffset.Length];
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 50f, WorldGenProgressStages.Stages.GenerateNoise);
			if (SettingsCache.noise.ShouldNormaliseTree("noise/Heat"))
			{
				Normalise(data.world.heatOffset);
			}
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 100f, WorldGenProgressStages.Stages.GenerateNoise);
		}

		public void WriteOverWorldNoise(OfflineCallbackFunction updateProgressFn)
		{
			float num = OverworldCells.Count;
			float perCell = 100f / num;
			float currentProgress = 0f;
			foreach (TerrainCell overworldCell in OverworldCells)
			{
				ProcGen.Noise.Tree tree = SettingsCache.noise.GetTree("noise/Default", SettingsCache.GetPath());
				ProcGen.Noise.Tree tree2 = SettingsCache.noise.GetTree("noise/DefaultCave", SettingsCache.GetPath());
				ProcGen.Noise.Tree tree3 = SettingsCache.noise.GetTree("noise/DefaultDensity", SettingsCache.GetPath());
				SubWorld subWorld = Settings.GetSubWorld(overworldCell.node.type);
				if (subWorld == null)
				{
					Debug.Log("Couldnt find Subworld for overworld node [" + overworldCell.node.type + "] using defaults");
				}
				else
				{
					if (subWorld.biomeNoise != null)
					{
						ProcGen.Noise.Tree tree4 = SettingsCache.noise.GetTree(subWorld.biomeNoise);
						if (tree4 != null)
						{
							tree = tree4;
						}
					}
					if (subWorld.overrideNoise != null)
					{
						ProcGen.Noise.Tree tree5 = SettingsCache.noise.GetTree(subWorld.overrideNoise);
						if (tree5 != null)
						{
							tree2 = tree5;
						}
					}
					if (subWorld.densityNoise != null)
					{
						ProcGen.Noise.Tree tree6 = SettingsCache.noise.GetTree(subWorld.densityNoise);
						if (tree6 != null)
						{
							tree3 = tree6;
						}
					}
				}
				int width = (int)Mathf.Ceil(overworldCell.poly.bounds.width + 1f);
				int height = (int)Mathf.Ceil(overworldCell.poly.bounds.height + 1f);
				int num2 = (int)Mathf.Floor(overworldCell.poly.bounds.xMin - 1f);
				int num3 = (int)Mathf.Floor(overworldCell.poly.bounds.yMin - 1f);
				Vector2 point;
				Vector2 offset = (point = new Vector2(num2, num3));
				NoiseMapBuilderCallback cb = delegate(int line)
				{
					updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (int)(currentProgress + perCell * ((float)line / (float)height)), WorldGenProgressStages.Stages.NoiseMapBuilder);
				};
				NoiseMap noiseMap = BuildNoiseMap(nmbp: BuildNoiseSource(width, height, tree), offset: offset, zoom: tree.settings.zoom, width: width, height: height, cb: cb);
				NoiseMap noiseMap2 = BuildNoiseMap(nmbp: BuildNoiseSource(width, height, tree2), offset: offset, zoom: tree2.settings.zoom, width: width, height: height, cb: cb);
				NoiseMap noiseMap3 = BuildNoiseMap(nmbp: BuildNoiseSource(width, height, tree3), offset: offset, zoom: tree3.settings.zoom, width: width, height: height, cb: cb);
				float num4 = float.MaxValue;
				float num5 = float.MinValue;
				float num6 = float.MaxValue;
				float num7 = float.MinValue;
				float num8 = float.MaxValue;
				float num9 = float.MinValue;
				List<int> list = new List<int>();
				point.x = (int)Mathf.Floor(overworldCell.poly.bounds.xMin);
				while (point.x <= (float)(int)Mathf.Ceil(overworldCell.poly.bounds.xMax))
				{
					point.y = (int)Mathf.Floor(overworldCell.poly.bounds.yMin);
					while (point.y <= (float)(int)Mathf.Ceil(overworldCell.poly.bounds.yMax))
					{
						if (overworldCell.poly.PointInPolygon(point))
						{
							int num10 = Grid.XYToCell((int)point.x, (int)point.y);
							list.Add(num10);
							int x = (int)point.x - num2;
							int y = (int)point.y - num3;
							BaseNoiseMap[num10] = noiseMap.GetValue(x, y);
							OverrideMap[num10] = noiseMap2.GetValue(x, y);
							DensityMap[num10] = noiseMap3.GetValue(x, y);
							num4 = Mathf.Min(BaseNoiseMap[num10], num4);
							num5 = Mathf.Max(BaseNoiseMap[num10], num5);
							num6 = Mathf.Min(OverrideMap[num10], num6);
							num7 = Mathf.Max(OverrideMap[num10], num7);
							num8 = Mathf.Min(DensityMap[num10], num8);
							num9 = Mathf.Max(DensityMap[num10], num9);
						}
						point.y += 1f;
					}
					point.x += 1f;
				}
				if (tree.settings.normalise)
				{
					float num11 = num5 - num4;
					for (int i = 0; i < list.Count; i++)
					{
						BaseNoiseMap[list[i]] = (BaseNoiseMap[list[i]] - num4) / num11;
					}
				}
				if (tree2.settings.normalise)
				{
					float num12 = num7 - num6;
					for (int j = 0; j < list.Count; j++)
					{
						OverrideMap[list[j]] = (OverrideMap[list[j]] - num6) / num12;
					}
				}
				if (tree3.settings.normalise)
				{
					float num13 = num9 - num8;
					for (int k = 0; k < list.Count; k++)
					{
						DensityMap[list[k]] = (DensityMap[list[k]] - num8) / num13;
					}
				}
				currentProgress += perCell;
			}
		}

		private float GetValue(Chunk chunk, Vector2I pos)
		{
			int num = pos.x + data.world.size.x * pos.y;
			if (num < 0 || num >= chunk.data.Length)
			{
				throw new ArgumentOutOfRangeException("chunkDataIndex [" + num + "]", "chunk data length [" + chunk.data.Length + "]");
			}
			return chunk.data[num];
		}

		public bool InChunkRange(Chunk chunk, Vector2I pos)
		{
			int num = pos.x + data.world.size.x * pos.y;
			if (num < 0 || num >= chunk.data.Length)
			{
				return false;
			}
			return true;
		}

		private TerrainCell.ElementOverride GetElementFromBiomeElementTable(Chunk chunk, Vector2I pos, List<ElementGradient> table, float erode)
		{
			float num = GetValue(chunk, pos) * erode;
			TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(voidElement.tag.ToString(), null);
			if (table.Count == 0)
			{
				return elementOverride;
			}
			for (int i = 0; i < table.Count; i++)
			{
				Debug.Assert(table[i].content != null, i.ToString());
				if (num < table[i].maxValue)
				{
					return TerrainCell.GetElementOverride(table[i].content, table[i].overrides);
				}
			}
			return TerrainCell.GetElementOverride(table[table.Count - 1].content, table[table.Count - 1].overrides);
		}

		public static bool CanLoad(string fileName)
		{
			if (fileName == null || fileName == "")
			{
				return false;
			}
			try
			{
				if (File.Exists(fileName))
				{
					using (BinaryReader binaryReader = new BinaryReader(File.Open(fileName, FileMode.Open)))
					{
						return binaryReader.BaseStream.CanRead;
					}
				}
				return false;
			}
			catch (FileNotFoundException)
			{
				return false;
			}
			catch (Exception ex2)
			{
				DebugUtil.LogWarningArgs("Failed to read " + fileName + "\n" + ex2.ToString());
				return false;
			}
		}

		public void SaveWorldGen()
		{
			try
			{
				Manager.Clear();
				WorldGenSave worldGenSave = new WorldGenSave();
				worldGenSave.version = new Vector2I(1, 1);
				worldGenSave.stats = stats;
				worldGenSave.data = data;
				worldGenSave.worldID = Settings.world.filePath;
				worldGenSave.traitIDs = new List<string>(Settings.GetTraitIDs());
				using MemoryStream memoryStream = new MemoryStream();
				using (BinaryWriter writer = new BinaryWriter(memoryStream))
				{
					try
					{
						Serializer.Serialize(worldGenSave, writer);
					}
					catch (Exception ex)
					{
						DebugUtil.LogErrorArgs("Couldn't serialize", ex.Message, ex.StackTrace);
					}
				}
				using BinaryWriter binaryWriter = new BinaryWriter(File.Open(WORLDGEN_SAVE_FILENAME, FileMode.Create));
				Manager.SerializeDirectory(binaryWriter);
				binaryWriter.Write(memoryStream.ToArray());
			}
			catch (Exception ex2)
			{
				DebugUtil.LogErrorArgs("Couldn't write", ex2.Message, ex2.StackTrace);
			}
		}

		public bool LoadWorldGen()
		{
			wasLoaded = LoadWorldGen(out var _, out var _, out var data, out var stats);
			if (wasLoaded)
			{
				this.data = data;
				this.stats = stats;
			}
			return wasLoaded;
		}

		public static bool LoadWorldGen(out string worldID, out List<string> traitIDs, out Data data, out Dictionary<string, object> stats)
		{
			bool flag = false;
			data = null;
			stats = null;
			worldID = null;
			traitIDs = new List<string>();
			try
			{
				WorldGenSave worldGenSave = new WorldGenSave();
				FastReader reader = new FastReader(File.ReadAllBytes(WORLDGEN_SAVE_FILENAME));
				Manager.DeserializeDirectory(reader);
				Deserializer.Deserialize(worldGenSave, reader);
				stats = worldGenSave.stats;
				data = worldGenSave.data;
				worldID = worldGenSave.worldID;
				traitIDs = worldGenSave.traitIDs;
				if (worldGenSave.version.x != 1 || worldGenSave.version.y > 1)
				{
					Debug.LogError("LoadWorldGenSim Error! Wrong save version Current: [" + 1 + "." + 1 + "] File: [" + worldGenSave.version.x + "." + worldGenSave.version.y + "]");
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				DebugUtil.LogErrorArgs("LoadWorldGenSim Error!\n", ex.Message, ex.StackTrace);
				return false;
			}
		}

		public static SimSaveFileStructure LoadWorldGenSim()
		{
			SimSaveFileStructure simSaveFileStructure = new SimSaveFileStructure();
			try
			{
				FastReader reader = new FastReader(File.ReadAllBytes(SIM_SAVE_FILENAME));
				Manager.DeserializeDirectory(reader);
				Deserializer.Deserialize(simSaveFileStructure, reader);
				return simSaveFileStructure;
			}
			catch (Exception ex)
			{
				DebugUtil.LogErrorArgs("LoadWorldGenSim Error!\n", ex.Message, ex.StackTrace);
				return null;
			}
		}

		public void DrawDebug()
		{
		}
	}
}
