using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class SodaFountain : StateMachineComponent<SodaFountain.StatesInstance>, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, SodaFountain>
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
			ready.idle.Transition(operational, GameStateMachine<States, StatesInstance, SodaFountain, object>.Not(IsReady)).EventTransition(GameHashes.OnStorageChange, operational, GameStateMachine<States, StatesInstance, SodaFountain, object>.Not(IsReady)).WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<SodaFountainWorkable>(), ready.working);
			ready.working.PlayAnim("working_pre").WorkableStopTransition((StatesInstance smi) => smi.master.GetComponent<SodaFountainWorkable>(), ready.post);
			ready.post.PlayAnim("working_pst").OnAnimQueueComplete(ready);
		}

		private Chore CreateChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<SodaFountainWorkable>();
			WorkChore<SodaFountainWorkable> workChore = new WorkChore<SodaFountainWorkable>(Db.Get().ChoreTypes.Relax, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, Db.Get().ScheduleBlockTypes.Recreation, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
			workChore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
			return workChore;
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
			if (smi.GetComponent<Storage>().GetAmountAvailable(smi.master.ingredientTag) < smi.master.ingredientMassPerUse)
			{
				return false;
			}
			return true;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, SodaFountain, object>.GameInstance
	{
		public StatesInstance(SodaFountain smi)
			: base(smi)
		{
		}
	}

	public string specificEffect;

	public string trackingEffect;

	public Tag ingredientTag;

	public float ingredientMassPerUse;

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
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
		descs.Add(item);
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION);
		list.Add(item);
		Effect.AddModifierDescriptions(base.gameObject, list, specificEffect, increase_indent: true);
		AddRequirementDesc(list, ingredientTag, ingredientMassPerUse);
		AddRequirementDesc(list, GameTags.Water, waterMassPerUse);
		return list;
	}
}
