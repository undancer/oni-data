namespace rail
{
	public class AsyncQueryQuotaResult : EventBase
	{
		public ulong available_quota;

		public ulong total_quota;
	}
}
