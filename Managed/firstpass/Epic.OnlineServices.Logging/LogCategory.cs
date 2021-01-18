namespace Epic.OnlineServices.Logging
{
	public enum LogCategory
	{
		Core = 0,
		Auth = 1,
		Friends = 2,
		Presence = 3,
		UserInfo = 4,
		HttpSerialization = 5,
		Ecom = 6,
		P2P = 7,
		Sessions = 8,
		RateLimiter = 9,
		PlayerDataStorage = 10,
		Analytics = 11,
		Messaging = 12,
		Connect = 13,
		Overlay = 14,
		Achievements = 0xF,
		Stats = 0x10,
		Ui = 17,
		Lobby = 18,
		Leaderboards = 19,
		Keychain = 20,
		IdentityProvider = 21,
		TitleStorage = 22,
		AllCategories = int.MaxValue
	}
}
