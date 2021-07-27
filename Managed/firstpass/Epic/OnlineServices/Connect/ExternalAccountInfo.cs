using System;

namespace Epic.OnlineServices.Connect
{
	public class ExternalAccountInfo
	{
		public int ApiVersion => 1;

		public ProductUserId ProductUserId { get; set; }

		public string DisplayName { get; set; }

		public string AccountId { get; set; }

		public ExternalAccountType AccountIdType { get; set; }

		public DateTimeOffset? LastLoginTime { get; set; }
	}
}
