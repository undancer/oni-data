using System;
using System.Collections.Generic;
using KSerialization.Converters;

namespace ProcGen
{
	[Serializable]
	public class SubWorld : SampleDescriber
	{
		public enum ZoneType
		{
			FrozenWastes,
			CrystalCaverns,
			BoggyMarsh,
			Sandstone,
			ToxicJungle,
			MagmaCore,
			OilField,
			Space,
			Ocean,
			Rust,
			Forest,
			Radioactive,
			Swamp,
			Wasteland,
			RocketInterior,
			Metallic,
			Barren,
			Moo
		}

		public string nameKey { get; protected set; }

		public string descriptionKey { get; protected set; }

		public string utilityKey { get; protected set; }

		public string biomeNoise { get; protected set; }

		public string overrideNoise { get; protected set; }

		public string densityNoise { get; protected set; }

		public string borderOverride { get; protected set; }

		public int borderOverridePriority { get; protected set; }

		public MinMax borderSizeOverride { get; protected set; }

		[StringEnumConverter]
		public Temperature.Range temperatureRange { get; protected set; }

		public Feature centralFeature { get; protected set; }

		public List<Feature> features { get; protected set; }

		public Override overrides { get; protected set; }

		public List<string> tags { get; protected set; }

		public int minChildCount { get; protected set; }

		public bool singleChildCount { get; protected set; }

		public int extraBiomeChildren { get; protected set; }

		public List<WeightedBiome> biomes { get; protected set; }

		public Dictionary<string, int> featureTemplates { get; protected set; }

		public List<World.TemplateSpawnRules> subworldTemplateRules { get; protected set; }

		public int iterations { get; protected set; }

		public float minEnergy { get; protected set; }

		public ZoneType zoneType { get; private set; }

		public List<SampleDescriber> samplers { get; private set; }

		public float pdWeight { get; private set; }

		public SubWorld()
		{
			minChildCount = 2;
			features = new List<Feature>();
			tags = new List<string>();
			biomes = new List<WeightedBiome>();
			samplers = new List<SampleDescriber>();
			featureTemplates = new Dictionary<string, int>();
			pdWeight = 1f;
			borderSizeOverride = new MinMax(1f, 2.5f);
		}

		public void EnforceTemplateSpawnRuleSelfConsistency()
		{
			if (subworldTemplateRules == null)
			{
				return;
			}
			foreach (World.TemplateSpawnRules subworldTemplateRule in subworldTemplateRules)
			{
				bool flag = true;
				foreach (World.AllowedCellsFilter item in subworldTemplateRule.allowedCellsFilter)
				{
					DebugUtil.DevAssert(item.command != World.AllowedCellsFilter.Command.Replace, "subworldTemplateRules in " + base.name + " contains an AllowedCellsFilter with Command.Replace, which replaces the implicit subworld filter.");
					DebugUtil.Assert(item.zoneTypes == null || item.zoneTypes.Count == 0, "subworldTemplateRules in " + base.name + " contains zoneTypes, which is unsupported since there is an implicit subworld filter. Use worldTemplateRules instead.");
					DebugUtil.Assert(item.command != World.AllowedCellsFilter.Command.All || flag, "subworldTemplateRules in " + base.name + " contains an All command that's not the first filter in the list.");
					flag = false;
				}
				DebugUtil.Assert(!subworldTemplateRule.IsGuaranteeRule(), "subworldTemplateRules in " + base.name + " contains a guaranteed rule, which is not allowed. Include such rules in worldTemplateRules.");
				World.AllowedCellsFilter allowedCellsFilter = new World.AllowedCellsFilter();
				allowedCellsFilter.subworldNames.Add(base.name);
				subworldTemplateRule.allowedCellsFilter.Insert(0, allowedCellsFilter);
			}
		}
	}
}
