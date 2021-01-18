using UnityEngine;

public class ReturnToChargeStationStates : GameStateMachine<ReturnToChargeStationStates, ReturnToChargeStationStates.Instance, IStateMachineTarget, ReturnToChargeStationStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Robots.Behaviours.RechargeBehaviour);
		}

		public bool ChargeAborted()
		{
			return base.smi.sm.GetSweepLocker(base.smi) == null || !base.smi.sm.GetSweepLocker(base.smi).GetComponent<Operational>().IsActive;
		}

		public bool StationReadyToCharge()
		{
			return base.smi.sm.GetSweepLocker(base.smi) != null && base.smi.sm.GetSweepLocker(base.smi).GetComponent<Operational>().IsActive;
		}
	}

	public class ChargingStates : State
	{
		public State waitingForCharging;

		public State charging;

		public State interupted;

		public State completed;
	}

	public State emote;

	public State idle;

	public State movingToChargingStation;

	public State behaviourcomplete;

	public ChargingStates chargingstates;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = emote;
		emote.ToggleStatusItem(Db.Get().RobotStatusItems.MovingToChargeStation, (Instance smi) => smi.gameObject, Db.Get().StatusItemCategories.Main).PlayAnim("react_lobatt", KAnim.PlayMode.Once).OnAnimQueueComplete(movingToChargingStation);
		idle.ToggleStatusItem(Db.Get().RobotStatusItems.MovingToChargeStation, (Instance smi) => smi.gameObject, Db.Get().StatusItemCategories.Main).ScheduleGoTo(1f, movingToChargingStation);
		movingToChargingStation.ToggleStatusItem(Db.Get().RobotStatusItems.MovingToChargeStation, (Instance smi) => smi.gameObject, Db.Get().StatusItemCategories.Main).MoveTo(delegate(Instance smi)
		{
			Storage sweepLocker = GetSweepLocker(smi);
			return (sweepLocker == null) ? Grid.InvalidCell : Grid.PosToCell(sweepLocker);
		}, chargingstates.waitingForCharging, idle);
		chargingstates.Enter(delegate(Instance smi)
		{
			smi.master.GetComponent<Facing>().Face(GetSweepLocker(smi).gameObject.transform.position + Vector3.right);
			Vector3 position2 = smi.transform.GetPosition();
			position2.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingUse);
			smi.transform.SetPosition(position2);
			KBatchedAnimController component2 = smi.GetComponent<KBatchedAnimController>();
			component2.enabled = false;
			component2.enabled = true;
		}).Exit(delegate(Instance smi)
		{
			Vector3 position = smi.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			smi.transform.SetPosition(position);
			KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
			component.enabled = false;
			component.enabled = true;
		}).Enter(delegate(Instance smi)
		{
			Station_DockRobot(smi, dockState: true);
		})
			.Exit(delegate(Instance smi)
			{
				Station_DockRobot(smi, dockState: false);
			});
		chargingstates.waitingForCharging.PlayAnim("react_base", KAnim.PlayMode.Loop).TagTransition(GameTags.Robots.Behaviours.RechargeBehaviour, chargingstates.completed, on_remove: true).Transition(chargingstates.charging, (Instance smi) => smi.StationReadyToCharge());
		chargingstates.charging.TagTransition(GameTags.Robots.Behaviours.RechargeBehaviour, chargingstates.completed, on_remove: true).Transition(chargingstates.interupted, (Instance smi) => !smi.StationReadyToCharge()).ToggleEffect("Charging")
			.PlayAnim("sleep_pre")
			.QueueAnim("sleep_idle", loop: true)
			.Enter(Station_StartCharging)
			.Exit(Station_StopCharging);
		chargingstates.interupted.PlayAnim("sleep_pst").TagTransition(GameTags.Robots.Behaviours.RechargeBehaviour, chargingstates.completed, on_remove: true).OnAnimQueueComplete(chargingstates.waitingForCharging);
		chargingstates.completed.PlayAnim("sleep_pst").OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Robots.Behaviours.RechargeBehaviour);
	}

	public Storage GetSweepLocker(Instance smi)
	{
		StorageUnloadMonitor.Instance sMI = smi.master.gameObject.GetSMI<StorageUnloadMonitor.Instance>();
		return sMI?.sm.sweepLocker.Get(sMI);
	}

	public void Station_StartCharging(Instance smi)
	{
		Storage sweepLocker = GetSweepLocker(smi);
		if (sweepLocker != null)
		{
			sweepLocker.GetComponent<SweepBotStation>().StartCharging();
		}
	}

	public void Station_StopCharging(Instance smi)
	{
		Storage sweepLocker = GetSweepLocker(smi);
		if (sweepLocker != null)
		{
			sweepLocker.GetComponent<SweepBotStation>().StopCharging();
		}
	}

	public void Station_DockRobot(Instance smi, bool dockState)
	{
		Storage sweepLocker = GetSweepLocker(smi);
		if (sweepLocker != null)
		{
			sweepLocker.GetComponent<SweepBotStation>().DockRobot(dockState);
		}
	}
}
