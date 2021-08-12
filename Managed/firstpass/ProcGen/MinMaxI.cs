using System;

namespace ProcGen
{
	[Serializable]
	public struct MinMaxI
	{
		public int min { get; private set; }

		public int max { get; private set; }

		public MinMaxI(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public int GetRandomValueWithinRange(SeededRandom rnd)
		{
			return rnd.RandomRange(min, max);
		}

		public int GetAverage()
		{
			return (min + max) / 2;
		}

		public void Mod(MinMaxI mod)
		{
			min += mod.min;
			max += mod.max;
		}

		public override string ToString()
		{
			return $"min:{min} max:{max}";
		}
	}
}
