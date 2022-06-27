using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyOfferImageInfoByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_OfferId;

		private uint m_ImageInfoIndex;

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

		public string OfferId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_OfferId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_OfferId, value);
			}
		}

		public uint ImageInfoIndex
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_ImageInfoIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ImageInfoIndex, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
