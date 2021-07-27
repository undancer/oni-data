using System;

namespace Epic.OnlineServices.Auth
{
	[Flags]
	public enum AuthScopeFlags
	{
		NoFlags = 0x0,
		BasicProfile = 0x1,
		FriendsList = 0x2,
		Presence = 0x4,
		FriendsManagement = 0x8
	}
}
