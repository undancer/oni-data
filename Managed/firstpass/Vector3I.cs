using System.Diagnostics;

[DebuggerDisplay("{x}, {y}, {z}")]
public struct Vector3I
{
	public int x;

	public int y;

	public int z;

	public Vector3I(int a, int b, int c)
	{
		x = a;
		y = b;
		z = c;
	}

	public static bool operator ==(Vector3I v1, Vector3I v2)
	{
		if (v1.x == v2.x && v1.y == v2.y)
		{
			return v1.z == v2.z;
		}
		return false;
	}

	public static bool operator !=(Vector3I v1, Vector3I v2)
	{
		return !(v1 == v2);
	}

	public override bool Equals(object o)
	{
		return base.Equals(o);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return $"{x}, {y}, {z}";
	}
}
