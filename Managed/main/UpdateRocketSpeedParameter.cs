using System.Collections.Generic;
using FMOD.Studio;

internal class UpdateRocketSpeedParameter : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public RocketModule rocketModule;

		public EventInstance ev;

		public PARAMETER_ID parameterId;
	}

	private List<Entry> entries = new List<Entry>();

	public UpdateRocketSpeedParameter()
		: base("rocketSpeed")
	{
	}

	public override void Add(Sound sound)
	{
		Entry entry = default(Entry);
		entry.rocketModule = sound.transform.GetComponent<RocketModule>();
		entry.ev = sound.ev;
		entry.parameterId = sound.description.GetParameterId(base.parameter);
		Entry item = entry;
		entries.Add(item);
	}

	public override void Update(float dt)
	{
		foreach (Entry entry in entries)
		{
			if (entry.rocketModule == null)
			{
				continue;
			}
			LaunchConditionManager conditionManager = entry.rocketModule.conditionManager;
			if (!(conditionManager == null))
			{
				ILaunchableRocket component = conditionManager.GetComponent<ILaunchableRocket>();
				if (component != null)
				{
					EventInstance ev = entry.ev;
					ev.setParameterByID(entry.parameterId, component.rocketSpeed);
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
