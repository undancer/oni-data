using System.Collections.Generic;
using FMOD.Studio;

internal class UpdatePercentCompleteParameter : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public Worker worker;

		public EventInstance ev;

		public PARAMETER_ID parameterId;
	}

	private List<Entry> entries = new List<Entry>();

	public UpdatePercentCompleteParameter()
		: base("percentComplete")
	{
	}

	public override void Add(Sound sound)
	{
		Entry entry = default(Entry);
		entry.worker = sound.transform.GetComponent<Worker>();
		entry.ev = sound.ev;
		entry.parameterId = sound.description.GetParameterId(base.parameter);
		Entry item = entry;
		entries.Add(item);
	}

	public override void Update(float dt)
	{
		foreach (Entry entry in entries)
		{
			if (!(entry.worker == null))
			{
				Workable workable = entry.worker.workable;
				if (!(workable == null))
				{
					float percentComplete = workable.GetPercentComplete();
					EventInstance ev = entry.ev;
					ev.setParameterByID(entry.parameterId, percentComplete);
				}
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
