namespace rail
{
	public interface IRailVoiceHelper
	{
		IRailVoiceChannel AsyncCreateVoiceChannel(CreateVoiceChannelOption options, string channel_name, string user_data, out RailResult result);

		IRailVoiceChannel OpenVoiceChannel(RailVoiceChannelID voice_channel_id, string channel_name, out RailResult result);

		EnumRailVoiceChannelSpeakerState GetSpeakerState();

		RailResult MuteSpeaker();

		RailResult ResumeSpeaker();

		RailResult SetupVoiceCapture(RailVoiceCaptureOption options, RailCaptureVoiceCallback callback);

		RailResult SetupVoiceCapture(RailVoiceCaptureOption options);

		RailResult StartVoiceCapturing(uint duration_milliseconds, bool repeat);

		RailResult StartVoiceCapturing(uint duration_milliseconds);

		RailResult StartVoiceCapturing();

		RailResult StopVoiceCapturing();

		RailResult GetCapturedVoiceData(byte[] buffer, uint buffer_length, out uint encoded_bytes_written);

		RailResult DecodeVoice(byte[] encoded_buffer, uint encoded_length, byte[] pcm_buffer, uint pcm_buffer_length, out uint pcm_buffer_written);

		RailResult GetVoiceCaptureSpecification(RailVoiceCaptureSpecification spec);

		RailResult EnableInGameVoiceSpeaking(bool can_speaking);

		RailResult SetPlayerNicknameInVoiceChannel(string nickname);

		RailResult SetPushToTalkKeyInVoiceChannel(uint push_to_talk_hot_key);

		uint GetPushToTalkKeyInVoiceChannel();

		RailResult ShowOverlayUI(bool show);

		RailResult SetMicroVolume(uint volume);

		RailResult SetSpeakerVolume(uint volume);
	}
}
