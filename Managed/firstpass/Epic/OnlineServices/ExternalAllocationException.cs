using System;

namespace Epic.OnlineServices
{
	internal class ExternalAllocationException : AllocationException
	{
		public ExternalAllocationException(IntPtr address, Type type)
			: base(string.Format("Attempting to allocate {0} over externally allocated memory at {1:X}", type, address.ToString("X")))
		{
		}
	}
}
