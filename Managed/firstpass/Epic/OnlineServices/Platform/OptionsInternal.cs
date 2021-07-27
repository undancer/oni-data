using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Platform
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_Reserved;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SandboxId;

		private ClientCredentialsInternal m_ClientCredentials;

		private int m_IsServer;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_EncryptionKey;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_OverrideCountryCode;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_OverrideLocaleCode;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DeploymentId;

		private PlatformFlags m_Flags;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CacheDirectory;

		private uint m_TickBudgetInMilliseconds;

		public int ApiVersion
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_ApiVersion, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ApiVersion, value);
			}
		}

		public IntPtr Reserved
		{
			get
			{
				IntPtr target = Helper.GetDefault<IntPtr>();
				Helper.TryMarshalGet(m_Reserved, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Reserved, value);
			}
		}

		public string ProductId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ProductId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ProductId, value);
			}
		}

		public string SandboxId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_SandboxId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SandboxId, value);
			}
		}

		public ClientCredentialsInternal ClientCredentials
		{
			get
			{
				ClientCredentialsInternal target = Helper.GetDefault<ClientCredentialsInternal>();
				Helper.TryMarshalGet(m_ClientCredentials, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ClientCredentials, value);
			}
		}

		public bool IsServer
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_IsServer, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_IsServer, value);
			}
		}

		public string EncryptionKey
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_EncryptionKey, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EncryptionKey, value);
			}
		}

		public string OverrideCountryCode
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_OverrideCountryCode, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_OverrideCountryCode, value);
			}
		}

		public string OverrideLocaleCode
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_OverrideLocaleCode, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_OverrideLocaleCode, value);
			}
		}

		public string DeploymentId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_DeploymentId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_DeploymentId, value);
			}
		}

		public PlatformFlags Flags
		{
			get
			{
				PlatformFlags target = Helper.GetDefault<PlatformFlags>();
				Helper.TryMarshalGet(m_Flags, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Flags, value);
			}
		}

		public string CacheDirectory
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_CacheDirectory, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CacheDirectory, value);
			}
		}

		public uint TickBudgetInMilliseconds
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_TickBudgetInMilliseconds, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_TickBudgetInMilliseconds, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_ClientCredentials);
		}
	}
}
