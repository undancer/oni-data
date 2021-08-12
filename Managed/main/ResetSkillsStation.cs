using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ResetSkillsStation")]
public class ResetSkillsStation : Workable
{
	[MyCmpReq]
	public Assignable assignable;

	private Notification notification;

	private Chore chore;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		lightEfficiencyBonus = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		OnAssign(assignable.assignee);
		assignable.OnAssign += OnAssign;
	}

	private void OnAssign(IAssignableIdentity obj)
	{
		if (obj != null)
		{
			CreateChore();
		}
		else if (chore != null)
		{
			chore.Cancel("Unassigned");
			chore = null;
		}
	}

	private void CreateChore()
	{
		chore = new WorkChore<ResetSkillsStation>(Db.Get().ChoreTypes.UnlearnSkill, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, null, ignore_schedule_block: true, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		GetComponent<Operational>().SetActive(value: true);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		assignable.Unassign();
		MinionResume component = worker.GetComponent<MinionResume>();
		if (component != null)
		{
			component.ResetSkillLevels();
			component.SetHats(component.CurrentHat, null);
			component.ApplyTargetHat();
			notification = new Notification(MISC.NOTIFICATIONS.RESETSKILL.NAME, NotificationType.Good, (List<Notification> notificationList, object data) => string.Concat(MISC.NOTIFICATIONS.RESETSKILL.TOOLTIP, notificationList.ReduceMessages(countNames: false)));
			worker.GetComponent<Notifier>().Add(notification);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		GetComponent<Operational>().SetActive(value: false);
		chore = null;
	}
}
