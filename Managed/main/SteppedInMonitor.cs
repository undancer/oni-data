using Klei.AI;
using UnityEngine;

public class SteppedInMonitor : GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Effects effects;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			effects = GetComponent<Effects>();
		}
	}

	public State satisfied;

	public State carpetedFloor;

	public State wetFloor;

	public State wetBody;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.Transition(carpetedFloor, IsOnCarpet).Transition(wetFloor, IsFloorWet).Transition(wetBody, IsSubmerged);
		carpetedFloor.Enter(GetCarpetFeet).ToggleExpression(Db.Get().Expressions.Tickled).Update(GetCarpetFeet, UpdateRate.SIM_1000ms)
			.Transition(satisfied, GameStateMachine<SteppedInMonitor, Instance, IStateMachineTarget, object>.Not(IsOnCarpet))
			.Transition(wetFloor, IsFloorWet)
			.Transition(wetBody, IsSubmerged);
		wetFloor.Enter(GetWetFeet).Update(GetWetFeet, UpdateRate.SIM_1000ms).Transition(satisfied, GameStateMachine<SteppedInMonitor, Instance, IStateMachineTarget, object>.Not(IsFloorWet))
			.Transition(wetBody, IsSubmerged);
		wetBody.Enter(GetSoaked).Update(GetSoaked, UpdateRate.SIM_1000ms).Transition(wetFloor, GameStateMachine<SteppedInMonitor, Instance, IStateMachineTarget, object>.Not(IsSubmerged));
	}

	private static void GetCarpetFeet(Instance smi, float dt)
	{
		GetCarpetFeet(smi);
	}

	private static void GetCarpetFeet(Instance smi)
	{
		if (!smi.effects.HasEffect("SoakingWet") && !smi.effects.HasEffect("WetFeet"))
		{
			smi.effects.Add("CarpetFeet", should_save: true);
		}
	}

	private static void GetWetFeet(Instance smi, float dt)
	{
		GetWetFeet(smi);
	}

	private static void GetWetFeet(Instance smi)
	{
		if (!smi.effects.HasEffect("CarpetFeet"))
		{
			smi.effects.Remove("CarpetFeet");
		}
		if (!smi.effects.HasEffect("SoakingWet"))
		{
			smi.effects.Add("WetFeet", should_save: true);
		}
	}

	private static void GetSoaked(Instance smi, float dt)
	{
		GetSoaked(smi);
	}

	private static void GetSoaked(Instance smi)
	{
		if (!smi.effects.HasEffect("CarpetFeet"))
		{
			smi.effects.Remove("CarpetFeet");
		}
		if (smi.effects.HasEffect("WetFeet"))
		{
			smi.effects.Remove("WetFeet");
		}
		smi.effects.Add("SoakingWet", should_save: true);
	}

	private static bool IsOnCarpet(Instance smi)
	{
		int cell = Grid.CellBelow(Grid.PosToCell(smi));
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		GameObject gameObject = Grid.Objects[cell, 9];
		if (Grid.IsValidCell(cell) && gameObject != null)
		{
			return gameObject.HasTag(GameTags.Carpeted);
		}
		return false;
	}

	private static bool IsFloorWet(Instance smi)
	{
		int num = Grid.PosToCell(smi);
		if (Grid.IsValidCell(num))
		{
			return Grid.Element[num].IsLiquid;
		}
		return false;
	}

	private static bool IsSubmerged(Instance smi)
	{
		int num = Grid.CellAbove(Grid.PosToCell(smi));
		if (Grid.IsValidCell(num))
		{
			return Grid.Element[num].IsLiquid;
		}
		return false;
	}
}
