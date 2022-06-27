namespace Epic.OnlineServices.Friends
{
	public class QueryFriendsOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }
	}
}
