namespace Epic.OnlineServices.Auth
{
	public class LoginCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public EpicAccountId LocalUserId { get; set; }

		public PinGrantInfo PinGrantInfo { get; set; }

		public ContinuanceToken ContinuanceToken { get; set; }

		public AccountFeatureRestrictedInfo AccountFeatureRestrictedInfo { get; set; }
	}
}
