namespace Epic.OnlineServices.UI
{
	public class AcknowledgeEventIdOptions
	{
		public int ApiVersion => 1;

		public ulong UiEventId
		{
			get;
			set;
		}

		public Result Result
		{
			get;
			set;
		}
	}
}
