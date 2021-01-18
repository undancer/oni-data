using System;
using System.Collections.Generic;

namespace MIConvexHull
{
	public class DelaunayTriangulation<TVertex, TCell> : ITriangulation<TVertex, TCell> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()
	{
		public IEnumerable<TCell> Cells
		{
			get;
			private set;
		}

		private DelaunayTriangulation()
		{
		}

		public static DelaunayTriangulation<TVertex, TCell> Create(IList<TVertex> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Count == 0)
			{
				DelaunayTriangulation<TVertex, TCell> delaunayTriangulation = new DelaunayTriangulation<TVertex, TCell>();
				delaunayTriangulation.Cells = new TCell[0];
				return delaunayTriangulation;
			}
			TCell[] delaunayTriangulation2 = ConvexHullAlgorithm.GetDelaunayTriangulation<TVertex, TCell>(data);
			return new DelaunayTriangulation<TVertex, TCell>
			{
				Cells = delaunayTriangulation2
			};
		}
	}
}
