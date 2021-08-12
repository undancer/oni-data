using System.Collections.Generic;
using System.Diagnostics;
using FMOD.Studio;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name} {WattsUsed}W")]
[AddComponentMenu("KMonoBehaviour/scripts/EnergyConsumer")]
public class EnergyConsumer : KMonoBehaviour, ISaveLoadable, IEnergyConsumer, ICircuitConnected, IGameObjectEffectDescriptor
{
	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	protected Operational operational;

	[MyCmpGet]
	private KSelectable selectable;

	[SerializeField]
	public int powerSortOrder;

	[Serialize]
	protected float circuitOverloadTime;

	public static readonly Operational.Flag PoweredFlag = new Operational.Flag("powered", Operational.Flag.Type.Requirement);

	private Dictionary<string, float> lastTimeSoundPlayed = new Dictionary<string, float>();

	private float soundDecayTime = 10f;

	private float _BaseWattageRating;

	public int PowerSortOrder => powerSortOrder;

	public int PowerCell { get; private set; }

	public bool HasWire => Grid.Objects[PowerCell, 26] != null;

	public virtual bool IsPowered
	{
		get
		{
			return operational.GetFlag(PoweredFlag);
		}
		private set
		{
			operational.SetFlag(PoweredFlag, value);
		}
	}

	public bool IsConnected => CircuitID != ushort.MaxValue;

	public string Name => selectable.GetName();

	public bool IsVirtual { get; private set; }

	public object VirtualCircuitKey { get; private set; }

	public ushort CircuitID { get; private set; }

	public float BaseWattageRating
	{
		get
		{
			return _BaseWattageRating;
		}
		set
		{
			_BaseWattageRating = value;
		}
	}

	public float WattsUsed
	{
		get
		{
			if (operational.IsActive)
			{
				return BaseWattageRating;
			}
			return 0f;
		}
	}

	public float WattsNeededWhenActive => building.Def.EnergyConsumptionWhenActive;

	protected override void OnPrefabInit()
	{
		CircuitID = ushort.MaxValue;
		IsPowered = false;
		BaseWattageRating = building.Def.EnergyConsumptionWhenActive;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.EnergyConsumers.Add(this);
		Building component = GetComponent<Building>();
		PowerCell = component.GetPowerInputCell();
		Game.Instance.circuitManager.Connect(this);
		Game.Instance.energySim.AddEnergyConsumer(this);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.energySim.RemoveEnergyConsumer(this);
		Game.Instance.circuitManager.Disconnect(this, isDestroy: true);
		Components.EnergyConsumers.Remove(this);
		base.OnCleanUp();
	}

	public virtual void EnergySim200ms(float dt)
	{
		CircuitID = Game.Instance.circuitManager.GetCircuitID(this);
		if (!IsConnected)
		{
			IsPowered = false;
		}
		circuitOverloadTime = Mathf.Max(0f, circuitOverloadTime - dt);
	}

	public virtual void SetConnectionStatus(CircuitManager.ConnectionStatus connection_status)
	{
		switch (connection_status)
		{
		case CircuitManager.ConnectionStatus.NotConnected:
			IsPowered = false;
			break;
		case CircuitManager.ConnectionStatus.Unpowered:
			if (IsPowered && GetComponent<Battery>() == null)
			{
				IsPowered = false;
				circuitOverloadTime = 6f;
				PlayCircuitSound("overdraw");
			}
			break;
		case CircuitManager.ConnectionStatus.Powered:
			if (!IsPowered && circuitOverloadTime <= 0f)
			{
				IsPowered = true;
				PlayCircuitSound("powered");
			}
			break;
		}
	}

	protected void PlayCircuitSound(string state)
	{
		string sound = null;
		if (state == "powered")
		{
			sound = Sounds.Instance.BuildingPowerOnMigrated;
		}
		else if (state == "overdraw")
		{
			sound = Sounds.Instance.ElectricGridOverloadMigrated;
		}
		else
		{
			Debug.Log("Invalid state for sound in EnergyConsumer.");
		}
		if (CameraController.Instance.IsAudibleSound(base.transform.GetPosition()))
		{
			if (!lastTimeSoundPlayed.TryGetValue(state, out var value))
			{
				value = 0f;
			}
			float value2 = (Time.time - value) / soundDecayTime;
			Vector3 position = base.transform.GetPosition();
			position.z = 0f;
			FMOD.Studio.EventInstance instance = KFMOD.BeginOneShot(sound, CameraController.Instance.GetVerticallyScaledPosition(position));
			instance.setParameterByName("timeSinceLast", value2);
			KFMOD.EndOneShot(instance);
			lastTimeSoundPlayed[state] = Time.time;
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return null;
	}
}
