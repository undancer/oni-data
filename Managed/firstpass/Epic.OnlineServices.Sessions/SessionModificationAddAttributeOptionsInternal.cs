using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionModificationAddAttributeOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_SessionAttribute;

		private SessionAttributeAdvertisementType m_AdvertisementType;

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

		public AttributeDataInternal? SessionAttribute
		{
			get
			{
				AttributeDataInternal? target = Helper.GetDefault<AttributeDataInternal?>();
				Helper.TryMarshalGet(m_SessionAttribute, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SessionAttribute, value);
			}
		}

		public SessionAttributeAdvertisementType AdvertisementType
		{
			get
			{
				SessionAttributeAdvertisementType target = Helper.GetDefault<SessionAttributeAdvertisementType>();
				Helper.TryMarshalGet(m_AdvertisementType, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AdvertisementType, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_SessionAttribute);
		}
	}
}
