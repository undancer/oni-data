using System.Collections.Generic;
using Delaunay.LR;
using Delaunay.Utils;

namespace Delaunay
{
	internal sealed class EdgeReorderer : IDisposable
	{
		private List<Edge> _edges;

		private List<Side> _edgeOrientations;

		public List<Edge> edges => _edges;

		public List<Side> edgeOrientations => _edgeOrientations;

		public EdgeReorderer(List<Edge> origEdges, VertexOrSite criterion)
		{
			_edges = new List<Edge>();
			_edgeOrientations = new List<Side>();
			if (origEdges.Count > 0)
			{
				_edges = ReorderEdges(origEdges, criterion);
			}
		}

		public void Dispose()
		{
			_edges = null;
			_edgeOrientations = null;
		}

		private List<Edge> ReorderEdges(List<Edge> origEdges, VertexOrSite criterion)
		{
			int count = origEdges.Count;
			bool[] array = new bool[count];
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				array[i] = false;
			}
			List<Edge> list = new List<Edge>();
			int num2 = 0;
			Edge edge = origEdges[num2];
			list.Add(edge);
			_edgeOrientations.Add(Side.LEFT);
			ICoord coord;
			if (criterion != 0)
			{
				ICoord leftSite = edge.leftSite;
				coord = leftSite;
			}
			else
			{
				ICoord leftSite = edge.leftVertex;
				coord = leftSite;
			}
			ICoord coord2 = coord;
			ICoord coord3;
			if (criterion != 0)
			{
				ICoord leftSite = edge.rightSite;
				coord3 = leftSite;
			}
			else
			{
				ICoord leftSite = edge.rightVertex;
				coord3 = leftSite;
			}
			ICoord coord4 = coord3;
			if (coord2 == Vertex.VERTEX_AT_INFINITY || coord4 == Vertex.VERTEX_AT_INFINITY)
			{
				return new List<Edge>();
			}
			array[num2] = true;
			num++;
			while (num < count)
			{
				for (num2 = 1; num2 < count; num2++)
				{
					if (!array[num2])
					{
						edge = origEdges[num2];
						ICoord coord5;
						if (criterion != 0)
						{
							ICoord leftSite = edge.leftSite;
							coord5 = leftSite;
						}
						else
						{
							ICoord leftSite = edge.leftVertex;
							coord5 = leftSite;
						}
						ICoord coord6 = coord5;
						ICoord coord7;
						if (criterion != 0)
						{
							ICoord leftSite = edge.rightSite;
							coord7 = leftSite;
						}
						else
						{
							ICoord leftSite = edge.rightVertex;
							coord7 = leftSite;
						}
						ICoord coord8 = coord7;
						if (coord6 == Vertex.VERTEX_AT_INFINITY || coord8 == Vertex.VERTEX_AT_INFINITY)
						{
							return new List<Edge>();
						}
						if (coord6 == coord4)
						{
							coord4 = coord8;
							_edgeOrientations.Add(Side.LEFT);
							list.Add(edge);
							array[num2] = true;
						}
						else if (coord8 == coord2)
						{
							coord2 = coord6;
							_edgeOrientations.Insert(0, Side.LEFT);
							list.Insert(0, edge);
							array[num2] = true;
						}
						else if (coord6 == coord2)
						{
							coord2 = coord8;
							_edgeOrientations.Insert(0, Side.RIGHT);
							list.Insert(0, edge);
							array[num2] = true;
						}
						else if (coord8 == coord4)
						{
							coord4 = coord6;
							_edgeOrientations.Add(Side.RIGHT);
							list.Add(edge);
							array[num2] = true;
						}
						if (array[num2])
						{
							num++;
						}
					}
				}
			}
			return list;
		}
	}
}
