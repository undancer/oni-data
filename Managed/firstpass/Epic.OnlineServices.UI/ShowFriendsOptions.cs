namespace Epic.OnlineServices.UI
{
	public class ShowFriendsOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}
	}
}
