using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicCounter : Switch, ISaveLoadable
{
	[Serialize]
	public int maxCount;

	[Serialize]
	public int currentCount;

	[Serialize]
	public bool resetCountAtMax;

	[Serialize]
	public bool advancedMode;

	private bool wasOn;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicCounter> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicCounter>(delegate(LogicCounter component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicCounter> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicCounter>(delegate(LogicCounter component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	public static readonly HashedString INPUT_PORT_ID = new HashedString("LogicCounterInput");

	public static readonly HashedString RESET_PORT_ID = new HashedString("LogicCounterReset");

	public static readonly HashedString OUTPUT_PORT_ID = new HashedString("LogicCounterOutput");

	private bool resetRequested;

	[Serialize]
	private bool wasResetting;

	[Serialize]
	private bool wasIncrementing;

	[Serialize]
	public bool receivedFirstSignal;

	private bool pulsingActive;

	private const int pulseLength = 1;

	private int pulseTicksRemaining;

	private MeterController meter;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		LogicCounter component = ((GameObject)data).GetComponent<LogicCounter>();
		if (component != null)
		{
			maxCount = component.maxCount;
			resetCountAtMax = component.resetCountAtMax;
			advancedMode = component.advancedMode;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += OnSwitchToggled;
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		logicCircuitManager.onLogicTick = (System.Action)Delegate.Combine(logicCircuitManager.onLogicTick, new System.Action(LogicTick));
		if (maxCount == 0)
		{
			maxCount = 10;
		}
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		UpdateLogicCircuit();
		UpdateVisualState(force: true);
		wasOn = switchedOn;
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		meter = new MeterController((KAnimControllerBase)component, "meter_target", component.FlipY ? "meter_dn" : "meter_up", Meter.Offset.UserSpecified, Grid.SceneLayer.LogicGatesFront, Vector3.zero, (string[])null);
		UpdateMeter();
	}

	protected override void OnCleanUp()
	{
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		logicCircuitManager.onLogicTick = (System.Action)Delegate.Remove(logicCircuitManager.onLogicTick, new System.Action(LogicTick));
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState();
	}

	public void UpdateLogicCircuit()
	{
		if (receivedFirstSignal)
		{
			GetComponent<LogicPorts>().SendSignal(OUTPUT_PORT_ID, switchedOn ? 1 : 0);
		}
	}

	public void UpdateMeter()
	{
		float num = (advancedMode ? (currentCount % maxCount) : currentCount);
		if (num == 10f)
		{
			num = 0f;
		}
		meter.SetPositionPercent(num / 10f);
	}

	public void UpdateVisualState(bool force = false)
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if (!receivedFirstSignal)
		{
			component.Play("off");
		}
		else if (wasOn != switchedOn || force)
		{
			int num = (switchedOn ? 4 : 0) + (wasResetting ? 2 : 0) + (wasIncrementing ? 1 : 0);
			wasOn = switchedOn;
			component.Play("on_" + num);
		}
	}

	public void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == INPUT_PORT_ID)
		{
			int newValue = logicValueChanged.newValue;
			receivedFirstSignal = true;
			if (LogicCircuitNetwork.IsBitActive(0, newValue))
			{
				if (!wasIncrementing)
				{
					wasIncrementing = true;
					if (!wasResetting)
					{
						if (currentCount == maxCount || currentCount >= 10)
						{
							currentCount = 0;
						}
						currentCount++;
						UpdateMeter();
						SetCounterState();
						if (currentCount == maxCount && resetCountAtMax)
						{
							resetRequested = true;
						}
					}
				}
			}
			else
			{
				wasIncrementing = false;
			}
		}
		else
		{
			if (!(logicValueChanged.portID == RESET_PORT_ID))
			{
				return;
			}
			int newValue2 = logicValueChanged.newValue;
			receivedFirstSignal = true;
			if (LogicCircuitNetwork.IsBitActive(0, newValue2))
			{
				if (!wasResetting)
				{
					wasResetting = true;
					ResetCounter();
				}
			}
			else
			{
				wasResetting = false;
			}
		}
		UpdateVisualState(force: true);
		UpdateLogicCircuit();
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
	}

	public void ResetCounter()
	{
		resetRequested = false;
		currentCount = 0;
		SetCounterState();
		UpdateVisualState(force: true);
		UpdateMeter();
		UpdateLogicCircuit();
	}

	public void LogicTick()
	{
		if (resetRequested)
		{
			ResetCounter();
		}
		if (pulsingActive)
		{
			pulseTicksRemaining--;
			if (pulseTicksRemaining <= 0)
			{
				pulsingActive = false;
				SetState(on: false);
				UpdateVisualState();
				UpdateMeter();
				UpdateLogicCircuit();
			}
		}
	}

	public void SetCounterState()
	{
		SetState(advancedMode ? (currentCount % maxCount == 0) : (currentCount == maxCount));
		if (advancedMode && currentCount % maxCount == 0)
		{
			pulsingActive = true;
			pulseTicksRemaining = 2;
		}
	}
}
