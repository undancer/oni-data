using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Metrics
{
	public sealed class MetricsInterface : Handle
	{
		public const int EndplayersessionApiLatest = 1;

		public const int BeginplayersessionApiLatest = 1;

		public MetricsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result BeginPlayerSession(BeginPlayerSessionOptions options)
		{
			BeginPlayerSessionOptionsInternal options2 = Helper.CopyProperties<BeginPlayerSessionOptionsInternal>(options);
			Result source = EOS_Metrics_BeginPlayerSession(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result EndPlayerSession(EndPlayerSessionOptions options)
		{
			EndPlayerSessionOptionsInternal options2 = Helper.CopyProperties<EndPlayerSessionOptionsInternal>(options);
			Result source = EOS_Metrics_EndPlayerSession(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Metrics_EndPlayerSession(IntPtr handle, ref EndPlayerSessionOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Metrics_BeginPlayerSession(IntPtr handle, ref BeginPlayerSessionOptionsInternal options);
	}
}
