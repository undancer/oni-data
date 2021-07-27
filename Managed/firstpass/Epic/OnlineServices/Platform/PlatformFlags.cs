using System;

namespace Epic.OnlineServices.Platform
{
	[Flags]
	public enum PlatformFlags : ulong
	{
		None = 0x0uL,
		LoadingInEditor = 0x1uL,
		DisableOverlay = 0x2uL,
		DisableSocialOverlay = 0x4uL
	}
}
