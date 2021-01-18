using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Subgraph : IGraph, IArcLookup
	{
		private IGraph graph;

		private bool defaultNodeEnabled;

		private HashSet<Node> nodeExceptions = new HashSet<Node>();

		private bool defaultArcEnabled;

		private HashSet<Arc> arcExceptions = new HashSet<Arc>();

		public Subgraph(IGraph graph)
		{
			this.graph = graph;
			EnableAllNodes(enabled: true);
			EnableAllArcs(enabled: true);
		}

		public void EnableAllNodes(bool enabled)
		{
			defaultNodeEnabled = enabled;
			nodeExceptions.Clear();
		}

		public void EnableAllArcs(bool enabled)
		{
			defaultArcEnabled = enabled;
			arcExceptions.Clear();
		}

		public void Enable(Node node, bool enabled)
		{
			if (defaultNodeEnabled != enabled)
			{
				nodeExceptions.Add(node);
			}
			else
			{
				nodeExceptions.Remove(node);
			}
		}

		public void Enable(Arc arc, bool enabled)
		{
			if (defaultArcEnabled != enabled)
			{
				arcExceptions.Add(arc);
			}
			else
			{
				arcExceptions.Remove(arc);
			}
		}

		public bool IsEnabled(Node node)
		{
			return defaultNodeEnabled ^ nodeExceptions.Contains(node);
		}

		public bool IsEnabled(Arc arc)
		{
			return defaultArcEnabled ^ arcExceptions.Contains(arc);
		}

		public Node U(Arc arc)
		{
			return graph.U(arc);
		}

		public Node V(Arc arc)
		{
			return graph.V(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return graph.IsEdge(arc);
		}

		private IEnumerable<Node> NodesInternal()
		{
			foreach (Node node in graph.Nodes())
			{
				if (IsEnabled(node))
				{
					yield return node;
				}
			}
		}

		public IEnumerable<Node> Nodes()
		{
			if (nodeExceptions.Count == 0)
			{
				if (defaultNodeEnabled)
				{
					return graph.Nodes();
				}
				return Enumerable.Empty<Node>();
			}
			return NodesInternal();
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			foreach (Arc arc in graph.Arcs(filter))
			{
				if (IsEnabled(arc) && IsEnabled(graph.U(arc)) && IsEnabled(graph.V(arc)))
				{
					yield return arc;
				}
			}
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (!IsEnabled(u))
			{
				yield break;
			}
			foreach (Arc arc in graph.Arcs(u, filter))
			{
				if (IsEnabled(arc) && IsEnabled(graph.Other(arc, u)))
				{
					yield return arc;
				}
			}
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (!IsEnabled(u) || !IsEnabled(v))
			{
				yield break;
			}
			foreach (Arc arc in graph.Arcs(u, v, filter))
			{
				if (IsEnabled(arc))
				{
					yield return arc;
				}
			}
		}

		public int NodeCount()
		{
			return defaultNodeEnabled ? (graph.NodeCount() - nodeExceptions.Count) : nodeExceptions.Count;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (nodeExceptions.Count == 0 && filter == ArcFilter.All)
			{
				return defaultNodeEnabled ? (defaultArcEnabled ? (graph.ArcCount() - arcExceptions.Count) : arcExceptions.Count) : 0;
			}
			return Arcs(filter).Count();
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
			return graph.HasNode(node) && IsEnabled(node);
		}

		public bool HasArc(Arc arc)
		{
			return graph.HasArc(arc) && IsEnabled(arc);
		}
	}
}
