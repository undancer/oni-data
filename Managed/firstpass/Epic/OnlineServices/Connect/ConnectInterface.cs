using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.Connect
{
	public sealed class ConnectInterface : Handle
	{
		public const int AddnotifyloginstatuschangedApiLatest = 1;

		public const int OnauthexpirationcallbackApiLatest = 1;

		public const int AddnotifyauthexpirationApiLatest = 1;

		public const int ExternalaccountinfoApiLatest = 1;

		public const int TimeUndefined = -1;

		public const int CopyproductuserinfoApiLatest = 1;

		public const int CopyproductuserexternalaccountbyaccountidApiLatest = 1;

		public const int CopyproductuserexternalaccountbyaccounttypeApiLatest = 1;

		public const int CopyproductuserexternalaccountbyindexApiLatest = 1;

		public const int GetproductuserexternalaccountcountApiLatest = 1;

		public const int GetproductuseridmappingApiLatest = 1;

		public const int QueryproductuseridmappingsApiLatest = 2;

		public const int GetexternalaccountmappingsApiLatest = 1;

		public const int GetexternalaccountmappingApiLatest = 1;

		public const int QueryexternalaccountmappingsMaxAccountIds = 128;

		public const int QueryexternalaccountmappingsApiLatest = 1;

		public const int TransferdeviceidaccountApiLatest = 1;

		public const int DeletedeviceidApiLatest = 1;

		public const int CreatedeviceidDevicemodelMaxLength = 64;

		public const int CreatedeviceidApiLatest = 1;

		public const int UnlinkaccountApiLatest = 1;

		public const int LinkaccountApiLatest = 1;

		public const int CreateuserApiLatest = 1;

		public const int LoginApiLatest = 2;

		public const int UserlogininfoApiLatest = 1;

		public const int UserlogininfoDisplaynameMaxLength = 32;

		public const int CredentialsApiLatest = 1;

		public const int ExternalAccountIdMaxLength = 256;

		public ConnectInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void Login(LoginOptions options, object clientData, OnLoginCallback completionDelegate)
		{
			LoginOptionsInternal options2 = Helper.CopyProperties<LoginOptionsInternal>(options);
			OnLoginCallbackInternal onLoginCallbackInternal = OnLogin;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onLoginCallbackInternal);
			EOS_Connect_Login(base.InnerHandle, ref options2, clientDataAddress, onLoginCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void CreateUser(CreateUserOptions options, object clientData, OnCreateUserCallback completionDelegate)
		{
			CreateUserOptionsInternal options2 = Helper.CopyProperties<CreateUserOptionsInternal>(options);
			OnCreateUserCallbackInternal onCreateUserCallbackInternal = OnCreateUser;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onCreateUserCallbackInternal);
			EOS_Connect_CreateUser(base.InnerHandle, ref options2, clientDataAddress, onCreateUserCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void LinkAccount(LinkAccountOptions options, object clientData, OnLinkAccountCallback completionDelegate)
		{
			LinkAccountOptionsInternal options2 = Helper.CopyProperties<LinkAccountOptionsInternal>(options);
			OnLinkAccountCallbackInternal onLinkAccountCallbackInternal = OnLinkAccount;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onLinkAccountCallbackInternal);
			EOS_Connect_LinkAccount(base.InnerHandle, ref options2, clientDataAddress, onLinkAccountCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void UnlinkAccount(UnlinkAccountOptions options, object clientData, OnUnlinkAccountCallback completionDelegate)
		{
			UnlinkAccountOptionsInternal options2 = Helper.CopyProperties<UnlinkAccountOptionsInternal>(options);
			OnUnlinkAccountCallbackInternal onUnlinkAccountCallbackInternal = OnUnlinkAccount;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onUnlinkAccountCallbackInternal);
			EOS_Connect_UnlinkAccount(base.InnerHandle, ref options2, clientDataAddress, onUnlinkAccountCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void CreateDeviceId(CreateDeviceIdOptions options, object clientData, OnCreateDeviceIdCallback completionDelegate)
		{
			CreateDeviceIdOptionsInternal options2 = Helper.CopyProperties<CreateDeviceIdOptionsInternal>(options);
			OnCreateDeviceIdCallbackInternal onCreateDeviceIdCallbackInternal = OnCreateDeviceId;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onCreateDeviceIdCallbackInternal);
			EOS_Connect_CreateDeviceId(base.InnerHandle, ref options2, clientDataAddress, onCreateDeviceIdCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void DeleteDeviceId(DeleteDeviceIdOptions options, object clientData, OnDeleteDeviceIdCallback completionDelegate)
		{
			DeleteDeviceIdOptionsInternal options2 = Helper.CopyProperties<DeleteDeviceIdOptionsInternal>(options);
			OnDeleteDeviceIdCallbackInternal onDeleteDeviceIdCallbackInternal = OnDeleteDeviceId;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onDeleteDeviceIdCallbackInternal);
			EOS_Connect_DeleteDeviceId(base.InnerHandle, ref options2, clientDataAddress, onDeleteDeviceIdCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void TransferDeviceIdAccount(TransferDeviceIdAccountOptions options, object clientData, OnTransferDeviceIdAccountCallback completionDelegate)
		{
			TransferDeviceIdAccountOptionsInternal options2 = Helper.CopyProperties<TransferDeviceIdAccountOptionsInternal>(options);
			OnTransferDeviceIdAccountCallbackInternal onTransferDeviceIdAccountCallbackInternal = OnTransferDeviceIdAccount;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onTransferDeviceIdAccountCallbackInternal);
			EOS_Connect_TransferDeviceIdAccount(base.InnerHandle, ref options2, clientDataAddress, onTransferDeviceIdAccountCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryExternalAccountMappings(QueryExternalAccountMappingsOptions options, object clientData, OnQueryExternalAccountMappingsCallback completionDelegate)
		{
			QueryExternalAccountMappingsOptionsInternal options2 = Helper.CopyProperties<QueryExternalAccountMappingsOptionsInternal>(options);
			OnQueryExternalAccountMappingsCallbackInternal onQueryExternalAccountMappingsCallbackInternal = OnQueryExternalAccountMappings;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryExternalAccountMappingsCallbackInternal);
			EOS_Connect_QueryExternalAccountMappings(base.InnerHandle, ref options2, clientDataAddress, onQueryExternalAccountMappingsCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryProductUserIdMappings(QueryProductUserIdMappingsOptions options, object clientData, OnQueryProductUserIdMappingsCallback completionDelegate)
		{
			QueryProductUserIdMappingsOptionsInternal options2 = Helper.CopyProperties<QueryProductUserIdMappingsOptionsInternal>(options);
			OnQueryProductUserIdMappingsCallbackInternal onQueryProductUserIdMappingsCallbackInternal = OnQueryProductUserIdMappings;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryProductUserIdMappingsCallbackInternal);
			EOS_Connect_QueryProductUserIdMappings(base.InnerHandle, ref options2, clientDataAddress, onQueryProductUserIdMappingsCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public ProductUserId GetExternalAccountMapping(GetExternalAccountMappingsOptions options)
		{
			GetExternalAccountMappingsOptionsInternal options2 = Helper.CopyProperties<GetExternalAccountMappingsOptionsInternal>(options);
			IntPtr source = EOS_Connect_GetExternalAccountMapping(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			ProductUserId target = Helper.GetDefault<ProductUserId>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetProductUserIdMapping(GetProductUserIdMappingOptions options, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			GetProductUserIdMappingOptionsInternal options2 = Helper.CopyProperties<GetProductUserIdMappingOptionsInternal>(options);
			Result source = EOS_Connect_GetProductUserIdMapping(base.InnerHandle, ref options2, outBuffer, ref inOutBufferLength);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetProductUserExternalAccountCount(GetProductUserExternalAccountCountOptions options)
		{
			GetProductUserExternalAccountCountOptionsInternal options2 = Helper.CopyProperties<GetProductUserExternalAccountCountOptionsInternal>(options);
			uint source = EOS_Connect_GetProductUserExternalAccountCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyProductUserExternalAccountByIndex(CopyProductUserExternalAccountByIndexOptions options, out ExternalAccountInfo outExternalAccountInfo)
		{
			CopyProductUserExternalAccountByIndexOptionsInternal options2 = Helper.CopyProperties<CopyProductUserExternalAccountByIndexOptionsInternal>(options);
			outExternalAccountInfo = Helper.GetDefault<ExternalAccountInfo>();
			IntPtr outExternalAccountInfo2 = IntPtr.Zero;
			Result source = EOS_Connect_CopyProductUserExternalAccountByIndex(base.InnerHandle, ref options2, ref outExternalAccountInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<ExternalAccountInfoInternal, ExternalAccountInfo>(outExternalAccountInfo2, out outExternalAccountInfo))
			{
				EOS_Connect_ExternalAccountInfo_Release(outExternalAccountInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyProductUserExternalAccountByAccountType(CopyProductUserExternalAccountByAccountTypeOptions options, out ExternalAccountInfo outExternalAccountInfo)
		{
			CopyProductUserExternalAccountByAccountTypeOptionsInternal options2 = Helper.CopyProperties<CopyProductUserExternalAccountByAccountTypeOptionsInternal>(options);
			outExternalAccountInfo = Helper.GetDefault<ExternalAccountInfo>();
			IntPtr outExternalAccountInfo2 = IntPtr.Zero;
			Result source = EOS_Connect_CopyProductUserExternalAccountByAccountType(base.InnerHandle, ref options2, ref outExternalAccountInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<ExternalAccountInfoInternal, ExternalAccountInfo>(outExternalAccountInfo2, out outExternalAccountInfo))
			{
				EOS_Connect_ExternalAccountInfo_Release(outExternalAccountInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyProductUserExternalAccountByAccountId(CopyProductUserExternalAccountByAccountIdOptions options, out ExternalAccountInfo outExternalAccountInfo)
		{
			CopyProductUserExternalAccountByAccountIdOptionsInternal options2 = Helper.CopyProperties<CopyProductUserExternalAccountByAccountIdOptionsInternal>(options);
			outExternalAccountInfo = Helper.GetDefault<ExternalAccountInfo>();
			IntPtr outExternalAccountInfo2 = IntPtr.Zero;
			Result source = EOS_Connect_CopyProductUserExternalAccountByAccountId(base.InnerHandle, ref options2, ref outExternalAccountInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<ExternalAccountInfoInternal, ExternalAccountInfo>(outExternalAccountInfo2, out outExternalAccountInfo))
			{
				EOS_Connect_ExternalAccountInfo_Release(outExternalAccountInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyProductUserInfo(CopyProductUserInfoOptions options, out ExternalAccountInfo outExternalAccountInfo)
		{
			CopyProductUserInfoOptionsInternal options2 = Helper.CopyProperties<CopyProductUserInfoOptionsInternal>(options);
			outExternalAccountInfo = Helper.GetDefault<ExternalAccountInfo>();
			IntPtr outExternalAccountInfo2 = IntPtr.Zero;
			Result source = EOS_Connect_CopyProductUserInfo(base.InnerHandle, ref options2, ref outExternalAccountInfo2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<ExternalAccountInfoInternal, ExternalAccountInfo>(outExternalAccountInfo2, out outExternalAccountInfo))
			{
				EOS_Connect_ExternalAccountInfo_Release(outExternalAccountInfo2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public int GetLoggedInUsersCount()
		{
			int source = EOS_Connect_GetLoggedInUsersCount(base.InnerHandle);
			int target = Helper.GetDefault<int>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ProductUserId GetLoggedInUserByIndex(int index)
		{
			IntPtr source = EOS_Connect_GetLoggedInUserByIndex(base.InnerHandle, index);
			ProductUserId target = Helper.GetDefault<ProductUserId>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public LoginStatus GetLoginStatus(ProductUserId localUserId)
		{
			LoginStatus source = EOS_Connect_GetLoginStatus(base.InnerHandle, localUserId.InnerHandle);
			LoginStatus target = Helper.GetDefault<LoginStatus>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ulong AddNotifyAuthExpiration(AddNotifyAuthExpirationOptions options, object clientData, OnAuthExpirationCallback notification)
		{
			AddNotifyAuthExpirationOptionsInternal options2 = Helper.CopyProperties<AddNotifyAuthExpirationOptionsInternal>(options);
			OnAuthExpirationCallbackInternal onAuthExpirationCallbackInternal = OnAuthExpiration;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notification, onAuthExpirationCallbackInternal);
			ulong num = EOS_Connect_AddNotifyAuthExpiration(base.InnerHandle, ref options2, clientDataAddress, onAuthExpirationCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyAuthExpiration(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Connect_RemoveNotifyAuthExpiration(base.InnerHandle, inId);
		}

		public ulong AddNotifyLoginStatusChanged(AddNotifyLoginStatusChangedOptions options, object clientData, OnLoginStatusChangedCallback notification)
		{
			AddNotifyLoginStatusChangedOptionsInternal options2 = Helper.CopyProperties<AddNotifyLoginStatusChangedOptionsInternal>(options);
			OnLoginStatusChangedCallbackInternal onLoginStatusChangedCallbackInternal = OnLoginStatusChanged;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notification, onLoginStatusChangedCallbackInternal);
			ulong num = EOS_Connect_AddNotifyLoginStatusChanged(base.InnerHandle, ref options2, clientDataAddress, onLoginStatusChangedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyLoginStatusChanged(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Connect_RemoveNotifyLoginStatusChanged(base.InnerHandle, inId);
		}

		[MonoPInvokeCallback]
		internal static void OnLoginStatusChanged(IntPtr address)
		{
			OnLoginStatusChangedCallback callback = null;
			LoginStatusChangedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLoginStatusChangedCallback, LoginStatusChangedCallbackInfoInternal, LoginStatusChangedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnAuthExpiration(IntPtr address)
		{
			OnAuthExpirationCallback callback = null;
			AuthExpirationCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnAuthExpirationCallback, AuthExpirationCallbackInfoInternal, AuthExpirationCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryProductUserIdMappings(IntPtr address)
		{
			OnQueryProductUserIdMappingsCallback callback = null;
			QueryProductUserIdMappingsCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryProductUserIdMappingsCallback, QueryProductUserIdMappingsCallbackInfoInternal, QueryProductUserIdMappingsCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryExternalAccountMappings(IntPtr address)
		{
			OnQueryExternalAccountMappingsCallback callback = null;
			QueryExternalAccountMappingsCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryExternalAccountMappingsCallback, QueryExternalAccountMappingsCallbackInfoInternal, QueryExternalAccountMappingsCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnTransferDeviceIdAccount(IntPtr address)
		{
			OnTransferDeviceIdAccountCallback callback = null;
			TransferDeviceIdAccountCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnTransferDeviceIdAccountCallback, TransferDeviceIdAccountCallbackInfoInternal, TransferDeviceIdAccountCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDeleteDeviceId(IntPtr address)
		{
			OnDeleteDeviceIdCallback callback = null;
			DeleteDeviceIdCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDeleteDeviceIdCallback, DeleteDeviceIdCallbackInfoInternal, DeleteDeviceIdCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnCreateDeviceId(IntPtr address)
		{
			OnCreateDeviceIdCallback callback = null;
			CreateDeviceIdCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnCreateDeviceIdCallback, CreateDeviceIdCallbackInfoInternal, CreateDeviceIdCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnUnlinkAccount(IntPtr address)
		{
			OnUnlinkAccountCallback callback = null;
			UnlinkAccountCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnUnlinkAccountCallback, UnlinkAccountCallbackInfoInternal, UnlinkAccountCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLinkAccount(IntPtr address)
		{
			OnLinkAccountCallback callback = null;
			LinkAccountCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLinkAccountCallback, LinkAccountCallbackInfoInternal, LinkAccountCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnCreateUser(IntPtr address)
		{
			OnCreateUserCallback callback = null;
			CreateUserCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnCreateUserCallback, CreateUserCallbackInfoInternal, CreateUserCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLogin(IntPtr address)
		{
			OnLoginCallback callback = null;
			LoginCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLoginCallback, LoginCallbackInfoInternal, LoginCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_ExternalAccountInfo_Release(IntPtr externalAccountInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_RemoveNotifyLoginStatusChanged(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Connect_AddNotifyLoginStatusChanged(IntPtr handle, ref AddNotifyLoginStatusChangedOptionsInternal options, IntPtr clientData, OnLoginStatusChangedCallbackInternal notification);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_RemoveNotifyAuthExpiration(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Connect_AddNotifyAuthExpiration(IntPtr handle, ref AddNotifyAuthExpirationOptionsInternal options, IntPtr clientData, OnAuthExpirationCallbackInternal notification);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern LoginStatus EOS_Connect_GetLoginStatus(IntPtr handle, IntPtr localUserId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Connect_GetLoggedInUserByIndex(IntPtr handle, int index);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern int EOS_Connect_GetLoggedInUsersCount(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Connect_CopyProductUserInfo(IntPtr handle, ref CopyProductUserInfoOptionsInternal options, ref IntPtr outExternalAccountInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Connect_CopyProductUserExternalAccountByAccountId(IntPtr handle, ref CopyProductUserExternalAccountByAccountIdOptionsInternal options, ref IntPtr outExternalAccountInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Connect_CopyProductUserExternalAccountByAccountType(IntPtr handle, ref CopyProductUserExternalAccountByAccountTypeOptionsInternal options, ref IntPtr outExternalAccountInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Connect_CopyProductUserExternalAccountByIndex(IntPtr handle, ref CopyProductUserExternalAccountByIndexOptionsInternal options, ref IntPtr outExternalAccountInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Connect_GetProductUserExternalAccountCount(IntPtr handle, ref GetProductUserExternalAccountCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Connect_GetProductUserIdMapping(IntPtr handle, ref GetProductUserIdMappingOptionsInternal options, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Connect_GetExternalAccountMapping(IntPtr handle, ref GetExternalAccountMappingsOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_QueryProductUserIdMappings(IntPtr handle, ref QueryProductUserIdMappingsOptionsInternal options, IntPtr clientData, OnQueryProductUserIdMappingsCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_QueryExternalAccountMappings(IntPtr handle, ref QueryExternalAccountMappingsOptionsInternal options, IntPtr clientData, OnQueryExternalAccountMappingsCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_TransferDeviceIdAccount(IntPtr handle, ref TransferDeviceIdAccountOptionsInternal options, IntPtr clientData, OnTransferDeviceIdAccountCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_DeleteDeviceId(IntPtr handle, ref DeleteDeviceIdOptionsInternal options, IntPtr clientData, OnDeleteDeviceIdCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_CreateDeviceId(IntPtr handle, ref CreateDeviceIdOptionsInternal options, IntPtr clientData, OnCreateDeviceIdCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_UnlinkAccount(IntPtr handle, ref UnlinkAccountOptionsInternal options, IntPtr clientData, OnUnlinkAccountCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_LinkAccount(IntPtr handle, ref LinkAccountOptionsInternal options, IntPtr clientData, OnLinkAccountCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_CreateUser(IntPtr handle, ref CreateUserOptionsInternal options, IntPtr clientData, OnCreateUserCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Connect_Login(IntPtr handle, ref LoginOptionsInternal options, IntPtr clientData, OnLoginCallbackInternal completionDelegate);
	}
}
