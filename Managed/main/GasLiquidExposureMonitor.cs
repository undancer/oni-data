using System.Collections.Generic;
using Klei.AI;
using KSerialization;

public class GasLiquidExposureMonitor : GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public class TUNING
	{
		public const float MINOR_IRRITATION_THRESHOLD = 8f;

		public const float MAJOR_IRRITATION_THRESHOLD = 15f;

		public const float MAX_EXPOSURE = 30f;

		public const float GAS_UNITS = 1f;

		public const float LIQUID_UNITS = 1000f;

		public const float REDUCE_EXPOSURE_RATE_FAST = -1f;

		public const float REDUCE_EXPOSURE_RATE_SLOW = -0.25f;

		public const float NO_CHANGE = 0f;

		public const float SLOW_EXPOSURE_RATE = 0.5f;

		public const float NORMAL_EXPOSURE_RATE = 1f;

		public const float QUICK_EXPOSURE_RATE = 3f;

		public const float DEFAULT_MIN_TEMPERATURE = -13657.5f;

		public const float DEFAULT_MAX_TEMPERATURE = 27315f;

		public const float DEFAULT_LOW_RATE = 1f;

		public const float DEFAULT_HIGH_RATE = 2f;
	}

	public class IrritatedStates : State
	{
		public State irritated;

		public State rubbingEyes;
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public float exposure;

		[Serialize]
		public float lastReactTime;

		[Serialize]
		public float exposureRate;

		public Effects effects;

		public bool isAirtightSuit;

		public float minorIrritationThreshold => 8f;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			effects = master.GetComponent<Effects>();
		}

		public Reactable GetReactable()
		{
			EmoteReactable emoteReactable = new SelfEmoteReactable(base.master.gameObject, "IrritatedEyes", Db.Get().ChoreTypes.Cough, "anim_irritated_eyes_kanim", 0f, 0f).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "irritated_eyes",
				finishcb = delegate
				{
					base.sm.reactFinished.Trigger(this);
				}
			});
			emoteReactable.preventChoreInterruption = true;
			return emoteReactable;
		}

		public bool IsMinorIrritation()
		{
			return exposure >= 8f && exposure < 15f;
		}

		public bool IsMajorIrritation()
		{
			return exposure >= 15f;
		}

		public Element CurrentlyExposedToElement()
		{
			if (isAirtightSuit)
			{
				return ElementLoader.GetElement(SimHashes.Oxygen.CreateTag());
			}
			int num = Grid.CellAbove(Grid.PosToCell(base.smi.gameObject));
			return Grid.Element[num];
		}

		public void ResetExposure()
		{
			exposure = 0f;
		}
	}

	public const float MIN_REACT_INTERVAL = 60f;

	private static Dictionary<SimHashes, float> customExposureRates;

	private static Effect minorIrritationEffect;

	private static Effect majorIrritationEffect;

	public BoolParameter isIrritated;

	public Signal reactFinished;

	public State normal;

	public IrritatedStates irritated;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = normal;
		root.Update(UpdateExposure, UpdateRate.SIM_33ms);
		normal.ParamTransition(isIrritated, irritated, (Instance smi, bool p) => isIrritated.Get(smi));
		irritated.ParamTransition(isIrritated, normal, (Instance smi, bool p) => !isIrritated.Get(smi)).ToggleStatusItem(Db.Get().DuplicantStatusItems.GasLiquidIrritation, (Instance smi) => smi).DefaultState(irritated.irritated);
		irritated.irritated.Transition(irritated.rubbingEyes, CanReact);
		irritated.rubbingEyes.Exit(delegate(Instance smi)
		{
			smi.lastReactTime = GameClock.Instance.GetTime();
		}).ToggleReactable((Instance smi) => smi.GetReactable()).OnSignal(reactFinished, irritated.irritated);
	}

	private static bool CanReact(Instance smi)
	{
		return GameClock.Instance.GetTime() > smi.lastReactTime + 60f;
	}

	private static void InitializeCustomRates()
	{
		if (customExposureRates == null)
		{
			minorIrritationEffect = Db.Get().effects.Get("MinorIrritation");
			majorIrritationEffect = Db.Get().effects.Get("MajorIrritation");
			customExposureRates = new Dictionary<SimHashes, float>();
			float value = -1f;
			customExposureRates[SimHashes.Water] = value;
			float value2 = -0.25f;
			customExposureRates[SimHashes.CarbonDioxide] = value2;
			customExposureRates[SimHashes.Oxygen] = value2;
			float value3 = 0f;
			customExposureRates[SimHashes.ContaminatedOxygen] = value3;
			customExposureRates[SimHashes.DirtyWater] = value3;
			customExposureRates[SimHashes.ViscoGel] = value3;
			float value4 = 0.5f;
			customExposureRates[SimHashes.Hydrogen] = value4;
			customExposureRates[SimHashes.SaltWater] = value4;
			float value5 = 1f;
			customExposureRates[SimHashes.ChlorineGas] = value5;
			customExposureRates[SimHashes.EthanolGas] = value5;
			float value6 = 3f;
			customExposureRates[SimHashes.Chlorine] = value6;
			customExposureRates[SimHashes.SourGas] = value6;
			customExposureRates[SimHashes.Brine] = value6;
			customExposureRates[SimHashes.Ethanol] = value6;
			customExposureRates[SimHashes.SuperCoolant] = value6;
			customExposureRates[SimHashes.CrudeOil] = value6;
			customExposureRates[SimHashes.Naphtha] = value6;
			customExposureRates[SimHashes.Petroleum] = value6;
		}
	}

	public float GetCurrentExposure(Instance smi)
	{
		if (customExposureRates.TryGetValue(smi.CurrentlyExposedToElement().id, out var value))
		{
			return value;
		}
		return 0f;
	}

	private void UpdateExposure(Instance smi, float dt)
	{
		InitializeCustomRates();
		float exposureRate = 0f;
		smi.isAirtightSuit = false;
		int num = Grid.CellAbove(Grid.PosToCell(smi.gameObject));
		if (Grid.IsValidCell(num))
		{
			Element element = Grid.Element[num];
			if (!customExposureRates.TryGetValue(element.id, out var value))
			{
				value = ((!(Grid.Temperature[num] >= -13657.5f) || !(Grid.Temperature[num] <= 27315f)) ? 2f : 1f);
			}
			if (smi.master.gameObject.HasTag(GameTags.HasSuitTank) && (bool)smi.gameObject.GetComponent<SuitEquipper>().IsWearingAirtightSuit())
			{
				smi.isAirtightSuit = true;
				exposureRate = customExposureRates[SimHashes.Oxygen];
			}
			else if (element.IsGas)
			{
				exposureRate = value * Grid.Mass[num] / 1f;
			}
			else if (element.IsLiquid)
			{
				exposureRate = value * Grid.Mass[num] / 1000f;
			}
		}
		smi.exposureRate = exposureRate;
		smi.exposure += smi.exposureRate * dt;
		smi.exposure = MathUtil.Clamp(0f, 30f, smi.exposure);
		ApplyEffects(smi);
	}

	private void ApplyEffects(Instance smi)
	{
		if (smi.IsMinorIrritation())
		{
			smi.effects.Add(minorIrritationEffect, should_save: true);
			isIrritated.Set(value: true, smi);
		}
		else if (smi.IsMajorIrritation())
		{
			smi.effects.Add(majorIrritationEffect, should_save: true);
			isIrritated.Set(value: true, smi);
		}
		else
		{
			smi.effects.Remove(minorIrritationEffect);
			smi.effects.Remove(majorIrritationEffect);
			isIrritated.Set(value: false, smi);
		}
	}

	public Effect GetAppliedEffect(Instance smi)
	{
		if (smi.IsMinorIrritation())
		{
			return minorIrritationEffect;
		}
		if (smi.IsMajorIrritation())
		{
			return majorIrritationEffect;
		}
		return null;
	}
}
