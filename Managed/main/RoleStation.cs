using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/RoleStation")]
public class RoleStation : Workable, IGameObjectEffectDescriptor
{
	public class RoleStationSM : GameStateMachine<RoleStationSM, RoleStationSM.Instance, RoleStation>
	{
		public new class Instance : GameInstance
		{
			public Instance(RoleStation master)
				: base(master)
			{
			}
		}

		public State unoperational;

		public State operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			unoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.GetComponent<Operational>().IsOperational);
			operational.ToggleChore((Instance smi) => smi.master.CreateWorkChore(), unoperational);
		}
	}

	private Chore chore;

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpAdd]
	private Operational operational;

	private RoleStationSM.Instance smi;

	private Guid skillPointAvailableStatusItem;

	private Action<object> UpdateStatusItemDelegate;

	private List<int> subscriptions = new List<int>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		synchronizeAnims = true;
		UpdateStatusItemDelegate = UpdateSkillPointAvailableStatusItem;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.RoleStations.Add(this);
		smi = new RoleStationSM.Instance(this);
		smi.StartSM();
		SetWorkTime(7.53f);
		resetProgressOnStop = true;
		subscriptions.Add(Game.Instance.Subscribe(-1523247426, UpdateStatusItemDelegate));
		subscriptions.Add(Game.Instance.Subscribe(1505456302, UpdateStatusItemDelegate));
		UpdateSkillPointAvailableStatusItem();
	}

	protected override void OnStopWork(Worker worker)
	{
		Telepad.StatesInstance sMI = this.GetSMI<Telepad.StatesInstance>();
		sMI.sm.idlePortal.Trigger(sMI);
	}

	private void UpdateSkillPointAvailableStatusItem(object data = null)
	{
		foreach (MinionResume minionResume in Components.MinionResumes)
		{
			if (!minionResume.HasTag(GameTags.Dead) && minionResume.TotalSkillPointsGained - minionResume.SkillsMastered > 0)
			{
				if (skillPointAvailableStatusItem == Guid.Empty)
				{
					skillPointAvailableStatusItem = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SkillPointsAvailable);
				}
				return;
			}
		}
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SkillPointsAvailable);
		skillPointAvailableStatusItem = Guid.Empty;
	}

	private Chore CreateWorkChore()
	{
		return new WorkChore<RoleStation>(Db.Get().ChoreTypes.LearnSkill, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, null, ignore_schedule_block: false, only_when_operational: true, Assets.GetAnim("anim_hat_kanim"), is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.personalNeeds, 5, ignore_building_assignment: false, add_to_daily_report: false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		worker.GetComponent<MinionResume>().SkillLearned();
	}

	private void OnSelectRolesClick()
	{
		DetailsScreen.Instance.Show(show: false);
		ManagementMenu.Instance.ToggleSkills();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		foreach (int subscription in subscriptions)
		{
			Game.Instance.Unsubscribe(subscription);
		}
		Components.RoleStations.Remove(this);
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		return base.GetDescriptors(go);
	}
}
