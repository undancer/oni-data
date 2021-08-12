using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class WorldTrait
	{
		[Serializable]
		public class ElementBandModifier
		{
			public string element { get; private set; }

			public float massMultiplier { get; private set; }

			public float bandMultiplier { get; private set; }

			public ElementBandModifier()
			{
				massMultiplier = 1f;
				bandMultiplier = 1f;
			}
		}

		public string filePath;

		public string name { get; private set; }

		public string description { get; private set; }

		public string colorHex { get; private set; }

		public List<string> exclusiveWith { get; private set; }

		public MinMax startingBasePositionHorizontalMod { get; private set; }

		public MinMax startingBasePositionVerticalMod { get; private set; }

		public List<WeightedSubworldName> additionalSubworldFiles { get; private set; }

		public List<World.AllowedCellsFilter> additionalUnknownCellFilters { get; private set; }

		public List<World.TemplateSpawnRules> additionalWorldTemplateRules { get; private set; }

		public Dictionary<string, int> globalFeatureTemplateMods { get; private set; }

		public Dictionary<string, int> globalFeatureMods { get; private set; }

		public List<string> removeWorldTemplateRulesById { get; private set; }

		public List<ElementBandModifier> elementBandModifiers { get; private set; }

		public WorldTrait()
		{
			additionalSubworldFiles = new List<WeightedSubworldName>();
			additionalUnknownCellFilters = new List<World.AllowedCellsFilter>();
			additionalWorldTemplateRules = new List<World.TemplateSpawnRules>();
			removeWorldTemplateRulesById = new List<string>();
			globalFeatureTemplateMods = new Dictionary<string, int>();
			globalFeatureMods = new Dictionary<string, int>();
			elementBandModifiers = new List<ElementBandModifier>();
			exclusiveWith = new List<string>();
		}
	}
}
