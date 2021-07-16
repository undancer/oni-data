using System;
using UnityEngine;

[Serializable]
public struct CellOffset : IEquatable<CellOffset>
{
	public int x;

	public int y;

	public static CellOffset none => new CellOffset(0, 0);

	public CellOffset(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public CellOffset(Vector2 offset)
	{
		x = Mathf.RoundToInt(offset.x);
		y = Mathf.RoundToInt(offset.y);
	}

	public Vector2I ToVector2I()
	{
		return new Vector2I(x, y);
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x, y, 0f);
	}

	public CellOffset Offset(CellOffset offset)
	{
		return new CellOffset(x + offset.x, y + offset.y);
	}

	public int GetOffsetDistance()
	{
		return Math.Abs(x) + Math.Abs(y);
	}

	public static CellOffset operator +(CellOffset a, CellOffset b)
	{
		return new CellOffset(a.x + b.x, a.y + b.y);
	}

	public static CellOffset operator -(CellOffset a, CellOffset b)
	{
		return new CellOffset(a.x - b.x, a.y - b.y);
	}

	public static CellOffset operator *(CellOffset offset, int value)
	{
		return new CellOffset(offset.x * value, offset.y * value);
	}

	public static CellOffset operator *(int value, CellOffset offset)
	{
		return new CellOffset(offset.x * value, offset.y * value);
	}

	public override bool Equals(object obj)
	{
		CellOffset cellOffset = (CellOffset)obj;
		if (x == cellOffset.x)
		{
			return y == cellOffset.y;
		}
		return false;
	}

	public bool Equals(CellOffset offset)
	{
		if (x == offset.x)
		{
			return y == offset.y;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return x + y * 8192;
	}

	public static bool operator ==(CellOffset a, CellOffset b)
	{
		if (a.x == b.x)
		{
			return a.y == b.y;
		}
		return false;
	}

	public static bool operator !=(CellOffset a, CellOffset b)
	{
		if (a.x == b.x)
		{
			return a.y != b.y;
		}
		return true;
	}

	public override string ToString()
	{
		return "(" + x + "," + y + ")";
	}
}
