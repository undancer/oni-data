using STRINGS;

public class DefendStates : GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Defend);
		}
	}

	public class ProtectStates : State
	{
		public ApproachSubState<AttackableBase> moveToThreat;

		public State attackThreat;
	}

	public TargetParameter target;

	public ProtectStates protectEntity;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = protectEntity.moveToThreat;
		root.Enter("SetTarget", delegate(Instance smi)
		{
			target.Set(smi.GetSMI<EggProtectionMonitor.Instance>().MainThreat, smi);
		}).ToggleStatusItem(CREATURES.STATUSITEMS.ATTACKINGENTITY.NAME, CREATURES.STATUSITEMS.ATTACKINGENTITY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		protectEntity.moveToThreat.InitializeStates(masterTarget, target, protectEntity.attackThreat, null, new CellOffset[5]
		{
			new CellOffset(0, 0),
			new CellOffset(1, 0),
			new CellOffset(-1, 0),
			new CellOffset(1, 1),
			new CellOffset(-1, 1)
		});
		protectEntity.attackThreat.Enter(delegate(Instance smi)
		{
			smi.Play("slap_pre");
			smi.Queue("slap");
			smi.Queue("slap_pst");
			smi.Schedule(0.5f, delegate
			{
				smi.GetComponent<Weapon>().AttackTarget(target.Get(smi));
			});
		}).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Defend);
	}
}
