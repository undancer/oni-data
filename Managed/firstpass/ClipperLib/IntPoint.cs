namespace ClipperLib
{
	public struct IntPoint
	{
		public long X;

		public long Y;

		public IntPoint(long X, long Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public IntPoint(double x, double y)
		{
			X = (long)x;
			Y = (long)y;
		}

		public IntPoint(IntPoint pt)
		{
			X = pt.X;
			Y = pt.Y;
		}

		public static bool operator ==(IntPoint a, IntPoint b)
		{
			if (a.X == b.X)
			{
				return a.Y == b.Y;
			}
			return false;
		}

		public static bool operator !=(IntPoint a, IntPoint b)
		{
			if (a.X == b.X)
			{
				return a.Y != b.Y;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is IntPoint)
			{
				IntPoint intPoint = (IntPoint)obj;
				if (X == intPoint.X)
				{
					return Y == intPoint.Y;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
