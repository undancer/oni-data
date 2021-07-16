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
		WorldContainer myWorld = base.gameObject.GetMyWorld();
		if (myWorld == null)
		{
			DebugUtil.LogWarningArgs($"Trying to schedule {schedule_block_type.Id} but {base.gameObject.name} is not on a valid world. Grid cell: {Grid.PosToCell(base.gameObject.GetComponent<KPrefabID>())}");
			return false;
		}
		if (!myWorld.AlertManager.IsRedAlert())
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
