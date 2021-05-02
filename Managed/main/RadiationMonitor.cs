using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class RadiationMonitor : GameStateMachine<RadiationMonitor, RadiationMonitor.Instance>
{
	public class SickStates : State
	{
		public class MinorStates : State
		{
			public State waiting;

			public State reacting;
		}

		public class MajorStates : State
		{
			public State waiting;

			public State vomiting;
		}

		public class ExtremeStates : State
		{
			public State waiting;

			public State vomiting;
		}

		public MinorStates minor;

		public MajorStates major;

		public ExtremeStates extreme;

		public State deadly;
	}

	public new class Instance : GameInstance
	{
		public Effects effects;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			effects = GetComponent<Effects>();
		}

		public Reactable GetReactable()
		{
			EmoteReactable emoteReactable = new SelfEmoteReactable(base.master.gameObject, "RadiationSicknessReact", Db.Get().ChoreTypes.RadiationPain, "anim_react_radiation_kanim", 0f, 0f).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "react_radiation_itch"
			});
			emoteReactable.preventChoreInterruption = false;
			return emoteReactable;
		}
	}

	public const float BASE_ABSORBTION_RATE = 1f;

	public const float REACT_THRESHOLD = 355f / (678f * (float)Math.PI);

	public const float MIN_TIME_BETWEEN_EXPOSURE_REACTS = 120f;

	public const float MIN_TIME_BETWEEN_SICK_REACTS = 60f;

	public const int VOMITS_PER_CYCLE_MAJOR = 5;

	public const int VOMITS_PER_CYCLE_EXTREME = 10;

	public FloatParameter radiationExposure;

	public FloatParameter currentExposurePerCycle;

	public BoolParameter isSick;

	public FloatParameter timeUntilNextExposureReact;

	public FloatParameter timeUntilNextSickReact;

	public State idle;

	public SickStates sick;

	protected static Parameter<float>.Callback COMPARE_LT_MINOR = (Instance smi, float p) => p < 100f;

	protected static Parameter<float>.Callback COMPARE_GTE_MINOR = (Instance smi, float p) => p >= 100f;

	protected static Parameter<float>.Callback COMPARE_GTE_MAJOR = (Instance smi, float p) => p >= 300f;

	protected static Parameter<float>.Callback COMPARE_GTE_EXTREME = (Instance smi, float p) => p >= 600f;

	protected static Parameter<float>.Callback COMPARE_GTE_DEADLY = (Instance smi, float p) => p >= 900f;

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = SerializeType.ParamsOnly;
		default_state = idle;
		root.Update(CheckRadiationLevel, UpdateRate.SIM_1000ms);
		idle.DoNothing().ParamTransition(radiationExposure, sick.deadly, COMPARE_GTE_DEADLY).ParamTransition(radiationExposure, sick.extreme, COMPARE_GTE_EXTREME)
			.ParamTransition(radiationExposure, sick.major, COMPARE_GTE_MAJOR)
			.ParamTransition(radiationExposure, sick.minor, COMPARE_GTE_MINOR);
		sick.ParamTransition(radiationExposure, idle, COMPARE_LT_MINOR).Enter(delegate(Instance smi)
		{
			smi.sm.isSick.Set(value: true, smi);
		}).Exit(delegate(Instance smi)
		{
			smi.sm.isSick.Set(value: false, smi);
		});
		sick.minor.ToggleEffect("RadiationExposureMinor").ParamTransition(radiationExposure, sick.deadly, COMPARE_GTE_DEADLY).ParamTransition(radiationExposure, sick.extreme, COMPARE_GTE_EXTREME)
			.ParamTransition(radiationExposure, sick.major, COMPARE_GTE_MAJOR)
			.ToggleAnims("anim_loco_radiation1_kanim", 4f)
			.ToggleAnims("anim_idle_radiation1_kanim", 4f)
			.ToggleExpression(Db.Get().Expressions.Radiation1)
			.DefaultState(sick.minor.waiting);
		sick.minor.reacting.ToggleChore(CreateVomitChore, sick.minor.waiting);
		sick.major.ToggleEffect("RadiationExposureMajor").ParamTransition(radiationExposure, sick.deadly, COMPARE_GTE_DEADLY).ParamTransition(radiationExposure, sick.extreme, COMPARE_GTE_EXTREME)
			.ToggleAnims("anim_loco_radiation2_kanim", 4f)
			.ToggleAnims("anim_idle_radiation2_kanim", 4f)
			.ToggleExpression(Db.Get().Expressions.Radiation2)
			.DefaultState(sick.major.waiting);
		sick.major.waiting.ScheduleGoTo(120f, sick.major.vomiting);
		sick.major.vomiting.ToggleChore(CreateVomitChore, sick.major.waiting);
		sick.extreme.ParamTransition(radiationExposure, sick.deadly, COMPARE_GTE_DEADLY).ToggleEffect("RadiationExposureExtreme").ToggleAnims("anim_loco_radiation3_kanim", 4f)
			.ToggleAnims("anim_idle_radiation3_kanim", 4f)
			.ToggleExpression(Db.Get().Expressions.Radiation3)
			.DefaultState(sick.extreme.waiting);
		sick.extreme.waiting.ScheduleGoTo(60f, sick.extreme.vomiting);
		sick.extreme.vomiting.ToggleChore(CreateVomitChore, sick.extreme.waiting);
		sick.deadly.ToggleAnims("anim_loco_radiation4_kanim", 4f).ToggleAnims("anim_idle_radiation4_kanim", 4f).ToggleExpression(Db.Get().Expressions.Radiation4)
			.Enter(delegate(Instance smi)
			{
				smi.GetComponent<Health>().Incapacitate(GameTags.RadiationSicknessIncapacitation);
			});
	}

	private Chore CreateVomitChore(Instance smi)
	{
		Notification notification = new Notification(DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => string.Concat(DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_TOOLTIP, notificationList.ReduceMessages(countNames: false)));
		return new VomitChore(Db.Get().ChoreTypes.Vomit, smi.master, Db.Get().DuplicantStatusItems.Vomiting, notification);
	}

	private static void RadiationRecovery(Instance smi, float dt)
	{
		smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).ApplyDelta(Db.Get().Attributes.RadiationRecovery.Lookup(smi.gameObject).GetTotalValue() * dt);
	}

	private static void CheckRadiationLevel(Instance smi, float dt)
	{
		RadiationRecovery(smi, dt);
		smi.sm.timeUntilNextExposureReact.Delta(0f - dt, smi);
		smi.sm.timeUntilNextSickReact.Delta(0f - dt, smi);
		int num = Grid.PosToCell(smi.gameObject);
		if (Grid.IsValidCell(num))
		{
			float num2 = 1f - Db.Get().Attributes.RadiationResistance.Lookup(smi.gameObject).GetTotalValue();
			float num3 = Grid.Radiation[num] * 1f * num2 / 600f * dt;
			smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).ApplyDelta(num3);
			smi.sm.currentExposurePerCycle.Set(num3 / dt * 600f, smi);
			if (smi.sm.timeUntilNextExposureReact.Get(smi) <= 0f && num3 > 355f / (678f * (float)Math.PI) / dt)
			{
				smi.sm.timeUntilNextExposureReact.Set(120f, smi);
				ReactionMonitor.Instance sMI = smi.master.gameObject.GetSMI<ReactionMonitor.Instance>();
				SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(smi.master.gameObject, "RadiationReact", Db.Get().ChoreTypes.EmoteHighPriority, "anim_react_radiation_kanim");
				selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
				{
					anim = "react_radiation_glare"
				});
				sMI.AddOneshotReactable(selfEmoteReactable);
			}
		}
		if (smi.sm.timeUntilNextSickReact.Get(smi) <= 0f && smi.sm.isSick.Get(smi))
		{
			smi.sm.timeUntilNextSickReact.Set(60f, smi);
			ReactionMonitor.Instance sMI2 = smi.master.gameObject.GetSMI<ReactionMonitor.Instance>();
			SelfEmoteReactable selfEmoteReactable2 = new SelfEmoteReactable(smi.master.gameObject, "RadiationReact", Db.Get().ChoreTypes.RadiationPain, "anim_react_radiation_kanim");
			selfEmoteReactable2.AddStep(new EmoteReactable.EmoteStep
			{
				anim = "react_radiation_itch"
			});
			sMI2.AddOneshotReactable(selfEmoteReactable2);
		}
		smi.sm.radiationExposure.Set(smi.master.gameObject.GetComponent<KSelectable>().GetAmounts().GetValue("RadiationBalance"), smi);
	}
}
