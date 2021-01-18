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
			Barren
		}

		public string nameKey
		{
			get;
			protected set;
		}

		public string descriptionKey
		{
			get;
			protected set;
		}

		public string utilityKey
		{
			get;
			protected set;
		}

		public string biomeNoise
		{
			get;
			protected set;
		}

		public string overrideNoise
		{
			get;
			protected set;
		}

		public string densityNoise
		{
			get;
			protected set;
		}

		public string borderOverride
		{
			get;
			protected set;
		}

		[StringEnumConverter]
		public Temperature.Range temperatureRange
		{
			get;
			protected set;
		}

		public Feature centralFeature
		{
			get;
			protected set;
		}

		public List<Feature> features
		{
			get;
			protected set;
		}

		public Override overrides
		{
			get;
			protected set;
		}

		public List<string> tags
		{
			get;
			protected set;
		}

		public int minChildCount
		{
			get;
			protected set;
		}

		public int extraBiomeChildren
		{
			get;
			protected set;
		}

		public List<WeightedBiome> biomes
		{
			get;
			protected set;
		}

		public Dictionary<string, string[]> pointsOfInterest
		{
			get;
			protected set;
		}

		public Dictionary<string, int> featureTemplates
		{
			get;
			protected set;
		}

		public List<World.FeatureSpawnRules> subworldFeatureRules
		{
			get;
			protected set;
		}

		public int iterations
		{
			get;
			protected set;
		}

		public float minEnergy
		{
			get;
			protected set;
		}

		public ZoneType zoneType
		{
			get;
			private set;
		}

		public List<SampleDescriber> samplers
		{
			get;
			private set;
		}

		public float pdWeight
		{
			get;
			private set;
		}

		public SubWorld()
		{
			minChildCount = 2;
			features = new List<Feature>();
			tags = new List<string>();
			biomes = new List<WeightedBiome>();
			samplers = new List<SampleDescriber>();
			pointsOfInterest = new Dictionary<string, string[]>();
			featureTemplates = new Dictionary<string, int>();
			pdWeight = 1f;
		}

		public void EnforceFeatureSpawnRuleSelfConsistency()
		{
			if (subworldFeatureRules == null)
			{
				return;
			}
			foreach (World.FeatureSpawnRules subworldFeatureRule in subworldFeatureRules)
			{
				World.AllowedCellsFilter allowedCellsFilter = new World.AllowedCellsFilter();
				allowedCellsFilter.subworldNames.Add(base.name);
				subworldFeatureRule.allowedCellsFilter.Insert(0, allowedCellsFilter);
			}
		}
	}
}
