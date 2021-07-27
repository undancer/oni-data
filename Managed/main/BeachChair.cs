using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BeachChair : StateMachineComponent<BeachChair.StatesInstance>, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, BeachChair>
	{
		public class LitWorkingStates : State
		{
			public State working;

			public State silly;

			public State post;
		}

		public class WorkingStates : State
		{
			public State working;

			public State post;
		}

		public class ReadyStates : State
		{
			public State idle;

			public State working_pre;

			public WorkingStates working_unlit;

			public LitWorkingStates working_lit;

			public State post;
		}

		public BoolParameter lit;

		public TargetParameter worker;

		private State inoperational;

		private ReadyStates ready;

		private HashedString[] UNLIT_PST_ANIMS = new HashedString[2] { "working_unlit_pst", "working_pst" };

		private HashedString[] LIT_PST_ANIMS = new HashedString[2] { "working_lit_pst", "working_pst" };

		private string[] SILLY_ANIMS = new string[3] { "working_lit_loop1", "working_lit_loop2", "working_lit_loop3" };

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = inoperational;
			inoperational.PlayAnim("off").TagTransition(GameTags.Operational, ready).ToggleMainStatusItem(Db.Get().BuildingStatusItems.MissingRequirements);
			ready.TagTransition(GameTags.Operational, inoperational, on_remove: true).DefaultState(ready.idle).ToggleChore(CreateChore, inoperational)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working);
			ready.idle.PlayAnim("on", KAnim.PlayMode.Loop).WorkableStartTransition((StatesInstance smi) => smi.master.GetComponent<BeachChairWorkable>(), ready.working_pre);
			ready.working_pre.PlayAnim("working_pre").QueueAnim("working_loop", loop: true).Target(worker)
				.PlayAnim("working_pre")
				.EventHandler(GameHashes.AnimQueueComplete, delegate(StatesInstance smi)
				{
					if (lit.Get(smi))
					{
						smi.GoTo(ready.working_lit);
					}
					else
					{
						smi.GoTo(ready.working_unlit);
					}
				});
			ready.working_unlit.DefaultState(ready.working_unlit.working).Enter(delegate(StatesInstance smi)
			{
				BeachChairWorkable component2 = smi.master.GetComponent<BeachChairWorkable>();
				component2.workingPstComplete = (component2.workingPstFailed = UNLIT_PST_ANIMS);
			}).ToggleStatusItem(Db.Get().BuildingStatusItems.TanningLightInsufficient)
				.WorkableStopTransition((StatesInstance smi) => smi.master.GetComponent<BeachChairWorkable>(), ready.post)
				.Target(worker)
				.PlayAnim("working_unlit_pre");
			ready.working_unlit.working.ParamTransition(lit, ready.working_unlit.post, GameStateMachine<States, StatesInstance, BeachChair, object>.IsTrue).Target(worker).QueueAnim("working_unlit_loop", loop: true);
			ready.working_unlit.post.Target(worker).PlayAnim("working_unlit_pst").EventHandler(GameHashes.AnimQueueComplete, delegate(StatesInstance smi)
			{
				if (lit.Get(smi))
				{
					smi.GoTo(ready.working_lit);
				}
				else
				{
					smi.GoTo(ready.working_unlit.working);
				}
			});
			ready.working_lit.DefaultState(ready.working_lit.working).Enter(delegate(StatesInstance smi)
			{
				BeachChairWorkable component = smi.master.GetComponent<BeachChairWorkable>();
				component.workingPstComplete = (component.workingPstFailed = LIT_PST_ANIMS);
			}).ToggleStatusItem(Db.Get().BuildingStatusItems.TanningLightSufficient)
				.WorkableStopTransition((StatesInstance smi) => smi.master.GetComponent<BeachChairWorkable>(), ready.post)
				.Target(worker)
				.PlayAnim("working_lit_pre");
			ready.working_lit.working.ParamTransition(lit, ready.working_lit.post, GameStateMachine<States, StatesInstance, BeachChair, object>.IsFalse).Target(worker).QueueAnim("working_lit_loop", loop: true)
				.ScheduleGoTo((StatesInstance smi) => Random.Range(5f, 15f), ready.working_lit.silly);
			ready.working_lit.silly.ParamTransition(lit, ready.working_lit.post, GameStateMachine<States, StatesInstance, BeachChair, object>.IsFalse).Target(worker).PlayAnim((StatesInstance smi) => SILLY_ANIMS[Random.Range(0, SILLY_ANIMS.Length)])
				.OnAnimQueueComplete(ready.working_lit.working);
			ready.working_lit.post.Target(worker).PlayAnim("working_lit_pst").EventHandler(GameHashes.AnimQueueComplete, delegate(StatesInstance smi)
			{
				if (!lit.Get(smi))
				{
					smi.GoTo(ready.working_unlit);
				}
				else
				{
					smi.GoTo(ready.working_lit.working);
				}
			});
			ready.post.PlayAnim("working_pst").Exit(delegate(StatesInstance smi)
			{
				worker.Set(null, smi);
			}).OnAnimQueueComplete(ready);
		}

		private Chore CreateChore(StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<BeachChairWorkable>();
			WorkChore<BeachChairWorkable> workChore = new WorkChore<BeachChairWorkable>(Db.Get().ChoreTypes.Relax, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, Db.Get().ScheduleBlockTypes.Recreation, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
			workChore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
			return workChore;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, BeachChair, object>.GameInstance
	{
		public StatesInstance(BeachChair smi)
			: base(smi)
		{
		}
	}

	public string specificEffectUnlit;

	public string specificEffectLit;

	public string trackingEffect;

	public const float LIT_RATIO_FOR_POSITIVE_EFFECT = 0.75f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public static void AddModifierDescriptions(List<Descriptor> descs, string effect_id, bool high_lux)
	{
		Effect effect = Db.Get().effects.Get(effect_id);
		LocString locString = (high_lux ? BUILDINGS.PREFABS.BEACHCHAIR.LIGHTEFFECT_HIGH : BUILDINGS.PREFABS.BEACHCHAIR.LIGHTEFFECT_LOW);
		LocString locString2 = (high_lux ? BUILDINGS.PREFABS.BEACHCHAIR.LIGHTEFFECT_HIGH_TOOLTIP : BUILDINGS.PREFABS.BEACHCHAIR.LIGHTEFFECT_LOW_TOOLTIP);
		foreach (AttributeModifier selfModifier in effect.SelfModifiers)
		{
			Descriptor item = new Descriptor(locString.Replace("{attrib}", Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME")).Replace("{amount}", selfModifier.GetFormattedString()).Replace("{lux}", GameUtil.GetFormattedLux(10000)), locString2.Replace("{attrib}", Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME")).Replace("{amount}", selfModifier.GetFormattedString()).Replace("{lux}", GameUtil.GetFormattedLux(10000)));
			item.IncreaseIndent();
			descs.Add(item);
		}
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> obj = new List<Descriptor>
		{
			new Descriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION)
		};
		AddModifierDescriptions(obj, specificEffectLit, high_lux: true);
		AddModifierDescriptions(obj, specificEffectUnlit, high_lux: false);
		return obj;
	}

	public void SetLit(bool v)
	{
		base.smi.sm.lit.Set(v, base.smi);
	}

	public void SetWorker(Worker worker)
	{
		base.smi.sm.worker.Set(worker, base.smi);
	}
}
