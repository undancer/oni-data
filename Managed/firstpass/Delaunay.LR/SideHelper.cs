namespace Delaunay.LR
{
	public class SideHelper
	{
		public static Side Other(Side leftRight)
		{
			if (leftRight != 0)
			{
				return Side.LEFT;
			}
			return Side.RIGHT;
		}
	}
}
