using System;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

[Serializable]
[DebuggerDisplay("{x}, {y}")]
[SerializationConfig(MemberSerialization.OptIn)]
public struct Vector2I : IComparable<Vector2I>, IEquatable<Vector2I>
{
	public static readonly Vector2I zero = new Vector2I(0, 0);

	public static readonly Vector2I one = new Vector2I(1, 1);

	public static readonly Vector2I minusone = new Vector2I(-1, -1);

	[Serialize]
	public int x;

	[Serialize]
	public int y;

	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public int magnitudeSqr => x * x + y * y;

	public Vector2I(int a, int b)
	{
		x = a;
		y = b;
	}

	public static Vector2I operator +(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x + v.x, u.y + v.y);
	}

	public static Vector2I operator -(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x - v.x, u.y - v.y);
	}

	public static Vector2I operator *(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x * v.x, u.y * v.y);
	}

	public static Vector2I operator /(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x / v.x, u.y / v.y);
	}

	public static Vector2I operator *(Vector2I v, int s)
	{
		return new Vector2I(v.x * s, v.y * s);
	}

	public static Vector2I operator /(Vector2I v, int s)
	{
		return new Vector2I(v.x / s, v.y / s);
	}

	public static Vector2I operator +(Vector2I u, int scalar)
	{
		return new Vector2I(u.x + scalar, u.y + scalar);
	}

	public static Vector2I operator -(Vector2I u, int scalar)
	{
		return new Vector2I(u.x - scalar, u.y - scalar);
	}

	public static bool operator ==(Vector2I u, Vector2I v)
	{
		if (u.x == v.x)
		{
			return u.y == v.y;
		}
		return false;
	}

	public static bool operator !=(Vector2I u, Vector2I v)
	{
		if (u.x == v.x)
		{
			return u.y != v.y;
		}
		return true;
	}

	public static Vector2I Min(Vector2I v, Vector2I w)
	{
		return new Vector2I((v.x < w.x) ? v.x : w.x, (v.y < w.y) ? v.y : w.y);
	}

	public static Vector2I Max(Vector2I v, Vector2I w)
	{
		return new Vector2I((v.x > w.x) ? v.x : w.x, (v.y > w.y) ? v.y : w.y);
	}

	public static bool operator <(Vector2I u, Vector2I v)
	{
		if (u.x < v.x)
		{
			return u.y < v.y;
		}
		return false;
	}

	public static bool operator >(Vector2I u, Vector2I v)
	{
		if (u.x > v.x)
		{
			return u.y > v.y;
		}
		return false;
	}

	public static bool operator <=(Vector2I u, Vector2I v)
	{
		if (u.x <= v.x)
		{
			return u.y <= v.y;
		}
		return false;
	}

	public static bool operator >=(Vector2I u, Vector2I v)
	{
		if (u.x >= v.x)
		{
			return u.y >= v.y;
		}
		return false;
	}

	public static implicit operator Vector2(Vector2I v)
	{
		return new Vector2(v.x, v.y);
	}

	public override bool Equals(object obj)
	{
		try
		{
			Vector2I vector2I = (Vector2I)obj;
			return vector2I.x == x && vector2I.y == y;
		}
		catch
		{
			return false;
		}
	}

	public bool Equals(Vector2I v)
	{
		if (v.x == x)
		{
			return v.y == y;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return x ^ y;
	}

	public static bool operator <=(Vector2I u, Vector2 v)
	{
		if ((float)u.x <= v.x)
		{
			return (float)u.y <= v.y;
		}
		return false;
	}

	public static bool operator >=(Vector2I u, Vector2 v)
	{
		if ((float)u.x >= v.x)
		{
			return (float)u.y >= v.y;
		}
		return false;
	}

	public static bool operator <=(Vector2 u, Vector2I v)
	{
		if (u.x <= (float)v.x)
		{
			return u.y <= (float)v.y;
		}
		return false;
	}

	public static bool operator >=(Vector2 u, Vector2I v)
	{
		if (u.x >= (float)v.x)
		{
			return u.y >= (float)v.y;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{x}, {y}";
	}

	public int CompareTo(Vector2I other)
	{
		int result = y - other.y;
		if (other.y == 0)
		{
			return x - other.x;
		}
		return result;
	}
}
