using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/ManualDeliveryKG")]
public class ManualDeliveryKG : KMonoBehaviour, ISim1000ms
{
	[MyCmpGet]
	private Operational operational;

	[SerializeField]
	private Storage storage;

	[SerializeField]
	public Tag requestedItemTag;

	private Tag[] forbiddenTags;

	[SerializeField]
	public float capacity = 100f;

	[SerializeField]
	public float refillMass = 10f;

	[SerializeField]
	public float minimumMass = 10f;

	[SerializeField]
	public FetchOrder2.OperationalRequirement operationalRequirement;

	[SerializeField]
	public bool allowPause;

	[SerializeField]
	private bool paused;

	[SerializeField]
	public HashedString choreTypeIDHash;

	[Serialize]
	private bool userPaused;

	public bool ShowStatusItem = true;

	private FetchList2 fetchList;

	private int onStorageChangeSubscription = -1;

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnStorageChanged(data);
	});

	public bool IsPaused => paused;

	public float Capacity => capacity;

	public Tag RequestedItemTag
	{
		get
		{
			return requestedItemTag;
		}
		set
		{
			requestedItemTag = value;
			AbortDelivery("Requested Item Tag Changed");
		}
	}

	public Tag[] ForbiddenTags
	{
		get
		{
			return forbiddenTags;
		}
		set
		{
			forbiddenTags = value;
			AbortDelivery("Forbidden Tags Changed");
		}
	}

	public Storage DebugStorage => storage;

	public FetchList2 DebugFetchList => fetchList;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		DebugUtil.Assert(choreTypeIDHash.IsValid, "ManualDeliveryKG Must have a valid chore type specified!", base.name);
		if (allowPause)
		{
			Subscribe(493375141, OnRefreshUserMenuDelegate);
			Subscribe(-111137758, OnRefreshUserMenuDelegate);
		}
		Subscribe(-592767678, OnOperationalChangedDelegate);
		if (storage != null)
		{
			SetStorage(storage);
		}
		Prioritizable.AddRef(base.gameObject);
		if (userPaused && allowPause)
		{
			OnPause();
		}
	}

	protected override void OnCleanUp()
	{
		AbortDelivery("ManualDeliverKG destroyed");
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCleanUp();
	}

	public void SetStorage(Storage storage)
	{
		if (this.storage != null)
		{
			this.storage.Unsubscribe(onStorageChangeSubscription);
			onStorageChangeSubscription = -1;
		}
		AbortDelivery("storage pointer changed");
		this.storage = storage;
		if (this.storage != null && base.isSpawned)
		{
			Debug.Assert(onStorageChangeSubscription == -1);
			onStorageChangeSubscription = this.storage.Subscribe(-1697596308, OnStorageChangedDelegate);
		}
	}

	public void Pause(bool pause, string reason)
	{
		if (paused != pause)
		{
			paused = pause;
			if (pause)
			{
				AbortDelivery(reason);
			}
		}
	}

	public void Sim1000ms(float dt)
	{
		UpdateDeliveryState();
	}

	[ContextMenu("UpdateDeliveryState")]
	public void UpdateDeliveryState()
	{
		if (requestedItemTag.IsValid && !(storage == null))
		{
			UpdateFetchList();
		}
	}

	public void RequestDelivery()
	{
		if (this.fetchList == null)
		{
			float massAvailable = storage.GetMassAvailable(requestedItemTag);
			if (massAvailable < capacity)
			{
				float b = capacity - massAvailable;
				b = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, b);
				ChoreType byHash = Db.Get().ChoreTypes.GetByHash(choreTypeIDHash);
				this.fetchList = new FetchList2(storage, byHash);
				this.fetchList.ShowStatusItem = ShowStatusItem;
				this.fetchList.MinimumAmount[requestedItemTag] = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, minimumMass);
				FetchList2 fetchList = this.fetchList;
				Tag[] tags = new Tag[1] { requestedItemTag };
				float amount = b;
				fetchList.Add(tags, null, forbiddenTags, amount);
				this.fetchList.Submit(null, check_storage_contents: false);
			}
		}
	}

	private void UpdateFetchList()
	{
		if (paused)
		{
			return;
		}
		if (fetchList != null && fetchList.IsComplete)
		{
			fetchList = null;
		}
		if (!OperationalRequirementsMet())
		{
			if (fetchList != null)
			{
				fetchList.Cancel("Operational requirements");
				fetchList = null;
			}
		}
		else if (fetchList == null && storage.GetMassAvailable(requestedItemTag) < refillMass)
		{
			RequestDelivery();
		}
	}

	private bool OperationalRequirementsMet()
	{
		if ((bool)operational)
		{
			if (operationalRequirement == FetchOrder2.OperationalRequirement.Operational)
			{
				return operational.IsOperational;
			}
			if (operationalRequirement == FetchOrder2.OperationalRequirement.Functional)
			{
				return operational.IsFunctional;
			}
		}
		return true;
	}

	public void AbortDelivery(string reason)
	{
		if (this.fetchList != null)
		{
			FetchList2 fetchList = this.fetchList;
			this.fetchList = null;
			fetchList.Cancel(reason);
		}
	}

	protected void OnStorageChanged(object data)
	{
		UpdateDeliveryState();
	}

	private void OnPause()
	{
		userPaused = true;
		Pause(pause: true, "Forbid manual delivery");
	}

	private void OnResume()
	{
		userPaused = false;
		Pause(pause: false, "Allow manual delivery");
	}

	private void OnRefreshUserMenu(object data)
	{
		if (allowPause)
		{
			KIconButtonMenu.ButtonInfo button = ((!paused) ? new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME, OnPause, Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME_OFF, OnResume, Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP_OFF));
			Game.Instance.userMenu.AddButton(base.gameObject, button);
		}
	}

	private void OnOperationalChanged(object data)
	{
		UpdateDeliveryState();
	}
}
