using System.Collections.Generic;

namespace ProcGen
{
	public class SpaceMapPOIPlacement
	{
		public List<string> pois
		{
			get;
			private set;
		}

		public int numToSpawn
		{
			get;
			set;
		}

		public MinMaxI allowedRings
		{
			get;
			set;
		}

		public bool avoidClumping
		{
			get;
			set;
		}

		public bool canSpawnDuplicates
		{
			get;
			set;
		}

		public SpaceMapPOIPlacement()
		{
			allowedRings = new MinMaxI(0, 9999);
		}
	}
}
