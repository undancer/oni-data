using System;

public class RocketPassengerMonitor : GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public int lastWorldID;

		public Action<Chore> moduleDeployCompleteCallback;

		public int moduleDeployTaskTargetMoveCell;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool ShouldMoveThroughRocketDoor()
		{
			int num = base.sm.targetCell.Get(this);
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			if (Grid.WorldIdx[num] == this.GetMyWorldId())
			{
				base.sm.targetCell.Set(Grid.InvalidCell, this);
				return false;
			}
			return true;
		}

		public void SetMoveTarget(int cell)
		{
			if (Grid.WorldIdx[cell] != this.GetMyWorldId())
			{
				base.sm.targetCell.Set(cell, this);
			}
		}

		public void SetModuleDeployChore(int cell, Action<Chore> OnChoreCompleteCallback)
		{
			moduleDeployCompleteCallback = OnChoreCompleteCallback;
			moduleDeployTaskTargetMoveCell = cell;
			GoTo(base.sm.movingToModuleDeployPre);
			base.sm.targetCell.Set(cell, this);
		}

		public void CancelModuleDeployChore()
		{
			moduleDeployCompleteCallback = null;
			moduleDeployTaskTargetMoveCell = Grid.InvalidCell;
			base.sm.targetCell.Set(Grid.InvalidCell, base.smi);
		}

		public void ClearMoveTarget(int testCell)
		{
			int num = base.sm.targetCell.Get(this);
			if (Grid.IsValidCell(num) && Grid.WorldIdx[num] == Grid.WorldIdx[testCell])
			{
				base.sm.targetCell.Set(Grid.InvalidCell, this);
				if (IsInsideState(base.sm.moving))
				{
					GoTo(base.sm.satisfied);
				}
			}
		}
	}

	public IntParameter targetCell = new IntParameter(Grid.InvalidCell);

	public State satisfied;

	public State moving;

	public State movingToModuleDeployPre;

	public State movingToModuleDeploy;

	public State moduleDeploy;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = SerializeType.ParamsOnly;
		satisfied.ParamTransition(targetCell, moving, (Instance smi, int p) => p != Grid.InvalidCell);
		moving.ParamTransition(targetCell, satisfied, (Instance smi, int p) => p == Grid.InvalidCell).ToggleChore((Instance smi) => CreateChore(smi), satisfied).Exit(delegate(Instance smi)
		{
			targetCell.Set(Grid.InvalidCell, smi);
		});
		movingToModuleDeployPre.Enter(delegate(Instance smi)
		{
			targetCell.Set(smi.moduleDeployTaskTargetMoveCell, smi);
			smi.GoTo(movingToModuleDeploy);
		});
		movingToModuleDeploy.ParamTransition(targetCell, satisfied, (Instance smi, int p) => p == Grid.InvalidCell).ToggleChore((Instance smi) => CreateChore(smi), moduleDeploy);
		moduleDeploy.Enter(delegate(Instance smi)
		{
			smi.moduleDeployCompleteCallback(null);
			targetCell.Set(Grid.InvalidCell, smi);
			smi.moduleDeployCompleteCallback = null;
			smi.GoTo(smi.sm.satisfied);
		});
	}

	public Chore CreateChore(Instance smi)
	{
		MoveChore moveChore = new MoveChore(smi.master, Db.Get().ChoreTypes.RocketEnterExit, (MoveChore.StatesInstance mover_smi) => targetCell.Get(smi));
		moveChore.AddPrecondition(ChorePreconditions.instance.CanMoveToCell, targetCell.Get(smi));
		return moveChore;
	}
}
