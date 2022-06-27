using System;

namespace Epic.OnlineServices.Platform
{
	public class Options
	{
		public int ApiVersion => 8;

		public IntPtr Reserved { get; set; }

		public string ProductId { get; set; }

		public string SandboxId { get; set; }

		public ClientCredentials ClientCredentials { get; set; }

		public bool IsServer { get; set; }

		public string EncryptionKey { get; set; }

		public string OverrideCountryCode { get; set; }

		public string OverrideLocaleCode { get; set; }

		public string DeploymentId { get; set; }

		public PlatformFlags Flags { get; set; }

		public string CacheDirectory { get; set; }

		public uint TickBudgetInMilliseconds { get; set; }
	}
}
