using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UserInfo
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UserInfoDataInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_UserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Country;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_PreferredLanguage;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Nickname;

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

		public EpicAccountId UserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_UserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UserId, value);
			}
		}

		public string Country
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Country, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Country, value);
			}
		}

		public string DisplayName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_DisplayName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_DisplayName, value);
			}
		}

		public string PreferredLanguage
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_PreferredLanguage, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PreferredLanguage, value);
			}
		}

		public string Nickname
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Nickname, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Nickname, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
