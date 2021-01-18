namespace Epic.OnlineServices.Sessions
{
	public class CopySessionHandleByUiEventIdOptions
	{
		public int ApiVersion => 1;

		public ulong UiEventId
		{
			get;
			set;
		}
	}
}
