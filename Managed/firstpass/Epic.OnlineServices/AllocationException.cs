using System;

namespace Epic.OnlineServices
{
	internal class AllocationException : Exception
	{
		public AllocationException(string message)
			: base(message)
		{
		}
	}
}
