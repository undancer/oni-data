using System;

namespace Epic.OnlineServices
{
	internal interface ICallbackInfo
	{
		IntPtr ClientDataAddress { get; }
	}
}
