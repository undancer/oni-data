namespace MIConvexHull
{
	public class VoronoiEdge<TVertex, TCell> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>
	{
		public TCell Source { get; internal set; }

		public TCell Target { get; internal set; }

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
			if (!(obj is VoronoiEdge<TVertex, TCell> voronoiEdge))
			{
				return false;
			}
			if (this == voronoiEdge)
			{
				return true;
			}
			if (Source != voronoiEdge.Source || Target != voronoiEdge.Target)
			{
				if (Source == voronoiEdge.Target)
				{
					return Target == voronoiEdge.Source;
				}
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return (23 * 31 + Source.GetHashCode()) * 31 + Target.GetHashCode();
		}
	}
}
