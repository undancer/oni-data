using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct EntitlementInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_EntitlementName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_EntitlementId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CatalogItemId;

		private int m_ServerIndex;

		private int m_Redeemed;

		private long m_EndTimestamp;

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

		public string EntitlementName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_EntitlementName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EntitlementName, value);
			}
		}

		public string EntitlementId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_EntitlementId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EntitlementId, value);
			}
		}

		public string CatalogItemId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_CatalogItemId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CatalogItemId, value);
			}
		}

		public int ServerIndex
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_ServerIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ServerIndex, value);
			}
		}

		public bool Redeemed
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_Redeemed, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Redeemed, value);
			}
		}

		public long EndTimestamp
		{
			get
			{
				long target = Helper.GetDefault<long>();
				Helper.TryMarshalGet(m_EndTimestamp, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EndTimestamp, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
