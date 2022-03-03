using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Activatable")]
public class Activatable : Workable, ISidescreenButtonControl
{
	private Operational.Flag activatedFlag = new Operational.Flag("activated", Operational.Flag.Type.Requirement);

	[Serialize]
	private bool activated;

	[Serialize]
	private bool awaitingActivation;

	private Guid statusItem;

	private Chore activateChore;

	public bool IsActivated => activated;

	public string SidescreenButtonText => (activateChore == null) ? UI.USERMENUACTIONS.ACTIVATEBUILDING.ACTIVATE : UI.USERMENUACTIONS.ACTIVATEBUILDING.ACTIVATE_CANCEL;

	public string SidescreenButtonTooltip => (activateChore == null) ? UI.USERMENUACTIONS.ACTIVATEBUILDING.TOOLTIP_ACTIVATE : UI.USERMENUACTIONS.ACTIVATEBUILDING.TOOLTIP_CANCEL;

	protected override void OnSpawn()
	{
		UpdateFlag();
		if (awaitingActivation && activateChore == null)
		{
			CreateChore();
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		activated = true;
		awaitingActivation = false;
		UpdateFlag();
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCompleteWork(worker);
	}

	private void UpdateFlag()
	{
		GetComponent<Operational>().SetFlag(activatedFlag, activated);
		GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.DuplicantActivationRequired, !activated);
		Trigger(-1909216579, IsActivated);
	}

	private void CreateChore()
	{
		if (activateChore == null)
		{
			Prioritizable.AddRef(base.gameObject);
			activateChore = new WorkChore<Activatable>(Db.Get().ChoreTypes.Toggle, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
			if (!string.IsNullOrEmpty(requiredSkillPerk))
			{
				shouldShowSkillPerkStatusItem = true;
				requireMinionToWork = true;
				UpdateStatusItem();
			}
		}
	}

	private void CancelChore()
	{
		if (activateChore != null)
		{
			activateChore.Cancel("User cancelled");
			activateChore = null;
		}
	}

	public bool SidescreenEnabled()
	{
		return !activated;
	}

	public void OnSidescreenButtonPressed()
	{
		if (activateChore == null)
		{
			CreateChore();
		}
		else
		{
			CancelChore();
		}
		awaitingActivation = activateChore != null;
	}

	public bool SidescreenButtonInteractable()
	{
		return !activated;
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}
}
