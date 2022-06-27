using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices
{
	public sealed class ProductUserId : Handle
	{
		public ProductUserId(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public bool IsValid()
		{
			int source = EOS_ProductUserId_IsValid(base.InnerHandle);
			bool target = Helper.GetDefault<bool>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result ToString(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result source = EOS_ProductUserId_ToString(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public static ProductUserId FromString(string accountIdString)
		{
			IntPtr source = EOS_ProductUserId_FromString(accountIdString);
			ProductUserId target = Helper.GetDefault<ProductUserId>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_ProductUserId_FromString([MarshalAs(UnmanagedType.LPStr)] string accountIdString);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_ProductUserId_ToString(IntPtr accountId, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern int EOS_ProductUserId_IsValid(IntPtr accountId);
	}
}
