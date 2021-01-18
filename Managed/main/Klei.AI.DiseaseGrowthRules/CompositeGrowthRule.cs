namespace Klei.AI.DiseaseGrowthRules
{
	public class CompositeGrowthRule
	{
		public string name;

		public float underPopulationDeathRate;

		public float populationHalfLife;

		public float overPopulationHalfLife;

		public float diffusionScale;

		public float minCountPerKG;

		public float maxCountPerKG;

		public int minDiffusionCount;

		public byte minDiffusionInfestationTickCount;

		public string Name()
		{
			return name;
		}

		public void Overlay(GrowthRule rule)
		{
			if (rule.underPopulationDeathRate.HasValue)
			{
				underPopulationDeathRate = rule.underPopulationDeathRate.Value;
			}
			if (rule.populationHalfLife.HasValue)
			{
				populationHalfLife = rule.populationHalfLife.Value;
			}
			if (rule.overPopulationHalfLife.HasValue)
			{
				overPopulationHalfLife = rule.overPopulationHalfLife.Value;
			}
			if (rule.diffusionScale.HasValue)
			{
				diffusionScale = rule.diffusionScale.Value;
			}
			if (rule.minCountPerKG.HasValue)
			{
				minCountPerKG = rule.minCountPerKG.Value;
			}
			if (rule.maxCountPerKG.HasValue)
			{
				maxCountPerKG = rule.maxCountPerKG.Value;
			}
			if (rule.minDiffusionCount.HasValue)
			{
				minDiffusionCount = rule.minDiffusionCount.Value;
			}
			if (rule.minDiffusionInfestationTickCount.HasValue)
			{
				minDiffusionInfestationTickCount = rule.minDiffusionInfestationTickCount.Value;
			}
			name = rule.Name();
		}

		public float GetHalfLifeForCount(int count, float kg)
		{
			int num = (int)(minCountPerKG * kg);
			int num2 = (int)(maxCountPerKG * kg);
			if (count < num)
			{
				return populationHalfLife;
			}
			if (count < num2)
			{
				return populationHalfLife;
			}
			return overPopulationHalfLife;
		}
	}
}
