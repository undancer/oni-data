using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Schedulable")]
public class Schedulable : KMonoBehaviour
{
	public Schedule GetSchedule()
	{
		return ScheduleManager.Instance.GetSchedule(this);
	}

	public bool IsAllowed(ScheduleBlockType schedule_block_type)
	{
		if (!VignetteManager.Instance.Get().IsRedAlert())
		{
			return ScheduleManager.Instance.IsAllowed(this, schedule_block_type);
		}
		return true;
	}

	public void OnScheduleChanged(Schedule schedule)
	{
		Trigger(467134493, schedule);
	}

	public void OnScheduleBlocksTick(Schedule schedule)
	{
		Trigger(1714332666, schedule);
	}

	public void OnScheduleBlocksChanged(Schedule schedule)
	{
		Trigger(-894023145, schedule);
	}
}
