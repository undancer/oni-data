using System;

public class Tuple<T, U> : IEquatable<Tuple<T, U>>
{
	public T first;

	public U second;

	public Tuple(T a, U b)
	{
		first = a;
		second = b;
	}

	public bool Equals(Tuple<T, U> other)
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
public class Tuple<T, U, V> : IEquatable<Tuple<T, U, V>>
{
	public T first;

	public U second;

	public V third;

	public Tuple(T a, U b, V c)
	{
		first = a;
		second = b;
		third = c;
	}

	public bool Equals(Tuple<T, U, V> other)
	{
		if (first.Equals(other.first) && second.Equals(other.second))
		{
			return third.Equals(other.third);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return first.GetHashCode() ^ second.GetHashCode() ^ third.GetHashCode();
	}
}
