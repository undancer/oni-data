using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klei;
using ObjectCloner;
using ProcGen.Noise;
using UnityEngine;

namespace ProcGen
{
	public static class SettingsCache
	{
		public static TerrainElementBandSettings biomes = new TerrainElementBandSettings();

		public static Worlds worlds = new Worlds();

		public static ClusterLayouts clusterLayouts = new ClusterLayouts();

		public static NoiseTreeFiles noise = new NoiseTreeFiles();

		private static Dictionary<string, FeatureSettings> featuresettings = new Dictionary<string, FeatureSettings>();

		private static Dictionary<string, WorldTrait> traits = new Dictionary<string, WorldTrait>();

		public static Dictionary<string, SubWorld> subworlds = new Dictionary<string, SubWorld>();

		private static string path = null;

		private static Dictionary<string, BiomeSettings> biomeSettingsCache = new Dictionary<string, BiomeSettings>();

		private const string LAYERS_FILE = "layers";

		private const string RIVERS_FILE = "rivers";

		private const string ROOMS_FILE = "rooms";

		private const string TEMPERATURES_FILE = "temperatures";

		private const string BORDERS_FILE = "borders";

		private const string DEFAULTS_FILE = "defaults";

		private const string MOBS_FILE = "mobs";

		private const string TRAITS_PATH = "traits";

		public static LevelLayerSettings layers
		{
			get;
			private set;
		}

		public static ComposableDictionary<string, River> rivers
		{
			get;
			private set;
		}

		public static ComposableDictionary<string, Room> rooms
		{
			get;
			private set;
		}

		public static ComposableDictionary<Temperature.Range, Temperature> temperatures
		{
			get;
			private set;
		}

		public static ComposableDictionary<string, List<WeightedSimHash>> borders
		{
			get;
			private set;
		}

		public static DefaultSettings defaults
		{
			get;
			set;
		}

		public static MobSettings mobs
		{
			get;
			private set;
		}

		public static string GetPath()
		{
			if (path == null)
			{
				path = FileSystem.Normalize(System.IO.Path.Combine(Application.streamingAssetsPath, "worldgen/"));
			}
			return path;
		}

		public static void CloneInToNewWorld(MutatedWorldData worldData)
		{
			worldData.subworlds = SerializingCloner.Copy(subworlds);
			worldData.features = SerializingCloner.Copy(featuresettings);
			worldData.biomes = SerializingCloner.Copy(biomes);
			worldData.mobs = SerializingCloner.Copy(mobs);
		}

		public static List<string> GetCachedFeatureNames()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, FeatureSettings> featuresetting in featuresettings)
			{
				list.Add(featuresetting.Key);
			}
			return list;
		}

		public static FeatureSettings GetCachedFeature(string name)
		{
			if (featuresettings.ContainsKey(name))
			{
				return featuresettings[name];
			}
			throw new Exception("Couldnt get feature from cache [" + name + "]");
		}

		public static List<string> GetCachedTraitNames()
		{
			return new List<string>(traits.Keys);
		}

		public static WorldTrait GetCachedTrait(string name, bool assertMissingTrait)
		{
			if (traits.ContainsKey(name))
			{
				return traits[name];
			}
			if (assertMissingTrait)
			{
				throw new Exception("Couldnt get trait [" + name + "]");
			}
			Debug.LogWarning("Couldnt get trait [" + name + "]");
			return null;
		}

		public static SubWorld GetCachedSubWorld(string name)
		{
			if (subworlds.ContainsKey(name))
			{
				return subworlds[name];
			}
			throw new Exception("Couldnt get subworld [" + name + "]");
		}

		private static bool GetPathAndName(string srcPath, string srcName, out string name)
		{
			if (FileSystem.FileExists(srcPath + srcName + ".yaml"))
			{
				name = srcName;
				return true;
			}
			string[] array = srcName.Split('/');
			name = array[0];
			for (int i = 1; i < array.Length - 1; i++)
			{
				name = name + "/" + array[i];
			}
			if (FileSystem.FileExists(srcPath + name + ".yaml"))
			{
				return true;
			}
			name = srcName;
			return false;
		}

		private static void LoadBiome(string longName, List<YamlIO.Error> errors)
		{
			string name = "";
			if (!GetPathAndName(GetPath(), longName, out name) || biomeSettingsCache.ContainsKey(name))
			{
				return;
			}
			BiomeSettings biomeSettings = MergeLoad<BiomeSettings>(GetPath() + name + ".yaml", errors);
			if (biomeSettings == null)
			{
				Debug.LogWarning("WorldGen: Attempting to load biome: " + name + " failed");
				return;
			}
			Debug.Assert(biomeSettings.TerrainBiomeLookupTable.Count > 0, longName);
			biomeSettingsCache.Add(name, biomeSettings);
			foreach (KeyValuePair<string, ElementBandConfiguration> item in biomeSettings.TerrainBiomeLookupTable)
			{
				string key = name + "/" + item.Key;
				if (!biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(key))
				{
					biomes.BiomeBackgroundElementBandConfigurations.Add(key, item.Value);
				}
			}
		}

		private static string LoadFeature(string longName, List<YamlIO.Error> errors)
		{
			string name = "";
			if (!GetPathAndName(GetPath(), longName, out name))
			{
				Debug.LogWarning("LoadFeature GetPathAndName: Attempting to load feature: " + name + " failed");
				return longName;
			}
			if (!featuresettings.ContainsKey(name))
			{
				FeatureSettings featureSettings = YamlIO.LoadFile<FeatureSettings>(GetPath() + name + ".yaml");
				if (featureSettings != null)
				{
					featuresettings.Add(name, featureSettings);
					if (featureSettings.forceBiome != null)
					{
						LoadBiome(featureSettings.forceBiome, errors);
						DebugUtil.Assert(biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(featureSettings.forceBiome), longName, "(feature) referenced a missing biome named", featureSettings.forceBiome);
					}
				}
				else
				{
					Debug.LogWarning("WorldGen: Attempting to load feature: " + name + " failed");
				}
			}
			return name;
		}

		public static void LoadFeatures(Dictionary<string, int> features, List<YamlIO.Error> errors)
		{
			foreach (KeyValuePair<string, int> feature in features)
			{
				LoadFeature(feature.Key, errors);
			}
		}

		public static void LoadSubworlds(List<WeightedSubworldName> subworlds, List<YamlIO.Error> errors)
		{
			foreach (WeightedSubworldName subworld in subworlds)
			{
				SubWorld subWorld = null;
				string text = subworld.name;
				if (subworld.overrideName != null && subworld.overrideName.Length > 0)
				{
					text = subworld.overrideName;
				}
				if (SettingsCache.subworlds.ContainsKey(text))
				{
					continue;
				}
				SubWorld subWorld2 = YamlIO.LoadFile<SubWorld>(path + subworld.name + ".yaml");
				if (subWorld2 != null)
				{
					subWorld = subWorld2;
					subWorld.name = text;
					subWorld.EnforceFeatureSpawnRuleSelfConsistency();
					SettingsCache.subworlds[text] = subWorld;
					noise.LoadTree(subWorld.biomeNoise, path);
					noise.LoadTree(subWorld.densityNoise, path);
					noise.LoadTree(subWorld.overrideNoise, path);
				}
				else
				{
					Debug.LogWarning("WorldGen: Attempting to load subworld: " + subworld.name + " failed");
				}
				if (subWorld.centralFeature != null)
				{
					subWorld.centralFeature.type = LoadFeature(subWorld.centralFeature.type, errors);
				}
				foreach (WeightedBiome biome in subWorld.biomes)
				{
					LoadBiome(biome.name, errors);
					DebugUtil.Assert(biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(biome.name), subWorld.name, "(subworld) referenced a missing biome named", biome.name);
				}
				DebugUtil.Assert(subWorld.features != null, "Features list for subworld", subWorld.name, "was null! Either remove it from the .yaml or set it to the empty list []");
				foreach (Feature feature in subWorld.features)
				{
					feature.type = LoadFeature(feature.type, errors);
				}
			}
		}

		public static void LoadWorldTraits(List<YamlIO.Error> errors)
		{
			List<FileHandle> list = new List<FileHandle>();
			FileSystem.GetFiles(FileSystem.Normalize(System.IO.Path.Combine(path, "traits")), "*.yaml", list);
			foreach (FileHandle item in list)
			{
				LoadWorldTrait(item, errors);
			}
		}

		public static void LoadWorldTrait(FileHandle file, List<YamlIO.Error> errors)
		{
			WorldTrait worldTrait = YamlIO.LoadFile<WorldTrait>(file, delegate(YamlIO.Error error, bool force_log_as_warning)
			{
				errors.Add(error);
			});
			int num = FirstUncommonCharacter(path, file.full_path);
			string text = ((num > -1) ? file.full_path.Substring(num) : file.full_path);
			text = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(text), System.IO.Path.GetFileNameWithoutExtension(text));
			text = text.Replace('\\', '/');
			if (worldTrait == null)
			{
				DebugUtil.LogWarningArgs("Failed to load trait: ", text);
			}
			else
			{
				traits[text] = worldTrait;
				worldTrait.filePath = text;
			}
		}

		public static List<string> GetWorldNames()
		{
			return worlds.GetNames();
		}

		public static void Save(string path)
		{
			YamlIO.Save(layers, path + "layers.yaml");
			YamlIO.Save(rivers, path + "rivers.yaml");
			YamlIO.Save(rooms, path + "rooms.yaml");
			YamlIO.Save(temperatures, path + "temperatures.yaml");
			YamlIO.Save(borders, path + "borders.yaml");
			YamlIO.Save(defaults, path + "defaults.yaml");
			YamlIO.Save(mobs, path + "mobs.yaml");
		}

		public static void Clear()
		{
			worlds.worldCache.Clear();
			layers = null;
			biomes.BiomeBackgroundElementBandConfigurations.Clear();
			biomeSettingsCache.Clear();
			rivers = null;
			rooms = null;
			temperatures = null;
			borders = null;
			noise.Clear();
			defaults = null;
			mobs = null;
			featuresettings.Clear();
			traits.Clear();
			subworlds.Clear();
			clusterLayouts.clusterCache.Clear();
			DebugUtil.LogArgs("World Settings cleared!");
		}

		private static T MergeLoad<T>(string filename, List<YamlIO.Error> errors) where T : class, IMerge<T>, new()
		{
			ListPool<FileHandle, WorldGenSettings>.PooledList pooledList = ListPool<FileHandle, WorldGenSettings>.Allocate();
			FileSystem.GetFiles(filename, pooledList);
			if (pooledList.Count == 0)
			{
				pooledList.Recycle();
				throw new Exception($"File not found in any file system: {filename}");
			}
			pooledList.Reverse();
			ListPool<T, WorldGenSettings>.PooledList pooledList2 = ListPool<T, WorldGenSettings>.Allocate();
			pooledList2.Add(new T());
			foreach (FileHandle item in pooledList)
			{
				T val = YamlIO.LoadFile<T>(item, delegate(YamlIO.Error error, bool force_log_as_warning)
				{
					errors.Add(error);
				});
				if (val != null)
				{
					pooledList2.Add(val);
				}
			}
			pooledList.Recycle();
			T val2 = pooledList2[0];
			for (int i = 1; i != pooledList2.Count; i++)
			{
				val2.Merge(pooledList2[i]);
			}
			pooledList2.Recycle();
			return val2;
		}

		private static int FirstUncommonCharacter(string a, string b)
		{
			int num = Mathf.Min(a.Length, b.Length);
			int num2 = -1;
			while (++num2 < num)
			{
				if (a[num2] != b[num2])
				{
					return num2;
				}
			}
			return num2;
		}

		public static bool LoadFiles(List<YamlIO.Error> errors)
		{
			if (worlds.worldCache.Count > 0)
			{
				return false;
			}
			clusterLayouts.LoadFiles(GetPath(), errors);
			HashSet<string> referencedWorlds = new HashSet<string>(from worldPlacment in clusterLayouts.clusterCache.Values.SelectMany((ClusterLayout clusterLayout) => clusterLayout.worldPlacements)
				select worldPlacment.world.Substring("worlds/".Length));
			worlds.LoadReferencedWorlds(GetPath(), referencedWorlds, errors);
			LoadWorldTraits(errors);
			foreach (KeyValuePair<string, World> item in worlds.worldCache)
			{
				LoadFeatures(item.Value.globalFeatures, errors);
				LoadSubworlds(item.Value.subworldFiles, errors);
			}
			foreach (KeyValuePair<string, WorldTrait> trait in traits)
			{
				LoadFeatures(trait.Value.globalFeatureMods, errors);
				LoadSubworlds(trait.Value.additionalSubworldFiles, errors);
			}
			layers = MergeLoad<LevelLayerSettings>(GetPath() + "layers.yaml", errors);
			layers.LevelLayers.ConvertBandSizeToMaxSize();
			rivers = MergeLoad<ComposableDictionary<string, River>>(GetPath() + "rivers.yaml", errors);
			rooms = MergeLoad<ComposableDictionary<string, Room>>(path + "rooms.yaml", errors);
			foreach (KeyValuePair<string, Room> room in rooms)
			{
				room.Value.name = room.Key;
			}
			temperatures = MergeLoad<ComposableDictionary<Temperature.Range, Temperature>>(GetPath() + "temperatures.yaml", errors);
			borders = MergeLoad<ComposableDictionary<string, List<WeightedSimHash>>>(GetPath() + "borders.yaml", errors);
			defaults = YamlIO.LoadFile<DefaultSettings>(GetPath() + "defaults.yaml");
			mobs = MergeLoad<MobSettings>(GetPath() + "mobs.yaml", errors);
			foreach (KeyValuePair<string, Mob> item2 in mobs.MobLookupTable)
			{
				item2.Value.name = item2.Key;
			}
			DebugUtil.LogArgs("World settings reload complete!");
			return true;
		}

		public static List<string> GetRandomTraits(int seed)
		{
			System.Random random = new System.Random(seed);
			int num = random.Next(2, 5);
			List<string> list = new List<string>(traits.Keys);
			list.Sort();
			List<string> list2 = new List<string>();
			while (list2.Count < num && list.Count > 0)
			{
				int index = random.Next(list.Count);
				string text = list[index];
				bool flag = false;
				foreach (string item in GetCachedTrait(text, assertMissingTrait: true).exclusiveWith)
				{
					if (list2.Contains(item))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list2.Add(text);
				}
				list.RemoveAt(index);
			}
			return list2;
		}
	}
}
