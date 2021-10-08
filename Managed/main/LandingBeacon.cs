public class LandingBeacon : GameStateMachine<LandingBeacon, LandingBeacon.Instance>
{
	public class Def : BaseDef
	{
	}

	public class WorkingStates : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public new class Instance : GameInstance
	{
		public Operational operational;

		public KSelectable selectable;

		public bool skyLastVisible = true;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, (object)def)
		{
			Components.LandingBeacons.Add(this);
			operational = GetComponent<Operational>();
			selectable = GetComponent<KSelectable>();
		}

		public override void StartSM()
		{
			base.StartSM();
			UpdateLineOfSight(this, 0f);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			Components.LandingBeacons.Remove(this);
		}

		public bool CanBeTargeted()
		{
			return IsInsideState(base.sm.working);
		}
	}

	public State off;

	public WorkingStates working;

	public static readonly Operational.Flag noSurfaceSight = new Operational.Flag("noSurfaceSight", Operational.Flag.Type.Requirement);

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		root.Update(UpdateLineOfSight);
		off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, working, (Instance smi) => smi.operational.IsOperational);
		working.DefaultState(working.pre).EventTransition(GameHashes.OperationalChanged, off, (Instance smi) => !smi.operational.IsOperational);
		working.pre.PlayAnim("working_pre").OnAnimQueueComplete(working.loop);
		working.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).Enter("SetActive", delegate(Instance smi)
		{
			smi.operational.SetActive(value: true);
		}).Exit("SetActive", delegate(Instance smi)
		{
			smi.operational.SetActive(value: false);
		});
	}

	public static void UpdateLineOfSight(Instance smi, float dt)
	{
		WorldContainer myWorld = smi.GetMyWorld();
		bool flag = true;
		int num = Grid.PosToCell(smi);
		int num2 = (int)myWorld.maximumBounds.y;
		while (Grid.CellRow(num) <= num2)
		{
			if (!Grid.IsValidCell(num) || Grid.Solid[num])
			{
				flag = false;
				break;
			}
			num = Grid.CellAbove(num);
		}
		if (smi.skyLastVisible != flag)
		{
			smi.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoSurfaceSight, !flag);
			smi.operational.SetFlag(noSurfaceSight, flag);
			smi.skyLastVisible = flag;
		}
	}
}
