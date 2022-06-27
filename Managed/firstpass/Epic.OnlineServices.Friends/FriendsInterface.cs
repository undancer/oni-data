using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Friends
{
	public sealed class FriendsInterface : Handle
	{
		public const int AddnotifyfriendsupdateApiLatest = 1;

		public const int GetstatusApiLatest = 1;

		public const int GetfriendatindexApiLatest = 1;

		public const int GetfriendscountApiLatest = 1;

		public const int DeletefriendApiLatest = 1;

		public const int RejectinviteApiLatest = 1;

		public const int AcceptinviteApiLatest = 1;

		public const int SendinviteApiLatest = 1;

		public const int QueryfriendsApiLatest = 1;

		public FriendsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryFriends(QueryFriendsOptions options, object clientData, OnQueryFriendsCallback completionDelegate)
		{
			QueryFriendsOptionsInternal options2 = Helper.CopyProperties<QueryFriendsOptionsInternal>(options);
			OnQueryFriendsCallbackInternal onQueryFriendsCallbackInternal = OnQueryFriends;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryFriendsCallbackInternal);
			EOS_Friends_QueryFriends(base.InnerHandle, ref options2, clientDataAddress, onQueryFriendsCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void SendInvite(SendInviteOptions options, object clientData, OnSendInviteCallback completionDelegate)
		{
			SendInviteOptionsInternal options2 = Helper.CopyProperties<SendInviteOptionsInternal>(options);
			OnSendInviteCallbackInternal onSendInviteCallbackInternal = OnSendInvite;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onSendInviteCallbackInternal);
			EOS_Friends_SendInvite(base.InnerHandle, ref options2, clientDataAddress, onSendInviteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void AcceptInvite(AcceptInviteOptions options, object clientData, OnAcceptInviteCallback completionDelegate)
		{
			AcceptInviteOptionsInternal options2 = Helper.CopyProperties<AcceptInviteOptionsInternal>(options);
			OnAcceptInviteCallbackInternal onAcceptInviteCallbackInternal = OnAcceptInvite;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onAcceptInviteCallbackInternal);
			EOS_Friends_AcceptInvite(base.InnerHandle, ref options2, clientDataAddress, onAcceptInviteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void RejectInvite(RejectInviteOptions options, object clientData, OnRejectInviteCallback completionDelegate)
		{
			RejectInviteOptionsInternal options2 = Helper.CopyProperties<RejectInviteOptionsInternal>(options);
			OnRejectInviteCallbackInternal onRejectInviteCallbackInternal = OnRejectInvite;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onRejectInviteCallbackInternal);
			EOS_Friends_RejectInvite(base.InnerHandle, ref options2, clientDataAddress, onRejectInviteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public int GetFriendsCount(GetFriendsCountOptions options)
		{
			GetFriendsCountOptionsInternal options2 = Helper.CopyProperties<GetFriendsCountOptionsInternal>(options);
			int source = EOS_Friends_GetFriendsCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			int target = Helper.GetDefault<int>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public EpicAccountId GetFriendAtIndex(GetFriendAtIndexOptions options)
		{
			GetFriendAtIndexOptionsInternal options2 = Helper.CopyProperties<GetFriendAtIndexOptionsInternal>(options);
			IntPtr source = EOS_Friends_GetFriendAtIndex(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			EpicAccountId target = Helper.GetDefault<EpicAccountId>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public FriendsStatus GetStatus(GetStatusOptions options)
		{
			GetStatusOptionsInternal options2 = Helper.CopyProperties<GetStatusOptionsInternal>(options);
			FriendsStatus source = EOS_Friends_GetStatus(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			FriendsStatus target = Helper.GetDefault<FriendsStatus>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ulong AddNotifyFriendsUpdate(AddNotifyFriendsUpdateOptions options, object clientData, OnFriendsUpdateCallback friendsUpdateHandler)
		{
			AddNotifyFriendsUpdateOptionsInternal options2 = Helper.CopyProperties<AddNotifyFriendsUpdateOptionsInternal>(options);
			OnFriendsUpdateCallbackInternal onFriendsUpdateCallbackInternal = OnFriendsUpdate;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, friendsUpdateHandler, onFriendsUpdateCallbackInternal);
			ulong num = EOS_Friends_AddNotifyFriendsUpdate(base.InnerHandle, ref options2, clientDataAddress, onFriendsUpdateCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyFriendsUpdate(ulong notificationId)
		{
			Helper.TryRemoveCallbackByNotificationId(notificationId);
			EOS_Friends_RemoveNotifyFriendsUpdate(base.InnerHandle, notificationId);
		}

		[MonoPInvokeCallback]
		internal static void OnFriendsUpdate(IntPtr address)
		{
			OnFriendsUpdateCallback callback = null;
			OnFriendsUpdateInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnFriendsUpdateCallback, OnFriendsUpdateInfoInternal, OnFriendsUpdateInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnRejectInvite(IntPtr address)
		{
			OnRejectInviteCallback callback = null;
			RejectInviteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnRejectInviteCallback, RejectInviteCallbackInfoInternal, RejectInviteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnAcceptInvite(IntPtr address)
		{
			OnAcceptInviteCallback callback = null;
			AcceptInviteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnAcceptInviteCallback, AcceptInviteCallbackInfoInternal, AcceptInviteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnSendInvite(IntPtr address)
		{
			OnSendInviteCallback callback = null;
			SendInviteCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnSendInviteCallback, SendInviteCallbackInfoInternal, SendInviteCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryFriends(IntPtr address)
		{
			OnQueryFriendsCallback callback = null;
			QueryFriendsCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryFriendsCallback, QueryFriendsCallbackInfoInternal, QueryFriendsCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Friends_RemoveNotifyFriendsUpdate(IntPtr handle, ulong notificationId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Friends_AddNotifyFriendsUpdate(IntPtr handle, ref AddNotifyFriendsUpdateOptionsInternal options, IntPtr clientData, OnFriendsUpdateCallbackInternal friendsUpdateHandler);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern FriendsStatus EOS_Friends_GetStatus(IntPtr handle, ref GetStatusOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Friends_GetFriendAtIndex(IntPtr handle, ref GetFriendAtIndexOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern int EOS_Friends_GetFriendsCount(IntPtr handle, ref GetFriendsCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Friends_RejectInvite(IntPtr handle, ref RejectInviteOptionsInternal options, IntPtr clientData, OnRejectInviteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Friends_AcceptInvite(IntPtr handle, ref AcceptInviteOptionsInternal options, IntPtr clientData, OnAcceptInviteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Friends_SendInvite(IntPtr handle, ref SendInviteOptionsInternal options, IntPtr clientData, OnSendInviteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Friends_QueryFriends(IntPtr handle, ref QueryFriendsOptionsInternal options, IntPtr clientData, OnQueryFriendsCallbackInternal completionDelegate);
	}
}
