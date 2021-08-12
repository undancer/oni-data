using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CatalogItemInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CatalogNamespace;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Id;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_EntitlementName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_TitleText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DescriptionText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LongDescriptionText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_TechnicalDetailsText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DeveloperText;

		private EcomItemType m_ItemType;

		private long m_EntitlementEndTimestamp;

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

		public string CatalogNamespace
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_CatalogNamespace, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CatalogNamespace, value);
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

		public string TitleText
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_TitleText, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_TitleText, value);
			}
		}

		public string DescriptionText
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_DescriptionText, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_DescriptionText, value);
			}
		}

		public string LongDescriptionText
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_LongDescriptionText, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LongDescriptionText, value);
			}
		}

		public string TechnicalDetailsText
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_TechnicalDetailsText, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_TechnicalDetailsText, value);
			}
		}

		public string DeveloperText
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_DeveloperText, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_DeveloperText, value);
			}
		}

		public EcomItemType ItemType
		{
			get
			{
				EcomItemType target = Helper.GetDefault<EcomItemType>();
				Helper.TryMarshalGet(m_ItemType, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ItemType, value);
			}
		}

		public long EntitlementEndTimestamp
		{
			get
			{
				long target = Helper.GetDefault<long>();
				Helper.TryMarshalGet(m_EntitlementEndTimestamp, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EntitlementEndTimestamp, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
