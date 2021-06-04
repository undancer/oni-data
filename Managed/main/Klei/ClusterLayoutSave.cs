using System.Collections.Generic;

namespace Klei
{
	public class ClusterLayoutSave
	{
		public class World
		{
			public Dictionary<string, object> stats = new Dictionary<string, object>();

			public Data data = new Data();

			public string name = string.Empty;

			public bool isDiscovered = false;

			public List<string> traits = new List<string>();
		}

		public enum POIType
		{
			TemporalTear,
			ResearchDestination
		}

		public string ID;

		public Vector2I version;

		public List<World> worlds;

		public Vector2I size;

		public int currentWorldIdx;

		public int numRings;

		public Dictionary<POIType, List<AxialI>> poiLocations = new Dictionary<POIType, List<AxialI>>();

		public Dictionary<AxialI, string> poiPlacements = new Dictionary<AxialI, string>();

		public ClusterLayoutSave()
		{
			worlds = new List<World>();
		}
	}
}
