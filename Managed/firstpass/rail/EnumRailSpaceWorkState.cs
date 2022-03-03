namespace rail
{
	public enum EnumRailSpaceWorkState
	{
		kRailSpaceWorkStateNone = 0,
		kRailSpaceWorkStateDownloaded = 1,
		kRailSpaceWorkStateNeedsSync = 2,
		kRailSpaceWorkStateDownloading = 4,
		kRailSpaceWorkStateUploading = 8
	}
}
