using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Path : IPath, IGraph, IArcLookup, IClearable
	{
		private int nodeCount;

		private Dictionary<Node, Arc> nextArc;

		private Dictionary<Node, Arc> prevArc;

		private HashSet<Arc> arcs;

		private int edgeCount;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Node FirstNode
		{
			get;
			private set;
		}

		public Node LastNode
		{
			get;
			private set;
		}

		public Path(IGraph graph)
		{
			Graph = graph;
			nextArc = new Dictionary<Node, Arc>();
			prevArc = new Dictionary<Node, Arc>();
			arcs = new HashSet<Arc>();
			Clear();
		}

		public void Clear()
		{
			FirstNode = Node.Invalid;
			LastNode = Node.Invalid;
			nodeCount = 0;
			nextArc.Clear();
			prevArc.Clear();
			arcs.Clear();
			edgeCount = 0;
		}

		public void Begin(Node node)
		{
			if (nodeCount > 0)
			{
				throw new InvalidOperationException("Path not empty.");
			}
			nodeCount = 1;
			Node node4 = (FirstNode = (LastNode = node));
		}

		public void AddFirst(Arc arc)
		{
			Node node = U(arc);
			Node node2 = V(arc);
			Node node3 = ((node == FirstNode) ? node2 : node);
			if ((node != FirstNode && node2 != FirstNode) || nextArc.ContainsKey(node3) || prevArc.ContainsKey(FirstNode))
			{
				throw new ArgumentException("Arc not valid or path is a cycle.");
			}
			if (node3 != LastNode)
			{
				nodeCount++;
			}
			nextArc[node3] = arc;
			prevArc[FirstNode] = arc;
			if (!arcs.Contains(arc))
			{
				arcs.Add(arc);
				if (IsEdge(arc))
				{
					edgeCount++;
				}
			}
			FirstNode = node3;
		}

		public void AddLast(Arc arc)
		{
			Node node = U(arc);
			Node node2 = V(arc);
			Node node3 = ((node == LastNode) ? node2 : node);
			if ((node != LastNode && node2 != LastNode) || nextArc.ContainsKey(LastNode) || prevArc.ContainsKey(node3))
			{
				throw new ArgumentException("Arc not valid or path is a cycle.");
			}
			if (node3 != FirstNode)
			{
				nodeCount++;
			}
			nextArc[LastNode] = arc;
			prevArc[node3] = arc;
			if (!arcs.Contains(arc))
			{
				arcs.Add(arc);
				if (IsEdge(arc))
				{
					edgeCount++;
				}
			}
			LastNode = node3;
		}

		public void Reverse()
		{
			Node firstNode = FirstNode;
			FirstNode = LastNode;
			LastNode = firstNode;
			Dictionary<Node, Arc> dictionary = nextArc;
			nextArc = prevArc;
			prevArc = dictionary;
		}

		public Arc NextArc(Node node)
		{
			if (!nextArc.TryGetValue(node, out var value))
			{
				return Arc.Invalid;
			}
			return value;
		}

		public Arc PrevArc(Node node)
		{
			if (!prevArc.TryGetValue(node, out var value))
			{
				return Arc.Invalid;
			}
			return value;
		}

		public Node U(Arc arc)
		{
			return Graph.U(arc);
		}

		public Node V(Arc arc)
		{
			return Graph.V(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return Graph.IsEdge(arc);
		}

		public IEnumerable<Node> Nodes()
		{
			Node i = FirstNode;
			if (i == Node.Invalid)
			{
				yield break;
			}
			do
			{
				yield return i;
				Arc arc = NextArc(i);
				if (arc == Arc.Invalid)
				{
					break;
				}
				i = Graph.Other(arc, i);
			}
			while (!(i == FirstNode));
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (filter == ArcFilter.All)
			{
				return arcs;
			}
			if (edgeCount == 0)
			{
				return Enumerable.Empty<Arc>();
			}
			return arcs.Where((Arc arc) => IsEdge(arc));
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

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (filter != 0)
			{
				return edgeCount;
			}
			return arcs.Count;
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
			if (!prevArc.ContainsKey(node))
			{
				if (node != Node.Invalid)
				{
					return node == FirstNode;
				}
				return false;
			}
			return true;
		}

		public bool HasArc(Arc arc)
		{
			return arcs.Contains(arc);
		}
	}
}
