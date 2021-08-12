using KSerialization;
using Satsuma;
using UnityEngine;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Edge : Arc
	{
		public Corner corner0;

		public Corner corner1;

		public Edge()
			: base(WorldGenTags.Edge.Name)
		{
			tags = new TagSet();
		}

		public Edge(Satsuma.Arc arc, Cell s0, Cell s1, Corner c0, Corner c1)
			: base(arc, WorldGenTags.Edge.Name)
		{
			corner0 = c0;
			corner1 = c1;
			tags = new TagSet();
		}

		public void SetCorners(Corner corner0, Corner corner1)
		{
			this.corner0 = corner0;
			this.corner1 = corner1;
		}

		public Vector2 MidPoint()
		{
			return (corner1.position - corner0.position) * 0.5f + corner0.position;
		}
	}
}
