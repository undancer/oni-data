namespace Epic.OnlineServices.Lobby
{
	public class QueryInvitesOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}
	}
}
