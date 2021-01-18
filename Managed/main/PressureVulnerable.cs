using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class PressureVulnerable : StateMachineComponent<PressureVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause, ISlicedSim1000ms
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PressureVulnerable, object>.GameInstance
	{
		public bool hasMaturity;

		public StatesInstance(PressureVulnerable master)
			: base(master)
		{
			if (Db.Get().Amounts.Maturity.Lookup(base.gameObject) != null)
			{
				hasMaturity = true;
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PressureVulnerable>
	{
		public FloatParameter pressure;

		public BoolParameter safe_element;

		public State unsafeElement;

		public State lethalLow;

		public State lethalHigh;

		public State warningLow;

		public State warningHigh;

		public State normal;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = normal;
			lethalLow.ParamTransition(pressure, warningLow, (StatesInstance smi, float p) => p > smi.master.pressureLethal_Low).ParamTransition(safe_element, unsafeElement, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsFalse).Enter(delegate(StatesInstance smi)
			{
				smi.master.pressureState = PressureState.LethalLow;
			})
				.TriggerOnEnter(GameHashes.LowPressureFatal);
			lethalHigh.ParamTransition(pressure, warningHigh, (StatesInstance smi, float p) => p < smi.master.pressureLethal_High).ParamTransition(safe_element, unsafeElement, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsFalse).Enter(delegate(StatesInstance smi)
			{
				smi.master.pressureState = PressureState.LethalHigh;
			})
				.TriggerOnEnter(GameHashes.HighPressureFatal);
			warningLow.ParamTransition(pressure, lethalLow, (StatesInstance smi, float p) => p < smi.master.pressureLethal_Low).ParamTransition(pressure, normal, (StatesInstance smi, float p) => p > smi.master.pressureWarning_Low).ParamTransition(safe_element, unsafeElement, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsFalse)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.pressureState = PressureState.WarningLow;
				})
				.TriggerOnEnter(GameHashes.LowPressureWarning);
			unsafeElement.ParamTransition(safe_element, normal, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsTrue).TriggerOnExit(GameHashes.CorrectAtmosphere).TriggerOnEnter(GameHashes.WrongAtmosphere);
			warningHigh.ParamTransition(pressure, lethalHigh, (StatesInstance smi, float p) => p > smi.master.pressureLethal_High).ParamTransition(pressure, normal, (StatesInstance smi, float p) => p < smi.master.pressureWarning_High).ParamTransition(safe_element, unsafeElement, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsFalse)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.pressureState = PressureState.WarningHigh;
				})
				.TriggerOnEnter(GameHashes.HighPressureWarning);
			normal.ParamTransition(pressure, warningHigh, (StatesInstance smi, float p) => p > smi.master.pressureWarning_High).ParamTransition(pressure, warningLow, (StatesInstance smi, float p) => p < smi.master.pressureWarning_Low).ParamTransition(safe_element, unsafeElement, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsFalse)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.pressureState = PressureState.Normal;
				})
				.TriggerOnEnter(GameHashes.OptimalPressureAchieved);
		}
	}

	public enum PressureState
	{
		LethalLow,
		WarningLow,
		Normal,
		WarningHigh,
		LethalHigh
	}

	private HandleVector<int>.Handle pressureAccumulator = HandleVector<int>.InvalidHandle;

	private HandleVector<int>.Handle elementAccumulator = HandleVector<int>.InvalidHandle;

	private OccupyArea _occupyArea;

	public float pressureLethal_Low;

	public float pressureWarning_Low;

	public float pressureWarning_High;

	public float pressureLethal_High;

	private static float testAreaPressure;

	private static int testAreaCount;

	public bool testAreaElementSafe = true;

	public Element currentAtmoElement;

	private static Func<int, object, bool> testAreaCB = delegate(int test_cell, object data)
	{
		PressureVulnerable pressureVulnerable = (PressureVulnerable)data;
		if (!Grid.IsSolidCell(test_cell))
		{
			Element element = Grid.Element[test_cell];
			if (pressureVulnerable.IsSafeElement(element))
			{
				testAreaPressure += Grid.Mass[test_cell];
				testAreaCount++;
				pressureVulnerable.testAreaElementSafe = true;
				pressureVulnerable.currentAtmoElement = element;
			}
			if (pressureVulnerable.currentAtmoElement == null)
			{
				pressureVulnerable.currentAtmoElement = element;
			}
		}
		return true;
	};

	private AmountInstance displayPressureAmount;

	public bool pressure_sensitive = true;

	public HashSet<Element> safe_atmospheres = new HashSet<Element>();

	private int cell;

	private PressureState pressureState = PressureState.Normal;

	private OccupyArea occupyArea
	{
		get
		{
			if (_occupyArea == null)
			{
				_occupyArea = GetComponent<OccupyArea>();
			}
			return _occupyArea;
		}
	}

	public PressureState ExternalPressureState => pressureState;

	public bool IsLethal
	{
		get
		{
			if (pressureState != PressureState.LethalHigh && pressureState != 0)
			{
				return !testAreaElementSafe;
			}
			return true;
		}
	}

	public bool IsNormal
	{
		get
		{
			if (testAreaElementSafe)
			{
				return pressureState == PressureState.Normal;
			}
			return false;
		}
	}

	WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[2]
	{
		WiltCondition.Condition.Pressure,
		WiltCondition.Condition.AtmosphereElement
	};

	public string WiltStateString
	{
		get
		{
			string text = "";
			if (base.smi.IsInsideState(base.smi.sm.warningLow) || base.smi.IsInsideState(base.smi.sm.lethalLow))
			{
				text += Db.Get().CreatureStatusItems.AtmosphericPressureTooLow.resolveStringCallback(CREATURES.STATUSITEMS.ATMOSPHERICPRESSURETOOLOW.NAME, this);
			}
			else if (base.smi.IsInsideState(base.smi.sm.warningHigh) || base.smi.IsInsideState(base.smi.sm.lethalHigh))
			{
				text += Db.Get().CreatureStatusItems.AtmosphericPressureTooHigh.resolveStringCallback(CREATURES.STATUSITEMS.ATMOSPHERICPRESSURETOOHIGH.NAME, this);
			}
			else if (base.smi.IsInsideState(base.smi.sm.unsafeElement))
			{
				text += Db.Get().CreatureStatusItems.WrongAtmosphere.resolveStringCallback(CREATURES.STATUSITEMS.WRONGATMOSPHERE.NAME, this);
			}
			return text;
		}
	}

	public bool IsSafeElement(Element element)
	{
		if (safe_atmospheres == null || safe_atmospheres.Count == 0 || safe_atmospheres.Contains(element))
		{
			return true;
		}
		return false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Amounts amounts = base.gameObject.GetAmounts();
		displayPressureAmount = amounts.Add(new AmountInstance(Db.Get().Amounts.AirPressure, base.gameObject));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SlicedUpdaterSim1000ms<PressureVulnerable>.instance.RegisterUpdate1000ms(this);
		cell = Grid.PosToCell(this);
		base.smi.sm.pressure.Set(1f, base.smi);
		base.smi.sm.safe_element.Set(testAreaElementSafe, base.smi);
		base.smi.master.pressureAccumulator = Game.Instance.accumulators.Add("pressureAccumulator", this);
		base.smi.master.elementAccumulator = Game.Instance.accumulators.Add("elementAccumulator", this);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		SlicedUpdaterSim1000ms<PressureVulnerable>.instance.UnregisterUpdate1000ms(this);
		base.OnCleanUp();
	}

	public void Configure(SimHashes[] safeAtmospheres = null)
	{
		pressure_sensitive = false;
		pressureWarning_Low = float.MinValue;
		pressureLethal_Low = float.MinValue;
		pressureLethal_High = float.MaxValue;
		pressureWarning_High = float.MaxValue;
		safe_atmospheres = new HashSet<Element>();
		if (safeAtmospheres != null)
		{
			foreach (SimHashes hash in safeAtmospheres)
			{
				safe_atmospheres.Add(ElementLoader.FindElementByHash(hash));
			}
		}
	}

	public void Configure(float pressureWarningLow = 0.25f, float pressureLethalLow = 0.01f, float pressureWarningHigh = 10f, float pressureLethalHigh = 30f, SimHashes[] safeAtmospheres = null)
	{
		pressure_sensitive = true;
		pressureWarning_Low = pressureWarningLow;
		pressureLethal_Low = pressureLethalLow;
		pressureLethal_High = pressureLethalHigh;
		pressureWarning_High = pressureWarningHigh;
		safe_atmospheres = new HashSet<Element>();
		if (safeAtmospheres != null)
		{
			foreach (SimHashes hash in safeAtmospheres)
			{
				safe_atmospheres.Add(ElementLoader.FindElementByHash(hash));
			}
		}
	}

	public bool IsSafePressure(float pressure)
	{
		if (pressure_sensitive)
		{
			if (pressure > pressureLethal_Low)
			{
				return pressure < pressureLethal_High;
			}
			return false;
		}
		return true;
	}

	public void SlicedSim1000ms(float dt)
	{
		float pressureOverArea = GetPressureOverArea(cell);
		Game.Instance.accumulators.Accumulate(base.smi.master.pressureAccumulator, pressureOverArea);
		float averageRate = Game.Instance.accumulators.GetAverageRate(base.smi.master.pressureAccumulator);
		displayPressureAmount.value = averageRate;
		Game.Instance.accumulators.Accumulate(base.smi.master.elementAccumulator, testAreaElementSafe ? 1f : 0f);
		bool value = ((Game.Instance.accumulators.GetAverageRate(base.smi.master.elementAccumulator) > 0f) ? true : false);
		base.smi.sm.safe_element.Set(value, base.smi);
		base.smi.sm.pressure.Set(averageRate, base.smi);
	}

	public float GetExternalPressure()
	{
		return GetPressureOverArea(cell);
	}

	private float GetPressureOverArea(int cell)
	{
		bool flag = testAreaElementSafe;
		testAreaPressure = 0f;
		testAreaCount = 0;
		testAreaElementSafe = false;
		currentAtmoElement = null;
		occupyArea.TestArea(cell, this, testAreaCB);
		occupyArea.TestAreaAbove(cell, this, testAreaCB);
		testAreaPressure = ((testAreaCount > 0) ? (testAreaPressure / (float)testAreaCount) : 0f);
		if (testAreaElementSafe != flag)
		{
			Trigger(-2023773544);
		}
		return testAreaPressure;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (pressure_sensitive)
		{
			list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_PRESSURE, GameUtil.GetFormattedMass(pressureWarning_Low)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_PRESSURE, GameUtil.GetFormattedMass(pressureWarning_Low)), Descriptor.DescriptorType.Requirement));
		}
		if (safe_atmospheres != null && safe_atmospheres.Count > 0)
		{
			string text = "";
			foreach (Element safe_atmosphere in safe_atmospheres)
			{
				text = text + "\n        • " + safe_atmosphere.name;
			}
			list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_ATMOSPHERE, text), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_ATMOSPHERE, text), Descriptor.DescriptorType.Requirement));
		}
		return list;
	}
}
