using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class PathGraph : IPath, IGraph, IArcLookup
	{
		public enum Topology
		{
			Path,
			Cycle
		}

		private readonly int nodeCount;

		private readonly bool isCycle;

		private readonly bool directed;

		public Node FirstNode
		{
			get
			{
				if (nodeCount <= 0)
				{
					return Node.Invalid;
				}
				return new Node(1L);
			}
		}

		public Node LastNode
		{
			get
			{
				if (nodeCount <= 0)
				{
					return Node.Invalid;
				}
				return new Node(isCycle ? 1 : nodeCount);
			}
		}

		public PathGraph(int nodeCount, Topology topology, Directedness directedness)
		{
			this.nodeCount = nodeCount;
			isCycle = topology == Topology.Cycle;
			directed = directedness == Directedness.Directed;
		}

		public Node GetNode(int index)
		{
			return new Node(1L + (long)index);
		}

		public int GetNodeIndex(Node node)
		{
			return (int)(node.Id - 1);
		}

		public Arc NextArc(Node node)
		{
			if (!isCycle && node.Id == nodeCount)
			{
				return Arc.Invalid;
			}
			return new Arc(node.Id);
		}

		public Arc PrevArc(Node node)
		{
			if (node.Id == 1)
			{
				if (!isCycle)
				{
					return Arc.Invalid;
				}
				return new Arc(nodeCount);
			}
			return new Arc(node.Id - 1);
		}

		public Node U(Arc arc)
		{
			return new Node(arc.Id);
		}

		public Node V(Arc arc)
		{
			return new Node((arc.Id == nodeCount) ? 1 : (arc.Id + 1));
		}

		public bool IsEdge(Arc arc)
		{
			return !directed;
		}

		public IEnumerable<Node> Nodes()
		{
			for (int i = 1; i <= nodeCount; i++)
			{
				yield return new Node(i);
			}
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (!directed || filter != ArcFilter.Edge)
			{
				int i = 1;
				for (int j = ArcCountInternal(); i <= j; i++)
				{
					yield return new Arc(i);
				}
			}
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return this.ArcsHelper(u, filter);
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return from arc in Arcs(u, filter)
				where this.Other(arc, u) == v
				select arc;
		}

		public int NodeCount()
		{
			return nodeCount;
		}

		private int ArcCountInternal()
		{
			if (nodeCount != 0)
			{
				if (!isCycle)
				{
					return nodeCount - 1;
				}
				return nodeCount;
			}
			return 0;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (!directed || filter != ArcFilter.Edge)
			{
				return ArcCountInternal();
			}
			return 0;
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			return Arcs(u, filter).Count();
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return Arcs(u, v, filter).Count();
		}

		public bool HasNode(Node node)
		{
			if (node.Id >= 1)
			{
				return node.Id <= nodeCount;
			}
			return false;
		}

		public bool HasArc(Arc arc)
		{
			if (arc.Id >= 1)
			{
				return arc.Id <= ArcCountInternal();
			}
			return false;
		}
	}
}
