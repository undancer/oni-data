using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GetInviteIdByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private uint m_Index;

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

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public uint Index
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
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
