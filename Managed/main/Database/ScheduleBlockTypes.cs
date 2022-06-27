using STRINGS;
using UnityEngine;

namespace Database
{
	public class ScheduleBlockTypes : ResourceSet<ScheduleBlockType>
	{
		public ScheduleBlockType Sleep;

		public ScheduleBlockType Eat;

		public ScheduleBlockType Work;

		public ScheduleBlockType Hygiene;

		public ScheduleBlockType Recreation;

		public ScheduleBlockTypes(ResourceSet parent)
			: base("ScheduleBlockTypes", parent)
		{
			Sleep = Add(new ScheduleBlockType("Sleep", this, UI.SCHEDULEBLOCKTYPES.SLEEP.NAME, UI.SCHEDULEBLOCKTYPES.SLEEP.DESCRIPTION, new Color(0.9843137f, 0.99215686f, 23f / 85f)));
			Eat = Add(new ScheduleBlockType("Eat", this, UI.SCHEDULEBLOCKTYPES.EAT.NAME, UI.SCHEDULEBLOCKTYPES.EAT.DESCRIPTION, new Color(0.80784315f, 0.5294118f, 0.11372549f)));
			Work = Add(new ScheduleBlockType("Work", this, UI.SCHEDULEBLOCKTYPES.WORK.NAME, UI.SCHEDULEBLOCKTYPES.WORK.DESCRIPTION, new Color(0.9372549f, 11f / 85f, 11f / 85f)));
			Hygiene = Add(new ScheduleBlockType("Hygiene", this, UI.SCHEDULEBLOCKTYPES.HYGIENE.NAME, UI.SCHEDULEBLOCKTYPES.HYGIENE.DESCRIPTION, new Color(39f / 85f, 0.1764706f, 0.34509805f)));
			Recreation = Add(new ScheduleBlockType("Recreation", this, UI.SCHEDULEBLOCKTYPES.RECREATION.NAME, UI.SCHEDULEBLOCKTYPES.RECREATION.DESCRIPTION, new Color(39f / 85f, 19f / 51f, 16f / 85f)));
		}
	}
}
