using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

internal class NestingPoopState : GameStateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>
{
	public class Def : BaseDef
	{
		public Tag nestingPoopElement = Tag.Invalid;

		public Def(Tag tag)
		{
			nestingPoopElement = tag;
		}
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		private int lastPoopCell = -1;

		public int targetPoopCell = -1;

		private Tag currentlyPoopingElement = Tag.Invalid;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Poop);
		}

		private static bool IsValidNestingCell(int cell, object arg)
		{
			if (Grid.IsValidCell(cell) && !Grid.Solid[cell] && Grid.Solid[Grid.CellBelow(cell)])
			{
				if (!IsValidPoopFromCell(cell, look_left: true))
				{
					return IsValidPoopFromCell(cell, look_left: false);
				}
				return true;
			}
			return false;
		}

		private static bool IsValidPoopFromCell(int cell, bool look_left)
		{
			if (look_left)
			{
				int num = Grid.CellDownLeft(cell);
				int num2 = Grid.CellLeft(cell);
				if (Grid.IsValidCell(num) && Grid.Solid[num] && Grid.IsValidCell(num2))
				{
					return !Grid.Solid[num2];
				}
				return false;
			}
			int num3 = Grid.CellDownRight(cell);
			int num4 = Grid.CellRight(cell);
			if (Grid.IsValidCell(num3) && Grid.Solid[num3] && Grid.IsValidCell(num4))
			{
				return !Grid.Solid[num4];
			}
			return false;
		}

		public int GetPoopPosition()
		{
			targetPoopCell = GetTargetPoopCell();
			List<Direction> list = new List<Direction>();
			if (IsValidPoopFromCell(targetPoopCell, look_left: true))
			{
				list.Add(Direction.Left);
			}
			if (IsValidPoopFromCell(targetPoopCell, look_left: false))
			{
				list.Add(Direction.Right);
			}
			if (list.Count > 0)
			{
				Direction d = list[Random.Range(0, list.Count)];
				int cellInDirection = Grid.GetCellInDirection(targetPoopCell, d);
				if (Grid.IsValidCell(cellInDirection))
				{
					return cellInDirection;
				}
			}
			if (Grid.IsValidCell(targetPoopCell))
			{
				return targetPoopCell;
			}
			if (!Grid.IsValidCell(Grid.PosToCell(this)))
			{
				Debug.LogWarning("This is bad, how is Mole occupying an invalid cell?");
			}
			return Grid.PosToCell(this);
		}

		private int GetTargetPoopCell()
		{
			CreatureCalorieMonitor.Instance sMI = base.smi.GetSMI<CreatureCalorieMonitor.Instance>();
			currentlyPoopingElement = sMI.stomach.GetNextPoopEntry();
			int num = GameUtil.FloodFillFind<object>(start_cell: (!(currentlyPoopingElement == base.smi.def.nestingPoopElement) || !(base.smi.def.nestingPoopElement != Tag.Invalid) || lastPoopCell == -1) ? Grid.PosToCell(this) : lastPoopCell, fn: IsValidNestingCell, arg: null, max_depth: 8, stop_at_solid: false, stop_at_liquid: true);
			if (num == -1)
			{
				CellOffset[] array = new CellOffset[5]
				{
					new CellOffset(0, 0),
					new CellOffset(-1, 0),
					new CellOffset(1, 0),
					new CellOffset(-1, -1),
					new CellOffset(1, -1)
				};
				num = Grid.OffsetCell(lastPoopCell, array[Random.Range(0, array.Length)]);
				int num2 = Grid.CellAbove(num);
				while (Grid.IsValidCell(num2) && Grid.Solid[num2])
				{
					num = num2;
					num2 = Grid.CellAbove(num);
				}
			}
			return num;
		}

		public void SetLastPoopCell()
		{
			if (currentlyPoopingElement == base.smi.def.nestingPoopElement)
			{
				lastPoopCell = Grid.PosToCell(this);
			}
		}
	}

	public State goingtopoop;

	public State pooping;

	public State behaviourcomplete;

	public State failedtonest;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = goingtopoop;
		goingtopoop.MoveTo((Instance smi) => smi.GetPoopPosition(), pooping, failedtonest);
		failedtonest.Enter(delegate(Instance smi)
		{
			smi.SetLastPoopCell();
		}).GoTo(pooping);
		pooping.Enter(delegate(Instance smi)
		{
			smi.master.GetComponent<Facing>().SetFacing(Grid.PosToCell(smi.master.gameObject) > smi.targetPoopCell);
		}).ToggleStatusItem(CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).PlayAnim("poop")
			.OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.Enter(delegate(Instance smi)
		{
			smi.SetLastPoopCell();
		}).PlayAnim("idle_loop", KAnim.PlayMode.Loop).BehaviourComplete(GameTags.Creatures.Poop);
	}
}
