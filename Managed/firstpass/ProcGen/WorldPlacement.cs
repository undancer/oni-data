using System.Diagnostics;

namespace ProcGen
{
	[DebuggerDisplay("{world} ({x}, {y})")]
	public class WorldPlacement
	{
		public enum LocationType
		{
			Cluster,
			Startworld,
			InnerCluster
		}

		public string world { get; set; }

		public MinMaxI allowedRings { get; set; }

		public int buffer { get; set; }

		public LocationType locationType { get; set; }

		public int x { get; private set; }

		public int y { get; private set; }

		public int width { get; private set; }

		public int height { get; private set; }

		public bool startWorld { get; set; }

		public WorldPlacement()
		{
			allowedRings = new MinMaxI(0, 9999);
			buffer = 2;
			locationType = LocationType.Cluster;
		}

		public void SetPosition(Vector2I pos)
		{
			x = pos.X;
			y = pos.Y;
		}

		public void SetSize(Vector2I size)
		{
			width = size.X;
			height = size.Y;
		}
	}
}
