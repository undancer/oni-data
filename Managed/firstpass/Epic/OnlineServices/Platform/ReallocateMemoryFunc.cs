using System;

namespace Epic.OnlineServices.Platform
{
	public delegate IntPtr ReallocateMemoryFunc(byte[] pointer, int sizeInBytes, int alignment);
}
