using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Toggleable")]
public class Toggleable : Workable
{
	private List<KeyValuePair<IToggleHandler, Chore>> targets;

	private Toggleable()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnPrefabInit()
	{
		faceTargetWhenWorking = true;
		base.OnPrefabInit();
		targets = new List<KeyValuePair<IToggleHandler, Chore>>();
		SetWorkTime(3f);
		workerStatusItem = Db.Get().DuplicantStatusItems.Toggling;
		overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_use_remote_kanim") };
		synchronizeAnims = false;
	}

	public int SetTarget(IToggleHandler handler)
	{
		targets.Add(new KeyValuePair<IToggleHandler, Chore>(handler, null));
		return targets.Count - 1;
	}

	public IToggleHandler GetToggleHandlerForWorker(Worker worker)
	{
		int targetForWorker = GetTargetForWorker(worker);
		if (targetForWorker != -1)
		{
			return targets[targetForWorker].Key;
		}
		return null;
	}

	private int GetTargetForWorker(Worker worker)
	{
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i].Value != null && targets[i].Value.driver != null && targets[i].Value.driver.gameObject == worker.gameObject)
			{
				return i;
			}
		}
		return -1;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		int targetForWorker = GetTargetForWorker(worker);
		if (targetForWorker != -1 && targets[targetForWorker].Key != null)
		{
			targets[targetForWorker] = new KeyValuePair<IToggleHandler, Chore>(targets[targetForWorker].Key, null);
			targets[targetForWorker].Key.HandleToggle();
		}
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingSwitchToggle);
	}

	private void QueueToggle(int targetIdx)
	{
		if (targets[targetIdx].Value == null)
		{
			if (DebugHandler.InstantBuildMode)
			{
				targets[targetIdx].Key.HandleToggle();
				return;
			}
			targets[targetIdx] = new KeyValuePair<IToggleHandler, Chore>(targets[targetIdx].Key, new WorkChore<Toggleable>(Db.Get().ChoreTypes.Toggle, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.basic, 5, ignore_building_assignment: true));
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingSwitchToggle);
		}
	}

	public void Toggle(int targetIdx)
	{
		if (targetIdx < targets.Count)
		{
			if (targets[targetIdx].Value == null)
			{
				QueueToggle(targetIdx);
			}
			else
			{
				CancelToggle(targetIdx);
			}
		}
	}

	private void CancelToggle(int targetIdx)
	{
		if (targets[targetIdx].Value != null)
		{
			targets[targetIdx].Value.Cancel("Toggle cancelled");
			targets[targetIdx] = new KeyValuePair<IToggleHandler, Chore>(targets[targetIdx].Key, null);
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingSwitchToggle);
		}
	}

	public bool IsToggleQueued(int targetIdx)
	{
		return targets[targetIdx].Value != null;
	}
}
