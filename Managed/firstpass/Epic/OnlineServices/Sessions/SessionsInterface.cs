using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.Sessions
{
	public sealed class SessionsInterface : Handle
	{
		public const int DumpsessionstateApiLatest = 1;

		public const int IsuserinsessionApiLatest = 1;

		public const int CopysessionhandleforpresenceApiLatest = 1;

		public const int CopysessionhandlebyuieventidApiLatest = 1;

		public const int CopysessionhandlebyinviteidApiLatest = 1;

		public const int AddnotifyjoinsessionacceptedApiLatest = 1;

		public const int AddnotifysessioninviteacceptedApiLatest = 1;

		public const int AddnotifysessioninvitereceivedApiLatest = 1;

		public const int CopyactivesessionhandleApiLatest = 1;

		public const int ActivesessionInfoApiLatest = 1;

		public const int SessiondetailsCopysessionattributebykeyApiLatest = 1;

		public const int SessiondetailsCopysessionattributebyindexApiLatest = 1;

		public const int SessiondetailsGetsessionattributecountApiLatest = 1;

		public const int SessiondetailsCopyinfoApiLatest = 1;

		public const int SessiondetailsInfoApiLatest = 1;

		public const int SessiondetailsSettingsApiLatest = 2;

		public const int SessionsearchRemoveparameterApiLatest = 1;

		public const int SessionsearchSetparameterApiLatest = 1;

		public const int SessionsearchSettargetuseridApiLatest = 1;

		public const int SessionsearchSetsessionidApiLatest = 1;

		public const int SessionsearchCopysearchresultbyindexApiLatest = 1;

		public const int SessionsearchGetsearchresultcountApiLatest = 1;

		public const int SessionsearchFindApiLatest = 2;

		public const int SessionsearchSetmaxsearchresultsApiLatest = 1;

		public const int MaxSearchResults = 200;

		public const int SessionmodificationRemoveattributeApiLatest = 1;

		public const int SessionmodificationAddattributeApiLatest = 1;

		public const int SessionattributeApiLatest = 1;

		public const int SessiondetailsAttributeApiLatest = 1;

		public const int ActivesessionGetregisteredplayerbyindexApiLatest = 1;

		public const int ActivesessionGetregisteredplayercountApiLatest = 1;

		public const int ActivesessionCopyinfoApiLatest = 1;

		public const int SessionattributedataApiLatest = 1;

		public const int AttributedataApiLatest = 1;

		public const string SearchMinslotsavailable = "minslotsavailable";

		public const string SearchNonemptyServersOnly = "nonemptyonly";

		public const string SearchEmptyServersOnly = "emptyonly";

		public const string SearchBucketId = "bucket";

		public const int SessionmodificationSetinvitesallowedApiLatest = 1;

		public const int SessionmodificationSetmaxplayersApiLatest = 1;

		public const int Maxregisteredplayers = 1000;

		public const int SessionmodificationSetjoininprogressallowedApiLatest = 1;

		public const int SessionmodificationSetpermissionlevelApiLatest = 1;

		public const int SessionmodificationSethostaddressApiLatest = 1;

		public const int SessionmodificationSetbucketidApiLatest = 1;

		public const int UnregisterplayersApiLatest = 1;

		public const int RegisterplayersApiLatest = 1;

		public const int EndsessionApiLatest = 1;

		public const int StartsessionApiLatest = 1;

		public const int JoinsessionApiLatest = 2;

		public const int DestroysessionApiLatest = 1;

		public const int UpdatesessionApiLatest = 1;

		public const int CreatesessionsearchApiLatest = 1;

		public const int GetinviteidbyindexApiLatest = 1;

		public const int GetinvitecountApiLatest = 1;

		public const int QueryinvitesApiLatest = 1;

		public const int RejectinviteApiLatest = 1;

		public const int SendinviteApiLatest = 1;

		public const int InviteidMaxLength = 64;

		public const int UpdatesessionmodificationApiLatest = 1;

		public const int CreatesessionmodificationApiLatest = 3;

		public const int SessionmodificationMaxSessionidoverrideLength = 64;

		public const int SessionmodificationMinSessionidoverrideLength = 16;

		public const int SessionmodificationMaxSessionAttributeLength = 64;

		public const int SessionmodificationMaxSessionAttributes = 64;

		public SessionsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result CreateSessionModification(CreateSessionModificationOptions options, out SessionModification outSessionModificationHandle)
		{
			CreateSessionModificationOptionsInternal options2 = Helper.CopyProperties<CreateSessionModificationOptionsInternal>(options);
			outSessionModificationHandle = Helper.GetDefault<SessionModification>();
			IntPtr outSessionModificationHandle2 = IntPtr.Zero;
			Result source = EOS_Sessions_CreateSessionModification(base.InnerHandle, ref options2, ref outSessionModificationHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outSessionModificationHandle2, out outSessionModificationHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result UpdateSessionModification(UpdateSessionModificationOptions options, out SessionModification outSessionModificationHandle)
		{
			UpdateSessionModificationOptionsInternal options2 = Helper.CopyProperties<UpdateSessionModificationOptionsInternal>(options);
			outSessionModificationHandle = Helper.GetDefault<SessionModification>();
			IntPtr outSessionModificationHandle2 = IntPtr.Zero;
			Result source = EOS_Sessions_UpdateSessionModification(base.InnerHandle, ref options2, ref outSessionModificationHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outSessionModificationHandle2, out outSessionModificationHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void UpdateSession(UpdateSessionOptions options, object clientData, OnUpdateSessionCallback completionDelegate)
		{
			UpdateSessionOptionsInternal options2 = Helper.CopyProperties<UpdateSessionOptionsInternal>(options);
			OnUpdateSessionCallbackInternal onUpdateSessionCallbackInternal = OnUpdateSession;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onUpdateSessionCallbackInternal);
			EOS_Sessions_UpdateSession(base.InnerHandle, ref options2, clientDataAddress, onUpdateSessionCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void DestroySession(DestroySessionOptions options, object clientData, OnDestroySessionCallback completionDelegate)
		{
			DestroySessionOptionsInternal options2 = Helper.CopyProperties<DestroySessionOptionsInternal>(options);
			OnDestroySessionCallbackInternal onDestroySessionCallbackInternal = OnDestroySession;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onDestroySessionCallbackInternal);
			EOS_Sessions_DestroySession(base.InnerHandle, ref options2, clientDataAddress, onDestroySessionCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void JoinSession(JoinSessionOptions options, object clientData, OnJoinSessionCallback completionDelegate)
		{
			JoinSessionOptionsInternal options2 = Helper.CopyProperties<JoinSessionOptionsInternal>(options);
			OnJoinSessionCallbackInternal onJoinSessionCallbackInternal = OnJoinSession;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onJoinSessionCallbackInternal);
			EOS_Sessions_JoinSession(base.InnerHandle, ref options2, clientDataAddress, onJoinSessionCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void StartSession(StartSessionOptions options, object clientData, OnStartSessionCallback completionDelegate)
		{
			StartSessionOptionsInternal options2 = Helper.CopyProperties<StartSessionOptionsInternal>(options);
			OnStartSessionCallbackInternal onStartSessionCallbackInternal = OnStartSession;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onStartSessionCallbackInternal);
			EOS_Sessions_StartSession(base.InnerHandle, ref options2, clientDataAddress, onStartSessionCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void EndSession(EndSessionOptions options, object clientData, OnEndSessionCallback completionDelegate)
		{
			EndSessionOptionsInternal options2 = Helper.CopyProperties<EndSessionOptionsInternal>(options);
			OnEndSessionCallbackInternal onEndSessionCallbackInternal = OnEndSession;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onEndSessionCallbackInternal);
			EOS_Sessions_EndSession(base.InnerHandle, ref options2, clientDataAddress, onEndSessionCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void RegisterPlayers(RegisterPlayersOptions options, object clientData, OnRegisterPlayersCallback completionDelegate)
		{
			RegisterPlayersOptionsInternal options2 = Helper.CopyProperties<RegisterPlayersOptionsInternal>(options);
			OnRegisterPlayersCallbackInternal onRegisterPlayersCallbackInternal = OnRegisterPlayers;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onRegisterPlayersCallbackInternal);
			EOS_Sessions_RegisterPlayers(base.InnerHandle, ref options2, clientDataAddress, onRegisterPlayersCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void UnregisterPlayers(UnregisterPlayersOptions options, object clientData, OnUnregisterPlayersCallback completionDelegate)
		{
			UnregisterPlayersOptionsInternal options2 = Helper.CopyProperties<UnregisterPlayersOptionsInternal>(options);
			OnUnregisterPlayersCallbackInternal onUnregisterPlayersCallbackInternal = OnUnregisterPlayers;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onUnregisterPlayersCallbackInternal);
			EOS_Sessions_UnregisterPlayers(base.InnerHandle, ref options2, clientDataAddress, onUnregisterPlayersCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void SendInvite(SendInviteOptions options, object clientData, OnSendInviteCallback completionDelegate)
		{
			SendInviteOptionsInternal options2 = Helper.CopyProperties<SendInviteOptionsInternal>(options);
			OnSendInviteCallbackInternal onSendInviteCallbackInternal = OnSendInvite;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onSendInviteCallbackInternal);
			EOS_Sessions_SendInvite(base.InnerHandle, ref options2, clientDataAddress, onSendInviteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void RejectInvite(RejectInviteOptions options, object clientData, OnRejectInviteCallback completionDelegate)
		{
			RejectInviteOptionsInternal options2 = Helper.CopyProperties<RejectInviteOptionsInternal>(options);
			OnRejectInviteCallbackInternal onRejectInviteCallbackInternal = OnRejectInvite;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onRejectInviteCallbackInternal);
			EOS_Sessions_RejectInvite(base.InnerHandle, ref options2, clientDataAddress, onRejectInviteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryInvites(QueryInvitesOptions options, object clientData, OnQueryInvitesCallback completionDelegate)
		{
			QueryInvitesOptionsInternal options2 = Helper.CopyProperties<QueryInvitesOptionsInternal>(options);
			OnQueryInvitesCallbackInternal onQueryInvitesCallbackInternal = OnQueryInvites;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryInvitesCallbackInternal);
			EOS_Sessions_QueryInvites(base.InnerHandle, ref options2, clientDataAddress, onQueryInvitesCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public uint GetInviteCount(GetInviteCountOptions options)
		{
			GetInviteCountOptionsInternal options2 = Helper.CopyProperties<GetInviteCountOptionsInternal>(options);
			uint source = EOS_Sessions_GetInviteCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetInviteIdByIndex(GetInviteIdByIndexOptions options, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			GetInviteIdByIndexOptionsInternal options2 = Helper.CopyProperties<GetInviteIdByIndexOptionsInternal>(options);
			Result source = EOS_Sessions_GetInviteIdByIndex(base.InnerHandle, ref options2, outBuffer, ref inOutBufferLength);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CreateSessionSearch(CreateSessionSearchOptions options, out SessionSearch outSessionSearchHandle)
		{
			CreateSessionSearchOptionsInternal options2 = Helper.CopyProperties<CreateSessionSearchOptionsInternal>(options);
			outSessionSearchHandle = Helper.GetDefault<SessionSearch>();
			IntPtr outSessionSearchHandle2 = IntPtr.Zero;
			Result source = EOS_Sessions_CreateSessionSearch(base.InnerHandle, ref options2, ref outSessionSearchHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outSessionSearchHandle2, out outSessionSearchHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyActiveSessionHandle(CopyActiveSessionHandleOptions options, out ActiveSession outSessionHandle)
		{
			CopyActiveSessionHandleOptionsInternal options2 = Helper.CopyProperties<CopyActiveSessionHandleOptionsInternal>(options);
			outSessionHandle = Helper.GetDefault<ActiveSession>();
			IntPtr outSessionHandle2 = IntPtr.Zero;
			Result source = EOS_Sessions_CopyActiveSessionHandle(base.InnerHandle, ref options2, ref outSessionHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outSessionHandle2, out outSessionHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ulong AddNotifySessionInviteReceived(AddNotifySessionInviteReceivedOptions options, object clientData, OnSessionInviteReceivedCallback notificationFn)
		{
			AddNotifySessionInviteReceivedOptionsInternal options2 = Helper.CopyProperties<AddNotifySessionInviteReceivedOptionsInternal>(options);
			OnSessionInviteReceivedCallbackInternal onSessionInviteReceivedCallbackInternal = OnSessionInviteReceived;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onSessionInviteReceivedCallbackInternal);
			ulong num = EOS_Sessions_AddNotifySessionInviteReceived(base.InnerHandle, ref options2, clientDataAddress, onSessionInviteReceivedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifySessionInviteReceived(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Sessions_RemoveNotifySessionInviteReceived(base.InnerHandle, inId);
		}

		public ulong AddNotifySessionInviteAccepted(AddNotifySessionInviteAcceptedOptions options, object clientData, OnSessionInviteAcceptedCallback notificationFn)
		{
			AddNotifySessionInviteAcceptedOptionsInternal options2 = Helper.CopyProperties<AddNotifySessionInviteAcceptedOptionsInternal>(options);
			OnSessionInviteAcceptedCallbackInternal onSessionInviteAcceptedCallbackInternal = OnSessionInviteAccepted;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onSessionInviteAcceptedCallbackInternal);
			ulong num = EOS_Sessions_AddNotifySessionInviteAccepted(base.InnerHandle, ref options2, clientDataAddress, onSessionInviteAcceptedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifySessionInviteAccepted(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Sessions_RemoveNotifySessionInviteAccepted(base.InnerHandle, inId);
		}

		public ulong AddNotifyJoinSessionAccepted(AddNotifyJoinSessionAcceptedOptions options, object clientData, OnJoinSessionAcceptedCallback notificationFn)
		{
			AddNotifyJoinSessionAcceptedOptionsInternal options2 = Helper.CopyProperties<AddNotifyJoinSessionAcceptedOptionsInternal>(options);
			OnJoinSessionAcceptedCallbackInternal onJoinSessionAcceptedCallbackInternal = OnJoinSessionAccepted;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onJoinSessionAcceptedCallbackInternal);
			ulong num = EOS_Sessions_AddNotifyJoinSessionAccepted(base.InnerHandle, ref options2, clientDataAddress, onJoinSessionAcceptedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyJoinSessionAccepted(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Sessions_RemoveNotifyJoinSessionAccepted(base.InnerHandle, inId);
		}

		public Result CopySessionHandleByInviteId(CopySessionHandleByInviteIdOptions options, out SessionDetails outSessionHandle)
		{
			CopySessionHandleByInviteIdOptionsInternal options2 = Helper.CopyProperties<CopySessionHandleByInviteIdOptionsInternal>(options);
			outSessionHandle = Helper.GetDefault<SessionDetails>();
			IntPtr outSessionHandle2 = IntPtr.Zero;
			Result source = EOS_Sessions_CopySessionHandleByInviteId(base.InnerHandle, ref options2, ref outSessionHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outSessionHandle2, out outSessionHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopySessionHandleByUiEventId(CopySessionHandleByUiEventIdOptions options, out SessionDetails outSessionHandle)
		{
			CopySessionHandleByUiEventIdOptionsInternal options2 = Helper.CopyProperties<CopySessionHandleByUiEventIdOptionsInternal>(options);
			outSessionHandle = Helper.GetDefault<SessionDetails>();
			IntPtr outSessionHandle2 = IntPtr.Zero;
			Result source = EOS_Sessions_CopySessionHandleByUiEventId(base.InnerHandle, ref options2, ref outSessionHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outSessionHandle2, out outSessionHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopySessionHandleForPresence(CopySessionHandleForPresenceOptions options, out SessionDetails outSessionHandle)
		{
			CopySessionHandleForPresenceOptionsInternal options2 = Helper.CopyProperties<CopySessionHandleForPresenceOptionsInternal>(options);
			outSessionHandle = Helper.GetDefault<SessionDetails>();
			IntPtr outSessionHandle2 = IntPtr.Zero;
			Result source = EOS_Sessions_CopySessionHandleForPresence(base.InnerHandle, ref options2, ref outSessionHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outSessionHandle2, out outSessionHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result IsUserInSession(IsUserInSessionOptions options)
		{
			IsUserInSessionOptionsInternal options2 = Helper.CopyProperties<IsUserInSessionOptionsInternal>(options);
			Result source = EOS_Sessions_IsUserInSession(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result DumpSessionState(DumpSessionStateOptions options)
		{
			DumpSessionStateOptionsInternal options2 = Helper.CopyProperties<DumpSessionStateOptionsInternal>(options);
			Result source = EOS_Sessions_DumpSessionState(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void OnJoinSessionAccepted(IntPtr address)
		{
			OnJoinSessionAcceptedCallback callback = null;
			JoinSessionAcceptedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnJoinSessionAcceptedCallback, JoinSessionAcceptedCallbackInfoInternal, JoinSessionAcceptedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnSessionInviteAccepted(IntPtr address)
		{
			OnSessionInviteAcceptedCallback callback = null;
			SessionInviteAcceptedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnSessionInviteAcceptedCallback, SessionInviteAcceptedCallbackInfoInternal, SessionInviteAcceptedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnSessionInviteReceived(IntPtr address)
		{
			OnSessionInviteReceivedCallback callback = null;
			SessionInviteReceivedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnSessionInviteReceivedCallback, SessionInviteReceivedCallbackInfoInternal, SessionInviteReceivedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryInvites(IntPtr address)
		{
			OnQueryInvitesCallback callback = null;
			QueryInvitesCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryInvitesCallback, QueryInvitesCallbackInfoInternal, QueryInvitesCallbackInfo>(address, out callback, out callbackInfo))
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
		internal static void OnUnregisterPlayers(IntPtr address)
		{
			OnUnregisterPlayersCallback callback = null;
			UnregisterPlayersCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnUnregisterPlayersCallback, UnregisterPlayersCallbackInfoInternal, UnregisterPlayersCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnRegisterPlayers(IntPtr address)
		{
			OnRegisterPlayersCallback callback = null;
			RegisterPlayersCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnRegisterPlayersCallback, RegisterPlayersCallbackInfoInternal, RegisterPlayersCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnEndSession(IntPtr address)
		{
			OnEndSessionCallback callback = null;
			EndSessionCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnEndSessionCallback, EndSessionCallbackInfoInternal, EndSessionCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnStartSession(IntPtr address)
		{
			OnStartSessionCallback callback = null;
			StartSessionCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnStartSessionCallback, StartSessionCallbackInfoInternal, StartSessionCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnJoinSession(IntPtr address)
		{
			OnJoinSessionCallback callback = null;
			JoinSessionCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnJoinSessionCallback, JoinSessionCallbackInfoInternal, JoinSessionCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDestroySession(IntPtr address)
		{
			OnDestroySessionCallback callback = null;
			DestroySessionCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDestroySessionCallback, DestroySessionCallbackInfoInternal, DestroySessionCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnUpdateSession(IntPtr address)
		{
			OnUpdateSessionCallback callback = null;
			UpdateSessionCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnUpdateSessionCallback, UpdateSessionCallbackInfoInternal, UpdateSessionCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Sessions_DumpSessionState(IntPtr handle, ref DumpSessionStateOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Sessions_IsUserInSession(IntPtr handle, ref IsUserInSessionOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Sessions_CopySessionHandleForPresence(IntPtr handle, ref CopySessionHandleForPresenceOptionsInternal options, ref IntPtr outSessionHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Sessions_CopySessionHandleByUiEventId(IntPtr handle, ref CopySessionHandleByUiEventIdOptionsInternal options, ref IntPtr outSessionHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Sessions_CopySessionHandleByInviteId(IntPtr handle, ref CopySessionHandleByInviteIdOptionsInternal options, ref IntPtr outSessionHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_RemoveNotifyJoinSessionAccepted(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Sessions_AddNotifyJoinSessionAccepted(IntPtr handle, ref AddNotifyJoinSessionAcceptedOptionsInternal options, IntPtr clientData, OnJoinSessionAcceptedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_RemoveNotifySessionInviteAccepted(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Sessions_AddNotifySessionInviteAccepted(IntPtr handle, ref AddNotifySessionInviteAcceptedOptionsInternal options, IntPtr clientData, OnSessionInviteAcceptedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_RemoveNotifySessionInviteReceived(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Sessions_AddNotifySessionInviteReceived(IntPtr handle, ref AddNotifySessionInviteReceivedOptionsInternal options, IntPtr clientData, OnSessionInviteReceivedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Sessions_CopyActiveSessionHandle(IntPtr handle, ref CopyActiveSessionHandleOptionsInternal options, ref IntPtr outSessionHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Sessions_CreateSessionSearch(IntPtr handle, ref CreateSessionSearchOptionsInternal options, ref IntPtr outSessionSearchHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Sessions_GetInviteIdByIndex(IntPtr handle, ref GetInviteIdByIndexOptionsInternal options, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Sessions_GetInviteCount(IntPtr handle, ref GetInviteCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_QueryInvites(IntPtr handle, ref QueryInvitesOptionsInternal options, IntPtr clientData, OnQueryInvitesCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_RejectInvite(IntPtr handle, ref RejectInviteOptionsInternal options, IntPtr clientData, OnRejectInviteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_SendInvite(IntPtr handle, ref SendInviteOptionsInternal options, IntPtr clientData, OnSendInviteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_UnregisterPlayers(IntPtr handle, ref UnregisterPlayersOptionsInternal options, IntPtr clientData, OnUnregisterPlayersCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_RegisterPlayers(IntPtr handle, ref RegisterPlayersOptionsInternal options, IntPtr clientData, OnRegisterPlayersCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_EndSession(IntPtr handle, ref EndSessionOptionsInternal options, IntPtr clientData, OnEndSessionCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_StartSession(IntPtr handle, ref StartSessionOptionsInternal options, IntPtr clientData, OnStartSessionCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_JoinSession(IntPtr handle, ref JoinSessionOptionsInternal options, IntPtr clientData, OnJoinSessionCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_DestroySession(IntPtr handle, ref DestroySessionOptionsInternal options, IntPtr clientData, OnDestroySessionCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Sessions_UpdateSession(IntPtr handle, ref UpdateSessionOptionsInternal options, IntPtr clientData, OnUpdateSessionCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Sessions_UpdateSessionModification(IntPtr handle, ref UpdateSessionModificationOptionsInternal options, ref IntPtr outSessionModificationHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Sessions_CreateSessionModification(IntPtr handle, ref CreateSessionModificationOptionsInternal options, ref IntPtr outSessionModificationHandle);
	}
}
