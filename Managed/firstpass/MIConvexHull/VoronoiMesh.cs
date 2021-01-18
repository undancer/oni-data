using System;
using System.Collections.Generic;
using System.Linq;

namespace MIConvexHull
{
	public static class VoronoiMesh
	{
		public static VoronoiMesh<TVertex, TCell, TEdge> Create<TVertex, TCell, TEdge>(IList<TVertex> data) where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()where TEdge : VoronoiEdge<TVertex, TCell>, new()
		{
			return VoronoiMesh<TVertex, TCell, TEdge>.Create(data);
		}

		public static VoronoiMesh<TVertex, DefaultTriangulationCell<TVertex>, VoronoiEdge<TVertex, DefaultTriangulationCell<TVertex>>> Create<TVertex>(IList<TVertex> data) where TVertex : IVertex
		{
			return VoronoiMesh<TVertex, DefaultTriangulationCell<TVertex>, VoronoiEdge<TVertex, DefaultTriangulationCell<TVertex>>>.Create(data);
		}

		public static VoronoiMesh<DefaultVertex, DefaultTriangulationCell<DefaultVertex>, VoronoiEdge<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>> Create(IList<double[]> data)
		{
			List<DefaultVertex> data2 = data.Select((double[] p) => new DefaultVertex
			{
				Position = p.ToArray()
			}).ToList();
			return VoronoiMesh<DefaultVertex, DefaultTriangulationCell<DefaultVertex>, VoronoiEdge<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>>.Create(data2);
		}

		public static VoronoiMesh<TVertex, TCell, VoronoiEdge<TVertex, TCell>> Create<TVertex, TCell>(IList<TVertex> data) where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()
		{
			return VoronoiMesh<TVertex, TCell, VoronoiEdge<TVertex, TCell>>.Create(data);
		}
	}
	public class VoronoiMesh<TVertex, TCell, TEdge> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()where TEdge : VoronoiEdge<TVertex, TCell>, new()
	{
		private class EdgeComparer : IEqualityComparer<TEdge>
		{
			public bool Equals(TEdge x, TEdge y)
			{
				return (x.Source == y.Source && x.Target == y.Target) || (x.Source == y.Target && x.Target == y.Source);
			}

			public int GetHashCode(TEdge obj)
			{
				return obj.Source.GetHashCode() ^ obj.Target.GetHashCode();
			}
		}

		public IEnumerable<TCell> Vertices
		{
			get;
			private set;
		}

		public IEnumerable<TEdge> Edges
		{
			get;
			private set;
		}

		private VoronoiMesh()
		{
		}

		public static VoronoiMesh<TVertex, TCell, TEdge> Create(IList<TVertex> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			DelaunayTriangulation<TVertex, TCell> delaunayTriangulation = DelaunayTriangulation<TVertex, TCell>.Create(data);
			List<TCell> list = delaunayTriangulation.Cells.ToList();
			HashSet<TEdge> hashSet = new HashSet<TEdge>(new EdgeComparer());
			foreach (TCell item in list)
			{
				for (int i = 0; i < item.Adjacency.Length; i++)
				{
					TCell val = item.Adjacency[i];
					if (val != null)
					{
						TEdge val2 = new TEdge();
						val2.Source = item;
						val2.Target = val;
						hashSet.Add(val2);
					}
				}
			}
			return new VoronoiMesh<TVertex, TCell, TEdge>
			{
				Vertices = list,
				Edges = hashSet.ToList()
			};
		}
	}
}
