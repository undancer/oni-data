using System;
using UnityEngine;

[Serializable]
public struct AABB3
{
	public Vector3 min;

	public Vector3 max;

	public Vector3 Center => (min + max) * 0.5f;

	public Vector3 Range => max - min;

	public float Width => max.x - min.x;

	public float Height => max.y - min.y;

	public float Depth => max.z - min.z;

	public AABB3(Vector3 pt)
	{
		min = pt;
		max = pt;
	}

	public AABB3(Vector3 min, Vector3 max)
	{
		this.min = min;
		this.max = max;
	}

	public bool IsValid()
	{
		return min.Min(max) == min;
	}

	public void Expand(float amount)
	{
		Vector3 vector = new Vector3(amount * 0.5f, amount * 0.5f, amount * 0.5f);
		min -= vector;
		max += vector;
	}

	public void ExpandToFit(Vector3 pt)
	{
		min = min.Min(pt);
		max = max.Max(pt);
	}

	public void ExpandToFit(AABB3 aabb)
	{
		min = min.Min(aabb.min);
		max = max.Max(aabb.max);
	}

	public bool Contains(Vector3 pt)
	{
		if (min.LessEqual(pt))
		{
			return pt.Less(max);
		}
		return false;
	}

	public bool Contains(AABB3 aabb)
	{
		if (Contains(aabb.min))
		{
			return Contains(aabb.max);
		}
		return false;
	}

	public bool Intersects(AABB3 aabb)
	{
		if (min.LessEqual(aabb.max))
		{
			return aabb.min.Less(max);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		AABB3 aABB = (AABB3)obj;
		if (min == aABB.min)
		{
			return max == aABB.max;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return min.GetHashCode() ^ max.GetHashCode();
	}

	public unsafe void Transform(Matrix4x4 t)
	{
		Vector3* ptr = stackalloc Vector3[8];
		*ptr = min;
		ptr[1] = new Vector3(min.x, min.y, max.z);
		ptr[2] = new Vector3(min.x, max.y, min.z);
		ptr[3] = new Vector3(max.x, min.y, min.z);
		ptr[4] = new Vector3(min.x, max.y, max.z);
		ptr[5] = new Vector3(max.x, min.y, max.z);
		ptr[6] = new Vector3(max.x, max.y, min.z);
		ptr[7] = max;
		min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		for (int i = 0; i < 8; i++)
		{
			ExpandToFit(t * ptr[i]);
		}
	}
}
