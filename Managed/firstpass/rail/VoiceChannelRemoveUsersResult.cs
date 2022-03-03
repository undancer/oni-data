using System.Collections.Generic;

namespace rail
{
	public class VoiceChannelRemoveUsersResult : EventBase
	{
		public List<RailID> success_ids = new List<RailID>();

		public RailVoiceChannelID voice_channel_id = new RailVoiceChannelID();

		public List<RailID> failed_ids = new List<RailID>();
	}
}
