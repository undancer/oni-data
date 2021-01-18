using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

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
		public class FeatureSpawnRules
		{
			public enum ListRule
			{
				GuaranteeOne,
				GuaranteeSome,
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

			public List<AllowedCellsFilter> allowedCellsFilter
			{
				get;
				private set;
			}

			public FeatureSpawnRules()
			{
				times = 1;
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
				SymmetricExceptWith
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

		public string coordinatePrefix
		{
			get;
			private set;
		}

		public string asteroidType
		{
			get;
			private set;
		}

		public int difficulty
		{
			get;
			private set;
		}

		public int tier
		{
			get;
			private set;
		}

		public bool disableWorldTraits
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

		public Dictionary<string, int> globalFeatureTemplates
		{
			get;
			private set;
		}

		public Dictionary<string, int> globalFeatures
		{
			get;
			private set;
		}

		public List<FeatureSpawnRules> worldFeatureRules
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
			globalFeatureTemplates = new Dictionary<string, int>();
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

		public string GetCoordinatePrefix()
		{
			if (string.IsNullOrEmpty(coordinatePrefix))
			{
				string text = "";
				string[] array = Strings.Get(name).String.Split(' ');
				int a = 5 - array.Length;
				bool flag = true;
				string[] array2 = array;
				foreach (string input in array2)
				{
					if (!flag)
					{
						text += "-";
					}
					string text2 = Regex.Replace(input, "(a|e|i|o|u)", "");
					text += text2.Substring(0, Mathf.Min(a, text2.Length)).ToUpper();
					flag = false;
				}
				coordinatePrefix = text;
			}
			return coordinatePrefix;
		}
	}
}
