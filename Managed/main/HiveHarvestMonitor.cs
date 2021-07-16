using STRINGS;

public class HiveHarvestMonitor : GameStateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>
{
	public class Def : BaseDef
	{
		public Tag producedOre;

		public float harvestThreshold;
	}

	public class HarvestStates : State
	{
		public State not_ready;

		public State ready;
	}

	public new class Instance : GameInstance
	{
		[MyCmpReq]
		public Storage storage;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void OnRefreshUserMenu()
		{
			if (base.sm.shouldHarvest.Get(this))
			{
				Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_building_disabled", UI.USERMENUACTIONS.CANCELEMPTYBEEHIVE.NAME, delegate
				{
					base.sm.shouldHarvest.Set(value: false, this);
				}, global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCELEMPTYBEEHIVE.TOOLTIP));
			}
			else
			{
				Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYBEEHIVE.NAME, delegate
				{
					base.sm.shouldHarvest.Set(value: true, this);
				}, global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYBEEHIVE.TOOLTIP));
			}
		}

		public Chore CreateHarvestChore()
		{
			return new WorkChore<HiveWorkableEmpty>(Db.Get().ChoreTypes.Ranch, base.master.GetComponent<HiveWorkableEmpty>(), null, run_until_complete: true, base.smi.OnEmptyComplete);
		}

		public void OnEmptyComplete(Chore chore)
		{
			base.smi.storage.Drop(base.smi.def.producedOre);
		}
	}

	public BoolParameter shouldHarvest;

	public State do_not_harvest;

	public HarvestStates harvest;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = do_not_harvest;
		base.serializable = SerializeType.ParamsOnly;
		root.EventHandler(GameHashes.RefreshUserMenu, delegate(Instance smi)
		{
			smi.OnRefreshUserMenu();
		});
		do_not_harvest.ParamTransition(shouldHarvest, harvest, (Instance smi, bool bShouldHarvest) => bShouldHarvest);
		harvest.ParamTransition(shouldHarvest, do_not_harvest, (Instance smi, bool bShouldHarvest) => !bShouldHarvest).DefaultState(harvest.not_ready);
		harvest.not_ready.EventTransition(GameHashes.OnStorageChange, harvest.ready, (Instance smi) => smi.storage.GetMassAvailable(smi.def.producedOre) >= smi.def.harvestThreshold);
		harvest.ready.ToggleChore((Instance smi) => smi.CreateHarvestChore(), harvest.not_ready).EventTransition(GameHashes.OnStorageChange, harvest.not_ready, (Instance smi) => smi.storage.GetMassAvailable(smi.def.producedOre) < smi.def.harvestThreshold);
	}
}
