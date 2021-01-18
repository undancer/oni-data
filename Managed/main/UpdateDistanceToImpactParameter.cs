using System.Collections.Generic;
using FMOD.Studio;

internal class UpdateDistanceToImpactParameter : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public Comet comet;

		public EventInstance ev;

		public PARAMETER_ID parameterId;
	}

	private List<Entry> entries = new List<Entry>();

	public UpdateDistanceToImpactParameter()
		: base("distanceToImpact")
	{
	}

	public override void Add(Sound sound)
	{
		Entry entry = default(Entry);
		entry.comet = sound.transform.GetComponent<Comet>();
		entry.ev = sound.ev;
		entry.parameterId = sound.description.GetParameterId(base.parameter);
		Entry item = entry;
		entries.Add(item);
	}

	public override void Update(float dt)
	{
		foreach (Entry entry in entries)
		{
			if (!(entry.comet == null))
			{
				float soundDistance = entry.comet.GetSoundDistance();
				EventInstance ev = entry.ev;
				ev.setParameterByID(entry.parameterId, soundDistance);
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
