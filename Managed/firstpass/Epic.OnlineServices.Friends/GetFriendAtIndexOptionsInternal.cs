using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Friends
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GetFriendAtIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private int m_Index;

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

		public int Index
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_Index, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Index, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
