using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class IntegerPreflow : IFlow<long>
	{
		private readonly Dictionary<Arc, long> flow;

		private readonly Dictionary<Node, long> excess;

		private readonly Dictionary<Node, long> label;

		private readonly PriorityQueue<Node, long> active;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Func<Arc, long> Capacity
		{
			get;
			private set;
		}

		public Node Source
		{
			get;
			private set;
		}

		public Node Target
		{
			get;
			private set;
		}

		public long FlowSize
		{
			get;
			private set;
		}

		public IEnumerable<KeyValuePair<Arc, long>> NonzeroArcs => flow.Where((KeyValuePair<Arc, long> kv) => kv.Value != 0);

		public IntegerPreflow(IGraph graph, Func<Arc, long> capacity, Node source, Node target)
		{
			Graph = graph;
			Capacity = capacity;
			Source = source;
			Target = target;
			flow = new Dictionary<Arc, long>();
			excess = new Dictionary<Node, long>();
			label = new Dictionary<Node, long>();
			active = new PriorityQueue<Node, long>();
			Run();
			excess = null;
			label = null;
			active = null;
		}

		private void Run()
		{
			foreach (Node item in Graph.Nodes())
			{
				label[item] = ((item == Source) ? (-Graph.NodeCount()) : 0);
				excess[item] = 0L;
			}
			long num = 0L;
			foreach (Arc item2 in Graph.Arcs(Source, ArcFilter.Forward))
			{
				Node node = Graph.Other(item2, Source);
				if (node == Source)
				{
					continue;
				}
				long num2 = ((Graph.U(item2) == Source) ? Capacity(item2) : (-Capacity(item2)));
				if (num2 != 0)
				{
					flow[item2] = num2;
					num2 = Math.Abs(num2);
					num = checked(num + num2);
					excess[node] += num2;
					if (node != Target)
					{
						active[node] = 0L;
					}
				}
			}
			excess[Source] = -num;
			while (active.Count > 0)
			{
				long priority;
				Node node2 = active.Peek(out priority);
				active.Pop();
				long num3 = excess[node2];
				long num4 = long.MinValue;
				foreach (Arc item3 in Graph.Arcs(node2))
				{
					Node node3 = Graph.U(item3);
					Node node4 = Graph.V(item3);
					if (node3 == node4)
					{
						continue;
					}
					Node key = ((node2 == node3) ? node4 : node3);
					bool flag = Graph.IsEdge(item3);
					flow.TryGetValue(item3, out var value);
					long num5 = Capacity(item3);
					long num6 = (flag ? (-Capacity(item3)) : 0);
					if (node3 == node2)
					{
						if (value == num5)
						{
							continue;
						}
						long num7 = label[key];
						if (num7 <= priority)
						{
							num4 = Math.Max(num4, num7 - 1);
							continue;
						}
						long num8 = (long)Math.Min((ulong)num3, (ulong)(num5 - value));
						flow[item3] = value + num8;
						excess[node4] += num8;
						if (node4 != Source && node4 != Target)
						{
							active[node4] = label[node4];
						}
						num3 -= num8;
						if (num3 == 0)
						{
							break;
						}
					}
					else
					{
						if (value == num6)
						{
							continue;
						}
						long num9 = label[key];
						if (num9 <= priority)
						{
							num4 = Math.Max(num4, num9 - 1);
							continue;
						}
						long num10 = (long)Math.Min((ulong)num3, (ulong)(value - num6));
						flow[item3] = value - num10;
						excess[node3] += num10;
						if (node3 != Source && node3 != Target)
						{
							active[node3] = label[node3];
						}
						num3 -= num10;
						if (num3 == 0)
						{
							break;
						}
					}
				}
				excess[node2] = num3;
				if (num3 > 0)
				{
					if (num4 == long.MinValue)
					{
						throw new InvalidOperationException("Internal error.");
					}
					long num13 = (active[node2] = (label[node2] = (priority = num4)));
				}
			}
			FlowSize = 0L;
			foreach (Arc item4 in Graph.Arcs(Source))
			{
				Node a = Graph.U(item4);
				Node b = Graph.V(item4);
				if (!(a == b) && flow.TryGetValue(item4, out var value2))
				{
					if (a == Source)
					{
						FlowSize += value2;
					}
					else
					{
						FlowSize -= value2;
					}
				}
			}
		}

		public long Flow(Arc arc)
		{
			flow.TryGetValue(arc, out var value);
			return value;
		}
	}
}
