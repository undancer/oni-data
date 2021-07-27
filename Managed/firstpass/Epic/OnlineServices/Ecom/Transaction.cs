using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.Ecom
{
	public sealed class Transaction : Handle
	{
		public Transaction(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result GetTransactionId(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result source = EOS_Ecom_Transaction_GetTransactionId(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetEntitlementsCount(TransactionGetEntitlementsCountOptions options)
		{
			TransactionGetEntitlementsCountOptionsInternal options2 = Helper.CopyProperties<TransactionGetEntitlementsCountOptionsInternal>(options);
			uint source = EOS_Ecom_Transaction_GetEntitlementsCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyEntitlementByIndex(TransactionCopyEntitlementByIndexOptions options, out Entitlement outEntitlement)
		{
			TransactionCopyEntitlementByIndexOptionsInternal options2 = Helper.CopyProperties<TransactionCopyEntitlementByIndexOptionsInternal>(options);
			outEntitlement = Helper.GetDefault<Entitlement>();
			IntPtr outEntitlement2 = IntPtr.Zero;
			Result source = EOS_Ecom_Transaction_CopyEntitlementByIndex(base.InnerHandle, ref options2, ref outEntitlement2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<EntitlementInternal, Entitlement>(outEntitlement2, out outEntitlement))
			{
				EOS_Ecom_Entitlement_Release(outEntitlement2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void Release()
		{
			EOS_Ecom_Transaction_Release(base.InnerHandle);
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_Entitlement_Release(IntPtr entitlement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Ecom_Transaction_Release(IntPtr transaction);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_Transaction_CopyEntitlementByIndex(IntPtr handle, ref TransactionCopyEntitlementByIndexOptionsInternal options, ref IntPtr outEntitlement);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Ecom_Transaction_GetEntitlementsCount(IntPtr handle, ref TransactionGetEntitlementsCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Ecom_Transaction_GetTransactionId(IntPtr handle, StringBuilder outBuffer, ref int inOutBufferLength);
	}
}
