public class ColonyRationMonitor : GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			UpdateIsOutOfRations();
		}

		public void UpdateIsOutOfRations()
		{
			bool value = true;
			foreach (Edible item in Components.Edibles.Items)
			{
				if (item.GetComponent<Pickupable>().UnreservedAmount > 0f)
				{
					value = false;
					break;
				}
			}
			base.smi.sm.isOutOfRations.Set(value, base.smi);
		}

		public bool IsOutOfRations()
		{
			return base.smi.sm.isOutOfRations.Get(base.smi);
		}
	}

	public State satisfied;

	public State outofrations;

	private BoolParameter isOutOfRations = new BoolParameter();

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.Update("UpdateOutOfRations", delegate(Instance smi, float dt)
		{
			smi.UpdateIsOutOfRations();
		});
		satisfied.ParamTransition(isOutOfRations, outofrations, GameStateMachine<ColonyRationMonitor, Instance, IStateMachineTarget, object>.IsTrue).TriggerOnEnter(GameHashes.ColonyHasRationsChanged);
		outofrations.ParamTransition(isOutOfRations, satisfied, GameStateMachine<ColonyRationMonitor, Instance, IStateMachineTarget, object>.IsFalse).TriggerOnEnter(GameHashes.ColonyHasRationsChanged);
	}
}
