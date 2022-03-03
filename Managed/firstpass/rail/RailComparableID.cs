using System;

namespace rail
{
	public class RailComparableID : IEquatable<RailComparableID>, IComparable<RailComparableID>
	{
		public static readonly RailComparableID Nil = new RailComparableID(0uL);

		public ulong id_;

		public RailComparableID()
		{
		}

		public RailComparableID(ulong id)
		{
			id_ = id;
		}

		public bool IsValid()
		{
			return id_ != 0;
		}

		public override bool Equals(object other)
		{
			if (other is RailComparableID)
			{
				return this == (RailComparableID)other;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return id_.GetHashCode();
		}

		public static bool operator ==(RailComparableID x, RailComparableID y)
		{
			return x?.Equals(y) ?? ((object)y == null);
		}

		public static bool operator !=(RailComparableID x, RailComparableID y)
		{
			return !(x == y);
		}

		public static explicit operator RailComparableID(ulong value)
		{
			return new RailComparableID(value);
		}

		public static explicit operator ulong(RailComparableID that)
		{
			return that.id_;
		}

		public bool Equals(RailComparableID other)
		{
			if ((object)other == null)
			{
				return false;
			}
			if ((object)this == other)
			{
				return true;
			}
			if (GetType() != other.GetType())
			{
				return false;
			}
			return other.id_ == id_;
		}

		public int CompareTo(RailComparableID other)
		{
			return id_.CompareTo(other.id_);
		}

		public override string ToString()
		{
			return id_.ToString();
		}
	}
}
