using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	public sealed class ActiveSession : Handle
	{
		public ActiveSession(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result CopyInfo(ActiveSessionCopyInfoOptions options, out ActiveSessionInfo outActiveSessionInfo)
		{
			ActiveSessionCopyInfoOptionsInternal options2 = Helper.CopyProperties<ActiveSessionCopyInfoOptionsInternal>(options);
			outActiveSessionInfo = Helper.GetDefault<ActiveSessionInfo>();
			IntPtr outActiveSessionInfo2 = IntPtr.Zero;
			Result source = EOS_ActiveSession_CopyInfo(base.InnerHandle, ref options2, ref outActiveSessionInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<ActiveSessionInfoInternal, ActiveSessionInfo>(outActiveSessionInfo2, out outActiveSessionInfo))
			{
				EOS_ActiveSession_Info_Release(outActiveSessionInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetRegisteredPlayerCount(ActiveSessionGetRegisteredPlayerCountOptions options)
		{
			ActiveSessionGetRegisteredPlayerCountOptionsInternal options2 = Helper.CopyProperties<ActiveSessionGetRegisteredPlayerCountOptionsInternal>(options);
			uint source = EOS_ActiveSession_GetRegisteredPlayerCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ProductUserId GetRegisteredPlayerByIndex(ActiveSessionGetRegisteredPlayerByIndexOptions options)
		{
			ActiveSessionGetRegisteredPlayerByIndexOptionsInternal options2 = Helper.CopyProperties<ActiveSessionGetRegisteredPlayerByIndexOptionsInternal>(options);
			IntPtr source = EOS_ActiveSession_GetRegisteredPlayerByIndex(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			ProductUserId target = Helper.GetDefault<ProductUserId>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void Release()
		{
			EOS_ActiveSession_Release(base.InnerHandle);
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_ActiveSession_Info_Release(IntPtr activeSessionInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_ActiveSession_Release(IntPtr activeSessionHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_ActiveSession_GetRegisteredPlayerByIndex(IntPtr handle, ref ActiveSessionGetRegisteredPlayerByIndexOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_ActiveSession_GetRegisteredPlayerCount(IntPtr handle, ref ActiveSessionGetRegisteredPlayerCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_ActiveSession_CopyInfo(IntPtr handle, ref ActiveSessionCopyInfoOptionsInternal options, ref IntPtr outActiveSessionInfo);
	}
}
