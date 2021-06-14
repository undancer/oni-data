using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioMixer
{
	public class UserVolumeBus
	{
		public string labelString;

		public float busLevel;
	}

	private static AudioMixer _instance = null;

	private const string DUPLICANT_COUNT_ID = "duplicantCount";

	private const string PULSE_ID = "Pulse";

	private const string SNAPSHOT_ACTIVE_ID = "snapshotActive";

	private const string SPACE_VISIBLE_ID = "spaceVisible";

	private const string FACILITY_VISIBLE_ID = "facilityVisible";

	private const string FOCUS_BUS_PATH = "bus:/SFX/Focus";

	public Dictionary<HashedString, EventInstance> activeSnapshots = new Dictionary<HashedString, EventInstance>();

	public List<HashedString> SnapshotDebugLog = new List<HashedString>();

	public bool activeNIS = false;

	public static float LOW_PRIORITY_CUTOFF_DISTANCE = 10f;

	public static float PULSE_SNAPSHOT_BPM = 120f;

	public static int VISIBLE_DUPLICANTS_BEFORE_ATTENUATION = 2;

	private EventInstance duplicantCountInst;

	private EventInstance pulseInst;

	private EventInstance duplicantCountMovingInst;

	private EventInstance duplicantCountSleepingInst;

	private EventInstance spaceVisibleInst;

	private EventInstance facilityVisibleInst;

	private static readonly HashedString UserVolumeSettingsHash = new HashedString("event:/Snapshots/Mixing/Snapshot_UserVolumeSettings");

	public bool persistentSnapshotsActive;

	private Dictionary<string, int> visibleDupes = new Dictionary<string, int>();

	public Dictionary<string, UserVolumeBus> userVolumeSettings = new Dictionary<string, UserVolumeBus>();

	public static AudioMixer instance => _instance;

	public static AudioMixer Create()
	{
		_instance = new AudioMixer();
		AudioMixerSnapshots audioMixerSnapshots = AudioMixerSnapshots.Get();
		if (audioMixerSnapshots != null)
		{
			audioMixerSnapshots.ReloadSnapshots();
		}
		return _instance;
	}

	public static void Destroy()
	{
		_instance.StopAll();
		_instance = null;
	}

	public EventInstance Start(string snapshot)
	{
		if (!activeSnapshots.TryGetValue(snapshot, out var value))
		{
			if (RuntimeManager.IsInitialized)
			{
				value = KFMOD.CreateInstance(snapshot);
				activeSnapshots[snapshot] = value;
				value.start();
				value.setParameterByName("snapshotActive", 1f);
			}
			else
			{
				value = default(EventInstance);
			}
		}
		instance.Log("Start Snapshot: " + snapshot);
		return value;
	}

	public bool Stop(HashedString snapshot, FMOD.Studio.STOP_MODE stop_mode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
	{
		bool result = false;
		if (activeSnapshots.TryGetValue(snapshot, out var value))
		{
			value.setParameterByName("snapshotActive", 0f);
			value.stop(stop_mode);
			value.release();
			activeSnapshots.Remove(snapshot);
			result = true;
			AudioMixer instance = AudioMixer.instance;
			string[] obj = new string[5]
			{
				"Stop Snapshot: [",
				null,
				null,
				null,
				null
			};
			HashedString hashedString = snapshot;
			obj[1] = hashedString.ToString();
			obj[2] = "] with fadeout mode: [";
			obj[3] = stop_mode.ToString();
			obj[4] = "]";
			instance.Log(string.Concat(obj));
		}
		else
		{
			AudioMixer instance2 = AudioMixer.instance;
			HashedString hashedString = snapshot;
			instance2.Log("Tried to stop snapshot: [" + hashedString.ToString() + "] but it wasn't active.");
		}
		return result;
	}

	public void Reset()
	{
		StopAll();
	}

	public void StopAll(FMOD.Studio.STOP_MODE stop_mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
	{
		List<HashedString> list = new List<HashedString>();
		foreach (KeyValuePair<HashedString, EventInstance> activeSnapshot in activeSnapshots)
		{
			if (activeSnapshot.Key != UserVolumeSettingsHash)
			{
				list.Add(activeSnapshot.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			Stop(list[i], stop_mode);
		}
	}

	public bool SnapshotIsActive(HashedString snapshot_name)
	{
		if (activeSnapshots.ContainsKey(snapshot_name))
		{
			return true;
		}
		return false;
	}

	public void SetSnapshotParameter(string snapshot_name, string parameter_name, float parameter_value, bool shouldLog = true)
	{
		if (shouldLog)
		{
			Log($"Set Param {snapshot_name}: {parameter_name}, {parameter_value}");
		}
		if (activeSnapshots.TryGetValue(snapshot_name, out var value))
		{
			value.setParameterByName(parameter_name, parameter_value);
			return;
		}
		Log("Tried to set [" + parameter_name + "] to [" + parameter_value + "] but [" + snapshot_name + "] is not active.");
	}

	public void StartPersistentSnapshots()
	{
		persistentSnapshotsActive = true;
		Start(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated);
		Start(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot);
		Start(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot);
		spaceVisibleInst = Start(AudioMixerSnapshots.Get().SpaceVisibleSnapshot);
		facilityVisibleInst = Start(AudioMixerSnapshots.Get().FacilityVisibleSnapshot);
		Start(AudioMixerSnapshots.Get().PulseSnapshot);
	}

	public void StopPersistentSnapshots()
	{
		persistentSnapshotsActive = false;
		Stop(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated);
		Stop(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot);
		Stop(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot);
		Stop(AudioMixerSnapshots.Get().SpaceVisibleSnapshot);
		Stop(AudioMixerSnapshots.Get().FacilityVisibleSnapshot);
		Stop(AudioMixerSnapshots.Get().PulseSnapshot);
	}

	public void UpdatePersistentSnapshotParameters()
	{
		SetVisibleDuplicants();
		if (activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot, out duplicantCountMovingInst))
		{
			duplicantCountMovingInst.setParameterByName("duplicantCount", Mathf.Max(0, visibleDupes["moving"] - VISIBLE_DUPLICANTS_BEFORE_ATTENUATION));
		}
		if (activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot, out duplicantCountSleepingInst))
		{
			duplicantCountSleepingInst.setParameterByName("duplicantCount", Mathf.Max(0, visibleDupes["sleeping"] - VISIBLE_DUPLICANTS_BEFORE_ATTENUATION));
		}
		if (activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated, out duplicantCountInst))
		{
			duplicantCountInst.setParameterByName("duplicantCount", Mathf.Max(0, visibleDupes["visible"] - VISIBLE_DUPLICANTS_BEFORE_ATTENUATION));
		}
		if (activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().PulseSnapshot, out pulseInst))
		{
			float num = PULSE_SNAPSHOT_BPM / 60f;
			switch (SpeedControlScreen.Instance.GetSpeed())
			{
			case 1:
				num /= 2f;
				break;
			case 2:
				num /= 3f;
				break;
			}
			float value = Mathf.Abs(Mathf.Sin(Time.time * (float)Math.PI * num));
			pulseInst.setParameterByName("Pulse", value);
		}
	}

	public void UpdateSpaceVisibleSnapshot(float percent)
	{
		spaceVisibleInst.setParameterByName("spaceVisible", percent);
	}

	public void UpdateFacilityVisibleSnapshot(float percent)
	{
		facilityVisibleInst.setParameterByName("facilityVisible", percent);
	}

	private void SetVisibleDuplicants()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			Vector3 position = Components.LiveMinionIdentities[i].transform.GetPosition();
			if (!CameraController.Instance.IsVisiblePos(position))
			{
				continue;
			}
			num++;
			Navigator component = Components.LiveMinionIdentities[i].GetComponent<Navigator>();
			if (component != null && component.IsMoving())
			{
				num2++;
				continue;
			}
			Worker component2 = Components.LiveMinionIdentities[i].GetComponent<Worker>();
			if (component2.GetSMI<StaminaMonitor.Instance>()?.IsSleeping() ?? false)
			{
				num3++;
			}
		}
		visibleDupes["visible"] = num;
		visibleDupes["moving"] = num2;
		visibleDupes["sleeping"] = num3;
	}

	public void StartUserVolumesSnapshot()
	{
		Start(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot);
		if (!activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot, out var value))
		{
			return;
		}
		value.getDescription(out var description);
		description.getUserProperty("buses", out var property);
		string text = property.stringValue();
		char c = '-';
		string[] array = text.Split(c);
		for (int i = 0; i < array.Length; i++)
		{
			float busLevel = 1f;
			string key = "Volume_" + array[i];
			if (KPlayerPrefs.HasKey(key))
			{
				busLevel = KPlayerPrefs.GetFloat(key);
			}
			UserVolumeBus userVolumeBus = new UserVolumeBus();
			userVolumeBus.busLevel = busLevel;
			userVolumeBus.labelString = Strings.Get("STRINGS.UI.FRONTEND.AUDIO_OPTIONS_SCREEN.AUDIO_BUS_" + array[i].ToUpper());
			userVolumeSettings.Add(array[i], userVolumeBus);
			SetUserVolume(array[i], userVolumeBus.busLevel);
		}
	}

	public void SetUserVolume(string bus, float value)
	{
		if (!userVolumeSettings.ContainsKey(bus))
		{
			Debug.LogError("The provided bus doesn't exist. Check yo'self fool!");
			return;
		}
		if (value > 1f)
		{
			value = 1f;
		}
		else if (value < 0f)
		{
			value = 0f;
		}
		userVolumeSettings[bus].busLevel = value;
		KPlayerPrefs.SetFloat("Volume_" + bus, value);
		if (activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot, out var value2))
		{
			value2.setParameterByName("userVolume_" + bus, userVolumeSettings[bus].busLevel);
		}
		else
		{
			Log("Tried to set [" + bus + "] to [" + value + "] but UserVolumeSettingsSnapshot is not active.");
		}
		if (bus == "Music")
		{
			SetSnapshotParameter(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, "userVolume_Music", value);
		}
	}

	private void Log(string s)
	{
	}
}
