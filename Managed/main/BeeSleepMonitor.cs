using UnityEngine;

public class BeeSleepMonitor : GameStateMachine<BeeSleepMonitor, BeeSleepMonitor.Instance, IStateMachineTarget, BeeSleepMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public float CO2Exposure = 0f;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.Update(UpdateCO2Exposure, UpdateRate.SIM_1000ms).ToggleBehaviour(GameTags.Creatures.BeeWantsToSleep, ShouldSleep);
	}

	public bool ShouldSleep(Instance smi)
	{
		return smi.CO2Exposure >= 5f;
	}

	public void UpdateCO2Exposure(Instance smi, float dt)
	{
		if (IsInCO2(smi))
		{
			smi.CO2Exposure += 1f;
		}
		else
		{
			smi.CO2Exposure -= 0.5f;
		}
		smi.CO2Exposure = Mathf.Clamp(smi.CO2Exposure, 0f, 10f);
	}

	public bool IsInCO2(Instance smi)
	{
		int num = Grid.PosToCell(smi.gameObject);
		if (Grid.IsValidCell(num))
		{
			Element element = Grid.Element[num];
			if (element.id == SimHashes.CarbonDioxide)
			{
				return true;
			}
		}
		return false;
	}
}
