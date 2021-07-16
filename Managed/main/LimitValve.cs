using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LimitValve : KMonoBehaviour, ISaveLoadable
{
	public static readonly HashedString RESET_PORT_ID = new HashedString("LimitValveReset");

	public static readonly HashedString OUTPUT_PORT_ID = new HashedString("LimitValveOutput");

	public static readonly Operational.Flag limitNotReached = new Operational.Flag("limitNotReached", Operational.Flag.Type.Requirement);

	public ConduitType conduitType;

	public float maxLimitKg = 100f;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private LogicPorts ports;

	[MyCmpGet]
	private KBatchedAnimController controller;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private ConduitBridge conduitBridge;

	[MyCmpGet]
	private SolidConduitBridge solidConduitBridge;

	[Serialize]
	[SerializeField]
	private float m_limit;

	[Serialize]
	private float m_amount;

	[Serialize]
	private bool m_resetRequested;

	private MeterController limitMeter;

	public bool displayUnitsInsteadOfMass;

	public NonLinearSlider.Range[] sliderRanges;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LimitValve> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LimitValve>(delegate(LimitValve component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LimitValve> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LimitValve>(delegate(LimitValve component, object data)
	{
		component.OnCopySettings(data);
	});

	public float RemainingCapacity => Mathf.Max(0f, m_limit - m_amount);

	public float Limit
	{
		get
		{
			return m_limit;
		}
		set
		{
			m_limit = value;
			Refresh();
		}
	}

	public float Amount
	{
		get
		{
			return m_amount;
		}
		set
		{
			m_amount = value;
			Trigger(-1722241721, Amount);
			Refresh();
		}
	}

	public NonLinearSlider.Range[] GetRanges()
	{
		if (sliderRanges != null && sliderRanges.Length != 0)
		{
			return sliderRanges;
		}
		return NonLinearSlider.GetDefaultRange(maxLimitKg);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		logicCircuitManager.onLogicTick = (System.Action)Delegate.Combine(logicCircuitManager.onLogicTick, new System.Action(LogicTick));
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		if (conduitType == ConduitType.Gas || conduitType == ConduitType.Liquid)
		{
			ConduitBridge obj = conduitBridge;
			obj.desiredMassTransfer = (ConduitBridgeBase.DesiredMassTransfer)Delegate.Combine(obj.desiredMassTransfer, new ConduitBridgeBase.DesiredMassTransfer(DesiredMassTransfer));
			ConduitBridge obj2 = conduitBridge;
			obj2.OnMassTransfer = (ConduitBridgeBase.ConduitBridgeEvent)Delegate.Combine(obj2.OnMassTransfer, new ConduitBridgeBase.ConduitBridgeEvent(OnMassTransfer));
		}
		else if (conduitType == ConduitType.Solid)
		{
			SolidConduitBridge obj3 = solidConduitBridge;
			obj3.desiredMassTransfer = (ConduitBridgeBase.DesiredMassTransfer)Delegate.Combine(obj3.desiredMassTransfer, new ConduitBridgeBase.DesiredMassTransfer(DesiredMassTransfer));
			SolidConduitBridge obj4 = solidConduitBridge;
			obj4.OnMassTransfer = (ConduitBridgeBase.ConduitBridgeEvent)Delegate.Combine(obj4.OnMassTransfer, new ConduitBridgeBase.ConduitBridgeEvent(OnMassTransfer));
		}
		if (limitMeter == null)
		{
			limitMeter = new MeterController(controller, "meter_target_counter", "meter_counter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target_counter");
		}
		Refresh();
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		logicCircuitManager.onLogicTick = (System.Action)Delegate.Remove(logicCircuitManager.onLogicTick, new System.Action(LogicTick));
		base.OnCleanUp();
	}

	private void LogicTick()
	{
		if (m_resetRequested)
		{
			ResetAmount();
		}
	}

	public void ResetAmount()
	{
		m_resetRequested = false;
		Amount = 0f;
	}

	private float DesiredMassTransfer(float dt, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, Pickupable pickupable)
	{
		if (!operational.IsOperational)
		{
			return 0f;
		}
		if (conduitType == ConduitType.Solid && pickupable != null && GameTags.DisplayAsUnits.Contains(pickupable.PrefabID()))
		{
			float num = pickupable.PrimaryElement.Units;
			if (RemainingCapacity < num)
			{
				num = Mathf.FloorToInt(RemainingCapacity);
			}
			return num * pickupable.PrimaryElement.MassPerUnit;
		}
		return Mathf.Min(mass, RemainingCapacity);
	}

	private void OnMassTransfer(SimHashes element, float transferredMass, float temperature, byte disease_idx, int disease_count, Pickupable pickupable)
	{
		if (!LogicCircuitNetwork.IsBitActive(0, ports.GetInputValue(RESET_PORT_ID)))
		{
			if (conduitType == ConduitType.Gas || conduitType == ConduitType.Liquid)
			{
				Amount += transferredMass;
			}
			else if (conduitType == ConduitType.Solid && pickupable != null)
			{
				Amount += transferredMass / pickupable.PrimaryElement.MassPerUnit;
			}
		}
		operational.SetActive(operational.IsOperational && transferredMass > 0f);
		Refresh();
	}

	private void Refresh()
	{
		if (!(operational == null))
		{
			ports.SendSignal(OUTPUT_PORT_ID, (RemainingCapacity <= 0f) ? 1 : 0);
			operational.SetFlag(limitNotReached, RemainingCapacity > 0f);
			if (RemainingCapacity > 0f)
			{
				limitMeter.meterController.Play("meter_counter", KAnim.PlayMode.Paused);
				limitMeter.SetPositionPercent(Amount / Limit);
				selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.LimitValveLimitNotReached, this);
			}
			else
			{
				limitMeter.meterController.Play("meter_on", KAnim.PlayMode.Paused);
				selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.LimitValveLimitReached, this);
			}
		}
	}

	public void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == RESET_PORT_ID && LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue))
		{
			ResetAmount();
		}
	}

	private void OnCopySettings(object data)
	{
		LimitValve component = ((GameObject)data).GetComponent<LimitValve>();
		if (component != null)
		{
			Limit = component.Limit;
		}
	}
}
