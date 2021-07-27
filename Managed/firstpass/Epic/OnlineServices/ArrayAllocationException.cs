using System;

namespace Epic.OnlineServices
{
	internal class ArrayAllocationException : AllocationException
	{
		public ArrayAllocationException(IntPtr address, int foundLength, int expectedLength)
			: base(string.Format("Found array allocation with length {0} but expected {1} at {2:X}", foundLength, expectedLength, address.ToString("X")))
		{
		}
	}
}
