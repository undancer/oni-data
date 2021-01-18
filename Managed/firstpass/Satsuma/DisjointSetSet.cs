using System;

namespace Satsuma
{
	public struct DisjointSetSet<T> : IEquatable<DisjointSetSet<T>>
	{
		public T Representative
		{
			get;
			private set;
		}

		public DisjointSetSet(T representative)
		{
			this = default(DisjointSetSet<T>);
			Representative = representative;
		}

		public bool Equals(DisjointSetSet<T> other)
		{
			return Representative.Equals(other.Representative);
		}

		public override bool Equals(object obj)
		{
			if (obj is DisjointSetSet<T>)
			{
				return Equals((DisjointSetSet<T>)obj);
			}
			return false;
		}

		public static bool operator ==(DisjointSetSet<T> a, DisjointSetSet<T> b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(DisjointSetSet<T> a, DisjointSetSet<T> b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return Representative.GetHashCode();
		}

		public override string ToString()
		{
			return string.Concat("[DisjointSetSet:", Representative, "]");
		}
	}
}
