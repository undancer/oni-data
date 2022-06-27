using System;

public class CritterElementMonitor : GameStateMachine<CritterElementMonitor, CritterElementMonitor.Instance, IStateMachineTarget>
{
	public new class Instance : GameInstance
	{
		public event Action<float> OnUpdateEggChances;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void UpdateCurrentElement(float dt)
		{
			this.OnUpdateEggChances(dt);
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.Update("UpdateInElement", delegate(Instance smi, float dt)
		{
			smi.UpdateCurrentElement(dt);
		}, UpdateRate.SIM_1000ms);
	}
}
