public class MouthFlapSoundEvent : SoundEvent
{
	public MouthFlapSoundEvent(string file_name, string sound_name, int frame, bool is_looping)
		: base(file_name, sound_name, frame, do_load: false, is_looping, SoundEvent.IGNORE_INTERVAL, is_dynamic: true)
	{
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		behaviour.controller.GetSMI<SpeechMonitor.Instance>().PlaySpeech(base.name, null);
	}
}
