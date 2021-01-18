using STRINGS;

public class AttackStates : GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Attack);
		}
	}

	public TargetParameter target;

	public ApproachSubState<AttackableBase> approach;

	public State attack;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = approach;
		root.Enter("SetTarget", delegate(Instance smi)
		{
			target.Set(smi.GetSMI<ThreatMonitor.Instance>().MainThreat, smi);
		});
		approach.InitializeStates(masterTarget, target, attack, null, new CellOffset[5]
		{
			new CellOffset(0, 0),
			new CellOffset(1, 0),
			new CellOffset(-1, 0),
			new CellOffset(1, 1),
			new CellOffset(-1, 1)
		}).ToggleStatusItem(CREATURES.STATUSITEMS.ATTACK_APPROACH.NAME, CREATURES.STATUSITEMS.ATTACK_APPROACH.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		attack.Enter(delegate(Instance smi)
		{
			smi.Play("eat_pre");
			smi.Queue("eat_pst");
			smi.Schedule(0.5f, delegate
			{
				smi.GetComponent<Weapon>().AttackTarget(target.Get(smi));
			});
		}).ToggleStatusItem(CREATURES.STATUSITEMS.ATTACK.NAME, CREATURES.STATUSITEMS.ATTACK.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Attack);
	}
}
