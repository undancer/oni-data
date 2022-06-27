using System;
using Epic.OnlineServices.Connect;

namespace Epic.OnlineServices.Auth
{
	public class Credentials
	{
		public int ApiVersion => 3;

		public string Id { get; set; }

		public string Token { get; set; }

		public LoginCredentialType Type { get; set; }

		public IntPtr SystemAuthCredentialsOptions { get; set; }

		public ExternalCredentialType ExternalType { get; set; }
	}
}
