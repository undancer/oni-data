using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CatalogOfferInternal : IDisposable
	{
		private int m_ApiVersion;

		private int m_ServerIndex;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CatalogNamespace;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Id;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_TitleText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DescriptionText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LongDescriptionText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_TechnicalDetailsText_DEPRECATED;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CurrencyCode;

		private Result m_PriceResult;

		private uint m_OriginalPrice;

		private uint m_CurrentPrice;

		private byte m_DiscountPercentage;

		private long m_ExpirationTimestamp;

		private uint m_PurchasedCount;

		private int m_PurchaseLimit;

		private int m_AvailableForPurchase;

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

		public string TechnicalDetailsText_DEPRECATED
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_TechnicalDetailsText_DEPRECATED, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_TechnicalDetailsText_DEPRECATED, value);
			}
		}

		public string CurrencyCode
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_CurrencyCode, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CurrencyCode, value);
			}
		}

		public Result PriceResult
		{
			get
			{
				Result target = Helper.GetDefault<Result>();
				Helper.TryMarshalGet(m_PriceResult, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PriceResult, value);
			}
		}

		public uint OriginalPrice
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_OriginalPrice, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_OriginalPrice, value);
			}
		}

		public uint CurrentPrice
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_CurrentPrice, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CurrentPrice, value);
			}
		}

		public byte DiscountPercentage
		{
			get
			{
				byte target = Helper.GetDefault<byte>();
				Helper.TryMarshalGet(m_DiscountPercentage, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_DiscountPercentage, value);
			}
		}

		public long ExpirationTimestamp
		{
			get
			{
				long target = Helper.GetDefault<long>();
				Helper.TryMarshalGet(m_ExpirationTimestamp, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ExpirationTimestamp, value);
			}
		}

		public uint PurchasedCount
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_PurchasedCount, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PurchasedCount, value);
			}
		}

		public int PurchaseLimit
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_PurchaseLimit, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PurchaseLimit, value);
			}
		}

		public bool AvailableForPurchase
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_AvailableForPurchase, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AvailableForPurchase, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
