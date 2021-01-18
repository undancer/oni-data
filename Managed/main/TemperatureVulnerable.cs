using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class TemperatureVulnerable : StateMachineComponent<TemperatureVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause, ISlicedSim1000ms
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, TemperatureVulnerable, object>.GameInstance
	{
		public bool hasMaturity = false;

		public StatesInstance(TemperatureVulnerable master)
			: base(master)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(base.gameObject);
			if (amountInstance != null)
			{
				hasMaturity = true;
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, TemperatureVulnerable>
	{
		public FloatParameter internalTemp;

		public State lethalCold;

		public State lethalHot;

		public State warningCold;

		public State warningHot;

		public State normal;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = normal;
			lethalCold.Enter(delegate(StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureState.LethalCold;
			}).TriggerOnEnter(GameHashes.TooColdFatal).ParamTransition(internalTemp, warningCold, (StatesInstance smi, float p) => p > smi.master.internalTemperatureLethal_Low)
				.Enter(Kill);
			lethalHot.Enter(delegate(StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureState.LethalHot;
			}).TriggerOnEnter(GameHashes.TooHotFatal).ParamTransition(internalTemp, warningHot, (StatesInstance smi, float p) => p < smi.master.internalTemperatureLethal_High)
				.Enter(Kill);
			warningCold.Enter(delegate(StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureState.WarningCold;
			}).TriggerOnEnter(GameHashes.TooColdWarning).ParamTransition(internalTemp, lethalCold, (StatesInstance smi, float p) => p < smi.master.internalTemperatureLethal_Low)
				.ParamTransition(internalTemp, normal, (StatesInstance smi, float p) => p > smi.master.internalTemperatureWarning_Low);
			warningHot.Enter(delegate(StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureState.WarningHot;
			}).TriggerOnEnter(GameHashes.TooHotWarning).ParamTransition(internalTemp, lethalHot, (StatesInstance smi, float p) => p > smi.master.internalTemperatureLethal_High)
				.ParamTransition(internalTemp, normal, (StatesInstance smi, float p) => p < smi.master.internalTemperatureWarning_High);
			normal.Enter(delegate(StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureState.Normal;
			}).TriggerOnEnter(GameHashes.OptimalTemperatureAchieved).ParamTransition(internalTemp, warningHot, (StatesInstance smi, float p) => p > smi.master.internalTemperatureWarning_High)
				.ParamTransition(internalTemp, warningCold, (StatesInstance smi, float p) => p < smi.master.internalTemperatureWarning_Low);
		}

		private static void Kill(Instance smi)
		{
			smi.GetSMI<DeathMonitor.Instance>()?.Kill(Db.Get().Deaths.Generic);
		}
	}

	public enum TemperatureState
	{
		LethalCold,
		WarningCold,
		Normal,
		WarningHot,
		LethalHot
	}

	private OccupyArea _occupyArea;

	public float internalTemperatureLethal_Low;

	public float internalTemperatureWarning_Low;

	public float internalTemperaturePerfect_Low;

	public float internalTemperaturePerfect_High;

	public float internalTemperatureWarning_High;

	public float internalTemperatureLethal_High;

	private const float minimumMassForReading = 0.1f;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private SimTemperatureTransfer temperatureTransfer;

	private AmountInstance displayTemperatureAmount;

	private TemperatureState internalTemperatureState = TemperatureState.Normal;

	private float averageTemp;

	private int cellCount;

	private static readonly Func<int, object, bool> GetAverageTemperatureCbDelegate = (int cell, object data) => GetAverageTemperatureCb(cell, data);

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

	public float InternalTemperature => primaryElement.Temperature;

	public TemperatureState GetInternalTemperatureState => internalTemperatureState;

	public bool IsLethal => GetInternalTemperatureState == TemperatureState.LethalHot || GetInternalTemperatureState == TemperatureState.LethalCold;

	public bool IsNormal => GetInternalTemperatureState == TemperatureState.Normal;

	WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[1];

	public string WiltStateString
	{
		get
		{
			if (base.smi.IsInsideState(base.smi.sm.warningCold))
			{
				return Db.Get().CreatureStatusItems.Cold_Crop.resolveStringCallback(CREATURES.STATUSITEMS.COLD_CROP.NAME, this);
			}
			if (base.smi.IsInsideState(base.smi.sm.warningHot))
			{
				return Db.Get().CreatureStatusItems.Hot_Crop.resolveStringCallback(CREATURES.STATUSITEMS.HOT_CROP.NAME, this);
			}
			return "";
		}
	}

	public event Action<float, float> OnTemperature;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Amounts amounts = base.gameObject.GetAmounts();
		displayTemperatureAmount = amounts.Add(new AmountInstance(Db.Get().Amounts.Temperature, base.gameObject));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SlicedUpdaterSim1000ms<TemperatureVulnerable>.instance.RegisterUpdate1000ms(this);
		base.smi.sm.internalTemp.Set(primaryElement.Temperature, base.smi);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		SlicedUpdaterSim1000ms<TemperatureVulnerable>.instance.UnregisterUpdate1000ms(this);
	}

	public void Configure(float tempWarningLow, float tempLethalLow, float tempWarningHigh, float tempLethalHigh)
	{
		internalTemperatureWarning_Low = tempWarningLow;
		internalTemperatureLethal_Low = tempLethalLow;
		internalTemperatureLethal_High = tempLethalHigh;
		internalTemperatureWarning_High = tempWarningHigh;
	}

	public bool IsCellSafe(int cell)
	{
		float averageTemperature = GetAverageTemperature(cell);
		return averageTemperature > -1f && averageTemperature > internalTemperatureLethal_Low && averageTemperature < internalTemperatureLethal_High;
	}

	public void SlicedSim1000ms(float dt)
	{
		int cell = Grid.PosToCell(base.gameObject);
		if (Grid.IsValidCell(cell))
		{
			base.smi.sm.internalTemp.Set(InternalTemperature, base.smi);
			displayTemperatureAmount.value = InternalTemperature;
			if (this.OnTemperature != null)
			{
				this.OnTemperature(dt, InternalTemperature);
			}
		}
	}

	private static bool GetAverageTemperatureCb(int cell, object data)
	{
		TemperatureVulnerable temperatureVulnerable = data as TemperatureVulnerable;
		if (Grid.Mass[cell] > 0.1f)
		{
			temperatureVulnerable.averageTemp += Grid.Temperature[cell];
			temperatureVulnerable.cellCount++;
		}
		return true;
	}

	private float GetAverageTemperature(int cell)
	{
		averageTemp = 0f;
		cellCount = 0;
		occupyArea.TestArea(cell, this, GetAverageTemperatureCbDelegate);
		if (cellCount > 0)
		{
			return averageTemp / (float)cellCount;
		}
		return -1f;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_TEMPERATURE, GameUtil.GetFormattedTemperature(internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, displayUnits: false), GameUtil.GetFormattedTemperature(internalTemperatureWarning_High)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_TEMPERATURE, GameUtil.GetFormattedTemperature(internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, displayUnits: false), GameUtil.GetFormattedTemperature(internalTemperatureWarning_High)), Descriptor.DescriptorType.Requirement)
		};
	}
}
