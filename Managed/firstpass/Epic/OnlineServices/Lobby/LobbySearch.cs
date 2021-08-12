using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	public sealed class LobbySearch : Handle
	{
		public LobbySearch(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void Find(LobbySearchFindOptions options, object clientData, LobbySearchOnFindCallback completionDelegate)
		{
			LobbySearchFindOptionsInternal options2 = Helper.CopyProperties<LobbySearchFindOptionsInternal>(options);
			LobbySearchOnFindCallbackInternal lobbySearchOnFindCallbackInternal = LobbySearchOnFind;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, lobbySearchOnFindCallbackInternal);
			EOS_LobbySearch_Find(base.InnerHandle, ref options2, clientDataAddress, lobbySearchOnFindCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public Result SetLobbyId(LobbySearchSetLobbyIdOptions options)
		{
			LobbySearchSetLobbyIdOptionsInternal options2 = Helper.CopyProperties<LobbySearchSetLobbyIdOptionsInternal>(options);
			Result source = EOS_LobbySearch_SetLobbyId(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetTargetUserId(LobbySearchSetTargetUserIdOptions options)
		{
			LobbySearchSetTargetUserIdOptionsInternal options2 = Helper.CopyProperties<LobbySearchSetTargetUserIdOptionsInternal>(options);
			Result source = EOS_LobbySearch_SetTargetUserId(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetParameter(LobbySearchSetParameterOptions options)
		{
			LobbySearchSetParameterOptionsInternal options2 = Helper.CopyProperties<LobbySearchSetParameterOptionsInternal>(options);
			Result source = EOS_LobbySearch_SetParameter(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result RemoveParameter(LobbySearchRemoveParameterOptions options)
		{
			LobbySearchRemoveParameterOptionsInternal options2 = Helper.CopyProperties<LobbySearchRemoveParameterOptionsInternal>(options);
			Result source = EOS_LobbySearch_RemoveParameter(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetMaxResults(LobbySearchSetMaxResultsOptions options)
		{
			LobbySearchSetMaxResultsOptionsInternal options2 = Helper.CopyProperties<LobbySearchSetMaxResultsOptionsInternal>(options);
			Result source = EOS_LobbySearch_SetMaxResults(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetSearchResultCount(LobbySearchGetSearchResultCountOptions options)
		{
			LobbySearchGetSearchResultCountOptionsInternal options2 = Helper.CopyProperties<LobbySearchGetSearchResultCountOptionsInternal>(options);
			uint source = EOS_LobbySearch_GetSearchResultCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopySearchResultByIndex(LobbySearchCopySearchResultByIndexOptions options, out LobbyDetails outLobbyDetailsHandle)
		{
			LobbySearchCopySearchResultByIndexOptionsInternal options2 = Helper.CopyProperties<LobbySearchCopySearchResultByIndexOptionsInternal>(options);
			outLobbyDetailsHandle = Helper.GetDefault<LobbyDetails>();
			IntPtr outLobbyDetailsHandle2 = IntPtr.Zero;
			Result source = EOS_LobbySearch_CopySearchResultByIndex(base.InnerHandle, ref options2, ref outLobbyDetailsHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outLobbyDetailsHandle2, out outLobbyDetailsHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void Release()
		{
			EOS_LobbySearch_Release(base.InnerHandle);
		}

		[MonoPInvokeCallback]
		internal static void LobbySearchOnFind(IntPtr address)
		{
			LobbySearchOnFindCallback callback = null;
			LobbySearchFindCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<LobbySearchOnFindCallback, LobbySearchFindCallbackInfoInternal, LobbySearchFindCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_LobbySearch_Release(IntPtr lobbySearchHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_LobbySearch_CopySearchResultByIndex(IntPtr handle, ref LobbySearchCopySearchResultByIndexOptionsInternal options, ref IntPtr outLobbyDetailsHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_LobbySearch_GetSearchResultCount(IntPtr handle, ref LobbySearchGetSearchResultCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_LobbySearch_SetMaxResults(IntPtr handle, ref LobbySearchSetMaxResultsOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_LobbySearch_RemoveParameter(IntPtr handle, ref LobbySearchRemoveParameterOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_LobbySearch_SetParameter(IntPtr handle, ref LobbySearchSetParameterOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_LobbySearch_SetTargetUserId(IntPtr handle, ref LobbySearchSetTargetUserIdOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_LobbySearch_SetLobbyId(IntPtr handle, ref LobbySearchSetLobbyIdOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_LobbySearch_Find(IntPtr handle, ref LobbySearchFindOptionsInternal options, IntPtr clientData, LobbySearchOnFindCallbackInternal completionDelegate);
	}
}
