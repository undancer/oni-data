using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	public sealed class AuthInterface : Handle
	{
		public const int DeletepersistentauthApiLatest = 2;

		public const int AddnotifyloginstatuschangedApiLatest = 1;

		public const int CopyuserauthtokenApiLatest = 1;

		public const int VerifyuserauthApiLatest = 1;

		public const int LinkaccountApiLatest = 1;

		public const int LogoutApiLatest = 1;

		public const int LoginApiLatest = 2;

		public const int AccountfeaturerestrictedinfoApiLatest = 1;

		public const int PingrantinfoApiLatest = 2;

		public const int CredentialsApiLatest = 3;

		public const int TokenApiLatest = 2;

		public AuthInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void Login(LoginOptions options, object clientData, OnLoginCallback completionDelegate)
		{
			LoginOptionsInternal options2 = Helper.CopyProperties<LoginOptionsInternal>(options);
			OnLoginCallbackInternal onLoginCallbackInternal = OnLogin;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onLoginCallbackInternal);
			EOS_Auth_Login(base.InnerHandle, ref options2, clientDataAddress, onLoginCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void Logout(LogoutOptions options, object clientData, OnLogoutCallback completionDelegate)
		{
			LogoutOptionsInternal options2 = Helper.CopyProperties<LogoutOptionsInternal>(options);
			OnLogoutCallbackInternal onLogoutCallbackInternal = OnLogout;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onLogoutCallbackInternal);
			EOS_Auth_Logout(base.InnerHandle, ref options2, clientDataAddress, onLogoutCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void LinkAccount(LinkAccountOptions options, object clientData, OnLinkAccountCallback completionDelegate)
		{
			LinkAccountOptionsInternal options2 = Helper.CopyProperties<LinkAccountOptionsInternal>(options);
			OnLinkAccountCallbackInternal onLinkAccountCallbackInternal = OnLinkAccount;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onLinkAccountCallbackInternal);
			EOS_Auth_LinkAccount(base.InnerHandle, ref options2, clientDataAddress, onLinkAccountCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void DeletePersistentAuth(DeletePersistentAuthOptions options, object clientData, OnDeletePersistentAuthCallback completionDelegate)
		{
			DeletePersistentAuthOptionsInternal options2 = Helper.CopyProperties<DeletePersistentAuthOptionsInternal>(options);
			OnDeletePersistentAuthCallbackInternal onDeletePersistentAuthCallbackInternal = OnDeletePersistentAuth;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onDeletePersistentAuthCallbackInternal);
			EOS_Auth_DeletePersistentAuth(base.InnerHandle, ref options2, clientDataAddress, onDeletePersistentAuthCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void VerifyUserAuth(VerifyUserAuthOptions options, object clientData, OnVerifyUserAuthCallback completionDelegate)
		{
			VerifyUserAuthOptionsInternal options2 = Helper.CopyProperties<VerifyUserAuthOptionsInternal>(options);
			OnVerifyUserAuthCallbackInternal onVerifyUserAuthCallbackInternal = OnVerifyUserAuth;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onVerifyUserAuthCallbackInternal);
			EOS_Auth_VerifyUserAuth(base.InnerHandle, ref options2, clientDataAddress, onVerifyUserAuthCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public int GetLoggedInAccountsCount()
		{
			int source = EOS_Auth_GetLoggedInAccountsCount(base.InnerHandle);
			int target = Helper.GetDefault<int>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public EpicAccountId GetLoggedInAccountByIndex(int index)
		{
			IntPtr source = EOS_Auth_GetLoggedInAccountByIndex(base.InnerHandle, index);
			EpicAccountId target = Helper.GetDefault<EpicAccountId>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public LoginStatus GetLoginStatus(EpicAccountId localUserId)
		{
			LoginStatus source = EOS_Auth_GetLoginStatus(base.InnerHandle, localUserId.InnerHandle);
			LoginStatus target = Helper.GetDefault<LoginStatus>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyUserAuthToken(CopyUserAuthTokenOptions options, EpicAccountId localUserId, out Token outUserAuthToken)
		{
			CopyUserAuthTokenOptionsInternal options2 = Helper.CopyProperties<CopyUserAuthTokenOptionsInternal>(options);
			outUserAuthToken = Helper.GetDefault<Token>();
			IntPtr outUserAuthToken2 = IntPtr.Zero;
			Result source = EOS_Auth_CopyUserAuthToken(base.InnerHandle, ref options2, localUserId.InnerHandle, ref outUserAuthToken2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<TokenInternal, Token>(outUserAuthToken2, out outUserAuthToken))
			{
				EOS_Auth_Token_Release(outUserAuthToken2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ulong AddNotifyLoginStatusChanged(AddNotifyLoginStatusChangedOptions options, object clientData, OnLoginStatusChangedCallback notification)
		{
			AddNotifyLoginStatusChangedOptionsInternal options2 = Helper.CopyProperties<AddNotifyLoginStatusChangedOptionsInternal>(options);
			OnLoginStatusChangedCallbackInternal onLoginStatusChangedCallbackInternal = OnLoginStatusChanged;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notification, onLoginStatusChangedCallbackInternal);
			ulong num = EOS_Auth_AddNotifyLoginStatusChanged(base.InnerHandle, ref options2, clientDataAddress, onLoginStatusChangedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyLoginStatusChanged(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Auth_RemoveNotifyLoginStatusChanged(base.InnerHandle, inId);
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
		internal static void OnVerifyUserAuth(IntPtr address)
		{
			OnVerifyUserAuthCallback callback = null;
			VerifyUserAuthCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnVerifyUserAuthCallback, VerifyUserAuthCallbackInfoInternal, VerifyUserAuthCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDeletePersistentAuth(IntPtr address)
		{
			OnDeletePersistentAuthCallback callback = null;
			DeletePersistentAuthCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDeletePersistentAuthCallback, DeletePersistentAuthCallbackInfoInternal, DeletePersistentAuthCallbackInfo>(address, out callback, out callbackInfo))
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
		internal static void OnLogout(IntPtr address)
		{
			OnLogoutCallback callback = null;
			LogoutCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLogoutCallback, LogoutCallbackInfoInternal, LogoutCallbackInfo>(address, out callback, out callbackInfo))
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
		private static extern void EOS_Auth_Token_Release(IntPtr authToken);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Auth_RemoveNotifyLoginStatusChanged(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Auth_AddNotifyLoginStatusChanged(IntPtr handle, ref AddNotifyLoginStatusChangedOptionsInternal options, IntPtr clientData, OnLoginStatusChangedCallbackInternal notification);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Auth_CopyUserAuthToken(IntPtr handle, ref CopyUserAuthTokenOptionsInternal options, IntPtr localUserId, ref IntPtr outUserAuthToken);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern LoginStatus EOS_Auth_GetLoginStatus(IntPtr handle, IntPtr localUserId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Auth_GetLoggedInAccountByIndex(IntPtr handle, int index);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern int EOS_Auth_GetLoggedInAccountsCount(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Auth_VerifyUserAuth(IntPtr handle, ref VerifyUserAuthOptionsInternal options, IntPtr clientData, OnVerifyUserAuthCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Auth_DeletePersistentAuth(IntPtr handle, ref DeletePersistentAuthOptionsInternal options, IntPtr clientData, OnDeletePersistentAuthCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Auth_LinkAccount(IntPtr handle, ref LinkAccountOptionsInternal options, IntPtr clientData, OnLinkAccountCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Auth_Logout(IntPtr handle, ref LogoutOptionsInternal options, IntPtr clientData, OnLogoutCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Auth_Login(IntPtr handle, ref LoginOptionsInternal options, IntPtr clientData, OnLoginCallbackInternal completionDelegate);
	}
}
