using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class TouristModule : StateMachineComponent<TouristModule.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, TouristModule, object>.GameInstance
	{
		public StatesInstance(TouristModule smi)
			: base(smi)
		{
			smi.gameObject.Subscribe(-887025858, delegate
			{
				smi.SetSuspended(state: false);
				smi.ReleaseAstronaut(null, applyBuff: true);
				smi.assignable.Unassign();
			});
		}
	}

	public class States : GameStateMachine<States, StatesInstance, TouristModule>
	{
		public State idle;

		public State awaitingTourist;

		public State hasTourist;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.PlayAnim("grounded", KAnim.PlayMode.Loop).GoTo(awaitingTourist);
			awaitingTourist.PlayAnim("grounded", KAnim.PlayMode.Loop).ToggleChore((StatesInstance smi) => smi.master.CreateWorkChore(), hasTourist);
			hasTourist.PlayAnim("grounded", KAnim.PlayMode.Loop).EventTransition(GameHashes.RocketLanded, idle).EventTransition(GameHashes.AssigneeChanged, idle);
		}
	}

	public Storage storage;

	[Serialize]
	private bool isSuspended;

	private bool releasingAstronaut;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)39;

	public Assignable assignable;

	private HandleVector<int>.Handle partitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<TouristModule> OnSuspendDelegate = new EventSystem.IntraObjectHandler<TouristModule>(delegate(TouristModule component, object data)
	{
		component.OnSuspend(data);
	});

	private static readonly EventSystem.IntraObjectHandler<TouristModule> OnAssigneeChangedDelegate = new EventSystem.IntraObjectHandler<TouristModule>(delegate(TouristModule component, object data)
	{
		component.OnAssigneeChanged(data);
	});

	public bool IsSuspended => isSuspended;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void SetSuspended(bool state)
	{
		isSuspended = state;
	}

	public void ReleaseAstronaut(object data, bool applyBuff = false)
	{
		if (releasingAstronaut)
		{
			return;
		}
		releasingAstronaut = true;
		MinionStorage component = GetComponent<MinionStorage>();
		List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
		for (int num = storedMinionInfo.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = component.DeserializeMinion(storedMinionInfo[num].id, Grid.CellToPos(Grid.PosToCell(base.smi.master.transform.GetPosition())));
			if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(base.smi.master.gameObject), 0, -1)])
			{
				gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
				if (applyBuff)
				{
					gameObject.GetComponent<Effects>().Add(Db.Get().effects.Get("SpaceTourist"), should_save: true);
					gameObject.GetSMI<JoyBehaviourMonitor.Instance>()?.GoToOverjoyed();
				}
			}
		}
		releasingAstronaut = false;
	}

	public void OnSuspend(object data)
	{
		Storage component = GetComponent<Storage>();
		if (component != null)
		{
			component.capacityKg = component.MassStored();
			component.allowItemRemoval = false;
		}
		if (GetComponent<ManualDeliveryKG>() != null)
		{
			Object.Destroy(GetComponent<ManualDeliveryKG>());
		}
		SetSuspended(state: true);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		storage = GetComponent<Storage>();
		assignable = GetComponent<Assignable>();
		base.smi.StartSM();
		int cell = Grid.PosToCell(base.gameObject);
		partitionerEntry = GameScenePartitioner.Instance.Add("TouristModule.gantryChanged", base.gameObject, cell, GameScenePartitioner.Instance.validNavCellChangedLayer, OnGantryChanged);
		OnGantryChanged(null);
		Subscribe(-1277991738, OnSuspendDelegate);
		Subscribe(684616645, OnAssigneeChangedDelegate);
	}

	private void OnGantryChanged(object data)
	{
		if (base.gameObject != null)
		{
			KSelectable component = GetComponent<KSelectable>();
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.HasGantry);
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.MissingGantry);
			int i = Grid.OffsetCell(Grid.PosToCell(base.smi.master.gameObject), 0, -1);
			if (Grid.FakeFloor[i])
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.HasGantry);
			}
			else
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.MissingGantry);
			}
		}
	}

	private Chore CreateWorkChore()
	{
		WorkChore<CommandModuleWorkable> workChore = new WorkChore<CommandModuleWorkable>(Db.Get().ChoreTypes.Astronaut, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, null, ignore_schedule_block: false, only_when_operational: true, Assets.GetAnim("anim_hat_kanim"), is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.personalNeeds);
		workChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, assignable);
		return workChore;
	}

	private void OnAssigneeChanged(object data)
	{
		if (GetComponent<MinionStorage>().GetStoredMinionInfo().Count > 0)
		{
			ReleaseAstronaut(null);
			Game.Instance.userMenu.Refresh(base.gameObject);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		partitionerEntry.Clear();
		ReleaseAstronaut(null);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.smi.StopSM("cleanup");
	}
}
