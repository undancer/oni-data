using System.Collections.Generic;
using UnityEngine;

public class CropSleepingMonitor : GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public float lightIntensityThreshold = 0f;

		public bool prefersDarkness = false;

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			return null;
		}
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public bool IsSleeping()
		{
			BaseState currentState = GetCurrentState();
			return currentState == base.smi.sm.sleeping;
		}

		public bool IsCellSafe(int cell)
		{
			float num = Grid.LightIntensity[cell];
			return base.def.prefersDarkness ? (num <= base.def.lightIntensityThreshold) : (num >= base.def.lightIntensityThreshold);
		}
	}

	public State sleeping;

	public State awake;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = awake;
		base.serializable = SerializeType.Never;
		root.Update("CropSleepingMonitor.root", delegate(Instance smi, float dt)
		{
			int cell = Grid.PosToCell(smi.master.gameObject);
			State state = (smi.IsCellSafe(cell) ? awake : sleeping);
			smi.GoTo(state);
		}, UpdateRate.SIM_1000ms);
		sleeping.TriggerOnEnter(GameHashes.CropSleep).ToggleStatusItem(Db.Get().CreatureStatusItems.CropSleeping, (Instance smi) => smi);
		awake.TriggerOnEnter(GameHashes.CropWakeUp);
	}
}
