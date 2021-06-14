using System;
using System.Collections.Generic;
using System.Linq;
using Klei;

namespace ProcGen
{
	[Serializable]
	public class World
	{
		public enum WorldCategory
		{
			Asteroid,
			Moon
		}

		public enum Skip
		{
			Never = 0,
			False = 0,
			Always = 99,
			True = 99,
			EditorOnly = 100
		}

		public enum LayoutMethod
		{
			Default = 0,
			VoronoiTree = 0,
			PowerTree = 1
		}

		[Serializable]
		public class TemplateSpawnRules
		{
			public enum ListRule
			{
				GuaranteeOne,
				GuaranteeSome,
				GuaranteeSomeTryMore,
				GuaranteeAll,
				TryOne,
				TrySome,
				TryAll
			}

			public List<string> names
			{
				get;
				private set;
			}

			public ListRule listRule
			{
				get;
				private set;
			}

			public int someCount
			{
				get;
				private set;
			}

			public int moreCount
			{
				get;
				private set;
			}

			public int times
			{
				get;
				private set;
			}

			public float priority
			{
				get;
				private set;
			}

			public bool allowDuplicates
			{
				get;
				private set;
			}

			public bool allowExtremeTemperatureOverlap
			{
				get;
				private set;
			}

			public List<AllowedCellsFilter> allowedCellsFilter
			{
				get;
				private set;
			}

			public TemplateSpawnRules()
			{
				times = 1;
				allowedCellsFilter = new List<AllowedCellsFilter>();
				allowDuplicates = false;
			}

			public bool IsGuaranteeRule()
			{
				return listRule switch
				{
					ListRule.GuaranteeOne => true, 
					ListRule.GuaranteeSome => true, 
					ListRule.GuaranteeSomeTryMore => true, 
					ListRule.GuaranteeAll => true, 
					_ => false, 
				};
			}
		}

		[Serializable]
		public class AllowedCellsFilter
		{
			public enum TagCommand
			{
				Default,
				AtTag,
				NotAtTag,
				DistanceFromTag
			}

			public enum Command
			{
				Clear,
				Replace,
				UnionWith,
				IntersectWith,
				ExceptWith,
				SymmetricExceptWith,
				All
			}

			public TagCommand tagcommand
			{
				get;
				private set;
			}

			public string tag
			{
				get;
				private set;
			}

			public int minDistance
			{
				get;
				private set;
			}

			public int maxDistance
			{
				get;
				private set;
			}

			public Command command
			{
				get;
				private set;
			}

			public List<Temperature.Range> temperatureRanges
			{
				get;
				private set;
			}

			public List<SubWorld.ZoneType> zoneTypes
			{
				get;
				private set;
			}

			public List<string> subworldNames
			{
				get;
				private set;
			}

			public AllowedCellsFilter()
			{
				temperatureRanges = new List<Temperature.Range>();
				zoneTypes = new List<SubWorld.ZoneType>();
				subworldNames = new List<string>();
				command = Command.Replace;
			}

			public void Validate(string parentFile, List<WeightedSubworldName> parentCachedFiles)
			{
				if (subworldNames == null)
				{
					return;
				}
				foreach (string subworld in subworldNames)
				{
					DebugUtil.DevAssert(parentCachedFiles.Any((WeightedSubworldName val) => val.name == subworld), "World " + parentFile + ": should include " + subworld + " in its subworldFiles since it's used in a command");
					DebugUtil.DevAssert(FileSystem.FileExists(SettingsCache.RewriteWorldgenPathYaml(subworld)), "World " + parentFile + ": Incorrect subworldFile " + subworld);
				}
			}
		}

		public string filePath;

		public string name
		{
			get;
			private set;
		}

		public string description
		{
			get;
			private set;
		}

		public string nameTable
		{
			get;
			private set;
		}

		public bool disableWorldTraits
		{
			get;
			private set;
		}

		public string asteroidIcon
		{
			get;
			private set;
		}

		public Skip skip
		{
			get;
			private set;
		}

		public bool moduleInterior
		{
			get;
			private set;
		}

		public WorldCategory category
		{
			get;
			private set;
		}

		public Vector2I worldsize
		{
			get;
			private set;
		}

		public DefaultSettings defaultsOverrides
		{
			get;
			private set;
		}

		public LayoutMethod layoutMethod
		{
			get;
			private set;
		}

		public List<WeightedSubworldName> subworldFiles
		{
			get;
			private set;
		}

		public List<AllowedCellsFilter> unknownCellsAllowedSubworlds
		{
			get;
			private set;
		}

		public string startSubworldName
		{
			get;
			private set;
		}

		public string startingBaseTemplate
		{
			get;
			set;
		}

		public MinMax startingBasePositionHorizontal
		{
			get;
			private set;
		}

		public MinMax startingBasePositionVertical
		{
			get;
			private set;
		}

		public Dictionary<string, int> globalFeatures
		{
			get;
			private set;
		}

		public List<TemplateSpawnRules> worldTemplateRules
		{
			get;
			private set;
		}

		public List<string> seasons
		{
			get;
			private set;
		}

		public bool adjacentTemporalTear
		{
			get;
			private set;
		}

		public World()
		{
			subworldFiles = new List<WeightedSubworldName>();
			unknownCellsAllowedSubworlds = new List<AllowedCellsFilter>();
			startingBasePositionHorizontal = new MinMax(0.5f, 0.5f);
			startingBasePositionVertical = new MinMax(0.5f, 0.5f);
			globalFeatures = new Dictionary<string, int>();
			seasons = new List<string>();
			category = WorldCategory.Asteroid;
		}

		public void ModStartLocation(MinMax hMod, MinMax vMod)
		{
			MinMax startingBasePositionHorizontal = this.startingBasePositionHorizontal;
			MinMax startingBasePositionVertical = this.startingBasePositionVertical;
			startingBasePositionHorizontal.Mod(hMod);
			startingBasePositionVertical.Mod(vMod);
			this.startingBasePositionHorizontal = startingBasePositionHorizontal;
			this.startingBasePositionVertical = startingBasePositionVertical;
		}

		public void Validate()
		{
			if (unknownCellsAllowedSubworlds == null)
			{
				return;
			}
			List<string> usedSubworldFiles = new List<string>();
			subworldFiles.ForEach(delegate(WeightedSubworldName x)
			{
				usedSubworldFiles.Add(x.name);
			});
			foreach (AllowedCellsFilter unknownCellsAllowedSubworld in unknownCellsAllowedSubworlds)
			{
				unknownCellsAllowedSubworld.Validate(name, subworldFiles);
				if (unknownCellsAllowedSubworld.subworldNames == null)
				{
					continue;
				}
				foreach (string subworldName in unknownCellsAllowedSubworld.subworldNames)
				{
					usedSubworldFiles.Remove(subworldName);
				}
			}
			usedSubworldFiles.Remove(startSubworldName);
			if (usedSubworldFiles.Count > 0)
			{
				DebugUtil.LogWarningArgs("World " + name + ": defines subworldNames that are not used in unknownCellsAllowedSubworlds: \n" + string.Join(", ", usedSubworldFiles));
			}
		}
	}
}
