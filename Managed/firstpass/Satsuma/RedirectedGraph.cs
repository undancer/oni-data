using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class RedirectedGraph : IGraph, IArcLookup
	{
		public enum Direction
		{
			Forward,
			Backward,
			Edge
		}

		private IGraph graph;

		private Func<Arc, Direction> getDirection;

		public RedirectedGraph(IGraph graph, Func<Arc, Direction> getDirection)
		{
			this.graph = graph;
			this.getDirection = getDirection;
		}

		public Node U(Arc arc)
		{
			if (getDirection(arc) != Direction.Backward)
			{
				return graph.U(arc);
			}
			return graph.V(arc);
		}

		public Node V(Arc arc)
		{
			if (getDirection(arc) != Direction.Backward)
			{
				return graph.V(arc);
			}
			return graph.U(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return getDirection(arc) == Direction.Edge;
		}

		public IEnumerable<Node> Nodes()
		{
			return graph.Nodes();
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (filter != 0)
			{
				return from x in graph.Arcs()
					where getDirection(x) == Direction.Edge
					select x;
			}
			return graph.Arcs();
		}

		private IEnumerable<Arc> FilterArcs(Node u, IEnumerable<Arc> arcs, ArcFilter filter)
		{
			return filter switch
			{
				ArcFilter.All => arcs, 
				ArcFilter.Edge => arcs.Where((Arc x) => getDirection(x) == Direction.Edge), 
				ArcFilter.Forward => arcs.Where((Arc x) => getDirection(x) switch
				{
					Direction.Forward => U(x) == u, 
					Direction.Backward => V(x) == u, 
					_ => true, 
				}), 
				_ => arcs.Where((Arc x) => getDirection(x) switch
				{
					Direction.Forward => V(x) == u, 
					Direction.Backward => U(x) == u, 
					_ => true, 
				}), 
			};
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return FilterArcs(u, graph.Arcs(u), filter);
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return FilterArcs(u, graph.Arcs(u, v), filter);
		}

		public int NodeCount()
		{
			return graph.NodeCount();
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (filter != 0)
			{
				return Arcs(filter).Count();
			}
			return graph.ArcCount();
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
			return graph.HasNode(node);
		}

		public bool HasArc(Arc arc)
		{
			return graph.HasArc(arc);
		}
	}
}
