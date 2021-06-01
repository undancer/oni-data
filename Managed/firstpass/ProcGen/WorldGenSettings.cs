using System;
using System.Collections.Generic;

namespace ProcGen
{
	public class WorldGenSettings
	{
		private delegate bool ParserFn<T>(string input, out T res);

		private MutatedWorldData mutatedWorldData;

		public World world => mutatedWorldData.world;

		public static string ClusterDefaultName => DlcManager.IsExpansion1Active() ? "expansion1::clusters/SandstoneStartCluster" : "clusters/SandstoneDefault";

		public WorldGenSettings(string worldName, List<string> traits, bool assertMissingTraits)
		{
			DebugUtil.Assert(SettingsCache.worlds.HasWorld(worldName), "Failed to load world " + worldName);
			World worldData = SettingsCache.worlds.GetWorldData(worldName);
			List<WorldTrait> list = new List<WorldTrait>();
			if (!worldData.disableWorldTraits && traits != null)
			{
				DebugUtil.LogArgs("Generating a world with the traits:", string.Join(", ", traits.ToArray()));
				foreach (string trait in traits)
				{
					WorldTrait cachedTrait = SettingsCache.GetCachedTrait(trait, assertMissingTraits);
					if (cachedTrait != null)
					{
						list.Add(cachedTrait);
					}
				}
			}
			else
			{
				Debug.Log("Generating a world without traits. Either this world has traits disabled or none were specified.");
			}
			mutatedWorldData = new MutatedWorldData(worldData, list);
			Debug.Log("Set world to [" + worldName + "]");
		}

		public BaseLocation GetBaseLocation()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.baseData != null)
			{
				DebugUtil.LogArgs($"World '{world.name}' is overriding baseData");
				return world.defaultsOverrides.baseData;
			}
			return SettingsCache.defaults.baseData;
		}

		public List<string> GetOverworldAddTags()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.overworldAddTags != null)
			{
				DebugUtil.LogArgs($"World '{world.name}' is overriding overworldAddTags");
				return world.defaultsOverrides.overworldAddTags;
			}
			return SettingsCache.defaults.overworldAddTags;
		}

		public List<string> GetDefaultMoveTags()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.defaultMoveTags != null)
			{
				DebugUtil.LogArgs($"World '{world.name}' is overriding defaultMoveTags");
				return world.defaultsOverrides.defaultMoveTags;
			}
			return SettingsCache.defaults.defaultMoveTags;
		}

		public List<StartingWorldElementSetting> GetDefaultStartingElements()
		{
			if (world != null && world.defaultsOverrides != null && world.defaultsOverrides.startingWorldElements != null)
			{
				DebugUtil.LogArgs($"World '{world.name}' is overriding startingWorldElements");
				return world.defaultsOverrides.startingWorldElements;
			}
			return SettingsCache.defaults.startingWorldElements;
		}

		public string[] GetTraitIDs()
		{
			if (mutatedWorldData.traits != null && mutatedWorldData.traits.Count > 0)
			{
				string[] array = new string[mutatedWorldData.traits.Count];
				for (int i = 0; i < mutatedWorldData.traits.Count; i++)
				{
					array[i] = mutatedWorldData.traits[i].filePath;
				}
				return array;
			}
			return new string[0];
		}

		private bool GetSetting<T>(DefaultSettings set, string target, ParserFn<T> parser, out T res)
		{
			if (set == null || set.data == null || !set.data.ContainsKey(target))
			{
				res = default(T);
				return false;
			}
			object obj = set.data[target];
			if (obj.GetType() == typeof(T))
			{
				res = (T)obj;
				return true;
			}
			bool flag = parser(obj as string, out res);
			if (flag)
			{
				set.data[target] = res;
			}
			return flag;
		}

		private T GetSetting<T>(string target, ParserFn<T> parser)
		{
			T res;
			if (world != null)
			{
				if (!GetSetting(world.defaultsOverrides, target, parser, out res))
				{
					GetSetting(SettingsCache.defaults, target, parser, out res);
				}
			}
			else if (!GetSetting(SettingsCache.defaults, target, parser, out res))
			{
				DebugUtil.LogWarningArgs($"Couldn't find setting '{target}' in default settings!");
			}
			return res;
		}

		public bool GetBoolSetting(string target)
		{
			return GetSetting<bool>(target, bool.TryParse);
		}

		private bool TryParseString(string input, out string res)
		{
			res = input;
			return true;
		}

		public string GetStringSetting(string target)
		{
			return GetSetting<string>(target, TryParseString);
		}

		public float GetFloatSetting(string target)
		{
			return GetSetting<float>(target, float.TryParse);
		}

		public int GetIntSetting(string target)
		{
			return GetSetting<int>(target, int.TryParse);
		}

		public E GetEnumSetting<E>(string target) where E : struct
		{
			return GetSetting<E>(target, TryParseEnum);
		}

		private static bool TryParseEnum<E>(string value, out E result) where E : struct
		{
			try
			{
				result = (E)Enum.Parse(typeof(E), value);
				return true;
			}
			catch (Exception)
			{
				result = new E();
			}
			return false;
		}

		public bool HasFeature(string name)
		{
			return mutatedWorldData.features.ContainsKey(name);
		}

		public FeatureSettings GetFeature(string name)
		{
			if (mutatedWorldData.features.ContainsKey(name))
			{
				return mutatedWorldData.features[name];
			}
			throw new Exception("Couldnt get feature from active world data [" + name + "]");
		}

		public FeatureSettings TryGetFeature(string name)
		{
			mutatedWorldData.features.TryGetValue(name, out var value);
			return value;
		}

		public bool HasSubworld(string name)
		{
			return mutatedWorldData.subworlds.ContainsKey(name);
		}

		public SubWorld GetSubWorld(string name)
		{
			if (mutatedWorldData.subworlds.ContainsKey(name))
			{
				return mutatedWorldData.subworlds[name];
			}
			throw new Exception("Couldnt get subworld from active world data [" + name + "]");
		}

		public SubWorld TryGetSubWorld(string name)
		{
			mutatedWorldData.subworlds.TryGetValue(name, out var value);
			return value;
		}

		public List<WeightedSubWorld> GetSubworldsForWorld(List<WeightedSubworldName> subworldList)
		{
			List<WeightedSubWorld> list = new List<WeightedSubWorld>();
			foreach (KeyValuePair<string, SubWorld> subworld in mutatedWorldData.subworlds)
			{
				foreach (WeightedSubworldName subworld2 in subworldList)
				{
					if (subworld.Key == subworld2.name)
					{
						list.Add(new WeightedSubWorld(subworld2.weight, subworld.Value, subworld2.overridePower, subworld2.minCount, subworld2.maxCount, subworld2.priority));
					}
				}
			}
			return list;
		}

		public bool HasMob(string id)
		{
			return mutatedWorldData.mobs.HasMob(id);
		}

		public Mob GetMob(string id)
		{
			return mutatedWorldData.mobs.GetMob(id);
		}

		public ElementBandConfiguration GetElementBandForBiome(string name)
		{
			if (mutatedWorldData.biomes.BiomeBackgroundElementBandConfigurations.TryGetValue(name, out var value))
			{
				return value;
			}
			return null;
		}
	}
}
