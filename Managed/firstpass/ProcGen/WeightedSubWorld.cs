namespace ProcGen
{
	public class WeightedSubWorld : IWeighted
	{
		public SubWorld subWorld
		{
			get;
			set;
		}

		public float weight
		{
			get;
			set;
		}

		public float overridePower
		{
			get;
			set;
		}

		public int minCount
		{
			get;
			set;
		}

		public int maxCount
		{
			get;
			set;
		}

		public WeightedSubWorld(float weight, SubWorld subWorld, float overridePower = -1f, int minCount = 0, int maxCount = int.MaxValue)
		{
			this.weight = weight;
			this.subWorld = subWorld;
			this.overridePower = overridePower;
			this.minCount = minCount;
			this.maxCount = maxCount;
		}

		public override int GetHashCode()
		{
			return subWorld.GetHashCode();
		}
	}
}
