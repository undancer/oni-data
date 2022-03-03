public class RobotAi : GameStateMachine<RobotAi, RobotAi.Instance>
{
	public class Def : BaseDef
	{
	}

	public class AliveStates : State
	{
		public State normal;

		public State stored;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, (object)def)
		{
			ChoreConsumer component = GetComponent<ChoreConsumer>();
			component.AddUrge(Db.Get().Urges.EmoteHighPriority);
			component.AddUrge(Db.Get().Urges.EmoteIdle);
		}

		public void RefreshUserMenu()
		{
			Game.Instance.userMenu.Refresh(base.master.gameObject);
		}
	}

	public AliveStates alive;

	public State dead;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleStateMachine((Instance smi) => new DeathMonitor.Instance(smi.master, new DeathMonitor.Def())).Enter(delegate(Instance smi)
		{
			if (smi.HasTag(GameTags.Dead))
			{
				smi.GoTo(dead);
			}
			else
			{
				smi.GoTo(alive);
			}
		});
		alive.DefaultState(alive.normal).TagTransition(GameTags.Dead, dead);
		alive.normal.TagTransition(GameTags.Stored, alive.stored).ToggleStateMachine((Instance smi) => new FallMonitor.Instance(smi.master, shouldPlayEmotes: false));
		alive.stored.PlayAnim("in_storage").TagTransition(GameTags.Stored, alive.normal, on_remove: true).ToggleBrain("stored")
			.Enter(delegate(Instance smi)
			{
				smi.GetComponent<Navigator>().Pause("stored");
			})
			.Exit(delegate(Instance smi)
			{
				smi.GetComponent<Navigator>().Unpause("unstored");
			});
		dead.ToggleBrain("dead").ToggleComponent<Deconstructable>().ToggleStateMachine((Instance smi) => new FallWhenDeadMonitor.Instance(smi.master))
			.Enter("RefreshUserMenu", delegate(Instance smi)
			{
				smi.RefreshUserMenu();
			})
			.Enter("DropStorage", delegate(Instance smi)
			{
				smi.GetComponent<Storage>().DropAll();
			});
	}
}
