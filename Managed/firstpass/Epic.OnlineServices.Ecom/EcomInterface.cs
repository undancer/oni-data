using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	public sealed class EcomInterface : Handle
	{
		public const int TransactionCopyentitlementbyindexApiLatest = 1;

		public const int TransactionGetentitlementscountApiLatest = 1;

		public const int CopytransactionbyidApiLatest = 1;

		public const int CopytransactionbyindexApiLatest = 1;

		public const int GettransactioncountApiLatest = 1;

		public const int CopyitemreleasebyindexApiLatest = 1;

		public const int GetitemreleasecountApiLatest = 1;

		public const int CopyitemimageinfobyindexApiLatest = 1;

		public const int GetitemimageinfocountApiLatest = 1;

		public const int CopyofferimageinfobyindexApiLatest = 1;

		public const int GetofferimageinfocountApiLatest = 1;

		public const int CopyitembyidApiLatest = 1;

		public const int CopyofferitembyindexApiLatest = 1;

		public const int GetofferitemcountApiLatest = 1;

		public const int CopyofferbyidApiLatest = 1;

		public const int CopyofferbyindexApiLatest = 1;

		public const int GetoffercountApiLatest = 1;

		public const int CopyentitlementbyidApiLatest = 2;

		public const int CopyentitlementbynameandindexApiLatest = 1;

		public const int CopyentitlementbyindexApiLatest = 1;

		public const int GetentitlementsbynamecountApiLatest = 1;

		public const int GetentitlementscountApiLatest = 1;

		public const int RedeementitlementsMaxIds = 32;

		public const int RedeementitlementsApiLatest = 1;

		public const int TransactionidMaximumLength = 64;

		public const int CheckoutMaxEntries = 10;

		public const int CheckoutApiLatest = 1;

		public const int QueryoffersApiLatest = 1;

		public const int QueryentitlementsMaxEntitlementIds = 32;

		public const int QueryentitlementsApiLatest = 2;

		public const int QueryownershiptokenMaxCatalogitemIds = 32;

		public const int QueryownershiptokenApiLatest = 2;

		public const int QueryownershipMaxCatalogIds = 32;

		public const int QueryownershipApiLatest = 2;

		public const int CheckoutentryApiLatest = 1;

		public const int CatalogreleaseApiLatest = 1;

		public const int KeyimageinfoApiLatest = 1;

		public const int CatalogofferExpirationtimestampUndefined = -1;

		public const int CatalogofferApiLatest = 2;

		public const int CatalogitemEntitlementendtimestampUndefined = -1;

		public const int CatalogitemApiLatest = 1;

		public const int ItemownershipApiLatest = 1;

		public const int EntitlementEndtimestampUndefined = -1;

		public const int EntitlementApiLatest = 2;

		public EcomInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryOwnership(QueryOwnershipOptions options, object clientData, OnQueryOwnershipCallback completionDelegate)
		{
			QueryOwnershipOptionsInternal options2 = Helper.CopyProperties<QueryOwnershipOptionsInternal>(options);
			OnQueryOwnershipCallbackInternal onQueryOwnershipCallbackInternal = OnQueryOwnership;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryOwnershipCallbackInternal);
			EOS_Ecom_QueryOwnership(base.InnerHandle, ref options2, clientDataAddress, onQueryOwnershipCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryOwnershipToken(QueryOwnershipTokenOptions options, object clientData, OnQueryOwnershipTokenCallback completionDelegate)
		{
			QueryOwnershipTokenOptionsInternal options2 = Helper.CopyProperties<QueryOwnershipTokenOptionsInternal>(options);
			OnQueryOwnershipTokenCallbackInternal onQueryOwnershipTokenCallbackInternal = OnQueryOwnershipToken;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryOwnershipTokenCallbackInternal);
			EOS_Ecom_QueryOwnershipToken(base.InnerHandle, ref options2, clientDataAddress, onQueryOwnershipTokenCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryEntitlements(QueryEntitlementsOptions options, object clientData, OnQueryEntitlementsCallback completionDelegate)
		{
			QueryEntitlementsOptionsInternal options2 = Helper.CopyProperties<QueryEntitlementsOptionsInternal>(options);
			OnQueryEntitlementsCallbackInternal onQueryEntitlementsCallbackInternal = OnQueryEntitlements;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryEntitlementsCallbackInternal);
			EOS_Ecom_QueryEntitlements(base.InnerHandle, ref options2, clientDataAddress, onQueryEntitlementsCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryOffers(QueryOffersOptions options, object clientData, OnQueryOffersCallback completionDelegate)
		{
			QueryOffersOptionsInternal options2 = Helper.CopyProperties<QueryOffersOptionsInternal>(options);
			OnQueryOffersCallbackInternal onQueryOffersCallbackInternal = OnQueryOffers;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryOffersCallbackInternal);
			EOS_Ecom_QueryOffers(base.InnerHandle, ref options2, clientDataAddress, onQueryOffersCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void Checkout(CheckoutOptions options, object clientData, OnCheckoutCallback completionDelegate)
		{
			CheckoutOptionsInternal options2 = Helper.CopyProperties<CheckoutOptionsInternal>(options);
			OnCheckoutCallbackInternal onCheckoutCallbackInternal = OnCheckout;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onCheckoutCallbackInternal);
			EOS_Ecom_Checkout(base.InnerHandle, ref options2, clientDataAddress, onCheckoutCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void RedeemEntitlements(RedeemEntitlementsOptions options, object clientData, OnRedeemEntitlementsCallback completionDelegate)
		{
			RedeemEntitlementsOptionsInternal options2 = Helper.CopyProperties<RedeemEntitlementsOptionsInternal>(options);
			OnRedeemEntitlementsCallbackInternal onRedeemEntitlementsCallbackInternal = OnRedeemEntitlements;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onRedeemEntitlementsCallbackInternal);
			EOS_Ecom_RedeemEntitlements(base.InnerHandle, ref options2, clientDataAddress, onRedeemEntitlementsCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public uint GetEntitlementsCount(GetEntitlementsCountOptions options)
		{
			GetEntitlementsCountOptionsInternal options2 = Helper.CopyProperties<GetEntitlementsCountOptionsInternal>(options);
			uint source = EOS_Ecom_GetEntitlementsCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetEntitlementsByNameCount(GetEntitlementsByNameCountOptions options)
		{
			GetEntitlementsByNameCountOptionsInternal options2 = Helper.CopyProperties<GetEntitlementsByNameCountOptionsInternal>(options);
			uint source = EOS_Ecom_GetEntitlementsByNameCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyEntitlementByIndex(CopyEntitlementByIndexOptions options, out Entitlement outEntitlement)
		{
			CopyEntitlementByIndexOptionsInternal options2 = Helper.CopyProperties<CopyEntitlementByIndexOptionsInternal>(options);
			outEntitlement = Helper.GetDefault<Entitlement>();
			IntPtr outEntitlement2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyEntitlementByIndex(base.InnerHandle, ref options2, ref outEntitlement2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<EntitlementInternal, Entitlement>(outEntitlement2, out outEntitlement))
			{
				EOS_Ecom_Entitlement_Release(outEntitlement2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyEntitlementByNameAndIndex(CopyEntitlementByNameAndIndexOptions options, out Entitlement outEntitlement)
		{
			CopyEntitlementByNameAndIndexOptionsInternal options2 = Helper.CopyProperties<CopyEntitlementByNameAndIndexOptionsInternal>(options);
			outEntitlement = Helper.GetDefault<Entitlement>();
			IntPtr outEntitlement2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyEntitlementByNameAndIndex(base.InnerHandle, ref options2, ref outEntitlement2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<EntitlementInternal, Entitlement>(outEntitlement2, out outEntitlement))
			{
				EOS_Ecom_Entitlement_Release(outEntitlement2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyEntitlementById(CopyEntitlementByIdOptions options, out Entitlement outEntitlement)
		{
			CopyEntitlementByIdOptionsInternal options2 = Helper.CopyProperties<CopyEntitlementByIdOptionsInternal>(options);
			outEntitlement = Helper.GetDefault<Entitlement>();
			IntPtr outEntitlement2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyEntitlementById(base.InnerHandle, ref options2, ref outEntitlement2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<EntitlementInternal, Entitlement>(outEntitlement2, out outEntitlement))
			{
				EOS_Ecom_Entitlement_Release(outEntitlement2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetOfferCount(GetOfferCountOptions options)
		{
			GetOfferCountOptionsInternal options2 = Helper.CopyProperties<GetOfferCountOptionsInternal>(options);
			uint source = EOS_Ecom_GetOfferCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyOfferByIndex(CopyOfferByIndexOptions options, out CatalogOffer outOffer)
		{
			CopyOfferByIndexOptionsInternal options2 = Helper.CopyProperties<CopyOfferByIndexOptionsInternal>(options);
			outOffer = Helper.GetDefault<CatalogOffer>();
			IntPtr outOffer2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyOfferByIndex(base.InnerHandle, ref options2, ref outOffer2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<CatalogOfferInternal, CatalogOffer>(outOffer2, out outOffer))
			{
				EOS_Ecom_CatalogOffer_Release(outOffer2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyOfferById(CopyOfferByIdOptions options, out CatalogOffer outOffer)
		{
			CopyOfferByIdOptionsInternal options2 = Helper.CopyProperties<CopyOfferByIdOptionsInternal>(options);
			outOffer = Helper.GetDefault<CatalogOffer>();
			IntPtr outOffer2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyOfferById(base.InnerHandle, ref options2, ref outOffer2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<CatalogOfferInternal, CatalogOffer>(outOffer2, out outOffer))
			{
				EOS_Ecom_CatalogOffer_Release(outOffer2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetOfferItemCount(GetOfferItemCountOptions options)
		{
			GetOfferItemCountOptionsInternal options2 = Helper.CopyProperties<GetOfferItemCountOptionsInternal>(options);
			uint source = EOS_Ecom_GetOfferItemCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyOfferItemByIndex(CopyOfferItemByIndexOptions options, out CatalogItem outItem)
		{
			CopyOfferItemByIndexOptionsInternal options2 = Helper.CopyProperties<CopyOfferItemByIndexOptionsInternal>(options);
			outItem = Helper.GetDefault<CatalogItem>();
			IntPtr outItem2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyOfferItemByIndex(base.InnerHandle, ref options2, ref outItem2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<CatalogItemInternal, CatalogItem>(outItem2, out outItem))
			{
				EOS_Ecom_CatalogItem_Release(outItem2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyItemById(CopyItemByIdOptions options, out CatalogItem outItem)
		{
			CopyItemByIdOptionsInternal options2 = Helper.CopyProperties<CopyItemByIdOptionsInternal>(options);
			outItem = Helper.GetDefault<CatalogItem>();
			IntPtr outItem2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyItemById(base.InnerHandle, ref options2, ref outItem2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<CatalogItemInternal, CatalogItem>(outItem2, out outItem))
			{
				EOS_Ecom_CatalogItem_Release(outItem2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetOfferImageInfoCount(GetOfferImageInfoCountOptions options)
		{
			GetOfferImageInfoCountOptionsInternal options2 = Helper.CopyProperties<GetOfferImageInfoCountOptionsInternal>(options);
			uint source = EOS_Ecom_GetOfferImageInfoCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyOfferImageInfoByIndex(CopyOfferImageInfoByIndexOptions options, out KeyImageInfo outImageInfo)
		{
			CopyOfferImageInfoByIndexOptionsInternal options2 = Helper.CopyProperties<CopyOfferImageInfoByIndexOptionsInternal>(options);
			outImageInfo = Helper.GetDefault<KeyImageInfo>();
			IntPtr outImageInfo2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyOfferImageInfoByIndex(base.InnerHandle, ref options2, ref outImageInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<KeyImageInfoInternal, KeyImageInfo>(outImageInfo2, out outImageInfo))
			{
				EOS_Ecom_KeyImageInfo_Release(outImageInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetItemImageInfoCount(GetItemImageInfoCountOptions options)
		{
			GetItemImageInfoCountOptionsInternal options2 = Helper.CopyProperties<GetItemImageInfoCountOptionsInternal>(options);
			uint source = EOS_Ecom_GetItemImageInfoCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyItemImageInfoByIndex(CopyItemImageInfoByIndexOptions options, out KeyImageInfo outImageInfo)
		{
			CopyItemImageInfoByIndexOptionsInternal options2 = Helper.CopyProperties<CopyItemImageInfoByIndexOptionsInternal>(options);
			outImageInfo = Helper.GetDefault<KeyImageInfo>();
			IntPtr outImageInfo2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyItemImageInfoByIndex(base.InnerHandle, ref options2, ref outImageInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<KeyImageInfoInternal, KeyImageInfo>(outImageInfo2, out outImageInfo))
			{
				EOS_Ecom_KeyImageInfo_Release(outImageInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetItemReleaseCount(GetItemReleaseCountOptions options)
		{
			GetItemReleaseCountOptionsInternal options2 = Helper.CopyProperties<GetItemReleaseCountOptionsInternal>(options);
			uint source = EOS_Ecom_GetItemReleaseCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyItemReleaseByIndex(CopyItemReleaseByIndexOptions options, out CatalogRelease outRelease)
		{
			CopyItemReleaseByIndexOptionsInternal options2 = Helper.CopyProperties<CopyItemReleaseByIndexOptionsInternal>(options);
			outRelease = Helper.GetDefault<CatalogRelease>();
			IntPtr outRelease2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyItemReleaseByIndex(base.InnerHandle, ref options2, ref outRelease2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<CatalogReleaseInternal, CatalogRelease>(outRelease2, out outRelease))
			{
				EOS_Ecom_CatalogRelease_Release(outRelease2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetTransactionCount(GetTransactionCountOptions options)
		{
			GetTransactionCountOptionsInternal options2 = Helper.CopyProperties<GetTransactionCountOptionsInternal>(options);
			uint source = EOS_Ecom_GetTransactionCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyTransactionByIndex(CopyTransactionByIndexOptions options, out Transaction outTransaction)
		{
			CopyTransactionByIndexOptionsInternal options2 = Helper.CopyProperties<CopyTransactionByIndexOptionsInternal>(options);
			outTransaction = Helper.GetDefault<Transaction>();
			IntPtr outTransaction2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyTransactionByIndex(base.InnerHandle, ref options2, ref outTransaction2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outTransaction2, out outTransaction);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyTransactionById(CopyTransactionByIdOptions options, out Transaction outTransaction)
		{
			CopyTransactionByIdOptionsInternal options2 = Helper.CopyProperties<CopyTransactionByIdOptionsInternal>(options);
			outTransaction = Helper.GetDefault<Transaction>();
			IntPtr outTransaction2 = IntPtr.Zero;
			Result source = EOS_Ecom_CopyTransactionById(base.InnerHandle, ref options2, ref outTransaction2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outTransaction2, out outTransaction);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void OnRedeemEntitlements(IntPtr address)
		{
			OnRedeemEntitlementsCallback callback = null;
			RedeemEntitlementsCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnRedeemEntitlementsCallback, RedeemEntitlementsCallbackInfoInternal, RedeemEntitlementsCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnCheckout(IntPtr address)
		{
			OnCheckoutCallback callback = null;
			CheckoutCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnCheckoutCallback, CheckoutCallbackInfoInternal, CheckoutCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryOffers(IntPtr address)
		{
			OnQueryOffersCallback callback = null;
			QueryOffersCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryOffersCallback, QueryOffersCallbackInfoInternal, QueryOffersCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryEntitlements(IntPtr address)
		{
			OnQueryEntitlementsCallback callback = null;
			QueryEntitlementsCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryEntitlementsCallback, QueryEntitlementsCallbackInfoInternal, QueryEntitlementsCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryOwnershipToken(IntPtr address)
		{
			OnQueryOwnershipTokenCallback callback = null;
			QueryOwnershipTokenCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryOwnershipTokenCallback, QueryOwnershipTokenCallbackInfoInternal, QueryOwnershipTokenCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryOwnership(IntPtr address)
		{
			OnQueryOwnershipCallback callback = null;
			QueryOwnershipCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryOwnershipCallback, QueryOwnershipCallbackInfoInternal, QueryOwnershipCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_CatalogRelease_Release(IntPtr catalogRelease);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_KeyImageInfo_Release(IntPtr keyImageInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_CatalogOffer_Release(IntPtr catalogOffer);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_CatalogItem_Release(IntPtr catalogItem);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_Entitlement_Release(IntPtr entitlement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyTransactionById(IntPtr handle, ref CopyTransactionByIdOptionsInternal options, ref IntPtr outTransaction);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyTransactionByIndex(IntPtr handle, ref CopyTransactionByIndexOptionsInternal options, ref IntPtr outTransaction);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Ecom_GetTransactionCount(IntPtr handle, ref GetTransactionCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyItemReleaseByIndex(IntPtr handle, ref CopyItemReleaseByIndexOptionsInternal options, ref IntPtr outRelease);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Ecom_GetItemReleaseCount(IntPtr handle, ref GetItemReleaseCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyItemImageInfoByIndex(IntPtr handle, ref CopyItemImageInfoByIndexOptionsInternal options, ref IntPtr outImageInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Ecom_GetItemImageInfoCount(IntPtr handle, ref GetItemImageInfoCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyOfferImageInfoByIndex(IntPtr handle, ref CopyOfferImageInfoByIndexOptionsInternal options, ref IntPtr outImageInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Ecom_GetOfferImageInfoCount(IntPtr handle, ref GetOfferImageInfoCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyItemById(IntPtr handle, ref CopyItemByIdOptionsInternal options, ref IntPtr outItem);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyOfferItemByIndex(IntPtr handle, ref CopyOfferItemByIndexOptionsInternal options, ref IntPtr outItem);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Ecom_GetOfferItemCount(IntPtr handle, ref GetOfferItemCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyOfferById(IntPtr handle, ref CopyOfferByIdOptionsInternal options, ref IntPtr outOffer);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyOfferByIndex(IntPtr handle, ref CopyOfferByIndexOptionsInternal options, ref IntPtr outOffer);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Ecom_GetOfferCount(IntPtr handle, ref GetOfferCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyEntitlementById(IntPtr handle, ref CopyEntitlementByIdOptionsInternal options, ref IntPtr outEntitlement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyEntitlementByNameAndIndex(IntPtr handle, ref CopyEntitlementByNameAndIndexOptionsInternal options, ref IntPtr outEntitlement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_CopyEntitlementByIndex(IntPtr handle, ref CopyEntitlementByIndexOptionsInternal options, ref IntPtr outEntitlement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Ecom_GetEntitlementsByNameCount(IntPtr handle, ref GetEntitlementsByNameCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Ecom_GetEntitlementsCount(IntPtr handle, ref GetEntitlementsCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_RedeemEntitlements(IntPtr handle, ref RedeemEntitlementsOptionsInternal options, IntPtr clientData, OnRedeemEntitlementsCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_Checkout(IntPtr handle, ref CheckoutOptionsInternal options, IntPtr clientData, OnCheckoutCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_QueryOffers(IntPtr handle, ref QueryOffersOptionsInternal options, IntPtr clientData, OnQueryOffersCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_QueryEntitlements(IntPtr handle, ref QueryEntitlementsOptionsInternal options, IntPtr clientData, OnQueryEntitlementsCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_QueryOwnershipToken(IntPtr handle, ref QueryOwnershipTokenOptionsInternal options, IntPtr clientData, OnQueryOwnershipTokenCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_QueryOwnership(IntPtr handle, ref QueryOwnershipOptionsInternal options, IntPtr clientData, OnQueryOwnershipCallbackInternal completionDelegate);
	}
}
