using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UI
{
	public sealed class UIInterface : Handle
	{
		public const int AcknowledgecorrelationidApiLatest = 1;

		public const int AcknowledgeeventidApiLatest = 1;

		public const int SetdisplaypreferenceApiLatest = 1;

		public const int GettogglefriendskeyApiLatest = 1;

		public const int SettogglefriendskeyApiLatest = 1;

		public const int AddnotifydisplaysettingsupdatedApiLatest = 1;

		public const int GetfriendsvisibleApiLatest = 1;

		public const int HidefriendsApiLatest = 1;

		public const int ShowfriendsApiLatest = 1;

		public const int EventidInvalid = 0;

		public UIInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void ShowFriends(ShowFriendsOptions options, object clientData, OnShowFriendsCallback completionDelegate)
		{
			ShowFriendsOptionsInternal options2 = Helper.CopyProperties<ShowFriendsOptionsInternal>(options);
			OnShowFriendsCallbackInternal onShowFriendsCallbackInternal = OnShowFriends;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onShowFriendsCallbackInternal);
			EOS_UI_ShowFriends(base.InnerHandle, ref options2, clientDataAddress, onShowFriendsCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void HideFriends(HideFriendsOptions options, object clientData, OnHideFriendsCallback completionDelegate)
		{
			HideFriendsOptionsInternal options2 = Helper.CopyProperties<HideFriendsOptionsInternal>(options);
			OnHideFriendsCallbackInternal onHideFriendsCallbackInternal = OnHideFriends;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onHideFriendsCallbackInternal);
			EOS_UI_HideFriends(base.InnerHandle, ref options2, clientDataAddress, onHideFriendsCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public bool GetFriendsVisible(GetFriendsVisibleOptions options)
		{
			GetFriendsVisibleOptionsInternal options2 = Helper.CopyProperties<GetFriendsVisibleOptionsInternal>(options);
			int source = EOS_UI_GetFriendsVisible(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			bool target = Helper.GetDefault<bool>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ulong AddNotifyDisplaySettingsUpdated(AddNotifyDisplaySettingsUpdatedOptions options, object clientData, OnDisplaySettingsUpdatedCallback notificationFn)
		{
			AddNotifyDisplaySettingsUpdatedOptionsInternal options2 = Helper.CopyProperties<AddNotifyDisplaySettingsUpdatedOptionsInternal>(options);
			OnDisplaySettingsUpdatedCallbackInternal onDisplaySettingsUpdatedCallbackInternal = OnDisplaySettingsUpdated;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onDisplaySettingsUpdatedCallbackInternal);
			ulong num = EOS_UI_AddNotifyDisplaySettingsUpdated(base.InnerHandle, ref options2, clientDataAddress, onDisplaySettingsUpdatedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyDisplaySettingsUpdated(ulong id)
		{
			Helper.TryRemoveCallbackByNotificationId(id);
			EOS_UI_RemoveNotifyDisplaySettingsUpdated(base.InnerHandle, id);
		}

		public Result SetToggleFriendsKey(SetToggleFriendsKeyOptions options)
		{
			SetToggleFriendsKeyOptionsInternal options2 = Helper.CopyProperties<SetToggleFriendsKeyOptionsInternal>(options);
			Result source = EOS_UI_SetToggleFriendsKey(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public KeyCombination GetToggleFriendsKey(GetToggleFriendsKeyOptions options)
		{
			GetToggleFriendsKeyOptionsInternal options2 = Helper.CopyProperties<GetToggleFriendsKeyOptionsInternal>(options);
			KeyCombination source = EOS_UI_GetToggleFriendsKey(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			KeyCombination target = Helper.GetDefault<KeyCombination>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public bool IsValidKeyCombination(KeyCombination keyCombination)
		{
			int source = EOS_UI_IsValidKeyCombination(base.InnerHandle, keyCombination);
			bool target = Helper.GetDefault<bool>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetDisplayPreference(SetDisplayPreferenceOptions options)
		{
			SetDisplayPreferenceOptionsInternal options2 = Helper.CopyProperties<SetDisplayPreferenceOptionsInternal>(options);
			Result source = EOS_UI_SetDisplayPreference(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public NotificationLocation GetNotificationLocationPreference()
		{
			NotificationLocation source = EOS_UI_GetNotificationLocationPreference(base.InnerHandle);
			NotificationLocation target = Helper.GetDefault<NotificationLocation>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result AcknowledgeEventId(AcknowledgeEventIdOptions options)
		{
			AcknowledgeEventIdOptionsInternal options2 = Helper.CopyProperties<AcknowledgeEventIdOptionsInternal>(options);
			Result source = EOS_UI_AcknowledgeEventId(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void OnDisplaySettingsUpdated(IntPtr address)
		{
			OnDisplaySettingsUpdatedCallback callback = null;
			OnDisplaySettingsUpdatedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDisplaySettingsUpdatedCallback, OnDisplaySettingsUpdatedCallbackInfoInternal, OnDisplaySettingsUpdatedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnHideFriends(IntPtr address)
		{
			OnHideFriendsCallback callback = null;
			HideFriendsCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnHideFriendsCallback, HideFriendsCallbackInfoInternal, HideFriendsCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnShowFriends(IntPtr address)
		{
			OnShowFriendsCallback callback = null;
			ShowFriendsCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnShowFriendsCallback, ShowFriendsCallbackInfoInternal, ShowFriendsCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_UI_AcknowledgeEventId(IntPtr handle, ref AcknowledgeEventIdOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern NotificationLocation EOS_UI_GetNotificationLocationPreference(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_UI_SetDisplayPreference(IntPtr handle, ref SetDisplayPreferenceOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern int EOS_UI_IsValidKeyCombination(IntPtr handle, KeyCombination keyCombination);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern KeyCombination EOS_UI_GetToggleFriendsKey(IntPtr handle, ref GetToggleFriendsKeyOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_UI_SetToggleFriendsKey(IntPtr handle, ref SetToggleFriendsKeyOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_UI_RemoveNotifyDisplaySettingsUpdated(IntPtr handle, ulong id);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_UI_AddNotifyDisplaySettingsUpdated(IntPtr handle, ref AddNotifyDisplaySettingsUpdatedOptionsInternal options, IntPtr clientData, OnDisplaySettingsUpdatedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern int EOS_UI_GetFriendsVisible(IntPtr handle, ref GetFriendsVisibleOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_UI_HideFriends(IntPtr handle, ref HideFriendsOptionsInternal options, IntPtr clientData, OnHideFriendsCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_UI_ShowFriends(IntPtr handle, ref ShowFriendsOptionsInternal options, IntPtr clientData, OnShowFriendsCallbackInternal completionDelegate);
	}
}
