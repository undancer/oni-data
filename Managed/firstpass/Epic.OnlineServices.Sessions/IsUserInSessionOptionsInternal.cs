using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct IsUserInSessionOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SessionName;

		private IntPtr m_TargetUserId;

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

		public string SessionName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_SessionName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SessionName, value);
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

		public void Dispose()
		{
		}
	}
}
