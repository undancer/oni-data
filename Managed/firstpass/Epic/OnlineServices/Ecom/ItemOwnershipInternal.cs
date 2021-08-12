using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ItemOwnershipInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Id;

		private OwnershipStatus m_OwnershipStatus;

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

		public string Id
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Id, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Id, value);
			}
		}

		public OwnershipStatus OwnershipStatus
		{
			get
			{
				OwnershipStatus target = Helper.GetDefault<OwnershipStatus>();
				Helper.TryMarshalGet(m_OwnershipStatus, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_OwnershipStatus, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
