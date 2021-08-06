using System.Collections.Generic;
using UnityEngine;

public abstract class AudioSheets : ScriptableObject
{
	public List<AudioSheet> sheets = new List<AudioSheet>();

	public Dictionary<HashedString, List<AnimEvent>> events = new Dictionary<HashedString, List<AnimEvent>>();

	public virtual void Initialize()
	{
		foreach (AudioSheet sheet in sheets)
		{
			AudioSheet.SoundInfo[] soundInfos = sheet.soundInfos;
			foreach (AudioSheet.SoundInfo soundInfo in soundInfos)
			{
				if (DlcManager.IsContentActive(soundInfo.RequiredDlcId))
				{
					string text = soundInfo.Type;
					if (text == null || text == "")
					{
						text = sheet.defaultType;
					}
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name0, soundInfo.Frame0, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name1, soundInfo.Frame1, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name2, soundInfo.Frame2, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name3, soundInfo.Frame3, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name4, soundInfo.Frame4, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name5, soundInfo.Frame5, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name6, soundInfo.Frame6, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name7, soundInfo.Frame7, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name8, soundInfo.Frame8, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name9, soundInfo.Frame9, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name10, soundInfo.Frame10, soundInfo.RequiredDlcId);
					CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name11, soundInfo.Frame11, soundInfo.RequiredDlcId);
				}
			}
		}
	}

	private void CreateSound(string file_name, string anim_name, string type, float min_interval, string sound_name, int frame, string dlcId)
	{
		if (string.IsNullOrEmpty(sound_name))
		{
			return;
		}
		HashedString key = file_name + "." + anim_name;
		AnimEvent animEvent = CreateSoundOfType(type, file_name, sound_name, frame, min_interval, dlcId);
		if (animEvent == null)
		{
			Debug.LogError("Unknown sound type: " + type);
			return;
		}
		List<AnimEvent> value = null;
		if (!events.TryGetValue(key, out value))
		{
			value = new List<AnimEvent>();
			events[key] = value;
		}
		value.Add(animEvent);
	}

	protected abstract AnimEvent CreateSoundOfType(string type, string file_name, string sound_name, int frame, float min_interval, string dlcId);

	public List<AnimEvent> GetEvents(HashedString anim_id)
	{
		List<AnimEvent> value = null;
		events.TryGetValue(anim_id, out value);
		return value;
	}
}
