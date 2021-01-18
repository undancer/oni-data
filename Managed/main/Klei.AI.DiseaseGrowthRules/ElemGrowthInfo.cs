using System;
using System.Collections.Generic;
using System.IO;

namespace Klei.AI.DiseaseGrowthRules
{
	public struct ElemGrowthInfo
	{
		public float underPopulationDeathRate;

		public float populationHalfLife;

		public float overPopulationHalfLife;

		public float diffusionScale;

		public float minCountPerKG;

		public float maxCountPerKG;

		public int minDiffusionCount;

		public byte minDiffusionInfestationTickCount;

		public void Write(BinaryWriter writer)
		{
			writer.Write(underPopulationDeathRate);
			writer.Write(populationHalfLife);
			writer.Write(overPopulationHalfLife);
			writer.Write(diffusionScale);
			writer.Write(minCountPerKG);
			writer.Write(maxCountPerKG);
			writer.Write(minDiffusionCount);
			writer.Write(minDiffusionInfestationTickCount);
		}

		public static void SetBulk(ElemGrowthInfo[] info, Func<Element, bool> test, ElemGrowthInfo settings)
		{
			List<Element> elements = ElementLoader.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				if (test(elements[i]))
				{
					info[i] = settings;
				}
			}
		}

		public float CalculateDiseaseCountDelta(int disease_count, float kg, float dt)
		{
			float num = minCountPerKG * kg;
			float num2 = maxCountPerKG * kg;
			if (num <= (float)disease_count && (float)disease_count <= num2)
			{
				return (Disease.HalfLifeToGrowthRate(populationHalfLife, dt) - 1f) * (float)disease_count;
			}
			if ((float)disease_count < num)
			{
				return (0f - underPopulationDeathRate) * dt;
			}
			return (Disease.HalfLifeToGrowthRate(overPopulationHalfLife, dt) - 1f) * (float)disease_count;
		}
	}
}
