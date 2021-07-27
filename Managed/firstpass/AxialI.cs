using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

[Serializable]
[DebuggerDisplay("{r}, {q}")]
[SerializationConfig(MemberSerialization.OptIn)]
public struct AxialI : IEquatable<AxialI>
{
	public static readonly AxialI ZERO = new AxialI(0, 0);

	public static readonly AxialI NORTHWEST = new AxialI(0, -1);

	public static readonly AxialI NORTHEAST = new AxialI(1, -1);

	public static readonly AxialI EAST = new AxialI(1, 0);

	public static readonly AxialI SOUTHEAST = new AxialI(0, 1);

	public static readonly AxialI SOUTHWEST = new AxialI(-1, 1);

	public static readonly AxialI WEST = new AxialI(-1, 0);

	public static readonly List<AxialI> DIRECTIONS = new List<AxialI> { NORTHWEST, NORTHEAST, EAST, SOUTHEAST, SOUTHWEST, WEST };

	public static readonly List<AxialI> CLOCKWISE = new List<AxialI> { EAST, SOUTHEAST, SOUTHWEST, WEST, NORTHWEST, NORTHEAST };

	[Serialize]
	public int r;

	[Serialize]
	public int q;

	public int R
	{
		get
		{
			return r;
		}
		set
		{
			r = value;
		}
	}

	public int Q
	{
		get
		{
			return q;
		}
		set
		{
			q = value;
		}
	}

	public AxialI(int a, int b)
	{
		r = a;
		q = b;
	}

	public Vector3I ToCube()
	{
		int num = q;
		int num2 = r;
		int b = -num - num2;
		return new Vector3I(num, b, num2);
	}

	public Vector3 ToWorld()
	{
		return AxialUtil.AxialToWorld(r, q);
	}

	public Vector2 ToWorld2D()
	{
		Vector3 vector = ToWorld();
		return new Vector2(vector.x, vector.y);
	}

	public static AxialI operator +(AxialI u, AxialI v)
	{
		return new AxialI(u.r + v.r, u.q + v.q);
	}

	public static AxialI operator -(AxialI u, AxialI v)
	{
		return new AxialI(u.r - v.r, u.q - v.q);
	}

	public static AxialI operator +(AxialI u, int scalar)
	{
		return new AxialI(u.r + scalar, u.q + scalar);
	}

	public static AxialI operator -(AxialI u, int scalar)
	{
		return new AxialI(u.r - scalar, u.q - scalar);
	}

	public static AxialI operator *(AxialI v, int s)
	{
		return new AxialI(v.r * s, v.q * s);
	}

	public static AxialI operator /(AxialI v, int s)
	{
		return new AxialI(v.r / s, v.q / s);
	}

	public static bool operator ==(AxialI u, AxialI v)
	{
		if (u.r == v.r)
		{
			return u.q == v.q;
		}
		return false;
	}

	public static bool operator !=(AxialI u, AxialI v)
	{
		if (u.r == v.r)
		{
			return u.q != v.q;
		}
		return true;
	}

	public static bool operator <(AxialI u, AxialI v)
	{
		if (u.r < v.r)
		{
			return u.q < v.q;
		}
		return false;
	}

	public static bool operator >(AxialI u, AxialI v)
	{
		if (u.r > v.r)
		{
			return u.q > v.q;
		}
		return false;
	}

	public static bool operator <=(AxialI u, AxialI v)
	{
		if (u.r <= v.r)
		{
			return u.q <= v.q;
		}
		return false;
	}

	public static bool operator >=(AxialI u, AxialI v)
	{
		if (u.r >= v.r)
		{
			return u.q >= v.q;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		try
		{
			AxialI axialI = (AxialI)obj;
			return axialI.r == r && axialI.q == q;
		}
		catch
		{
			return false;
		}
	}

	public bool Equals(AxialI v)
	{
		if (v.r == r)
		{
			return v.q == q;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return r ^ q;
	}

	public override string ToString()
	{
		return $"{r}, {q}";
	}
}
