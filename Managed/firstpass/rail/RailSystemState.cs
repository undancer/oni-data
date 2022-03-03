namespace rail
{
	public enum RailSystemState
	{
		kSystemStateUnknown = 0,
		kSystemStatePlatformOnline = 1,
		kSystemStatePlatformOffline = 2,
		kSystemStatePlatformExit = 3,
		kSystemStatePlayerOwnershipExpired = 20,
		kSystemStatePlayerOwnershipActivated = 21,
		kSystemStatePlayerOwnershipBanned = 22,
		kSystemStateGameExitByAntiAddiction = 40
	}
}
