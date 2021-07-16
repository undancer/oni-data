using System.Collections.Generic;
using Delaunay.LR;
using Delaunay.Utils;
using UnityEngine;

namespace Delaunay
{
	public sealed class Halfedge : IDisposable
	{
		private static Stack<Halfedge> _pool = new Stack<Halfedge>();

		public Halfedge edgeListLeftNeighbor;

		public Halfedge edgeListRightNeighbor;

		public Halfedge nextInPriorityQueue;

		public Edge edge;

		public Side? leftRight;

		public Vertex vertex;

		public float ystar;

		public static Halfedge Create(Edge edge, Side? lr)
		{
			if (_pool.Count > 0)
			{
				return _pool.Pop().Init(edge, lr);
			}
			return new Halfedge(edge, lr);
		}

		public static Halfedge CreateDummy()
		{
			return Create(null, null);
		}

		public Halfedge(Edge edge = null, Side? lr = null)
		{
			Init(edge, lr);
		}

		private Halfedge Init(Edge edge, Side? lr)
		{
			this.edge = edge;
			leftRight = lr;
			nextInPriorityQueue = null;
			vertex = null;
			return this;
		}

		public override string ToString()
		{
			return "Halfedge (leftRight: " + leftRight.ToString() + "; vertex: " + vertex.ToString() + ")";
		}

		public void Dispose()
		{
			if (edgeListLeftNeighbor == null && edgeListRightNeighbor == null && nextInPriorityQueue == null)
			{
				edge = null;
				leftRight = null;
				vertex = null;
				_pool.Push(this);
			}
		}

		public void ReallyDispose()
		{
			edgeListLeftNeighbor = null;
			edgeListRightNeighbor = null;
			nextInPriorityQueue = null;
			edge = null;
			leftRight = null;
			vertex = null;
			_pool.Push(this);
		}

		internal bool IsLeftOf(Vector2 p)
		{
			Vector2 coord = edge.rightSite.Coord;
			bool flag = p.x > coord.x;
			if (flag && leftRight == Side.LEFT)
			{
				return true;
			}
			if (!flag && leftRight == Side.RIGHT)
			{
				return false;
			}
			bool flag3;
			if ((double)edge.a == 1.0)
			{
				float num = p.y - coord.y;
				float num2 = p.x - coord.x;
				bool flag2 = false;
				if ((!flag && (double)edge.b < 0.0) || (flag && (double)edge.b >= 0.0))
				{
					flag3 = num >= edge.b * num2;
					flag2 = flag3;
				}
				else
				{
					flag3 = p.x + p.y * edge.b > edge.c;
					if ((double)edge.b < 0.0)
					{
						flag3 = !flag3;
					}
					if (!flag3)
					{
						flag2 = true;
					}
				}
				if (!flag2)
				{
					float num3 = coord.x - edge.leftSite.x;
					flag3 = (double)(edge.b * (num2 * num2 - num * num)) < (double)(num3 * num) * (1.0 + 2.0 * (double)num2 / (double)num3 + (double)(edge.b * edge.b));
					if ((double)edge.b < 0.0)
					{
						flag3 = !flag3;
					}
				}
			}
			else
			{
				float num4 = edge.c - edge.a * p.x;
				float num5 = p.y - num4;
				float num6 = p.x - coord.x;
				float num7 = num4 - coord.y;
				flag3 = num5 * num5 > num6 * num6 + num7 * num7;
			}
			if (leftRight != Side.LEFT)
			{
				return !flag3;
			}
			return flag3;
		}
	}
}
