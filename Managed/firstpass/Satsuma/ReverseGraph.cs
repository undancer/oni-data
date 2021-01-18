using System.Collections.Generic;

namespace Satsuma
{
	public sealed class ReverseGraph : IGraph, IArcLookup
	{
		private IGraph graph;

		public static ArcFilter Reverse(ArcFilter filter)
		{
			return filter switch
			{
				ArcFilter.Forward => ArcFilter.Backward, 
				ArcFilter.Backward => ArcFilter.Forward, 
				_ => filter, 
			};
		}

		public ReverseGraph(IGraph graph)
		{
			this.graph = graph;
		}

		public Node U(Arc arc)
		{
			return graph.V(arc);
		}

		public Node V(Arc arc)
		{
			return graph.U(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return graph.IsEdge(arc);
		}

		public IEnumerable<Node> Nodes()
		{
			return graph.Nodes();
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			return graph.Arcs(filter);
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return graph.Arcs(u, Reverse(filter));
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return graph.Arcs(u, v, Reverse(filter));
		}

		public int NodeCount()
		{
			return graph.NodeCount();
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return graph.ArcCount(filter);
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			return graph.ArcCount(u, Reverse(filter));
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return graph.ArcCount(u, v, Reverse(filter));
		}

		public bool HasNode(Node node)
		{
			return graph.HasNode(node);
		}

		public bool HasArc(Arc arc)
		{
			return graph.HasArc(arc);
		}
	}
}
