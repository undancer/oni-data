using UnityEngine;

public static class VectorUtil
{
	public static bool Less(this Vector2 v, Vector2 other)
	{
		if (v.x < other.x)
		{
			return v.y < other.y;
		}
		return false;
	}

	public static bool LessEqual(this Vector2 v, Vector2 other)
	{
		if (v.x <= other.x)
		{
			return v.y <= other.y;
		}
		return false;
	}

	public static bool Less(this Vector3 v, Vector3 other)
	{
		if (v.x < other.x && v.y < other.y)
		{
			return v.z < other.z;
		}
		return false;
	}

	public static bool LessEqual(this Vector3 v, Vector3 other)
	{
		if (v.x <= other.x && v.y <= other.y)
		{
			return v.z <= other.z;
		}
		return false;
	}
}
