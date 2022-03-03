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
		public static ClusterLayouts clusterLayouts = new ClusterLayouts();

		public static Worlds worlds = new Worlds();

		public static Dictionary<string, SubWorld> subworlds = new Dictionary<string, SubWorld>();

		public static TerrainElementBandSettings biomes = new TerrainElementBandSettings();

		public static NoiseTreeFiles noise = new NoiseTreeFiles();

		private static Dictionary<string, FeatureSettings> featureSettings = new Dictionary<string, FeatureSettings>();

		private static Dictionary<string, WorldTrait> traits = new Dictionary<string, WorldTrait>();

		private static Dictionary<string, BiomeSettings> biomeSettingsCache = new Dictionary<string, BiomeSettings>();

		private static string[] s_sourceDelimiter = new string[1] { "::" };

		private static Dictionary<string, string> s_cachedPaths = new Dictionary<string, string>();

		private const string LAYERS_FILE = "layers";

		private const string RIVERS_FILE = "rivers";

		private const string ROOMS_FILE = "rooms";

		private const string TEMPERATURES_FILE = "temperatures";

		private const string BORDERS_FILE = "borders";

		private const string DEFAULTS_FILE = "defaults";

		private const string MOBS_FILE = "mobs";

		private const string TRAITS_PATH = "traits";

		public static LevelLayerSettings layers { get; private set; }

		public static ComposableDictionary<string, River> rivers { get; private set; }

		public static ComposableDictionary<string, Room> rooms { get; private set; }

		public static ComposableDictionary<Temperature.Range, Temperature> temperatures { get; private set; }

		public static ComposableDictionary<string, List<WeightedSimHash>> borders { get; private set; }

		public static DefaultSettings defaults { get; set; }

		public static MobSettings mobs { get; private set; }

		public static string GetAbsoluteContentPath(string dlcId, string optionalSubpath = "")
		{
			if (!s_cachedPaths.TryGetValue(dlcId, out var value))
			{
				if (dlcId == "")
				{
					value = FileSystem.Normalize(System.IO.Path.Combine(Application.streamingAssetsPath));
				}
				else
				{
					string contentDirectoryName = DlcManager.GetContentDirectoryName(dlcId);
					value = FileSystem.Normalize(System.IO.Path.Combine(Application.streamingAssetsPath, "dlc", contentDirectoryName));
				}
				s_cachedPaths[dlcId] = value;
			}
			return FileSystem.Normalize(System.IO.Path.Combine(value, optionalSubpath));
		}

		public static string RewriteWorldgenPath(string scopePath)
		{
			GetDlcIdAndPath(scopePath, out var dlcId, out var path);
			return GetAbsoluteContentPath(dlcId, "worldgen/" + path);
		}

		public static string RewriteWorldgenPathYaml(string scopePath)
		{
			return RewriteWorldgenPath(scopePath) + ".yaml";
		}

		public static string GetScope(string dlcId)
		{
			if (dlcId == "")
			{
				return "";
			}
			return DlcManager.GetContentDirectoryName(dlcId) + "::";
		}

		public static void GetDlcIdAndPath(string scopePath, out string dlcId, out string path)
		{
			string[] array = scopePath.Split(s_sourceDelimiter, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 1 && scopePath.EndsWith("::"))
			{
				dlcId = DlcManager.GetDlcIdFromContentDirectory(array[0]);
				path = "";
			}
			else if (array.Length > 1)
			{
				dlcId = DlcManager.GetDlcIdFromContentDirectory(array[0]);
				path = array[1];
			}
			else
			{
				dlcId = "";
				path = scopePath;
			}
		}

		public static string GuessScopedPath(string path)
		{
			foreach (string item in DlcManager.RELEASE_ORDER)
			{
				if (DlcManager.IsContentActive(item))
				{
					string absoluteContentPath = GetAbsoluteContentPath(item, "worldgen/");
					if (path.StartsWith(absoluteContentPath))
					{
						return GetScope(item) + path.Substring(absoluteContentPath.Length);
					}
				}
			}
			return null;
		}

		public static void CloneInToNewWorld(MutatedWorldData worldData)
		{
			worldData.subworlds = SerializingCloner.Copy(subworlds);
			worldData.features = SerializingCloner.Copy(featureSettings);
			worldData.biomes = SerializingCloner.Copy(biomes);
			worldData.mobs = SerializingCloner.Copy(mobs);
		}

		public static List<string> GetCachedFeatureNames()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, FeatureSettings> featureSetting in featureSettings)
			{
				list.Add(featureSetting.Key);
			}
			return list;
		}

		public static FeatureSettings GetCachedFeature(string name)
		{
			if (featureSettings.ContainsKey(name))
			{
				return featureSettings[name];
			}
			throw new Exception("Couldnt get feature from cache [" + name + "]");
		}

		public static List<string> GetCachedTraitNames()
		{
			return new List<string>(traits.Keys);
		}

		public static List<WorldTrait> GetCachedTraits()
		{
			return new List<WorldTrait>(traits.Values);
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

		private static void SplitNameFromPath(string scopePath, out string path, out string name)
		{
			int num = scopePath.LastIndexOf('/');
			name = scopePath.Substring(num + 1);
			path = scopePath.Substring(0, num);
		}

		private static bool LoadBiome(string longName, List<YamlIO.Error> errors)
		{
			SplitNameFromPath(longName, out var path, out var name);
			if (biomeSettingsCache.ContainsKey(path))
			{
				return true;
			}
			string filename = RewriteWorldgenPathYaml(path);
			BiomeSettings biomeSettings = MergeLoad<BiomeSettings>(null, filename, errors);
			if (biomeSettings == null)
			{
				Debug.LogWarning("WorldGen: Attempting to load biome: " + name + " failed");
				return false;
			}
			Debug.Assert(biomeSettings.TerrainBiomeLookupTable.Count > 0, "Worldgen: TerrainBiomeLookupTable is empty: " + longName);
			biomeSettingsCache.Add(path, biomeSettings);
			foreach (KeyValuePair<string, ElementBandConfiguration> item in biomeSettings.TerrainBiomeLookupTable)
			{
				string key = path + "/" + item.Key;
				if (!biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(key))
				{
					biomes.BiomeBackgroundElementBandConfigurations.Add(key, item.Value);
				}
			}
			return true;
		}

		private static string LoadFeature(string longName, List<YamlIO.Error> errors)
		{
			if (SettingsCache.featureSettings.ContainsKey(longName))
			{
				return longName;
			}
			FeatureSettings featureSettings = YamlIO.LoadFile<FeatureSettings>(RewriteWorldgenPathYaml(longName));
			if (featureSettings != null)
			{
				SettingsCache.featureSettings.Add(longName, featureSettings);
				if (featureSettings.forceBiome != null)
				{
					DebugUtil.Assert(LoadBiome(featureSettings.forceBiome, errors), longName, "(feature) referenced a missing biome named", featureSettings.forceBiome);
				}
			}
			else
			{
				Debug.LogWarning("WorldGen: Attempting to load feature: " + longName + " failed");
			}
			return longName;
		}

		public static void LoadFeatures(Dictionary<string, int> features, List<YamlIO.Error> errors)
		{
			foreach (KeyValuePair<string, int> feature in features)
			{
				LoadFeature(feature.Key, errors);
			}
		}

		public static void LoadSubworlds(List<WeightedSubworldName> subworlds, string prefix, List<YamlIO.Error> errors)
		{
			foreach (WeightedSubworldName subworld in subworlds)
			{
				SubWorld subWorld = null;
				string text = subworld.name;
				if (subworld.overrideName != null && subworld.overrideName.Length > 0)
				{
					text = subworld.overrideName;
				}
				SubWorld subWorld2 = YamlIO.LoadFile<SubWorld>(RewriteWorldgenPathYaml(text));
				if (subWorld2 != null)
				{
					subWorld = subWorld2;
					subWorld.name = text;
					subWorld.EnforceTemplateSpawnRuleSelfConsistency();
					SettingsCache.subworlds[text] = subWorld;
					noise.LoadTree(subWorld.biomeNoise);
					noise.LoadTree(subWorld.densityNoise);
					noise.LoadTree(subWorld.overrideNoise);
				}
				else
				{
					Debug.LogWarning("WorldGen: Attempting to load subworld: " + text + " failed");
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

		public static void LoadWorldTraits(string path, string prefix, List<YamlIO.Error> errors)
		{
			List<FileHandle> list = new List<FileHandle>();
			FileSystem.GetFiles(FileSystem.Normalize(System.IO.Path.Combine(path, "traits")), "*.yaml", list);
			list.Sort((FileHandle s1, FileHandle s2) => string.Compare(s1.full_path, s2.full_path, StringComparison.OrdinalIgnoreCase));
			foreach (FileHandle item in list)
			{
				LoadWorldTrait(item, path, prefix, errors);
			}
		}

		public static void LoadWorldTrait(FileHandle file, string path, string prefix, List<YamlIO.Error> errors)
		{
			WorldTrait worldTrait = YamlIO.LoadFile<WorldTrait>(file, delegate(YamlIO.Error error, bool force_log_as_warning)
			{
				errors.Add(error);
			});
			if (worldTrait.forbiddenDLCIds != null)
			{
				foreach (string forbiddenDLCId in worldTrait.forbiddenDLCIds)
				{
					if (DlcManager.IsContentActive(forbiddenDLCId))
					{
						return;
					}
				}
			}
			int num = FirstUncommonCharacter(path, file.full_path);
			string path2 = ((num > -1) ? file.full_path.Substring(num) : file.full_path);
			path2 = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path2), System.IO.Path.GetFileNameWithoutExtension(path2));
			path2 = path2.Replace('\\', '/');
			path2 = prefix + path2;
			if (worldTrait == null)
			{
				DebugUtil.LogWarningArgs("Failed to load trait: ", path2);
			}
			else
			{
				worldTrait.filePath = path2;
				DebugUtil.DevAssert(!traits.ContainsKey(path2), "Overwriting trait " + path2 + " already exists");
				traits[path2] = worldTrait;
			}
		}

		public static List<string> GetWorldNames()
		{
			return worlds.GetNames();
		}

		public static List<string> GetClusterNames()
		{
			return clusterLayouts.GetNames();
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
			featureSettings.Clear();
			traits.Clear();
			subworlds.Clear();
			clusterLayouts.clusterCache.Clear();
			DebugUtil.LogArgs("World Settings cleared!");
		}

		private static T MergeLoad<T>(T existing, string filename, List<YamlIO.Error> errors) where T : class, IMerge<T>, new()
		{
			ListPool<FileHandle, WorldGenSettings>.PooledList pooledList = ListPool<FileHandle, WorldGenSettings>.Allocate();
			FileSystem.GetFiles(filename, pooledList);
			if (pooledList.Count == 0)
			{
				pooledList.Recycle();
				if (existing != null)
				{
					return existing;
				}
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
			if (existing != null)
			{
				return existing.Merge(val2);
			}
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
			defaults = YamlIO.LoadFile<DefaultSettings>(GetAbsoluteContentPath("", "worldgen/") + "defaults.yaml");
			foreach (string item in DlcManager.RELEASE_ORDER)
			{
				if (DlcManager.IsContentActive(item))
				{
					LoadFiles(GetAbsoluteContentPath(item, "worldgen/"), GetScope(item), errors);
				}
			}
			worlds.Validate();
			DebugUtil.LogArgs("World settings reload complete!");
			return true;
		}

		private static bool LoadFiles(string worldgenFolderPath, string addPrefix, List<YamlIO.Error> errors)
		{
			clusterLayouts.LoadFiles(worldgenFolderPath, addPrefix, errors);
			HashSet<string> referencedWorlds = new HashSet<string>(from worldPlacment in clusterLayouts.clusterCache.Values.SelectMany((ClusterLayout clusterLayout) => clusterLayout.worldPlacements)
				select worldPlacment.world);
			worlds.LoadReferencedWorlds(worldgenFolderPath, addPrefix, referencedWorlds, errors);
			LoadWorldTraits(worldgenFolderPath, addPrefix, errors);
			foreach (KeyValuePair<string, World> item in worlds.worldCache)
			{
				LoadFeatures(item.Value.globalFeatures, errors);
				LoadSubworlds(item.Value.subworldFiles, addPrefix, errors);
			}
			foreach (KeyValuePair<string, WorldTrait> trait in traits)
			{
				LoadFeatures(trait.Value.globalFeatureMods, errors);
				LoadSubworlds(trait.Value.additionalSubworldFiles, addPrefix, errors);
			}
			layers = MergeLoad(layers, worldgenFolderPath + "layers.yaml", errors);
			layers.LevelLayers.ConvertBandSizeToMaxSize();
			rivers = MergeLoad(rivers, worldgenFolderPath + "rivers.yaml", errors);
			rooms = MergeLoad(rooms, worldgenFolderPath + "rooms.yaml", errors);
			foreach (KeyValuePair<string, Room> room in rooms)
			{
				room.Value.name = room.Key;
			}
			temperatures = MergeLoad(temperatures, worldgenFolderPath + "temperatures.yaml", errors);
			borders = MergeLoad(borders, worldgenFolderPath + "borders.yaml", errors);
			mobs = MergeLoad(mobs, worldgenFolderPath + "mobs.yaml", errors);
			foreach (KeyValuePair<string, Mob> item2 in mobs.MobLookupTable)
			{
				item2.Value.name = item2.Key;
			}
			return true;
		}

		public static List<string> GetRandomTraits(int seed, World world)
		{
			if (world.disableWorldTraits || world.worldTraitRules == null || seed == 0)
			{
				return new List<string>();
			}
			KRandom kRandom = new KRandom(seed);
			List<WorldTrait> list = new List<WorldTrait>(traits.Values);
			List<WorldTrait> list2 = new List<WorldTrait>();
			TagSet tagSet = new TagSet();
			foreach (World.TraitRule rule in world.worldTraitRules)
			{
				if (rule.specificTraits != null)
				{
					foreach (string specificTrait in rule.specificTraits)
					{
						list2.Add(traits[specificTrait]);
					}
				}
				List<WorldTrait> list3 = new List<WorldTrait>(list);
				TagSet requiredTags = ((rule.requiredTags != null) ? new TagSet(rule.requiredTags) : null);
				TagSet forbiddenTags = ((rule.forbiddenTags != null) ? new TagSet(rule.forbiddenTags) : null);
				list3.RemoveAll((WorldTrait trait) => (requiredTags != null && !trait.traitTagsSet.ContainsAll(requiredTags)) || (forbiddenTags != null && trait.traitTagsSet.ContainsOne(forbiddenTags)) || (rule.forbiddenTraits != null && rule.forbiddenTraits.Contains(trait.filePath)) || !trait.IsValid(world, logErrors: true));
				int num = kRandom.Next(rule.min, Mathf.Max(rule.min, rule.max + 1));
				int count = list2.Count;
				while (list2.Count < count + num && list3.Count > 0)
				{
					int index = kRandom.Next(list3.Count);
					WorldTrait worldTrait = list3[index];
					bool flag = false;
					foreach (string exclusiveId in worldTrait.exclusiveWith)
					{
						if (list2.Find((WorldTrait t) => t.filePath == exclusiveId) != null)
						{
							flag = true;
							break;
						}
					}
					foreach (string exclusiveWithTag in worldTrait.exclusiveWithTags)
					{
						if (tagSet.Contains(exclusiveWithTag))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list2.Add(worldTrait);
						list.Remove(worldTrait);
						foreach (string exclusiveWithTag2 in worldTrait.exclusiveWithTags)
						{
							tagSet.Add(exclusiveWithTag2);
						}
					}
					list3.RemoveAt(index);
				}
				if (list2.Count != count + num)
				{
					Debug.LogWarning($"TraitRule on {world.name} tried to generate {num} but only generated {list2.Count - count}");
				}
			}
			List<string> list4 = new List<string>();
			foreach (WorldTrait item in list2)
			{
				list4.Add(item.filePath);
			}
			DebugUtil.LogArgs("Getting traits for seed", seed, string.Join(", ", list4.ToArray()));
			return list4;
		}
	}
}
