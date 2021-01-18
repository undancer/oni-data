using System.Collections.Generic;

namespace Satsuma
{
	public static class PathExtensions
	{
		public static bool IsCycle(this IPath path)
		{
			return path.FirstNode == path.LastNode && path.ArcCount() > 0;
		}

		public static Node NextNode(this IPath path, Node node)
		{
			Arc arc = path.NextArc(node);
			if (arc == Arc.Invalid)
			{
				return Node.Invalid;
			}
			return path.Other(arc, node);
		}

		public static Node PrevNode(this IPath path, Node node)
		{
			Arc arc = path.PrevArc(node);
			if (arc == Arc.Invalid)
			{
				return Node.Invalid;
			}
			return path.Other(arc, node);
		}

		internal static IEnumerable<Arc> ArcsHelper(this IPath path, Node u, ArcFilter filter)
		{
			Arc arc2 = path.PrevArc(u);
			Arc arc3 = path.NextArc(u);
			if (arc2 == arc3)
			{
				arc3 = Arc.Invalid;
			}
			for (int i = 0; i < 2; i++)
			{
				Arc arc = ((i == 0) ? arc2 : arc3);
				if (arc == Arc.Invalid)
				{
					continue;
				}
				switch (filter)
				{
				case ArcFilter.All:
					yield return arc;
					break;
				case ArcFilter.Edge:
					if (path.IsEdge(arc))
					{
						yield return arc;
					}
					break;
				case ArcFilter.Forward:
					if (path.IsEdge(arc) || path.U(arc) == u)
					{
						yield return arc;
					}
					break;
				case ArcFilter.Backward:
					if (path.IsEdge(arc) || path.V(arc) == u)
					{
						yield return arc;
					}
					break;
				}
			}
		}
	}
}
