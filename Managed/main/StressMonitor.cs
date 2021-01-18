using Klei.AI;
using Klei.CustomSettings;

public class StressMonitor : GameStateMachine<StressMonitor, StressMonitor.Instance>
{
	public class Stressed : State
	{
		public State tier1;

		public State tier2;
	}

	public new class Instance : GameInstance
	{
		public AmountInstance stress;

		private bool allowStressBreak = true;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			stress = Db.Get().Amounts.Stress.Lookup(base.gameObject);
			SettingConfig settingConfig = CustomGameSettings.Instance.QualitySettings[CustomGameSettingConfigs.StressBreaks.id];
			SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.StressBreaks);
			allowStressBreak = settingConfig.IsDefaultLevel(currentQualitySetting.id);
		}

		public bool IsStressed()
		{
			return IsInsideState(base.sm.stressed);
		}

		public bool HasHadEnough()
		{
			if (allowStressBreak)
			{
				return stress.value >= 100f;
			}
			return false;
		}

		public void ReportStress(float dt)
		{
			for (int i = 0; i != stress.deltaAttribute.Modifiers.Count; i++)
			{
				AttributeModifier attributeModifier = stress.deltaAttribute.Modifiers[i];
				DebugUtil.DevAssert(!attributeModifier.IsMultiplier, "Reporting stress for multipliers not supported yet.");
				ReportManager.Instance.ReportValue(ReportManager.ReportType.StressDelta, attributeModifier.Value * dt, attributeModifier.GetDescription(), base.gameObject.GetProperName());
			}
		}

		public Reactable CreateConcernReactable()
		{
			return new EmoteReactable(base.master.gameObject, "StressConcern", Db.Get().ChoreTypes.Emote, "anim_react_concern_kanim", 15, 8, 0f, 30f).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "react"
			});
		}
	}

	public State satisfied;

	public Stressed stressed;

	private const float StressThreshold_One = 60f;

	private const float StressThreshold_Two = 100f;

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = true;
		default_state = satisfied;
		root.Update("StressMonitor", delegate(Instance smi, float dt)
		{
			smi.ReportStress(dt);
		});
		satisfied.Transition(stressed.tier1, (Instance smi) => smi.stress.value >= 60f).ToggleExpression(Db.Get().Expressions.Neutral);
		stressed.ToggleStatusItem(Db.Get().DuplicantStatusItems.Stressed).Transition(satisfied, (Instance smi) => smi.stress.value < 60f).ToggleReactable((Instance smi) => smi.CreateConcernReactable())
			.TriggerOnEnter(GameHashes.Stressed);
		stressed.tier1.Transition(stressed.tier2, (Instance smi) => smi.HasHadEnough());
		stressed.tier2.TriggerOnEnter(GameHashes.StressedHadEnough).Transition(stressed.tier1, (Instance smi) => !smi.HasHadEnough());
	}
}
