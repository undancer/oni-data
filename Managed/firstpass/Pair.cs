using System;

[Serializable]
public struct Pair<T, U> : IEquatable<Pair<T, U>>
{
	public T first;

	public U second;

	public Pair(T a, U b)
	{
		first = a;
		second = b;
	}

	public bool Equals(Pair<T, U> other)
	{
		if (first.Equals(other.first))
		{
			return second.Equals(other.second);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return first.GetHashCode() ^ second.GetHashCode();
	}
}
