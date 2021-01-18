using FMOD.Studio;
using Klei.AI;
using UnityEngine;

public class VoiceSoundEvent : SoundEvent
{
	public static float locomotionSoundProb = 50f;

	public float timeLastSpoke;

	public float intervalBetweenSpeaking = 10f;

	public VoiceSoundEvent(string file_name, string sound_name, int frame, bool is_looping)
		: base(file_name, sound_name, frame, do_load: false, is_looping, SoundEvent.IGNORE_INTERVAL, is_dynamic: true)
	{
		base.noiseValues = SoundEventVolumeCache.instance.GetVolume("VoiceSoundEvent", sound_name);
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		PlayVoice(base.name, behaviour.controller, intervalBetweenSpeaking, base.looping);
	}

	public static EventInstance PlayVoice(string name, KBatchedAnimController controller, float interval_between_speaking, bool looping, bool objectIsSelectedAndVisible = false)
	{
		EventInstance eventInstance = default(EventInstance);
		MinionIdentity component = controller.GetComponent<MinionIdentity>();
		if (component == null || (name.Contains("state") && Time.time - component.timeLastSpoke < interval_between_speaking))
		{
			return eventInstance;
		}
		if (name.Contains(":"))
		{
			string[] array = name.Split(':');
			float num = float.Parse(array[1]);
			float num2 = Random.Range(0, 100);
			if (num2 > num)
			{
				return eventInstance;
			}
		}
		Worker component2 = controller.GetComponent<Worker>();
		string assetName = GetAssetName(name, component2);
		StaminaMonitor.Instance sMI = component2.GetSMI<StaminaMonitor.Instance>();
		if (!name.Contains("sleep_") && sMI != null && sMI.IsSleeping())
		{
			return eventInstance;
		}
		Vector3 vector = component2.transform.GetPosition();
		vector.z = 0f;
		GameObject gameObject = controller.gameObject;
		if (SoundEvent.ObjectIsSelectedAndVisible(gameObject))
		{
			vector = SoundEvent.AudioHighlightListenerPosition(vector);
		}
		string sound = GlobalAssets.GetSound(assetName, force_no_warning: true);
		if (!SoundEvent.ShouldPlaySound(controller, sound, looping, is_dynamic: false))
		{
			return eventInstance;
		}
		if (sound != null)
		{
			if (looping)
			{
				LoopingSounds component3 = controller.GetComponent<LoopingSounds>();
				if (component3 == null)
				{
					Debug.Log(controller.name + " is missing LoopingSounds component. ");
				}
				else if (!component3.StartSound(sound))
				{
					DebugUtil.LogWarningArgs($"SoundEvent has invalid sound [{sound}] on behaviour [{controller.name}]");
				}
			}
			else
			{
				eventInstance = SoundEvent.BeginOneShot(sound, vector);
				if (sound.Contains("sleep_"))
				{
					Traits component4 = controller.GetComponent<Traits>();
					if (component4.HasTrait("Snorer"))
					{
						eventInstance.setParameterByName("snoring", 1f);
					}
				}
				SoundEvent.EndOneShot(eventInstance);
				component.timeLastSpoke = Time.time;
			}
		}
		else if (AudioDebug.Get().debugVoiceSounds)
		{
			Debug.LogWarning("Missing voice sound: " + assetName);
		}
		return eventInstance;
	}

	private static string GetAssetName(string name, Component cmp)
	{
		string b = "F01";
		if (cmp != null)
		{
			MinionIdentity component = cmp.GetComponent<MinionIdentity>();
			if (component != null)
			{
				b = component.GetVoiceId();
			}
		}
		string d = name;
		if (name.Contains(":"))
		{
			string[] array = name.Split(':');
			d = array[0];
		}
		return StringFormatter.Combine("DupVoc_", b, "_", d);
	}

	public override void Stop(AnimEventManager.EventPlayerData behaviour)
	{
		if (base.looping)
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if (component != null)
			{
				string assetName = GetAssetName(base.name, component);
				string sound = GlobalAssets.GetSound(assetName, force_no_warning: true);
				component.StopSound(sound);
			}
		}
	}
}
