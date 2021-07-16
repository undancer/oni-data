using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Bfs
	{
		private readonly Dictionary<Node, Arc> parentArc;

		private readonly Dictionary<Node, int> level;

		private readonly Queue<Node> queue;

		public IGraph Graph
		{
			get;
			private set;
		}

		public IEnumerable<Node> ReachedNodes => parentArc.Keys;

		public Bfs(IGraph graph)
		{
			Graph = graph;
			parentArc = new Dictionary<Node, Arc>();
			level = new Dictionary<Node, int>();
			queue = new Queue<Node>();
		}

		public void AddSource(Node node)
		{
			if (!Reached(node))
			{
				parentArc[node] = Arc.Invalid;
				level[node] = 0;
				queue.Enqueue(node);
			}
		}

		public bool Step(Func<Node, bool> isTarget, out Node reachedTargetNode)
		{
			reachedTargetNode = Node.Invalid;
			if (queue.Count == 0)
			{
				return false;
			}
			Node node = queue.Dequeue();
			int value = level[node] + 1;
			foreach (Arc item in Graph.Arcs(node, ArcFilter.Forward))
			{
				Node node2 = Graph.Other(item, node);
				if (!parentArc.ContainsKey(node2))
				{
					queue.Enqueue(node2);
					level[node2] = value;
					parentArc[node2] = item;
					if (isTarget != null && isTarget(node2))
					{
						reachedTargetNode = node2;
						return false;
					}
				}
			}
			return true;
		}

		public void Run()
		{
			Node reachedTargetNode;
			while (Step(null, out reachedTargetNode))
			{
			}
		}

		public Node RunUntilReached(Node target)
		{
			if (Reached(target))
			{
				return target;
			}
			Node reachedTargetNode;
			while (Step((Node node) => node == target, out reachedTargetNode))
			{
			}
			return reachedTargetNode;
		}

		public Node RunUntilReached(Func<Node, bool> isTarget)
		{
			Node reachedTargetNode = ReachedNodes.FirstOrDefault(isTarget);
			if (reachedTargetNode != Node.Invalid)
			{
				return reachedTargetNode;
			}
			while (Step(isTarget, out reachedTargetNode))
			{
			}
			return reachedTargetNode;
		}

		public bool Reached(Node x)
		{
			return parentArc.ContainsKey(x);
		}

		public int GetLevel(Node node)
		{
			if (!level.TryGetValue(node, out var value))
			{
				return -1;
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
