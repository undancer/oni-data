using System;
using FMOD.Studio;
using UnityEngine;

public class CountedSoundEvent : SoundEvent
{
	private const int COUNTER_MODULUS_INVALID = int.MinValue;

	private const int COUNTER_MODULUS_CLEAR = -1;

	private int counterModulus = int.MinValue;

	private static string BaseSoundName(string sound_name)
	{
		int num = sound_name.IndexOf(":");
		if (num > 0)
		{
			return sound_name.Substring(0, num);
		}
		return sound_name;
	}

	public CountedSoundEvent(string file_name, string sound_name, int frame, bool do_load, bool is_looping, float min_interval, bool is_dynamic)
		: base(file_name, BaseSoundName(sound_name), frame, do_load, is_looping, min_interval, is_dynamic)
	{
		if (sound_name.Contains(":"))
		{
			string[] array = sound_name.Split(':');
			if (array.Length != 2)
			{
				DebugUtil.LogErrorArgs("Invalid CountedSoundEvent parameter for", file_name + "." + sound_name + "." + frame + ":", "'" + sound_name + "'");
			}
			for (int i = 1; i < array.Length; i++)
			{
				ParseParameter(array[i]);
			}
		}
		else
		{
			DebugUtil.LogErrorArgs("CountedSoundEvent for", file_name + "." + sound_name + "." + frame, " - Must specify max number of steps on event: '" + sound_name + "'");
		}
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		if (string.IsNullOrEmpty(base.sound))
		{
			return;
		}
		GameObject gameObject = behaviour.controller.gameObject;
		base.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
		if (!base.objectIsSelectedAndVisible && !SoundEvent.ShouldPlaySound(behaviour.controller, base.sound, base.soundHash, base.looping, isDynamic))
		{
			return;
		}
		int num = -1;
		if (counterModulus >= -1)
		{
			HandleVector<int>.Handle h = GameComps.WhiteBoards.GetHandle(gameObject);
			if (!h.IsValid())
			{
				h = GameComps.WhiteBoards.Add(gameObject);
			}
			num = (GameComps.WhiteBoards.HasValue(h, base.soundHash) ? ((int)GameComps.WhiteBoards.GetValue(h, base.soundHash)) : 0);
			int num2 = ((counterModulus != -1) ? ((num + 1) % counterModulus) : 0);
			GameComps.WhiteBoards.SetValue(h, base.soundHash, num2);
		}
		Vector3 vector = behaviour.GetComponent<Transform>().GetPosition();
		vector.z = 0f;
		if (base.objectIsSelectedAndVisible)
		{
			vector = SoundEvent.AudioHighlightListenerPosition(vector);
		}
		EventInstance instance = SoundEvent.BeginOneShot(base.sound, vector, SoundEvent.GetVolume(base.objectIsSelectedAndVisible));
		if (instance.isValid())
		{
			if (num >= 0)
			{
				instance.setParameterByName("eventCount", num);
			}
			SoundEvent.EndOneShot(instance);
		}
	}

	private void ParseParameter(string param)
	{
		counterModulus = int.Parse(param);
		if (counterModulus != -1 && counterModulus < 2)
		{
			throw new ArgumentException("CountedSoundEvent modulus must be 2 or larger");
		}
	}
}
