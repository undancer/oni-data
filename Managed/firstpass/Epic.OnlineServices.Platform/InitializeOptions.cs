using System;

namespace Epic.OnlineServices.Platform
{
	public class InitializeOptions
	{
		public int ApiVersion => 3;

		public AllocateMemoryFunc AllocateMemoryFunction
		{
			get;
			set;
		}

		public ReallocateMemoryFunc ReallocateMemoryFunction
		{
			get;
			set;
		}

		public ReleaseMemoryFunc ReleaseMemoryFunction
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public string ProductVersion
		{
			get;
			set;
		}

		public IntPtr SystemInitializeOptions
		{
			get;
			set;
		}
	}
}
