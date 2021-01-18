using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class MaximumMatching : IClearable
	{
		private readonly Matching matching;

		private readonly HashSet<Node> unmatchedRedNodes;

		private Dictionary<Node, Arc> parentArc;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Func<Node, bool> IsRed
		{
			get;
			private set;
		}

		public IMatching Matching => matching;

		public MaximumMatching(IGraph graph, Func<Node, bool> isRed)
		{
			Graph = graph;
			IsRed = isRed;
			matching = new Matching(Graph);
			unmatchedRedNodes = new HashSet<Node>();
			Clear();
		}

		public void Clear()
		{
			matching.Clear();
			unmatchedRedNodes.Clear();
			foreach (Node item in Graph.Nodes())
			{
				if (IsRed(item))
				{
					unmatchedRedNodes.Add(item);
				}
			}
		}

		public int GreedyGrow(int maxImprovements = int.MaxValue)
		{
			int num = 0;
			List<Node> list = new List<Node>();
			foreach (Node unmatchedRedNode in unmatchedRedNodes)
			{
				foreach (Arc item in Graph.Arcs(unmatchedRedNode))
				{
					Node node = Graph.Other(item, unmatchedRedNode);
					if (matching.HasNode(node))
					{
						continue;
					}
					matching.Enable(item, enabled: true);
					list.Add(unmatchedRedNode);
					num++;
					if (num >= maxImprovements)
					{
						goto IL_00cd;
					}
					break;
				}
			}
			goto IL_00cd;
			IL_00cd:
			foreach (Node item2 in list)
			{
				unmatchedRedNodes.Remove(item2);
			}
			return num;
		}

		public void Add(Arc arc)
		{
			if (!matching.HasArc(arc))
			{
				matching.Enable(arc, enabled: true);
				Node node = Graph.U(arc);
				unmatchedRedNodes.Remove(IsRed(node) ? node : Graph.V(arc));
			}
		}

		private Node Traverse(Node node)
		{
			Arc arc = matching.MatchedArc(node);
			if (IsRed(node))
			{
				foreach (Arc item in Graph.Arcs(node))
				{
					if (!(item != arc))
					{
						continue;
					}
					Node node2 = Graph.Other(item, node);
					if (!parentArc.ContainsKey(node2))
					{
						parentArc[node2] = item;
						if (!matching.HasNode(node2))
						{
							return node2;
						}
						Node node3 = Traverse(node2);
						if (node3 != Node.Invalid)
						{
							return node3;
						}
					}
				}
			}
			else
			{
				Node node4 = Graph.Other(arc, node);
				if (!parentArc.ContainsKey(node4))
				{
					parentArc[node4] = arc;
					Node node5 = Traverse(node4);
					if (node5 != Node.Invalid)
					{
						return node5;
					}
				}
			}
			return Node.Invalid;
		}

		public void Run()
		{
			List<Node> list = new List<Node>();
			parentArc = new Dictionary<Node, Arc>();
			foreach (Node unmatchedRedNode in unmatchedRedNodes)
			{
				parentArc.Clear();
				parentArc[unmatchedRedNode] = Arc.Invalid;
				Node node = Traverse(unmatchedRedNode);
				if (node == Node.Invalid)
				{
					continue;
				}
				while (true)
				{
					Arc arc = parentArc[node];
					Node node2 = Graph.Other(arc, node);
					Arc arc2 = ((node2 == unmatchedRedNode) ? Arc.Invalid : parentArc[node2]);
					if (arc2 != Arc.Invalid)
					{
						matching.Enable(arc2, enabled: false);
					}
					matching.Enable(arc, enabled: true);
					if (arc2 == Arc.Invalid)
					{
						break;
					}
					node = Graph.Other(arc2, node2);
				}
				list.Add(unmatchedRedNode);
			}
			parentArc = null;
			foreach (Node item in list)
			{
				unmatchedRedNodes.Remove(item);
			}
		}
	}
}
