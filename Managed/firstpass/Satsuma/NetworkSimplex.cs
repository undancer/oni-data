using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class NetworkSimplex : IClearable
	{
		private class RecalculatePotentialDfs : Dfs
		{
			public NetworkSimplex Parent;

			protected override void Start(out Direction direction)
			{
				direction = Direction.Undirected;
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					Parent.Potential[node] = 0.0;
				}
				else
				{
					Node key = Parent.MyGraph.Other(arc, node);
					Parent.Potential[node] = Parent.Potential[key] + ((node == Parent.MyGraph.V(arc)) ? Parent.ActualCost(arc) : (0.0 - Parent.ActualCost(arc)));
				}
				return true;
			}
		}

		private class UpdatePotentialDfs : Dfs
		{
			public NetworkSimplex Parent;

			public double Diff;

			protected override void Start(out Direction direction)
			{
				direction = Direction.Undirected;
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				Parent.Potential[node] += Diff;
				return true;
			}
		}

		private double Epsilon;

		private Supergraph MyGraph;

		private Node ArtificialNode;

		private HashSet<Arc> ArtificialArcs;

		private Dictionary<Arc, long> Tree;

		private Subgraph TreeSubgraph;

		private HashSet<Arc> Saturated;

		private Dictionary<Node, double> Potential;

		private IEnumerator<Arc> EnteringArcEnumerator;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Func<Arc, long> LowerBound
		{
			get;
			private set;
		}

		public Func<Arc, long> UpperBound
		{
			get;
			private set;
		}

		public Func<Node, long> Supply
		{
			get;
			private set;
		}

		public Func<Arc, double> Cost
		{
			get;
			private set;
		}

		public SimplexState State
		{
			get;
			private set;
		}

		public IEnumerable<KeyValuePair<Arc, long>> Forest => Tree.Where((KeyValuePair<Arc, long> kv) => Graph.HasArc(kv.Key));

		public IEnumerable<Arc> UpperBoundArcs => Saturated;

		public NetworkSimplex(IGraph graph, Func<Arc, long> lowerBound = null, Func<Arc, long> upperBound = null, Func<Node, long> supply = null, Func<Arc, double> cost = null)
		{
			Graph = graph;
			LowerBound = lowerBound ?? ((Func<Arc, long>)((Arc x) => 0L));
			UpperBound = upperBound ?? ((Func<Arc, long>)((Arc x) => long.MaxValue));
			Supply = supply ?? ((Func<Node, long>)((Node x) => 0L));
			Cost = cost ?? ((Func<Arc, double>)((Arc x) => 1.0));
			Epsilon = 1.0;
			foreach (Arc item in graph.Arcs())
			{
				double num = Math.Abs(Cost(item));
				if (num > 0.0 && num < Epsilon)
				{
					Epsilon = num;
				}
			}
			Epsilon *= 1E-12;
			Clear();
		}

		public long Flow(Arc arc)
		{
			if (Saturated.Contains(arc))
			{
				return UpperBound(arc);
			}
			if (Tree.TryGetValue(arc, out var value))
			{
				return value;
			}
			value = LowerBound(arc);
			if (value != long.MinValue)
			{
				return value;
			}
			return 0L;
		}

		public void Clear()
		{
			Dictionary<Node, long> dictionary = new Dictionary<Node, long>();
			foreach (Node item in Graph.Nodes())
			{
				dictionary[item] = Supply(item);
			}
			Saturated = new HashSet<Arc>();
			foreach (Arc item2 in Graph.Arcs())
			{
				LowerBound(item2);
				if (UpperBound(item2) < long.MaxValue)
				{
					Saturated.Add(item2);
				}
				long num = Flow(item2);
				dictionary[Graph.U(item2)] -= num;
				dictionary[Graph.V(item2)] += num;
			}
			Potential = new Dictionary<Node, double>();
			MyGraph = new Supergraph(Graph);
			ArtificialNode = MyGraph.AddNode();
			Potential[ArtificialNode] = 0.0;
			ArtificialArcs = new HashSet<Arc>();
			Dictionary<Node, Arc> dictionary2 = new Dictionary<Node, Arc>();
			foreach (Node item3 in Graph.Nodes())
			{
				long num2 = dictionary[item3];
				Arc arc = ((num2 > 0) ? MyGraph.AddArc(item3, ArtificialNode, Directedness.Directed) : MyGraph.AddArc(ArtificialNode, item3, Directedness.Directed));
				Potential[item3] = ((num2 <= 0) ? 1 : (-1));
				ArtificialArcs.Add(arc);
				dictionary2[item3] = arc;
			}
			Tree = new Dictionary<Arc, long>();
			TreeSubgraph = new Subgraph(MyGraph);
			TreeSubgraph.EnableAllArcs(enabled: false);
			foreach (KeyValuePair<Node, Arc> item4 in dictionary2)
			{
				Tree[item4.Value] = Math.Abs(dictionary[item4.Key]);
				TreeSubgraph.Enable(item4.Value, enabled: true);
			}
			State = SimplexState.FirstPhase;
			EnteringArcEnumerator = MyGraph.Arcs().GetEnumerator();
			EnteringArcEnumerator.MoveNext();
		}

		private long ActualLowerBound(Arc arc)
		{
			if (!ArtificialArcs.Contains(arc))
			{
				return LowerBound(arc);
			}
			return 0L;
		}

		private long ActualUpperBound(Arc arc)
		{
			if (!ArtificialArcs.Contains(arc))
			{
				return UpperBound(arc);
			}
			if (State != 0)
			{
				return 0L;
			}
			return long.MaxValue;
		}

		private double ActualCost(Arc arc)
		{
			if (!ArtificialArcs.Contains(arc))
			{
				if (State != 0)
				{
					return Cost(arc);
				}
				return 0.0;
			}
			return 1.0;
		}

		private static ulong MySubtract(long a, long b)
		{
			if (a == long.MaxValue || b == long.MinValue)
			{
				return ulong.MaxValue;
			}
			return (ulong)(a - b);
		}

		public void Step()
		{
			if (State != 0 && State != SimplexState.SecondPhase)
			{
				return;
			}
			Arc current = EnteringArcEnumerator.Current;
			Arc arc = Arc.Invalid;
			double num = double.NaN;
			bool flag = false;
			do
			{
				Arc current2 = EnteringArcEnumerator.Current;
				if (!Tree.ContainsKey(current2))
				{
					bool flag2 = Saturated.Contains(current2);
					double num2 = ActualCost(current2) - (Potential[MyGraph.V(current2)] - Potential[MyGraph.U(current2)]);
					if ((num2 < 0.0 - Epsilon && !flag2) || (num2 > Epsilon && (flag2 || ActualLowerBound(current2) == long.MinValue)))
					{
						arc = current2;
						num = num2;
						flag = flag2;
						break;
					}
				}
				if (!EnteringArcEnumerator.MoveNext())
				{
					EnteringArcEnumerator = MyGraph.Arcs().GetEnumerator();
					EnteringArcEnumerator.MoveNext();
				}
			}
			while (!(EnteringArcEnumerator.Current == current));
			if (arc == Arc.Invalid)
			{
				if (State == SimplexState.FirstPhase)
				{
					State = SimplexState.SecondPhase;
					foreach (Arc artificialArc in ArtificialArcs)
					{
						if (Flow(artificialArc) > 0)
						{
							State = SimplexState.Infeasible;
							break;
						}
					}
					if (State == SimplexState.SecondPhase)
					{
						RecalculatePotentialDfs recalculatePotentialDfs = new RecalculatePotentialDfs();
						recalculatePotentialDfs.Parent = this;
						recalculatePotentialDfs.Run(TreeSubgraph);
					}
				}
				else
				{
					State = SimplexState.Optimal;
				}
				return;
			}
			Node node = MyGraph.U(arc);
			Node node2 = MyGraph.V(arc);
			List<Arc> list = new List<Arc>();
			List<Arc> list2 = new List<Arc>();
			IPath path = TreeSubgraph.FindPath(node2, node, Dfs.Direction.Undirected);
			foreach (Node item in path.Nodes())
			{
				Arc arc2 = path.NextArc(item);
				((MyGraph.U(arc2) == item) ? list : list2).Add(arc2);
			}
			ulong num3 = ((num < 0.0) ? MySubtract(ActualUpperBound(arc), Flow(arc)) : MySubtract(Flow(arc), ActualLowerBound(arc)));
			Arc arc3 = arc;
			bool flag3 = !flag;
			foreach (Arc item2 in list)
			{
				ulong num4 = ((num < 0.0) ? MySubtract(ActualUpperBound(item2), Tree[item2]) : MySubtract(Tree[item2], ActualLowerBound(item2)));
				if (num4 < num3)
				{
					num3 = num4;
					arc3 = item2;
					flag3 = num < 0.0;
				}
			}
			foreach (Arc item3 in list2)
			{
				ulong num5 = ((num > 0.0) ? MySubtract(ActualUpperBound(item3), Tree[item3]) : MySubtract(Tree[item3], ActualLowerBound(item3)));
				if (num5 < num3)
				{
					num3 = num5;
					arc3 = item3;
					flag3 = num > 0.0;
				}
			}
			long num6 = 0L;
			switch (num3)
			{
			case ulong.MaxValue:
				State = SimplexState.Unbounded;
				return;
			default:
				num6 = (long)((num < 0.0) ? num3 : (0L - num3));
				foreach (Arc item4 in list)
				{
					Tree[item4] += num6;
				}
				foreach (Arc item5 in list2)
				{
					Tree[item5] -= num6;
				}
				break;
			case 0uL:
				break;
			}
			if (arc3 == arc)
			{
				if (flag)
				{
					Saturated.Remove(arc);
				}
				else
				{
					Saturated.Add(arc);
				}
				return;
			}
			Tree.Remove(arc3);
			TreeSubgraph.Enable(arc3, enabled: false);
			if (flag3)
			{
				Saturated.Add(arc3);
			}
			double num7 = ActualCost(arc) - (Potential[node2] - Potential[node]);
			if (num7 != 0.0)
			{
				UpdatePotentialDfs updatePotentialDfs = new UpdatePotentialDfs();
				updatePotentialDfs.Parent = this;
				updatePotentialDfs.Diff = num7;
				updatePotentialDfs.Run(TreeSubgraph, new Node[1]
				{
					node2
				});
			}
			Tree[arc] = Flow(arc) + num6;
			if (flag)
			{
				Saturated.Remove(arc);
			}
			TreeSubgraph.Enable(arc, enabled: true);
		}

		public void Run()
		{
			while (State == SimplexState.FirstPhase || State == SimplexState.SecondPhase)
			{
				Step();
			}
		}
	}
}
