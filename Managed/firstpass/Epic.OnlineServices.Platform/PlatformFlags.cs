using System;

namespace Epic.OnlineServices.Platform
{
	[Flags]
	public enum PlatformFlags : ulong
	{
		None = 0uL,
		LoadingInEditor = 1uL,
		DisableOverlay = 2uL,
		DisableSocialOverlay = 4uL
	}
}
