public class TeleportalPad : StateMachineComponent<TeleportalPad.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, TeleportalPad, object>.GameInstance
	{
		public StatesInstance(TeleportalPad master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, TeleportalPad>
	{
		public class PortalOnStates : State
		{
			public State turn_on;

			public State loop;

			public State turn_off;
		}

		public Signal targetTeleporter;

		public Signal doTeleport;

		public State inactive;

		public State no_target;

		public PortalOnStates portal_on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = inactive;
			base.serializable = SerializeType.Both_DEPRECATED;
			root.EventTransition(GameHashes.OperationalChanged, inactive, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			inactive.PlayAnim("idle").EventTransition(GameHashes.OperationalChanged, no_target, (StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			no_target.Enter(delegate(StatesInstance smi)
			{
				if (smi.master.GetComponent<Teleporter>().HasTeleporterTarget())
				{
					smi.GoTo(portal_on.turn_on);
				}
			}).PlayAnim("idle").EventTransition(GameHashes.TeleporterIDsChanged, portal_on.turn_on, (StatesInstance smi) => smi.master.GetComponent<Teleporter>().HasTeleporterTarget());
			portal_on.EventTransition(GameHashes.TeleporterIDsChanged, portal_on.turn_off, (StatesInstance smi) => !smi.master.GetComponent<Teleporter>().HasTeleporterTarget());
			portal_on.turn_on.PlayAnim("working_pre").OnAnimQueueComplete(portal_on.loop);
			portal_on.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).Update(delegate(StatesInstance smi, float dt)
			{
				Teleporter component = smi.master.GetComponent<Teleporter>();
				Teleporter teleporter = component.FindTeleportTarget();
				component.SetTeleportTarget(teleporter);
				if (teleporter != null)
				{
					component.TeleportObjects();
				}
			});
			portal_on.turn_off.PlayAnim("working_pst").OnAnimQueueComplete(no_target);
		}
	}

	[MyCmpReq]
	private Operational operational;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
