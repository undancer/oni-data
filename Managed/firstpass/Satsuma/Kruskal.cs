using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Kruskal<TCost> where TCost : IComparable<TCost>
	{
		private IEnumerator<Arc> arcEnumerator;

		private int arcsToGo;

		private DisjointSet<Node> components;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Func<Arc, TCost> Cost
		{
			get;
			private set;
		}

		public Func<Node, int> MaxDegree
		{
			get;
			private set;
		}

		public HashSet<Arc> Forest
		{
			get;
			private set;
		}

		public Dictionary<Node, int> Degree
		{
			get;
			private set;
		}

		public Kruskal(IGraph graph, Func<Arc, TCost> cost, Func<Node, int> maxDegree = null)
		{
			Graph = graph;
			Cost = cost;
			MaxDegree = maxDegree;
			Forest = new HashSet<Arc>();
			Degree = new Dictionary<Node, int>();
			foreach (Node item in Graph.Nodes())
			{
				Degree[item] = 0;
			}
			List<Arc> list = Graph.Arcs().ToList();
			list.Sort((Arc a, Arc b) => Cost(a).CompareTo(Cost(b)));
			arcEnumerator = list.GetEnumerator();
			arcsToGo = Graph.NodeCount() - new ConnectedComponents(Graph).Count;
			components = new DisjointSet<Node>();
		}

		public bool Step()
		{
			if (arcsToGo <= 0 || arcEnumerator == null || !arcEnumerator.MoveNext())
			{
				arcEnumerator = null;
				return false;
			}
			AddArc(arcEnumerator.Current);
			return true;
		}

		public void Run()
		{
			while (Step())
			{
			}
		}

		public bool AddArc(Arc arc)
		{
			Node node = Graph.U(arc);
			if (MaxDegree != null && Degree[node] >= MaxDegree(node))
			{
				return false;
			}
			DisjointSetSet<Node> a = components.WhereIs(node);
			Node node2 = Graph.V(arc);
			if (MaxDegree != null && Degree[node2] >= MaxDegree(node2))
			{
				return false;
			}
			DisjointSetSet<Node> b = components.WhereIs(node2);
			if (a == b)
			{
				return false;
			}
			Forest.Add(arc);
			components.Union(a, b);
			Degree[node]++;
			Degree[node2]++;
			arcsToGo--;
			return true;
		}
	}
}
