using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.Presence
{
	public sealed class PresenceInterface : Handle
	{
		public const int DeletedataApiLatest = 1;

		public const int PresencemodificationDeletedataApiLatest = 1;

		public const int PresencemodificationDatarecordidApiLatest = 1;

		public const int SetdataApiLatest = 1;

		public const int PresencemodificationSetdataApiLatest = 1;

		public const int SetrawrichtextApiLatest = 1;

		public const int PresencemodificationSetrawrichtextApiLatest = 1;

		public const int SetstatusApiLatest = 1;

		public const int PresencemodificationSetstatusApiLatest = 1;

		public const int RichTextMaxValueLength = 255;

		public const int DataMaxValueLength = 255;

		public const int DataMaxKeyLength = 64;

		public const int DataMaxKeys = 32;

		public const int PresencemodificationSetjoininfoApiLatest = 1;

		public const int PresencemodificationJoininfoMaxLength = 255;

		public const int GetjoininfoApiLatest = 1;

		public const int AddnotifyjoingameacceptedApiLatest = 2;

		public const int AddnotifyonpresencechangedApiLatest = 1;

		public const int SetpresenceApiLatest = 1;

		public const int CreatepresencemodificationApiLatest = 1;

		public const int CopypresenceApiLatest = 2;

		public const int HaspresenceApiLatest = 1;

		public const int QuerypresenceApiLatest = 1;

		public const int InfoApiLatest = 2;

		public const int DatarecordApiLatest = 1;

		public PresenceInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryPresence(QueryPresenceOptions options, object clientData, OnQueryPresenceCompleteCallback completionDelegate)
		{
			QueryPresenceOptionsInternal options2 = Helper.CopyProperties<QueryPresenceOptionsInternal>(options);
			OnQueryPresenceCompleteCallbackInternal onQueryPresenceCompleteCallbackInternal = OnQueryPresenceComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryPresenceCompleteCallbackInternal);
			EOS_Presence_QueryPresence(base.InnerHandle, ref options2, clientDataAddress, onQueryPresenceCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public bool HasPresence(HasPresenceOptions options)
		{
			HasPresenceOptionsInternal options2 = Helper.CopyProperties<HasPresenceOptionsInternal>(options);
			int source = EOS_Presence_HasPresence(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			bool target = Helper.GetDefault<bool>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyPresence(CopyPresenceOptions options, out Info outPresence)
		{
			CopyPresenceOptionsInternal options2 = Helper.CopyProperties<CopyPresenceOptionsInternal>(options);
			outPresence = Helper.GetDefault<Info>();
			IntPtr outPresence2 = IntPtr.Zero;
			Result source = EOS_Presence_CopyPresence(base.InnerHandle, ref options2, ref outPresence2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<InfoInternal, Info>(outPresence2, out outPresence))
			{
				EOS_Presence_Info_Release(outPresence2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CreatePresenceModification(CreatePresenceModificationOptions options, out PresenceModification outPresenceModificationHandle)
		{
			CreatePresenceModificationOptionsInternal options2 = Helper.CopyProperties<CreatePresenceModificationOptionsInternal>(options);
			outPresenceModificationHandle = Helper.GetDefault<PresenceModification>();
			IntPtr outPresenceModificationHandle2 = IntPtr.Zero;
			Result source = EOS_Presence_CreatePresenceModification(base.InnerHandle, ref options2, ref outPresenceModificationHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outPresenceModificationHandle2, out outPresenceModificationHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void SetPresence(SetPresenceOptions options, object clientData, SetPresenceCompleteCallback completionDelegate)
		{
			SetPresenceOptionsInternal options2 = Helper.CopyProperties<SetPresenceOptionsInternal>(options);
			SetPresenceCompleteCallbackInternal setPresenceCompleteCallbackInternal = SetPresenceComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, setPresenceCompleteCallbackInternal);
			EOS_Presence_SetPresence(base.InnerHandle, ref options2, clientDataAddress, setPresenceCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public ulong AddNotifyOnPresenceChanged(AddNotifyOnPresenceChangedOptions options, object clientData, OnPresenceChangedCallback notificationHandler)
		{
			AddNotifyOnPresenceChangedOptionsInternal options2 = Helper.CopyProperties<AddNotifyOnPresenceChangedOptionsInternal>(options);
			OnPresenceChangedCallbackInternal onPresenceChangedCallbackInternal = OnPresenceChanged;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationHandler, onPresenceChangedCallbackInternal);
			ulong num = EOS_Presence_AddNotifyOnPresenceChanged(base.InnerHandle, ref options2, clientDataAddress, onPresenceChangedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyOnPresenceChanged(ulong notificationId)
		{
			Helper.TryRemoveCallbackByNotificationId(notificationId);
			EOS_Presence_RemoveNotifyOnPresenceChanged(base.InnerHandle, notificationId);
		}

		public ulong AddNotifyJoinGameAccepted(AddNotifyJoinGameAcceptedOptions options, object clientData, OnJoinGameAcceptedCallback notificationFn)
		{
			AddNotifyJoinGameAcceptedOptionsInternal options2 = Helper.CopyProperties<AddNotifyJoinGameAcceptedOptionsInternal>(options);
			OnJoinGameAcceptedCallbackInternal onJoinGameAcceptedCallbackInternal = OnJoinGameAccepted;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onJoinGameAcceptedCallbackInternal);
			ulong num = EOS_Presence_AddNotifyJoinGameAccepted(base.InnerHandle, ref options2, clientDataAddress, onJoinGameAcceptedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyJoinGameAccepted(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Presence_RemoveNotifyJoinGameAccepted(base.InnerHandle, inId);
		}

		public Result GetJoinInfo(GetJoinInfoOptions options, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			GetJoinInfoOptionsInternal options2 = Helper.CopyProperties<GetJoinInfoOptionsInternal>(options);
			Result source = EOS_Presence_GetJoinInfo(base.InnerHandle, ref options2, outBuffer, ref inOutBufferLength);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void OnJoinGameAccepted(IntPtr address)
		{
			OnJoinGameAcceptedCallback callback = null;
			JoinGameAcceptedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnJoinGameAcceptedCallback, JoinGameAcceptedCallbackInfoInternal, JoinGameAcceptedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnPresenceChanged(IntPtr address)
		{
			OnPresenceChangedCallback callback = null;
			PresenceChangedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnPresenceChangedCallback, PresenceChangedCallbackInfoInternal, PresenceChangedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void SetPresenceComplete(IntPtr address)
		{
			SetPresenceCompleteCallback callback = null;
			SetPresenceCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<SetPresenceCompleteCallback, SetPresenceCallbackInfoInternal, SetPresenceCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryPresenceComplete(IntPtr address)
		{
			OnQueryPresenceCompleteCallback callback = null;
			QueryPresenceCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryPresenceCompleteCallback, QueryPresenceCallbackInfoInternal, QueryPresenceCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Presence_Info_Release(IntPtr presenceInfo);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Presence_GetJoinInfo(IntPtr handle, ref GetJoinInfoOptionsInternal options, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Presence_RemoveNotifyJoinGameAccepted(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Presence_AddNotifyJoinGameAccepted(IntPtr handle, ref AddNotifyJoinGameAcceptedOptionsInternal options, IntPtr clientData, OnJoinGameAcceptedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Presence_RemoveNotifyOnPresenceChanged(IntPtr handle, ulong notificationId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Presence_AddNotifyOnPresenceChanged(IntPtr handle, ref AddNotifyOnPresenceChangedOptionsInternal options, IntPtr clientData, OnPresenceChangedCallbackInternal notificationHandler);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Presence_SetPresence(IntPtr handle, ref SetPresenceOptionsInternal options, IntPtr clientData, SetPresenceCompleteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Presence_CreatePresenceModification(IntPtr handle, ref CreatePresenceModificationOptionsInternal options, ref IntPtr outPresenceModificationHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Presence_CopyPresence(IntPtr handle, ref CopyPresenceOptionsInternal options, ref IntPtr outPresence);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern int EOS_Presence_HasPresence(IntPtr handle, ref HasPresenceOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Presence_QueryPresence(IntPtr handle, ref QueryPresenceOptionsInternal options, IntPtr clientData, OnQueryPresenceCompleteCallbackInternal completionDelegate);
	}
}
