using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class UglyCryChore : Chore<UglyCryChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, UglyCryChore, object>.GameInstance
	{
		private AmountInstance bodyTemperature;

		public StatesInstance(UglyCryChore master, GameObject crier)
			: base(master)
		{
			base.sm.crier.Set(crier, base.smi);
			bodyTemperature = Db.Get().Amounts.Temperature.Lookup(crier);
		}

		public void ProduceTears(float dt)
		{
			if (!(dt <= 0f))
			{
				int gameCell = Grid.PosToCell(base.smi.master.gameObject);
				Equippable equippable = GetComponent<SuitEquipper>().IsWearingAirtightSuit();
				if (equippable != null)
				{
					equippable.GetComponent<Storage>().AddLiquid(SimHashes.Water, 1f * STRESS.TEARS_RATE * dt, bodyTemperature.value, byte.MaxValue, 0);
				}
				else
				{
					SimMessages.AddRemoveSubstance(gameCell, SimHashes.Water, CellEventLogger.Instance.Tears, 1f * STRESS.TEARS_RATE * dt, bodyTemperature.value, byte.MaxValue, 0);
				}
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, UglyCryChore>
	{
		public class Cry : State
		{
			public State cry_pre;

			public State cry_loop;

			public State cry_pst;
		}

		public TargetParameter crier;

		public Cry cry;

		public State complete;

		private Effect uglyCryingEffect;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = cry;
			Target(crier);
			uglyCryingEffect = new Effect("UglyCrying", DUPLICANTS.MODIFIERS.UGLY_CRYING.NAME, DUPLICANTS.MODIFIERS.UGLY_CRYING.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
			uglyCryingEffect.Add(new AttributeModifier(Db.Get().Attributes.Decor.Id, -30f, DUPLICANTS.MODIFIERS.UGLY_CRYING.NAME));
			Db.Get().effects.Add(uglyCryingEffect);
			cry.defaultState = cry.cry_pre.RemoveEffect("CryFace").ToggleAnims("anim_cry_kanim");
			cry.cry_pre.PlayAnim("working_pre").ScheduleGoTo(2f, cry.cry_loop);
			cry.cry_loop.ToggleAnims("anim_cry_kanim").Enter(delegate(StatesInstance smi)
			{
				smi.Play("working_loop", KAnim.PlayMode.Loop);
			}).ScheduleGoTo(18f, cry.cry_pst)
				.ToggleEffect((StatesInstance smi) => uglyCryingEffect)
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.ProduceTears(dt);
				});
			cry.cry_pst.QueueAnim("working_pst").OnAnimQueueComplete(complete);
			complete.AddEffect("CryFace").Enter(delegate(StatesInstance smi)
			{
				smi.StopSM("complete");
			});
		}
	}

	public UglyCryChore(ChoreType chore_type, IStateMachineTarget target, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.UglyCry, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, on_complete, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
