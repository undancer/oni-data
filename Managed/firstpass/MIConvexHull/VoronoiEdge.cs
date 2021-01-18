namespace MIConvexHull
{
	public class VoronoiEdge<TVertex, TCell> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>
	{
		public TCell Source
		{
			get;
			internal set;
		}

		public TCell Target
		{
			get;
			internal set;
		}

		public VoronoiEdge()
		{
		}

		public VoronoiEdge(TCell source, TCell target)
		{
			Source = source;
			Target = target;
		}

		public override bool Equals(object obj)
		{
			VoronoiEdge<TVertex, TCell> voronoiEdge = obj as VoronoiEdge<TVertex, TCell>;
			if (voronoiEdge == null)
			{
				return false;
			}
			if (this == voronoiEdge)
			{
				return true;
			}
			return (Source == voronoiEdge.Source && Target == voronoiEdge.Target) || (Source == voronoiEdge.Target && Target == voronoiEdge.Source);
		}

		public override int GetHashCode()
		{
			int num = 23;
			num = num * 31 + Source.GetHashCode();
			return num * 31 + Target.GetHashCode();
		}
	}
}
