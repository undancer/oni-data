using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MournChore : Chore<MournChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, MournChore, object>.GameInstance
	{
		private int locatorCell = -1;

		public StatesInstance(MournChore master)
			: base(master)
		{
		}

		public void CreateLocator()
		{
			Grave grave = FindGraveToMournAt();
			int cell = Grid.PosToCell(grave.transform.GetPosition());
			Navigator component = base.master.GetComponent<Navigator>();
			int standableCell = GetStandableCell(cell, component);
			if (standableCell < 0)
			{
				base.smi.GoTo((StateMachine.BaseState)null);
				return;
			}
			Grid.Reserved[standableCell] = true;
			Vector3 pos = Grid.CellToPosCBC(standableCell, Grid.SceneLayer.Move);
			GameObject value = ChoreHelpers.CreateLocator("MournLocator", pos);
			base.smi.sm.locator.Set(value, base.smi);
			locatorCell = standableCell;
			base.smi.GoTo(base.sm.moveto);
		}

		public void DestroyLocator()
		{
			if (locatorCell >= 0)
			{
				Grid.Reserved[locatorCell] = false;
				ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
				base.sm.locator.Set(null, this);
				locatorCell = -1;
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, MournChore>
	{
		public TargetParameter mourner;

		public TargetParameter locator;

		public State findOffset;

		public ApproachSubState<IApproachable> moveto;

		public State mourn;

		public State completed;

		private static readonly HashedString[] WORK_ANIMS = new HashedString[2]
		{
			"working_pre",
			"working_loop"
		};

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = findOffset;
			Target(mourner);
			root.ToggleAnims("anim_react_mourning_kanim").Exit("DestroyLocator", delegate(StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			findOffset.Enter("CreateLocator", delegate(StatesInstance smi)
			{
				smi.CreateLocator();
			});
			moveto.InitializeStates(mourner, locator, mourn);
			mourn.PlayAnims((StatesInstance smi) => WORK_ANIMS, KAnim.PlayMode.Loop).ScheduleGoTo(10f, completed);
			completed.PlayAnim("working_pst").OnAnimQueueComplete(null).Exit(delegate(StatesInstance smi)
			{
				mourner.Get<Effects>(smi).Remove(Db.Get().effects.Get("Mourning"));
			});
		}
	}

	private static readonly CellOffset[] ValidStandingOffsets = new CellOffset[3]
	{
		new CellOffset(0, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 0)
	};

	private static readonly Precondition HasValidMournLocation = new Precondition
	{
		id = "HasPlaceToStand",
		description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_PLACE_TO_STAND,
		fn = delegate(ref Precondition.Context context, object data)
		{
			IStateMachineTarget stateMachineTarget = (IStateMachineTarget)data;
			Navigator component = stateMachineTarget.GetComponent<Navigator>();
			bool result = false;
			Grave grave = FindGraveToMournAt();
			if (grave != null)
			{
				int cell = Grid.PosToCell(grave);
				int standableCell = GetStandableCell(cell, component);
				if (Grid.IsValidCell(standableCell))
				{
					result = true;
				}
			}
			return result;
		}
	};

	private static int GetStandableCell(int cell, Navigator navigator)
	{
		CellOffset[] validStandingOffsets = ValidStandingOffsets;
		foreach (CellOffset offset in validStandingOffsets)
		{
			if (Grid.IsCellOffsetValid(cell, offset))
			{
				int num = Grid.OffsetCell(cell, offset);
				if (!Grid.Reserved[num] && navigator.NavGrid.NavTable.IsValid(num) && navigator.GetNavigationCost(num) != -1)
				{
					return num;
				}
			}
		}
		return -1;
	}

	public MournChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.Mourn, master, master.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.high, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
		AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
		AddPrecondition(ChorePreconditions.instance.NoDeadBodies);
		AddPrecondition(HasValidMournLocation, master);
	}

	public static Grave FindGraveToMournAt()
	{
		Grave result = null;
		float num = -1f;
		foreach (Grave grafe in Components.Graves)
		{
			if (grafe.burialTime > num)
			{
				num = grafe.burialTime;
				result = grafe;
			}
		}
		return result;
	}

	public override void Begin(Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			Debug.LogError("MournChore null context.consumer");
			return;
		}
		if (base.smi == null)
		{
			Debug.LogError("MournChore null smi");
			return;
		}
		if (base.smi.sm == null)
		{
			Debug.LogError("MournChore null smi.sm");
			return;
		}
		Grave x = FindGraveToMournAt();
		if (x == null)
		{
			Debug.LogError("MournChore no grave");
			return;
		}
		base.smi.sm.mourner.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
	}
}
