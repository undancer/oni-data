using System.Collections.Generic;
using Klei.AI;
using Klei.CustomSettings;
using STRINGS;
using TUNING;
using UnityEngine;

public class RadiationMonitor : GameStateMachine<RadiationMonitor, RadiationMonitor.Instance>
{
	public class ActiveStates : State
	{
		public State idle;

		public SickStates sick;
	}

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

		public float difficultySettingMod = 1f;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			effects = GetComponent<Effects>();
			if (!Sim.IsRadiationEnabled())
			{
				return;
			}
			SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Radiation);
			if (currentQualitySetting != null)
			{
				switch (currentQualitySetting.id)
				{
				case "Easiest":
					difficultySettingMod = DUPLICANTSTATS.RADIATION_DIFFICULTY_MODIFIERS.EASIEST;
					break;
				case "Easier":
					difficultySettingMod = DUPLICANTSTATS.RADIATION_DIFFICULTY_MODIFIERS.EASIER;
					break;
				case "Harder":
					difficultySettingMod = DUPLICANTSTATS.RADIATION_DIFFICULTY_MODIFIERS.HARDER;
					break;
				case "Hardest":
					difficultySettingMod = DUPLICANTSTATS.RADIATION_DIFFICULTY_MODIFIERS.HARDEST;
					break;
				}
			}
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

		public float SicknessSecondsRemaining()
		{
			return 600f * (Mathf.Max(0f, base.sm.radiationExposure.Get(base.smi) - 100f * difficultySettingMod) / 100f);
		}

		public string GetEffectStatusTooltip()
		{
			if (effects.HasEffect(minorSicknessEffect))
			{
				return base.smi.master.gameObject.GetComponent<Effects>().Get(minorSicknessEffect).statusItem.GetTooltip(effects.Get(minorSicknessEffect));
			}
			if (effects.HasEffect(majorSicknessEffect))
			{
				return base.smi.master.gameObject.GetComponent<Effects>().Get(majorSicknessEffect).statusItem.GetTooltip(effects.Get(majorSicknessEffect));
			}
			if (effects.HasEffect(extremeSicknessEffect))
			{
				return base.smi.master.gameObject.GetComponent<Effects>().Get(extremeSicknessEffect).statusItem.GetTooltip(effects.Get(extremeSicknessEffect));
			}
			return DUPLICANTS.MODIFIERS.RADIATIONEXPOSUREDEADLY.TOOLTIP;
		}
	}

	public const float BASE_ABSORBTION_RATE = 1f;

	public const float MIN_TIME_BETWEEN_EXPOSURE_REACTS = 120f;

	public const float MIN_TIME_BETWEEN_SICK_REACTS = 60f;

	public const int VOMITS_PER_CYCLE_MAJOR = 5;

	public const int VOMITS_PER_CYCLE_EXTREME = 10;

	public FloatParameter radiationExposure;

	public FloatParameter currentExposurePerCycle;

	public BoolParameter isSick;

	public FloatParameter timeUntilNextExposureReact;

	public FloatParameter timeUntilNextSickReact;

	public static string minorSicknessEffect = "RadiationExposureMinor";

	public static string majorSicknessEffect = "RadiationExposureMajor";

	public static string extremeSicknessEffect = "RadiationExposureExtreme";

	public State init;

	public ActiveStates active;

	public static readonly Parameter<float>.Callback COMPARE_RECOVERY_IMMEDIATE = (Instance smi, float p) => p > 100f * smi.difficultySettingMod / 2f;

	public static readonly Parameter<float>.Callback COMPARE_REACT = (Instance smi, float p) => p >= 133f * smi.difficultySettingMod;

	public static readonly Parameter<float>.Callback COMPARE_LT_MINOR = (Instance smi, float p) => p < 100f * smi.difficultySettingMod;

	public static readonly Parameter<float>.Callback COMPARE_GTE_MINOR = (Instance smi, float p) => p >= 100f * smi.difficultySettingMod;

	public static readonly Parameter<float>.Callback COMPARE_GTE_MAJOR = (Instance smi, float p) => p >= 300f * smi.difficultySettingMod;

	public static readonly Parameter<float>.Callback COMPARE_GTE_EXTREME = (Instance smi, float p) => p >= 600f * smi.difficultySettingMod;

	public static readonly Parameter<float>.Callback COMPARE_GTE_DEADLY = (Instance smi, float p) => p >= 900f * smi.difficultySettingMod;

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = SerializeType.ParamsOnly;
		default_state = init;
		init.Transition(null, (Instance smi) => !Sim.IsRadiationEnabled()).Transition(active, (Instance smi) => Sim.IsRadiationEnabled());
		active.Update(CheckRadiationLevel, UpdateRate.SIM_1000ms).DefaultState(active.idle);
		active.idle.DoNothing().ParamTransition(radiationExposure, active.sick.deadly, COMPARE_GTE_DEADLY).ParamTransition(radiationExposure, active.sick.extreme, COMPARE_GTE_EXTREME)
			.ParamTransition(radiationExposure, active.sick.major, COMPARE_GTE_MAJOR)
			.ParamTransition(radiationExposure, active.sick.minor, COMPARE_GTE_MINOR);
		active.sick.ParamTransition(radiationExposure, active.idle, COMPARE_LT_MINOR).Enter(delegate(Instance smi)
		{
			smi.sm.isSick.Set(value: true, smi);
		}).Exit(delegate(Instance smi)
		{
			smi.sm.isSick.Set(value: false, smi);
		});
		active.sick.minor.ToggleEffect(minorSicknessEffect).ParamTransition(radiationExposure, active.sick.deadly, COMPARE_GTE_DEADLY).ParamTransition(radiationExposure, active.sick.extreme, COMPARE_GTE_EXTREME)
			.ParamTransition(radiationExposure, active.sick.major, COMPARE_GTE_MAJOR)
			.ToggleAnims("anim_loco_radiation1_kanim", 4f)
			.ToggleAnims("anim_idle_radiation1_kanim", 4f)
			.ToggleExpression(Db.Get().Expressions.Radiation1)
			.DefaultState(active.sick.minor.waiting);
		active.sick.minor.reacting.ToggleChore(CreateVomitChore, active.sick.minor.waiting);
		active.sick.major.ToggleEffect(majorSicknessEffect).ParamTransition(radiationExposure, active.sick.deadly, COMPARE_GTE_DEADLY).ParamTransition(radiationExposure, active.sick.extreme, COMPARE_GTE_EXTREME)
			.ToggleAnims("anim_loco_radiation2_kanim", 4f)
			.ToggleAnims("anim_idle_radiation2_kanim", 4f)
			.ToggleExpression(Db.Get().Expressions.Radiation2)
			.DefaultState(active.sick.major.waiting);
		active.sick.major.waiting.ScheduleGoTo(120f, active.sick.major.vomiting);
		active.sick.major.vomiting.ToggleChore(CreateVomitChore, active.sick.major.waiting);
		active.sick.extreme.ParamTransition(radiationExposure, active.sick.deadly, COMPARE_GTE_DEADLY).ToggleEffect(extremeSicknessEffect).ToggleAnims("anim_loco_radiation3_kanim", 4f)
			.ToggleAnims("anim_idle_radiation3_kanim", 4f)
			.ToggleExpression(Db.Get().Expressions.Radiation3)
			.DefaultState(active.sick.extreme.waiting);
		active.sick.extreme.waiting.ScheduleGoTo(60f, active.sick.extreme.vomiting);
		active.sick.extreme.vomiting.ToggleChore(CreateVomitChore, active.sick.extreme.waiting);
		active.sick.deadly.ToggleAnims("anim_loco_radiation4_kanim", 4f).ToggleAnims("anim_idle_radiation4_kanim", 4f).ToggleExpression(Db.Get().Expressions.Radiation4)
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
		float num = Db.Get().Attributes.RadiationRecovery.Lookup(smi.gameObject).GetTotalValue() * dt;
		smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).ApplyDelta(num);
		smi.master.Trigger(1556680150, num);
	}

	private static void CheckRadiationLevel(Instance smi, float dt)
	{
		RadiationRecovery(smi, dt);
		smi.sm.timeUntilNextExposureReact.Delta(0f - dt, smi);
		smi.sm.timeUntilNextSickReact.Delta(0f - dt, smi);
		int num = Grid.PosToCell(smi.gameObject);
		if (Grid.IsValidCell(num))
		{
			float num2 = Mathf.Clamp01(1f - Db.Get().Attributes.RadiationResistance.Lookup(smi.gameObject).GetTotalValue());
			float num3 = Grid.Radiation[num] * 1f * num2 / 600f * dt;
			smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).ApplyDelta(num3);
			float num4 = num3 / dt * 600f;
			smi.sm.currentExposurePerCycle.Set(num4, smi);
			if (smi.sm.timeUntilNextExposureReact.Get(smi) <= 0f && COMPARE_REACT(smi, num4))
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
