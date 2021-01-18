using System;
using System.Collections.Generic;
using UnityEngine;

public class FleeChore : Chore<FleeChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, FleeChore, object>.GameInstance
	{
		public StatesInstance(FleeChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, FleeChore>
	{
		public TargetParameter fleeFromTarget;

		public TargetParameter fleeToTarget;

		public TargetParameter self;

		public State planFleeRoute;

		public ApproachSubState<IApproachable> flee;

		public State cower;

		public State end;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = planFleeRoute;
			root.ToggleStatusItem(Db.Get().DuplicantStatusItems.Fleeing, null);
			planFleeRoute.Enter(delegate(StatesInstance smi)
			{
				int num = -1;
				int num2 = Grid.PosToCell(fleeFromTarget.Get(smi));
				HashSet<int> hashSet = GameUtil.FloodCollectCells(Grid.PosToCell(smi.master.gameObject), smi.master.CanFleeTo);
				int num3 = -1;
				int num4 = -1;
				foreach (int item in hashSet)
				{
					if (smi.master.nav.CanReach(item))
					{
						int num5 = -1;
						num5 += Grid.GetCellDistance(item, num2);
						if (smi.master.isInFavoredDirection(item, num2))
						{
							num5 += 8;
						}
						if (num5 > num4)
						{
							num4 = num5;
							num3 = item;
						}
					}
				}
				num = num3;
				if (num != -1)
				{
					smi.sm.fleeToTarget.Set(smi.master.CreateLocator(Grid.CellToPos(num)), smi);
					smi.sm.fleeToTarget.Get(smi).name = "FleeLocator";
					if (num == num2)
					{
						smi.GoTo(cower);
					}
					else
					{
						smi.GoTo(flee);
					}
				}
				else
				{
					smi.GoTo(cower);
				}
			});
			flee.InitializeStates(self, fleeToTarget, cower, cower, null, NavigationTactics.ReduceTravelDistance).ToggleAnims("anim_loco_run_insane_kanim", 2f);
			cower.ToggleAnims("anim_cringe_kanim", 4f).PlayAnim("cringe_pre").QueueAnim("cringe_loop")
				.QueueAnim("cringe_pst")
				.OnAnimQueueComplete(end);
			end.Enter(delegate(StatesInstance smi)
			{
				smi.StopSM("stopped");
			});
		}
	}

	private Navigator nav;

	public FleeChore(IStateMachineTarget target, GameObject enemy)
		: base(Db.Get().ChoreTypes.Flee, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
		base.smi.sm.self.Set(gameObject, base.smi);
		nav = gameObject.GetComponent<Navigator>();
		base.smi.sm.fleeFromTarget.Set(enemy, base.smi);
	}

	private bool isInFavoredDirection(int cell, int fleeFromCell)
	{
		int num = ((Grid.CellToPos(fleeFromCell).x < gameObject.transform.GetPosition().x) ? 1 : 0);
		bool flag = ((Grid.CellToPos(fleeFromCell).x < Grid.CellToPos(cell).x) ? true : false);
		return num == (flag ? 1 : 0);
	}

	private bool CanFleeTo(int cell)
	{
		if (!nav.CanReach(cell) && !nav.CanReach(Grid.OffsetCell(cell, -1, -1)) && !nav.CanReach(Grid.OffsetCell(cell, 1, -1)) && !nav.CanReach(Grid.OffsetCell(cell, -1, 1)))
		{
			return nav.CanReach(Grid.OffsetCell(cell, 1, 1));
		}
		return true;
	}

	public GameObject CreateLocator(Vector3 pos)
	{
		return ChoreHelpers.CreateLocator("GoToLocator", pos);
	}

	protected override void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		if (base.smi.sm.fleeToTarget.Get(base.smi) != null)
		{
			ChoreHelpers.DestroyLocator(base.smi.sm.fleeToTarget.Get(base.smi));
		}
		base.OnStateMachineStop(reason, status);
	}
}
