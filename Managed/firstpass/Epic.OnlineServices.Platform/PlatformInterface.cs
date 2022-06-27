using System;
using System.Runtime.InteropServices;
using System.Text;
using Epic.OnlineServices.Achievements;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Ecom;
using Epic.OnlineServices.Friends;
using Epic.OnlineServices.Leaderboards;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.Metrics;
using Epic.OnlineServices.P2P;
using Epic.OnlineServices.PlayerDataStorage;
using Epic.OnlineServices.Presence;
using Epic.OnlineServices.Sessions;
using Epic.OnlineServices.Stats;
using Epic.OnlineServices.TitleStorage;
using Epic.OnlineServices.UI;
using Epic.OnlineServices.UserInfo;

namespace Epic.OnlineServices.Platform
{
	public sealed class PlatformInterface : Handle
	{
		public const int OptionsApiLatest = 8;

		public const int LocalecodeMaxBufferLen = 10;

		public const int LocalecodeMaxLength = 9;

		public const int CountrycodeMaxBufferLen = 5;

		public const int CountrycodeMaxLength = 4;

		public const int InitializeApiLatest = 3;

		public PlatformInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public static Result Initialize(InitializeOptions options)
		{
			InitializeOptionsInternal options2 = Helper.CopyProperties<InitializeOptionsInternal>(options);
			int[] source = new int[2] { 1, 1 };
			IntPtr target = IntPtr.Zero;
			Helper.TryMarshalSet(ref target, source);
			options2.Reserved = target;
			Result source2 = EOS_Initialize(ref options2);
			Helper.TryMarshalDispose(ref options2);
			Helper.TryMarshalDispose(ref target);
			Result target2 = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source2, out target2);
			return target2;
		}

		public static Result Shutdown()
		{
			Result source = EOS_Shutdown();
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public static PlatformInterface Create(Options options)
		{
			OptionsInternal options2 = Helper.CopyProperties<OptionsInternal>(options);
			IntPtr source = EOS_Platform_Create(ref options2);
			Helper.TryMarshalDispose(ref options2);
			PlatformInterface target = Helper.GetDefault<PlatformInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void Release()
		{
			EOS_Platform_Release(base.InnerHandle);
		}

		public void Tick()
		{
			EOS_Platform_Tick(base.InnerHandle);
		}

		public MetricsInterface GetMetricsInterface()
		{
			IntPtr source = EOS_Platform_GetMetricsInterface(base.InnerHandle);
			MetricsInterface target = Helper.GetDefault<MetricsInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public AuthInterface GetAuthInterface()
		{
			IntPtr source = EOS_Platform_GetAuthInterface(base.InnerHandle);
			AuthInterface target = Helper.GetDefault<AuthInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public ConnectInterface GetConnectInterface()
		{
			IntPtr source = EOS_Platform_GetConnectInterface(base.InnerHandle);
			ConnectInterface target = Helper.GetDefault<ConnectInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public EcomInterface GetEcomInterface()
		{
			IntPtr source = EOS_Platform_GetEcomInterface(base.InnerHandle);
			EcomInterface target = Helper.GetDefault<EcomInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public UIInterface GetUIInterface()
		{
			IntPtr source = EOS_Platform_GetUIInterface(base.InnerHandle);
			UIInterface target = Helper.GetDefault<UIInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public FriendsInterface GetFriendsInterface()
		{
			IntPtr source = EOS_Platform_GetFriendsInterface(base.InnerHandle);
			FriendsInterface target = Helper.GetDefault<FriendsInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public PresenceInterface GetPresenceInterface()
		{
			IntPtr source = EOS_Platform_GetPresenceInterface(base.InnerHandle);
			PresenceInterface target = Helper.GetDefault<PresenceInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public SessionsInterface GetSessionsInterface()
		{
			IntPtr source = EOS_Platform_GetSessionsInterface(base.InnerHandle);
			SessionsInterface target = Helper.GetDefault<SessionsInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public LobbyInterface GetLobbyInterface()
		{
			IntPtr source = EOS_Platform_GetLobbyInterface(base.InnerHandle);
			LobbyInterface target = Helper.GetDefault<LobbyInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public UserInfoInterface GetUserInfoInterface()
		{
			IntPtr source = EOS_Platform_GetUserInfoInterface(base.InnerHandle);
			UserInfoInterface target = Helper.GetDefault<UserInfoInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public P2PInterface GetP2PInterface()
		{
			IntPtr source = EOS_Platform_GetP2PInterface(base.InnerHandle);
			P2PInterface target = Helper.GetDefault<P2PInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public PlayerDataStorageInterface GetPlayerDataStorageInterface()
		{
			IntPtr source = EOS_Platform_GetPlayerDataStorageInterface(base.InnerHandle);
			PlayerDataStorageInterface target = Helper.GetDefault<PlayerDataStorageInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public TitleStorageInterface GetTitleStorageInterface()
		{
			IntPtr source = EOS_Platform_GetTitleStorageInterface(base.InnerHandle);
			TitleStorageInterface target = Helper.GetDefault<TitleStorageInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public AchievementsInterface GetAchievementsInterface()
		{
			IntPtr source = EOS_Platform_GetAchievementsInterface(base.InnerHandle);
			AchievementsInterface target = Helper.GetDefault<AchievementsInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public StatsInterface GetStatsInterface()
		{
			IntPtr source = EOS_Platform_GetStatsInterface(base.InnerHandle);
			StatsInterface target = Helper.GetDefault<StatsInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public LeaderboardsInterface GetLeaderboardsInterface()
		{
			IntPtr source = EOS_Platform_GetLeaderboardsInterface(base.InnerHandle);
			LeaderboardsInterface target = Helper.GetDefault<LeaderboardsInterface>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetActiveCountryCode(EpicAccountId localUserId, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result source = EOS_Platform_GetActiveCountryCode(base.InnerHandle, localUserId.InnerHandle, outBuffer, ref inOutBufferLength);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetActiveLocaleCode(EpicAccountId localUserId, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result source = EOS_Platform_GetActiveLocaleCode(base.InnerHandle, localUserId.InnerHandle, outBuffer, ref inOutBufferLength);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetOverrideCountryCode(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result source = EOS_Platform_GetOverrideCountryCode(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetOverrideLocaleCode(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result source = EOS_Platform_GetOverrideLocaleCode(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetOverrideCountryCode(string newCountryCode)
		{
			Result source = EOS_Platform_SetOverrideCountryCode(base.InnerHandle, newCountryCode);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result SetOverrideLocaleCode(string newLocaleCode)
		{
			Result source = EOS_Platform_SetOverrideLocaleCode(base.InnerHandle, newLocaleCode);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CheckForLauncherAndRestart()
		{
			Result source = EOS_Platform_CheckForLauncherAndRestart(base.InnerHandle);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Platform_CheckForLauncherAndRestart(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Platform_SetOverrideLocaleCode(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string newLocaleCode);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Platform_SetOverrideCountryCode(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string newCountryCode);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Platform_GetOverrideLocaleCode(IntPtr handle, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Platform_GetOverrideCountryCode(IntPtr handle, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Platform_GetActiveLocaleCode(IntPtr handle, IntPtr localUserId, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Platform_GetActiveCountryCode(IntPtr handle, IntPtr localUserId, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetLeaderboardsInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetStatsInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetAchievementsInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetTitleStorageInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetPlayerDataStorageInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetP2PInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetUserInfoInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetLobbyInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetSessionsInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetPresenceInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetFriendsInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetUIInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetEcomInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetConnectInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetAuthInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_GetMetricsInterface(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Platform_Tick(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_Platform_Release(IntPtr handle);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_Platform_Create(ref OptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Shutdown();

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_Initialize(ref InitializeOptionsInternal options);
	}
}
