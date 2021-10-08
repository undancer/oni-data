[SkipSaveFileSerialization]
public class Loner : StateMachineComponent<Loner.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Loner, object>.GameInstance
	{
		public StatesInstance(Loner master)
			: base(master)
		{
		}

		public bool IsAlone()
		{
			WorldContainer myWorld = this.GetMyWorld();
			if (!myWorld)
			{
				return false;
			}
			int parentWorldId = myWorld.ParentWorldId;
			int id = myWorld.id;
			MinionIdentity component = GetComponent<MinionIdentity>();
			foreach (MinionIdentity liveMinionIdentity in Components.LiveMinionIdentities)
			{
				if (component != liveMinionIdentity)
				{
					int myWorldId = liveMinionIdentity.GetMyWorldId();
					if (id == myWorldId || parentWorldId == myWorldId)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Loner>
	{
		public State idle;

		public State alone;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.Enter(delegate(StatesInstance smi)
			{
				if (smi.IsAlone())
				{
					smi.GoTo(alone);
				}
			});
			idle.EventTransition(GameHashes.MinionMigration, (StatesInstance smi) => Game.Instance, alone, (StatesInstance smi) => smi.IsAlone()).EventTransition(GameHashes.MinionDelta, (StatesInstance smi) => Game.Instance, alone, (StatesInstance smi) => smi.IsAlone());
			alone.EventTransition(GameHashes.MinionMigration, (StatesInstance smi) => Game.Instance, idle, (StatesInstance smi) => !smi.IsAlone()).EventTransition(GameHashes.MinionDelta, (StatesInstance smi) => Game.Instance, idle, (StatesInstance smi) => !smi.IsAlone()).ToggleEffect("Loner");
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}
}
