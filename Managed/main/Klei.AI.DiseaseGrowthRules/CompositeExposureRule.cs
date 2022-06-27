namespace Klei.AI.DiseaseGrowthRules
{
	public class CompositeExposureRule
	{
		public string name;

		public float populationHalfLife;

		public string Name()
		{
			return name;
		}

		public void Overlay(ExposureRule rule)
		{
			if (rule.populationHalfLife.HasValue)
			{
				populationHalfLife = rule.populationHalfLife.Value;
			}
			name = rule.Name();
		}

		public float GetHalfLifeForCount(int count)
		{
			return populationHalfLife;
		}
	}
}
