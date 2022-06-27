namespace Epic.OnlineServices.Friends
{
	public class GetFriendsCountOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }
	}
}
