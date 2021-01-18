using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
	public class SlimeGerms : Disease
	{
		private const float COUGH_FREQUENCY = 20f;

		private const int DISEASE_AMOUNT = 1000;

		public const string ID = "SlimeLung";

		public SlimeGerms()
			: base("SlimeLung", 20, new RangeInfo(283.15f, 293.15f, 363.15f, 373.15f), new RangeInfo(10f, 1200f, 1200f, 10f), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent())
		{
		}

		protected override void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = 2.6666667f,
				minCountPerKG = 0.4f,
				populationHalfLife = 12000f,
				maxCountPerKG = 500f,
				overPopulationHalfLife = 1200f,
				minDiffusionCount = 1000,
				diffusionScale = 0.001f,
				minDiffusionInfestationTickCount = (byte)1
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				minCountPerKG = 0.4f,
				populationHalfLife = 3000f,
				overPopulationHalfLife = 1200f,
				diffusionScale = 1E-06f,
				minDiffusionCount = 1000000
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.SlimeMold)
			{
				underPopulationDeathRate = 0f,
				populationHalfLife = -3000f,
				overPopulationHalfLife = 3000f,
				maxCountPerKG = 4500f,
				diffusionScale = 0.05f
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.BleachStone)
			{
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				minDiffusionCount = 100000,
				diffusionScale = 0.001f
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				minCountPerKG = 250f,
				populationHalfLife = 12000f,
				overPopulationHalfLife = 1200f,
				maxCountPerKG = 10000f,
				minDiffusionCount = 5100,
				diffusionScale = 0.005f
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ContaminatedOxygen)
			{
				underPopulationDeathRate = 0f,
				populationHalfLife = -300f,
				overPopulationHalfLife = 1200f
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.Oxygen)
			{
				populationHalfLife = 1200f,
				overPopulationHalfLife = 10f
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				minDiffusionCount = 100000,
				diffusionScale = 0.001f
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Liquid)
			{
				minCountPerKG = 0.4f,
				populationHalfLife = 1200f,
				overPopulationHalfLife = 300f,
				maxCountPerKG = 100f,
				diffusionScale = 0.01f
			});
			InitializeElemExposureArray(ref elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
			AddExposureRule(new ExposureRule
			{
				populationHalfLife = float.PositiveInfinity
			});
			AddExposureRule(new ElementExposureRule(SimHashes.DirtyWater)
			{
				populationHalfLife = -12000f
			});
			AddExposureRule(new ElementExposureRule(SimHashes.ContaminatedOxygen)
			{
				populationHalfLife = -12000f
			});
			AddExposureRule(new ElementExposureRule(SimHashes.Oxygen)
			{
				populationHalfLife = 3000f
			});
			AddExposureRule(new ElementExposureRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = 10f
			});
		}
	}
}
