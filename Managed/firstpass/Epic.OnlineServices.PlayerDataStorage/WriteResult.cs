namespace Epic.OnlineServices.PlayerDataStorage
{
	public enum WriteResult
	{
		ContinueWriting = 1,
		CompleteRequest,
		FailRequest,
		CancelRequest
	}
}
