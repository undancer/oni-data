using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UserInfo
{
	public sealed class UserInfoInterface : Handle
	{
		public const int CopyexternaluserinfobyaccountidApiLatest = 1;

		public const int CopyexternaluserinfobyaccounttypeApiLatest = 1;

		public const int CopyexternaluserinfobyindexApiLatest = 1;

		public const int GetexternaluserinfocountApiLatest = 1;

		public const int ExternaluserinfoApiLatest = 1;

		public const int CopyuserinfoApiLatest = 2;

		public const int MaxDisplaynameUtf8Length = 64;

		public const int MaxDisplaynameCharacters = 16;

		public const int QueryuserinfobyexternalaccountApiLatest = 1;

		public const int QueryuserinfobydisplaynameApiLatest = 1;

		public const int QueryuserinfoApiLatest = 1;

		public UserInfoInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryUserInfo(QueryUserInfoOptions options, object clientData, OnQueryUserInfoCallback completionDelegate)
		{
			QueryUserInfoOptionsInternal options2 = Helper.CopyProperties<QueryUserInfoOptionsInternal>(options);
			OnQueryUserInfoCallbackInternal onQueryUserInfoCallbackInternal = OnQueryUserInfo;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryUserInfoCallbackInternal);
			EOS_UserInfo_QueryUserInfo(base.InnerHandle, ref options2, clientDataAddress, onQueryUserInfoCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryUserInfoByDisplayName(QueryUserInfoByDisplayNameOptions options, object clientData, OnQueryUserInfoByDisplayNameCallback completionDelegate)
		{
			QueryUserInfoByDisplayNameOptionsInternal options2 = Helper.CopyProperties<QueryUserInfoByDisplayNameOptionsInternal>(options);
			OnQueryUserInfoByDisplayNameCallbackInternal onQueryUserInfoByDisplayNameCallbackInternal = OnQueryUserInfoByDisplayName;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryUserInfoByDisplayNameCallbackInternal);
			EOS_UserInfo_QueryUserInfoByDisplayName(base.InnerHandle, ref options2, clientDataAddress, onQueryUserInfoByDisplayNameCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryUserInfoByExternalAccount(QueryUserInfoByExternalAccountOptions options, object clientData, OnQueryUserInfoByExternalAccountCallback completionDelegate)
		{
			QueryUserInfoByExternalAccountOptionsInternal options2 = Helper.CopyProperties<QueryUserInfoByExternalAccountOptionsInternal>(options);
			OnQueryUserInfoByExternalAccountCallbackInternal onQueryUserInfoByExternalAccountCallbackInternal = OnQueryUserInfoByExternalAccount;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryUserInfoByExternalAccountCallbackInternal);
			EOS_UserInfo_QueryUserInfoByExternalAccount(base.InnerHandle, ref options2, clientDataAddress, onQueryUserInfoByExternalAccountCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public Result CopyUserInfo(CopyUserInfoOptions options, out UserInfoData outUserInfo)
		{
			CopyUserInfoOptionsInternal options2 = Helper.CopyProperties<CopyUserInfoOptionsInternal>(options);
			outUserInfo = Helper.GetDefault<UserInfoData>();
			IntPtr outUserInfo2 = IntPtr.Zero;
			Result source = EOS_UserInfo_CopyUserInfo(base.InnerHandle, ref options2, ref outUserInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<UserInfoDataInternal, UserInfoData>(outUserInfo2, out outUserInfo))
			{
				EOS_UserInfo_Release(outUserInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetExternalUserInfoCount(GetExternalUserInfoCountOptions options)
		{
			GetExternalUserInfoCountOptionsInternal options2 = Helper.CopyProperties<GetExternalUserInfoCountOptionsInternal>(options);
			uint source = EOS_UserInfo_GetExternalUserInfoCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyExternalUserInfoByIndex(CopyExternalUserInfoByIndexOptions options, out ExternalUserInfo outExternalUserInfo)
		{
			CopyExternalUserInfoByIndexOptionsInternal options2 = Helper.CopyProperties<CopyExternalUserInfoByIndexOptionsInternal>(options);
			outExternalUserInfo = Helper.GetDefault<ExternalUserInfo>();
			IntPtr outExternalUserInfo2 = IntPtr.Zero;
			Result source = EOS_UserInfo_CopyExternalUserInfoByIndex(base.InnerHandle, ref options2, ref outExternalUserInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<ExternalUserInfoInternal, ExternalUserInfo>(outExternalUserInfo2, out outExternalUserInfo))
			{
				EOS_UserInfo_ExternalUserInfo_Release(outExternalUserInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyExternalUserInfoByAccountType(CopyExternalUserInfoByAccountTypeOptions options, out ExternalUserInfo outExternalUserInfo)
		{
			CopyExternalUserInfoByAccountTypeOptionsInternal options2 = Helper.CopyProperties<CopyExternalUserInfoByAccountTypeOptionsInternal>(options);
			outExternalUserInfo = Helper.GetDefault<ExternalUserInfo>();
			IntPtr outExternalUserInfo2 = IntPtr.Zero;
			Result source = EOS_UserInfo_CopyExternalUserInfoByAccountType(base.InnerHandle, ref options2, ref outExternalUserInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<ExternalUserInfoInternal, ExternalUserInfo>(outExternalUserInfo2, out outExternalUserInfo))
			{
				EOS_UserInfo_ExternalUserInfo_Release(outExternalUserInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyExternalUserInfoByAccountId(CopyExternalUserInfoByAccountIdOptions options, out ExternalUserInfo outExternalUserInfo)
		{
			CopyExternalUserInfoByAccountIdOptionsInternal options2 = Helper.CopyProperties<CopyExternalUserInfoByAccountIdOptionsInternal>(options);
			outExternalUserInfo = Helper.GetDefault<ExternalUserInfo>();
			IntPtr outExternalUserInfo2 = IntPtr.Zero;
			Result source = EOS_UserInfo_CopyExternalUserInfoByAccountId(base.InnerHandle, ref options2, ref outExternalUserInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<ExternalUserInfoInternal, ExternalUserInfo>(outExternalUserInfo2, out outExternalUserInfo))
			{
				EOS_UserInfo_ExternalUserInfo_Release(outExternalUserInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void OnQueryUserInfoByExternalAccount(IntPtr address)
		{
			OnQueryUserInfoByExternalAccountCallback callback = null;
			QueryUserInfoByExternalAccountCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryUserInfoByExternalAccountCallback, QueryUserInfoByExternalAccountCallbackInfoInternal, QueryUserInfoByExternalAccountCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryUserInfoByDisplayName(IntPtr address)
		{
			OnQueryUserInfoByDisplayNameCallback callback = null;
			QueryUserInfoByDisplayNameCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryUserInfoByDisplayNameCallback, QueryUserInfoByDisplayNameCallbackInfoInternal, QueryUserInfoByDisplayNameCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryUserInfo(IntPtr address)
		{
			OnQueryUserInfoCallback callback = null;
			QueryUserInfoCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryUserInfoCallback, QueryUserInfoCallbackInfoInternal, QueryUserInfoCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_UserInfo_ExternalUserInfo_Release(IntPtr externalUserInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_UserInfo_Release(IntPtr userInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_UserInfo_CopyExternalUserInfoByAccountId(IntPtr handle, ref CopyExternalUserInfoByAccountIdOptionsInternal options, ref IntPtr outExternalUserInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_UserInfo_CopyExternalUserInfoByAccountType(IntPtr handle, ref CopyExternalUserInfoByAccountTypeOptionsInternal options, ref IntPtr outExternalUserInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_UserInfo_CopyExternalUserInfoByIndex(IntPtr handle, ref CopyExternalUserInfoByIndexOptionsInternal options, ref IntPtr outExternalUserInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_UserInfo_GetExternalUserInfoCount(IntPtr handle, ref GetExternalUserInfoCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_UserInfo_CopyUserInfo(IntPtr handle, ref CopyUserInfoOptionsInternal options, ref IntPtr outUserInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_UserInfo_QueryUserInfoByExternalAccount(IntPtr handle, ref QueryUserInfoByExternalAccountOptionsInternal options, IntPtr clientData, OnQueryUserInfoByExternalAccountCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_UserInfo_QueryUserInfoByDisplayName(IntPtr handle, ref QueryUserInfoByDisplayNameOptionsInternal options, IntPtr clientData, OnQueryUserInfoByDisplayNameCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_UserInfo_QueryUserInfo(IntPtr handle, ref QueryUserInfoOptionsInternal options, IntPtr clientData, OnQueryUserInfoCallbackInternal completionDelegate);
	}
}
