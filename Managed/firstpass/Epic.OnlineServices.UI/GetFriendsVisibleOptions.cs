namespace Epic.OnlineServices.UI
{
	public class GetFriendsVisibleOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}
	}
}
