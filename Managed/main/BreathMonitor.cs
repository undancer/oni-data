using Klei.AI;

public class BreathMonitor : GameStateMachine<BreathMonitor, BreathMonitor.Instance>
{
	public class LowBreathState : State
	{
		public State nowheretorecover;

		public State recoveryavailable;
	}

	public class SatisfiedState : State
	{
		public State full;

		public State notfull;
	}

	public new class Instance : GameInstance
	{
		public AmountInstance breath;

		public SafetyQuery query;

		public Navigator navigator;

		public OxygenBreather breather;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			breath = Db.Get().Amounts.Breath.Lookup(master.gameObject);
			query = new SafetyQuery(Game.Instance.safetyConditions.RecoverBreathChecker, GetComponent<KMonoBehaviour>(), int.MaxValue);
			navigator = GetComponent<Navigator>();
			breather = GetComponent<OxygenBreather>();
		}

		public int GetRecoverCell()
		{
			return base.sm.recoverBreathCell.Get(base.smi);
		}

		public float GetBreath()
		{
			return breath.value / breath.GetMax();
		}
	}

	public SatisfiedState satisfied;

	public LowBreathState lowbreath;

	public IntParameter recoverBreathCell;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.DefaultState(satisfied.full).Transition(lowbreath, IsLowBreath);
		satisfied.full.Transition(satisfied.notfull, IsNotFullBreath).Enter(HideBreathBar);
		satisfied.notfull.Transition(satisfied.full, IsFullBreath).Enter(ShowBreathBar);
		lowbreath.DefaultState(lowbreath.nowheretorecover).Transition(satisfied, IsFullBreath).ToggleExpression(Db.Get().Expressions.RecoverBreath, IsNotInBreathableArea)
			.ToggleUrge(Db.Get().Urges.RecoverBreath)
			.ToggleThought(Db.Get().Thoughts.Suffocating)
			.ToggleTag(GameTags.HoldingBreath)
			.Enter(ShowBreathBar)
			.Enter(UpdateRecoverBreathCell)
			.Update(UpdateRecoverBreathCell, UpdateRate.RENDER_1000ms, load_balance: true);
		lowbreath.nowheretorecover.ParamTransition(recoverBreathCell, lowbreath.recoveryavailable, IsValidRecoverCell);
		lowbreath.recoveryavailable.ParamTransition(recoverBreathCell, lowbreath.nowheretorecover, IsNotValidRecoverCell).Enter(UpdateRecoverBreathCell).ToggleChore(CreateRecoverBreathChore, lowbreath.nowheretorecover);
	}

	private static bool IsLowBreath(Instance smi)
	{
		if (smi.master.gameObject.GetMyWorld().AlertManager.IsRedAlert())
		{
			return smi.breath.value < 45.454548f;
		}
		return smi.breath.value < 72.72727f;
	}

	private static Chore CreateRecoverBreathChore(Instance smi)
	{
		return new RecoverBreathChore(smi.master);
	}

	private static bool IsNotFullBreath(Instance smi)
	{
		return !IsFullBreath(smi);
	}

	private static bool IsFullBreath(Instance smi)
	{
		return smi.breath.value >= smi.breath.GetMax();
	}

	private static bool IsNotInBreathableArea(Instance smi)
	{
		return !smi.breather.IsBreathableElementAtCell(Grid.PosToCell(smi));
	}

	private static void ShowBreathBar(Instance smi)
	{
		if (NameDisplayScreen.Instance != null)
		{
			NameDisplayScreen.Instance.SetBreathDisplay(smi.gameObject, smi.GetBreath, bVisible: true);
		}
	}

	private static void HideBreathBar(Instance smi)
	{
		if (NameDisplayScreen.Instance != null)
		{
			NameDisplayScreen.Instance.SetBreathDisplay(smi.gameObject, null, bVisible: false);
		}
	}

	private static bool IsValidRecoverCell(Instance smi, int cell)
	{
		return cell != Grid.InvalidCell;
	}

	private static bool IsNotValidRecoverCell(Instance smi, int cell)
	{
		return !IsValidRecoverCell(smi, cell);
	}

	private static void UpdateRecoverBreathCell(Instance smi, float dt)
	{
		UpdateRecoverBreathCell(smi);
	}

	private static void UpdateRecoverBreathCell(Instance smi)
	{
		smi.query.Reset();
		smi.navigator.RunQuery(smi.query);
		int num = smi.query.GetResultCell();
		if (!smi.breather.IsBreathableElementAtCell(num))
		{
			num = PathFinder.InvalidCell;
		}
		smi.sm.recoverBreathCell.Set(num, smi);
	}
}
