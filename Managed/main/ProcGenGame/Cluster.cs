using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Klei;
using KSerialization;
using ProcGen;
using UnityEngine;

namespace ProcGenGame
{
	[Serializable]
	public class Cluster
	{
		public List<WorldGen> worlds = new List<WorldGen>();

		public WorldGen currentWorld;

		public Vector2I size;

		public string Id;

		public int numRings = 5;

		private int seed;

		private SeededRandom myRandom;

		private bool doSimSettle = true;

		public Action<int, WorldGen> PerWorldGenBeginCallback;

		public Action<int, WorldGen, Sim.Cell[], Sim.DiseaseCell[]> PerWorldGenCompleteCallback;

		public Func<int, WorldGen, bool> ShouldSkipWorldCallback;

		public Dictionary<ClusterLayoutSave.POIType, List<AxialI>> poiLocations = new Dictionary<ClusterLayoutSave.POIType, List<AxialI>>();

		public Dictionary<AxialI, string> poiPlacements = new Dictionary<AxialI, string>();

		private Thread thread;

		public ClusterLayout clusterLayout { get; private set; }

		public bool IsGenerationComplete { get; private set; }

		public bool IsGenerating
		{
			get
			{
				if (thread != null)
				{
					return thread.IsAlive;
				}
				return false;
			}
		}

		private Cluster()
		{
		}

		public Cluster(string name, int seed, bool assertMissingTraits, bool skipWorldTraits)
		{
			DebugUtil.Assert(!string.IsNullOrEmpty(name), "Cluster file is missing");
			this.seed = seed;
			WorldGen.LoadSettings();
			clusterLayout = SettingsCache.clusterLayouts.clusterCache[name];
			Id = name;
			for (int i = 0; i < clusterLayout.worldPlacements.Count; i++)
			{
				ProcGen.World worldData = SettingsCache.worlds.GetWorldData(clusterLayout.worldPlacements[i].world);
				if (worldData != null)
				{
					clusterLayout.worldPlacements[i].SetSize(worldData.worldsize);
					if (i == clusterLayout.startWorldIndex)
					{
						clusterLayout.worldPlacements[i].startWorld = true;
					}
				}
			}
			size = BestFit.BestFitWorlds(clusterLayout.worldPlacements);
			foreach (WorldPlacement worldPlacement in clusterLayout.worldPlacements)
			{
				List<string> chosenTraits = new List<string>();
				if (seed > 0 && !skipWorldTraits)
				{
					ProcGen.World worldData2 = SettingsCache.worlds.GetWorldData(worldPlacement.world);
					chosenTraits = SettingsCache.GetRandomTraits(seed, worldData2);
					seed++;
				}
				WorldGen worldGen = new WorldGen(worldPlacement.world, chosenTraits, assertMissingTraits);
				Vector2I worldsize = worldGen.Settings.world.worldsize;
				worldGen.SetWorldSize(worldsize.x, worldsize.y);
				worldGen.SetPosition(new Vector2I(worldPlacement.x, worldPlacement.y));
				worlds.Add(worldGen);
				if (worldPlacement.startWorld)
				{
					currentWorld = worldGen;
					worldGen.isStartingWorld = true;
				}
			}
			if (currentWorld == null)
			{
				Debug.LogWarning($"Start world not set. Defaulting to first world {worlds[0].Settings.world.name}");
				currentWorld = worlds[0];
			}
			if (clusterLayout.numRings > 0)
			{
				numRings = clusterLayout.numRings;
			}
		}

		public void Reset()
		{
			worlds.Clear();
		}

		public void Generate(WorldGen.OfflineCallbackFunction callbackFn, Action<OfflineWorldGen.ErrorInfo> error_cb, int worldSeed = -1, int layoutSeed = -1, int terrainSeed = -1, int noiseSeed = -1, bool doSimSettle = true, bool debug = false)
		{
			this.doSimSettle = doSimSettle;
			for (int i = 0; i != worlds.Count; i++)
			{
				worlds[i].Initialise(callbackFn, error_cb, worldSeed + i, layoutSeed + i, terrainSeed + i, noiseSeed + i, debug);
			}
			IsGenerationComplete = false;
			thread = new Thread(ThreadMain);
			Util.ApplyInvariantCultureToThread(thread);
			thread.Start();
		}

		private void BeginGeneration()
		{
			Sim.Cell[] array = null;
			Sim.DiseaseCell[] array2 = null;
			int num = 0;
			for (int i = 0; i < worlds.Count; i++)
			{
				WorldGen worldGen = worlds[i];
				if (ShouldSkipWorldCallback != null && ShouldSkipWorldCallback(i, worldGen))
				{
					Debug.Log("Skipping worldgen for " + worldGen.Settings.world.name);
					continue;
				}
				if (PerWorldGenBeginCallback != null)
				{
					PerWorldGenBeginCallback(i, worldGen);
				}
				GridSettings.Reset(worldGen.GetSize().x, worldGen.GetSize().y);
				worldGen.GenerateOffline();
				worldGen.FinalizeStartLocation();
				array = null;
				array2 = null;
				if (!worldGen.RenderOffline(doSimSettle, ref array, ref array2, num, worldGen.isStartingWorld))
				{
					thread = null;
					return;
				}
				if (PerWorldGenCompleteCallback != null)
				{
					PerWorldGenCompleteCallback(i, worldGen, array, array2);
				}
				num++;
			}
			AssignClusterLocations();
			Save();
			thread = null;
			IsGenerationComplete = true;
		}

		private bool IsValidHex(AxialI location)
		{
			return location.IsWithinRadius(AxialI.ZERO, numRings - 1);
		}

		public void AssignClusterLocations()
		{
			myRandom = new SeededRandom(seed);
			ClusterLayout obj = SettingsCache.clusterLayouts.clusterCache[Id];
			List<WorldPlacement> worldPlacements = obj.worldPlacements;
			List<SpaceMapPOIPlacement> list = obj.poiPlacements;
			currentWorld.SetClusterLocation(AxialI.ZERO);
			HashSet<AxialI> assignedLocations = new HashSet<AxialI>();
			HashSet<AxialI> worldForbiddenLocations = new HashSet<AxialI>();
			new HashSet<AxialI>();
			HashSet<AxialI> poiWorldAvoidance = new HashSet<AxialI>();
			int maxRadius = 2;
			for (int i = 0; i < worlds.Count; i++)
			{
				WorldGen worldGen = worlds[i];
				WorldPlacement worldPlacement = worldPlacements[i];
				DebugUtil.Assert(worldPlacement != null, "Somehow we're trying to generate a cluster with a world that isn't the cluster .yaml's world list!", worldGen.Settings.world.filePath);
				HashSet<AxialI> antiBuffer = new HashSet<AxialI>();
				foreach (AxialI item in assignedLocations)
				{
					antiBuffer.UnionWith(AxialUtil.GetRings(item, 1, worldPlacement.buffer));
				}
				List<AxialI> list2 = (from location in AxialUtil.GetRings(AxialI.ZERO, worldPlacement.allowedRings.min, Mathf.Min(worldPlacement.allowedRings.max, numRings - 1))
					where !assignedLocations.Contains(location) && !worldForbiddenLocations.Contains(location) && !antiBuffer.Contains(location)
					select location).ToList();
				if (list2.Count > 0)
				{
					AxialI axialI = list2[myRandom.RandomRange(0, list2.Count)];
					worldGen.SetClusterLocation(axialI);
					assignedLocations.Add(axialI);
					worldForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI, 1, worldPlacement.buffer));
					poiWorldAvoidance.UnionWith(AxialUtil.GetRings(axialI, 1, maxRadius));
					continue;
				}
				DebugUtil.DevLogError("Could not find a spot in the cluster for " + worldGen.Settings.world.filePath + ". Check the placement settings in " + Id + ".yaml to ensure there are no conflicts.");
				HashSet<AxialI> minBuffers = new HashSet<AxialI>();
				foreach (AxialI item2 in assignedLocations)
				{
					minBuffers.UnionWith(AxialUtil.GetRings(item2, 1, 2));
				}
				list2 = (from location in AxialUtil.GetRings(AxialI.ZERO, worldPlacement.allowedRings.min, Mathf.Min(worldPlacement.allowedRings.max, numRings - 1))
					where !assignedLocations.Contains(location) && !minBuffers.Contains(location)
					select location).ToList();
				DebugUtil.Assert(list2.Count > 0, "Could not find a spot in the cluster for " + worldGen.Settings.world.filePath + " EVEN AFTER REDUCING BUFFERS. Check the placement settings in " + Id + ".yaml to ensure there are no conflicts.");
				AxialI axialI2 = list2[myRandom.RandomRange(0, list2.Count)];
				worldGen.SetClusterLocation(axialI2);
				assignedLocations.Add(axialI2);
				worldForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI2, 1, worldPlacement.buffer));
				poiWorldAvoidance.UnionWith(AxialUtil.GetRings(axialI2, 1, maxRadius));
			}
			if (!DlcManager.FeatureClusterSpaceEnabled() || list == null)
			{
				return;
			}
			HashSet<AxialI> poiClumpLocations = new HashSet<AxialI>();
			HashSet<AxialI> poiForbiddenLocations = new HashSet<AxialI>();
			float num = 0.5f;
			int num2 = 3;
			int num3 = 0;
			foreach (SpaceMapPOIPlacement item3 in list)
			{
				List<string> pois = item3.pois;
				for (int j = 0; j < item3.numToSpawn; j++)
				{
					bool num4 = myRandom.RandomRange(0f, 1f) <= num;
					List<AxialI> list3 = null;
					if (num4 && num3 < num2 && !item3.avoidClumping)
					{
						num3++;
						list3 = (from location in AxialUtil.GetRings(AxialI.ZERO, item3.allowedRings.min, Mathf.Min(item3.allowedRings.max, numRings - 1))
							where !assignedLocations.Contains(location) && poiClumpLocations.Contains(location) && !poiWorldAvoidance.Contains(location)
							select location).ToList();
					}
					if (list3 == null || list3.Count <= 0)
					{
						num3 = 0;
						poiClumpLocations.Clear();
						list3 = (from location in AxialUtil.GetRings(AxialI.ZERO, item3.allowedRings.min, Mathf.Min(item3.allowedRings.max, numRings - 1))
							where !assignedLocations.Contains(location) && !poiWorldAvoidance.Contains(location) && !poiForbiddenLocations.Contains(location)
							select location).ToList();
					}
					if (list3 != null && list3.Count > 0)
					{
						AxialI axialI3 = list3[myRandom.RandomRange(0, list3.Count)];
						string text = pois[myRandom.RandomRange(0, pois.Count)];
						if (!item3.canSpawnDuplicates)
						{
							pois.Remove(text);
						}
						poiPlacements[axialI3] = text;
						poiForbiddenLocations.UnionWith(AxialUtil.GetRings(axialI3, 1, 3));
						poiClumpLocations.UnionWith(AxialUtil.GetRings(axialI3, 1, 1));
						assignedLocations.Add(axialI3);
					}
					else
					{
						Debug.LogWarning($"There is no room for a Space POI in ring range [{item3.allowedRings.min}, {item3.allowedRings.max}]");
					}
				}
			}
		}

		public void AbortGeneration()
		{
			if (thread != null && thread.IsAlive)
			{
				thread.Abort();
				thread = null;
			}
		}

		private void ThreadMain()
		{
			BeginGeneration();
		}

		private void Save()
		{
			try
			{
				using MemoryStream memoryStream = new MemoryStream();
				using (BinaryWriter writer = new BinaryWriter(memoryStream))
				{
					try
					{
						Manager.Clear();
						ClusterLayoutSave clusterLayoutSave = new ClusterLayoutSave();
						clusterLayoutSave.version = new Vector2I(1, 1);
						clusterLayoutSave.size = size;
						clusterLayoutSave.ID = Id;
						clusterLayoutSave.numRings = numRings;
						clusterLayoutSave.poiLocations = poiLocations;
						clusterLayoutSave.poiPlacements = poiPlacements;
						for (int i = 0; i != worlds.Count; i++)
						{
							WorldGen worldGen = worlds[i];
							if (ShouldSkipWorldCallback == null || !ShouldSkipWorldCallback(i, worldGen))
							{
								clusterLayoutSave.worlds.Add(new ClusterLayoutSave.World
								{
									data = worldGen.data,
									stats = worldGen.stats,
									name = worldGen.Settings.world.filePath,
									isDiscovered = worldGen.isStartingWorld,
									traits = worldGen.Settings.GetTraitIDs().ToList()
								});
								if (worldGen == currentWorld)
								{
									clusterLayoutSave.currentWorldIdx = i;
								}
							}
						}
						Serializer.Serialize(clusterLayoutSave, writer);
					}
					catch (Exception ex)
					{
						DebugUtil.LogErrorArgs("Couldn't serialize", ex.Message, ex.StackTrace);
					}
				}
				using BinaryWriter binaryWriter = new BinaryWriter(File.Open(WorldGen.WORLDGEN_SAVE_FILENAME, FileMode.Create));
				Manager.SerializeDirectory(binaryWriter);
				binaryWriter.Write(memoryStream.ToArray());
			}
			catch (Exception ex2)
			{
				DebugUtil.LogErrorArgs("Couldn't write", ex2.Message, ex2.StackTrace);
			}
		}

		public static Cluster Load()
		{
			Cluster cluster = new Cluster();
			try
			{
				FastReader fastReader = new FastReader(File.ReadAllBytes(WorldGen.WORLDGEN_SAVE_FILENAME));
				Manager.DeserializeDirectory(fastReader);
				int position = fastReader.Position;
				ClusterLayoutSave clusterLayoutSave = new ClusterLayoutSave();
				if (!Deserializer.Deserialize(clusterLayoutSave, fastReader))
				{
					fastReader.Position = position;
					WorldGen worldGen = WorldGen.Load(fastReader, defaultDiscovered: true);
					cluster.worlds.Add(worldGen);
					cluster.size = worldGen.GetSize();
					cluster.currentWorld = cluster.worlds[0] ?? null;
				}
				else
				{
					for (int i = 0; i != clusterLayoutSave.worlds.Count; i++)
					{
						ClusterLayoutSave.World world = clusterLayoutSave.worlds[i];
						WorldGen item = new WorldGen(world.name, world.data, world.stats, world.traits, assertMissingTraits: false);
						cluster.worlds.Add(item);
						if (i == clusterLayoutSave.currentWorldIdx)
						{
							cluster.currentWorld = item;
							cluster.worlds[i].isStartingWorld = true;
						}
					}
					cluster.size = clusterLayoutSave.size;
					cluster.Id = clusterLayoutSave.ID;
					cluster.numRings = clusterLayoutSave.numRings;
					cluster.poiLocations = clusterLayoutSave.poiLocations;
					cluster.poiPlacements = clusterLayoutSave.poiPlacements;
				}
				DebugUtil.Assert(cluster.currentWorld != null);
				if (cluster.currentWorld == null)
				{
					DebugUtil.Assert(0 < cluster.worlds.Count);
					cluster.currentWorld = cluster.worlds[0];
					return cluster;
				}
				return cluster;
			}
			catch (Exception ex)
			{
				DebugUtil.LogErrorArgs("SolarSystem.Load Error!\n", ex.Message, ex.StackTrace);
				return null;
			}
		}

		public void LoadClusterLayoutSim(List<SimSaveFileStructure> loadedWorlds)
		{
			for (int i = 0; i != worlds.Count; i++)
			{
				SimSaveFileStructure simSaveFileStructure = new SimSaveFileStructure();
				try
				{
					FastReader reader = new FastReader(File.ReadAllBytes(WorldGen.GetSIMSaveFilename(i)));
					Manager.DeserializeDirectory(reader);
					Deserializer.Deserialize(simSaveFileStructure, reader);
				}
				catch (Exception ex)
				{
					if (!GenericGameSettings.instance.devAutoWorldGenActive)
					{
						DebugUtil.LogErrorArgs("LoadSim Error!\n", ex.Message, ex.StackTrace);
						return;
					}
				}
				if (simSaveFileStructure.worldDetail == null)
				{
					if (!GenericGameSettings.instance.devAutoWorldGenActive)
					{
						Debug.LogError("Detail is null for world " + i);
					}
				}
				else
				{
					loadedWorlds.Add(simSaveFileStructure);
				}
			}
		}

		public void SetIsRunningDebug(bool isDebug)
		{
			foreach (WorldGen world in worlds)
			{
				world.isRunningDebugGen = isDebug;
			}
		}

		public void DEBUG_UpdateSeed(int seed)
		{
			this.seed = seed;
		}
	}
}
