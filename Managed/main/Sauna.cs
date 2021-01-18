using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Sauna : StateMachineComponent<Sauna.StatesInstance>, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, Sauna>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State working;
		}

		private State inoperational;

		private State operational;

		private ReadyStates ready;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = inoperational;
			inoperational.PlayAnim("off").TagTransition(GameTags.Operational, operational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.MissingRequirements);
			operational.TagTransition(GameTags.Operational, inoperational, on_remove: true).ToggleMainStatusItem(Db.Get().BuildingStatusItems.GettingReady).EventTransition(GameHashes.OnStorageChange, ready, IsReady);
			ready.TagTransition(GameTags.Operational, inoperational, on_remove: true).DefaultState(ready.idle).ToggleChore(CreateChore, inoperational)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working);
			ready.idle.WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<SaunaWorkable>(), ready.working).EventTransition(GameHashes.OnStorageChange, operational, GameStateMachine<States, StatesInstance, Sauna, object>.Not(IsReady));
			ready.working.WorkableCompleteTransition((StatesInstance smi) => smi.master.GetComponent<SaunaWorkable>(), ready.idle).WorkableStopTransition((StatesInstance smi) => smi.master.GetComponent<SaunaWorkable>(), ready.idle);
		}

		private Chore CreateChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<SaunaWorkable>();
			Chore chore = new WorkChore<SaunaWorkable>(Db.Get().ChoreTypes.Relax, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, Db.Get().ScheduleBlockTypes.Recreation, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
			chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
			return chore;
		}

		private bool IsReady(StatesInstance smi)
		{
			PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Steam);
			return primaryElement != null && primaryElement.Mass >= smi.master.steamPerUseKG;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Sauna, object>.GameInstance
	{
		public StatesInstance(Sauna smi)
			: base(smi)
		{
		}
	}

	public string specificEffect;

	public string trackingEffect;

	public float steamPerUseKG;

	public float waterOutputTemp;

	public static readonly Operational.Flag sufficientSteam = new Operational.Flag("sufficientSteam", Operational.Flag.Type.Requirement);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
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
		list.Add(new Descriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION));
		Effect.AddModifierDescriptions(base.gameObject, list, specificEffect, increase_indent: true);
		Element element = ElementLoader.FindElementByHash(SimHashes.Steam);
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, element.name, GameUtil.GetFormattedMass(steamPerUseKG, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, element.name, GameUtil.GetFormattedMass(steamPerUseKG, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), Descriptor.DescriptorType.Requirement));
		Element element2 = ElementLoader.FindElementByHash(SimHashes.Water);
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, element2.name, GameUtil.GetFormattedMass(steamPerUseKG, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, element2.name, GameUtil.GetFormattedMass(steamPerUseKG, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"))));
		return list;
	}
}
