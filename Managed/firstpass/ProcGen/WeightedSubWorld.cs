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

		public WeightedSubWorld(float weight, SubWorld subWorld, float overridePower = -1f)
		{
			this.weight = weight;
			this.subWorld = subWorld;
			this.overridePower = overridePower;
		}

		public override int GetHashCode()
		{
			return subWorld.GetHashCode();
		}
	}
}
