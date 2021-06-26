using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database;
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

		private class NoiseNormalizationStats
		{
			public float[] noise;

			public float min = float.MaxValue;

			public float max = float.MinValue;

			public HashSet<int> cells = new HashSet<int>();

			public NoiseNormalizationStats(float[] noise)
			{
				this.noise = noise;
			}
		}

		private const string _SIM_SAVE_FILENAME = "WorldGenSimSave";

		private const string _SIM_SAVE_EXTENSION = ".dat";

		private const string _WORLDGEN_SAVE_FILENAME = "WorldGenDataSave.dat";

		private const int heatScale = 2;

		private const int UNPASSABLE_EDGE_COUNT = 4;

		private const string heat_noise_name = "noise/Heat";

		private const string default_base_noise_name = "noise/Default";

		private const string default_cave_noise_name = "noise/DefaultCave";

		private const string default_density_noise_name = "noise/DefaultDensity";

		public const int WORLDGEN_SAVE_MAJOR_VERSION = 1;

		public const int WORLDGEN_SAVE_MINOR_VERSION = 1;

		private const float EXTREME_TEMPERATURE_BORDER_RANGE = 150f;

		private const float EXTREME_TEMPERATURE_BORDER_MIN_WIDTH = 2f;

		public static Element voidElement;

		public static Element vacuumElement;

		public static Element katairiteElement;

		public static Element unobtaniumElement;

		private static Diseases m_diseasesDb;

		public bool isRunningDebugGen;

		private bool generateNoiseData = true;

		private HashSet<int> claimedCells = new HashSet<int>();

		public Dictionary<int, int> claimedPOICells = new Dictionary<int, int>();

		private HashSet<int> highPriorityClaims = new HashSet<int>();

		public List<RectInt> POIBounds = new List<RectInt>();

		private OfflineCallbackFunction successCallbackFn;

		private bool running = true;

		private Action<OfflineWorldGen.ErrorInfo> errorCallback;

		private SeededRandom myRandom = null;

		private NoiseMapBuilderPlane heatSource = null;

		private bool wasLoaded = false;

		public int polyIndex = -1;

		public bool isStartingWorld = false;

		public bool isModuleInterior = false;

		public static string WORLDGEN_SAVE_FILENAME => System.IO.Path.Combine(Util.RootFolder(), "WorldGenDataSave.dat");

		public static Diseases diseaseStats
		{
			get
			{
				if (m_diseasesDb == null)
				{
					m_diseasesDb = new Diseases(null, statsOnly: true);
				}
				return m_diseasesDb;
			}
		}

		public int BaseLeft => Settings.GetBaseLocation().left;

		public int BaseRight => Settings.GetBaseLocation().right;

		public int BaseTop => Settings.GetBaseLocation().top;

		public int BaseBot => Settings.GetBaseLocation().bottom;

		public Dictionary<string, object> stats
		{
			get;
			private set;
		}

		public Data data
		{
			get;
			private set;
		}

		public bool HasData => data != null;

		public bool HasNoiseData => HasData && data.world != null;

		public float[] DensityMap => data.world.density;

		public float[] HeatMap => data.world.heatOffset;

		public float[] OverrideMap => data.world.overrides;

		public float[] BaseNoiseMap => data.world.data;

		public float[] DefaultTendMap => data.world.defaultTemp;

		public Chunk World => data.world;

		public Vector2I WorldSize => data.world.size;

		public Vector2I WorldOffset => data.world.offset;

		public WorldLayout WorldLayout => data.worldLayout;

		public List<TerrainCell> OverworldCells => data.overworldCells;

		public List<TerrainCell> TerrainCells => data.terrainCells;

		public List<ProcGen.River> Rivers => data.rivers;

		public GameSpawnData SpawnData => data.gameSpawnData;

		public int ChunkEdgeSize => data.chunkEdgeSize;

		public HashSet<int> ClaimedCells => claimedCells;

		public HashSet<int> HighPriorityClaimedCells => highPriorityClaims;

		public WorldGenSettings Settings
		{
			get;
			private set;
		}

		public static string GetSIMSaveFilename(int baseID = -1)
		{
			return System.IO.Path.Combine(Util.RootFolder(), (baseID == -1) ? "WorldGenSimSave.dat" : string.Format("{0}{1}{2}", "WorldGenSimSave", baseID, ".dat"));
		}

		public void ClearClaimedCells()
		{
			claimedCells.Clear();
			highPriorityClaims.Clear();
		}

		public void AddHighPriorityCells(HashSet<int> cells)
		{
			highPriorityClaims.Union(cells);
		}

		public WorldGen(string worldName, List<string> chosenTraits, bool assertMissingTraits)
		{
			LoadSettings();
			Settings = new WorldGenSettings(worldName, chosenTraits, assertMissingTraits);
			data = new Data();
			data.chunkEdgeSize = Settings.GetIntSetting("ChunkEdgeSize");
			stats = new Dictionary<string, object>();
		}

		public WorldGen(string worldName, Data data, Dictionary<string, object> stats, List<string> chosenTraits, bool assertMissingTraits)
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
			if (CustomGameSettings.Instance != null)
			{
			}
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

		public void InitRandom(int worldSeed, int layoutSeed, int terrainSeed, int noiseSeed)
		{
			data.globalWorldSeed = worldSeed;
			data.globalWorldLayoutSeed = layoutSeed;
			data.globalTerrainSeed = terrainSeed;
			data.globalNoiseSeed = noiseSeed;
			Console.WriteLine($"Seeds are [{worldSeed}/{layoutSeed}/{terrainSeed}/{noiseSeed}]");
			myRandom = new SeededRandom(worldSeed);
		}

		public void Initialise(OfflineCallbackFunction callbackFn, Action<OfflineWorldGen.ErrorInfo> error_cb, int worldSeed = -1, int layoutSeed = -1, int terrainSeed = -1, int noiseSeed = -1, bool debug = false)
		{
			if (wasLoaded)
			{
				Debug.LogError("Initialise called after load");
				return;
			}
			successCallbackFn = callbackFn;
			errorCallback = error_cb;
			Debug.Assert(successCallbackFn != null);
			isRunningDebugGen = debug;
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
			DebugUtil.LogArgs(string.Format("World Seeds: {0} for world {1} [{2}/{3}/{4}/{5}]", worldSeed, Settings.world.name.Replace("STRINGS.WORLDS.", "").Replace(".NAME", ""), worldSeed, layoutSeed, terrainSeed, noiseSeed));
			InitRandom(worldSeed, layoutSeed, terrainSeed, noiseSeed);
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

		private void PlaceTemplateSpawners(Vector2I position, TemplateContainer template, ref Dictionary<int, int> claimedCells)
		{
			data.gameSpawnData.AddTemplate(template, position, ref claimedCells);
		}

		public bool RenderOffline(bool doSettle, ref Sim.Cell[] cells, ref Sim.DiseaseCell[] dc, int baseId, bool isStartingWorld = false)
		{
			float[] bgTemp = null;
			dc = null;
			HashSet<int> borderCells = new HashSet<int>();
			POIBounds = new List<RectInt>();
			WriteOverWorldNoise(successCallbackFn);
			if (!RenderToMap(successCallbackFn, ref cells, ref bgTemp, ref dc, ref borderCells, ref POIBounds))
			{
				successCallbackFn(UI.WORLDGEN.FAILED.key, -100f, WorldGenProgressStages.Stages.Failure);
				if (!isRunningDebugGen)
				{
					return false;
				}
			}
			foreach (int item in borderCells)
			{
				cells[item].SetValues(unobtaniumElement, ElementLoader.elements);
				claimedPOICells[item] = 1;
			}
			List<KeyValuePair<Vector2I, TemplateContainer>> list = null;
			try
			{
				list = TemplateSpawning.DetermineTemplatesForWorld(Settings, data.terrainCells, myRandom, ref POIBounds, isRunningDebugGen, successCallbackFn);
			}
			catch (Exception e)
			{
				if (!isRunningDebugGen)
				{
					ReportWorldGenError(e);
					return false;
				}
			}
			if (isStartingWorld)
			{
				EnsureEnoughElementsInStartingBiome(cells);
			}
			List<TerrainCell> terrainCellsForTag = GetTerrainCellsForTag(WorldGenTags.StartWorld);
			foreach (TerrainCell overworldCell in OverworldCells)
			{
				foreach (TerrainCell item2 in terrainCellsForTag)
				{
					if (overworldCell.poly.PointInPolygon(item2.poly.Centroid()))
					{
						overworldCell.node.tags.Add(WorldGenTags.StartWorld);
						break;
					}
				}
			}
			if (doSettle)
			{
				running = WorldGenSimUtil.DoSettleSim(Settings, ref cells, ref bgTemp, ref dc, successCallbackFn, data, list, errorCallback, baseId);
			}
			foreach (KeyValuePair<Vector2I, TemplateContainer> item3 in list)
			{
				PlaceTemplateSpawners(item3.Key, item3.Value, ref claimedPOICells);
			}
			if (doSettle)
			{
				SpawnMobsAndTemplates(cells, bgTemp, dc, new HashSet<int>(claimedPOICells.Keys));
			}
			successCallbackFn(UI.WORLDGEN.COMPLETE.key, 1f, WorldGenProgressStages.Stages.Complete);
			running = false;
			return true;
		}

		private void SpawnMobsAndTemplates(Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> claimedCells)
		{
			MobSpawning.DetectNaturalCavities(TerrainCells, successCallbackFn, cells);
			SeededRandom rnd = new SeededRandom(data.globalTerrainSeed);
			for (int i = 0; i < TerrainCells.Count; i++)
			{
				float completePercent = (float)i / (float)TerrainCells.Count;
				successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, completePercent, WorldGenProgressStages.Stages.PlacingCreatures);
				TerrainCell tc = TerrainCells[i];
				Dictionary<int, string> dictionary = MobSpawning.PlaceFeatureAmbientMobs(Settings, tc, rnd, cells, bgTemp, dc, claimedCells, isRunningDebugGen);
				if (dictionary != null)
				{
					data.gameSpawnData.AddRange(dictionary);
				}
				dictionary = MobSpawning.PlaceBiomeAmbientMobs(Settings, tc, rnd, cells, bgTemp, dc, claimedCells, isRunningDebugGen);
				if (dictionary != null)
				{
					data.gameSpawnData.AddRange(dictionary);
				}
			}
			successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, 1f, WorldGenProgressStages.Stages.PlacingCreatures);
		}

		private void ReportWorldGenError(Exception e)
		{
			string settingsCoordinate = CustomGameSettings.Instance.GetSettingsCoordinate();
			Debug.LogWarning("Worldgen Failure on seed " + settingsCoordinate);
			errorCallback(new OfflineWorldGen.ErrorInfo
			{
				errorDesc = string.Format(UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FAILURE, settingsCoordinate),
				exception = e
			});
			KCrashReporter.ReportErrorDevNotification("WorldgenFailure", e.StackTrace, $"{settingsCoordinate} - {e.Message} [Build: {469369u}]");
		}

		public void SetWorldSize(int width, int height)
		{
			if (data.world != null && data.world.offset != Vector2I.zero)
			{
				Debug.LogWarning("Resetting world chunk to defaults.");
			}
			data.world = new Chunk(0, 0, width, height);
		}

		public Vector2I GetSize()
		{
			return data.world.size;
		}

		public void SetPosition(Vector2I position)
		{
			data.world.offset = position;
		}

		public Vector2I GetPosition()
		{
			return data.world.offset;
		}

		public void SetClusterLocation(AxialI location)
		{
			data.clusterLocation = location;
		}

		public AxialI GetClusterLocation()
		{
			return data.clusterLocation;
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
				running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 1f, WorldGenProgressStages.Stages.SetupNoise);
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
				running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 1f, WorldGenProgressStages.Stages.WorldLayout);
				data.voronoiTree = null;
				try
				{
					data.voronoiTree = WorldLayout.GenerateOverworld(Settings.world.layoutMethod == ProcGen.World.LayoutMethod.PowerTree, isRunningDebugGen);
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
					Cell node = data.worldLayout.overworldGraph.FindNodeByID(tree.site.id);
					data.overworldCells.Add(new TerrainCellLogged(node, tree.site, tree.minDistanceToTag));
				}
				running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 1f, WorldGenProgressStages.Stages.WorldLayout);
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
				running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0.65f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!running)
				{
					return false;
				}
				running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0.75f, WorldGenProgressStages.Stages.CompleteLayout);
				if (!running)
				{
					return false;
				}
				data.terrainCells = new List<TerrainCell>(4000);
				List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
				data.voronoiTree.ForceLowestToLeaf();
				ApplyStartNode();
				ApplySwapTags();
				data.voronoiTree.GetLeafNodes(list);
				WorldLayout.ResetMapGraphFromVoronoiTree(list, WorldLayout.localGraph, clear: true);
				for (int i = 0; i < list.Count; i++)
				{
					VoronoiTree.Node node = list[i];
					Cell tn = data.worldLayout.localGraph.FindNodeByID(node.site.id);
					if (tn != null)
					{
						TerrainCell terrainCell = data.terrainCells.Find((TerrainCell c) => c.node == tn);
						if (terrainCell == null)
						{
							TerrainCell item = new TerrainCellLogged(tn, node.site, node.parent.minDistanceToTag);
							data.terrainCells.Add(item);
						}
						else
						{
							Debug.LogWarning("Duplicate cell found" + terrainCell.node.NodeId);
						}
					}
				}
				for (int j = 0; j < data.terrainCells.Count; j++)
				{
					TerrainCell terrainCell2 = data.terrainCells[j];
					for (int k = j + 1; k < data.terrainCells.Count; k++)
					{
						int edgeIdx = 0;
						TerrainCell terrainCell3 = data.terrainCells[k];
						if (terrainCell3.poly.SharesEdge(terrainCell2.poly, ref edgeIdx, out var _) == Polygon.Commonality.Edge)
						{
							terrainCell2.neighbourTerrainCells.Add(k);
							terrainCell3.neighbourTerrainCells.Add(j);
						}
					}
				}
				running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 1f, WorldGenProgressStages.Stages.CompleteLayout);
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

		public void EnsureEnoughElementsInStartingBiome(Sim.Cell[] cells)
		{
			List<StartingWorldElementSetting> defaultStartingElements = Settings.GetDefaultStartingElements();
			List<TerrainCell> terrainCellsForTag = GetTerrainCellsForTag(WorldGenTags.StartWorld);
			foreach (StartingWorldElementSetting item in defaultStartingElements)
			{
				float amount = item.amount;
				Element element = ElementLoader.GetElement(new Tag(((SimHashes)Enum.Parse(typeof(SimHashes), item.element, ignoreCase: true)).ToString()));
				float num = 0f;
				int num2 = 0;
				foreach (TerrainCell item2 in terrainCellsForTag)
				{
					foreach (int allCell in item2.GetAllCells())
					{
						if (element.idx == cells[allCell].elementIdx)
						{
							num2++;
							num += cells[allCell].mass;
						}
					}
				}
				DebugUtil.DevAssert(num2 > 0, $"No {element.id} found in starting biome and trying to ensure at least {amount}. Skipping.");
				if (!(num < amount) || num2 <= 0)
				{
					continue;
				}
				float num3 = num / (float)num2;
				float num4 = (amount - num) / (float)num2;
				DebugUtil.DevAssert(num3 + num4 <= 2f * element.maxMass, $"Number of cells ({num2}) of {element.id} in the starting biome is insufficient, this will result in extremely dense cells. {num3 + num4} but expecting less than {2f * element.maxMass}");
				foreach (TerrainCell item3 in terrainCellsForTag)
				{
					foreach (int allCell2 in item3.GetAllCells())
					{
						if (element.idx == cells[allCell2].elementIdx)
						{
							cells[allCell2].mass += num4;
						}
					}
				}
			}
		}

		public bool RenderToMap(OfflineCallbackFunction updateProgressFn, ref Sim.Cell[] cells, ref float[] bgTemp, ref Sim.DiseaseCell[] dcs, ref HashSet<int> borderCells, ref List<RectInt> poiBounds)
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
				running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, (float)i / (float)Grid.CellCount, WorldGenProgressStages.Stages.ClearingLevel);
				if (!running)
				{
					return false;
				}
			}
			updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 1f, WorldGenProgressStages.Stages.ClearingLevel);
			try
			{
				ProcessByTerrainCell(cells, bgTemp, dcs, updateProgressFn, highPriorityClaims);
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
				DrawWorldBorder(cells, data.world, rnd, ref borderCells, ref poiBounds, updateProgressFn);
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 1f, WorldGenProgressStages.Stages.DrawWorldBorder);
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

		public void FinalizeStartLocation()
		{
			if (!string.IsNullOrEmpty(Settings.world.startSubworldName))
			{
				List<VoronoiTree.Node> startNodes = WorldLayout.GetStartNodes();
				Debug.Assert(startNodes.Count > 0, "Couldn't find a start node on a world that expects it!!");
				TagSet tagSet = new TagSet();
				tagSet.Add(WorldGenTags.StartLocation);
				for (int i = 1; i < startNodes.Count; i++)
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
			Diagram.Site site = n1.site;
			n1.site = n2.site;
			n2.site = site;
			ProcGen.Node node = data.worldLayout.localGraph.FindNodeByID(n1.site.id);
			ProcGen.Node node2 = data.worldLayout.localGraph.FindNodeByID(n2.site.id);
			string type = node.type;
			node.SetType(node2.type);
			node2.SetType(type);
		}

		private void ApplyStartNode()
		{
			List<VoronoiTree.Node> leafNodesWithTag = data.worldLayout.GetLeafNodesWithTag(WorldGenTags.StartLocation);
			if (leafNodesWithTag.Count != 0)
			{
				VoronoiTree.Node node = leafNodesWithTag[0];
				VoronoiTree.Tree parent = node.parent;
				node.parent.AddTagToChildren(WorldGenTags.IgnoreCaveOverride);
				node.parent.tags.Remove(WorldGenTags.StartLocation);
			}
		}

		private void ApplySwapTags()
		{
			List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
			for (int i = 0; i < data.voronoiTree.ChildCount(); i++)
			{
				if (data.voronoiTree.GetChild(i).tags.Contains(WorldGenTags.SwapLakesToBelow))
				{
					list.Add(data.voronoiTree.GetChild(i));
				}
			}
			foreach (VoronoiTree.Node item in list)
			{
				if (!item.tags.Contains(WorldGenTags.CenteralFeature))
				{
					List<VoronoiTree.Node> nodes = new List<VoronoiTree.Node>();
					((VoronoiTree.Tree)item).GetNodesWithoutTag(WorldGenTags.CenteralFeature, nodes);
					SwapNodesAround(WorldGenTags.Wet, sendTagToBottom: true, nodes, item.site.poly.Centroid());
				}
			}
		}

		private void SwapNodesAround(Tag swapTag, bool sendTagToBottom, List<VoronoiTree.Node> nodes, Vector2 pivot)
		{
			nodes.ShuffleSeeded(myRandom.RandomSource());
			List<VoronoiTree.Node> list = new List<VoronoiTree.Node>();
			List<VoronoiTree.Node> list2 = new List<VoronoiTree.Node>();
			foreach (VoronoiTree.Node node in nodes)
			{
				bool flag = node.tags.Contains(swapTag);
				bool flag2 = node.site.poly.Centroid().y > pivot.y;
				bool flag3 = (flag2 && sendTagToBottom) || (!flag2 && !sendTagToBottom);
				if (flag && flag3)
				{
					if (list2.Count > 0)
					{
						SwitchNodes(node, list2[0]);
						list2.RemoveAt(0);
					}
					else
					{
						list.Add(node);
					}
				}
				else if (!flag && !flag3)
				{
					if (list.Count > 0)
					{
						SwitchNodes(node, list[0]);
						list.RemoveAt(0);
					}
					else
					{
						list2.Add(node);
					}
				}
			}
			if (list2.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list2.Count <= 0)
				{
					break;
				}
				SwitchNodes(list[i], list2[0]);
				list2.RemoveAt(0);
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

		private void ProcessByTerrainCell(Sim.Cell[] map_cells, float[] bgTemp, Sim.DiseaseCell[] dcs, OfflineCallbackFunction updateProgressFn, HashSet<int> hightPriorityCells)
		{
			updateProgressFn(UI.WORLDGEN.PROCESSING.key, 0f, WorldGenProgressStages.Stages.Processing);
			SeededRandom seededRandom = new SeededRandom(data.globalTerrainSeed);
			try
			{
				for (int i = 0; i < data.terrainCells.Count; i++)
				{
					updateProgressFn(UI.WORLDGEN.PROCESSING.key, (float)i / (float)data.terrainCells.Count, WorldGenProgressStages.Stages.Processing);
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
					Edge edge = edgesWithTag[j];
					List<Cell> cells2 = data.worldLayout.overworldGraph.GetNodes(edge);
					Debug.Assert(cells2[0] != cells2[1], "Both nodes on an arc were the same. Allegedly this means it was a world border but I don't think we do that anymore.");
					TerrainCell terrainCell = data.overworldCells.Find((TerrainCell c) => c.node == cells2[0]);
					TerrainCell terrainCell2 = data.overworldCells.Find((TerrainCell c) => c.node == cells2[1]);
					Debug.Assert(terrainCell != null && terrainCell2 != null, "NULL Terrainell nodes with EdgeUnpassable");
					terrainCell.LogInfo("BORDER WITH " + terrainCell2.site.id, "UNPASSABLE", 0f);
					terrainCell2.LogInfo("BORDER WITH " + terrainCell.site.id, "UNPASSABLE", 0f);
					Border border = new Border(new Neighbors(terrainCell, terrainCell2), edge.corner0.position, edge.corner1.position);
					border.element = SettingsCache.borders["impenetrable"];
					border.width = seededRandom.RandomRange(2, 3);
					list.Add(border);
				}
				List<Edge> edgesWithTag2 = data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeClosed);
				for (int k = 0; k < edgesWithTag2.Count; k++)
				{
					Edge edge2 = edgesWithTag2[k];
					if (edgesWithTag.Contains(edge2))
					{
						continue;
					}
					List<Cell> cells = data.worldLayout.overworldGraph.GetNodes(edge2);
					Debug.Assert(cells[0] != cells[1], "Both nodes on an arc were the same. Allegedly this means it was a world border but I don't think we do that anymore.");
					TerrainCell terrainCell3 = data.overworldCells.Find((TerrainCell c) => c.node == cells[0]);
					TerrainCell terrainCell4 = data.overworldCells.Find((TerrainCell c) => c.node == cells[1]);
					Debug.Assert(terrainCell3 != null && terrainCell4 != null, "NULL Terraincell nodes with EdgeClosed");
					string borderOverride = Settings.GetSubWorld(terrainCell3.node.type).borderOverride;
					string borderOverride2 = Settings.GetSubWorld(terrainCell4.node.type).borderOverride;
					string text;
					if (!string.IsNullOrEmpty(borderOverride2) && !string.IsNullOrEmpty(borderOverride))
					{
						text = ((seededRandom.RandomValue() > 0.5f) ? borderOverride2 : borderOverride);
						terrainCell3.LogInfo("BORDER WITH " + terrainCell4.site.id, "Picked Random:" + text, 0f);
						terrainCell4.LogInfo("BORDER WITH " + terrainCell3.site.id, "Picked Random:" + text, 0f);
					}
					else if (string.IsNullOrEmpty(borderOverride2) && string.IsNullOrEmpty(borderOverride))
					{
						text = "hardToDig";
						terrainCell3.LogInfo("BORDER WITH " + terrainCell4.site.id, "Both null", 0f);
						terrainCell4.LogInfo("BORDER WITH " + terrainCell3.site.id, "Both null", 0f);
					}
					else
					{
						text = ((!string.IsNullOrEmpty(borderOverride2)) ? borderOverride2 : borderOverride);
						terrainCell3.LogInfo("BORDER WITH " + terrainCell4.site.id, "Picked specific " + text, 0f);
						terrainCell4.LogInfo("BORDER WITH " + terrainCell3.site.id, "Picked specific " + text, 0f);
					}
					if (!(text == "NONE"))
					{
						Border border2 = new Border(new Neighbors(terrainCell3, terrainCell4), edge2.corner0.position, edge2.corner1.position);
						border2.element = SettingsCache.borders[text];
						MinMax minMax = new MinMax(1.5f, 2f);
						MinMax borderSizeOverride = Settings.GetSubWorld(terrainCell3.node.type).borderSizeOverride;
						MinMax borderSizeOverride2 = Settings.GetSubWorld(terrainCell4.node.type).borderSizeOverride;
						bool flag = borderSizeOverride.min != 0f || borderSizeOverride.max != 0f;
						bool flag2 = borderSizeOverride2.min != 0f || borderSizeOverride2.max != 0f;
						if (flag && flag2)
						{
							minMax = ((borderSizeOverride.max > borderSizeOverride2.max) ? borderSizeOverride : borderSizeOverride2);
						}
						else if (flag)
						{
							minMax = borderSizeOverride;
						}
						else if (flag2)
						{
							minMax = borderSizeOverride2;
						}
						border2.width = seededRandom.RandomRange(minMax.min, minMax.max);
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
						if (!highPriorityClaims.Contains(index))
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
					float num = (SettingsCache.temperatures[subWorld.temperatureRange].min + SettingsCache.temperatures[subWorld.temperatureRange].max) / 2f;
					float num2 = (SettingsCache.temperatures[subWorld2.temperatureRange].min + SettingsCache.temperatures[subWorld2.temperatureRange].max) / 2f;
					float num3 = Mathf.Min(SettingsCache.temperatures[subWorld.temperatureRange].min, SettingsCache.temperatures[subWorld2.temperatureRange].min);
					float num4 = Mathf.Max(SettingsCache.temperatures[subWorld.temperatureRange].max, SettingsCache.temperatures[subWorld2.temperatureRange].max);
					float midTemp = (num + num2) / 2f;
					float num5 = num4 - num3;
					float rangeLow = 2f;
					float rangeHigh = 5f;
					int snapLastCells = 1;
					if (num5 >= 150f)
					{
						rangeLow = 0f;
						rangeHigh = border3.width * 0.2f;
						snapLastCells = 2;
						border3.width = Mathf.Max(border3.width, 2f);
						float f = num - 273.15f;
						float f2 = num2 - 273.15f;
						midTemp = ((!(Mathf.Abs(f) < Mathf.Abs(f2))) ? num2 : num);
					}
					border3.Stagger(seededRandom, seededRandom.RandomRange(8, 13), seededRandom.RandomRange(rangeLow, rangeHigh));
					border3.ConvertToMap(data.world, setValues, num, num2, midTemp, seededRandom, snapLastCells);
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

		private void DrawWorldBorder(Sim.Cell[] cells, Chunk world, SeededRandom rnd, ref HashSet<int> borderCells, ref List<RectInt> poiBounds, OfflineCallbackFunction updateProgressFn)
		{
			bool boolSetting = Settings.GetBoolSetting("DrawWorldBorderOverVacuum");
			int intSetting = Settings.GetIntSetting("WorldBorderThickness");
			int intSetting2 = Settings.GetIntSetting("WorldBorderRange");
			byte b = (byte)ElementLoader.elements.IndexOf(vacuumElement);
			byte b2 = (byte)ElementLoader.elements.IndexOf(voidElement);
			byte new_elem_idx = (byte)ElementLoader.elements.IndexOf(unobtaniumElement);
			float temperature = unobtaniumElement.defaultValues.temperature;
			float mass = unobtaniumElement.defaultValues.mass;
			int num = 0;
			int num2 = 0;
			updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 0f, WorldGenProgressStages.Stages.DrawWorldBorder);
			int num3 = world.size.y - 1;
			int num4 = 0;
			int num5 = world.size.x - 1;
			for (int num6 = num3; num6 >= 0; num6--)
			{
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, (float)num6 / (float)num3 * 0.33f, WorldGenProgressStages.Stages.DrawWorldBorder);
				num = Mathf.Max(-intSetting2, Mathf.Min(num + rnd.RandomRange(-2, 2), intSetting2));
				for (int i = 0; i < intSetting + num; i++)
				{
					int num7 = Grid.XYToCell(i, num6);
					if (boolSetting || (cells[num7].elementIdx != b && cells[num7].elementIdx != b2))
					{
						borderCells.Add(num7);
						cells[num7].SetValues(new_elem_idx, temperature, mass);
						num4 = Mathf.Max(num4, i);
					}
				}
				num2 = Mathf.Max(-intSetting2, Mathf.Min(num2 + rnd.RandomRange(-2, 2), intSetting2));
				for (int j = 0; j < intSetting + num2; j++)
				{
					int num8 = world.size.x - 1 - j;
					int num9 = Grid.XYToCell(num8, num6);
					if (boolSetting || (cells[num9].elementIdx != b && cells[num9].elementIdx != b2))
					{
						borderCells.Add(num9);
						cells[num9].SetValues(new_elem_idx, temperature, mass);
						num5 = Mathf.Min(num5, num8);
					}
				}
			}
			POIBounds.Add(new RectInt(0, 0, num4 + 1, World.size.y));
			POIBounds.Add(new RectInt(num5, 0, world.size.x - num5, World.size.y));
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			int num13 = World.size.y - 1;
			for (int k = 0; k < world.size.x; k++)
			{
				updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, (float)k / (float)world.size.x * 0.66f + 0.33f, WorldGenProgressStages.Stages.DrawWorldBorder);
				num10 = Mathf.Max(-intSetting2, Mathf.Min(num10 + rnd.RandomRange(-2, 2), intSetting2));
				for (int l = 0; l < intSetting + num10; l++)
				{
					int num14 = Grid.XYToCell(k, l);
					if (boolSetting || (cells[num14].elementIdx != b && cells[num14].elementIdx != b2))
					{
						borderCells.Add(num14);
						cells[num14].SetValues(new_elem_idx, temperature, mass);
						num12 = Mathf.Max(num12, l);
					}
				}
				num11 = Mathf.Max(-intSetting2, Mathf.Min(num11 + rnd.RandomRange(-2, 2), intSetting2));
				for (int m = 0; m < intSetting + num11; m++)
				{
					int num15 = world.size.y - 1 - m;
					int num16 = Grid.XYToCell(k, num15);
					if (boolSetting || (cells[num16].elementIdx != b && cells[num16].elementIdx != b2))
					{
						borderCells.Add(num16);
						cells[num16].SetValues(new_elem_idx, temperature, mass);
						num13 = Mathf.Min(num13, num15);
					}
				}
			}
			POIBounds.Add(new RectInt(0, 0, World.size.x, num12 + 1));
			POIBounds.Add(new RectInt(0, num13, World.size.x, World.size.y - num13));
		}

		private void SetupNoise(OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 0f, WorldGenProgressStages.Stages.SetupNoise);
			heatSource = BuildNoiseSource(data.world.size.x, data.world.size.y, "noise/Heat");
			updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 1f, WorldGenProgressStages.Stages.SetupNoise);
		}

		public NoiseMapBuilderPlane BuildNoiseSource(int width, int height, string name)
		{
			ProcGen.Noise.Tree tree = SettingsCache.noise.GetTree(name);
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
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (int)(0f + 0.25f * ((float)line / (float)data.world.size.y)), WorldGenProgressStages.Stages.GenerateNoise);
			};
			noiseMapBuilderCallback = delegate(int line)
			{
				updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (int)(0.25f + 0.25f * ((float)line / (float)data.world.size.y)), WorldGenProgressStages.Stages.GenerateNoise);
			};
			if (noiseMapBuilderCallback == null)
			{
				Debug.LogError("nupd is null");
			}
			data.world.heatOffset = GenerateNoise(offset, SettingsCache.noise.GetZoomForTree("noise/Heat"), heatSource, data.world.size.x, data.world.size.y, noiseMapBuilderCallback);
			data.world.data = new float[data.world.heatOffset.Length];
			data.world.density = new float[data.world.heatOffset.Length];
			data.world.overrides = new float[data.world.heatOffset.Length];
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 0.5f, WorldGenProgressStages.Stages.GenerateNoise);
			if (SettingsCache.noise.ShouldNormaliseTree("noise/Heat"))
			{
				Normalise(data.world.heatOffset);
			}
			updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 1f, WorldGenProgressStages.Stages.GenerateNoise);
		}

		public void WriteOverWorldNoise(OfflineCallbackFunction updateProgressFn)
		{
			Dictionary<HashedString, NoiseNormalizationStats> dictionary = new Dictionary<HashedString, NoiseNormalizationStats>();
			float num = OverworldCells.Count;
			float perCell = 1f / num;
			float currentProgress = 0f;
			foreach (TerrainCell overworldCell in OverworldCells)
			{
				ProcGen.Noise.Tree tree = SettingsCache.noise.GetTree("noise/Default");
				ProcGen.Noise.Tree tree2 = SettingsCache.noise.GetTree("noise/DefaultCave");
				ProcGen.Noise.Tree tree3 = SettingsCache.noise.GetTree("noise/DefaultDensity");
				string s = "noise/Default";
				string s2 = "noise/DefaultCave";
				string s3 = "noise/DefaultDensity";
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
							s = subWorld.biomeNoise;
						}
					}
					if (subWorld.overrideNoise != null)
					{
						ProcGen.Noise.Tree tree5 = SettingsCache.noise.GetTree(subWorld.overrideNoise);
						if (tree5 != null)
						{
							tree2 = tree5;
							s2 = subWorld.overrideNoise;
						}
					}
					if (subWorld.densityNoise != null)
					{
						ProcGen.Noise.Tree tree6 = SettingsCache.noise.GetTree(subWorld.densityNoise);
						if (tree6 != null)
						{
							tree3 = tree6;
							s3 = subWorld.densityNoise;
						}
					}
				}
				if (!dictionary.TryGetValue(s, out var value))
				{
					value = new NoiseNormalizationStats(BaseNoiseMap);
					dictionary.Add(s, value);
				}
				if (!dictionary.TryGetValue(s2, out var value2))
				{
					value2 = new NoiseNormalizationStats(OverrideMap);
					dictionary.Add(s2, value2);
				}
				if (!dictionary.TryGetValue(s3, out var value3))
				{
					value3 = new NoiseNormalizationStats(DensityMap);
					dictionary.Add(s3, value3);
				}
				int width = (int)Mathf.Ceil(overworldCell.poly.bounds.width + 2f);
				int height = (int)Mathf.Ceil(overworldCell.poly.bounds.height + 2f);
				int num2 = (int)Mathf.Floor(overworldCell.poly.bounds.xMin - 1f);
				int num3 = (int)Mathf.Floor(overworldCell.poly.bounds.yMin - 1f);
				Vector2 vector = new Vector2(num2, num3);
				Vector2 point = vector;
				NoiseMapBuilderCallback cb = delegate(int line)
				{
					updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, (int)(currentProgress + perCell * ((float)line / (float)height)), WorldGenProgressStages.Stages.NoiseMapBuilder);
				};
				NoiseMapBuilderPlane nmbp = BuildNoiseSource(width, height, tree);
				NoiseMap noiseMap = BuildNoiseMap(vector, tree.settings.zoom, nmbp, width, height, cb);
				NoiseMapBuilderPlane nmbp2 = BuildNoiseSource(width, height, tree2);
				NoiseMap noiseMap2 = BuildNoiseMap(vector, tree2.settings.zoom, nmbp2, width, height, cb);
				NoiseMapBuilderPlane nmbp3 = BuildNoiseSource(width, height, tree3);
				NoiseMap noiseMap3 = BuildNoiseMap(vector, tree3.settings.zoom, nmbp3, width, height, cb);
				point.x = (int)Mathf.Floor(overworldCell.poly.bounds.xMin);
				while (point.x <= (float)(int)Mathf.Ceil(overworldCell.poly.bounds.xMax))
				{
					point.y = (int)Mathf.Floor(overworldCell.poly.bounds.yMin);
					while (point.y <= (float)(int)Mathf.Ceil(overworldCell.poly.bounds.yMax))
					{
						if (overworldCell.poly.PointInPolygon(point))
						{
							int num4 = Grid.XYToCell((int)point.x, (int)point.y);
							if (tree.settings.normalise)
							{
								value.cells.Add(num4);
							}
							if (tree2.settings.normalise)
							{
								value2.cells.Add(num4);
							}
							if (tree3.settings.normalise)
							{
								value3.cells.Add(num4);
							}
							int x = (int)point.x - num2;
							int y = (int)point.y - num3;
							BaseNoiseMap[num4] = noiseMap.GetValue(x, y);
							OverrideMap[num4] = noiseMap2.GetValue(x, y);
							DensityMap[num4] = noiseMap3.GetValue(x, y);
							value.min = Mathf.Min(BaseNoiseMap[num4], value.min);
							value.max = Mathf.Max(BaseNoiseMap[num4], value.max);
							value2.min = Mathf.Min(OverrideMap[num4], value2.min);
							value2.max = Mathf.Max(OverrideMap[num4], value2.max);
							value3.min = Mathf.Min(DensityMap[num4], value3.min);
							value3.max = Mathf.Max(DensityMap[num4], value3.max);
						}
						point.y += 1f;
					}
					point.x += 1f;
				}
			}
			foreach (KeyValuePair<HashedString, NoiseNormalizationStats> item in dictionary)
			{
				float num5 = item.Value.max - item.Value.min;
				foreach (int cell in item.Value.cells)
				{
					item.Value.noise[cell] = (item.Value.noise[cell] - item.Value.min) / num5;
				}
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

		public static WorldGen Load(IReader reader, bool defaultDiscovered)
		{
			try
			{
				WorldGenSave worldGenSave = new WorldGenSave();
				Deserializer.Deserialize(worldGenSave, reader);
				WorldGen worldGen = new WorldGen(worldGenSave.worldID, worldGenSave.data, worldGenSave.stats, worldGenSave.traitIDs, assertMissingTraits: false);
				worldGen.isStartingWorld = true;
				if (worldGenSave.version.x != 1 || worldGenSave.version.y > 1)
				{
					DebugUtil.LogErrorArgs("LoadWorldGenSim Error! Wrong save version Current: [" + 1 + "." + 1 + "] File: [" + worldGenSave.version.x + "." + worldGenSave.version.y + "]");
					worldGen.wasLoaded = false;
				}
				else
				{
					worldGen.wasLoaded = true;
				}
				return worldGen;
			}
			catch (Exception ex)
			{
				DebugUtil.LogErrorArgs("WorldGen.Load Error!\n", ex.Message, ex.StackTrace);
				return null;
			}
		}

		public void DrawDebug()
		{
		}
	}
}
