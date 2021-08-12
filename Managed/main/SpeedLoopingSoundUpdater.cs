using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class SpeedLoopingSoundUpdater : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public EventInstance ev;

		public PARAMETER_ID parameterId;
	}

	private List<Entry> entries = new List<Entry>();

	public SpeedLoopingSoundUpdater()
		: base("Speed")
	{
	}

	public override void Add(Sound sound)
	{
		Entry entry = default(Entry);
		entry.ev = sound.ev;
		entry.parameterId = sound.description.GetParameterId(base.parameter);
		Entry item = entry;
		entries.Add(item);
	}

	public override void Update(float dt)
	{
		float speedParameterValue = GetSpeedParameterValue();
		foreach (Entry entry in entries)
		{
			EventInstance ev = entry.ev;
			ev.setParameterByID(entry.parameterId, speedParameterValue);
		}
	}

	public override void Remove(Sound sound)
	{
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].ev.handle == sound.ev.handle)
			{
				entries.RemoveAt(i);
				break;
			}
		}
	}

	public static float GetSpeedParameterValue()
	{
		return Time.timeScale * 1f;
	}
}
