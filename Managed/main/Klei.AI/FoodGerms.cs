using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
	public class FoodGerms : Disease
	{
		public const string ID = "FoodPoisoning";

		private const float VOMIT_FREQUENCY = 200f;

		public FoodGerms()
			: base("FoodPoisoning", 10, new RangeInfo(248.15f, 278.15f, 313.15f, 348.15f), new RangeInfo(10f, 1200f, 1200f, 10f), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent())
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
				maxCountPerKG = 1000f,
				overPopulationHalfLife = 3000f,
				minDiffusionCount = 1000,
				diffusionScale = 0.001f,
				minDiffusionInfestationTickCount = (byte)1
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				minCountPerKG = 0.4f,
				populationHalfLife = 300f,
				overPopulationHalfLife = 10f,
				minDiffusionCount = 1000000
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ToxicSand)
			{
				populationHalfLife = float.PositiveInfinity,
				overPopulationHalfLife = 12000f
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.Creature)
			{
				populationHalfLife = float.PositiveInfinity,
				maxCountPerKG = 4000f,
				overPopulationHalfLife = 3000f
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.BleachStone)
			{
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				diffusionScale = 0.001f
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				minCountPerKG = 250f,
				populationHalfLife = 1200f,
				overPopulationHalfLife = 300f,
				diffusionScale = 0.01f
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ContaminatedOxygen)
			{
				populationHalfLife = 12000f,
				maxCountPerKG = 10000f,
				overPopulationHalfLife = 3000f,
				diffusionScale = 0.05f
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				minDiffusionCount = 1000000
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Liquid)
			{
				minCountPerKG = 0.4f,
				populationHalfLife = 12000f,
				maxCountPerKG = 5000f,
				diffusionScale = 0.2f
			});
			AddGrowthRule(new ElementGrowthRule(SimHashes.DirtyWater)
			{
				populationHalfLife = -12000f,
				overPopulationHalfLife = 12000f
			});
			AddGrowthRule(new TagGrowthRule(GameTags.Edible)
			{
				populationHalfLife = -12000f,
				overPopulationHalfLife = float.PositiveInfinity
			});
			AddGrowthRule(new TagGrowthRule(GameTags.Pickled)
			{
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f
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
			AddExposureRule(new ElementExposureRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = 10f
			});
		}
	}
}
