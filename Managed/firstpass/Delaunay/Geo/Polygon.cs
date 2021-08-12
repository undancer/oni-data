using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClipperLib;
using KSerialization;
using UnityEngine;

namespace Delaunay.Geo
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public sealed class Polygon
	{
		public enum Commonality
		{
			None,
			Point,
			Edge
		}

		[Serialize]
		private List<Vector2> vertices;

		private Vector2? centroid;

		public static bool DoDebugSpew;

		private const int CLIPPER_INTEGER_SCALE = 10000;

		private const float CLIPPER_INVERSE_SCALE = 0.0001f;

		public Rect bounds { get; private set; }

		public List<Vector2> Vertices => vertices;

		public float MinX => vertices.Min((Vector2 point) => point.x);

		public float MinY => vertices.Min((Vector2 point) => point.y);

		public float MaxX => vertices.Max((Vector2 point) => point.x);

		public float MaxY => vertices.Max((Vector2 point) => point.y);

		public Polygon()
		{
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			vertices = new List<Vector2>();
		}

		[OnDeserialized]
		internal void OnDeserializedMethod()
		{
			Initialize();
		}

		public Polygon(List<Vector2> verts)
		{
			vertices = verts;
			Initialize();
		}

		public Polygon(Rect bounds)
		{
			vertices = new List<Vector2>();
			vertices.Add(new Vector2(bounds.x, bounds.y));
			vertices.Add(new Vector2(bounds.x + bounds.width, bounds.y));
			vertices.Add(new Vector2(bounds.x + bounds.width, bounds.y + bounds.height));
			vertices.Add(new Vector2(bounds.x, bounds.y + bounds.height));
			Initialize();
		}

		public void Add(Vector2 newVert)
		{
			if (vertices == null)
			{
				vertices = new List<Vector2>();
			}
			vertices.Add(newVert);
		}

		public void Initialize()
		{
			RefreshBounds();
		}

		public void RefreshBounds()
		{
			Debug.Assert(vertices != null, "No verts added");
			Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
			for (int i = 0; i < vertices.Count; i++)
			{
				if (vertices[i].y < vector.y)
				{
					vector.y = vertices[i].y;
				}
				if (vertices[i].x < vector.x)
				{
					vector.x = vertices[i].x;
				}
				if (vertices[i].y > vector2.y)
				{
					vector2.y = vertices[i].y;
				}
				if (vertices[i].x > vector2.x)
				{
					vector2.x = vertices[i].x;
				}
			}
			bounds = Rect.MinMaxRect(vector.x, vector.y, vector2.x, vector2.y);
		}

		public float Area()
		{
			return Mathf.Abs(SignedDoubleArea() * 0.5f);
		}

		public Winding Winding()
		{
			float num = SignedDoubleArea();
			if (num < 0f)
			{
				return Delaunay.Geo.Winding.CLOCKWISE;
			}
			if (num > 0f)
			{
				return Delaunay.Geo.Winding.COUNTERCLOCKWISE;
			}
			return Delaunay.Geo.Winding.NONE;
		}

		public void ForceWinding(Winding wind)
		{
			if (Winding() != wind)
			{
				vertices.Reverse();
			}
		}

		private float SignedDoubleArea()
		{
			int count = vertices.Count;
			float num = 0f;
			for (int i = 0; i < count; i++)
			{
				int index = (i + 1) % count;
				Vector2 vector = vertices[i];
				Vector2 vector2 = vertices[index];
				num += vector.x * vector2.y - vector2.x * vector.y;
			}
			return num;
		}

		public Vector2 Centroid()
		{
			if (!centroid.HasValue)
			{
				centroid = Vector2.zero;
				if (vertices.Count > 1)
				{
					float num = Area();
					int num2 = 1;
					for (int i = 0; i < vertices.Count; i++)
					{
						float num3 = vertices[i].x * vertices[num2].y - vertices[num2].x * vertices[i].y;
						centroid += new Vector2((vertices[i].x + vertices[num2].x) * num3, (vertices[i].y + vertices[num2].y) * num3);
						num2 = (num2 + 1) % vertices.Count;
					}
					centroid /= 6f * num;
				}
			}
			return centroid.Value;
		}

		public bool PointInPolygon(Vector2I point)
		{
			return PointInPolygon(new Vector2(point.x, point.y));
		}

		public bool Contains(Vector2 point)
		{
			return PointInPolygon(point);
		}

		public bool PointInPolygon(Vector2 point)
		{
			if (!bounds.Contains(point))
			{
				return false;
			}
			int index = vertices.Count - 1;
			bool flag = false;
			int num = 0;
			while (num < vertices.Count)
			{
				if (((vertices[num].y <= point.y && point.y < vertices[index].y) || (vertices[index].y <= point.y && point.y < vertices[num].y)) && point.x < (vertices[index].x - vertices[num].x) * (point.y - vertices[num].y) / (vertices[index].y - vertices[num].y) + vertices[num].x)
				{
					flag = !flag;
				}
				index = num++;
			}
			return flag;
		}

		public LineSegment GetEdge(int edgeIndex)
		{
			return new LineSegment(vertices[edgeIndex], vertices[(edgeIndex + 1) % vertices.Count]);
		}

		public Commonality SharesEdgeClosest(Polygon other)
		{
			Commonality result = Commonality.None;
			float timeOnEdge = 0f;
			MathUtil.Pair<Vector2, Vector2> closestEdge = GetClosestEdge(other.Centroid(), ref timeOnEdge);
			MathUtil.Pair<Vector2, Vector2> closestEdge2 = other.GetClosestEdge(Centroid(), ref timeOnEdge);
			if (Vector2.Distance(closestEdge.First, closestEdge2.First) < 1E-05f || Vector2.Distance(closestEdge.First, closestEdge2.Second) < 1E-05f)
			{
				if (Vector2.Distance(closestEdge.Second, closestEdge2.First) < 1E-05f || Vector2.Distance(closestEdge.Second, closestEdge2.Second) < 1E-05f)
				{
					return Commonality.Edge;
				}
				return Commonality.Point;
			}
			return result;
		}

		public static void DebugLog(string message)
		{
			if (DoDebugSpew)
			{
				Debug.Log(message);
			}
		}

		public Commonality SharesEdge(Polygon other, ref int edgeIdx, out LineSegment overlapSegment)
		{
			Commonality result = Commonality.None;
			int num = vertices.Count - 1;
			int num2 = 0;
			while (num2 < vertices.Count)
			{
				Vector2 vector = vertices[num];
				Vector2 vector2 = vertices[num2];
				Bounds bounds = new Bounds(vector, Vector3.zero);
				bounds.Encapsulate(vector2);
				int index = other.vertices.Count - 1;
				int num3 = 0;
				while (num3 < other.vertices.Count)
				{
					Vector2 vector3 = other.vertices[index];
					Vector2 vector4 = other.vertices[num3];
					if (0 + ((Vector2.Distance(vector4, vector2) < 0.001f) ? 1 : 0) + ((Vector2.Distance(vector4, vector) < 0.001f) ? 1 : 0) + ((Vector2.Distance(vector3, vector2) < 0.001f) ? 1 : 0) + ((Vector2.Distance(vector3, vector) < 0.001f) ? 1 : 0) == 1)
					{
						result = Commonality.Point;
					}
					Bounds bounds2 = new Bounds(vector3, Vector3.zero);
					bounds2.Encapsulate(vector4);
					if (bounds.Intersects(bounds2))
					{
						float f = (vector2.x - vector.x) * (vector3.y - vector.y) - (vector3.x - vector.x) * (vector2.y - vector.y);
						float f2 = (vector4.x - vector3.x) * (vector.y - vector3.y) - (vector.x - vector3.x) * (vector4.y - vector3.y);
						if (Mathf.Abs(f) < 0.001f && Mathf.Abs(f2) < 0.001f)
						{
							bool num4 = vector.x < vector2.x || (vector.x == vector2.x && vector.y < vector2.y);
							Vector2 vector5 = (num4 ? vector : vector2);
							Vector2 vector6 = (num4 ? vector2 : vector);
							bool num5 = vector3.x < vector4.x || (vector3.x == vector4.x && vector3.y < vector4.y);
							Vector2 vector7 = (num5 ? vector3 : vector4);
							Vector2 vector8 = (num5 ? vector4 : vector3);
							if (!(vector5.x < vector7.x) && (vector5.x != vector7.x || !(vector5.y < vector7.y)))
							{
								Vector2 vector9 = vector5;
								Vector2 vector10 = vector6;
								vector5 = vector7;
								vector6 = vector8;
								vector7 = vector9;
								vector8 = vector10;
							}
							if (Vector2.Distance(vector6, vector7) < 0.001f)
							{
								result = Commonality.Point;
							}
							else if (vector6.x - vector7.x > 0f || (vector6.x - vector7.x == 0f && vector6.y - vector7.y > 0f))
							{
								edgeIdx = num;
								Vector2 value;
								Vector2 value2;
								if (vector6.x - vector8.x > 0f || (vector6.x - vector8.x == 0f && vector6.y - vector7.y > 0f))
								{
									value = vector7;
									value2 = vector6;
								}
								else
								{
									value = vector7;
									value2 = vector8;
								}
								overlapSegment = new LineSegment(value, value2);
								return Commonality.Edge;
							}
						}
					}
					index = num3++;
				}
				num = num2++;
			}
			overlapSegment = null;
			return result;
		}

		public float DistanceToClosestEdge(Vector2? point = null)
		{
			if (!point.HasValue)
			{
				point = Centroid();
			}
			float timeOnEdge = 0f;
			MathUtil.Pair<Vector2, Vector2> closestEdge = GetClosestEdge(point.Value, ref timeOnEdge);
			Vector2 vector = closestEdge.Second - closestEdge.First;
			return Vector2.Distance(closestEdge.First + vector * timeOnEdge, point.Value);
		}

		public MathUtil.Pair<Vector2, Vector2> GetClosestEdge(Vector2 point, ref float timeOnEdge)
		{
			MathUtil.Pair<Vector2, Vector2> result = null;
			float closest_point = 0f;
			timeOnEdge = 0f;
			float num = float.MaxValue;
			int index = vertices.Count - 1;
			int num2 = 0;
			while (num2 < vertices.Count)
			{
				MathUtil.Pair<Vector2, Vector2> pair = new MathUtil.Pair<Vector2, Vector2>(vertices[index], vertices[num2]);
				float num3 = Mathf.Abs(MathUtil.GetClosestPointBetweenPointAndLineSegment(pair, point, ref closest_point));
				if (num3 < num)
				{
					num = num3;
					result = pair;
					timeOnEdge = closest_point;
				}
				index = num2++;
			}
			return result;
		}

		public List<KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>> GetEdgesWithinDistance(Vector2 point, float distance = float.MaxValue)
		{
			List<KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>> list = new List<KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>>();
			float closest_point = 0f;
			int index = vertices.Count - 1;
			int num = 0;
			while (num < vertices.Count)
			{
				MathUtil.Pair<Vector2, Vector2> pair = new MathUtil.Pair<Vector2, Vector2>(vertices[index], vertices[num]);
				MathUtil.Pair<float, float> pair2 = new MathUtil.Pair<float, float>();
				float num2 = Mathf.Abs(MathUtil.GetClosestPointBetweenPointAndLineSegment(pair, point, ref closest_point));
				if (num2 < distance)
				{
					pair2.First = num2;
					pair2.Second = closest_point;
					list.Add(new KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>(pair2, pair));
				}
				index = num++;
			}
			list.Sort((KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>> a, KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>> b) => a.Key.First.CompareTo(b.Key.First));
			return list;
		}

		public bool IsConvex()
		{
			if (vertices.Count < 4)
			{
				return true;
			}
			bool flag = false;
			int count = vertices.Count;
			for (int i = 0; i < count; i++)
			{
				double num = vertices[(i + 2) % count].x - vertices[(i + 1) % count].x;
				double num2 = vertices[(i + 2) % count].y - vertices[(i + 1) % count].y;
				double num3 = vertices[i].x - vertices[(i + 1) % count].x;
				double num4 = vertices[i].y - vertices[(i + 1) % count].y;
				double num5 = num * num4 - num2 * num3;
				if (i == 0)
				{
					flag = num5 > 0.0;
				}
				else if (flag != num5 > 0.0)
				{
					return false;
				}
			}
			return true;
		}

		private List<IntPoint> GetPath()
		{
			List<IntPoint> list = new List<IntPoint>();
			for (int i = 0; i < vertices.Count; i++)
			{
				list.Add(new IntPoint(vertices[i].x * 10000f, vertices[i].y * 10000f));
			}
			return list;
		}

		public Polygon Clip(Polygon clippingPoly, ClipType type = ClipType.ctIntersection)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Add(GetPath());
			List<List<IntPoint>> list2 = new List<List<IntPoint>>();
			list2.Add(clippingPoly.GetPath());
			Clipper clipper = new Clipper();
			PolyTree polytree = new PolyTree();
			clipper.AddPaths(list, PolyType.ptSubject, closed: true);
			clipper.AddPaths(list2, PolyType.ptClip, closed: true);
			clipper.Execute(type, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
			List<List<IntPoint>> list3 = Clipper.PolyTreeToPaths(polytree);
			if (list3.Count > 0)
			{
				List<Vector2> list4 = new List<Vector2>();
				for (int i = 0; i < list3[0].Count; i++)
				{
					list4.Add(new Vector2((float)list3[0][i].X * 0.0001f, (float)list3[0][i].Y * 0.0001f));
				}
				return new Polygon(list4);
			}
			return null;
		}

		private int CrossingNumber(Vector2 point)
		{
			int num = 0;
			for (int i = 0; i < vertices.Count; i++)
			{
				int index = i;
				int index2 = ((i < vertices.Count - 1) ? (i + 1) : 0);
				if ((vertices[index].y <= point.y && vertices[index2].y > point.y) || (vertices[index].y > point.y && vertices[index2].y <= point.y))
				{
					float num2 = (point.y - vertices[index].y) / (vertices[index2].y - vertices[index].y);
					if (point.x < vertices[index].x + num2 * (vertices[index2].x - vertices[index].x))
					{
						num++;
					}
				}
			}
			return num & 1;
		}

		private float perp(Vector2 u, Vector2 v)
		{
			return u.x * v.y - u.y * v.x;
		}

		public bool ClipSegment(LineSegment segment, ref LineSegment intersectingSegment)
		{
			Vector2 normNear = Vector2.zero;
			Vector2 normFar = Vector2.zero;
			return ClipSegment(segment, ref intersectingSegment, ref normNear, ref normFar);
		}

		public bool ClipSegment(LineSegment segment, ref LineSegment intersectingSegment, ref Vector2 normNear, ref Vector2 normFar)
		{
			normNear = Vector2.zero;
			normFar = Vector2.zero;
			if (segment.p0 == segment.p1)
			{
				intersectingSegment = segment;
				return CrossingNumber(segment.p0.Value) == 1;
			}
			float num = 0f;
			float num2 = 1f;
			Vector2 vector = segment.Direction();
			for (int i = 0; i < vertices.Count; i++)
			{
				int index = i;
				int index2 = ((i < vertices.Count - 1) ? (i + 1) : 0);
				Vector2 u = vertices[index2] - vertices[index];
				Vector2 vector2 = new Vector2(u.y, 0f - u.x);
				float num3 = perp(u, segment.p0.Value - vertices[index]);
				float num4 = 0f - perp(u, vector);
				if (Mathf.Abs(num4) < Mathf.Epsilon)
				{
					if (num3 < 0f)
					{
						return false;
					}
					continue;
				}
				float num5 = num3 / num4;
				if (num4 < 0f)
				{
					if (num5 > num)
					{
						num = num5;
						normNear = vector2;
						if (num > num2)
						{
							return false;
						}
					}
				}
				else if (num5 < num2)
				{
					num2 = num5;
					normFar = vector2;
					if (num2 < num)
					{
						return false;
					}
				}
			}
			intersectingSegment.p0 = segment.p0 + num * vector;
			intersectingSegment.p1 = segment.p0 + num2 * vector;
			normFar.Normalize();
			normNear.Normalize();
			return true;
		}

		public bool ClipSegmentSAT(LineSegment segment, ref LineSegment intersectingSegment, ref Vector2 normNear, ref Vector2 normFar)
		{
			normNear = Vector2.zero;
			normFar = Vector2.zero;
			float num = 0f;
			float num2 = 1f;
			Vector2 vector = segment.Direction();
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 vector2 = vertices[i];
				Vector2 vector3 = vertices[(i < vertices.Count - 1) ? (i + 1) : 0] - vector2;
				Vector2 vector4 = new Vector2(vector3.y, 0f - vector3.x);
				Vector2 u = vector2 - segment.p0.Value;
				float num3 = perp(u, vector4);
				float num4 = perp(vector, vector4);
				if (Mathf.Abs(num4) < Mathf.Epsilon)
				{
					if (num3 < 0f)
					{
						return false;
					}
					continue;
				}
				float num5 = num3 / num4;
				if (num4 < 0f)
				{
					if (num5 > num2)
					{
						return false;
					}
					if (num5 > num)
					{
						num = num5;
						normNear = vector4;
					}
				}
				else
				{
					if (num5 < num)
					{
						return false;
					}
					if (num5 < num2)
					{
						num2 = num5;
						normFar = vector4;
					}
				}
			}
			intersectingSegment.p0 = segment.p0 + num * vector;
			intersectingSegment.p1 = segment.p0 + num2 * vector;
			normFar.Normalize();
			normNear.Normalize();
			return true;
		}

		public void DebugDraw(Color colour, Vector2 offset, bool drawCentroid = false, float duration = 1f, float inset = 0f)
		{
			Vector2 vector = Centroid();
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 vector2 = vertices[i];
				Vector2 vector3 = vertices[(i < vertices.Count - 1) ? (i + 1) : 0];
				if (inset != 0f)
				{
					_ = (vector2 - vector).normalized * (0f - inset);
					_ = (vector3 - vector).normalized * (0f - inset);
				}
			}
		}
	}
}
