using System.Collections.Generic;

namespace ProcGen
{
	public static class WeightedRandom
	{
		public static T Choose<T>(List<T> list, SeededRandom rand) where T : IWeighted
		{
			if (list.Count == 0)
			{
				return default(T);
			}
			float num = 0f;
			for (int i = 0; i < list.Count; i++)
			{
				num += list[i].weight;
			}
			float num2 = rand.RandomValue() * num;
			float num3 = 0f;
			for (int j = 0; j < list.Count; j++)
			{
				num3 += list[j].weight;
				if (num3 > num2)
				{
					return list[j];
				}
			}
			return list[list.Count - 1];
		}
	}
}
