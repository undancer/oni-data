using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.Lobby
{
	public sealed class LobbyInterface : Handle
	{
		public const int LobbysearchCopysearchresultbyindexApiLatest = 1;

		public const int LobbysearchGetsearchresultcountApiLatest = 1;

		public const int LobbysearchSetmaxresultsApiLatest = 1;

		public const int LobbysearchRemoveparameterApiLatest = 1;

		public const int LobbysearchSetparameterApiLatest = 1;

		public const int LobbysearchSettargetuseridApiLatest = 1;

		public const int LobbysearchSetlobbyidApiLatest = 1;

		public const int LobbysearchFindApiLatest = 1;

		public const int LobbydetailsGetmemberbyindexApiLatest = 1;

		public const int LobbydetailsGetmembercountApiLatest = 1;

		public const int LobbydetailsCopymemberattributebykeyApiLatest = 1;

		public const int LobbydetailsCopymemberattributebyindexApiLatest = 1;

		public const int LobbydetailsGetmemberattributecountApiLatest = 1;

		public const int LobbydetailsCopyattributebykeyApiLatest = 1;

		public const int LobbydetailsCopyattributebyindexApiLatest = 1;

		public const int LobbydetailsGetattributecountApiLatest = 1;

		public const int LobbydetailsCopyinfoApiLatest = 1;

		public const int LobbydetailsGetlobbyownerApiLatest = 1;

		public const int LobbymodificationRemovememberattributeApiLatest = 1;

		public const int LobbymodificationAddmemberattributeApiLatest = 1;

		public const int LobbymodificationRemoveattributeApiLatest = 1;

		public const int LobbymodificationAddattributeApiLatest = 1;

		public const int LobbymodificationSetmaxmembersApiLatest = 1;

		public const int LobbymodificationSetpermissionlevelApiLatest = 1;

		public const int AttributeApiLatest = 1;

		public const int AttributedataApiLatest = 1;

		public const string SearchMinslotsavailable = "minslotsavailable";

		public const string SearchMincurrentmembers = "mincurrentmembers";

		public const int CopylobbydetailshandleApiLatest = 1;

		public const int GetinviteidbyindexApiLatest = 1;

		public const int GetinvitecountApiLatest = 1;

		public const int QueryinvitesApiLatest = 1;

		public const int RejectinviteApiLatest = 1;

		public const int SendinviteApiLatest = 1;

		public const int CreatelobbysearchApiLatest = 1;

		public const int CopylobbydetailshandlebyuieventidApiLatest = 1;

		public const int CopylobbydetailshandlebyinviteidApiLatest = 1;

		public const int AddnotifyjoinlobbyacceptedApiLatest = 1;

		public const int AddnotifylobbyinviteacceptedApiLatest = 1;

		public const int AddnotifylobbyinvitereceivedApiLatest = 1;

		public const int InviteidMaxLength = 64;

		public const int AddnotifylobbymemberstatusreceivedApiLatest = 1;

		public const int AddnotifylobbymemberupdatereceivedApiLatest = 1;

		public const int AddnotifylobbyupdatereceivedApiLatest = 1;

		public const int KickmemberApiLatest = 1;

		public const int PromotememberApiLatest = 1;

		public const int UpdatelobbyApiLatest = 1;

		public const int UpdatelobbymodificationApiLatest = 1;

		public const int LeavelobbyApiLatest = 1;

		public const int JoinlobbyApiLatest = 2;

		public const int DestroylobbyApiLatest = 1;

		public const int CreatelobbyApiLatest = 2;

		public const int LobbydetailsInfoApiLatest = 1;

		public const int LobbymodificationMaxAttributeLength = 64;

		public const int LobbymodificationMaxAttributes = 64;

		public const int MaxSearchResults = 200;

		public const int MaxLobbyMembers = 64;

		public const int MaxLobbies = 4;

		public LobbyInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void CreateLobby(CreateLobbyOptions options, object clientData, OnCreateLobbyCallback completionDelegate)
		{
			CreateLobbyOptionsInternal options2 = Helper.CopyProperties<CreateLobbyOptionsInternal>(options);
			OnCreateLobbyCallbackInternal onCreateLobbyCallbackInternal = OnCreateLobby;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onCreateLobbyCallbackInternal);
			EOS_Lobby_CreateLobby(base.InnerHandle, ref options2, clientDataAddress, onCreateLobbyCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void DestroyLobby(DestroyLobbyOptions options, object clientData, OnDestroyLobbyCallback completionDelegate)
		{
			DestroyLobbyOptionsInternal options2 = Helper.CopyProperties<DestroyLobbyOptionsInternal>(options);
			OnDestroyLobbyCallbackInternal onDestroyLobbyCallbackInternal = OnDestroyLobby;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onDestroyLobbyCallbackInternal);
			EOS_Lobby_DestroyLobby(base.InnerHandle, ref options2, clientDataAddress, onDestroyLobbyCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void JoinLobby(JoinLobbyOptions options, object clientData, OnJoinLobbyCallback completionDelegate)
		{
			JoinLobbyOptionsInternal options2 = Helper.CopyProperties<JoinLobbyOptionsInternal>(options);
			OnJoinLobbyCallbackInternal onJoinLobbyCallbackInternal = OnJoinLobby;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onJoinLobbyCallbackInternal);
			EOS_Lobby_JoinLobby(base.InnerHandle, ref options2, clientDataAddress, onJoinLobbyCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void LeaveLobby(LeaveLobbyOptions options, object clientData, OnLeaveLobbyCallback completionDelegate)
		{
			LeaveLobbyOptionsInternal options2 = Helper.CopyProperties<LeaveLobbyOptionsInternal>(options);
			OnLeaveLobbyCallbackInternal onLeaveLobbyCallbackInternal = OnLeaveLobby;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onLeaveLobbyCallbackInternal);
			EOS_Lobby_LeaveLobby(base.InnerHandle, ref options2, clientDataAddress, onLeaveLobbyCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public Result UpdateLobbyModification(UpdateLobbyModificationOptions options, out LobbyModification outLobbyModificationHandle)
		{
			UpdateLobbyModificationOptionsInternal options2 = Helper.CopyProperties<UpdateLobbyModificationOptionsInternal>(options);
			outLobbyModificationHandle = Helper.GetDefault<LobbyModification>();
			IntPtr outLobbyModificationHandle2 = IntPtr.Zero;
			Result source = EOS_Lobby_UpdateLobbyModification(base.InnerHandle, ref options2, ref outLobbyModificationHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outLobbyModificationHandle2, out outLobbyModificationHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void UpdateLobby(UpdateLobbyOptions options, object clientData, OnUpdateLobbyCallback completionDelegate)
		{
			UpdateLobbyOptionsInternal options2 = Helper.CopyProperties<UpdateLobbyOptionsInternal>(options);
			OnUpdateLobbyCallbackInternal onUpdateLobbyCallbackInternal = OnUpdateLobby;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onUpdateLobbyCallbackInternal);
			EOS_Lobby_UpdateLobby(base.InnerHandle, ref options2, clientDataAddress, onUpdateLobbyCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void PromoteMember(PromoteMemberOptions options, object clientData, OnPromoteMemberCallback completionDelegate)
		{
			PromoteMemberOptionsInternal options2 = Helper.CopyProperties<PromoteMemberOptionsInternal>(options);
			OnPromoteMemberCallbackInternal onPromoteMemberCallbackInternal = OnPromoteMember;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onPromoteMemberCallbackInternal);
			EOS_Lobby_PromoteMember(base.InnerHandle, ref options2, clientDataAddress, onPromoteMemberCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void KickMember(KickMemberOptions options, object clientData, OnKickMemberCallback completionDelegate)
		{
			KickMemberOptionsInternal options2 = Helper.CopyProperties<KickMemberOptionsInternal>(options);
			OnKickMemberCallbackInternal onKickMemberCallbackInternal = OnKickMember;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onKickMemberCallbackInternal);
			EOS_Lobby_KickMember(base.InnerHandle, ref options2, clientDataAddress, onKickMemberCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public ulong AddNotifyLobbyUpdateReceived(AddNotifyLobbyUpdateReceivedOptions options, object clientData, OnLobbyUpdateReceivedCallback notificationFn)
		{
			AddNotifyLobbyUpdateReceivedOptionsInternal options2 = Helper.CopyProperties<AddNotifyLobbyUpdateReceivedOptionsInternal>(options);
			OnLobbyUpdateReceivedCallbackInternal onLobbyUpdateReceivedCallbackInternal = OnLobbyUpdateReceived;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onLobbyUpdateReceivedCallbackInternal);
			ulong num = EOS_Lobby_AddNotifyLobbyUpdateReceived(base.InnerHandle, ref options2, clientDataAddress, onLobbyUpdateReceivedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyLobbyUpdateReceived(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Lobby_RemoveNotifyLobbyUpdateReceived(base.InnerHandle, inId);
		}

		public ulong AddNotifyLobbyMemberUpdateReceived(AddNotifyLobbyMemberUpdateReceivedOptions options, object clientData, OnLobbyMemberUpdateReceivedCallback notificationFn)
		{
			AddNotifyLobbyMemberUpdateReceivedOptionsInternal options2 = Helper.CopyProperties<AddNotifyLobbyMemberUpdateReceivedOptionsInternal>(options);
			OnLobbyMemberUpdateReceivedCallbackInternal onLobbyMemberUpdateReceivedCallbackInternal = OnLobbyMemberUpdateReceived;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onLobbyMemberUpdateReceivedCallbackInternal);
			ulong num = EOS_Lobby_AddNotifyLobbyMemberUpdateReceived(base.InnerHandle, ref options2, clientDataAddress, onLobbyMemberUpdateReceivedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyLobbyMemberUpdateReceived(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Lobby_RemoveNotifyLobbyMemberUpdateReceived(base.InnerHandle, inId);
		}

		public ulong AddNotifyLobbyMemberStatusReceived(AddNotifyLobbyMemberStatusReceivedOptions options, object clientData, OnLobbyMemberStatusReceivedCallback notificationFn)
		{
			AddNotifyLobbyMemberStatusReceivedOptionsInternal options2 = Helper.CopyProperties<AddNotifyLobbyMemberStatusReceivedOptionsInternal>(options);
			OnLobbyMemberStatusReceivedCallbackInternal onLobbyMemberStatusReceivedCallbackInternal = OnLobbyMemberStatusReceived;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onLobbyMemberStatusReceivedCallbackInternal);
			ulong num = EOS_Lobby_AddNotifyLobbyMemberStatusReceived(base.InnerHandle, ref options2, clientDataAddress, onLobbyMemberStatusReceivedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyLobbyMemberStatusReceived(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Lobby_RemoveNotifyLobbyMemberStatusReceived(base.InnerHandle, inId);
		}

		public void SendInvite(SendInviteOptions options, object clientData, OnSendInviteCallback completionDelegate)
		{
			SendInviteOptionsInternal options2 = Helper.CopyProperties<SendInviteOptionsInternal>(options);
			OnSendInviteCallbackInternal onSendInviteCallbackInternal = OnSendInvite;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onSendInviteCallbackInternal);
			EOS_Lobby_SendInvite(base.InnerHandle, ref options2, clientDataAddress, onSendInviteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void RejectInvite(RejectInviteOptions options, object clientData, OnRejectInviteCallback completionDelegate)
		{
			RejectInviteOptionsInternal options2 = Helper.CopyProperties<RejectInviteOptionsInternal>(options);
			OnRejectInviteCallbackInternal onRejectInviteCallbackInternal = OnRejectInvite;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onRejectInviteCallbackInternal);
			EOS_Lobby_RejectInvite(base.InnerHandle, ref options2, clientDataAddress, onRejectInviteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryInvites(QueryInvitesOptions options, object clientData, OnQueryInvitesCallback completionDelegate)
		{
			QueryInvitesOptionsInternal options2 = Helper.CopyProperties<QueryInvitesOptionsInternal>(options);
			OnQueryInvitesCallbackInternal onQueryInvitesCallbackInternal = OnQueryInvites;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionDelegate, onQueryInvitesCallbackInternal);
			EOS_Lobby_QueryInvites(base.InnerHandle, ref options2, clientDataAddress, onQueryInvitesCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public uint GetInviteCount(GetInviteCountOptions options)
		{
			GetInviteCountOptionsInternal options2 = Helper.CopyProperties<GetInviteCountOptionsInternal>(options);
			uint source = EOS_Lobby_GetInviteCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetInviteIdByIndex(GetInviteIdByIndexOptions options, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			GetInviteIdByIndexOptionsInternal options2 = Helper.CopyProperties<GetInviteIdByIndexOptionsInternal>(options);
			Result source = EOS_Lobby_GetInviteIdByIndex(base.InnerHandle, ref options2, outBuffer, ref inOutBufferLength);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CreateLobbySearch(CreateLobbySearchOptions options, out LobbySearch outLobbySearchHandle)
		{
			CreateLobbySearchOptionsInternal options2 = Helper.CopyProperties<CreateLobbySearchOptionsInternal>(options);
			outLobbySearchHandle = Helper.GetDefault<LobbySearch>();
			IntPtr outLobbySearchHandle2 = IntPtr.Zero;
			Result source = EOS_Lobby_CreateLobbySearch(base.InnerHandle, ref options2, ref outLobbySearchHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outLobbySearchHandle2, out outLobbySearchHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ulong AddNotifyLobbyInviteReceived(AddNotifyLobbyInviteReceivedOptions options, object clientData, OnLobbyInviteReceivedCallback notificationFn)
		{
			AddNotifyLobbyInviteReceivedOptionsInternal options2 = Helper.CopyProperties<AddNotifyLobbyInviteReceivedOptionsInternal>(options);
			OnLobbyInviteReceivedCallbackInternal onLobbyInviteReceivedCallbackInternal = OnLobbyInviteReceived;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onLobbyInviteReceivedCallbackInternal);
			ulong num = EOS_Lobby_AddNotifyLobbyInviteReceived(base.InnerHandle, ref options2, clientDataAddress, onLobbyInviteReceivedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyLobbyInviteReceived(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Lobby_RemoveNotifyLobbyInviteReceived(base.InnerHandle, inId);
		}

		public ulong AddNotifyLobbyInviteAccepted(AddNotifyLobbyInviteAcceptedOptions options, object clientData, OnLobbyInviteAcceptedCallback notificationFn)
		{
			AddNotifyLobbyInviteAcceptedOptionsInternal options2 = Helper.CopyProperties<AddNotifyLobbyInviteAcceptedOptionsInternal>(options);
			OnLobbyInviteAcceptedCallbackInternal onLobbyInviteAcceptedCallbackInternal = OnLobbyInviteAccepted;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onLobbyInviteAcceptedCallbackInternal);
			ulong num = EOS_Lobby_AddNotifyLobbyInviteAccepted(base.InnerHandle, ref options2, clientDataAddress, onLobbyInviteAcceptedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyLobbyInviteAccepted(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Lobby_RemoveNotifyLobbyInviteAccepted(base.InnerHandle, inId);
		}

		public ulong AddNotifyJoinLobbyAccepted(AddNotifyJoinLobbyAcceptedOptions options, object clientData, OnJoinLobbyAcceptedCallback notificationFn)
		{
			AddNotifyJoinLobbyAcceptedOptionsInternal options2 = Helper.CopyProperties<AddNotifyJoinLobbyAcceptedOptionsInternal>(options);
			OnJoinLobbyAcceptedCallbackInternal onJoinLobbyAcceptedCallbackInternal = OnJoinLobbyAccepted;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, notificationFn, onJoinLobbyAcceptedCallbackInternal);
			ulong num = EOS_Lobby_AddNotifyJoinLobbyAccepted(base.InnerHandle, ref options2, clientDataAddress, onJoinLobbyAcceptedCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryAssignNotificationIdToCallback(clientDataAddress, num);
			ulong target = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet(num, out target);
			return target;
		}

		public void RemoveNotifyJoinLobbyAccepted(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			EOS_Lobby_RemoveNotifyJoinLobbyAccepted(base.InnerHandle, inId);
		}

		public Result CopyLobbyDetailsHandleByInviteId(CopyLobbyDetailsHandleByInviteIdOptions options, out LobbyDetails outLobbyDetailsHandle)
		{
			CopyLobbyDetailsHandleByInviteIdOptionsInternal options2 = Helper.CopyProperties<CopyLobbyDetailsHandleByInviteIdOptionsInternal>(options);
			outLobbyDetailsHandle = Helper.GetDefault<LobbyDetails>();
			IntPtr outLobbyDetailsHandle2 = IntPtr.Zero;
			Result source = EOS_Lobby_CopyLobbyDetailsHandleByInviteId(base.InnerHandle, ref options2, ref outLobbyDetailsHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outLobbyDetailsHandle2, out outLobbyDetailsHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyLobbyDetailsHandleByUiEventId(CopyLobbyDetailsHandleByUiEventIdOptions options, out LobbyDetails outLobbyDetailsHandle)
		{
			CopyLobbyDetailsHandleByUiEventIdOptionsInternal options2 = Helper.CopyProperties<CopyLobbyDetailsHandleByUiEventIdOptionsInternal>(options);
			outLobbyDetailsHandle = Helper.GetDefault<LobbyDetails>();
			IntPtr outLobbyDetailsHandle2 = IntPtr.Zero;
			Result source = EOS_Lobby_CopyLobbyDetailsHandleByUiEventId(base.InnerHandle, ref options2, ref outLobbyDetailsHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outLobbyDetailsHandle2, out outLobbyDetailsHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyLobbyDetailsHandle(CopyLobbyDetailsHandleOptions options, out LobbyDetails outLobbyDetailsHandle)
		{
			CopyLobbyDetailsHandleOptionsInternal options2 = Helper.CopyProperties<CopyLobbyDetailsHandleOptionsInternal>(options);
			outLobbyDetailsHandle = Helper.GetDefault<LobbyDetails>();
			IntPtr outLobbyDetailsHandle2 = IntPtr.Zero;
			Result source = EOS_Lobby_CopyLobbyDetailsHandle(base.InnerHandle, ref options2, ref outLobbyDetailsHandle2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalGet(outLobbyDetailsHandle2, out outLobbyDetailsHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static void OnJoinLobbyAccepted(IntPtr address)
		{
			OnJoinLobbyAcceptedCallback callback = null;
			JoinLobbyAcceptedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnJoinLobbyAcceptedCallback, JoinLobbyAcceptedCallbackInfoInternal, JoinLobbyAcceptedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLobbyInviteAccepted(IntPtr address)
		{
			OnLobbyInviteAcceptedCallback callback = null;
			LobbyInviteAcceptedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLobbyInviteAcceptedCallback, LobbyInviteAcceptedCallbackInfoInternal, LobbyInviteAcceptedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLobbyInviteReceived(IntPtr address)
		{
			OnLobbyInviteReceivedCallback callback = null;
			LobbyInviteReceivedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLobbyInviteReceivedCallback, LobbyInviteReceivedCallbackInfoInternal, LobbyInviteReceivedCallbackInfo>(address, out callback, out callbackInfo))
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
		internal static void OnLobbyMemberStatusReceived(IntPtr address)
		{
			OnLobbyMemberStatusReceivedCallback callback = null;
			LobbyMemberStatusReceivedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLobbyMemberStatusReceivedCallback, LobbyMemberStatusReceivedCallbackInfoInternal, LobbyMemberStatusReceivedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLobbyMemberUpdateReceived(IntPtr address)
		{
			OnLobbyMemberUpdateReceivedCallback callback = null;
			LobbyMemberUpdateReceivedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLobbyMemberUpdateReceivedCallback, LobbyMemberUpdateReceivedCallbackInfoInternal, LobbyMemberUpdateReceivedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLobbyUpdateReceived(IntPtr address)
		{
			OnLobbyUpdateReceivedCallback callback = null;
			LobbyUpdateReceivedCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLobbyUpdateReceivedCallback, LobbyUpdateReceivedCallbackInfoInternal, LobbyUpdateReceivedCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnKickMember(IntPtr address)
		{
			OnKickMemberCallback callback = null;
			KickMemberCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnKickMemberCallback, KickMemberCallbackInfoInternal, KickMemberCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnPromoteMember(IntPtr address)
		{
			OnPromoteMemberCallback callback = null;
			PromoteMemberCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnPromoteMemberCallback, PromoteMemberCallbackInfoInternal, PromoteMemberCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnUpdateLobby(IntPtr address)
		{
			OnUpdateLobbyCallback callback = null;
			UpdateLobbyCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnUpdateLobbyCallback, UpdateLobbyCallbackInfoInternal, UpdateLobbyCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLeaveLobby(IntPtr address)
		{
			OnLeaveLobbyCallback callback = null;
			LeaveLobbyCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLeaveLobbyCallback, LeaveLobbyCallbackInfoInternal, LeaveLobbyCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnJoinLobby(IntPtr address)
		{
			OnJoinLobbyCallback callback = null;
			JoinLobbyCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnJoinLobbyCallback, JoinLobbyCallbackInfoInternal, JoinLobbyCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDestroyLobby(IntPtr address)
		{
			OnDestroyLobbyCallback callback = null;
			DestroyLobbyCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDestroyLobbyCallback, DestroyLobbyCallbackInfoInternal, DestroyLobbyCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnCreateLobby(IntPtr address)
		{
			OnCreateLobbyCallback callback = null;
			CreateLobbyCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnCreateLobbyCallback, CreateLobbyCallbackInfoInternal, CreateLobbyCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_Attribute_Release(IntPtr lobbyAttribute);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Lobby_CopyLobbyDetailsHandle(IntPtr handle, ref CopyLobbyDetailsHandleOptionsInternal options, ref IntPtr outLobbyDetailsHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Lobby_CopyLobbyDetailsHandleByUiEventId(IntPtr handle, ref CopyLobbyDetailsHandleByUiEventIdOptionsInternal options, ref IntPtr outLobbyDetailsHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Lobby_CopyLobbyDetailsHandleByInviteId(IntPtr handle, ref CopyLobbyDetailsHandleByInviteIdOptionsInternal options, ref IntPtr outLobbyDetailsHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyJoinLobbyAccepted(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyJoinLobbyAccepted(IntPtr handle, ref AddNotifyJoinLobbyAcceptedOptionsInternal options, IntPtr clientData, OnJoinLobbyAcceptedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyLobbyInviteAccepted(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyLobbyInviteAccepted(IntPtr handle, ref AddNotifyLobbyInviteAcceptedOptionsInternal options, IntPtr clientData, OnLobbyInviteAcceptedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyLobbyInviteReceived(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyLobbyInviteReceived(IntPtr handle, ref AddNotifyLobbyInviteReceivedOptionsInternal options, IntPtr clientData, OnLobbyInviteReceivedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Lobby_CreateLobbySearch(IntPtr handle, ref CreateLobbySearchOptionsInternal options, ref IntPtr outLobbySearchHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Lobby_GetInviteIdByIndex(IntPtr handle, ref GetInviteIdByIndexOptionsInternal options, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_Lobby_GetInviteCount(IntPtr handle, ref GetInviteCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_QueryInvites(IntPtr handle, ref QueryInvitesOptionsInternal options, IntPtr clientData, OnQueryInvitesCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_RejectInvite(IntPtr handle, ref RejectInviteOptionsInternal options, IntPtr clientData, OnRejectInviteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_SendInvite(IntPtr handle, ref SendInviteOptionsInternal options, IntPtr clientData, OnSendInviteCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyLobbyMemberStatusReceived(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyLobbyMemberStatusReceived(IntPtr handle, ref AddNotifyLobbyMemberStatusReceivedOptionsInternal options, IntPtr clientData, OnLobbyMemberStatusReceivedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyLobbyMemberUpdateReceived(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyLobbyMemberUpdateReceived(IntPtr handle, ref AddNotifyLobbyMemberUpdateReceivedOptionsInternal options, IntPtr clientData, OnLobbyMemberUpdateReceivedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyLobbyUpdateReceived(IntPtr handle, ulong inId);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyLobbyUpdateReceived(IntPtr handle, ref AddNotifyLobbyUpdateReceivedOptionsInternal options, IntPtr clientData, OnLobbyUpdateReceivedCallbackInternal notificationFn);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_KickMember(IntPtr handle, ref KickMemberOptionsInternal options, IntPtr clientData, OnKickMemberCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_PromoteMember(IntPtr handle, ref PromoteMemberOptionsInternal options, IntPtr clientData, OnPromoteMemberCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_UpdateLobby(IntPtr handle, ref UpdateLobbyOptionsInternal options, IntPtr clientData, OnUpdateLobbyCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Lobby_UpdateLobbyModification(IntPtr handle, ref UpdateLobbyModificationOptionsInternal options, ref IntPtr outLobbyModificationHandle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_LeaveLobby(IntPtr handle, ref LeaveLobbyOptionsInternal options, IntPtr clientData, OnLeaveLobbyCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_JoinLobby(IntPtr handle, ref JoinLobbyOptionsInternal options, IntPtr clientData, OnJoinLobbyCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_DestroyLobby(IntPtr handle, ref DestroyLobbyOptionsInternal options, IntPtr clientData, OnDestroyLobbyCallbackInternal completionDelegate);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Lobby_CreateLobby(IntPtr handle, ref CreateLobbyOptionsInternal options, IntPtr clientData, OnCreateLobbyCallbackInternal completionDelegate);
	}
}
