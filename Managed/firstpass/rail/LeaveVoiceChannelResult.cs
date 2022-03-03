namespace rail
{
	public class LeaveVoiceChannelResult : EventBase
	{
		public RailVoiceChannelID voice_channel_id = new RailVoiceChannelID();

		public EnumRailVoiceLeaveChannelReason reason;
	}
}
