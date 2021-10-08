using System;
using System.Collections.Generic;
using UnityEngine;

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

		private TagSet m_traitTagSet;

		public string name { get; private set; }

		public string description { get; private set; }

		public string colorHex { get; private set; }

		public List<string> forbiddenDLCIds { get; private set; }

		public List<string> exclusiveWith { get; private set; }

		public List<string> exclusiveWithTags { get; private set; }

		public List<string> traitTags { get; private set; }

		public MinMax startingBasePositionHorizontalMod { get; private set; }

		public MinMax startingBasePositionVerticalMod { get; private set; }

		public List<WeightedSubworldName> additionalSubworldFiles { get; private set; }

		public List<World.AllowedCellsFilter> additionalUnknownCellFilters { get; private set; }

		public List<World.TemplateSpawnRules> additionalWorldTemplateRules { get; private set; }

		public Dictionary<string, int> globalFeatureTemplateMods { get; private set; }

		public Dictionary<string, int> globalFeatureMods { get; private set; }

		public List<string> removeWorldTemplateRulesById { get; private set; }

		public List<ElementBandModifier> elementBandModifiers { get; private set; }

		public TagSet traitTagsSet
		{
			get
			{
				if (m_traitTagSet == null)
				{
					m_traitTagSet = new TagSet(traitTags);
				}
				return m_traitTagSet;
			}
		}

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
			exclusiveWithTags = new List<string>();
			forbiddenDLCIds = new List<string>();
		}

		public bool IsValid(World world, bool logErrors)
		{
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<string, int> globalFeatureMod in globalFeatureMods)
			{
				num += globalFeatureMod.Value;
				num2 += Mathf.FloorToInt(world.worldTraitScale * (float)globalFeatureMod.Value);
			}
			if (globalFeatureMods.Count > 0 && num2 == 0)
			{
				if (logErrors)
				{
					DebugUtil.LogWarningArgs("Trait '" + filePath + "' cannot be applied to world '" + world.name + "' due to globalFeatureMods and worldTraitScale resulting in no features being generated.");
				}
				return false;
			}
			return true;
		}
	}
}
