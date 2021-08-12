using Klei.AI;
using UnityEngine;

public class StaterpillarGenerator : Generator
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, StaterpillarGenerator, object>.GameInstance
	{
		private Attributes attributes;

		public StatesInstance(StaterpillarGenerator master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, StaterpillarGenerator>
	{
		public State idle;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			root.EventTransition(GameHashes.OperationalChanged, idle, (StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			idle.EventTransition(GameHashes.OperationalChanged, root, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational).Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: true);
			});
		}
	}

	private StatesInstance smi;

	protected override void OnSpawn()
	{
		smi = new StatesInstance(this);
		smi.StartSM();
		base.OnSpawn();
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		ushort circuitID = base.CircuitID;
		operational.SetFlag(Generator.wireConnectedFlag, circuitID != ushort.MaxValue);
		if (operational.IsOperational)
		{
			float wattageRating = GetComponent<Generator>().WattageRating;
			if (wattageRating > 0f)
			{
				wattageRating *= dt;
				wattageRating = Mathf.Max(wattageRating, 1f * dt);
				GenerateJoules(wattageRating);
			}
		}
	}
}
