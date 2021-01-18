using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Juicer : StateMachineComponent<Juicer.StatesInstance>, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, Juicer>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State working;

			public State post;
		}

		private State unoperational;

		private State operational;

		private ReadyStates ready;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			unoperational.PlayAnim("off").TagTransition(GameTags.Operational, operational);
			operational.PlayAnim("off").TagTransition(GameTags.Operational, unoperational, on_remove: true).Transition(ready, IsReady)
				.EventTransition(GameHashes.OnStorageChange, ready, IsReady);
			ready.TagTransition(GameTags.Operational, unoperational, on_remove: true).DefaultState(ready.idle).ToggleChore(CreateChore, operational);
			ready.idle.Transition(operational, GameStateMachine<States, StatesInstance, Juicer, object>.Not(IsReady)).EventTransition(GameHashes.OnStorageChange, operational, GameStateMachine<States, StatesInstance, Juicer, object>.Not(IsReady)).PlayAnim("on")
				.WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<JuicerWorkable>(), ready.working);
			ready.working.PlayAnim("working_pre").QueueAnim("working_loop", loop: true).WorkableStopTransition((StatesInstance smi) => smi.master.GetComponent<JuicerWorkable>(), ready.post);
			ready.post.PlayAnim("working_pst").OnAnimQueueComplete(ready);
		}

		private Chore CreateChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<JuicerWorkable>();
			Chore chore = new WorkChore<JuicerWorkable>(Db.Get().ChoreTypes.Relax, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, Db.Get().ScheduleBlockTypes.Recreation, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
			chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
			return chore;
		}

		private bool IsReady(StatesInstance smi)
		{
			PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Water);
			if (primaryElement == null)
			{
				return false;
			}
			if (primaryElement.Mass < smi.master.waterMassPerUse)
			{
				return false;
			}
			for (int i = 0; i < smi.master.ingredientTags.Length; i++)
			{
				float amountAvailable = smi.GetComponent<Storage>().GetAmountAvailable(smi.master.ingredientTags[i]);
				if (amountAvailable < smi.master.ingredientMassesPerUse[i])
				{
					return false;
				}
			}
			return true;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Juicer, object>.GameInstance
	{
		public StatesInstance(Juicer smi)
			: base(smi)
		{
		}
	}

	public string specificEffect;

	public string trackingEffect;

	public Tag[] ingredientTags;

	public float[] ingredientMassesPerUse;

	public float waterMassPerUse;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		});
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void AddRequirementDesc(List<Descriptor> descs, Tag tag, float mass)
	{
		string arg = tag.ProperName();
		Descriptor item = default(Descriptor);
		EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(tag.Name);
		string arg2 = ((foodInfo != null) ? GameUtil.GetFormattedCaloriesForItem(tag, mass) : GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram));
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, arg, arg2), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, arg, arg2), Descriptor.DescriptorType.Requirement);
		descs.Add(item);
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION);
		list.Add(item);
		Effect.AddModifierDescriptions(base.gameObject, list, specificEffect, increase_indent: true);
		for (int i = 0; i < ingredientTags.Length; i++)
		{
			AddRequirementDesc(list, ingredientTags[i], ingredientMassesPerUse[i]);
		}
		AddRequirementDesc(list, GameTags.Water, waterMassPerUse);
		return list;
	}
}
