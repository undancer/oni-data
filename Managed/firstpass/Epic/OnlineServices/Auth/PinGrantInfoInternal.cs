using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PinGrantInfoInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UserCode;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_VerificationURI;

		private int m_ExpiresIn;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_VerificationURIComplete;

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

		public string UserCode
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_UserCode, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UserCode, value);
			}
		}

		public string VerificationURI
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_VerificationURI, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_VerificationURI, value);
			}
		}

		public int ExpiresIn
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_ExpiresIn, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ExpiresIn, value);
			}
		}

		public string VerificationURIComplete
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_VerificationURIComplete, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_VerificationURIComplete, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
