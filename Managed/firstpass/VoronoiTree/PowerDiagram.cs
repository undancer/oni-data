using System;
using System.Collections.Generic;
using System.Linq;
using Delaunay.Geo;
using MIConvexHull;
using UnityEngine;

namespace VoronoiTree
{
	public class PowerDiagram
	{
		private class Edge : MathUtil.Pair<DualSite3d, DualSite3d>
		{
			public Edge(DualSite3d first, DualSite3d second)
			{
				base.First = first;
				base.Second = second;
			}
		}

		public class ConvexFaceExt<TVertex> : ConvexFace<TVertex, ConvexFaceExt<TVertex>> where TVertex : IVertex
		{
			private PowerDiagramSite site;

			private Vector2 dualPoint;

			private Vector2? circumCenter;

			public TVertex vertex0 => base.Vertices[0];

			public TVertex vertex1 => base.Vertices[1];

			public TVertex vertex2 => base.Vertices[2];

			public ConvexFaceExt<TVertex> edge0 => base.Adjacency[0];

			public ConvexFaceExt<TVertex> edge1 => base.Adjacency[1];

			public ConvexFaceExt<TVertex> edge2 => base.Adjacency[2];

			public Vector2 Circumcenter
			{
				get
				{
					circumCenter = circumCenter ?? GetCircumcenter();
					return circumCenter.Value;
				}
			}

			public Vector2 GetDualPoint()
			{
				if (dualPoint.x == 0f && dualPoint.y == 0f)
				{
					Vector3 vector = new Vector3((float)vertex0.Position[0], (float)vertex0.Position[1], (float)vertex0.Position[2]);
					Vector3 vector2 = new Vector3((float)vertex1.Position[0], (float)vertex1.Position[1], (float)vertex1.Position[2]);
					Vector3 vector3 = new Vector3((float)vertex2.Position[0], (float)vertex2.Position[1], (float)vertex2.Position[2]);
					double num = vector.y * (vector2.z - vector3.z) + vector2.y * (vector3.z - vector.z) + vector3.y * (vector.z - vector2.z);
					double num2 = vector.z * (vector2.x - vector3.x) + vector2.z * (vector3.x - vector.x) + vector3.z * (vector.x - vector2.x);
					double num3 = -0.5 / (double)(vector.x * (vector2.y - vector3.y) + vector2.x * (vector3.y - vector.y) + vector3.x * (vector.y - vector2.y));
					dualPoint = new Vector2((float)(num * num3), (float)(num2 * num3));
				}
				return dualPoint;
			}

			private double Det(double[,] m)
			{
				return m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) - m[0, 1] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]) + m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);
			}

			private double LengthSquared(double[] v)
			{
				double num = 0.0;
				foreach (double num2 in v)
				{
					num += num2 * num2;
				}
				return num;
			}

			private Vector2 GetCircumcenter()
			{
				TVertex[] vertices = base.Vertices;
				double[,] array = new double[3, 3];
				for (int i = 0; i < 3; i++)
				{
					array[i, 0] = vertices[i].Position[0];
					array[i, 1] = vertices[i].Position[1];
					array[i, 2] = 1.0;
				}
				double num = Det(array);
				double num2 = -1.0 / (2.0 * num);
				for (int j = 0; j < 3; j++)
				{
					array[j, 0] = LengthSquared(vertices[j].Position);
				}
				double num3 = 0.0 - Det(array);
				for (int k = 0; k < 3; k++)
				{
					array[k, 1] = vertices[k].Position[0];
				}
				double num4 = Det(array);
				return new Vector2((float)(num2 * num3), (float)(num2 * num4));
			}
		}

		public class TriangulationCellExt<TVertex> : TriangulationCell<TVertex, TriangulationCellExt<TVertex>> where TVertex : IVertex
		{
			public TVertex Vertex0 => base.Vertices[0];

			public TVertex Vertex1 => base.Vertices[1];

			public TVertex Vertex2 => base.Vertices[2];

			public TriangulationCellExt<TVertex> Edge0 => base.Adjacency[0];

			public TriangulationCellExt<TVertex> Edge1 => base.Adjacency[1];

			public TriangulationCellExt<TVertex> Edge2 => base.Adjacency[2];
		}

		public class DualSite2d : IVertex
		{
			public double[] Position => new double[2]
			{
				site.position[0],
				site.position[1]
			};

			public PowerDiagramSite site
			{
				get;
				set;
			}

			public bool visited
			{
				get;
				set;
			}

			public DualSite2d(PowerDiagramSite site)
			{
				this.site = site;
				visited = false;
			}
		}

		public class DualSite3d : IVertex
		{
			public double[] Position => new double[3]
			{
				coord[0],
				coord[1],
				coord[2]
			};

			public Vector3 coord
			{
				get;
				set;
			}

			public PowerDiagramSite site
			{
				get;
				set;
			}

			public bool visited
			{
				get;
				set;
			}

			public DualSite3d()
				: this(0.0, 0.0, 0.0)
			{
			}

			public DualSite3d(double _x, double _y, double _z)
			{
				coord = new Vector3((float)_x, (float)_y, (float)_z);
				visited = false;
			}

			public DualSite3d(Vector3 pos)
			{
				coord = pos;
				visited = false;
			}

			public DualSite3d(double _x, double _y, double _z, PowerDiagramSite _originalSite)
				: this(_x, _y, _z)
			{
				site = _originalSite;
				visited = false;
			}
		}

		public const Winding ForcedWinding = Winding.COUNTERCLOCKWISE;

		private Polygon bounds;

		private List<PowerDiagramSite> externalEdgePoints = new List<PowerDiagramSite>();

		private float weightSum;

		private List<PowerDiagramSite> sites = new List<PowerDiagramSite>();

		private List<DualSite2d> dualSites = new List<DualSite2d>();

		private ConvexHull<DualSite3d, ConvexFaceExt<DualSite3d>> debug_LastHull;

		public VoronoiMesh<DualSite2d, PowerDiagramSite, VoronoiEdge<DualSite2d, PowerDiagramSite>> voronoiMesh
		{
			get;
			private set;
		}

		public int completedIterations
		{
			get;
			set;
		}

		public List<PowerDiagramSite> GetSites()
		{
			return sites;
		}

		public PowerDiagram(Polygon polyBounds, IEnumerable<PowerDiagramSite> newSites)
		{
			bounds = polyBounds;
			bounds.ForceWinding(Winding.COUNTERCLOCKWISE);
			weightSum = 0f;
			sites.Clear();
			IEnumerator<PowerDiagramSite> enumerator = newSites.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				if (!bounds.Contains(enumerator.Current.position))
				{
					Debug.LogErrorFormat("Cant feed points [{0}] to powerdiagram that are outside its area [{1}] ", enumerator.Current.id, enumerator.Current.position);
				}
				if (bounds.Contains(enumerator.Current.position))
				{
					AddSite(enumerator.Current);
				}
				num++;
			}
			Vector2 b = bounds.Centroid();
			for (int i = 0; i < bounds.Vertices.Count; i++)
			{
				Vector2 vector = bounds.Vertices[i];
				Vector2 vector2 = bounds.Vertices[(i < bounds.Vertices.Count - 1) ? (i + 1) : 0];
				Vector2 b2 = (vector - b).normalized * 1000f;
				PowerDiagramSite powerDiagramSite = new PowerDiagramSite(vector + b2)
				{
					dummy = true
				};
				externalEdgePoints.Add(powerDiagramSite);
				powerDiagramSite.weight = Mathf.Epsilon;
				powerDiagramSite.currentWeight = Mathf.Epsilon;
				dualSites.Add(new DualSite2d(powerDiagramSite));
				Vector2 b3 = ((vector2 - vector) * 0.5f + vector2 - b).normalized * 1000f;
				PowerDiagramSite powerDiagramSite2 = new PowerDiagramSite(vector2 + b3)
				{
					dummy = true,
					weight = Mathf.Epsilon,
					currentWeight = Mathf.Epsilon
				};
				externalEdgePoints.Add(powerDiagramSite2);
				dualSites.Add(new DualSite2d(powerDiagramSite2));
			}
		}

		public void ComputePowerDiagram(int maxIterations, float threashold = 1f)
		{
			completedIterations = 0;
			float num = 0f;
			foreach (PowerDiagramSite site in sites)
			{
				if (site.poly != null)
				{
					site.position = site.poly.Centroid();
					continue;
				}
				string str = site.id.ToString();
				Vector2 position = site.position;
				throw new Exception("site poly is null for [" + str + "]" + position.ToString());
			}
			for (int i = 0; i <= maxIterations; i++)
			{
				try
				{
					UpdateWeights(sites);
					ComputePD();
				}
				catch (Exception ex)
				{
					Debug.LogError("Error [" + num + "] iters " + completedIterations + "/" + maxIterations + " Exception:" + ex.Message + "\n" + ex.StackTrace);
					return;
				}
				num = 0f;
				foreach (PowerDiagramSite site2 in sites)
				{
					float num2 = ((site2.poly == null) ? 0.1f : site2.poly.Area());
					float num3 = site2.weight / weightSum * bounds.Area();
					num = Mathf.Max(Mathf.Abs(num2 - num3) / num3, num);
				}
				if (num < threashold)
				{
					completedIterations = i;
					break;
				}
				completedIterations++;
			}
		}

		public void ComputeVD()
		{
			voronoiMesh = VoronoiMesh.Create<DualSite2d, PowerDiagramSite>(dualSites);
			foreach (PowerDiagramSite vertex in voronoiMesh.Vertices)
			{
				_ = vertex.Circumcenter;
				DualSite2d[] vertices = vertex.Vertices;
				foreach (DualSite2d dualSite2d in vertices)
				{
					if (dualSite2d.visited)
					{
						continue;
					}
					dualSite2d.visited = true;
					if (dualSite2d.site.dummy)
					{
						continue;
					}
					List<Vector2> list = new List<Vector2>();
					dualSite2d.site.neighbours = TouchingFaces(dualSite2d, vertex);
					foreach (PowerDiagramSite neighbour in dualSite2d.site.neighbours)
					{
						Vector2 circumcenter = neighbour.Circumcenter;
						Color red = Color.red;
						red.a = 0.3f;
						list.Add(circumcenter);
					}
					if (list.Count > 0)
					{
						Polygon polygon = PolyForRandomPoints(list);
						dualSite2d.site.poly = polygon.Clip(bounds);
					}
				}
			}
		}

		public void ComputeVD3d()
		{
			List<DualSite3d> list = new List<DualSite3d>();
			foreach (PowerDiagramSite site in sites)
			{
				list.Add(site.ToDualSite());
			}
			for (int i = 0; i < externalEdgePoints.Count; i++)
			{
				list.Add(externalEdgePoints[i].ToDualSite());
			}
			foreach (TriangulationCellExt<DualSite3d> vertex in VoronoiMesh.Create<DualSite3d, TriangulationCellExt<DualSite3d>>(list).Vertices)
			{
				Vector3 zero = Vector3.zero;
				DualSite3d[] vertices = vertex.Vertices;
				foreach (DualSite3d dualSite3d in vertices)
				{
					zero += dualSite3d.coord;
				}
				zero *= 0.33333334f;
				DebugExtension.DebugPoint(zero, Color.red);
			}
		}

		private bool ContainsVert(PowerDiagramSite face, DualSite2d target)
		{
			if (face == null || face.Vertices == null)
			{
				return false;
			}
			for (int i = 0; i < face.Vertices.Length; i++)
			{
				if (face.Vertices[i] == target)
				{
					return true;
				}
			}
			return false;
		}

		private void AddSite(PowerDiagramSite site)
		{
			weightSum += site.weight;
			site.currentWeight = site.weight;
			sites.Add(site);
			dualSites.Add(new DualSite2d(site));
		}

		private List<PowerDiagramSite> TouchingFaces(DualSite2d site, PowerDiagramSite startingFace)
		{
			List<PowerDiagramSite> list = new List<PowerDiagramSite>();
			Stack<PowerDiagramSite> stack = new Stack<PowerDiagramSite>();
			stack.Push(startingFace);
			while (stack.Count > 0)
			{
				PowerDiagramSite powerDiagramSite = stack.Pop();
				if (!ContainsVert(powerDiagramSite, site) || list.Contains(powerDiagramSite))
				{
					continue;
				}
				list.Add(powerDiagramSite);
				for (int i = 0; i < powerDiagramSite.Adjacency.Length; i++)
				{
					if (ContainsVert(powerDiagramSite.Adjacency[i], site))
					{
						stack.Push(powerDiagramSite.Adjacency[i]);
					}
				}
			}
			return list;
		}

		private ConvexFaceExt<DualSite3d> GetNeigborFaceForEdge(ConvexFaceExt<DualSite3d> currentFace, DualSite3d sharedVert0, DualSite3d sharedVert1)
		{
			for (int i = 0; i < currentFace.Adjacency.Length; i++)
			{
				ConvexFaceExt<DualSite3d> convexFaceExt = currentFace.Adjacency[i];
				if (convexFaceExt == null)
				{
					continue;
				}
				int num = 0;
				for (int j = 0; j < convexFaceExt.Vertices.Length; j++)
				{
					if (sharedVert0 == convexFaceExt.Vertices[j])
					{
						num++;
					}
					else if (sharedVert1 == convexFaceExt.Vertices[j])
					{
						num++;
					}
					if (num == 2)
					{
						return convexFaceExt;
					}
				}
			}
			return null;
		}

		private Edge GetEdge(ConvexFaceExt<DualSite3d> face0, ConvexFaceExt<DualSite3d> face1)
		{
			Edge edge = null;
			for (int i = 0; i < face0.Vertices.Length; i++)
			{
				for (int j = 0; j < face1.Vertices.Length; j++)
				{
					if (face0.Vertices[i] == face1.Vertices[j])
					{
						if (edge == null)
						{
							edge = new Edge(face0.Vertices[i], null);
						}
						else
						{
							edge.Second = face0.Vertices[i];
						}
					}
				}
			}
			return edge;
		}

		private bool ContainsVert(ConvexFaceExt<DualSite3d> face, DualSite3d target)
		{
			for (int i = 0; i < face.Vertices.Length; i++)
			{
				if (face.Vertices[i] == target)
				{
					return true;
				}
			}
			return false;
		}

		private List<ConvexFaceExt<DualSite3d>> TouchingFaces(DualSite3d site, ConvexFaceExt<DualSite3d> startingFace)
		{
			List<ConvexFaceExt<DualSite3d>> list = new List<ConvexFaceExt<DualSite3d>>();
			Stack<ConvexFaceExt<DualSite3d>> stack = new Stack<ConvexFaceExt<DualSite3d>>();
			stack.Push(startingFace);
			while (stack.Count > 0)
			{
				ConvexFaceExt<DualSite3d> convexFaceExt = stack.Pop();
				if (!ContainsVert(convexFaceExt, site) || list.Contains(convexFaceExt))
				{
					continue;
				}
				list.Add(convexFaceExt);
				for (int i = 0; i < convexFaceExt.Adjacency.Length; i++)
				{
					if (ContainsVert(convexFaceExt.Adjacency[i], site) && !list.Contains(convexFaceExt.Adjacency[i]))
					{
						stack.Push(convexFaceExt.Adjacency[i]);
					}
				}
			}
			return list;
		}

		private List<PowerDiagramSite> GenerateNeighbors(DualSite3d dualSite, ConvexFaceExt<DualSite3d> startingFace)
		{
			List<PowerDiagramSite> list = new List<PowerDiagramSite>();
			List<ConvexFaceExt<DualSite3d>> list2 = new List<ConvexFaceExt<DualSite3d>>();
			Stack<ConvexFaceExt<DualSite3d>> stack = new Stack<ConvexFaceExt<DualSite3d>>();
			stack.Push(startingFace);
			while (stack.Count > 0)
			{
				ConvexFaceExt<DualSite3d> convexFaceExt = stack.Pop();
				list2.Add(convexFaceExt);
				for (int i = 0; i < convexFaceExt.Adjacency.Length; i++)
				{
					if (ContainsVert(convexFaceExt.Adjacency[i], dualSite) && !list2.Contains(convexFaceExt.Adjacency[i]))
					{
						Edge edge = GetEdge(convexFaceExt, convexFaceExt.Adjacency[i]);
						DualSite3d dualSite3d = ((edge.First == dualSite) ? edge.Second : edge.First);
						Debug.Assert(dualSite3d != dualSite, "We're our own neighbour??");
						Debug.Assert(dualSite3d.site.id == -1 || !list.Contains(dualSite3d.site), "Tried adding a site twice!");
						list.Add(dualSite3d.site);
						stack.Push(convexFaceExt.Adjacency[i]);
					}
				}
			}
			return list;
		}

		private void ComputePD()
		{
			List<DualSite3d> list = new List<DualSite3d>();
			foreach (PowerDiagramSite site in sites)
			{
				list.Add(site.ToDualSite());
			}
			for (int i = 0; i < externalEdgePoints.Count; i++)
			{
				list.Add(externalEdgePoints[i].ToDualSite());
			}
			CheckPositions(list);
			ConvexHull<DualSite3d, ConvexFaceExt<DualSite3d>> convexHull = CreateHull(list);
			foreach (ConvexFaceExt<DualSite3d> face in convexHull.Faces)
			{
				if (face.Normal[2] >= (double)(0f - Mathf.Epsilon))
				{
					continue;
				}
				DualSite3d[] vertices = face.Vertices;
				foreach (DualSite3d dualSite3d in vertices)
				{
					if (dualSite3d.site.dummy || dualSite3d.visited)
					{
						continue;
					}
					dualSite3d.visited = true;
					List<Vector2> list2 = new List<Vector2>();
					List<ConvexFaceExt<DualSite3d>> list3 = TouchingFaces(dualSite3d, face);
					dualSite3d.site.neighbours = GenerateNeighbors(dualSite3d, face);
					foreach (ConvexFaceExt<DualSite3d> item in list3)
					{
						Vector2 dualPoint = item.GetDualPoint();
						list2.Add(dualPoint);
					}
					Polygon polygon = PolyForRandomPoints(list2).Clip(bounds);
					if (polygon == null)
					{
						DebugExtension.DebugCircle2d(dualSite3d.site.position, Color.magenta, 5f, 0f, depthTest: true, 20f);
					}
					else
					{
						dualSite3d.site.poly = polygon;
					}
				}
			}
			debug_LastHull = convexHull;
		}

		private void UpdateWeights(List<PowerDiagramSite> sites)
		{
			foreach (PowerDiagramSite site in sites)
			{
				if (site.poly != null)
				{
					site.position = site.poly.Centroid();
					site.currentWeight = Mathf.Max(site.currentWeight, 1f);
					continue;
				}
				string str = site.id.ToString();
				Vector2 position = site.position;
				throw new Exception("site poly is null for [" + str + "]" + position.ToString());
			}
			float num = 0f;
			foreach (PowerDiagramSite site2 in sites)
			{
				float num2 = ((site2.poly == null) ? 0.1f : site2.poly.Area());
				float num3 = site2.weight / weightSum * bounds.Area();
				float num4 = Mathf.Sqrt(num2 / (float)Math.PI);
				float num5 = Mathf.Sqrt(num3 / (float)Math.PI);
				float num6 = num4 - num5;
				float num7 = num3 / num2;
				if (((double)num7 > 1.1 && (double)site2.previousWeightAdaption < 0.9) || ((double)num7 < 0.9 && (double)site2.previousWeightAdaption > 1.1))
				{
					num7 = Mathf.Sqrt(num7);
				}
				if ((double)num7 < 1.1 && (double)num7 > 0.9 && site2.currentWeight != 1f)
				{
					num7 = Mathf.Sqrt(num7);
				}
				if (site2.currentWeight < 10f)
				{
					num7 *= num7;
				}
				if (site2.currentWeight > 10f)
				{
					num7 = Mathf.Sqrt(num7);
				}
				site2.previousWeightAdaption = num7;
				site2.currentWeight *= num7;
				if (!(site2.currentWeight < 1f))
				{
					continue;
				}
				float num8 = Mathf.Sqrt(site2.currentWeight) - num6;
				if (num8 < 0f)
				{
					site2.currentWeight = 0f - num8 * num8;
					if (site2.currentWeight < num)
					{
						num = site2.currentWeight;
					}
				}
			}
			if (num < 0f)
			{
				num = 0f - num;
				foreach (PowerDiagramSite site3 in sites)
				{
					site3.currentWeight += num + 1f;
				}
			}
			float num9 = 1f;
			foreach (PowerDiagramSite site4 in sites)
			{
				foreach (PowerDiagramSite neighbour in site4.neighbours)
				{
					float num10 = (site4.position - neighbour.position).sqrMagnitude / (Mathf.Abs(site4.currentWeight - neighbour.currentWeight) + 1f);
					if (num10 < num9)
					{
						num9 = num10;
					}
				}
			}
			foreach (PowerDiagramSite site5 in sites)
			{
				site5.currentWeight *= num9;
			}
		}

		private List<ConvexFaceExt<DualSite3d>> GetNeigborFaces(ConvexFaceExt<DualSite3d> currentFace)
		{
			List<ConvexFaceExt<DualSite3d>> list = new List<ConvexFaceExt<DualSite3d>>();
			for (int i = 0; i < currentFace.Adjacency.Length; i++)
			{
				ConvexFaceExt<DualSite3d> convexFaceExt = currentFace.Adjacency[i];
				if (convexFaceExt != null)
				{
					list.Add(convexFaceExt);
				}
			}
			return list;
		}

		private void CheckPositions(List<DualSite3d> dual3dSites)
		{
			for (int i = 0; i < dual3dSites.Count; i++)
			{
				if (dual3dSites[i].site.dummy)
				{
					continue;
				}
				Debug.Assert(dual3dSites[i].site.currentWeight != 0f);
				for (int j = i + 1; j < dual3dSites.Count; j++)
				{
					if (!dual3dSites[j].site.dummy && dual3dSites[i].coord == dual3dSites[j].coord)
					{
						dual3dSites[j].coord += new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, 0f);
					}
				}
			}
		}

		public static Polygon PolyForRandomPoints(List<Vector2> verts)
		{
			double[][] array = new double[verts.Count][];
			for (int i = 0; i < verts.Count; i++)
			{
				array[i] = new double[2]
				{
					verts[i].x,
					verts[i].y
				};
			}
			double[][] array2 = ConvexHull.Create(array).Points.Select((DefaultVertex p) => p.Position).ToArray();
			Polygon polygon = new Polygon();
			for (int j = 0; j < array2.Length; j++)
			{
				polygon.Add(new Vector2((float)array2[j][0], (float)array2[j][1]));
			}
			polygon.Initialize();
			polygon.ForceWinding(Winding.COUNTERCLOCKWISE);
			return polygon;
		}

		public static ConvexHull<DualSite3d, ConvexFaceExt<DualSite3d>> CreateHull(IList<DualSite3d> data, double PlaneDistanceTolerance = 1E-10)
		{
			return ConvexHull<DualSite3d, ConvexFaceExt<DualSite3d>>.Create(data, PlaneDistanceTolerance);
		}
	}
}
