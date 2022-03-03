namespace rail
{
	public class VoiceChannelInviteEvent : EventBase
	{
		public string channel_name;

		public RailVoiceChannelID voice_channel_id = new RailVoiceChannelID();

		public string inviter_name;
	}
}
