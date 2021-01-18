namespace Epic.OnlineServices.Lobby
{
	public class GetInviteCountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}
	}
}
