using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Dijkstra
	{
		private readonly Dictionary<Node, double> distance;

		private readonly Dictionary<Node, Arc> parentArc;

		private readonly PriorityQueue<Node, double> priorityQueue;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Func<Arc, double> Cost
		{
			get;
			private set;
		}

		public DijkstraMode Mode
		{
			get;
			private set;
		}

		public double NullCost
		{
			get;
			private set;
		}

		public IEnumerable<Node> ReachedNodes => parentArc.Keys;

		public IEnumerable<Node> FixedNodes => distance.Keys;

		public Dijkstra(IGraph graph, Func<Arc, double> cost, DijkstraMode mode)
		{
			Graph = graph;
			Cost = cost;
			Mode = mode;
			NullCost = ((mode == DijkstraMode.Sum) ? 0.0 : double.NegativeInfinity);
			distance = new Dictionary<Node, double>();
			parentArc = new Dictionary<Node, Arc>();
			priorityQueue = new PriorityQueue<Node, double>();
		}

		private void ValidateCost(double c)
		{
			if (Mode == DijkstraMode.Sum && c < 0.0)
			{
				throw new InvalidOperationException("Invalid cost: " + c);
			}
		}

		public void AddSource(Node node)
		{
			AddSource(node, NullCost);
		}

		public void AddSource(Node node, double nodeCost)
		{
			if (Reached(node))
			{
				throw new InvalidOperationException("Cannot add a reached node as a source.");
			}
			ValidateCost(nodeCost);
			parentArc[node] = Arc.Invalid;
			priorityQueue[node] = nodeCost;
		}

		public Node Step()
		{
			if (priorityQueue.Count == 0)
			{
				return Node.Invalid;
			}
			double priority;
			Node node = priorityQueue.Peek(out priority);
			priorityQueue.Pop();
			if (double.IsPositiveInfinity(priority))
			{
				return Node.Invalid;
			}
			distance[node] = priority;
			foreach (Arc item in Graph.Arcs(node, ArcFilter.Forward))
			{
				Node node2 = Graph.Other(item, node);
				if (!Fixed(node2))
				{
					double num = Cost(item);
					ValidateCost(num);
					double num2 = ((Mode == DijkstraMode.Sum) ? (priority + num) : Math.Max(priority, num));
					if (!priorityQueue.TryGetPriority(node2, out var priority2))
					{
						priority2 = double.PositiveInfinity;
					}
					if (num2 < priority2)
					{
						priorityQueue[node2] = num2;
						parentArc[node2] = item;
					}
				}
			}
			return node;
		}

		public void Run()
		{
			while (Step() != Node.Invalid)
			{
			}
		}

		public Node RunUntilFixed(Node target)
		{
			if (Fixed(target))
			{
				return target;
			}
			Node node;
			do
			{
				node = Step();
			}
			while (!(node == Node.Invalid) && !(node == target));
			return node;
		}

		public Node RunUntilFixed(Func<Node, bool> isTarget)
		{
			Node node = FixedNodes.FirstOrDefault(isTarget);
			if (node != Node.Invalid)
			{
				return node;
			}
			do
			{
				node = Step();
			}
			while (!(node == Node.Invalid) && !isTarget(node));
			return node;
		}

		public bool Reached(Node node)
		{
			return parentArc.ContainsKey(node);
		}

		public bool Fixed(Node node)
		{
			return distance.ContainsKey(node);
		}

		public double GetDistance(Node node)
		{
			if (!distance.TryGetValue(node, out var value))
			{
				return double.PositiveInfinity;
			}
			return value;
		}

		public Arc GetParentArc(Node node)
		{
			if (!parentArc.TryGetValue(node, out var value))
			{
				return Arc.Invalid;
			}
			return value;
		}

		public IPath GetPath(Node node)
		{
			if (!Reached(node))
			{
				return null;
			}
			Path path = new Path(Graph);
			path.Begin(node);
			while (true)
			{
				Arc arc = GetParentArc(node);
				if (arc == Arc.Invalid)
				{
					break;
				}
				path.AddFirst(arc);
				node = Graph.Other(arc, node);
			}
			return path;
		}
	}
}
