using System;

namespace Steamworks
{
	[Serializable]
	public struct ISteamNetworkingConnectionSignaling
	{
		public bool SendSignal(HSteamNetConnection hConn, ref SteamNetConnectionInfo_t info, IntPtr pMsg, int cbMsg)
		{
			return NativeMethods.SteamAPI_ISteamNetworkingConnectionSignaling_SendSignal(ref this, hConn, ref info, pMsg, cbMsg);
		}

		public void Release()
		{
			NativeMethods.SteamAPI_ISteamNetworkingConnectionSignaling_Release(ref this);
		}
	}
}
