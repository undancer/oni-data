using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices
{
	public sealed class ContinuanceToken : Handle
	{
		public ContinuanceToken(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result ToString(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result source = EOS_ContinuanceToken_ToString(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_ContinuanceToken_ToString(IntPtr continuanceToken, StringBuilder outBuffer, ref int inOutBufferLength);
	}
}
