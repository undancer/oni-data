using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Metrics
{
	[StructLayout(LayoutKind.Explicit, Pack = 4)]
	internal struct EndPlayerSessionOptionsAccountIdInternal : IDisposable
	{
		[FieldOffset(0)]
		private MetricsAccountIdType m_AccountIdType;

		[FieldOffset(4)]
		private IntPtr m_Epic;

		[FieldOffset(4)]
		private IntPtr m_External;

		public MetricsAccountIdType AccountIdType
		{
			get
			{
				MetricsAccountIdType target = Helper.GetDefault<MetricsAccountIdType>();
				Helper.TryMarshalGet(m_AccountIdType, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccountIdType, value);
			}
		}

		public EpicAccountId Epic
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_Epic, out target, m_AccountIdType, MetricsAccountIdType.Epic);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Epic, value, ref m_AccountIdType, MetricsAccountIdType.Epic, this);
			}
		}

		public string External
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_External, out target, m_AccountIdType, MetricsAccountIdType.External);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_External, value, ref m_AccountIdType, MetricsAccountIdType.External, this);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_External, m_AccountIdType, MetricsAccountIdType.External);
		}
	}
}
