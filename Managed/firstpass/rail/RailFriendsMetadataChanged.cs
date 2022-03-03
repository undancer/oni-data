using System.Collections.Generic;

namespace rail
{
	public class RailFriendsMetadataChanged : EventBase
	{
		public List<RailFriendMetadata> friends_changed_metadata = new List<RailFriendMetadata>();
	}
}
