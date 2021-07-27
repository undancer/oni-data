using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	public sealed class SessionSearch : Handle
	{
		public SessionSearch(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result SetSessionId(SessionSearchSetSessionIdOptions options)
		{
			SessionSearchSetSessionIdOptionsInternal options2 = Helper.CopyProperties<SessionSearchSetSessionIdOptionsInternal>(options);
			Result source = EOS_SessionSearch_SetSessionId(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetTargetUserId(SessionSearchSetTargetUserIdOptions options)
		{
			SessionSearchSetTargetUserIdOptionsInternal options2 = Helper.CopyProperties<SessionSearchSetTargetUserIdOptionsInternal>(options);
			Result source = EOS_SessionSearch_SetTargetUserId(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetParameter(SessionSearchSetParameterOptions options)
		{
			SessionSearchSetParameterOptionsInternal options2 = Helper.CopyProperties<SessionSearchSetParameterOptionsInternal>(options);
			Result source = EOS_SessionSearch_SetParameter(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result RemoveParameter(SessionSearchRemoveParameterOptions options)
		{
			SessionSearchRemoveParameterOptionsInternal options2 = Helper.CopyProperties<SessionSearchRemoveParameterOptionsInternal>(options);
			Result source = EOS_SessionSearch_RemoveParameter(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetMaxResults(SessionSearchSetMaxResultsOptions options)
		{
			SessionSearchSetMaxResultsOptionsInternal options2 = Helper.CopyProperties<SessionSearchSetMaxResultsOptionsInternal>(options);
			Result source = EOS_SessionSearch_SetMaxResults(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void Find(SessionSearchFindOptions options, object clientData, SessionSearchOnFindCallback completionDelegate)
		{
			SessionSearchFindOptionsInternal options2 = Helper.CopyProperties<SessionSearchFindOptionsInternal>(options);
			SessionSearchOnFindCallbackInternal sessionSearchOnFindCallbackInternal = SessionSearchOnFind;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, sessionSearchOnFindCallbackInternal);
			EOS_SessionSearch_Find(base.InnerHandle, ref options2, clientDataAddress, sessionSearchOnFindCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public uint GetSearchResultCount(SessionSearchGetSearchResultCountOptions options)
		{
			SessionSearchGetSearchResultCountOptionsInternal options2 = Helper.CopyProperties<SessionSearchGetSearchResultCountOptionsInternal>(options);
			uint source = EOS_SessionSearch_GetSearchResultCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopySearchResultByIndex(SessionSearchCopySearchResultByIndexOptions options, out SessionDetails outSessionHandle)
		{
			SessionSearchCopySearchResultByIndexOptionsInternal options2 = Helper.CopyProperties<SessionSearchCopySearchResultByIndexOptionsInternal>(options);
			outSessionHandle = Helper.GetDefault<SessionDetails>();
			IntPtr outSessionHandle2 = IntPtr.Zero;
			Result source = EOS_SessionSearch_CopySearchResultByIndex(base.InnerHandle, ref options2, ref outSessionHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outSessionHandle2, out outSessionHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void Release()
		{
			EOS_SessionSearch_Release(base.InnerHandle);
		}

		[MonoPInvokeCallback]
		internal static void SessionSearchOnFind(IntPtr address)
		{
			SessionSearchOnFindCallback callback = null;
			SessionSearchFindCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<SessionSearchOnFindCallback, SessionSearchFindCallbackInfoInternal, SessionSearchFindCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_SessionSearch_Release(IntPtr sessionSearchHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_SessionSearch_CopySearchResultByIndex(IntPtr handle, ref SessionSearchCopySearchResultByIndexOptionsInternal options, ref IntPtr outSessionHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_SessionSearch_GetSearchResultCount(IntPtr handle, ref SessionSearchGetSearchResultCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_SessionSearch_Find(IntPtr handle, ref SessionSearchFindOptionsInternal options, IntPtr clientData, SessionSearchOnFindCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_SessionSearch_SetMaxResults(IntPtr handle, ref SessionSearchSetMaxResultsOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_SessionSearch_RemoveParameter(IntPtr handle, ref SessionSearchRemoveParameterOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_SessionSearch_SetParameter(IntPtr handle, ref SessionSearchSetParameterOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_SessionSearch_SetTargetUserId(IntPtr handle, ref SessionSearchSetTargetUserIdOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_SessionSearch_SetSessionId(IntPtr handle, ref SessionSearchSetSessionIdOptionsInternal options);
	}
}
