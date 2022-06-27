using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UserInfo
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyExternalUserInfoByAccountIdOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

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

		public EpicAccountId LocalUserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public EpicAccountId TargetUserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
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
