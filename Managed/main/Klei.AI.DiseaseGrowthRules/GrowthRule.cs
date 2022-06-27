using System.Collections.Generic;

namespace Klei.AI.DiseaseGrowthRules
{
	public class GrowthRule
	{
		public float? underPopulationDeathRate;

		public float? populationHalfLife;

		public float? overPopulationHalfLife;

		public float? diffusionScale;

		public float? minCountPerKG;

		public float? maxCountPerKG;

		public int? minDiffusionCount;

		public byte? minDiffusionInfestationTickCount;

		public void Apply(ElemGrowthInfo[] infoList)
		{
			List<Element> elements = ElementLoader.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				Element element = elements[i];
				if (element.id != SimHashes.Vacuum && Test(element))
				{
					ElemGrowthInfo elemGrowthInfo = infoList[i];
					if (underPopulationDeathRate.HasValue)
					{
						elemGrowthInfo.underPopulationDeathRate = underPopulationDeathRate.Value;
					}
					if (populationHalfLife.HasValue)
					{
						elemGrowthInfo.populationHalfLife = populationHalfLife.Value;
					}
					if (overPopulationHalfLife.HasValue)
					{
						elemGrowthInfo.overPopulationHalfLife = overPopulationHalfLife.Value;
					}
					if (diffusionScale.HasValue)
					{
						elemGrowthInfo.diffusionScale = diffusionScale.Value;
					}
					if (minCountPerKG.HasValue)
					{
						elemGrowthInfo.minCountPerKG = minCountPerKG.Value;
					}
					if (maxCountPerKG.HasValue)
					{
						elemGrowthInfo.maxCountPerKG = maxCountPerKG.Value;
					}
					if (minDiffusionCount.HasValue)
					{
						elemGrowthInfo.minDiffusionCount = minDiffusionCount.Value;
					}
					if (minDiffusionInfestationTickCount.HasValue)
					{
						elemGrowthInfo.minDiffusionInfestationTickCount = minDiffusionInfestationTickCount.Value;
					}
					infoList[i] = elemGrowthInfo;
				}
			}
		}

		public virtual bool Test(Element e)
		{
			return true;
		}

		public virtual string Name()
		{
			return null;
		}
	}
}
