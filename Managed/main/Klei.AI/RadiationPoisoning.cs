using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
	public class RadiationPoisoning : Disease
	{
		private const float COUGH_FREQUENCY = 20f;

		private const float COUGH_MASS = 0.1f;

		private const int DISEASE_AMOUNT = 1000;

		private const float DEATH_TIMER = 600f;

		public const string ID = "RadiationSickness";

		public RadiationPoisoning(bool statsOnly)
			: base("RadiationSickness", 100, RangeInfo.Idempotent(), RangeInfo.Idempotent(), RangeInfo.Idempotent(), RangeInfo.Idempotent(), statsOnly)
		{
		}

		protected override void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = 0f,
				minCountPerKG = 0f,
				populationHalfLife = float.PositiveInfinity,
				maxCountPerKG = 10000f,
				overPopulationHalfLife = float.PositiveInfinity,
				minDiffusionCount = 10000,
				diffusionScale = 0f,
				minDiffusionInfestationTickCount = (byte)1
			});
			InitializeElemExposureArray(ref elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
		}
	}
}
