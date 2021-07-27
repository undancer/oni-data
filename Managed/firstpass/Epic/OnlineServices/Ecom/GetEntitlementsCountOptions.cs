namespace Epic.OnlineServices.Ecom
{
	public class GetEntitlementsCountOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }
	}
}
