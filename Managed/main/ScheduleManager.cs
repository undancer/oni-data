using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FMOD.Studio;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduleManager")]
public class ScheduleManager : KMonoBehaviour, ISim33ms
{
	public class Tuning : TuningData<Tuning>
	{
		public float toneSpacingSeconds;

		public int minToneIndex;

		public int maxToneIndex;

		public int firstLastToneSpacing;
	}

	[Serialize]
	private List<Schedule> schedules;

	[Serialize]
	private int lastIdx = 0;

	[Serialize]
	private int scheduleNameIncrementor = 0;

	public static ScheduleManager Instance;

	public event Action<List<Schedule>> onSchedulesChanged;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (schedules.Count == 0)
		{
			AddDefaultSchedule(alarmOn: true);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		schedules = new List<Schedule>();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		if (schedules.Count == 0)
		{
			AddDefaultSchedule(alarmOn: true);
		}
		foreach (Schedule schedule in schedules)
		{
			schedule.ClearNullReferences();
		}
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			Schedulable component = item.GetComponent<Schedulable>();
			if (GetSchedule(component) == null)
			{
				schedules[0].Assign(component);
			}
		}
		Components.LiveMinionIdentities.OnAdd += OnAddDupe;
		Components.LiveMinionIdentities.OnRemove += OnRemoveDupe;
	}

	private void OnAddDupe(MinionIdentity minion)
	{
		Schedulable component = minion.GetComponent<Schedulable>();
		if (GetSchedule(component) == null)
		{
			schedules[0].Assign(component);
		}
	}

	private void OnRemoveDupe(MinionIdentity minion)
	{
		Schedulable component = minion.GetComponent<Schedulable>();
		GetSchedule(component)?.Unassign(component);
	}

	public void OnStoredDupeDestroyed(StoredMinionIdentity dupe)
	{
		foreach (Schedule schedule in schedules)
		{
			schedule.Unassign(dupe.gameObject.GetComponent<Schedulable>());
		}
	}

	public void AddDefaultSchedule(bool alarmOn)
	{
		Schedule schedule = AddSchedule(Db.Get().ScheduleGroups.allGroups, UI.SCHEDULESCREEN.SCHEDULE_NAME_DEFAULT, alarmOn);
		if (Game.Instance.FastWorkersModeActive)
		{
			for (int i = 0; i < 21; i++)
			{
				schedule.SetGroup(i, Db.Get().ScheduleGroups.Worktime);
			}
			schedule.SetGroup(21, Db.Get().ScheduleGroups.Recreation);
			schedule.SetGroup(22, Db.Get().ScheduleGroups.Recreation);
			schedule.SetGroup(23, Db.Get().ScheduleGroups.Sleep);
		}
	}

	public Schedule AddSchedule(List<ScheduleGroup> groups, string name = null, bool alarmOn = false)
	{
		scheduleNameIncrementor++;
		if (name == null)
		{
			name = string.Format(UI.SCHEDULESCREEN.SCHEDULE_NAME_FORMAT, scheduleNameIncrementor.ToString());
		}
		Schedule schedule = new Schedule(name, groups, alarmOn);
		schedules.Add(schedule);
		if (this.onSchedulesChanged != null)
		{
			this.onSchedulesChanged(schedules);
		}
		return schedule;
	}

	public void DeleteSchedule(Schedule schedule)
	{
		if (schedules.Count == 1)
		{
			return;
		}
		List<Ref<Schedulable>> assigned = schedule.GetAssigned();
		schedules.Remove(schedule);
		foreach (Ref<Schedulable> item in assigned)
		{
			schedules[0].Assign(item.Get());
		}
		if (this.onSchedulesChanged != null)
		{
			this.onSchedulesChanged(schedules);
		}
	}

	public Schedule GetSchedule(Schedulable schedulable)
	{
		foreach (Schedule schedule in schedules)
		{
			if (schedule.IsAssigned(schedulable))
			{
				return schedule;
			}
		}
		return null;
	}

	public List<Schedule> GetSchedules()
	{
		return schedules;
	}

	public bool IsAllowed(Schedulable schedulable, ScheduleBlockType schedule_block_type)
	{
		int blockIdx = Schedule.GetBlockIdx();
		return GetSchedule(schedulable)?.GetBlock(blockIdx).IsAllowed(schedule_block_type) ?? false;
	}

	public void Sim33ms(float dt)
	{
		int blockIdx = Schedule.GetBlockIdx();
		if (blockIdx == lastIdx)
		{
			return;
		}
		foreach (Schedule schedule in schedules)
		{
			schedule.Tick();
		}
		lastIdx = blockIdx;
	}

	public void PlayScheduleAlarm(Schedule schedule, ScheduleBlock block, bool forwards)
	{
		Notification notification = new Notification(string.Format(MISC.NOTIFICATIONS.SCHEDULE_CHANGED.NAME, schedule.name, block.name), NotificationType.Good, (List<Notification> notificationList, object data) => string.Format(MISC.NOTIFICATIONS.SCHEDULE_CHANGED.TOOLTIP, schedule.name, block.name, Db.Get().ScheduleGroups.Get(block.GroupId).notificationTooltip));
		GetComponent<Notifier>().Add(notification);
		StartCoroutine(PlayScheduleTone(schedule, forwards));
	}

	private IEnumerator PlayScheduleTone(Schedule schedule, bool forwards)
	{
		int[] tones = schedule.GetTones();
		for (int i = 0; i < tones.Length; i++)
		{
			int t = (forwards ? i : (tones.Length - 1 - i));
			PlayTone(tones[t], forwards);
			yield return new WaitForSeconds(TuningData<Tuning>.Get().toneSpacingSeconds);
		}
	}

	private void PlayTone(int pitch, bool forwards)
	{
		EventInstance instance = KFMOD.BeginOneShot(GlobalAssets.GetSound("WorkChime_tone"), Vector3.zero);
		instance.setParameterByName("WorkChime_pitch", pitch);
		instance.setParameterByName("WorkChime_start", forwards ? 1 : 0);
		KFMOD.EndOneShot(instance);
	}
}
