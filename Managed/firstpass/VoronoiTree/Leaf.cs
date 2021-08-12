using System.Collections.Generic;
using Delaunay.Geo;

namespace VoronoiTree
{
	public class Leaf : Node
	{
		public Leaf()
			: base(NodeType.Leaf)
		{
		}

		public Leaf(Diagram.Site site, Tree parent)
			: base(site, NodeType.Leaf, parent)
		{
		}

		public void GetIntersectingSites(LineSegment edge, List<Diagram.Site> intersectingSites)
		{
			if (site != null && site.poly != null)
			{
				LineSegment intersectingSegment = new LineSegment(null, null);
				if (site.poly.ClipSegment(edge, ref intersectingSegment))
				{
					intersectingSites.Add(site);
				}
			}
		}
	}
}
