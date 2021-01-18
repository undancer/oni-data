using System.Collections.Generic;
using FMOD.Studio;

internal abstract class UserVolumeLoopingUpdater : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public EventInstance ev;

		public PARAMETER_ID parameterId;
	}

	private List<Entry> entries = new List<Entry>();

	private string playerPref;

	public UserVolumeLoopingUpdater(string parameter, string player_pref)
		: base(parameter)
	{
		playerPref = player_pref;
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
		if (string.IsNullOrEmpty(playerPref))
		{
			return;
		}
		float @float = KPlayerPrefs.GetFloat(playerPref);
		foreach (Entry entry in entries)
		{
			EventInstance ev = entry.ev;
			ev.setParameterByID(entry.parameterId, @float);
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
}
