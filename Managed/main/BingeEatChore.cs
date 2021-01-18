using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BingeEatChore : Chore<BingeEatChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, BingeEatChore, object>.GameInstance
	{
		public StatesInstance(BingeEatChore master, GameObject eater)
			: base(master)
		{
			base.sm.eater.Set(eater, base.smi);
			base.sm.bingeremaining.Set(2f, base.smi);
		}

		public void FindFood()
		{
			Navigator component = GetComponent<Navigator>();
			int num = int.MaxValue;
			Edible edible = null;
			if (base.sm.bingeremaining.Get(base.smi) <= PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
			{
				GoTo(base.sm.eat_pst);
				return;
			}
			foreach (Edible item in Components.Edibles.Items)
			{
				if (!(item == null) && !(item == base.sm.ediblesource.Get<Edible>(base.smi)) && !item.isBeingConsumed && !(item.GetComponent<Pickupable>().UnreservedAmount <= 0f) && item.GetComponent<Pickupable>().CouldBePickedUpByMinion(base.gameObject))
				{
					int navigationCost = component.GetNavigationCost(item);
					if (navigationCost != -1 && navigationCost < num)
					{
						num = navigationCost;
						edible = item;
					}
				}
			}
			base.sm.ediblesource.Set(edible, base.smi);
			base.sm.requestedfoodunits.Set(base.sm.bingeremaining.Get(base.smi), base.smi);
			if (edible == null)
			{
				GoTo(base.sm.cantFindFood);
			}
			else
			{
				GoTo(base.sm.fetch);
			}
		}

		public bool IsBingeEating()
		{
			return base.sm.isBingeEating.Get(base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, BingeEatChore>
	{
		public TargetParameter eater;

		public TargetParameter ediblesource;

		public TargetParameter ediblechunk;

		public BoolParameter isBingeEating;

		public FloatParameter requestedfoodunits;

		public FloatParameter actualfoodunits;

		public FloatParameter bingeremaining;

		public State noTarget;

		public State findfood;

		public State eat;

		public State eat_pst;

		public State cantFindFood;

		public State finish;

		public FetchSubState fetch;

		private Effect bingeEatingEffect;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = findfood;
			Target(eater);
			bingeEatingEffect = new Effect("Binge_Eating", DUPLICANTS.MODIFIERS.BINGE_EATING.NAME, DUPLICANTS.MODIFIERS.BINGE_EATING.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
			bingeEatingEffect.Add(new AttributeModifier(Db.Get().Attributes.Decor.Id, -30f, DUPLICANTS.MODIFIERS.BINGE_EATING.NAME));
			bingeEatingEffect.Add(new AttributeModifier("CaloriesDelta", -6666.6665f, DUPLICANTS.MODIFIERS.BINGE_EATING.NAME));
			Db.Get().effects.Add(bingeEatingEffect);
			root.ToggleEffect((StatesInstance smi) => bingeEatingEffect);
			noTarget.GoTo(finish);
			eat_pst.ToggleAnims("anim_eat_overeat_kanim").PlayAnim("working_pst").OnAnimQueueComplete(finish);
			finish.Enter(delegate(StatesInstance smi)
			{
				smi.StopSM("complete/no more food");
			});
			findfood.Enter("FindFood", delegate(StatesInstance smi)
			{
				smi.FindFood();
			});
			fetch.InitializeStates(eater, ediblesource, ediblechunk, requestedfoodunits, actualfoodunits, eat, cantFindFood);
			eat.ToggleAnims("anim_eat_overeat_kanim").QueueAnim("working_loop", loop: true).Enter(delegate(StatesInstance smi)
			{
				isBingeEating.Set(value: true, smi);
			})
				.DoEat(ediblechunk, actualfoodunits, findfood, findfood)
				.Exit("ClearIsBingeEating", delegate(StatesInstance smi)
				{
					isBingeEating.Set(value: false, smi);
				});
			cantFindFood.ToggleAnims("anim_interrupt_binge_eat_kanim").PlayAnim("interrupt_binge_eat").OnAnimQueueComplete(noTarget);
		}
	}

	public BingeEatChore(IStateMachineTarget target, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.BingeEat, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, on_complete, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
		Subscribe(1121894420, OnEat);
	}

	private void OnEat(object data)
	{
		Edible edible = (Edible)data;
		if (edible != null)
		{
			base.smi.sm.bingeremaining.Set(Mathf.Max(0f, base.smi.sm.bingeremaining.Get(base.smi) - edible.unitsConsumed), base.smi);
		}
	}

	public override void Cleanup()
	{
		base.Cleanup();
		Unsubscribe(1121894420, OnEat);
	}
}
