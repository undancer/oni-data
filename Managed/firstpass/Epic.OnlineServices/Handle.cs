using System;

namespace Epic.OnlineServices
{
	public class Handle : IEquatable<Handle>
	{
		public IntPtr InnerHandle
		{
			get;
			private set;
		}

		public Handle(IntPtr innerHandle)
		{
			InnerHandle = innerHandle;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Handle);
		}

		public override int GetHashCode()
		{
			return (int)(65536 + InnerHandle.ToInt64());
		}

		public bool Equals(Handle other)
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
			return InnerHandle == other.InnerHandle;
		}

		public static bool operator ==(Handle lhs, Handle rhs)
		{
			if ((object)lhs == null)
			{
				if ((object)rhs == null)
				{
					return true;
				}
				return false;
			}
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Handle lhs, Handle rhs)
		{
			return !(lhs == rhs);
		}
	}
}
