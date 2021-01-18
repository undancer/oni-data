public class LaserSoundEvent : SoundEvent
{
	public LaserSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, do_load: true, is_looping: true, min_interval, is_dynamic: false)
	{
		base.noiseValues = SoundEventVolumeCache.instance.GetVolume("LaserSoundEvent", sound_name);
	}
}
