using System.Collections.Generic;

namespace rail
{
	public class VoiceChannelUsersSpeakingStateChangedEvent : EventBase
	{
		public RailVoiceChannelID voice_channel_id = new RailVoiceChannelID();

		public List<RailVoiceChannelUserSpeakingState> users_speaking_state = new List<RailVoiceChannelUserSpeakingState>();
	}
}
