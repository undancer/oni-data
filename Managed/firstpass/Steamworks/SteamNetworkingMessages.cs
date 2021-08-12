using System;

namespace Steamworks
{
	public static class SteamNetworkingMessages
	{
		public static EResult SendMessageToUser(ref SteamNetworkingIdentity identityRemote, IntPtr pubData, uint cubData, int nSendFlags, int nRemoteChannel)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamNetworkingMessages_SendMessageToUser(CSteamAPIContext.GetSteamNetworkingMessages(), ref identityRemote, pubData, cubData, nSendFlags, nRemoteChannel);
		}

		public static int ReceiveMessagesOnChannel(int nLocalChannel, IntPtr[] ppOutMessages, int nMaxMessages)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamNetworkingMessages_ReceiveMessagesOnChannel(CSteamAPIContext.GetSteamNetworkingMessages(), nLocalChannel, ppOutMessages, nMaxMessages);
		}

		public static bool AcceptSessionWithUser(ref SteamNetworkingIdentity identityRemote)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamNetworkingMessages_AcceptSessionWithUser(CSteamAPIContext.GetSteamNetworkingMessages(), ref identityRemote);
		}

		public static bool CloseSessionWithUser(ref SteamNetworkingIdentity identityRemote)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamNetworkingMessages_CloseSessionWithUser(CSteamAPIContext.GetSteamNetworkingMessages(), ref identityRemote);
		}

		public static bool CloseChannelWithUser(ref SteamNetworkingIdentity identityRemote, int nLocalChannel)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamNetworkingMessages_CloseChannelWithUser(CSteamAPIContext.GetSteamNetworkingMessages(), ref identityRemote, nLocalChannel);
		}

		public static ESteamNetworkingConnectionState GetSessionConnectionInfo(ref SteamNetworkingIdentity identityRemote, out SteamNetConnectionInfo_t pConnectionInfo, out SteamNetworkingQuickConnectionStatus pQuickStatus)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamNetworkingMessages_GetSessionConnectionInfo(CSteamAPIContext.GetSteamNetworkingMessages(), ref identityRemote, out pConnectionInfo, out pQuickStatus);
		}
	}
}
