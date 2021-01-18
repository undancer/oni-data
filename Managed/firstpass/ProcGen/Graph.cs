using System;
using System.Collections.Generic;
using Delaunay.Geo;
using KSerialization;
using Satsuma;
using Satsuma.Drawing;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Graph<N, A> where N : Node, new()where A : Arc, new()
	{
		[Serialize]
		public List<N> nodeList;

		[Serialize]
		public List<A> arcList;

		private SeededRandom myRandom = null;

		public List<N> nodes => nodeList;

		public List<A> arcs => arcList;

		public CustomGraph baseGraph
		{
			get;
			private set;
		}

		public void SetSeed(int seed)
		{
			myRandom = new SeededRandom(seed);
		}

		public Graph(int seed)
		{
			SetSeed(seed);
			nodeList = new List<N>();
			arcList = new List<A>();
			baseGraph = new CustomGraph();
		}

		public N AddNode(string type, Vector2 position = default(Vector2))
		{
			N val = new N();
			val.SetNode(baseGraph.AddNode());
			val.SetType(type);
			val.SetPosition(position);
			nodeList.Add(val);
			return val;
		}

		public void Remove(N n)
		{
			baseGraph.DeleteNode(n.node);
			nodes.Remove(n);
		}

		public A AddArc(N nodeA, N nodeB, string type)
		{
			Satsuma.Arc arc = baseGraph.AddArc(nodeA.node, nodeB.node, Directedness.Undirected);
			A val = new A();
			val.SetArc(arc);
			val.SetType(type);
			arcList.Add(val);
			return val;
		}

		public N FindNodeByID(uint id)
		{
			return nodeList.Find((N node) => node.node.Id == id);
		}

		public A FindArcByID(uint id)
		{
			return arcList.Find((A arc) => arc.arc.Id == id);
		}

		public N FindNode(Predicate<N> pred)
		{
			return nodeList.Find(pred);
		}

		public A FindArc(Predicate<A> pred)
		{
			return arcList.Find(pred);
		}

		public List<A> GetArcs(N node0)
		{
			List<A> list = new List<A>();
			foreach (Satsuma.Arc sarc in baseGraph.Arcs(node0.node))
			{
				list.Add(arcList.Find((A a) => a.arc == sarc));
			}
			return list;
		}

		public A GetArc(N node0, N node1)
		{
			IEnumerator<Satsuma.Arc> enumerator = baseGraph.Arcs(node0.node, node1.node).GetEnumerator();
			if (enumerator.MoveNext())
			{
				Satsuma.Arc sarc = enumerator.Current;
				return arcList.Find((A a) => a.arc == sarc);
			}
			return null;
		}

		public List<N> GetNodes(A arc)
		{
			Satsuma.Node u = baseGraph.U(arc.arc);
			Satsuma.Node v = baseGraph.V(arc.arc);
			return new List<N>
			{
				nodeList.Find((N n) => n.node == u),
				nodeList.Find((N n) => n.node == v)
			};
		}

		public int GetDistanceToTagSetFromNode(N node, TagSet tagset)
		{
			List<N> nodesWithAtLeastOneTag = GetNodesWithAtLeastOneTag(tagset);
			if (nodesWithAtLeastOneTag.Count > 0)
			{
				Dijkstra dijkstra = new Dijkstra(baseGraph, (Satsuma.Arc arc) => 1.0, DijkstraMode.Sum);
				for (int i = 0; i < nodesWithAtLeastOneTag.Count; i++)
				{
					dijkstra.AddSource(nodesWithAtLeastOneTag[i].node);
				}
				dijkstra.RunUntilFixed(node.node);
				return (int)dijkstra.GetDistance(node.node);
			}
			return -1;
		}

		public int GetDistanceToTagFromNode(N node, Tag tag)
		{
			List<N> nodesWithTag = GetNodesWithTag(tag);
			if (nodesWithTag.Count > 0)
			{
				Dijkstra dijkstra = new Dijkstra(baseGraph, (Satsuma.Arc arc) => 1.0, DijkstraMode.Sum);
				for (int i = 0; i < nodesWithTag.Count; i++)
				{
					dijkstra.AddSource(nodesWithTag[i].node);
				}
				dijkstra.RunUntilFixed(node.node);
				return (int)dijkstra.GetDistance(node.node);
			}
			return -1;
		}

		public Dictionary<uint, int> GetDistanceToTag(Tag tag)
		{
			List<N> nodesWithTag = GetNodesWithTag(tag);
			if (nodesWithTag.Count > 0)
			{
				Dijkstra dijkstra = new Dijkstra(baseGraph, (Satsuma.Arc arc) => 1.0, DijkstraMode.Sum);
				for (int i = 0; i < nodesWithTag.Count; i++)
				{
					dijkstra.AddSource(nodesWithTag[i].node);
				}
				Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
				for (int j = 0; j < nodes.Count; j++)
				{
					dijkstra.RunUntilFixed(nodes[j].node);
					dictionary[(uint)nodes[j].node.Id] = (int)dijkstra.GetDistance(nodes[j].node);
				}
				return dictionary;
			}
			return null;
		}

		public List<N> GetNodesWithAtLeastOneTag(TagSet tagset)
		{
			return nodeList.FindAll((N node) => node.tags.ContainsOne(tagset));
		}

		public List<N> GetNodesWithTag(Tag tag)
		{
			return nodeList.FindAll((N node) => node.tags.Contains(tag));
		}

		public List<A> GetArcsWithTag(Tag tag)
		{
			return arcList.FindAll((A arc) => arc.tags.Contains(tag));
		}

		public static PointD GetForceForBoundry(PointD particle, Polygon bounds)
		{
			Vector2 point = new Vector2((float)particle.X, (float)particle.Y);
			List<KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>> edgesWithinDistance = bounds.GetEdgesWithinDistance(point);
			double num = 0.0;
			double num2 = 0.0;
			for (int i = 0; i < edgesWithinDistance.Count; i++)
			{
				KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>> keyValuePair = edgesWithinDistance[i];
				MathUtil.Pair<Vector2, Vector2> value = keyValuePair.Value;
				float second = keyValuePair.Key.Second;
				double num3 = keyValuePair.Key.First;
				Vector2 vector = value.First + (value.Second - value.First) * second;
				PointD pointD = new PointD(vector.x, vector.y);
				double num4 = 1.0 / (num3 * num3);
				num += (particle.X - pointD.X) / num3 * num4;
				num2 += (particle.Y - pointD.Y) / num3 * num4;
			}
			if (bounds.Contains(point))
			{
				return new PointD(num, num2);
			}
			return new PointD(0.0 - num, 0.0 - num2);
		}

		public PointD GetPositionForNode(Satsuma.Node node)
		{
			Node node2 = nodeList.Find((N n) => n.node == node);
			return new PointD(node2.position.x, node2.position.y);
		}

		public void SetInitialNodePositions(Polygon bounds)
		{
			List<Vector2> randomPoints = PointGenerator.GetRandomPoints(bounds, 50f, 0f, null, PointGenerator.SampleBehaviour.PoissonDisk, testInsideBounds: true, myRandom);
			int num = 0;
			for (int i = 0; i < nodeList.Count; i++)
			{
				if (num == randomPoints.Count - 1)
				{
					randomPoints = PointGenerator.GetRandomPoints(bounds, 10f, 20f, randomPoints, PointGenerator.SampleBehaviour.PoissonDisk, testInsideBounds: true, myRandom);
					num = 0;
				}
				nodeList[i].SetPosition(randomPoints[num++]);
			}
		}

		public bool Layout(Polygon bounds = null)
		{
			bool flag = false;
			int num = 0;
			Vector2 vector = default(Vector2);
			while (!flag && num < 100)
			{
				flag = true;
				Func<Satsuma.Node, PointD> initialPositions = (Satsuma.Node n) => GetPositionForNode(n);
				CustomGraph baseGraph = this.baseGraph;
				int seed = num;
				ForceDirectedLayout forceDirectedLayout = new ForceDirectedLayout(baseGraph, initialPositions, seed);
				forceDirectedLayout.ExternalForce = (PointD point) => GetForceForBoundry(point, bounds);
				forceDirectedLayout.Run();
				IEnumerator<Satsuma.Node> enumerator = this.baseGraph.Nodes().GetEnumerator();
				int num2 = 0;
				while (enumerator.MoveNext())
				{
					Satsuma.Node node = enumerator.Current;
					Node node2 = nodeList.Find((N n) => n.node == node);
					if (node2 != null)
					{
						vector.x = (float)forceDirectedLayout.NodePositions[node].X;
						vector.y = (float)forceDirectedLayout.NodePositions[node].Y;
						if (!bounds.Contains(vector))
						{
							flag = false;
							Debug.LogWarning("Re-doing layout - cell was off map");
							break;
						}
						node2.SetPosition(vector);
					}
					if (!flag)
					{
						break;
					}
					num2++;
				}
				num++;
			}
			if (num >= 10)
			{
				Debug.LogWarning("Re-ran layout " + num + " times");
			}
			return flag;
		}
	}
}
