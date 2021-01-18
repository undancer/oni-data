using System;
using System.Diagnostics;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name}")]
[AddComponentMenu("KMonoBehaviour/scripts/Generator")]
public class Generator : KMonoBehaviour, ISaveLoadable, IEnergyProducer
{
	protected const int SimUpdateSortKey = 1001;

	[MyCmpReq]
	protected Building building;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpReq]
	protected KSelectable selectable;

	[Serialize]
	private float joulesAvailable;

	[SerializeField]
	public int powerDistributionOrder;

	public static readonly Operational.Flag generatorConnectedFlag = new Operational.Flag("GeneratorConnected", Operational.Flag.Type.Requirement);

	protected static readonly Operational.Flag wireConnectedFlag = new Operational.Flag("generatorWireConnected", Operational.Flag.Type.Requirement);

	private float capacity;

	private StatusItem currentStatusItem;

	private Guid statusItemID;

	private AttributeInstance generatorOutputAttribute;

	private static readonly EventSystem.IntraObjectHandler<Generator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Generator>(delegate(Generator component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public int PowerDistributionOrder => powerDistributionOrder;

	public virtual float Capacity => capacity;

	public virtual float BaseCapacity => capacity;

	public virtual bool IsEmpty => joulesAvailable <= 0f;

	public virtual float JoulesAvailable => joulesAvailable;

	public float WattageRating => building.Def.GeneratorWattageRating * Efficiency;

	public float BaseWattageRating => building.Def.GeneratorWattageRating;

	public float PercentFull
	{
		get
		{
			if (Capacity == 0f)
			{
				return 1f;
			}
			return joulesAvailable / Capacity;
		}
	}

	public bool HasWire
	{
		get
		{
			bool result = false;
			GameObject gameObject = Grid.Objects[PowerCell, 26];
			if (gameObject != null && gameObject.GetComponent<BuildingComplete>() != null)
			{
				result = true;
			}
			return result;
		}
	}

	public int PowerCell
	{
		get;
		private set;
	}

	public ushort CircuitID => Game.Instance.circuitManager.GetCircuitID(PowerCell);

	private float Efficiency => Mathf.Max(1f + generatorOutputAttribute.GetTotalValue() / 100f, 0.1f);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Attributes attributes = base.gameObject.GetAttributes();
		generatorOutputAttribute = attributes.Add(Db.Get().Attributes.GeneratorOutput);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Generators.Add(this);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		capacity = CalculateCapacity(building.Def, null);
		PowerCell = building.GetPowerOutputCell();
		CheckConnectionStatus();
		OnOperationalChanged(null);
		Game.Instance.energySim.AddGenerator(this);
	}

	public virtual void EnergySim200ms(float dt)
	{
		CheckConnectionStatus();
	}

	private void SetStatusItem(StatusItem status_item)
	{
		if (status_item != currentStatusItem && currentStatusItem != null)
		{
			statusItemID = selectable.RemoveStatusItem(statusItemID);
		}
		if (status_item != null && statusItemID == Guid.Empty)
		{
			statusItemID = selectable.AddStatusItem(status_item, this);
		}
		currentStatusItem = status_item;
	}

	private void CheckConnectionStatus()
	{
		if (CircuitID == ushort.MaxValue)
		{
			if (HasWire)
			{
				SetStatusItem(Db.Get().BuildingStatusItems.NoPowerConsumers);
				operational.SetFlag(generatorConnectedFlag, value: true);
			}
			else
			{
				SetStatusItem(Db.Get().BuildingStatusItems.NoWireConnected);
				operational.SetFlag(generatorConnectedFlag, value: false);
			}
		}
		else
		{
			SetStatusItem(null);
			operational.SetFlag(generatorConnectedFlag, value: true);
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.energySim.RemoveGenerator(this);
		Game.Instance.circuitManager.Disconnect(this);
		Components.Generators.Remove(this);
		base.OnCleanUp();
	}

	public static float CalculateCapacity(BuildingDef def, Element element)
	{
		if (element == null)
		{
			return def.GeneratorBaseCapacity;
		}
		return def.GeneratorBaseCapacity * (1f + (element.HasTag(GameTags.RefinedMetal) ? 1f : 0f));
	}

	public void ResetJoules()
	{
		joulesAvailable = 0f;
	}

	public virtual void ApplyDeltaJoules(float joulesDelta, bool canOverPower = false)
	{
		joulesAvailable = Mathf.Clamp(joulesAvailable + joulesDelta, 0f, canOverPower ? float.MaxValue : Capacity);
	}

	public void GenerateJoules(float joulesAvailable, bool canOverPower = false)
	{
		Debug.Assert(GetComponent<Battery>() == null);
		this.joulesAvailable = Mathf.Clamp(joulesAvailable, 0f, canOverPower ? float.MaxValue : Capacity);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, this.joulesAvailable, this.GetProperName());
		if (!Game.Instance.savedInfo.powerCreatedbyGeneratorType.ContainsKey(this.PrefabID()))
		{
			Game.Instance.savedInfo.powerCreatedbyGeneratorType.Add(this.PrefabID(), 0f);
		}
		Game.Instance.savedInfo.powerCreatedbyGeneratorType[this.PrefabID()] += this.joulesAvailable;
	}

	public void AssignJoulesAvailable(float joulesAvailable)
	{
		Debug.Assert(GetComponent<PowerTransformer>() != null);
		this.joulesAvailable = joulesAvailable;
	}

	private void OnOperationalChanged(object data)
	{
		if (operational.IsOperational)
		{
			Game.Instance.circuitManager.Connect(this);
		}
		else
		{
			Game.Instance.circuitManager.Disconnect(this);
		}
	}

	public virtual void ConsumeEnergy(float joules)
	{
		joulesAvailable = Mathf.Max(0f, JoulesAvailable - joules);
	}
}
