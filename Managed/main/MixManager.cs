using UnityEngine;

public class MixManager : MonoBehaviour
{
	private void Update()
	{
		if (AudioMixer.instance != null && AudioMixer.instance.persistentSnapshotsActive)
		{
			AudioMixer.instance.UpdatePersistentSnapshotParameters();
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (AudioMixer.instance != null && !(AudioMixerSnapshots.Get() == null))
		{
			if (!hasFocus && KPlayerPrefs.GetInt(AudioOptionsScreen.MuteOnFocusLost) == 1)
			{
				AudioMixer.instance.Start(AudioMixerSnapshots.Get().GameNotFocusedSnapshot);
			}
			else
			{
				AudioMixer.instance.Stop(AudioMixerSnapshots.Get().GameNotFocusedSnapshot);
			}
		}
	}
}
