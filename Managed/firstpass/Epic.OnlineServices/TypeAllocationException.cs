using System;

namespace Epic.OnlineServices
{
	internal class TypeAllocationException : AllocationException
	{
		public TypeAllocationException(IntPtr address, Type foundType, Type expectedType)
			: base(string.Format("Found allocation of {0} but expected {1} at {2}", foundType, expectedType, address.ToString("X")))
		{
		}
	}
}
