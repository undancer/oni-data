using System;
using System.Collections.Generic;

namespace rail
{
	public class RailCallBackHelper
	{
		private static volatile RailCallBackHelper instance_;

		private static readonly object locker_ = new object();

		private static Dictionary<RAILEventID, RailEventCallBackHandler> eventHandlers_ = new Dictionary<RAILEventID, RailEventCallBackHandler>();

		private static RailEventCallBackFunction delegate_ = OnRailCallBack;

		public static RailCallBackHelper Instance
		{
			get
			{
				if (instance_ == null)
				{
					lock (locker_)
					{
						if (instance_ == null)
						{
							instance_ = new RailCallBackHelper();
						}
					}
				}
				return instance_;
			}
		}

		private RailCallBackHelper()
		{
		}

		public void RegisterCallback(RAILEventID event_id, RailEventCallBackHandler handler)
		{
			lock (locker_)
			{
				if (eventHandlers_.ContainsKey(event_id))
				{
					Dictionary<RAILEventID, RailEventCallBackHandler> dictionary = eventHandlers_;
					dictionary[event_id] = (RailEventCallBackHandler)Delegate.Combine(dictionary[event_id], handler);
				}
				else
				{
					eventHandlers_.Add(event_id, handler);
					RAIL_API_PINVOKE.CSharpRailRegisterEvent((int)event_id, delegate_);
				}
			}
		}

		public void UnregisterCallback(RAILEventID event_id, RailEventCallBackHandler handler)
		{
			lock (locker_)
			{
				if (eventHandlers_.ContainsKey(event_id))
				{
					Dictionary<RAILEventID, RailEventCallBackHandler> dictionary = eventHandlers_;
					dictionary[event_id] = (RailEventCallBackHandler)Delegate.Remove(dictionary[event_id], handler);
					if (eventHandlers_[event_id] == null)
					{
						RAIL_API_PINVOKE.CSharpRailUnRegisterEvent((int)event_id, delegate_);
						eventHandlers_.Remove(event_id);
					}
				}
			}
		}

		public void UnregisterCallback(RAILEventID event_id)
		{
			lock (locker_)
			{
				RAIL_API_PINVOKE.CSharpRailUnRegisterEvent((int)event_id, delegate_);
				if (eventHandlers_[event_id] != null)
				{
					eventHandlers_.Remove(event_id);
				}
			}
		}

		public void UnregisterAllCallback()
		{
			lock (locker_)
			{
				RAIL_API_PINVOKE.CSharpRailUnRegisterAllEvent();
				eventHandlers_.Clear();
			}
		}

		[MonoPInvokeCallback(typeof(RailEventCallBackFunction))]
		public static void OnRailCallBack(RAILEventID event_id, IntPtr data)
		{
			RailEventCallBackHandler railEventCallBackHandler = eventHandlers_[event_id];
			if (railEventCallBackHandler != null)
			{
				switch (event_id)
				{
				case RAILEventID.kRailPlatformNotifyEventJoinGameByGameServer:
				{
					RailPlatformNotifyEventJoinGameByGameServer railPlatformNotifyEventJoinGameByGameServer = new RailPlatformNotifyEventJoinGameByGameServer();
					RailConverter.Cpp2Csharp(data, railPlatformNotifyEventJoinGameByGameServer);
					railEventCallBackHandler(event_id, railPlatformNotifyEventJoinGameByGameServer);
					break;
				}
				case RAILEventID.kRailEventInGameActivityGameActivityPlayerEvent:
				{
					RailGameActivityPlayerEvent railGameActivityPlayerEvent = new RailGameActivityPlayerEvent();
					RailConverter.Cpp2Csharp(data, railGameActivityPlayerEvent);
					railEventCallBackHandler(event_id, railGameActivityPlayerEvent);
					break;
				}
				case RAILEventID.kRailEventGameServerGetMetadataResult:
				{
					GetGameServerMetadataResult getGameServerMetadataResult = new GetGameServerMetadataResult();
					RailConverter.Cpp2Csharp(data, getGameServerMetadataResult);
					railEventCallBackHandler(event_id, getGameServerMetadataResult);
					break;
				}
				case RAILEventID.kRailEventNetworkCreateSessionFailed:
				{
					CreateSessionFailed createSessionFailed = new CreateSessionFailed();
					RailConverter.Cpp2Csharp(data, createSessionFailed);
					railEventCallBackHandler(event_id, createSessionFailed);
					break;
				}
				case RAILEventID.kRailEventGameServerRemoveFavoriteGameServer:
				{
					AsyncRemoveFavoriteGameServerResult asyncRemoveFavoriteGameServerResult = new AsyncRemoveFavoriteGameServerResult();
					RailConverter.Cpp2Csharp(data, asyncRemoveFavoriteGameServerResult);
					railEventCallBackHandler(event_id, asyncRemoveFavoriteGameServerResult);
					break;
				}
				case RAILEventID.kRailEventInGameCoinPurchaseCoinsResult:
				{
					RailInGameCoinPurchaseCoinsResponse railInGameCoinPurchaseCoinsResponse = new RailInGameCoinPurchaseCoinsResponse();
					RailConverter.Cpp2Csharp(data, railInGameCoinPurchaseCoinsResponse);
					railEventCallBackHandler(event_id, railInGameCoinPurchaseCoinsResponse);
					break;
				}
				case RAILEventID.kRailEventNetworkCreateRawSessionFailed:
				{
					NetworkCreateRawSessionFailed networkCreateRawSessionFailed = new NetworkCreateRawSessionFailed();
					RailConverter.Cpp2Csharp(data, networkCreateRawSessionFailed);
					railEventCallBackHandler(event_id, networkCreateRawSessionFailed);
					break;
				}
				case RAILEventID.kRailEventUsersInviteJoinGameResult:
				{
					RailUsersInviteJoinGameResult railUsersInviteJoinGameResult = new RailUsersInviteJoinGameResult();
					RailConverter.Cpp2Csharp(data, railUsersInviteJoinGameResult);
					railEventCallBackHandler(event_id, railUsersInviteJoinGameResult);
					break;
				}
				case RAILEventID.kRailEventInGameActivityNotifyNewGameActivities:
				{
					RailNotifyNewGameActivities railNotifyNewGameActivities = new RailNotifyNewGameActivities();
					RailConverter.Cpp2Csharp(data, railNotifyNewGameActivities);
					railEventCallBackHandler(event_id, railNotifyNewGameActivities);
					break;
				}
				case RAILEventID.kRailEventGameServerGetSessionTicket:
				{
					AsyncAcquireGameServerSessionTicketResponse asyncAcquireGameServerSessionTicketResponse = new AsyncAcquireGameServerSessionTicketResponse();
					RailConverter.Cpp2Csharp(data, asyncAcquireGameServerSessionTicketResponse);
					railEventCallBackHandler(event_id, asyncAcquireGameServerSessionTicketResponse);
					break;
				}
				case RAILEventID.kRailEventBrowserStateChanged:
				{
					BrowserRenderStateChanged browserRenderStateChanged = new BrowserRenderStateChanged();
					RailConverter.Cpp2Csharp(data, browserRenderStateChanged);
					railEventCallBackHandler(event_id, browserRenderStateChanged);
					break;
				}
				case RAILEventID.kRailEventUserSpaceRemoveSpaceWorkResult:
				{
					AsyncRemoveSpaceWorkResult asyncRemoveSpaceWorkResult = new AsyncRemoveSpaceWorkResult();
					RailConverter.Cpp2Csharp(data, asyncRemoveSpaceWorkResult);
					railEventCallBackHandler(event_id, asyncRemoveSpaceWorkResult);
					break;
				}
				case RAILEventID.kRailEventScreenshotTakeScreenshotRequest:
				{
					ScreenshotRequestInfo screenshotRequestInfo = new ScreenshotRequestInfo();
					RailConverter.Cpp2Csharp(data, screenshotRequestInfo);
					railEventCallBackHandler(event_id, screenshotRequestInfo);
					break;
				}
				case RAILEventID.kRailEventSessionTicketAuthSessionTicket:
				{
					StartSessionWithPlayerResponse startSessionWithPlayerResponse = new StartSessionWithPlayerResponse();
					RailConverter.Cpp2Csharp(data, startSessionWithPlayerResponse);
					railEventCallBackHandler(event_id, startSessionWithPlayerResponse);
					break;
				}
				case RAILEventID.kRailEventStatsPlayerStatsStored:
				{
					PlayerStatsStored playerStatsStored = new PlayerStatsStored();
					RailConverter.Cpp2Csharp(data, playerStatsStored);
					railEventCallBackHandler(event_id, playerStatsStored);
					break;
				}
				case RAILEventID.kRailEventInGamePurchaseAllProductsInfoReceived:
				{
					RailInGamePurchaseRequestAllProductsResponse railInGamePurchaseRequestAllProductsResponse = new RailInGamePurchaseRequestAllProductsResponse();
					RailConverter.Cpp2Csharp(data, railInGamePurchaseRequestAllProductsResponse);
					railEventCallBackHandler(event_id, railInGamePurchaseRequestAllProductsResponse);
					break;
				}
				case RAILEventID.kRailEventAppQuerySubscribeWishPlayStateResult:
				{
					QuerySubscribeWishPlayStateResult querySubscribeWishPlayStateResult = new QuerySubscribeWishPlayStateResult();
					RailConverter.Cpp2Csharp(data, querySubscribeWishPlayStateResult);
					railEventCallBackHandler(event_id, querySubscribeWishPlayStateResult);
					break;
				}
				case RAILEventID.kRailEventSessionTicketGetSessionTicket:
				{
					AcquireSessionTicketResponse acquireSessionTicketResponse = new AcquireSessionTicketResponse();
					RailConverter.Cpp2Csharp(data, acquireSessionTicketResponse);
					railEventCallBackHandler(event_id, acquireSessionTicketResponse);
					break;
				}
				case RAILEventID.kRailEventGameServerAddFavoriteGameServer:
				{
					AsyncAddFavoriteGameServerResult asyncAddFavoriteGameServerResult = new AsyncAddFavoriteGameServerResult();
					RailConverter.Cpp2Csharp(data, asyncAddFavoriteGameServerResult);
					railEventCallBackHandler(event_id, asyncAddFavoriteGameServerResult);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelAddUsersResult:
				{
					VoiceChannelAddUsersResult voiceChannelAddUsersResult = new VoiceChannelAddUsersResult();
					RailConverter.Cpp2Csharp(data, voiceChannelAddUsersResult);
					railEventCallBackHandler(event_id, voiceChannelAddUsersResult);
					break;
				}
				case RAILEventID.kRailEventScreenshotPublishScreenshotFinished:
				{
					PublishScreenshotResult publishScreenshotResult = new PublishScreenshotResult();
					RailConverter.Cpp2Csharp(data, publishScreenshotResult);
					railEventCallBackHandler(event_id, publishScreenshotResult);
					break;
				}
				case RAILEventID.kRailEventQueryPlayerBannedStatus:
				{
					QueryPlayerBannedStatus queryPlayerBannedStatus = new QueryPlayerBannedStatus();
					RailConverter.Cpp2Csharp(data, queryPlayerBannedStatus);
					railEventCallBackHandler(event_id, queryPlayerBannedStatus);
					break;
				}
				case RAILEventID.kRailEventAssetsSplitToFinished:
				{
					SplitAssetsToFinished splitAssetsToFinished = new SplitAssetsToFinished();
					RailConverter.Cpp2Csharp(data, splitAssetsToFinished);
					railEventCallBackHandler(event_id, splitAssetsToFinished);
					break;
				}
				case RAILEventID.kRailEventBrowserCreateResult:
				{
					CreateBrowserResult createBrowserResult = new CreateBrowserResult();
					RailConverter.Cpp2Csharp(data, createBrowserResult);
					railEventCallBackHandler(event_id, createBrowserResult);
					break;
				}
				case RAILEventID.kRailEventFriendsSetMetadataResult:
				{
					RailFriendsSetMetadataResult railFriendsSetMetadataResult = new RailFriendsSetMetadataResult();
					RailConverter.Cpp2Csharp(data, railFriendsSetMetadataResult);
					railEventCallBackHandler(event_id, railFriendsSetMetadataResult);
					break;
				}
				case RAILEventID.kRailEventStorageAsyncDeleteStreamFileResult:
				{
					AsyncDeleteStreamFileResult asyncDeleteStreamFileResult = new AsyncDeleteStreamFileResult();
					RailConverter.Cpp2Csharp(data, asyncDeleteStreamFileResult);
					railEventCallBackHandler(event_id, asyncDeleteStreamFileResult);
					break;
				}
				case RAILEventID.kRailEventUsersGetUsersInfo:
				{
					RailUsersInfoData railUsersInfoData = new RailUsersInfoData();
					RailConverter.Cpp2Csharp(data, railUsersInfoData);
					railEventCallBackHandler(event_id, railUsersInfoData);
					break;
				}
				case RAILEventID.kRailEventGameServerPlayerListResult:
				{
					GetGameServerPlayerListResult getGameServerPlayerListResult = new GetGameServerPlayerListResult();
					RailConverter.Cpp2Csharp(data, getGameServerPlayerListResult);
					railEventCallBackHandler(event_id, getGameServerPlayerListResult);
					break;
				}
				case RAILEventID.kRailEventDlcRefundChanged:
				{
					DlcRefundChanged dlcRefundChanged = new DlcRefundChanged();
					RailConverter.Cpp2Csharp(data, dlcRefundChanged);
					railEventCallBackHandler(event_id, dlcRefundChanged);
					break;
				}
				case RAILEventID.kRailEventAntiAddictionCustomizeAntiAddictionActions:
				{
					RailCustomizeAntiAddictionActions railCustomizeAntiAddictionActions = new RailCustomizeAntiAddictionActions();
					RailConverter.Cpp2Csharp(data, railCustomizeAntiAddictionActions);
					railEventCallBackHandler(event_id, railCustomizeAntiAddictionActions);
					break;
				}
				case RAILEventID.kRailEventIMEHelperTextInputCompositionStateChanged:
				{
					RailIMEHelperTextInputCompositionState railIMEHelperTextInputCompositionState = new RailIMEHelperTextInputCompositionState();
					RailConverter.Cpp2Csharp(data, railIMEHelperTextInputCompositionState);
					railEventCallBackHandler(event_id, railIMEHelperTextInputCompositionState);
					break;
				}
				case RAILEventID.kRailEventAssetsAssetsChanged:
				{
					RailAssetsChanged railAssetsChanged = new RailAssetsChanged();
					RailConverter.Cpp2Csharp(data, railAssetsChanged);
					railEventCallBackHandler(event_id, railAssetsChanged);
					break;
				}
				case RAILEventID.kRailEventAssetsUpdateAssetPropertyFinished:
				{
					UpdateAssetsPropertyFinished updateAssetsPropertyFinished = new UpdateAssetsPropertyFinished();
					RailConverter.Cpp2Csharp(data, updateAssetsPropertyFinished);
					railEventCallBackHandler(event_id, updateAssetsPropertyFinished);
					break;
				}
				case RAILEventID.kRailEventUserSpaceSyncResult:
				{
					SyncSpaceWorkResult syncSpaceWorkResult = new SyncSpaceWorkResult();
					RailConverter.Cpp2Csharp(data, syncSpaceWorkResult);
					railEventCallBackHandler(event_id, syncSpaceWorkResult);
					break;
				}
				case RAILEventID.kRailEventDlcQueryIsOwnedDlcsResult:
				{
					QueryIsOwnedDlcsResult queryIsOwnedDlcsResult = new QueryIsOwnedDlcsResult();
					RailConverter.Cpp2Csharp(data, queryIsOwnedDlcsResult);
					railEventCallBackHandler(event_id, queryIsOwnedDlcsResult);
					break;
				}
				case RAILEventID.kRailEventRoomNotifyRoomDataReceived:
				{
					RoomDataReceived roomDataReceived = new RoomDataReceived();
					RailConverter.Cpp2Csharp(data, roomDataReceived);
					railEventCallBackHandler(event_id, roomDataReceived);
					break;
				}
				case RAILEventID.kRailEventRoomNotifyRoomGameServerChanged:
				{
					NotifyRoomGameServerChange notifyRoomGameServerChange = new NotifyRoomGameServerChange();
					RailConverter.Cpp2Csharp(data, notifyRoomGameServerChange);
					railEventCallBackHandler(event_id, notifyRoomGameServerChange);
					break;
				}
				case RAILEventID.kRailEventLeaderboardEntryReceived:
				{
					LeaderboardEntryReceived leaderboardEntryReceived = new LeaderboardEntryReceived();
					RailConverter.Cpp2Csharp(data, leaderboardEntryReceived);
					railEventCallBackHandler(event_id, leaderboardEntryReceived);
					break;
				}
				case RAILEventID.kRailEventFriendsFriendsListChanged:
				{
					RailFriendsListChanged railFriendsListChanged = new RailFriendsListChanged();
					RailConverter.Cpp2Csharp(data, railFriendsListChanged);
					railEventCallBackHandler(event_id, railFriendsListChanged);
					break;
				}
				case RAILEventID.kRailEventFriendsClearMetadataResult:
				{
					RailFriendsClearMetadataResult railFriendsClearMetadataResult = new RailFriendsClearMetadataResult();
					RailConverter.Cpp2Csharp(data, railFriendsClearMetadataResult);
					railEventCallBackHandler(event_id, railFriendsClearMetadataResult);
					break;
				}
				case RAILEventID.kRailEventRoomGetAllDataResult:
				{
					GetAllRoomDataResult getAllRoomDataResult = new GetAllRoomDataResult();
					RailConverter.Cpp2Csharp(data, getAllRoomDataResult);
					railEventCallBackHandler(event_id, getAllRoomDataResult);
					break;
				}
				case RAILEventID.kRailEventSmallObjectServiceDownloadResult:
				{
					RailSmallObjectDownloadResult railSmallObjectDownloadResult = new RailSmallObjectDownloadResult();
					RailConverter.Cpp2Csharp(data, railSmallObjectDownloadResult);
					railEventCallBackHandler(event_id, railSmallObjectDownloadResult);
					break;
				}
				case RAILEventID.kRailEventRoomClearRoomMetadataResult:
				{
					ClearRoomMetadataResult clearRoomMetadataResult = new ClearRoomMetadataResult();
					RailConverter.Cpp2Csharp(data, clearRoomMetadataResult);
					railEventCallBackHandler(event_id, clearRoomMetadataResult);
					break;
				}
				case RAILEventID.kRailEventUserSpaceDownloadResult:
				{
					UserSpaceDownloadResult userSpaceDownloadResult = new UserSpaceDownloadResult();
					RailConverter.Cpp2Csharp(data, userSpaceDownloadResult);
					railEventCallBackHandler(event_id, userSpaceDownloadResult);
					break;
				}
				case RAILEventID.kRailEventHttpSessionResponseResult:
				{
					RailHttpSessionResponse railHttpSessionResponse = new RailHttpSessionResponse();
					RailConverter.Cpp2Csharp(data, railHttpSessionResponse);
					railEventCallBackHandler(event_id, railHttpSessionResponse);
					break;
				}
				case RAILEventID.kRailEventUsersInviteUsersResult:
				{
					RailUsersInviteUsersResult railUsersInviteUsersResult = new RailUsersInviteUsersResult();
					RailConverter.Cpp2Csharp(data, railUsersInviteUsersResult);
					railEventCallBackHandler(event_id, railUsersInviteUsersResult);
					break;
				}
				case RAILEventID.kRailEventDlcUninstallFinished:
				{
					DlcUninstallFinished dlcUninstallFinished = new DlcUninstallFinished();
					RailConverter.Cpp2Csharp(data, dlcUninstallFinished);
					railEventCallBackHandler(event_id, dlcUninstallFinished);
					break;
				}
				case RAILEventID.kRailEventSystemStateChanged:
				{
					RailSystemStateChanged railSystemStateChanged = new RailSystemStateChanged();
					RailConverter.Cpp2Csharp(data, railSystemStateChanged);
					railEventCallBackHandler(event_id, railSystemStateChanged);
					break;
				}
				case RAILEventID.kRailEventUsersGetUserLimitsResult:
				{
					RailUsersGetUserLimitsResult railUsersGetUserLimitsResult = new RailUsersGetUserLimitsResult();
					RailConverter.Cpp2Csharp(data, railUsersGetUserLimitsResult);
					railEventCallBackHandler(event_id, railUsersGetUserLimitsResult);
					break;
				}
				case RAILEventID.kRailEventUtilsGetImageDataResult:
				{
					RailGetImageDataResult railGetImageDataResult = new RailGetImageDataResult();
					RailConverter.Cpp2Csharp(data, railGetImageDataResult);
					railEventCallBackHandler(event_id, railGetImageDataResult);
					break;
				}
				case RAILEventID.kRailEventGroupChatOpenGroupChatResult:
				{
					RailOpenGroupChatResult railOpenGroupChatResult = new RailOpenGroupChatResult();
					RailConverter.Cpp2Csharp(data, railOpenGroupChatResult);
					railEventCallBackHandler(event_id, railOpenGroupChatResult);
					break;
				}
				case RAILEventID.kRailEventStorageShareToSpaceWorkResult:
				{
					ShareStorageToSpaceWorkResult shareStorageToSpaceWorkResult = new ShareStorageToSpaceWorkResult();
					RailConverter.Cpp2Csharp(data, shareStorageToSpaceWorkResult);
					railEventCallBackHandler(event_id, shareStorageToSpaceWorkResult);
					break;
				}
				case RAILEventID.kRailEventUserSpaceGetMySubscribedWorksResult:
				{
					AsyncGetMySubscribedWorksResult asyncGetMySubscribedWorksResult = new AsyncGetMySubscribedWorksResult();
					RailConverter.Cpp2Csharp(data, asyncGetMySubscribedWorksResult);
					railEventCallBackHandler(event_id, asyncGetMySubscribedWorksResult);
					break;
				}
				case RAILEventID.kRailEventUsersShowChatWindowWithFriendResult:
				{
					RailShowChatWindowWithFriendResult railShowChatWindowWithFriendResult = new RailShowChatWindowWithFriendResult();
					RailConverter.Cpp2Csharp(data, railShowChatWindowWithFriendResult);
					railEventCallBackHandler(event_id, railShowChatWindowWithFriendResult);
					break;
				}
				case RAILEventID.kRailEventBrowserTitleChanged:
				{
					BrowserRenderTitleChanged browserRenderTitleChanged = new BrowserRenderTitleChanged();
					RailConverter.Cpp2Csharp(data, browserRenderTitleChanged);
					railEventCallBackHandler(event_id, browserRenderTitleChanged);
					break;
				}
				case RAILEventID.kRailEventStorageAsyncReadStreamFileResult:
				{
					AsyncReadStreamFileResult asyncReadStreamFileResult = new AsyncReadStreamFileResult();
					RailConverter.Cpp2Csharp(data, asyncReadStreamFileResult);
					railEventCallBackHandler(event_id, asyncReadStreamFileResult);
					break;
				}
				case RAILEventID.kRailEventRoomSetNewRoomOwnerResult:
				{
					SetNewRoomOwnerResult setNewRoomOwnerResult = new SetNewRoomOwnerResult();
					RailConverter.Cpp2Csharp(data, setNewRoomOwnerResult);
					railEventCallBackHandler(event_id, setNewRoomOwnerResult);
					break;
				}
				case RAILEventID.kRailEventRoomGetRoomListResult:
				{
					GetRoomListResult getRoomListResult = new GetRoomListResult();
					RailConverter.Cpp2Csharp(data, getRoomListResult);
					railEventCallBackHandler(event_id, getRoomListResult);
					break;
				}
				case RAILEventID.kRailEventRoomSetRoomTagResult:
				{
					SetRoomTagResult setRoomTagResult = new SetRoomTagResult();
					RailConverter.Cpp2Csharp(data, setRoomTagResult);
					railEventCallBackHandler(event_id, setRoomTagResult);
					break;
				}
				case RAILEventID.kRailEventUserSpaceQuerySpaceWorksInfoResult:
				{
					AsyncQuerySpaceWorksInfoResult asyncQuerySpaceWorksInfoResult = new AsyncQuerySpaceWorksInfoResult();
					RailConverter.Cpp2Csharp(data, asyncQuerySpaceWorksInfoResult);
					railEventCallBackHandler(event_id, asyncQuerySpaceWorksInfoResult);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelDataCaptured:
				{
					VoiceDataCapturedEvent voiceDataCapturedEvent = new VoiceDataCapturedEvent();
					RailConverter.Cpp2Csharp(data, voiceDataCapturedEvent);
					railEventCallBackHandler(event_id, voiceDataCapturedEvent);
					break;
				}
				case RAILEventID.kRailEventRoomOpenRoomResult:
				{
					OpenRoomResult openRoomResult = new OpenRoomResult();
					RailConverter.Cpp2Csharp(data, openRoomResult);
					railEventCallBackHandler(event_id, openRoomResult);
					break;
				}
				case RAILEventID.kRailEventInGameStorePurchasePayWindowClosed:
				{
					RailInGameStorePurchasePayWindowClosed railInGameStorePurchasePayWindowClosed = new RailInGameStorePurchasePayWindowClosed();
					RailConverter.Cpp2Csharp(data, railInGameStorePurchasePayWindowClosed);
					railEventCallBackHandler(event_id, railInGameStorePurchasePayWindowClosed);
					break;
				}
				case RAILEventID.kRailEventLeaderboardUploaded:
				{
					LeaderboardUploaded leaderboardUploaded = new LeaderboardUploaded();
					RailConverter.Cpp2Csharp(data, leaderboardUploaded);
					railEventCallBackHandler(event_id, leaderboardUploaded);
					break;
				}
				case RAILEventID.kRailEventGroupChatQueryGroupsInfoResult:
				{
					RailQueryGroupsInfoResult railQueryGroupsInfoResult = new RailQueryGroupsInfoResult();
					RailConverter.Cpp2Csharp(data, railQueryGroupsInfoResult);
					railEventCallBackHandler(event_id, railQueryGroupsInfoResult);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelLeaveResult:
				{
					LeaveVoiceChannelResult leaveVoiceChannelResult = new LeaveVoiceChannelResult();
					RailConverter.Cpp2Csharp(data, leaveVoiceChannelResult);
					railEventCallBackHandler(event_id, leaveVoiceChannelResult);
					break;
				}
				case RAILEventID.kRailEventPlayerGetAuthenticateURL:
				{
					GetAuthenticateURLResult getAuthenticateURLResult = new GetAuthenticateURLResult();
					RailConverter.Cpp2Csharp(data, getAuthenticateURLResult);
					railEventCallBackHandler(event_id, getAuthenticateURLResult);
					break;
				}
				case RAILEventID.kRailEventRoomGetRoomMembersResult:
				{
					GetRoomMembersResult getRoomMembersResult = new GetRoomMembersResult();
					RailConverter.Cpp2Csharp(data, getRoomMembersResult);
					railEventCallBackHandler(event_id, getRoomMembersResult);
					break;
				}
				case RAILEventID.kRailEventInGameActivityOpenGameActivityWindowResult:
				{
					RailOpenGameActivityWindowResult railOpenGameActivityWindowResult = new RailOpenGameActivityWindowResult();
					RailConverter.Cpp2Csharp(data, railOpenGameActivityWindowResult);
					railEventCallBackHandler(event_id, railOpenGameActivityWindowResult);
					break;
				}
				case RAILEventID.kRailEventAssetsCompleteConsumeFinished:
				{
					CompleteConsumeAssetsFinished completeConsumeAssetsFinished = new CompleteConsumeAssetsFinished();
					RailConverter.Cpp2Csharp(data, completeConsumeAssetsFinished);
					railEventCallBackHandler(event_id, completeConsumeAssetsFinished);
					break;
				}
				case RAILEventID.kRailEventStatsPlayerStatsReceived:
				{
					PlayerStatsReceived playerStatsReceived = new PlayerStatsReceived();
					RailConverter.Cpp2Csharp(data, playerStatsReceived);
					railEventCallBackHandler(event_id, playerStatsReceived);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelPushToTalkKeyChangedEvent:
				{
					VoiceChannelPushToTalkKeyChangedEvent voiceChannelPushToTalkKeyChangedEvent = new VoiceChannelPushToTalkKeyChangedEvent();
					RailConverter.Cpp2Csharp(data, voiceChannelPushToTalkKeyChangedEvent);
					railEventCallBackHandler(event_id, voiceChannelPushToTalkKeyChangedEvent);
					break;
				}
				case RAILEventID.kRailEventFriendsGetMetadataResult:
				{
					RailFriendsGetMetadataResult railFriendsGetMetadataResult = new RailFriendsGetMetadataResult();
					RailConverter.Cpp2Csharp(data, railFriendsGetMetadataResult);
					railEventCallBackHandler(event_id, railFriendsGetMetadataResult);
					break;
				}
				case RAILEventID.kRailEventRoomGetMemberMetadataResult:
				{
					GetMemberMetadataResult getMemberMetadataResult = new GetMemberMetadataResult();
					RailConverter.Cpp2Csharp(data, getMemberMetadataResult);
					railEventCallBackHandler(event_id, getMemberMetadataResult);
					break;
				}
				case RAILEventID.kRailEventUserSpaceRateSpaceWorkResult:
				{
					AsyncRateSpaceWorkResult asyncRateSpaceWorkResult = new AsyncRateSpaceWorkResult();
					RailConverter.Cpp2Csharp(data, asyncRateSpaceWorkResult);
					railEventCallBackHandler(event_id, asyncRateSpaceWorkResult);
					break;
				}
				case RAILEventID.kRailEventAssetsCompleteConsumeByExchangeAssetsToFinished:
				{
					CompleteConsumeByExchangeAssetsToFinished completeConsumeByExchangeAssetsToFinished = new CompleteConsumeByExchangeAssetsToFinished();
					RailConverter.Cpp2Csharp(data, completeConsumeByExchangeAssetsToFinished);
					railEventCallBackHandler(event_id, completeConsumeByExchangeAssetsToFinished);
					break;
				}
				case RAILEventID.kRailEventUsersRespondInvitation:
				{
					RailUsersRespondInvitation railUsersRespondInvitation = new RailUsersRespondInvitation();
					RailConverter.Cpp2Csharp(data, railUsersRespondInvitation);
					railEventCallBackHandler(event_id, railUsersRespondInvitation);
					break;
				}
				case RAILEventID.kRailEventUserSpaceSearchSpaceWorkResult:
				{
					AsyncSearchSpaceWorksResult asyncSearchSpaceWorksResult = new AsyncSearchSpaceWorksResult();
					RailConverter.Cpp2Csharp(data, asyncSearchSpaceWorksResult);
					railEventCallBackHandler(event_id, asyncSearchSpaceWorksResult);
					break;
				}
				case RAILEventID.kRailEventStatsGlobalStatsReceived:
				{
					GlobalStatsRequestReceived globalStatsRequestReceived = new GlobalStatsRequestReceived();
					RailConverter.Cpp2Csharp(data, globalStatsRequestReceived);
					railEventCallBackHandler(event_id, globalStatsRequestReceived);
					break;
				}
				case RAILEventID.kRailEventAssetsExchangeAssetsToFinished:
				{
					ExchangeAssetsToFinished exchangeAssetsToFinished = new ExchangeAssetsToFinished();
					RailConverter.Cpp2Csharp(data, exchangeAssetsToFinished);
					railEventCallBackHandler(event_id, exchangeAssetsToFinished);
					break;
				}
				case RAILEventID.kRailEventInGameActivityQueryGameActivityResult:
				{
					RailQueryGameActivityResult railQueryGameActivityResult = new RailQueryGameActivityResult();
					RailConverter.Cpp2Csharp(data, railQueryGameActivityResult);
					railEventCallBackHandler(event_id, railQueryGameActivityResult);
					break;
				}
				case RAILEventID.kRailEventUserSpaceVoteSpaceWorkResult:
				{
					AsyncVoteSpaceWorkResult asyncVoteSpaceWorkResult = new AsyncVoteSpaceWorkResult();
					RailConverter.Cpp2Csharp(data, asyncVoteSpaceWorkResult);
					railEventCallBackHandler(event_id, asyncVoteSpaceWorkResult);
					break;
				}
				case RAILEventID.kRailEventStatsNumOfPlayerReceived:
				{
					NumberOfPlayerReceived numberOfPlayerReceived = new NumberOfPlayerReceived();
					RailConverter.Cpp2Csharp(data, numberOfPlayerReceived);
					railEventCallBackHandler(event_id, numberOfPlayerReceived);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelUsersSpeakingStateChangedEvent:
				{
					VoiceChannelUsersSpeakingStateChangedEvent voiceChannelUsersSpeakingStateChangedEvent = new VoiceChannelUsersSpeakingStateChangedEvent();
					RailConverter.Cpp2Csharp(data, voiceChannelUsersSpeakingStateChangedEvent);
					railEventCallBackHandler(event_id, voiceChannelUsersSpeakingStateChangedEvent);
					break;
				}
				case RAILEventID.kRailEventFriendsGetInviteCommandLine:
				{
					RailFriendsGetInviteCommandLine railFriendsGetInviteCommandLine = new RailFriendsGetInviteCommandLine();
					RailConverter.Cpp2Csharp(data, railFriendsGetInviteCommandLine);
					railEventCallBackHandler(event_id, railFriendsGetInviteCommandLine);
					break;
				}
				case RAILEventID.kRailEventBrowserNavigeteResult:
				{
					BrowserRenderNavigateResult browserRenderNavigateResult = new BrowserRenderNavigateResult();
					RailConverter.Cpp2Csharp(data, browserRenderNavigateResult);
					railEventCallBackHandler(event_id, browserRenderNavigateResult);
					break;
				}
				case RAILEventID.kRailEventDlcOwnershipChanged:
				{
					DlcOwnershipChanged dlcOwnershipChanged = new DlcOwnershipChanged();
					RailConverter.Cpp2Csharp(data, dlcOwnershipChanged);
					railEventCallBackHandler(event_id, dlcOwnershipChanged);
					break;
				}
				case RAILEventID.kRailEventStorageQueryQuotaResult:
				{
					AsyncQueryQuotaResult asyncQueryQuotaResult = new AsyncQueryQuotaResult();
					RailConverter.Cpp2Csharp(data, asyncQueryQuotaResult);
					railEventCallBackHandler(event_id, asyncQueryQuotaResult);
					break;
				}
				case RAILEventID.kRailEventRoomCreated:
				{
					CreateRoomResult createRoomResult = new CreateRoomResult();
					RailConverter.Cpp2Csharp(data, createRoomResult);
					railEventCallBackHandler(event_id, createRoomResult);
					break;
				}
				case RAILEventID.kRailEventRoomLeaveRoomResult:
				{
					LeaveRoomResult leaveRoomResult = new LeaveRoomResult();
					RailConverter.Cpp2Csharp(data, leaveRoomResult);
					railEventCallBackHandler(event_id, leaveRoomResult);
					break;
				}
				case RAILEventID.kRailEventAchievementPlayerAchievementReceived:
				{
					PlayerAchievementReceived playerAchievementReceived = new PlayerAchievementReceived();
					RailConverter.Cpp2Csharp(data, playerAchievementReceived);
					railEventCallBackHandler(event_id, playerAchievementReceived);
					break;
				}
				case RAILEventID.kRailEventBrowserJavascriptEvent:
				{
					JavascriptEventResult javascriptEventResult = new JavascriptEventResult();
					RailConverter.Cpp2Csharp(data, javascriptEventResult);
					railEventCallBackHandler(event_id, javascriptEventResult);
					break;
				}
				case RAILEventID.kRailEventGameServerListResult:
				{
					GetGameServerListResult getGameServerListResult = new GetGameServerListResult();
					RailConverter.Cpp2Csharp(data, getGameServerListResult);
					railEventCallBackHandler(event_id, getGameServerListResult);
					break;
				}
				case RAILEventID.kRailEventAssetsStartConsumeFinished:
				{
					StartConsumeAssetsFinished startConsumeAssetsFinished = new StartConsumeAssetsFinished();
					RailConverter.Cpp2Csharp(data, startConsumeAssetsFinished);
					railEventCallBackHandler(event_id, startConsumeAssetsFinished);
					break;
				}
				case RAILEventID.kRailEventInGamePurchaseFinishOrderResult:
				{
					RailInGamePurchaseFinishOrderResponse railInGamePurchaseFinishOrderResponse = new RailInGamePurchaseFinishOrderResponse();
					RailConverter.Cpp2Csharp(data, railInGamePurchaseFinishOrderResponse);
					railEventCallBackHandler(event_id, railInGamePurchaseFinishOrderResponse);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelJoinedResult:
				{
					JoinVoiceChannelResult joinVoiceChannelResult = new JoinVoiceChannelResult();
					RailConverter.Cpp2Csharp(data, joinVoiceChannelResult);
					railEventCallBackHandler(event_id, joinVoiceChannelResult);
					break;
				}
				case RAILEventID.kRailEventPlayerGetPlayerMetadataResult:
				{
					RailGetPlayerMetadataResult railGetPlayerMetadataResult = new RailGetPlayerMetadataResult();
					RailConverter.Cpp2Csharp(data, railGetPlayerMetadataResult);
					railEventCallBackHandler(event_id, railGetPlayerMetadataResult);
					break;
				}
				case RAILEventID.kRailEventAssetsUpdateConsumeFinished:
				{
					UpdateConsumeAssetsFinished updateConsumeAssetsFinished = new UpdateConsumeAssetsFinished();
					RailConverter.Cpp2Csharp(data, updateConsumeAssetsFinished);
					railEventCallBackHandler(event_id, updateConsumeAssetsFinished);
					break;
				}
				case RAILEventID.kRailEventRoomSetRoomMaxMemberResult:
				{
					SetRoomMaxMemberResult setRoomMaxMemberResult = new SetRoomMaxMemberResult();
					RailConverter.Cpp2Csharp(data, setRoomMaxMemberResult);
					railEventCallBackHandler(event_id, setRoomMaxMemberResult);
					break;
				}
				case RAILEventID.kRailEventUserSpaceUpdateMetadataResult:
				{
					AsyncUpdateMetadataResult asyncUpdateMetadataResult = new AsyncUpdateMetadataResult();
					RailConverter.Cpp2Csharp(data, asyncUpdateMetadataResult);
					railEventCallBackHandler(event_id, asyncUpdateMetadataResult);
					break;
				}
				case RAILEventID.kRailEventFriendsOnlineStateChanged:
				{
					RailFriendsOnlineStateChanged railFriendsOnlineStateChanged = new RailFriendsOnlineStateChanged();
					RailConverter.Cpp2Csharp(data, railFriendsOnlineStateChanged);
					railEventCallBackHandler(event_id, railFriendsOnlineStateChanged);
					break;
				}
				case RAILEventID.kRailEventAntiAddictionQueryGameOnlineTimeResult:
				{
					RailQueryGameOnlineTimeResult railQueryGameOnlineTimeResult = new RailQueryGameOnlineTimeResult();
					RailConverter.Cpp2Csharp(data, railQueryGameOnlineTimeResult);
					railEventCallBackHandler(event_id, railQueryGameOnlineTimeResult);
					break;
				}
				case RAILEventID.kRailEventUserSpaceDownloadProgress:
				{
					UserSpaceDownloadProgress userSpaceDownloadProgress = new UserSpaceDownloadProgress();
					RailConverter.Cpp2Csharp(data, userSpaceDownloadProgress);
					railEventCallBackHandler(event_id, userSpaceDownloadProgress);
					break;
				}
				case RAILEventID.kRailEventGameServerSetMetadataResult:
				{
					SetGameServerMetadataResult setGameServerMetadataResult = new SetGameServerMetadataResult();
					RailConverter.Cpp2Csharp(data, setGameServerMetadataResult);
					railEventCallBackHandler(event_id, setGameServerMetadataResult);
					break;
				}
				case RAILEventID.kRailEventTextInputShowTextInputWindowResult:
				{
					RailTextInputResult railTextInputResult = new RailTextInputResult();
					RailConverter.Cpp2Csharp(data, railTextInputResult);
					railEventCallBackHandler(event_id, railTextInputResult);
					break;
				}
				case RAILEventID.kRailEventRoomNotifyMetadataChanged:
				{
					NotifyMetadataChange notifyMetadataChange = new NotifyMetadataChange();
					RailConverter.Cpp2Csharp(data, notifyMetadataChange);
					railEventCallBackHandler(event_id, notifyMetadataChange);
					break;
				}
				case RAILEventID.kRailEventIMEHelperTextInputSelectedResult:
				{
					RailIMEHelperTextInputSelectedResult railIMEHelperTextInputSelectedResult = new RailIMEHelperTextInputSelectedResult();
					RailConverter.Cpp2Csharp(data, railIMEHelperTextInputSelectedResult);
					railEventCallBackHandler(event_id, railIMEHelperTextInputSelectedResult);
					break;
				}
				case RAILEventID.kRailEventNetworkCreateRawSessionRequest:
				{
					NetworkCreateRawSessionRequest networkCreateRawSessionRequest = new NetworkCreateRawSessionRequest();
					RailConverter.Cpp2Csharp(data, networkCreateRawSessionRequest);
					railEventCallBackHandler(event_id, networkCreateRawSessionRequest);
					break;
				}
				case RAILEventID.kRailEventRoomSetMemberMetadataResult:
				{
					SetMemberMetadataResult setMemberMetadataResult = new SetMemberMetadataResult();
					RailConverter.Cpp2Csharp(data, setMemberMetadataResult);
					railEventCallBackHandler(event_id, setMemberMetadataResult);
					break;
				}
				case RAILEventID.kRailEventInGameStorePurchasePayWindowDisplayed:
				{
					RailInGameStorePurchasePayWindowDisplayed railInGameStorePurchasePayWindowDisplayed = new RailInGameStorePurchasePayWindowDisplayed();
					RailConverter.Cpp2Csharp(data, railInGameStorePurchasePayWindowDisplayed);
					railEventCallBackHandler(event_id, railInGameStorePurchasePayWindowDisplayed);
					break;
				}
				case RAILEventID.kRailEventUsersShowUserHomepageWindowResult:
				{
					RailShowUserHomepageWindowResult railShowUserHomepageWindowResult = new RailShowUserHomepageWindowResult();
					RailConverter.Cpp2Csharp(data, railShowUserHomepageWindowResult);
					railEventCallBackHandler(event_id, railShowUserHomepageWindowResult);
					break;
				}
				case RAILEventID.kRailThirdPartyAccountLoginNotifyQrCodeInfo:
				{
					RailNotifyThirdPartyAccountQrCodeInfo railNotifyThirdPartyAccountQrCodeInfo = new RailNotifyThirdPartyAccountQrCodeInfo();
					RailConverter.Cpp2Csharp(data, railNotifyThirdPartyAccountQrCodeInfo);
					railEventCallBackHandler(event_id, railNotifyThirdPartyAccountQrCodeInfo);
					break;
				}
				case RAILEventID.kRailEventInGameCoinRequestCoinInfoResult:
				{
					RailInGameCoinRequestCoinInfoResponse railInGameCoinRequestCoinInfoResponse = new RailInGameCoinRequestCoinInfoResponse();
					RailConverter.Cpp2Csharp(data, railInGameCoinRequestCoinInfoResponse);
					railEventCallBackHandler(event_id, railInGameCoinRequestCoinInfoResponse);
					break;
				}
				case RAILEventID.kRailEventInGamePurchasePurchaseProductsToAssetsResult:
				{
					RailInGamePurchasePurchaseProductsToAssetsResponse railInGamePurchasePurchaseProductsToAssetsResponse = new RailInGamePurchasePurchaseProductsToAssetsResponse();
					RailConverter.Cpp2Csharp(data, railInGamePurchasePurchaseProductsToAssetsResponse);
					railEventCallBackHandler(event_id, railInGamePurchasePurchaseProductsToAssetsResponse);
					break;
				}
				case RAILEventID.kRailEventShowFloatingNotifyWindow:
				{
					ShowNotifyWindow showNotifyWindow = new ShowNotifyWindow();
					RailConverter.Cpp2Csharp(data, showNotifyWindow);
					railEventCallBackHandler(event_id, showNotifyWindow);
					break;
				}
				case RAILEventID.kRailEventRoomGetRoomMetadataResult:
				{
					GetRoomMetadataResult getRoomMetadataResult = new GetRoomMetadataResult();
					RailConverter.Cpp2Csharp(data, getRoomMetadataResult);
					railEventCallBackHandler(event_id, getRoomMetadataResult);
					break;
				}
				case RAILEventID.kRailEventScreenshotTakeScreenshotFinished:
				{
					TakeScreenshotResult takeScreenshotResult = new TakeScreenshotResult();
					RailConverter.Cpp2Csharp(data, takeScreenshotResult);
					railEventCallBackHandler(event_id, takeScreenshotResult);
					break;
				}
				case RAILEventID.kRailEventDlcCheckAllDlcsStateReadyResult:
				{
					CheckAllDlcsStateReadyResult checkAllDlcsStateReadyResult = new CheckAllDlcsStateReadyResult();
					RailConverter.Cpp2Csharp(data, checkAllDlcsStateReadyResult);
					railEventCallBackHandler(event_id, checkAllDlcsStateReadyResult);
					break;
				}
				case RAILEventID.kRailEventInGameStorePurchasePaymentResult:
				{
					RailInGameStorePurchaseResult railInGameStorePurchaseResult = new RailInGameStorePurchaseResult();
					RailConverter.Cpp2Csharp(data, railInGameStorePurchaseResult);
					railEventCallBackHandler(event_id, railInGameStorePurchaseResult);
					break;
				}
				case RAILEventID.kRailEventGameServerRegisterToServerListResult:
				{
					GameServerRegisterToServerListResult gameServerRegisterToServerListResult = new GameServerRegisterToServerListResult();
					RailConverter.Cpp2Csharp(data, gameServerRegisterToServerListResult);
					railEventCallBackHandler(event_id, gameServerRegisterToServerListResult);
					break;
				}
				case RAILEventID.kRailEventFriendsQueryPlayedWithFriendsTimeResult:
				{
					RailFriendsQueryPlayedWithFriendsTimeResult railFriendsQueryPlayedWithFriendsTimeResult = new RailFriendsQueryPlayedWithFriendsTimeResult();
					RailConverter.Cpp2Csharp(data, railFriendsQueryPlayedWithFriendsTimeResult);
					railEventCallBackHandler(event_id, railFriendsQueryPlayedWithFriendsTimeResult);
					break;
				}
				case RAILEventID.kRailEventBrowserReloadResult:
				{
					ReloadBrowserResult reloadBrowserResult = new ReloadBrowserResult();
					RailConverter.Cpp2Csharp(data, reloadBrowserResult);
					railEventCallBackHandler(event_id, reloadBrowserResult);
					break;
				}
				case RAILEventID.kRailEventGameServerAuthSessionTicket:
				{
					GameServerStartSessionWithPlayerResponse gameServerStartSessionWithPlayerResponse = new GameServerStartSessionWithPlayerResponse();
					RailConverter.Cpp2Csharp(data, gameServerStartSessionWithPlayerResponse);
					railEventCallBackHandler(event_id, gameServerStartSessionWithPlayerResponse);
					break;
				}
				case RAILEventID.kRailEventRoomJoinRoomResult:
				{
					JoinRoomResult joinRoomResult = new JoinRoomResult();
					RailConverter.Cpp2Csharp(data, joinRoomResult);
					railEventCallBackHandler(event_id, joinRoomResult);
					break;
				}
				case RAILEventID.kRailEventRoomNotifyMemberkicked:
				{
					NotifyRoomMemberKicked notifyRoomMemberKicked = new NotifyRoomMemberKicked();
					RailConverter.Cpp2Csharp(data, notifyRoomMemberKicked);
					railEventCallBackHandler(event_id, notifyRoomMemberKicked);
					break;
				}
				case RAILEventID.kRailEventAssetsMergeFinished:
				{
					MergeAssetsFinished mergeAssetsFinished = new MergeAssetsFinished();
					RailConverter.Cpp2Csharp(data, mergeAssetsFinished);
					railEventCallBackHandler(event_id, mergeAssetsFinished);
					break;
				}
				case RAILEventID.kRailEventRoomGetRoomTagResult:
				{
					GetRoomTagResult getRoomTagResult = new GetRoomTagResult();
					RailConverter.Cpp2Csharp(data, getRoomTagResult);
					railEventCallBackHandler(event_id, getRoomTagResult);
					break;
				}
				case RAILEventID.kRailEventFriendsAddFriendResult:
				{
					RailFriendsAddFriendResult railFriendsAddFriendResult = new RailFriendsAddFriendResult();
					RailConverter.Cpp2Csharp(data, railFriendsAddFriendResult);
					railEventCallBackHandler(event_id, railFriendsAddFriendResult);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelMemberChangedEvent:
				{
					VoiceChannelMemeberChangedEvent voiceChannelMemeberChangedEvent = new VoiceChannelMemeberChangedEvent();
					RailConverter.Cpp2Csharp(data, voiceChannelMemeberChangedEvent);
					railEventCallBackHandler(event_id, voiceChannelMemeberChangedEvent);
					break;
				}
				case RAILEventID.kRailEventStorageAsyncListStreamFileResult:
				{
					AsyncListFileResult asyncListFileResult = new AsyncListFileResult();
					RailConverter.Cpp2Csharp(data, asyncListFileResult);
					railEventCallBackHandler(event_id, asyncListFileResult);
					break;
				}
				case RAILEventID.kRailEventFriendsGetFriendPlayedGamesResult:
				{
					RailFriendsQueryFriendPlayedGamesResult railFriendsQueryFriendPlayedGamesResult = new RailFriendsQueryFriendPlayedGamesResult();
					RailConverter.Cpp2Csharp(data, railFriendsQueryFriendPlayedGamesResult);
					railEventCallBackHandler(event_id, railFriendsQueryFriendPlayedGamesResult);
					break;
				}
				case RAILEventID.kRailEventInGamePurchaseAllPurchasableProductsInfoReceived:
				{
					RailInGamePurchaseRequestAllPurchasableProductsResponse railInGamePurchaseRequestAllPurchasableProductsResponse = new RailInGamePurchaseRequestAllPurchasableProductsResponse();
					RailConverter.Cpp2Csharp(data, railInGamePurchaseRequestAllPurchasableProductsResponse);
					railEventCallBackHandler(event_id, railInGamePurchaseRequestAllPurchasableProductsResponse);
					break;
				}
				case RAILEventID.kRailEventAssetsExchangeAssetsFinished:
				{
					ExchangeAssetsFinished exchangeAssetsFinished = new ExchangeAssetsFinished();
					RailConverter.Cpp2Csharp(data, exchangeAssetsFinished);
					railEventCallBackHandler(event_id, exchangeAssetsFinished);
					break;
				}
				case RAILEventID.kRailEventBrowserPaint:
				{
					BrowserNeedsPaintRequest browserNeedsPaintRequest = new BrowserNeedsPaintRequest();
					RailConverter.Cpp2Csharp(data, browserNeedsPaintRequest);
					railEventCallBackHandler(event_id, browserNeedsPaintRequest);
					break;
				}
				case RAILEventID.kRailEventRoomNotifyRoomDestroyed:
				{
					NotifyRoomDestroy notifyRoomDestroy = new NotifyRoomDestroy();
					RailConverter.Cpp2Csharp(data, notifyRoomDestroy);
					railEventCallBackHandler(event_id, notifyRoomDestroy);
					break;
				}
				case RAILEventID.kRailEventUserSpaceGetMyFavoritesWorksResult:
				{
					AsyncGetMyFavoritesWorksResult asyncGetMyFavoritesWorksResult = new AsyncGetMyFavoritesWorksResult();
					RailConverter.Cpp2Csharp(data, asyncGetMyFavoritesWorksResult);
					railEventCallBackHandler(event_id, asyncGetMyFavoritesWorksResult);
					break;
				}
				case RAILEventID.kRailEventStorageAsyncWriteFileResult:
				{
					AsyncWriteFileResult asyncWriteFileResult = new AsyncWriteFileResult();
					RailConverter.Cpp2Csharp(data, asyncWriteFileResult);
					railEventCallBackHandler(event_id, asyncWriteFileResult);
					break;
				}
				case RAILEventID.kRailEventAssetsDirectConsumeFinished:
				{
					DirectConsumeAssetsFinished directConsumeAssetsFinished = new DirectConsumeAssetsFinished();
					RailConverter.Cpp2Csharp(data, directConsumeAssetsFinished);
					railEventCallBackHandler(event_id, directConsumeAssetsFinished);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelCreateResult:
				{
					CreateVoiceChannelResult createVoiceChannelResult = new CreateVoiceChannelResult();
					RailConverter.Cpp2Csharp(data, createVoiceChannelResult);
					railEventCallBackHandler(event_id, createVoiceChannelResult);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelSpeakingUsersChangedEvent:
				{
					VoiceChannelSpeakingUsersChangedEvent voiceChannelSpeakingUsersChangedEvent = new VoiceChannelSpeakingUsersChangedEvent();
					RailConverter.Cpp2Csharp(data, voiceChannelSpeakingUsersChangedEvent);
					railEventCallBackHandler(event_id, voiceChannelSpeakingUsersChangedEvent);
					break;
				}
				case RAILEventID.kRailEventUserSpaceModifyFavoritesWorksResult:
				{
					AsyncModifyFavoritesWorksResult asyncModifyFavoritesWorksResult = new AsyncModifyFavoritesWorksResult();
					RailConverter.Cpp2Csharp(data, asyncModifyFavoritesWorksResult);
					railEventCallBackHandler(event_id, asyncModifyFavoritesWorksResult);
					break;
				}
				case RAILEventID.kRailEventPlayerAntiAddictionGameOnlineTimeChanged:
				{
					RailAntiAddictionGameOnlineTimeChanged railAntiAddictionGameOnlineTimeChanged = new RailAntiAddictionGameOnlineTimeChanged();
					RailConverter.Cpp2Csharp(data, railAntiAddictionGameOnlineTimeChanged);
					railEventCallBackHandler(event_id, railAntiAddictionGameOnlineTimeChanged);
					break;
				}
				case RAILEventID.kRailPlatformNotifyEventJoinGameByRoom:
				{
					RailPlatformNotifyEventJoinGameByRoom railPlatformNotifyEventJoinGameByRoom = new RailPlatformNotifyEventJoinGameByRoom();
					RailConverter.Cpp2Csharp(data, railPlatformNotifyEventJoinGameByRoom);
					railEventCallBackHandler(event_id, railPlatformNotifyEventJoinGameByRoom);
					break;
				}
				case RAILEventID.kRailEventPlayerGetGamePurchaseKey:
				{
					PlayerGetGamePurchaseKeyResult playerGetGamePurchaseKeyResult = new PlayerGetGamePurchaseKeyResult();
					RailConverter.Cpp2Csharp(data, playerGetGamePurchaseKeyResult);
					railEventCallBackHandler(event_id, playerGetGamePurchaseKeyResult);
					break;
				}
				case RAILEventID.kRailEventZoneServerSwitchPlayerSelectedZoneResult:
				{
					RailSwitchPlayerSelectedZoneResult railSwitchPlayerSelectedZoneResult = new RailSwitchPlayerSelectedZoneResult();
					RailConverter.Cpp2Csharp(data, railSwitchPlayerSelectedZoneResult);
					railEventCallBackHandler(event_id, railSwitchPlayerSelectedZoneResult);
					break;
				}
				case RAILEventID.kRailEventStorageAsyncReadFileResult:
				{
					AsyncReadFileResult asyncReadFileResult = new AsyncReadFileResult();
					RailConverter.Cpp2Csharp(data, asyncReadFileResult);
					railEventCallBackHandler(event_id, asyncReadFileResult);
					break;
				}
				case RAILEventID.kRailEventBrowserDamageRectPaint:
				{
					BrowserDamageRectNeedsPaintRequest browserDamageRectNeedsPaintRequest = new BrowserDamageRectNeedsPaintRequest();
					RailConverter.Cpp2Csharp(data, browserDamageRectNeedsPaintRequest);
					railEventCallBackHandler(event_id, browserDamageRectNeedsPaintRequest);
					break;
				}
				case RAILEventID.kRailThirdPartyAccountLoginResult:
				{
					RailThirdPartyAccountLoginResult railThirdPartyAccountLoginResult = new RailThirdPartyAccountLoginResult();
					RailConverter.Cpp2Csharp(data, railThirdPartyAccountLoginResult);
					railEventCallBackHandler(event_id, railThirdPartyAccountLoginResult);
					break;
				}
				case RAILEventID.kRailEventDlcInstallStartResult:
				{
					DlcInstallStartResult dlcInstallStartResult = new DlcInstallStartResult();
					RailConverter.Cpp2Csharp(data, dlcInstallStartResult);
					railEventCallBackHandler(event_id, dlcInstallStartResult);
					break;
				}
				case RAILEventID.kRailEventUsersCancelInviteResult:
				{
					RailUsersCancelInviteResult railUsersCancelInviteResult = new RailUsersCancelInviteResult();
					RailConverter.Cpp2Csharp(data, railUsersCancelInviteResult);
					railEventCallBackHandler(event_id, railUsersCancelInviteResult);
					break;
				}
				case RAILEventID.kRailEventFinalize:
				{
					RailFinalize railFinalize = new RailFinalize();
					RailConverter.Cpp2Csharp(data, railFinalize);
					railEventCallBackHandler(event_id, railFinalize);
					break;
				}
				case RAILEventID.kRailEventRoomNotifyRoomOwnerChanged:
				{
					NotifyRoomOwnerChange notifyRoomOwnerChange = new NotifyRoomOwnerChange();
					RailConverter.Cpp2Csharp(data, notifyRoomOwnerChange);
					railEventCallBackHandler(event_id, notifyRoomOwnerChange);
					break;
				}
				case RAILEventID.kRailEventShowFloatingWindow:
				{
					ShowFloatingWindowResult showFloatingWindowResult = new ShowFloatingWindowResult();
					RailConverter.Cpp2Csharp(data, showFloatingWindowResult);
					railEventCallBackHandler(event_id, showFloatingWindowResult);
					break;
				}
				case RAILEventID.kRailEventRoomKickOffMemberResult:
				{
					KickOffMemberResult kickOffMemberResult = new KickOffMemberResult();
					RailConverter.Cpp2Csharp(data, kickOffMemberResult);
					railEventCallBackHandler(event_id, kickOffMemberResult);
					break;
				}
				case RAILEventID.kRailEventStorageAsyncRenameStreamFileResult:
				{
					AsyncRenameStreamFileResult asyncRenameStreamFileResult = new AsyncRenameStreamFileResult();
					RailConverter.Cpp2Csharp(data, asyncRenameStreamFileResult);
					railEventCallBackHandler(event_id, asyncRenameStreamFileResult);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelInviteEvent:
				{
					VoiceChannelInviteEvent voiceChannelInviteEvent = new VoiceChannelInviteEvent();
					RailConverter.Cpp2Csharp(data, voiceChannelInviteEvent);
					railEventCallBackHandler(event_id, voiceChannelInviteEvent);
					break;
				}
				case RAILEventID.kRailEventFriendsReportPlayedWithUserListResult:
				{
					RailFriendsReportPlayedWithUserListResult railFriendsReportPlayedWithUserListResult = new RailFriendsReportPlayedWithUserListResult();
					RailConverter.Cpp2Csharp(data, railFriendsReportPlayedWithUserListResult);
					railEventCallBackHandler(event_id, railFriendsReportPlayedWithUserListResult);
					break;
				}
				case RAILEventID.kRailEventStorageAsyncWriteStreamFileResult:
				{
					AsyncWriteStreamFileResult asyncWriteStreamFileResult = new AsyncWriteStreamFileResult();
					RailConverter.Cpp2Csharp(data, asyncWriteStreamFileResult);
					railEventCallBackHandler(event_id, asyncWriteStreamFileResult);
					break;
				}
				case RAILEventID.kRailEventAchievementGlobalAchievementReceived:
				{
					GlobalAchievementReceived globalAchievementReceived = new GlobalAchievementReceived();
					RailConverter.Cpp2Csharp(data, globalAchievementReceived);
					railEventCallBackHandler(event_id, globalAchievementReceived);
					break;
				}
				case RAILEventID.kRailEventUserSpaceQuerySpaceWorksResult:
				{
					AsyncQuerySpaceWorksResult asyncQuerySpaceWorksResult = new AsyncQuerySpaceWorksResult();
					RailConverter.Cpp2Csharp(data, asyncQuerySpaceWorksResult);
					railEventCallBackHandler(event_id, asyncQuerySpaceWorksResult);
					break;
				}
				case RAILEventID.kRailEventUsersGetInviteDetailResult:
				{
					RailUsersGetInviteDetailResult railUsersGetInviteDetailResult = new RailUsersGetInviteDetailResult();
					RailConverter.Cpp2Csharp(data, railUsersGetInviteDetailResult);
					railEventCallBackHandler(event_id, railUsersGetInviteDetailResult);
					break;
				}
				case RAILEventID.kRailEventDlcInstallStart:
				{
					DlcInstallStart dlcInstallStart = new DlcInstallStart();
					RailConverter.Cpp2Csharp(data, dlcInstallStart);
					railEventCallBackHandler(event_id, dlcInstallStart);
					break;
				}
				case RAILEventID.kRailEventGameServerCreated:
				{
					CreateGameServerResult createGameServerResult = new CreateGameServerResult();
					RailConverter.Cpp2Csharp(data, createGameServerResult);
					railEventCallBackHandler(event_id, createGameServerResult);
					break;
				}
				case RAILEventID.kRailEventUtilsGameSettingMetadataChanged:
				{
					RailGameSettingMetadataChanged railGameSettingMetadataChanged = new RailGameSettingMetadataChanged();
					RailConverter.Cpp2Csharp(data, railGameSettingMetadataChanged);
					railEventCallBackHandler(event_id, railGameSettingMetadataChanged);
					break;
				}
				case RAILEventID.kRailEventSmallObjectServiceQueryObjectStateResult:
				{
					RailSmallObjectStateQueryResult railSmallObjectStateQueryResult = new RailSmallObjectStateQueryResult();
					RailConverter.Cpp2Csharp(data, railSmallObjectStateQueryResult);
					railEventCallBackHandler(event_id, railSmallObjectStateQueryResult);
					break;
				}
				case RAILEventID.kRailEventVoiceChannelRemoveUsersResult:
				{
					VoiceChannelRemoveUsersResult voiceChannelRemoveUsersResult = new VoiceChannelRemoveUsersResult();
					RailConverter.Cpp2Csharp(data, voiceChannelRemoveUsersResult);
					railEventCallBackHandler(event_id, voiceChannelRemoveUsersResult);
					break;
				}
				case RAILEventID.kRailEventAchievementPlayerAchievementStored:
				{
					PlayerAchievementStored playerAchievementStored = new PlayerAchievementStored();
					RailConverter.Cpp2Csharp(data, playerAchievementStored);
					railEventCallBackHandler(event_id, playerAchievementStored);
					break;
				}
				case RAILEventID.kRailEventFriendsQueryPlayedWithFriendsGamesResult:
				{
					RailFriendsQueryPlayedWithFriendsGamesResult railFriendsQueryPlayedWithFriendsGamesResult = new RailFriendsQueryPlayedWithFriendsGamesResult();
					RailConverter.Cpp2Csharp(data, railFriendsQueryPlayedWithFriendsGamesResult);
					railEventCallBackHandler(event_id, railFriendsQueryPlayedWithFriendsGamesResult);
					break;
				}
				case RAILEventID.kRailEventAssetsSplitFinished:
				{
					SplitAssetsFinished splitAssetsFinished = new SplitAssetsFinished();
					RailConverter.Cpp2Csharp(data, splitAssetsFinished);
					railEventCallBackHandler(event_id, splitAssetsFinished);
					break;
				}
				case RAILEventID.kRailEventLeaderboardReceived:
				{
					LeaderboardReceived leaderboardReceived = new LeaderboardReceived();
					RailConverter.Cpp2Csharp(data, leaderboardReceived);
					railEventCallBackHandler(event_id, leaderboardReceived);
					break;
				}
				case RAILEventID.kRailEventBrowserTryNavigateNewPageRequest:
				{
					BrowserTryNavigateNewPageRequest browserTryNavigateNewPageRequest = new BrowserTryNavigateNewPageRequest();
					RailConverter.Cpp2Csharp(data, browserTryNavigateNewPageRequest);
					railEventCallBackHandler(event_id, browserTryNavigateNewPageRequest);
					break;
				}
				case RAILEventID.kRailEventRoomGetUserRoomListResult:
				{
					GetUserRoomListResult getUserRoomListResult = new GetUserRoomListResult();
					RailConverter.Cpp2Csharp(data, getUserRoomListResult);
					railEventCallBackHandler(event_id, getUserRoomListResult);
					break;
				}
				case RAILEventID.kRailEventFriendsMetadataChanged:
				{
					RailFriendsMetadataChanged railFriendsMetadataChanged = new RailFriendsMetadataChanged();
					RailConverter.Cpp2Csharp(data, railFriendsMetadataChanged);
					railEventCallBackHandler(event_id, railFriendsMetadataChanged);
					break;
				}
				case RAILEventID.kRailEventLeaderboardAttachSpaceWork:
				{
					LeaderboardAttachSpaceWork leaderboardAttachSpaceWork = new LeaderboardAttachSpaceWork();
					RailConverter.Cpp2Csharp(data, leaderboardAttachSpaceWork);
					railEventCallBackHandler(event_id, leaderboardAttachSpaceWork);
					break;
				}
				case RAILEventID.kRailEventLeaderboardAsyncCreated:
				{
					LeaderboardCreated leaderboardCreated = new LeaderboardCreated();
					RailConverter.Cpp2Csharp(data, leaderboardCreated);
					railEventCallBackHandler(event_id, leaderboardCreated);
					break;
				}
				case RAILEventID.kRailEventFriendsQueryPlayedWithFriendsListResult:
				{
					RailFriendsQueryPlayedWithFriendsListResult railFriendsQueryPlayedWithFriendsListResult = new RailFriendsQueryPlayedWithFriendsListResult();
					RailConverter.Cpp2Csharp(data, railFriendsQueryPlayedWithFriendsListResult);
					railEventCallBackHandler(event_id, railFriendsQueryPlayedWithFriendsListResult);
					break;
				}
				case RAILEventID.kRailEventUsersNotifyInviter:
				{
					RailUsersNotifyInviter railUsersNotifyInviter = new RailUsersNotifyInviter();
					RailConverter.Cpp2Csharp(data, railUsersNotifyInviter);
					railEventCallBackHandler(event_id, railUsersNotifyInviter);
					break;
				}
				case RAILEventID.kRailEventDlcInstallProgress:
				{
					DlcInstallProgress dlcInstallProgress = new DlcInstallProgress();
					RailConverter.Cpp2Csharp(data, dlcInstallProgress);
					railEventCallBackHandler(event_id, dlcInstallProgress);
					break;
				}
				case RAILEventID.kRailPlatformNotifyEventJoinGameByUser:
				{
					RailPlatformNotifyEventJoinGameByUser railPlatformNotifyEventJoinGameByUser = new RailPlatformNotifyEventJoinGameByUser();
					RailConverter.Cpp2Csharp(data, railPlatformNotifyEventJoinGameByUser);
					railEventCallBackHandler(event_id, railPlatformNotifyEventJoinGameByUser);
					break;
				}
				case RAILEventID.kRailEventDlcInstallFinished:
				{
					DlcInstallFinished dlcInstallFinished = new DlcInstallFinished();
					RailConverter.Cpp2Csharp(data, dlcInstallFinished);
					railEventCallBackHandler(event_id, dlcInstallFinished);
					break;
				}
				case RAILEventID.kRailEventNetworkCreateSessionRequest:
				{
					CreateSessionRequest createSessionRequest = new CreateSessionRequest();
					RailConverter.Cpp2Csharp(data, createSessionRequest);
					railEventCallBackHandler(event_id, createSessionRequest);
					break;
				}
				case RAILEventID.kRailEventBrowserCloseResult:
				{
					CloseBrowserResult closeBrowserResult = new CloseBrowserResult();
					RailConverter.Cpp2Csharp(data, closeBrowserResult);
					railEventCallBackHandler(event_id, closeBrowserResult);
					break;
				}
				case RAILEventID.kRailEventRoomSetRoomMetadataResult:
				{
					SetRoomMetadataResult setRoomMetadataResult = new SetRoomMetadataResult();
					RailConverter.Cpp2Csharp(data, setRoomMetadataResult);
					railEventCallBackHandler(event_id, setRoomMetadataResult);
					break;
				}
				case RAILEventID.kRailEventGameServerFavoriteGameServers:
				{
					AsyncGetFavoriteGameServersResult asyncGetFavoriteGameServersResult = new AsyncGetFavoriteGameServersResult();
					RailConverter.Cpp2Csharp(data, asyncGetFavoriteGameServersResult);
					railEventCallBackHandler(event_id, asyncGetFavoriteGameServersResult);
					break;
				}
				case RAILEventID.kRailEventPlayerGetEncryptedGameTicketResult:
				{
					RailGetEncryptedGameTicketResult railGetEncryptedGameTicketResult = new RailGetEncryptedGameTicketResult();
					RailConverter.Cpp2Csharp(data, railGetEncryptedGameTicketResult);
					railEventCallBackHandler(event_id, railGetEncryptedGameTicketResult);
					break;
				}
				case RAILEventID.kRailEventInGamePurchasePurchaseProductsResult:
				{
					RailInGamePurchasePurchaseProductsResponse railInGamePurchasePurchaseProductsResponse = new RailInGamePurchasePurchaseProductsResponse();
					RailConverter.Cpp2Csharp(data, railInGamePurchasePurchaseProductsResponse);
					railEventCallBackHandler(event_id, railInGamePurchasePurchaseProductsResponse);
					break;
				}
				case RAILEventID.kRailEventRoomNotifyMemberChanged:
				{
					NotifyRoomMemberChange notifyRoomMemberChange = new NotifyRoomMemberChange();
					RailConverter.Cpp2Csharp(data, notifyRoomMemberChange);
					railEventCallBackHandler(event_id, notifyRoomMemberChange);
					break;
				}
				case RAILEventID.kRailEventUserSpaceSubscribeResult:
				{
					AsyncSubscribeSpaceWorksResult asyncSubscribeSpaceWorksResult = new AsyncSubscribeSpaceWorksResult();
					RailConverter.Cpp2Csharp(data, asyncSubscribeSpaceWorksResult);
					railEventCallBackHandler(event_id, asyncSubscribeSpaceWorksResult);
					break;
				}
				case RAILEventID.kRailEventAssetsMergeToFinished:
				{
					MergeAssetsToFinished mergeAssetsToFinished = new MergeAssetsToFinished();
					RailConverter.Cpp2Csharp(data, mergeAssetsToFinished);
					railEventCallBackHandler(event_id, mergeAssetsToFinished);
					break;
				}
				case RAILEventID.kRailEventRoomSetRoomTypeResult:
				{
					SetRoomTypeResult setRoomTypeResult = new SetRoomTypeResult();
					RailConverter.Cpp2Csharp(data, setRoomTypeResult);
					railEventCallBackHandler(event_id, setRoomTypeResult);
					break;
				}
				case RAILEventID.kRailEventAssetsRequestAllAssetsFinished:
				{
					RequestAllAssetsFinished requestAllAssetsFinished = new RequestAllAssetsFinished();
					RailConverter.Cpp2Csharp(data, requestAllAssetsFinished);
					railEventCallBackHandler(event_id, requestAllAssetsFinished);
					break;
				}
				}
			}
		}
	}
}
