using System;

namespace Epic.OnlineServices.Auth
{
	[Flags]
	public enum AuthScopeFlags
	{
		NoFlags = 0,
		BasicProfile = 1,
		FriendsList = 2,
		Presence = 4,
		FriendsManagement = 8
	}
}
