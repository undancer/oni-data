using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CommandModule : StateMachineComponent<CommandModule.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, CommandModule, object>.GameInstance
	{
		public StatesInstance(CommandModule master)
			: base(master)
		{
		}

		public void SetSuspended(bool suspended)
		{
			Storage component = GetComponent<Storage>();
			if (component != null)
			{
				component.allowItemRemoval = !suspended;
			}
			ManualDeliveryKG component2 = GetComponent<ManualDeliveryKG>();
			if (component2 != null)
			{
				component2.Pause(suspended, "Rocket is suspended");
			}
		}

		public bool CheckStoredMinionIsAssignee()
		{
			foreach (MinionStorage.Info item in GetComponent<MinionStorage>().GetStoredMinionInfo())
			{
				if (item.serializedMinion == null)
				{
					continue;
				}
				KPrefabID kPrefabID = item.serializedMinion.Get();
				if (!(kPrefabID == null))
				{
					StoredMinionIdentity component = kPrefabID.GetComponent<StoredMinionIdentity>();
					if (GetComponent<Assignable>().assignee == component.assignableProxy.Get())
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, CommandModule>
	{
		public class GroundedStates : State
		{
			public State awaitingAstronaut;

			public State hasAstronaut;

			public State waitingToRelease;
		}

		public class SpaceborneStates : State
		{
			public State launch;

			public State idle;

			public State land;
		}

		public Signal gantryChanged;

		public BoolParameter accumulatedPee;

		public GroundedStates grounded;

		public SpaceborneStates spaceborne;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grounded;
			grounded.PlayAnim("grounded", KAnim.PlayMode.Loop).DefaultState(grounded.awaitingAstronaut).TagTransition(GameTags.RocketNotOnGround, spaceborne);
			grounded.awaitingAstronaut.Enter(delegate(StatesInstance smi)
			{
				if (smi.CheckStoredMinionIsAssignee())
				{
					smi.GoTo(grounded.hasAstronaut);
				}
				Game.Instance.userMenu.Refresh(smi.gameObject);
			}).EventHandler(GameHashes.AssigneeChanged, delegate(StatesInstance smi)
			{
				if (smi.CheckStoredMinionIsAssignee())
				{
					smi.GoTo(grounded.hasAstronaut);
				}
				Game.Instance.userMenu.Refresh(smi.gameObject);
			}).ToggleChore((StatesInstance smi) => smi.master.CreateWorkChore(), grounded.hasAstronaut);
			grounded.hasAstronaut.EventHandler(GameHashes.AssigneeChanged, delegate(StatesInstance smi)
			{
				if (!smi.CheckStoredMinionIsAssignee())
				{
					smi.GoTo(grounded.waitingToRelease);
				}
			});
			grounded.waitingToRelease.ToggleStatusItem(Db.Get().BuildingStatusItems.DisembarkingDuplicant).OnSignal(gantryChanged, grounded.awaitingAstronaut, delegate(StatesInstance smi)
			{
				if (HasValidGantry(smi.gameObject))
				{
					smi.master.ReleaseAstronaut(accumulatedPee.Get(smi));
					accumulatedPee.Set(value: false, smi);
					Game.Instance.userMenu.Refresh(smi.gameObject);
					return true;
				}
				return false;
			});
			spaceborne.DefaultState(spaceborne.launch);
			spaceborne.launch.Enter(delegate(StatesInstance smi)
			{
				smi.SetSuspended(suspended: true);
			}).GoTo(spaceborne.idle);
			spaceborne.idle.TagTransition(GameTags.RocketNotOnGround, spaceborne.land, on_remove: true);
			spaceborne.land.Enter(delegate(StatesInstance smi)
			{
				smi.SetSuspended(suspended: false);
				Game.Instance.userMenu.Refresh(smi.gameObject);
				accumulatedPee.Set(value: true, smi);
			}).GoTo(grounded.waitingToRelease);
		}
	}

	public Storage storage;

	public RocketStats rocketStats;

	public RocketCommandConditions conditions;

	private bool releasingAstronaut;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)39;

	public Assignable assignable;

	private HandleVector<int>.Handle partitionerEntry;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		rocketStats = new RocketStats(this);
		conditions = GetComponent<RocketCommandConditions>();
	}

	public void ReleaseAstronaut(bool fill_bladder)
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
			if (!(gameObject == null))
			{
				if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(base.smi.master.gameObject), 0, -1)])
				{
					gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
				}
				if (fill_bladder)
				{
					AmountInstance amountInstance = Db.Get().Amounts.Bladder.Lookup(gameObject);
					if (amountInstance != null)
					{
						amountInstance.value = amountInstance.GetMax();
					}
				}
			}
		}
		releasingAstronaut = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		storage = GetComponent<Storage>();
		assignable = GetComponent<Assignable>();
		assignable.AddAssignPrecondition(CanAssignTo);
		base.smi.StartSM();
		int cell = Grid.PosToCell(base.gameObject);
		partitionerEntry = GameScenePartitioner.Instance.Add("CommandModule.gantryChanged", base.gameObject, cell, GameScenePartitioner.Instance.validNavCellChangedLayer, OnGantryChanged);
		OnGantryChanged(null);
	}

	private bool CanAssignTo(MinionAssignablesProxy worker)
	{
		if (worker.target is MinionIdentity)
		{
			return (worker.target as KMonoBehaviour).GetComponent<MinionResume>().HasPerk(Db.Get().SkillPerks.CanUseRockets);
		}
		if (worker.target is StoredMinionIdentity)
		{
			return (worker.target as StoredMinionIdentity).HasPerk(Db.Get().SkillPerks.CanUseRockets);
		}
		return false;
	}

	private static bool HasValidGantry(GameObject go)
	{
		int num = Grid.OffsetCell(Grid.PosToCell(go), 0, -1);
		if (Grid.IsValidCell(num))
		{
			return Grid.FakeFloor[num];
		}
		return false;
	}

	private void OnGantryChanged(object data)
	{
		if (base.gameObject != null)
		{
			KSelectable component = GetComponent<KSelectable>();
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.HasGantry);
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.MissingGantry);
			if (HasValidGantry(base.smi.master.gameObject))
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.HasGantry);
			}
			else
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.MissingGantry);
			}
			base.smi.sm.gantryChanged.Trigger(base.smi);
		}
	}

	private Chore CreateWorkChore()
	{
		WorkChore<CommandModuleWorkable> workChore = new WorkChore<CommandModuleWorkable>(Db.Get().ChoreTypes.Astronaut, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, null, ignore_schedule_block: false, only_when_operational: true, Assets.GetAnim("anim_hat_kanim"), is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.personalNeeds);
		workChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRockets);
		workChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, assignable);
		return workChore;
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		partitionerEntry.Clear();
		ReleaseAstronaut(fill_bladder: false);
		base.smi.StopSM("cleanup");
	}
}
