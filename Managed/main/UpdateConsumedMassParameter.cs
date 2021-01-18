using System.Collections.Generic;
using FMOD.Studio;

internal class UpdateConsumedMassParameter : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public CreatureCalorieMonitor.Instance creatureCalorieMonitor;

		public EventInstance ev;

		public PARAMETER_ID parameterId;
	}

	private List<Entry> entries = new List<Entry>();

	public UpdateConsumedMassParameter()
		: base("consumedMass")
	{
	}

	public override void Add(Sound sound)
	{
		Entry entry = default(Entry);
		entry.creatureCalorieMonitor = sound.transform.GetSMI<CreatureCalorieMonitor.Instance>();
		entry.ev = sound.ev;
		entry.parameterId = sound.description.GetParameterId(base.parameter);
		Entry item = entry;
		entries.Add(item);
	}

	public override void Update(float dt)
	{
		foreach (Entry entry in entries)
		{
			if (!entry.creatureCalorieMonitor.IsNullOrStopped())
			{
				float fullness = entry.creatureCalorieMonitor.stomach.GetFullness();
				EventInstance ev = entry.ev;
				ev.setParameterByID(entry.parameterId, fullness);
			}
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
