using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
	public class PollenGerms : Disease
	{
		public const string ID = "PollenGerms";

		public PollenGerms(bool statsOnly)
			: base("PollenGerms", 5, new RangeInfo(263.15f, 273.15f, 363.15f, 373.15f), new RangeInfo(10f, 100f, 100f, 10f), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent(), statsOnly)
		{
		}

		protected override void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = 2f / 3f,
				minCountPerKG = 0.4f,
				populationHalfLife = 3000f,
				maxCountPerKG = 500f,
				overPopulationHalfLife = 10f,
				minDiffusionCount = 3000,
				diffusionScale = 0.001f,
				minDiffusionInfestationTickCount = (byte)1
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				minCountPerKG = 0.4f,
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				diffusionScale = 1E-06f,
				minDiffusionCount = 1000000
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				minCountPerKG = 500f,
				underPopulationDeathRate = 2.6666667f,
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				maxCountPerKG = 1000000f,
				minDiffusionCount = 1000,
				diffusionScale = 0.015f
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.Oxygen)
			{
				populationHalfLife = 200f,
				overPopulationHalfLife = 10f
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Liquid)
			{
				minCountPerKG = 0.4f,
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				maxCountPerKG = 100f,
				diffusionScale = 0.01f
			});
			InitializeElemExposureArray(ref elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
			AddExposureRule(new ExposureRule
			{
				populationHalfLife = 1200f
			});
			AddExposureRule(new ElementExposureRule(SimHashes.Oxygen)
			{
				populationHalfLife = float.PositiveInfinity
			});
		}
	}
}
