using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class ScheduleGroups : ResourceSet<ScheduleGroup>
	{
		public List<ScheduleGroup> allGroups;

		public ScheduleGroup Hygene;

		public ScheduleGroup Worktime;

		public ScheduleGroup Recreation;

		public ScheduleGroup Sleep;

		public ScheduleGroup Add(string id, int defaultSegments, string name, string description, string notificationTooltip, List<ScheduleBlockType> allowedTypes, bool alarm = false)
		{
			ScheduleGroup scheduleGroup = new ScheduleGroup(id, this, defaultSegments, name, description, notificationTooltip, allowedTypes, alarm);
			allGroups.Add(scheduleGroup);
			return scheduleGroup;
		}

		public ScheduleGroups(ResourceSet parent)
			: base("ScheduleGroups", parent)
		{
			allGroups = new List<ScheduleGroup>();
			Hygene = Add("Hygene", 1, UI.SCHEDULEGROUPS.HYGENE.NAME, UI.SCHEDULEGROUPS.HYGENE.DESCRIPTION, UI.SCHEDULEGROUPS.HYGENE.NOTIFICATION_TOOLTIP, new List<ScheduleBlockType>
			{
				Db.Get().ScheduleBlockTypes.Hygiene,
				Db.Get().ScheduleBlockTypes.Work
			});
			Worktime = Add("Worktime", 18, UI.SCHEDULEGROUPS.WORKTIME.NAME, UI.SCHEDULEGROUPS.WORKTIME.DESCRIPTION, UI.SCHEDULEGROUPS.WORKTIME.NOTIFICATION_TOOLTIP, new List<ScheduleBlockType>
			{
				Db.Get().ScheduleBlockTypes.Work
			}, alarm: true);
			Recreation = Add("Recreation", 2, UI.SCHEDULEGROUPS.RECREATION.NAME, UI.SCHEDULEGROUPS.RECREATION.DESCRIPTION, UI.SCHEDULEGROUPS.RECREATION.NOTIFICATION_TOOLTIP, new List<ScheduleBlockType>
			{
				Db.Get().ScheduleBlockTypes.Hygiene,
				Db.Get().ScheduleBlockTypes.Eat,
				Db.Get().ScheduleBlockTypes.Recreation,
				Db.Get().ScheduleBlockTypes.Work
			});
			Sleep = Add("Sleep", 3, UI.SCHEDULEGROUPS.SLEEP.NAME, UI.SCHEDULEGROUPS.SLEEP.DESCRIPTION, UI.SCHEDULEGROUPS.SLEEP.NOTIFICATION_TOOLTIP, new List<ScheduleBlockType>
			{
				Db.Get().ScheduleBlockTypes.Sleep
			});
			int num = 0;
			foreach (ScheduleGroup allGroup in allGroups)
			{
				num += allGroup.defaultSegments;
			}
			Debug.Assert(num == 24, "Default schedule groups must add up to exactly 1 cycle!");
		}

		public ScheduleGroup FindGroupForScheduleTypes(List<ScheduleBlockType> types)
		{
			foreach (ScheduleGroup allGroup in allGroups)
			{
				if (Schedule.AreScheduleTypesIdentical(allGroup.allowedTypes, types))
				{
					return allGroup;
				}
			}
			return null;
		}
	}
}
