using System.Collections.Generic;

namespace rail
{
	public class VoiceChannelMemeberChangedEvent : EventBase
	{
		public RailVoiceChannelID voice_channel_id = new RailVoiceChannelID();

		public List<RailID> member_ids = new List<RailID>();
	}
}
