using System;

namespace ProcGen
{
	[Serializable]
	public class WeightedSubworldName : IWeighted
	{
		public string name
		{
			get;
			private set;
		}

		public string overrideName
		{
			get;
			private set;
		}

		public float overridePower
		{
			get;
			private set;
		}

		public float weight
		{
			get;
			set;
		}

		public WeightedSubworldName()
		{
			weight = 1f;
		}

		public WeightedSubworldName(string name, float weight)
		{
			this.name = name;
			this.weight = weight;
		}
	}
}
