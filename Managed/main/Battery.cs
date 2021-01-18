using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name}")]
[AddComponentMenu("KMonoBehaviour/scripts/Battery")]
public class Battery : KMonoBehaviour, IEnergyConsumer, IGameObjectEffectDescriptor, IEnergyProducer
{
	[SerializeField]
	public float capacity;

	[SerializeField]
	public float chargeWattage = float.PositiveInfinity;

	[Serialize]
	private float joulesAvailable;

	[MyCmpGet]
	protected Operational operational;

	[MyCmpGet]
	public PowerTransformer powerTransformer;

	private MeterController meter;

	public float joulesLostPerSecond = 0f;

	[SerializeField]
	public int powerSortOrder;

	private float PreviousJoulesAvailable;

	private CircuitManager.ConnectionStatus connectionStatus;

	private static readonly EventSystem.IntraObjectHandler<Battery> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Battery>(delegate(Battery component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private float dt;

	private float joulesConsumed = 0f;

	public float WattsUsed
	{
		get;
		private set;
	}

	public float WattsNeededWhenActive => 0f;

	public float PercentFull => joulesAvailable / capacity;

	public float PreviousPercentFull => PreviousJoulesAvailable / capacity;

	public float JoulesAvailable => joulesAvailable;

	public float Capacity => capacity;

	public float ChargeCapacity
	{
		get;
		private set;
	}

	public int PowerSortOrder => powerSortOrder;

	public string Name => GetComponent<KSelectable>().GetName();

	public int PowerCell
	{
		get;
		private set;
	}

	public ushort CircuitID => Game.Instance.circuitManager.GetCircuitID(PowerCell);

	public bool IsConnected
	{
		get
		{
			GameObject x = Grid.Objects[PowerCell, 26];
			return x != null;
		}
	}

	public bool IsPowered => connectionStatus == CircuitManager.ConnectionStatus.Powered;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Batteries.Add(this);
		Building component = GetComponent<Building>();
		PowerCell = component.GetPowerInputCell();
		Subscribe(-592767678, OnOperationalChangedDelegate);
		OnOperationalChanged(null);
		bool flag = GetComponent<PowerTransformer>();
		meter = (flag ? null : new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL"));
		Game.Instance.circuitManager.Connect(this);
		Game.Instance.energySim.AddBattery(this);
	}

	private void OnOperationalChanged(object data)
	{
		if (operational.IsOperational)
		{
			Game.Instance.circuitManager.Connect(this);
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.JoulesAvailable, this);
		}
		else
		{
			Game.Instance.circuitManager.Disconnect(this);
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.JoulesAvailable);
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.energySim.RemoveBattery(this);
		Game.Instance.circuitManager.Disconnect(this);
		Components.Batteries.Remove(this);
		base.OnCleanUp();
	}

	public virtual void EnergySim200ms(float dt)
	{
		this.dt = dt;
		joulesConsumed = 0f;
		WattsUsed = 0f;
		ChargeCapacity = chargeWattage * dt;
		if (meter != null)
		{
			float percentFull = PercentFull;
			meter.SetPositionPercent(percentFull);
		}
		UpdateSounds();
		PreviousJoulesAvailable = JoulesAvailable;
		ConsumeEnergy(joulesLostPerSecond * dt, report: true);
	}

	private void UpdateSounds()
	{
		float previousPercentFull = PreviousPercentFull;
		float percentFull = PercentFull;
		if (percentFull == 0f && previousPercentFull != 0f)
		{
			GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryDischarged);
		}
		if (percentFull > 0.999f && previousPercentFull <= 0.999f)
		{
			GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryFull);
		}
		if (percentFull < 0.25f && previousPercentFull >= 0.25f)
		{
			GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryWarning);
		}
	}

	public void SetConnectionStatus(CircuitManager.ConnectionStatus status)
	{
		connectionStatus = status;
		if (status == CircuitManager.ConnectionStatus.NotConnected)
		{
			operational.SetActive(value: false);
		}
		else
		{
			operational.SetActive(operational.IsOperational && JoulesAvailable > 0f);
		}
	}

	public void AddEnergy(float joules)
	{
		joulesAvailable = Mathf.Min(capacity, JoulesAvailable + joules);
		joulesConsumed += joules;
		ChargeCapacity -= joules;
		WattsUsed = joulesConsumed / dt;
	}

	public void ConsumeEnergy(float joules, bool report = false)
	{
		if (report)
		{
			float num = Mathf.Min(JoulesAvailable, joules);
			ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, 0f - num, StringFormatter.Replace(BUILDINGS.PREFABS.BATTERY.CHARGE_LOSS, "{Battery}", this.GetProperName()));
		}
		joulesAvailable = Mathf.Max(0f, JoulesAvailable - joules);
	}

	public void ConsumeEnergy(float joules)
	{
		ConsumeEnergy(joules, report: false);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (powerTransformer == null)
		{
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.REQUIRESPOWERGENERATOR, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESPOWERGENERATOR, Descriptor.DescriptorType.Requirement));
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.BATTERYCAPACITY, GameUtil.GetFormattedJoules(capacity, "")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.BATTERYCAPACITY, GameUtil.GetFormattedJoules(capacity, ""))));
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.BATTERYLEAK, GameUtil.GetFormattedJoules(joulesLostPerSecond, "F1", GameUtil.TimeSlice.PerCycle)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.BATTERYLEAK, GameUtil.GetFormattedJoules(joulesLostPerSecond, "F1", GameUtil.TimeSlice.PerCycle))));
		}
		else
		{
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.TRANSFORMER_INPUT_WIRE, UI.BUILDINGEFFECTS.TOOLTIPS.TRANSFORMER_INPUT_WIRE, Descriptor.DescriptorType.Requirement));
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.TRANSFORMER_OUTPUT_WIRE, GameUtil.GetFormattedWattage(capacity)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.TRANSFORMER_OUTPUT_WIRE, GameUtil.GetFormattedWattage(capacity)), Descriptor.DescriptorType.Requirement));
		}
		return list;
	}

	[ContextMenu("Refill Power")]
	public void DEBUG_RefillPower()
	{
		joulesAvailable = capacity;
	}
}
