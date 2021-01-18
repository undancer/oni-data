using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/StorageLocker")]
public class StorageLocker : KMonoBehaviour, IUserControlledCapacity
{
	private LoggerFS log;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	[Serialize]
	public string lockerName = "";

	protected FilteredStorage filteredStorage;

	[MyCmpGet]
	private UserNameable nameable;

	public string choreTypeID = Db.Get().ChoreTypes.StorageFetch.Id;

	private static readonly EventSystem.IntraObjectHandler<StorageLocker> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<StorageLocker>(delegate(StorageLocker component, object data)
	{
		component.OnCopySettings(data);
	});

	public virtual float UserMaxCapacity
	{
		get
		{
			return Mathf.Min(userMaxCapacity, GetComponent<Storage>().capacityKg);
		}
		set
		{
			userMaxCapacity = value;
			filteredStorage.FilterChanged();
		}
	}

	public float AmountStored => GetComponent<Storage>().MassStored();

	public float MinCapacity => 0f;

	public float MaxCapacity => GetComponent<Storage>().capacityKg;

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

	protected override void OnPrefabInit()
	{
		Initialize(use_logic_meter: false);
	}

	protected void Initialize(bool use_logic_meter)
	{
		base.OnPrefabInit();
		log = new LoggerFS("StorageLocker");
		ChoreType fetch_chore_type = Db.Get().ChoreTypes.Get(choreTypeID);
		filteredStorage = new FilteredStorage(this, null, null, this, use_logic_meter, fetch_chore_type);
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		filteredStorage.FilterChanged();
		if (nameable != null && !lockerName.IsNullOrWhiteSpace())
		{
			nameable.SetName(lockerName);
		}
	}

	protected override void OnCleanUp()
	{
		filteredStorage.CleanUp();
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (!(gameObject == null))
		{
			StorageLocker component = gameObject.GetComponent<StorageLocker>();
			if (!(component == null))
			{
				UserMaxCapacity = component.UserMaxCapacity;
			}
		}
	}
}
