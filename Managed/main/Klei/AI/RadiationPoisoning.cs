using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
	public class RadiationPoisoning : Disease
	{
		public const string ID = "RadiationSickness";

		public RadiationPoisoning(bool statsOnly)
			: base("RadiationSickness", 100, RangeInfo.Idempotent(), RangeInfo.Idempotent(), RangeInfo.Idempotent(), RangeInfo.Idempotent(), 0f, statsOnly)
		{
		}

		protected override void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = 0f,
				minCountPerKG = 0f,
				populationHalfLife = 600f,
				maxCountPerKG = float.PositiveInfinity,
				overPopulationHalfLife = 600f,
				minDiffusionCount = 10000,
				diffusionScale = 0f,
				minDiffusionInfestationTickCount = (byte)1
			});
			InitializeElemExposureArray(ref elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
		}
	}
}
