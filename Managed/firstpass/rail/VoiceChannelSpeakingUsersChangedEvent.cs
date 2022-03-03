using System.Collections.Generic;

namespace rail
{
	public class VoiceChannelSpeakingUsersChangedEvent : EventBase
	{
		public List<RailID> speaking_users = new List<RailID>();

		public RailVoiceChannelID voice_channel_id = new RailVoiceChannelID();

		public List<RailID> not_speaking_users = new List<RailID>();
	}
}
