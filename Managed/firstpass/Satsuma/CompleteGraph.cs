using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class CompleteGraph : IGraph, IArcLookup
	{
		private readonly int nodeCount;

		public bool Directed
		{
			get;
			private set;
		}

		public CompleteGraph(int nodeCount, Directedness directedness)
		{
			this.nodeCount = nodeCount;
			Directed = directedness == Directedness.Directed;
			if (nodeCount < 0)
			{
				throw new ArgumentException("Invalid node count: " + nodeCount);
			}
			long num = (long)nodeCount * (long)(nodeCount - 1);
			if (!Directed)
			{
				num /= 2;
			}
			if (num > int.MaxValue)
			{
				throw new ArgumentException("Too many nodes: " + nodeCount);
			}
		}

		public Node GetNode(int index)
		{
			return new Node(1L + (long)index);
		}

		public int GetNodeIndex(Node node)
		{
			return (int)(node.Id - 1);
		}

		public Arc GetArc(Node u, Node v)
		{
			int num = GetNodeIndex(u);
			int num2 = GetNodeIndex(v);
			if (num == num2)
			{
				return Arc.Invalid;
			}
			if (!Directed && num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			return GetArcInternal(num, num2);
		}

		private Arc GetArcInternal(int x, int y)
		{
			return new Arc(1 + (long)y * (long)nodeCount + x);
		}

		public Node U(Arc arc)
		{
			return new Node(1 + (arc.Id - 1) % nodeCount);
		}

		public Node V(Arc arc)
		{
			return new Node(1 + (arc.Id - 1) / nodeCount);
		}

		public bool IsEdge(Arc arc)
		{
			return !Directed;
		}

		public IEnumerable<Node> Nodes()
		{
			for (int i = 0; i < nodeCount; i++)
			{
				yield return GetNode(i);
			}
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (Directed)
			{
				for (int j = 0; j < nodeCount; j++)
				{
					for (int l = 0; l < nodeCount; l++)
					{
						if (j != l)
						{
							yield return GetArcInternal(j, l);
						}
					}
				}
				yield break;
			}
			for (int j = 0; j < nodeCount; j++)
			{
				for (int l = j + 1; l < nodeCount; l++)
				{
					yield return GetArcInternal(j, l);
				}
			}
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (Directed)
			{
				switch (filter)
				{
				default:
					foreach (Node item in Nodes())
					{
						if (item != u)
						{
							yield return GetArc(item, u);
						}
					}
					break;
				case ArcFilter.Forward:
					break;
				case ArcFilter.Edge:
					yield break;
				}
			}
			if (Directed && filter == ArcFilter.Backward)
			{
				yield break;
			}
			foreach (Node item2 in Nodes())
			{
				if (item2 != u)
				{
					yield return GetArc(u, item2);
				}
			}
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (Directed)
			{
				switch (filter)
				{
				case ArcFilter.Edge:
					yield break;
				default:
					yield return GetArc(v, u);
					break;
				case ArcFilter.Forward:
					break;
				}
			}
			if (!Directed || filter != ArcFilter.Backward)
			{
				yield return GetArc(u, v);
			}
		}

		public int NodeCount()
		{
			return nodeCount;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			int num = nodeCount * (nodeCount - 1);
			if (!Directed)
			{
				num /= 2;
			}
			return num;
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (!Directed)
			{
				return nodeCount - 1;
			}
			return filter switch
			{
				ArcFilter.All => 2 * (nodeCount - 1), 
				ArcFilter.Edge => 0, 
				_ => nodeCount - 1, 
			};
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (!Directed)
			{
				return 1;
			}
			return filter switch
			{
				ArcFilter.All => 2, 
				ArcFilter.Edge => 0, 
				_ => 1, 
			};
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
			Node node = V(arc);
			if (!HasNode(node))
			{
				return false;
			}
			Node node2 = U(arc);
			if (!Directed)
			{
				return node2.Id < node.Id;
			}
			return true;
		}
	}
}
