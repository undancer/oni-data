using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyProductUserExternalAccountByAccountIdOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_TargetUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AccountId;

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

		public ProductUserId TargetUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_TargetUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_TargetUserId, value);
			}
		}

		public string AccountId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_AccountId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccountId, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
