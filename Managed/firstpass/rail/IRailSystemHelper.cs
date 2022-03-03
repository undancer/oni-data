namespace rail
{
	public interface IRailSystemHelper
	{
		RailResult SetTerminationTimeoutOwnershipExpired(int timeout_seconds);

		RailSystemState GetPlatformSystemState();
	}
}
