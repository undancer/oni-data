namespace rail
{
	public class JoinVoiceChannelResult : EventBase
	{
		public RailVoiceChannelID already_joined_channel_id = new RailVoiceChannelID();

		public RailVoiceChannelID voice_channel_id = new RailVoiceChannelID();
	}
}
