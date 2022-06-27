using STRINGS;
using UnityEngine;

public class FlopStates : GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public float currentDir = 1f;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Flopping);
		}
	}

	private State flop_pre;

	private State flop_cycle;

	private State pst;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = flop_pre;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.FLOPPING.NAME, CREATURES.STATUSITEMS.FLOPPING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		flop_pre.Enter(ChooseDirection).Transition(flop_cycle, ShouldFlop).Transition(pst, GameStateMachine<FlopStates, Instance, IStateMachineTarget, Def>.Not(ShouldFlop));
		flop_cycle.PlayAnim("flop_loop", KAnim.PlayMode.Once).Transition(pst, IsSubstantialLiquid).Update("Flop", FlopForward, UpdateRate.SIM_33ms)
			.OnAnimQueueComplete(flop_pre);
		pst.QueueAnim("flop_loop", loop: true).BehaviourComplete(GameTags.Creatures.Flopping);
	}

	public static bool ShouldFlop(Instance smi)
	{
		int num = Grid.CellBelow(Grid.PosToCell(smi.transform.GetPosition()));
		if (Grid.IsValidCell(num))
		{
			return Grid.Solid[num];
		}
		return false;
	}

	public static void ChooseDirection(Instance smi)
	{
		int cell = Grid.PosToCell(smi.transform.GetPosition());
		if (SearchForLiquid(cell, 1))
		{
			smi.currentDir = 1f;
		}
		else if (SearchForLiquid(cell, -1))
		{
			smi.currentDir = -1f;
		}
		else if (Random.value > 0.5f)
		{
			smi.currentDir = 1f;
		}
		else
		{
			smi.currentDir = -1f;
		}
	}

	private static bool SearchForLiquid(int cell, int delta_x)
	{
		while (true)
		{
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			if (Grid.IsSubstantialLiquid(cell))
			{
				return true;
			}
			if (Grid.Solid[cell])
			{
				return false;
			}
			if (Grid.CritterImpassable[cell])
			{
				break;
			}
			int num = Grid.CellBelow(cell);
			cell = ((!Grid.IsValidCell(num) || !Grid.Solid[num]) ? num : (cell + delta_x));
		}
		return false;
	}

	public static void FlopForward(Instance smi, float dt)
	{
		KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
		int currentFrame = component.currentFrame;
		if (!component.IsVisible() || (currentFrame >= 23 && currentFrame <= 36))
		{
			Vector3 position = smi.transform.GetPosition();
			Vector3 vector = position;
			vector.x = position.x + smi.currentDir * dt * 1f;
			int num = Grid.PosToCell(vector);
			if (Grid.IsValidCell(num) && !Grid.Solid[num] && !Grid.CritterImpassable[num])
			{
				smi.transform.SetPosition(vector);
			}
			else
			{
				smi.currentDir = 0f - smi.currentDir;
			}
		}
	}

	public static bool IsSubstantialLiquid(Instance smi)
	{
		return Grid.IsSubstantialLiquid(Grid.PosToCell(smi.transform.GetPosition()));
	}
}
