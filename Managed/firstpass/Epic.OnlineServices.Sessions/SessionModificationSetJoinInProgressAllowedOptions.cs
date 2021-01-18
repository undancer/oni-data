namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetJoinInProgressAllowedOptions
	{
		public int ApiVersion => 1;

		public bool AllowJoinInProgress
		{
			get;
			set;
		}
	}
}
