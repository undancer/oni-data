using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyProductUserExternalAccountByAccountTypeOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_TargetUserId;

		private ExternalAccountType m_AccountIdType;

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

		public ExternalAccountType AccountIdType
		{
			get
			{
				ExternalAccountType target = Helper.GetDefault<ExternalAccountType>();
				Helper.TryMarshalGet(m_AccountIdType, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccountIdType, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
