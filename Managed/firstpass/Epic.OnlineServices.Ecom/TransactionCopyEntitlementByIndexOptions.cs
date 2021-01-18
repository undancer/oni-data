namespace Epic.OnlineServices.Ecom
{
	public class TransactionCopyEntitlementByIndexOptions
	{
		public int ApiVersion => 1;

		public uint EntitlementIndex
		{
			get;
			set;
		}
	}
}
