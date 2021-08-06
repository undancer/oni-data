using System;
using System.Runtime.InteropServices;
using System.Text;

public static class RailAPI
{
	public enum Platform
	{
		TGP = 1,
		QQGame
	}

	public enum Event
	{
		AuthTicketAcquired = 13001,
		EventSystemChanged = 2,
		QueryIsOwnedDlcsResult = 17007,
		CheckAllDlcsStateReadyResult = 17006
	}

	[Serializable]
	public struct AuthTicketResponse
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
		public string ticket;
	}

	[Serializable]
	public struct RailSystemStateChanged
	{
		public bool mRequestExit;
	}

	[Serializable]
	public struct QueryIsOwnedDlcsResult
	{
		public int dlcGameId;

		public bool isOwned;
	}

	[Serializable]
	public struct CheckAllDlcsStateReadyResult
	{
		public bool ready;
	}

	public delegate void EventHandler(Event eventId, IntPtr data);

	[DllImport("RailAPI")]
	public static extern bool RestartAppIfNecessary(ulong gameId);

	[DllImport("RailAPI")]
	public static extern bool Initialize();

	[DllImport("RailAPI")]
	public static extern uint NotifyWindowAntiAddiction();

	[DllImport("RailAPI")]
	public static extern void Shutdown();

	[DllImport("RailAPI")]
	public static extern void FireEvents();

	[DllImport("RailAPI")]
	public static extern void RegisterEventHandler(Event eventId, EventHandler handler);

	[DllImport("RailAPI")]
	public static extern void UnregisterEventHandler(Event eventId, EventHandler handler);

	[DllImport("RailAPI")]
	public static extern void ApplyWordFilter(string strIn, StringBuilder strOut, ref int strOutLen);

	[DllImport("RailAPI")]
	public static extern void GetRailPlatformId(ref int idOut);

	[DllImport("RailAPI")]
	public static extern void GetLocalUserName(StringBuilder strOut, ref int strOutLen);

	[DllImport("RailAPI")]
	public static extern void GetLocalUserId(ref ulong idOut);

	[DllImport("RailAPI")]
	public static extern void RequestAuthTicket();

	[DllImport("RailAPI")]
	public static extern void QueryIsOwnedDlcsOnServer();

	[DllImport("RailAPI")]
	public static extern bool IsOwnedDlc(ulong dlcId);

	[DllImport("RailAPI")]
	public static extern void CheckAllDlcsStateReady(string user_data);
}
