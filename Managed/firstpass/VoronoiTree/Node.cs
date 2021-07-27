using System;
using System.Collections.Generic;
using Delaunay.Geo;
using KSerialization;
using UnityEngine;

namespace VoronoiTree
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Node
	{
		public enum NodeType
		{
			Unknown,
			Internal,
			Leaf
		}

		public enum VisitedType
		{
			MissingData = -2,
			Error,
			NotVisited,
			VisitedSuccess
		}

		public class SplitCommand
		{
			public enum SplitType
			{
				KeepParentAsCentroid = 1,
				ChildrenDuplicateParent = 2,
				ChildrenChosenFromLayer = 4
			}

			public delegate string NodeTypeOverride(Vector2 position);

			public SplitType splitType;

			public TagSet dontCopyTags;

			public TagSet moveTags;

			public int minChildCount = 2;

			public NodeTypeOverride typeOverride;

			public Action<Tree, SplitCommand> SplitFunction;
		}

		public static int maxDepth;

		public static uint maxIndex;

		[Serialize]
		public NodeType type;

		public VisitedType visited;

		public LoggerSSF log;

		[Serialize]
		public Diagram.Site site;

		[Serialize]
		public TagSet tags;

		public Dictionary<Tag, int> minDistanceToTag = new Dictionary<Tag, int>();

		public Tree parent { get; private set; }

		public PowerDiagram debug_LastPD { get; private set; }

		public void SetParent(Tree newParent)
		{
			parent = newParent;
		}

		public Node()
		{
			type = NodeType.Unknown;
			log = new LoggerSSF("VoronoiNode", 100);
		}

		public Node(NodeType type)
		{
			this.type = type;
			tags = new TagSet();
			log = new LoggerSSF("VoronoiNode", 100);
		}

		protected Node(Diagram.Site site, NodeType type, Tree parent)
		{
			tags = new TagSet();
			this.site = site;
			this.type = type;
			this.parent = parent;
			log = new LoggerSSF("VoronoiNode", 100);
		}

		public Node GetNeighbour(uint id)
		{
			HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = site.neighbours.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Key == id)
				{
					return GetSibling(id);
				}
			}
			return null;
		}

		public List<Node> GetNeighbors()
		{
			List<Node> list = new List<Node>();
			if (site.neighbours != null)
			{
				HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = site.neighbours.GetEnumerator();
				while (enumerator.MoveNext())
				{
					list.Add(GetSibling(enumerator.Current.Key));
				}
			}
			return list;
		}

		public List<KeyValuePair<Node, LineSegment>> GetNeighborsByEdge()
		{
			List<KeyValuePair<Node, LineSegment>> list = new List<KeyValuePair<Node, LineSegment>>();
			for (int i = 0; i < site.poly.Vertices.Count; i++)
			{
				if (site.neighbours == null)
				{
					continue;
				}
				LineSegment edge = site.poly.GetEdge(i);
				Node node = null;
				HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = site.neighbours.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Value == i)
					{
						node = GetSibling(enumerator.Current.Key);
					}
				}
				if (node != null)
				{
					list.Add(new KeyValuePair<Node, LineSegment>(node, edge));
				}
			}
			return list;
		}

		public Node GetSibling(uint siteId)
		{
			return parent.GetChildByID(siteId);
		}

		public List<Node> GetSiblings()
		{
			List<Node> list = new List<Node>();
			for (int i = 0; i < parent.ChildCount(); i++)
			{
				Node child = parent.GetChild(i);
				if (child != this)
				{
					list.Add(child);
				}
			}
			return list;
		}

		public void PlaceSites(List<Diagram.Site> sites, int seed)
		{
			SeededRandom seededRandom = new SeededRandom(seed);
			List<Vector2> list = null;
			List<Vector2> list2 = new List<Vector2>();
			for (int i = 0; i < sites.Count; i++)
			{
				list2.Add(sites[i].position);
			}
			int num = 0;
			for (int j = 0; j < sites.Count; j++)
			{
				if (!site.poly.Contains(sites[j].position))
				{
					if (list == null)
					{
						list = PointGenerator.GetRandomPoints(site.poly, 5f, 1f, list2, PointGenerator.SampleBehaviour.PoissonDisk, testInsideBounds: true, seededRandom);
					}
					if (num >= list.Count - 1)
					{
						list2.AddRange(list);
						list = PointGenerator.GetRandomPoints(site.poly, 0.5f, 0.5f, list2, PointGenerator.SampleBehaviour.PoissonDisk, testInsideBounds: true, seededRandom);
						num = 0;
					}
					if (list.Count == 0)
					{
						sites[j].position = sites[0].position + Vector2.one * seededRandom.RandomValue();
					}
					else
					{
						sites[j].position = list[num++];
					}
				}
			}
			HashSet<Vector2> hashSet = new HashSet<Vector2>();
			for (int k = 0; k < sites.Count; k++)
			{
				if (hashSet.Contains(sites[k].position))
				{
					visited = VisitedType.Error;
					sites[k].position += new Vector2(seededRandom.RandomRange(0, 1), seededRandom.RandomRange(0, 1));
				}
				hashSet.Add(sites[k].position);
				sites[k].poly = null;
			}
		}

		public bool ComputeNode(List<Diagram.Site> diagramSites)
		{
			if (site.poly == null || diagramSites == null || diagramSites.Count == 0)
			{
				visited = VisitedType.MissingData;
				return false;
			}
			visited = VisitedType.VisitedSuccess;
			if (diagramSites.Count == 1)
			{
				diagramSites[0].poly = site.poly;
				diagramSites[0].position = diagramSites[0].poly.Centroid();
				return true;
			}
			HashSet<Diagram.Site> hashSet = new HashSet<Diagram.Site>();
			for (int i = 0; i < diagramSites.Count; i++)
			{
				hashSet.Add(new Diagram.Site(diagramSites[i].id, diagramSites[i].position, diagramSites[i].weight));
			}
			hashSet.Add(new Diagram.Site(maxIndex + 1, new Vector2(site.poly.bounds.xMin - 500f, site.poly.bounds.yMin + site.poly.bounds.height / 2f)));
			hashSet.Add(new Diagram.Site(maxIndex + 2, new Vector2(site.poly.bounds.xMax + 500f, site.poly.bounds.yMin + site.poly.bounds.height / 2f)));
			hashSet.Add(new Diagram.Site(maxIndex + 3, new Vector2(site.poly.bounds.xMin + site.poly.bounds.width / 2f, site.poly.bounds.yMin - 500f)));
			hashSet.Add(new Diagram.Site(maxIndex + 4, new Vector2(site.poly.bounds.xMin + site.poly.bounds.width / 2f, site.poly.bounds.yMax + 500f)));
			Diagram diagram = new Diagram(new Rect(site.poly.bounds.xMin - 500f, site.poly.bounds.yMin - 500f, site.poly.bounds.width + 500f, site.poly.bounds.height + 500f), hashSet);
			for (int j = 0; j < diagramSites.Count; j++)
			{
				if (diagramSites[j].id > maxIndex)
				{
					continue;
				}
				List<Vector2> list = diagram.diagram.Region(diagramSites[j].position);
				if (list == null)
				{
					if (type != NodeType.Leaf)
					{
						visited = VisitedType.Error;
						return false;
					}
					continue;
				}
				Polygon polygon = new Polygon(list).Clip(site.poly);
				if (polygon == null || polygon.Vertices.Count < 3)
				{
					if (type != NodeType.Leaf)
					{
						visited = VisitedType.Error;
						return false;
					}
				}
				else
				{
					diagramSites[j].poly = polygon;
				}
			}
			for (int k = 0; k < diagramSites.Count; k++)
			{
				if (diagramSites[k].id <= maxIndex)
				{
					HashSet<uint> neighbours = diagram.diagram.NeighborSitesIDsForSite(diagramSites[k].position);
					FilterNeighbours(diagramSites[k], neighbours, diagramSites);
					diagramSites[k].position = diagramSites[k].poly.Centroid();
				}
			}
			return true;
		}

		public bool ComputeNodePD(List<Diagram.Site> diagramSites, int maxIters = 500, float threshold = 0.2f)
		{
			if (site.poly == null || diagramSites == null || diagramSites.Count == 0)
			{
				visited = VisitedType.MissingData;
				return false;
			}
			visited = VisitedType.VisitedSuccess;
			List<PowerDiagramSite> list = new List<PowerDiagramSite>();
			for (int i = 0; i < diagramSites.Count; i++)
			{
				PowerDiagramSite item = new PowerDiagramSite(diagramSites[i].id, diagramSites[i].position, diagramSites[i].weight);
				list.Add(item);
			}
			PowerDiagram powerDiagram = new PowerDiagram(site.poly, list);
			powerDiagram.ComputeVD();
			powerDiagram.ComputePowerDiagram(maxIters, threshold);
			for (int j = 0; j < diagramSites.Count; j++)
			{
				diagramSites[j].poly = list[j].poly;
				if (diagramSites[j].poly == null)
				{
					Debug.LogErrorFormat("Site [{0}] at index [{1}]: Poly shouldnt be null here ever", diagramSites[j].id, j);
				}
			}
			for (int k = 0; k < diagramSites.Count; k++)
			{
				HashSet<uint> hashSet = new HashSet<uint>();
				for (int l = 0; l < list[k].neighbours.Count; l++)
				{
					if (!list[k].neighbours[l].dummy)
					{
						hashSet.Add((uint)list[k].neighbours[l].id);
					}
				}
				FilterNeighbours(diagramSites[k], hashSet, diagramSites);
				diagramSites[k].position = diagramSites[k].poly.Centroid();
			}
			debug_LastPD = powerDiagram;
			return true;
		}

		private static void FilterNeighbours(Diagram.Site home, HashSet<uint> neighbours, List<Diagram.Site> sites)
		{
			if (home == null)
			{
				Debug.LogError("FilterNeighbours home == null");
			}
			HashSet<KeyValuePair<uint, int>> hashSet = new HashSet<KeyValuePair<uint, int>>();
			HashSet<uint>.Enumerator niter = neighbours.GetEnumerator();
			while (niter.MoveNext())
			{
				Diagram.Site site = sites.Find((Diagram.Site s) => s.id == niter.Current);
				if (site != null)
				{
					if (site.poly == null)
					{
						Debug.LogError("FilterNeighbours neighbour.poly == null");
					}
					int edgeIdx = -1;
					Polygon.DebugLog($"Testing for {home.id} common edge with {site.id}");
					if (home.poly.SharesEdge(site.poly, ref edgeIdx, out var _) == Polygon.Commonality.Edge)
					{
						hashSet.Add(new KeyValuePair<uint, int>(niter.Current, edgeIdx));
						Polygon.DebugLog($" -> {home.id} common edge with {site.id}: {edgeIdx}");
					}
					else
					{
						Polygon.DebugLog($" -> {home.id} NO COMMON with {site.id}: {edgeIdx}");
					}
				}
			}
			home.neighbours = hashSet;
		}

		public void Reset(List<Diagram.Site> sites = null)
		{
			visited = VisitedType.NotVisited;
			if (sites == null)
			{
				return;
			}
			HashSet<Vector2> hashSet = new HashSet<Vector2>();
			for (int i = 0; i < sites.Count; i++)
			{
				if (hashSet.Contains(sites[i].position))
				{
					visited = VisitedType.Error;
					break;
				}
				hashSet.Add(sites[i].position);
			}
		}

		public void SetTags(TagSet originalTags)
		{
			tags = new TagSet(originalTags);
		}

		public void AddTag(Tag tag)
		{
			if (tags == null)
			{
				tags = new TagSet();
			}
			tags.Add(tag);
		}

		public void AddTagToNeighbors(Tag tag)
		{
			HashSet<KeyValuePair<uint, int>>.Enumerator enumerator = site.neighbours.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GetNeighbour(enumerator.Current.Key).AddTag(tag);
			}
		}
	}
}
