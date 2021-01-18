using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MechanicalSurfboard : StateMachineComponent<MechanicalSurfboard.StatesInstance>, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, MechanicalSurfboard>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State working;

			public State post;
		}

		private State inoperational;

		private State operational;

		private ReadyStates ready;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = inoperational;
			inoperational.PlayAnim("off").TagTransition(GameTags.Operational, operational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.MissingRequirements);
			operational.PlayAnim("off").TagTransition(GameTags.Operational, inoperational, on_remove: true).EventTransition(GameHashes.OnStorageChange, ready, IsReady)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.GettingReady);
			ready.TagTransition(GameTags.Operational, inoperational, on_remove: true).DefaultState(ready.idle).ToggleChore(CreateChore, operational)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working);
			ready.idle.PlayAnim("on", KAnim.PlayMode.Loop).WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<MechanicalSurfboardWorkable>(), ready.working).EventTransition(GameHashes.OnStorageChange, operational, GameStateMachine<States, StatesInstance, MechanicalSurfboard, object>.Not(IsReady));
			ready.working.PlayAnim("working_pre").QueueAnim("working_loop", loop: true).WorkableStopTransition((StatesInstance smi) => smi.master.GetComponent<MechanicalSurfboardWorkable>(), ready.post);
			ready.post.PlayAnim("working_pst").OnAnimQueueComplete(ready);
		}

		private Chore CreateChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<MechanicalSurfboardWorkable>();
			WorkChore<MechanicalSurfboardWorkable> workChore = new WorkChore<MechanicalSurfboardWorkable>(Db.Get().ChoreTypes.Relax, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, Db.Get().ScheduleBlockTypes.Recreation, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
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
			if (primaryElement.Mass < smi.master.minOperationalWaterKG)
			{
				return false;
			}
			return true;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, MechanicalSurfboard, object>.GameInstance
	{
		public StatesInstance(MechanicalSurfboard smi)
			: base(smi)
		{
		}
	}

	public string specificEffect;

	public string trackingEffect;

	public float waterSpillRateKG;

	public float minOperationalWaterKG;

	public string[] interactAnims = new string[3]
	{
		"anim_interacts_mechanical_surfboard_kanim",
		"anim_interacts_mechanical_surfboard2_kanim",
		"anim_interacts_mechanical_surfboard3_kanim"
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(SimHashes.Water);
		list.Add(new Descriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION));
		Effect.AddModifierDescriptions(base.gameObject, list, specificEffect, increase_indent: true);
		list.Add(new Descriptor(BUILDINGS.PREFABS.MECHANICALSURFBOARD.WATER_REQUIREMENT.Replace("{element}", element.name).Replace("{amount}", GameUtil.GetFormattedMass(minOperationalWaterKG)), BUILDINGS.PREFABS.MECHANICALSURFBOARD.WATER_REQUIREMENT_TOOLTIP.Replace("{element}", element.name).Replace("{amount}", GameUtil.GetFormattedMass(minOperationalWaterKG)), Descriptor.DescriptorType.Requirement));
		list.Add(new Descriptor(BUILDINGS.PREFABS.MECHANICALSURFBOARD.LEAK_REQUIREMENT.Replace("{amount}", GameUtil.GetFormattedMass(waterSpillRateKG, GameUtil.TimeSlice.PerSecond)), BUILDINGS.PREFABS.MECHANICALSURFBOARD.LEAK_REQUIREMENT_TOOLTIP.Replace("{amount}", GameUtil.GetFormattedMass(waterSpillRateKG, GameUtil.TimeSlice.PerSecond)), Descriptor.DescriptorType.Requirement).IncreaseIndent());
		return list;
	}
}
