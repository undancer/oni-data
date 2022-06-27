namespace Epic.OnlineServices.Sessions
{
	public enum OnlineSessionState
	{
		NoSession,
		Creating,
		Pending,
		Starting,
		InProgress,
		Ending,
		Ended,
		Destroying
	}
}
