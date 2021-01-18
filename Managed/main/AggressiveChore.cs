using System;
using STRINGS;
using UnityEngine;

public class AggressiveChore : Chore<AggressiveChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, AggressiveChore, object>.GameInstance
	{
		public StatesInstance(AggressiveChore master, GameObject breaker)
			: base(master)
		{
			base.sm.breaker.Set(breaker, base.smi);
		}

		public void FindBreakable()
		{
			Navigator navigator = GetComponent<Navigator>();
			int num = int.MaxValue;
			Breakable breakable = null;
			if (UnityEngine.Random.Range(0, 100) >= 50)
			{
				foreach (Breakable item in Components.Breakables.Items)
				{
					if (!(item == null) && !item.isBroken())
					{
						int navigationCost = navigator.GetNavigationCost(item);
						if (navigationCost != -1 && navigationCost < num)
						{
							num = navigationCost;
							breakable = item;
						}
					}
				}
			}
			if (breakable == null)
			{
				int value = GameUtil.FloodFillFind(delegate(int cell, object arg)
				{
					if (Grid.Solid[cell])
					{
						return false;
					}
					if (!navigator.CanReach(cell))
					{
						return false;
					}
					if (Grid.IsValidCell(Grid.CellLeft(cell)) && Grid.Solid[Grid.CellLeft(cell)])
					{
						return true;
					}
					if (Grid.IsValidCell(Grid.CellRight(cell)) && Grid.Solid[Grid.CellRight(cell)])
					{
						return true;
					}
					if (Grid.IsValidCell(Grid.OffsetCell(cell, 1, 1)) && Grid.Solid[Grid.OffsetCell(cell, 1, 1)])
					{
						return true;
					}
					return (Grid.IsValidCell(Grid.OffsetCell(cell, -1, 1)) && Grid.Solid[Grid.OffsetCell(cell, -1, 1)]) ? true : false;
				}, null, Grid.PosToCell(navigator.gameObject), 128, stop_at_solid: true, stop_at_liquid: true);
				base.sm.moveToWallTarget.Set(value, base.smi);
				GoTo(base.sm.move_notarget);
			}
			else
			{
				base.sm.breakable.Set(breakable, base.smi);
				GoTo(base.sm.move_target);
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, AggressiveChore>
	{
		public class BreakingWall : State
		{
			public State Pre;

			public State Loop;

			public State Pst;
		}

		public TargetParameter breaker;

		public TargetParameter breakable;

		public IntParameter moveToWallTarget;

		public int wallCellToBreak;

		public ApproachSubState<Breakable> move_target;

		public State move_notarget;

		public State findbreakable;

		public State noTarget;

		public State breaking;

		public BreakingWall breaking_wall;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = findbreakable;
			Target(breaker);
			root.ToggleAnims("anim_loco_destructive_kanim");
			noTarget.Enter(delegate(StatesInstance smi)
			{
				smi.StopSM("complete/no more food");
			});
			findbreakable.Enter("FindBreakable", delegate(StatesInstance smi)
			{
				smi.FindBreakable();
			});
			move_notarget.MoveTo((StatesInstance smi) => smi.sm.moveToWallTarget.Get(smi), breaking_wall, noTarget);
			move_target.InitializeStates(breaker, breakable, breaking, findbreakable).ToggleStatusItem(Db.Get().DuplicantStatusItems.LashingOut);
			breaking_wall.DefaultState(breaking_wall.Pre).Enter(delegate(StatesInstance smi)
			{
				int cell = Grid.PosToCell(smi.master.gameObject);
				if (Grid.Solid[Grid.OffsetCell(cell, 1, 0)])
				{
					smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_low_kanim"));
					int num = (wallCellToBreak = Grid.OffsetCell(cell, 1, 0));
				}
				else if (Grid.Solid[Grid.OffsetCell(cell, -1, 0)])
				{
					smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_low_kanim"));
					int num2 = (wallCellToBreak = Grid.OffsetCell(cell, -1, 0));
				}
				else if (Grid.Solid[Grid.OffsetCell(cell, 1, 1)])
				{
					smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_high_kanim"));
					int num3 = (wallCellToBreak = Grid.OffsetCell(cell, 1, 1));
				}
				else if (Grid.Solid[Grid.OffsetCell(cell, -1, 1)])
				{
					smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_high_kanim"));
					int num4 = (wallCellToBreak = Grid.OffsetCell(cell, -1, 1));
				}
				smi.master.GetComponent<Facing>().Face(Grid.CellToPos(wallCellToBreak));
			}).Exit(delegate(StatesInstance smi)
			{
				smi.sm.masterTarget.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_high_kanim"));
				smi.sm.masterTarget.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(Assets.GetAnim("anim_out_of_reach_destructive_low_kanim"));
			});
			breaking_wall.Pre.PlayAnim("working_pre").OnAnimQueueComplete(breaking_wall.Loop);
			breaking_wall.Loop.ScheduleGoTo(26f, breaking_wall.Pst).Update("PunchWallDamage", delegate(StatesInstance smi, float dt)
			{
				smi.master.PunchWallDamage(dt);
			}, UpdateRate.SIM_1000ms).Enter(delegate(StatesInstance smi)
			{
				smi.Play("working_loop", KAnim.PlayMode.Loop);
			})
				.Update(delegate(StatesInstance smi, float dt)
				{
					if (!Grid.Solid[smi.sm.wallCellToBreak])
					{
						smi.GoTo(breaking_wall.Pst);
					}
				});
			breaking_wall.Pst.QueueAnim("working_pst").OnAnimQueueComplete(noTarget);
			breaking.ToggleWork<Breakable>(breakable, null, null, null);
		}
	}

	public AggressiveChore(IStateMachineTarget target, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.StressActingOut, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, on_complete, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}

	public override void Cleanup()
	{
		base.Cleanup();
	}

	public void PunchWallDamage(float dt)
	{
		if (Grid.Solid[base.smi.sm.wallCellToBreak] && Grid.StrengthInfo[base.smi.sm.wallCellToBreak] < 100)
		{
			WorldDamage.Instance.ApplyDamage(base.smi.sm.wallCellToBreak, 0.06f * dt, base.smi.sm.wallCellToBreak, BUILDINGS.DAMAGESOURCES.MINION_DESTRUCTION, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.MINION_DESTRUCTION);
		}
	}
}
