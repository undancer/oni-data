using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class RecoverBreathChore : Chore<RecoverBreathChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RecoverBreathChore, object>.GameInstance
	{
		public AttributeModifier recoveringbreath;

		public StatesInstance(RecoverBreathChore master, GameObject recoverer)
			: base(master)
		{
			base.sm.recoverer.Set(recoverer, base.smi);
			Klei.AI.Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
			float value = 3f;
			recoveringbreath = new AttributeModifier(deltaAttribute.Id, value, DUPLICANTS.MODIFIERS.RECOVERINGBREATH.NAME);
		}

		public void CreateLocator()
		{
			GameObject value = ChoreHelpers.CreateLocator("RecoverBreathLocator", Vector3.zero);
			base.sm.locator.Set(value, this);
			UpdateLocator();
		}

		public void UpdateLocator()
		{
			int num = base.sm.recoverer.GetSMI<BreathMonitor.Instance>(base.smi).GetRecoverCell();
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(base.sm.recoverer.Get<Transform>(base.smi).GetPosition());
			}
			Vector3 position = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			base.sm.locator.Get<Transform>(base.smi).SetPosition(position);
		}

		public void DestroyLocator()
		{
			ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
			base.sm.locator.Set(null, this);
		}

		public void RemoveSuitIfNecessary()
		{
			Equipment equipment = base.sm.recoverer.Get<Equipment>(base.smi);
			if (!(equipment == null))
			{
				Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
				if (!(assignable == null))
				{
					assignable.Unassign();
				}
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RecoverBreathChore>
	{
		public ApproachSubState<IApproachable> approach;

		public PreLoopPostState recover;

		public State remove_suit;

		public TargetParameter recoverer;

		public TargetParameter locator;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approach;
			Target(recoverer);
			root.Enter("CreateLocator", delegate(StatesInstance smi)
			{
				smi.CreateLocator();
			}).Exit("DestroyLocator", delegate(StatesInstance smi)
			{
				smi.DestroyLocator();
			}).Update("UpdateLocator", delegate(StatesInstance smi, float dt)
			{
				smi.UpdateLocator();
			}, UpdateRate.SIM_200ms, load_balance: true);
			approach.InitializeStates(recoverer, locator, remove_suit);
			remove_suit.GoTo(recover);
			recover.ToggleAnims("anim_emotes_default_kanim").DefaultState(recover.pre).ToggleAttributeModifier("Recovering Breath", (StatesInstance smi) => smi.recoveringbreath)
				.ToggleTag(GameTags.RecoveringBreath)
				.TriggerOnEnter(GameHashes.BeginBreathRecovery)
				.TriggerOnExit(GameHashes.EndBreathRecovery);
			recover.pre.PlayAnim("breathe_pre").OnAnimQueueComplete(recover.loop);
			recover.loop.PlayAnim("breathe_loop", KAnim.PlayMode.Loop);
			recover.pst.QueueAnim("breathe_pst").OnAnimQueueComplete(null);
		}
	}

	public RecoverBreathChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.RecoverBreath, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
