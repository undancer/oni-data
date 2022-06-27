using System;

namespace KMod
{
	[Flags]
	public enum Content : byte
	{
		LayerableFiles = 1,
		Strings = 2,
		DLL = 4,
		Translation = 8,
		Animation = 0x10
	}
}
