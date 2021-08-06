using KSerialization;
using STRINGS;
using UnityEngine;

public class RocketUsageRestriction : GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>
{
	public class Def : BaseDef
	{
		public bool initialControlledStateWhenBuilt = true;

		public override void Configure(GameObject prefab)
		{
			RocketControlStation.CONTROLLED_BUILDINGS.Add(prefab.PrefabID());
		}
	}

	public class ControlledStates : State
	{
		public State nostation;

		public State controlled;
	}

	public class RestrictionStates : State
	{
		public State uncontrolled;

		public ControlledStates controlled;
	}

	public class StatesInstance : GameInstance
	{
		[MyCmpGet]
		public Operational operational;

		public bool[] previousStorageAllowItemRemovalStates;

		[Serialize]
		public bool isControlled = true;

		public bool isRestrictionApplied;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			isControlled = def.initialControlledStateWhenBuilt;
		}

		public void OnRefreshUserMenu(object data)
		{
			KIconButtonMenu.ButtonInfo buttonInfo = null;
			buttonInfo = ((!isControlled) ? new KIconButtonMenu.ButtonInfo("action_rocket_restriction_controlled", UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.NAME_CONTROLLED, OnChange, global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.TOOLTIP_CONTROLLED) : new KIconButtonMenu.ButtonInfo("action_rocket_restriction_uncontrolled", UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.NAME_UNCONTROLLED, OnChange, global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.TOOLTIP_UNCONTROLLED));
			Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 11f);
		}

		public void ControlStationBuilt(object o)
		{
			base.sm.AquireRocketControlStation(base.smi);
		}

		private void OnChange()
		{
			isControlled = !isControlled;
			GoToRestrictionState();
		}

		public void GoToRestrictionState()
		{
			if (base.smi.isControlled)
			{
				base.smi.GoTo(base.sm.restriction.controlled);
			}
			else
			{
				base.smi.GoTo(base.sm.restriction.uncontrolled);
			}
		}

		public bool BuildingRestrictionsActive()
		{
			if (isControlled && !base.sm.rocketControlStation.IsNull(base.smi))
			{
				return base.sm.rocketControlStation.Get<RocketControlStation>(base.smi).BuildingRestrictionsActive;
			}
			return false;
		}
	}

	public static readonly Operational.Flag rocketUsageAllowed = new Operational.Flag("rocketUsageAllowed", Operational.Flag.Type.Requirement);

	private TargetParameter rocketControlStation;

	public RestrictionStates restriction;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		base.serializable = SerializeType.ParamsOnly;
		root.Enter(delegate(StatesInstance smi)
		{
			if (DlcManager.FeatureClusterSpaceEnabled() && smi.master.gameObject.GetMyWorld().IsModuleInterior)
			{
				smi.Subscribe(493375141, smi.OnRefreshUserMenu);
				smi.GoToRestrictionState();
			}
			else
			{
				smi.StopSM("Not inside rocket or no cluster space");
			}
		});
		restriction.Enter(AquireRocketControlStation).Enter(delegate(StatesInstance smi)
		{
			Components.RocketControlStations.OnAdd += smi.ControlStationBuilt;
		}).Exit(delegate(StatesInstance smi)
		{
			Components.RocketControlStations.OnAdd -= smi.ControlStationBuilt;
		});
		restriction.uncontrolled.ToggleStatusItem(Db.Get().BuildingStatusItems.NoRocketRestriction).Enter(delegate(StatesInstance smi)
		{
			RestrictUsage(smi, restrict: false);
		});
		restriction.controlled.DefaultState(restriction.controlled.nostation);
		restriction.controlled.nostation.Enter(OnRocketRestrictionChanged).ParamTransition(rocketControlStation, restriction.controlled.controlled, GameStateMachine<RocketUsageRestriction, StatesInstance, IStateMachineTarget, Def>.IsNotNull);
		restriction.controlled.controlled.OnTargetLost(rocketControlStation, restriction.controlled.nostation).Enter(OnRocketRestrictionChanged).Target(rocketControlStation)
			.EventHandler(GameHashes.RocketRestrictionChanged, OnRocketRestrictionChanged)
			.Target(masterTarget);
	}

	private void OnRocketRestrictionChanged(StatesInstance smi)
	{
		RestrictUsage(smi, smi.BuildingRestrictionsActive());
	}

	private void RestrictUsage(StatesInstance smi, bool restrict)
	{
		smi.master.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.RocketRestrictionInactive, !restrict && smi.isControlled);
		if (smi.isRestrictionApplied == restrict)
		{
			return;
		}
		smi.isRestrictionApplied = restrict;
		smi.operational.SetFlag(rocketUsageAllowed, !restrict);
		smi.master.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.RocketRestrictionActive, restrict);
		Storage[] components = smi.master.gameObject.GetComponents<Storage>();
		if (components == null || components.Length == 0)
		{
			return;
		}
		for (int i = 0; i < components.Length; i++)
		{
			if (restrict)
			{
				smi.previousStorageAllowItemRemovalStates = new bool[components.Length];
				smi.previousStorageAllowItemRemovalStates[i] = components[i].allowItemRemoval;
				components[i].allowItemRemoval = false;
			}
			else if (smi.previousStorageAllowItemRemovalStates != null && i < smi.previousStorageAllowItemRemovalStates.Length)
			{
				components[i].allowItemRemoval = smi.previousStorageAllowItemRemovalStates[i];
			}
			foreach (GameObject item in components[i].items)
			{
				item.Trigger(-778359855, components[i]);
			}
		}
		Ownable component = smi.master.GetComponent<Ownable>();
		if (restrict && component != null && component.IsAssigned())
		{
			component.Unassign();
		}
	}

	private void AquireRocketControlStation(StatesInstance smi)
	{
		if (!this.rocketControlStation.IsNull(smi))
		{
			return;
		}
		foreach (RocketControlStation rocketControlStation in Components.RocketControlStations)
		{
			if (rocketControlStation.GetMyWorldId() == smi.GetMyWorldId())
			{
				this.rocketControlStation.Set(rocketControlStation, smi);
			}
		}
	}
}
