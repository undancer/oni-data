using STRINGS;
using UnityEngine;

public class IdleStates : GameStateMachine<IdleStates, IdleStates.Instance, IStateMachineTarget, IdleStates.Def>
{
	public class Def : BaseDef
	{
		public delegate HashedString IdleAnimCallback(Instance smi, ref HashedString pre_anim);

		public IdleAnimCallback customIdleAnim;
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
		}
	}

	public class MoveCellQuery : PathFinderQuery
	{
		private NavType navType;

		private int targetCell = Grid.InvalidCell;

		private int maxIterations;

		public bool allowLiquid
		{
			get;
			set;
		}

		public MoveCellQuery(NavType navType)
		{
			this.navType = navType;
			maxIterations = Random.Range(5, 25);
		}

		public override bool IsMatch(int cell, int parent_cell, int cost)
		{
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			bool flag = navType != NavType.Swim;
			bool flag2 = navType == NavType.Swim || allowLiquid;
			bool flag3 = Grid.IsSubstantialLiquid(cell);
			if (flag3 && !flag2)
			{
				return false;
			}
			if (!flag3 && !flag)
			{
				return false;
			}
			targetCell = cell;
			return --maxIterations <= 0;
		}

		public override int GetResultCell()
		{
			return targetCell;
		}
	}

	private State loop;

	private State move;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = loop;
		root.Exit("StopNavigator", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().Stop();
		}).ToggleStatusItem(CREATURES.STATUSITEMS.IDLE.NAME, CREATURES.STATUSITEMS.IDLE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).ToggleTag(GameTags.Idle);
		loop.Enter(PlayIdle).ToggleScheduleCallback("IdleMove", (Instance smi) => Random.Range(3, 10), delegate(Instance smi)
		{
			smi.GoTo(move);
		});
		move.Enter(MoveToNewCell).EventTransition(GameHashes.DestinationReached, loop).EventTransition(GameHashes.NavigationFailed, loop);
	}

	public void MoveToNewCell(Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		MoveCellQuery moveCellQuery = new MoveCellQuery(component.CurrentNavType)
		{
			allowLiquid = smi.gameObject.HasTag(GameTags.Amphibious)
		};
		component.RunQuery(moveCellQuery);
		component.GoTo(moveCellQuery.GetResultCell());
	}

	public void PlayIdle(Instance smi)
	{
		KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
		Navigator component2 = smi.GetComponent<Navigator>();
		NavType nav_type = component2.CurrentNavType;
		if (smi.GetComponent<Facing>().GetFacing())
		{
			nav_type = NavGrid.MirrorNavType(nav_type);
		}
		if (smi.def.customIdleAnim != null)
		{
			HashedString pre_anim = HashedString.Invalid;
			HashedString hashedString = smi.def.customIdleAnim(smi, ref pre_anim);
			if (hashedString != HashedString.Invalid)
			{
				if (pre_anim != HashedString.Invalid)
				{
					component.Play(pre_anim);
				}
				component.Queue(hashedString, KAnim.PlayMode.Loop);
				return;
			}
		}
		HashedString idleAnim = component2.NavGrid.GetIdleAnim(nav_type);
		component.Play(idleAnim, KAnim.PlayMode.Loop);
	}
}
