using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Matching : IMatching, IGraph, IArcLookup, IClearable
	{
		private readonly Dictionary<Node, Arc> matchedArc;

		private readonly HashSet<Arc> arcs;

		private int edgeCount;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Matching(IGraph graph)
		{
			Graph = graph;
			matchedArc = new Dictionary<Node, Arc>();
			arcs = new HashSet<Arc>();
			Clear();
		}

		public void Clear()
		{
			matchedArc.Clear();
			arcs.Clear();
			edgeCount = 0;
		}

		public void Enable(Arc arc, bool enabled)
		{
			if (enabled == arcs.Contains(arc))
			{
				return;
			}
			Node node = Graph.U(arc);
			Node node2 = Graph.V(arc);
			if (enabled)
			{
				if (node == node2)
				{
					throw new ArgumentException("Matchings cannot have loop arcs.");
				}
				if (matchedArc.ContainsKey(node))
				{
					throw new ArgumentException("Node is already matched: " + node);
				}
				if (matchedArc.ContainsKey(node2))
				{
					throw new ArgumentException("Node is already matched: " + node2);
				}
				matchedArc[node] = arc;
				matchedArc[node2] = arc;
				arcs.Add(arc);
				if (Graph.IsEdge(arc))
				{
					edgeCount++;
				}
			}
			else
			{
				matchedArc.Remove(node);
				matchedArc.Remove(node2);
				arcs.Remove(arc);
				if (Graph.IsEdge(arc))
				{
					edgeCount--;
				}
			}
		}

		public Arc MatchedArc(Node node)
		{
			if (!matchedArc.TryGetValue(node, out var value))
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
			return matchedArc.Keys;
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

		private bool YieldArc(Node u, ArcFilter filter, Arc arc)
		{
			if (filter != 0 && !IsEdge(arc) && (filter != ArcFilter.Forward || !(U(arc) == u)))
			{
				if (filter == ArcFilter.Backward)
				{
					return V(arc) == u;
				}
				return false;
			}
			return true;
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			Arc arc = MatchedArc(u);
			if (arc != Arc.Invalid && YieldArc(u, filter, arc))
			{
				yield return arc;
			}
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (u != v)
			{
				Arc arc = MatchedArc(u);
				if (arc != Arc.Invalid && arc == MatchedArc(v) && YieldArc(u, filter, arc))
				{
					yield return arc;
				}
			}
		}

		public int NodeCount()
		{
			return matchedArc.Count;
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
			Arc arc = MatchedArc(u);
			if (!(arc != Arc.Invalid) || !YieldArc(u, filter, arc))
			{
				return 0;
			}
			return 1;
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (u != v)
			{
				Arc arc = MatchedArc(u);
				if (!(arc != Arc.Invalid) || !(arc == MatchedArc(v)) || !YieldArc(u, filter, arc))
				{
					return 0;
				}
				return 1;
			}
			return 0;
		}

		public bool HasNode(Node node)
		{
			if (Graph.HasNode(node))
			{
				return matchedArc.ContainsKey(node);
			}
			return false;
		}

		public bool HasArc(Arc arc)
		{
			if (Graph.HasArc(arc))
			{
				return arcs.Contains(arc);
			}
			return false;
		}
	}
}
