using System;

namespace rail
{
	public class RailObject
	{
		protected IntPtr swigCPtr_ = IntPtr.Zero;

		internal RailObject()
		{
		}

		internal static IntPtr getCPtr(RailObject obj)
		{
			return obj?.swigCPtr_ ?? IntPtr.Zero;
		}

		~RailObject()
		{
		}
	}
}
