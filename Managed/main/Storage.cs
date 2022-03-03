using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Storage")]
public class Storage : Workable, ISaveLoadableDetails, IGameObjectEffectDescriptor, IStorage
{
	public enum StoredItemModifier
	{
		Insulate,
		Hide,
		Seal,
		Preserve
	}

	public enum FetchCategory
	{
		Building,
		GeneralStorage,
		StorageSweepOnly
	}

	public enum FXPrefix
	{
		Delivered,
		PickedUp
	}

	private struct StoredItemModifierInfo
	{
		public StoredItemModifier modifier;

		public Action<GameObject, bool, bool> toggleState;

		public StoredItemModifierInfo(StoredItemModifier modifier, Action<GameObject, bool, bool> toggle_state)
		{
			this.modifier = modifier;
			toggleState = toggle_state;
		}
	}

	public bool ignoreSourcePriority;

	public bool allowItemRemoval;

	public bool onlyTransferFromLowerPriority;

	public float capacityKg = 20000f;

	public bool showDescriptor;

	public bool doDiseaseTransfer = true;

	public List<Tag> storageFilters;

	public bool useGunForDelivery = true;

	public bool sendOnStoreOnSpawn;

	public bool showInUI = true;

	public bool allowClearable;

	public bool showCapacityStatusItem;

	public bool showCapacityAsMainStatus;

	public bool showUnreachableStatus;

	public bool useWideOffsets;

	[MyCmpGet]
	private Rotatable rotatable;

	public Vector2 gunTargetOffset;

	public FetchCategory fetchCategory;

	public int storageNetworkID = -1;

	public float storageFullMargin;

	public Vector3 storageFXOffset = Vector3.zero;

	private static readonly EventSystem.IntraObjectHandler<Storage> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Storage>(delegate(Storage component, object data)
	{
		component.OnReachableChanged(data);
	});

	public FXPrefix fxPrefix;

	public List<GameObject> items = new List<GameObject>();

	[MyCmpGet]
	public Prioritizable prioritizable;

	[MyCmpGet]
	public Automatable automatable;

	[MyCmpGet]
	protected PrimaryElement primaryElement;

	public bool dropOnLoad;

	protected float maxKGPerItem = float.MaxValue;

	private bool endOfLife;

	public bool allowSettingOnlyFetchMarkedItems = true;

	[Serialize]
	private bool onlyFetchMarkedItems;

	public float storageWorkTime = 1.5f;

	private static readonly List<StoredItemModifierInfo> StoredItemModifierHandlers = new List<StoredItemModifierInfo>
	{
		new StoredItemModifierInfo(StoredItemModifier.Hide, MakeItemInvisible),
		new StoredItemModifierInfo(StoredItemModifier.Insulate, MakeItemTemperatureInsulated),
		new StoredItemModifierInfo(StoredItemModifier.Seal, MakeItemSealed),
		new StoredItemModifierInfo(StoredItemModifier.Preserve, MakeItemPreserved)
	};

	[SerializeField]
	private List<StoredItemModifier> defaultStoredItemModifers = new List<StoredItemModifier> { StoredItemModifier.Hide };

	public static readonly List<StoredItemModifier> StandardSealedStorage = new List<StoredItemModifier>
	{
		StoredItemModifier.Hide,
		StoredItemModifier.Seal
	};

	public static readonly List<StoredItemModifier> StandardFabricatorStorage = new List<StoredItemModifier>
	{
		StoredItemModifier.Hide,
		StoredItemModifier.Preserve
	};

	public static readonly List<StoredItemModifier> StandardInsulatedStorage = new List<StoredItemModifier>
	{
		StoredItemModifier.Hide,
		StoredItemModifier.Seal,
		StoredItemModifier.Insulate
	};

	private static StatusItem capacityStatusItem;

	private static readonly EventSystem.IntraObjectHandler<Storage> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler(GameTags.Dead, delegate(Storage component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Storage> OnQueueDestroyObjectDelegate = new EventSystem.IntraObjectHandler<Storage>(delegate(Storage component, object data)
	{
		component.OnQueueDestroyObject(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Storage> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Storage>(delegate(Storage component, object data)
	{
		component.OnCopySettings(data);
	});

	private List<GameObject> deleted_objects;

	public bool ShouldOnlyTransferFromLowerPriority
	{
		get
		{
			if (!onlyTransferFromLowerPriority)
			{
				return allowItemRemoval;
			}
			return true;
		}
	}

	public bool allowUIItemRemoval { get; set; }

	public GameObject this[int idx] => items[idx];

	public int Count => items.Count;

	public PrioritySetting masterPriority
	{
		get
		{
			if ((bool)prioritizable)
			{
				return prioritizable.GetMasterPriority();
			}
			return Chore.DefaultPrioritySetting;
		}
	}

	public event System.Action OnStorageIncreased;

	public bool ShouldShowInUI()
	{
		return showInUI;
	}

	public List<GameObject> GetItems()
	{
		return items;
	}

	public void SetDefaultStoredItemModifiers(List<StoredItemModifier> modifiers)
	{
		defaultStoredItemModifers = modifiers;
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		if (useGunForDelivery && worker.usesMultiTool)
		{
			AnimInfo anim = base.GetAnim(worker);
			anim.smi = new MultitoolController.Instance(this, worker, "store", Assets.GetPrefab(EffectConfigs.OreAbsorbId));
			return anim;
		}
		return base.GetAnim(worker);
	}

	public override Vector3 GetTargetPoint()
	{
		Vector3 targetPoint = base.GetTargetPoint();
		if (useGunForDelivery && gunTargetOffset != Vector2.zero)
		{
			if (rotatable != null)
			{
				targetPoint += rotatable.GetRotatedOffset(gunTargetOffset);
			}
			else
			{
				targetPoint += new Vector3(gunTargetOffset.x, gunTargetOffset.y, 0f);
			}
		}
		return targetPoint;
	}

	protected override void OnPrefabInit()
	{
		if (useWideOffsets)
		{
			SetOffsetTable(OffsetGroups.InvertedWideTable);
		}
		else
		{
			SetOffsetTable(OffsetGroups.InvertedStandardTable);
		}
		showProgressBar = false;
		faceTargetWhenWorking = true;
		base.OnPrefabInit();
		GameUtil.SubscribeToTags(this, OnDeadTagAddedDelegate, triggerImmediately: true);
		Subscribe(1502190696, OnQueueDestroyObjectDelegate);
		Subscribe(-905833192, OnCopySettingsDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.Storing;
		resetProgressOnStop = true;
		synchronizeAnims = false;
		workingPstComplete = null;
		workingPstFailed = null;
		SetupStorageStatusItems();
	}

	private void SetupStorageStatusItems()
	{
		if (capacityStatusItem == null)
		{
			capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			capacityStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				Storage storage = (Storage)data;
				float num = storage.MassStored();
				float num2 = storage.capacityKg;
				num = ((!(num > num2 - storage.storageFullMargin) || !(num < num2)) ? Mathf.Floor(num) : num2);
				string newValue = Util.FormatWholeNumber(num);
				IUserControlledCapacity component = storage.GetComponent<IUserControlledCapacity>();
				if (component != null)
				{
					num2 = Mathf.Min(component.UserMaxCapacity, num2);
				}
				string newValue2 = Util.FormatWholeNumber(num2);
				str = str.Replace("{Stored}", newValue);
				str = str.Replace("{Capacity}", newValue2);
				str = ((component == null) ? str.Replace("{Units}", GameUtil.GetCurrentMassUnit()) : str.Replace("{Units}", component.CapacityUnits));
				return str;
			};
		}
		if (showCapacityStatusItem)
		{
			if (showCapacityAsMainStatus)
			{
				GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, capacityStatusItem, this);
			}
			else
			{
				GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Stored, capacityStatusItem, this);
			}
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (!allowSettingOnlyFetchMarkedItems)
		{
			onlyFetchMarkedItems = false;
		}
		UpdateFetchCategory();
	}

	protected override void OnSpawn()
	{
		SetWorkTime(storageWorkTime);
		foreach (GameObject item in items)
		{
			ApplyStoredItemModifiers(item, is_stored: true, is_initializing: true);
			if (sendOnStoreOnSpawn)
			{
				item.Trigger(856640610, this);
			}
		}
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.SetSymbolVisiblity("sweep", onlyFetchMarkedItems);
		}
		Prioritizable component2 = GetComponent<Prioritizable>();
		if (component2 != null)
		{
			component2.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(component2.onPriorityChanged, new Action<PrioritySetting>(OnPriorityChanged));
		}
		UpdateFetchCategory();
		if (showUnreachableStatus)
		{
			Subscribe(-1432940121, OnReachableChangedDelegate);
			new ReachabilityMonitor.Instance(this).StartSM();
		}
	}

	public GameObject Store(GameObject go, bool hide_popups = false, bool block_events = false, bool do_disease_transfer = true, bool is_deserializing = false)
	{
		if (go == null)
		{
			return null;
		}
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		GameObject result = go;
		if (!hide_popups && PopFXManager.Instance != null)
		{
			LocString locString;
			Transform target_transform;
			if (fxPrefix == FXPrefix.Delivered)
			{
				locString = UI.DELIVERED;
				target_transform = base.transform;
			}
			else
			{
				locString = UI.PICKEDUP;
				target_transform = go.transform;
			}
			string text = (Assets.IsTagCountable(go.PrefabID()) ? string.Format(locString, (int)component.Units, go.GetProperName()) : string.Format(locString, GameUtil.GetFormattedMass(component.Units), go.GetProperName()));
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, text, target_transform, storageFXOffset);
		}
		go.transform.parent = base.transform;
		Vector3 position = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Move);
		position.z = go.transform.GetPosition().z;
		go.transform.SetPosition(position);
		if (!block_events && do_disease_transfer)
		{
			TransferDiseaseWithObject(go);
		}
		if (!is_deserializing)
		{
			Pickupable component2 = go.GetComponent<Pickupable>();
			if (component2 != null)
			{
				if (component2 != null && component2.prevent_absorb_until_stored)
				{
					component2.prevent_absorb_until_stored = false;
				}
				foreach (GameObject item in items)
				{
					if (!(item != null))
					{
						continue;
					}
					Pickupable component3 = item.GetComponent<Pickupable>();
					if (!(component3 != null) || !component3.TryAbsorb(component2, hide_popups, allow_cross_storage: true))
					{
						continue;
					}
					if (!block_events)
					{
						Trigger(-1697596308, go);
						Trigger(-778359855, this);
						if (this.OnStorageIncreased != null)
						{
							this.OnStorageIncreased();
						}
					}
					ApplyStoredItemModifiers(go, is_stored: true, is_initializing: false);
					result = item;
					go = null;
					break;
				}
			}
		}
		if (go != null)
		{
			items.Add(go);
			if (!is_deserializing)
			{
				ApplyStoredItemModifiers(go, is_stored: true, is_initializing: false);
			}
			if (!block_events)
			{
				go.Trigger(856640610, this);
				Trigger(-1697596308, go);
				Trigger(-778359855, this);
				if (this.OnStorageIncreased != null)
				{
					this.OnStorageIncreased();
				}
			}
		}
		return result;
	}

	public PrimaryElement AddOre(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass = false, bool do_disease_transfer = true)
	{
		if (mass <= 0f)
		{
			return null;
		}
		PrimaryElement primaryElement = FindPrimaryElement(element);
		if (primaryElement != null)
		{
			float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, mass);
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			primaryElement.Mass += mass;
			primaryElement.Temperature = finalTemperature;
			primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddOre");
			Trigger(-1697596308, primaryElement.gameObject);
		}
		else
		{
			Element element2 = ElementLoader.FindElementByHash(element);
			GameObject gameObject = element2.substance.SpawnResource(base.transform.GetPosition(), mass, temperature, disease_idx, disease_count, prevent_merge: true, forceTemperature: false, manual_activation: true);
			gameObject.GetComponent<Pickupable>().prevent_absorb_until_stored = true;
			element2.substance.ActivateSubstanceGameObject(gameObject, disease_idx, disease_count);
			Store(gameObject, hide_popups: true, block_events: false, do_disease_transfer);
		}
		return primaryElement;
	}

	public PrimaryElement AddLiquid(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass = false, bool do_disease_transfer = true)
	{
		if (mass <= 0f)
		{
			return null;
		}
		PrimaryElement primaryElement = FindPrimaryElement(element);
		if (primaryElement != null)
		{
			float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, mass);
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			primaryElement.Mass += mass;
			primaryElement.Temperature = finalTemperature;
			primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddLiquid");
			Trigger(-1697596308, primaryElement.gameObject);
		}
		else
		{
			SubstanceChunk substanceChunk = LiquidSourceManager.Instance.CreateChunk(element, mass, temperature, disease_idx, disease_count, base.transform.GetPosition());
			primaryElement = substanceChunk.GetComponent<PrimaryElement>();
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			Store(substanceChunk.gameObject, hide_popups: true, block_events: false, do_disease_transfer);
		}
		return primaryElement;
	}

	public PrimaryElement AddGasChunk(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass, bool do_disease_transfer = true)
	{
		if (mass <= 0f)
		{
			return null;
		}
		PrimaryElement primaryElement = FindPrimaryElement(element);
		if (primaryElement != null)
		{
			float mass2 = primaryElement.Mass;
			float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, mass2, temperature, mass);
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			primaryElement.SetMassTemperature(mass2 + mass, finalTemperature);
			primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddGasChunk");
			Trigger(-1697596308, primaryElement.gameObject);
		}
		else
		{
			SubstanceChunk substanceChunk = GasSourceManager.Instance.CreateChunk(element, mass, temperature, disease_idx, disease_count, base.transform.GetPosition());
			primaryElement = substanceChunk.GetComponent<PrimaryElement>();
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			Store(substanceChunk.gameObject, hide_popups: true, block_events: false, do_disease_transfer);
		}
		return primaryElement;
	}

	public void Transfer(Storage target, bool block_events = false, bool hide_popups = false)
	{
		while (items.Count > 0)
		{
			Transfer(items[0], target, block_events, hide_popups);
		}
	}

	public float Transfer(Storage dest_storage, Tag tag, float amount, bool block_events = false, bool hide_popups = false)
	{
		GameObject gameObject = FindFirst(tag);
		if (gameObject != null)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (amount < component.Units)
			{
				Pickupable component2 = gameObject.GetComponent<Pickupable>();
				Pickupable pickupable = component2.Take(amount);
				dest_storage.Store(pickupable.gameObject, hide_popups, block_events);
				if (!block_events)
				{
					Trigger(-1697596308, component2.gameObject);
				}
			}
			else
			{
				Transfer(gameObject, dest_storage, block_events, hide_popups);
				amount = component.Units;
			}
			return amount;
		}
		return 0f;
	}

	public bool Transfer(GameObject go, Storage target, bool block_events = false, bool hide_popups = false)
	{
		items.RemoveAll((GameObject it) => it == null);
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			if (items[i] == go)
			{
				items.RemoveAt(i);
				ApplyStoredItemModifiers(go, is_stored: false, is_initializing: false);
				target.Store(go, hide_popups, block_events);
				if (!block_events)
				{
					Trigger(-1697596308, go);
				}
				return true;
			}
		}
		return false;
	}

	public List<GameObject> FindSome(Tag tag, float amount)
	{
		float num = amount;
		List<GameObject> list = new List<GameObject>();
		ListPool<GameObject, Storage>.PooledList pooledList = ListPool<GameObject, Storage>.Allocate();
		Find(tag, pooledList);
		foreach (GameObject item in pooledList)
		{
			Pickupable component = item.GetComponent<Pickupable>();
			if ((bool)component)
			{
				Pickupable pickupable = component.Take(num);
				num -= pickupable.GetComponent<PrimaryElement>().Mass;
				list.Add(pickupable.gameObject);
			}
		}
		pooledList.Recycle();
		return list;
	}

	public bool DropSome(Tag tag, float amount, bool ventGas = false, bool dumpLiquid = false, Vector3 offset = default(Vector3), bool doDiseaseTransfer = true, bool showInWorldNotification = false)
	{
		bool result = false;
		float num = amount;
		ListPool<GameObject, Storage>.PooledList pooledList = ListPool<GameObject, Storage>.Allocate();
		Find(tag, pooledList);
		foreach (GameObject item in pooledList)
		{
			Pickupable component = item.GetComponent<Pickupable>();
			if ((bool)component)
			{
				Pickupable pickupable = component.Take(num);
				if (pickupable != null)
				{
					bool flag = false;
					if (ventGas || dumpLiquid)
					{
						Dumpable component2 = pickupable.GetComponent<Dumpable>();
						if (component2 != null)
						{
							if (ventGas && pickupable.GetComponent<PrimaryElement>().Element.IsGas)
							{
								component2.Dump(base.transform.GetPosition() + offset);
								flag = true;
								num -= pickupable.GetComponent<PrimaryElement>().Mass;
								Trigger(-1697596308, pickupable.gameObject);
								result = true;
								if (showInWorldNotification)
								{
									PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, pickupable.GetComponent<PrimaryElement>().Element.name + " " + GameUtil.GetFormattedMass(pickupable.TotalAmount), pickupable.transform, storageFXOffset);
								}
							}
							if (dumpLiquid && pickupable.GetComponent<PrimaryElement>().Element.IsLiquid)
							{
								component2.Dump(base.transform.GetPosition() + offset);
								flag = true;
								num -= pickupable.GetComponent<PrimaryElement>().Mass;
								Trigger(-1697596308, pickupable.gameObject);
								result = true;
								if (showInWorldNotification)
								{
									PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, pickupable.GetComponent<PrimaryElement>().Element.name + " " + GameUtil.GetFormattedMass(pickupable.TotalAmount), pickupable.transform, storageFXOffset);
								}
							}
						}
					}
					if (!flag)
					{
						Vector3 position = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore) + offset;
						pickupable.transform.SetPosition(position);
						KBatchedAnimController component3 = pickupable.GetComponent<KBatchedAnimController>();
						if ((bool)component3)
						{
							component3.SetSceneLayer(Grid.SceneLayer.Ore);
						}
						num -= pickupable.GetComponent<PrimaryElement>().Mass;
						MakeWorldActive(pickupable.gameObject);
						Trigger(-1697596308, pickupable.gameObject);
						result = true;
						if (showInWorldNotification)
						{
							PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, pickupable.GetComponent<PrimaryElement>().Element.name + " " + GameUtil.GetFormattedMass(pickupable.TotalAmount), pickupable.transform, storageFXOffset);
						}
					}
				}
			}
			if (num <= 0f)
			{
				break;
			}
		}
		pooledList.Recycle();
		return result;
	}

	public void DropAll(Vector3 position, bool vent_gas = false, bool dump_liquid = false, Vector3 offset = default(Vector3), bool do_disease_transfer = true, List<GameObject> collect_dropped_items = null)
	{
		while (items.Count > 0)
		{
			GameObject gameObject = items[0];
			if (do_disease_transfer)
			{
				TransferDiseaseWithObject(gameObject);
			}
			items.RemoveAt(0);
			if (!(gameObject != null))
			{
				continue;
			}
			bool flag = false;
			if (vent_gas || dump_liquid)
			{
				Dumpable component = gameObject.GetComponent<Dumpable>();
				if (component != null)
				{
					if (vent_gas && gameObject.GetComponent<PrimaryElement>().Element.IsGas)
					{
						component.Dump(position + offset);
						flag = true;
					}
					if (dump_liquid && gameObject.GetComponent<PrimaryElement>().Element.IsLiquid)
					{
						component.Dump(position + offset);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				gameObject.transform.SetPosition(position + offset);
				KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
				if ((bool)component2)
				{
					component2.SetSceneLayer(Grid.SceneLayer.Ore);
				}
				MakeWorldActive(gameObject);
				collect_dropped_items?.Add(gameObject);
			}
		}
	}

	public void DropAll(bool vent_gas = false, bool dump_liquid = false, Vector3 offset = default(Vector3), bool do_disease_transfer = true, List<GameObject> collect_dropped_items = null)
	{
		DropAll(Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore), vent_gas, dump_liquid, offset, do_disease_transfer, collect_dropped_items);
	}

	public void Drop(Tag t, List<GameObject> obj_list)
	{
		Find(t, obj_list);
		foreach (GameObject item in obj_list)
		{
			Drop(item);
		}
	}

	public void Drop(Tag t)
	{
		ListPool<GameObject, Storage>.PooledList pooledList = ListPool<GameObject, Storage>.Allocate();
		Find(t, pooledList);
		foreach (GameObject item in pooledList)
		{
			Drop(item);
		}
		pooledList.Recycle();
	}

	public void DropUnlessHasTags(TagBits any_tags, TagBits required_tags, TagBits forbidden_tags, bool do_disease_transfer = true, bool dumpElements = false)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i] == null)
			{
				continue;
			}
			KPrefabID component = items[i].GetComponent<KPrefabID>();
			if (component.HasAnyTags(ref any_tags) && component.HasAllTags(ref required_tags) && !component.HasAnyTags(ref forbidden_tags))
			{
				continue;
			}
			GameObject gameObject = items[i];
			items.RemoveAt(i);
			i--;
			if (do_disease_transfer)
			{
				TransferDiseaseWithObject(gameObject);
			}
			MakeWorldActive(gameObject);
			if (dumpElements)
			{
				Dumpable component2 = gameObject.GetComponent<Dumpable>();
				if (component2 != null)
				{
					component2.Dump(base.transform.GetPosition());
				}
			}
		}
	}

	public GameObject Drop(GameObject go, bool do_disease_transfer = true)
	{
		if (go != null)
		{
			int count = items.Count;
			for (int i = 0; i < count; i++)
			{
				if (go == items[i])
				{
					items[i] = items[count - 1];
					items.RemoveAt(count - 1);
					if (do_disease_transfer)
					{
						TransferDiseaseWithObject(go);
					}
					MakeWorldActive(go);
					break;
				}
			}
		}
		return go;
	}

	public void RenotifyAll()
	{
		items.RemoveAll((GameObject it) => it == null);
		foreach (GameObject item in items)
		{
			item.Trigger(856640610, this);
		}
	}

	private void TransferDiseaseWithObject(GameObject obj)
	{
		if (obj == null || !doDiseaseTransfer || primaryElement == null)
		{
			return;
		}
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		if (!(component == null))
		{
			SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
			invalid.idx = component.DiseaseIdx;
			invalid.count = (int)((float)component.DiseaseCount * 0.05f);
			SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid;
			invalid2.idx = primaryElement.DiseaseIdx;
			invalid2.count = (int)((float)primaryElement.DiseaseCount * 0.05f);
			component.ModifyDiseaseCount(-invalid.count, "Storage.TransferDiseaseWithObject");
			primaryElement.ModifyDiseaseCount(-invalid2.count, "Storage.TransferDiseaseWithObject");
			if (invalid.count > 0)
			{
				primaryElement.AddDisease(invalid.idx, invalid.count, "Storage.TransferDiseaseWithObject");
			}
			if (invalid2.count > 0)
			{
				component.AddDisease(invalid2.idx, invalid2.count, "Storage.TransferDiseaseWithObject");
			}
		}
	}

	private void MakeWorldActive(GameObject go)
	{
		go.transform.parent = null;
		go.Trigger(856640610);
		Trigger(-1697596308, go);
		ApplyStoredItemModifiers(go, is_stored: false, is_initializing: false);
		if (!(go != null))
		{
			return;
		}
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		if (component != null && component.KeepZeroMassObject)
		{
			component.KeepZeroMassObject = false;
			if (component.Mass <= 0f)
			{
				Util.KDestroyGameObject(go);
			}
		}
	}

	public List<GameObject> Find(Tag tag, List<GameObject> result)
	{
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (!(gameObject == null) && gameObject.HasTag(tag))
			{
				result.Add(gameObject);
			}
		}
		return result;
	}

	public GameObject FindFirst(Tag tag)
	{
		GameObject result = null;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (!(gameObject == null) && gameObject.HasTag(tag))
			{
				result = gameObject;
				break;
			}
		}
		return result;
	}

	public PrimaryElement FindFirstWithMass(Tag tag, float mass = 0f)
	{
		PrimaryElement result = null;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (!(gameObject == null) && gameObject.HasTag(tag))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (component.Mass > 0f && component.Mass >= mass)
				{
					result = component;
					break;
				}
			}
		}
		return result;
	}

	public List<Tag> GetAllTagsInStorage()
	{
		List<Tag> list = new List<Tag>();
		for (int i = 0; i < items.Count; i++)
		{
			GameObject go = items[i];
			if (!list.Contains(go.PrefabID()))
			{
				list.Add(go.PrefabID());
			}
		}
		return list;
	}

	public GameObject Find(int ID)
	{
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (ID == gameObject.PrefabID().GetHashCode())
			{
				return gameObject;
			}
		}
		return null;
	}

	public void ConsumeAllIgnoringDisease()
	{
		for (int num = items.Count - 1; num >= 0; num--)
		{
			ConsumeIgnoringDisease(items[num]);
		}
	}

	public void ConsumeAndGetDisease(Tag tag, float amount, out float amount_consumed, out SimUtil.DiseaseInfo disease_info, out float aggregate_temperature)
	{
		DebugUtil.Assert(tag.IsValid);
		amount_consumed = 0f;
		disease_info = SimUtil.DiseaseInfo.Invalid;
		aggregate_temperature = 0f;
		bool flag = false;
		for (int i = 0; i < items.Count; i++)
		{
			if (amount <= 0f)
			{
				break;
			}
			GameObject gameObject = items[i];
			if (gameObject == null || !gameObject.HasTag(tag))
			{
				continue;
			}
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component.Units > 0f)
			{
				flag = true;
				float num = Math.Min(component.Units, amount);
				Debug.Assert(num > 0f, "Delta amount was zero, which should be impossible.");
				aggregate_temperature = SimUtil.CalculateFinalTemperature(amount_consumed, aggregate_temperature, num, component.Temperature);
				SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(component, num / component.Units);
				disease_info = SimUtil.CalculateFinalDiseaseInfo(disease_info, percentOfDisease);
				component.Units -= num;
				component.ModifyDiseaseCount(-percentOfDisease.count, "Storage.ConsumeAndGetDisease");
				amount -= num;
				amount_consumed += num;
			}
			if (component.Units <= 0f && !component.KeepZeroMassObject)
			{
				if (deleted_objects == null)
				{
					deleted_objects = new List<GameObject>();
				}
				deleted_objects.Add(gameObject);
			}
			Trigger(-1697596308, gameObject);
		}
		if (!flag)
		{
			aggregate_temperature = GetComponent<PrimaryElement>().Temperature;
		}
		if (deleted_objects != null)
		{
			for (int j = 0; j < deleted_objects.Count; j++)
			{
				items.Remove(deleted_objects[j]);
				Util.KDestroyGameObject(deleted_objects[j]);
			}
			deleted_objects.Clear();
		}
	}

	public void ConsumeAndGetDisease(Recipe.Ingredient ingredient, out SimUtil.DiseaseInfo disease_info, out float temperature)
	{
		ConsumeAndGetDisease(ingredient.tag, ingredient.amount, out var _, out disease_info, out temperature);
	}

	public void ConsumeIgnoringDisease(Tag tag, float amount)
	{
		ConsumeAndGetDisease(tag, amount, out var _, out var _, out var _);
	}

	public void ConsumeIgnoringDisease(GameObject item_go)
	{
		if (items.Contains(item_go))
		{
			PrimaryElement component = item_go.GetComponent<PrimaryElement>();
			if (component != null && component.KeepZeroMassObject)
			{
				component.Units = 0f;
				component.ModifyDiseaseCount(-component.DiseaseCount, "consume item");
				Trigger(-1697596308, item_go);
			}
			else
			{
				items.Remove(item_go);
				Trigger(-1697596308, item_go);
				item_go.DeleteObject();
			}
		}
	}

	public GameObject Drop(int ID)
	{
		return Drop(Find(ID));
	}

	private void OnDeath(object data)
	{
		DropAll(vent_gas: true, dump_liquid: true);
	}

	public bool IsFull()
	{
		return RemainingCapacity() <= 0f;
	}

	public bool IsEmpty()
	{
		return items.Count == 0;
	}

	public float Capacity()
	{
		return capacityKg;
	}

	public bool IsEndOfLife()
	{
		return endOfLife;
	}

	public float MassStored()
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			if (!(items[i] == null))
			{
				PrimaryElement component = items[i].GetComponent<PrimaryElement>();
				if (component != null)
				{
					num += component.Units * component.MassPerUnit;
				}
			}
		}
		return (float)Mathf.RoundToInt(num * 1000f) / 1000f;
	}

	public float UnitsStored()
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			if (!(items[i] == null))
			{
				PrimaryElement component = items[i].GetComponent<PrimaryElement>();
				if (component != null)
				{
					num += component.Units;
				}
			}
		}
		return (float)Mathf.RoundToInt(num * 1000f) / 1000f;
	}

	public bool Has(Tag tag)
	{
		bool result = false;
		foreach (GameObject item in items)
		{
			if (!(item == null))
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (component.HasTag(tag) && component.Mass > 0f)
				{
					return true;
				}
			}
		}
		return result;
	}

	public PrimaryElement AddToPrimaryElement(SimHashes element, float additional_mass, float temperature)
	{
		PrimaryElement primaryElement = FindPrimaryElement(element);
		if (primaryElement != null)
		{
			float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, additional_mass);
			primaryElement.Mass += additional_mass;
			primaryElement.Temperature = finalTemperature;
		}
		return primaryElement;
	}

	public PrimaryElement FindPrimaryElement(SimHashes element)
	{
		PrimaryElement result = null;
		foreach (GameObject item in items)
		{
			if (!(item == null))
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (component.ElementID == element)
				{
					return component;
				}
			}
		}
		return result;
	}

	public float RemainingCapacity()
	{
		return capacityKg - MassStored();
	}

	public bool GetOnlyFetchMarkedItems()
	{
		return onlyFetchMarkedItems;
	}

	public void SetOnlyFetchMarkedItems(bool is_set)
	{
		if (is_set != onlyFetchMarkedItems)
		{
			onlyFetchMarkedItems = is_set;
			UpdateFetchCategory();
			Trigger(644822890);
			GetComponent<KBatchedAnimController>().SetSymbolVisiblity("sweep", is_set);
		}
	}

	private void UpdateFetchCategory()
	{
		if (fetchCategory != 0)
		{
			fetchCategory = ((!onlyFetchMarkedItems) ? FetchCategory.GeneralStorage : FetchCategory.StorageSweepOnly);
		}
	}

	protected override void OnCleanUp()
	{
		if (items.Count != 0)
		{
			Debug.LogWarning("Storage for [" + base.gameObject.name + "] is being destroyed but it still contains items!", base.gameObject);
		}
		base.OnCleanUp();
	}

	private void OnQueueDestroyObject(object data)
	{
		endOfLife = true;
		DropAll(vent_gas: true);
		OnCleanUp();
	}

	public void Remove(GameObject go, bool do_disease_transfer = true)
	{
		items.Remove(go);
		if (do_disease_transfer)
		{
			TransferDiseaseWithObject(go);
		}
		Trigger(-1697596308, go);
		ApplyStoredItemModifiers(go, is_stored: false, is_initializing: false);
	}

	public bool ForceStore(Tag tag, float amount)
	{
		Debug.Assert(amount < PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT);
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (gameObject != null && gameObject.HasTag(tag))
			{
				gameObject.GetComponent<PrimaryElement>().Mass += amount;
				return true;
			}
		}
		return false;
	}

	public float GetAmountAvailable(Tag tag)
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (gameObject != null && gameObject.HasTag(tag))
			{
				num += gameObject.GetComponent<PrimaryElement>().Units;
			}
		}
		return num;
	}

	public float GetAmountAvailable(Tag tag, Tag[] forbiddenTags = null)
	{
		if (forbiddenTags == null)
		{
			return GetAmountAvailable(tag);
		}
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (gameObject != null && gameObject.HasTag(tag) && !gameObject.HasAnyTags(forbiddenTags))
			{
				num += gameObject.GetComponent<PrimaryElement>().Units;
			}
		}
		return num;
	}

	public float GetUnitsAvailable(Tag tag)
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (gameObject != null && gameObject.HasTag(tag))
			{
				num += gameObject.GetComponent<PrimaryElement>().Units;
			}
		}
		return num;
	}

	public float GetMassAvailable(Tag tag)
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (gameObject != null && gameObject.HasTag(tag))
			{
				num += gameObject.GetComponent<PrimaryElement>().Mass;
			}
		}
		return num;
	}

	public float GetMassAvailable(SimHashes element)
	{
		float num = 0f;
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = items[i];
			if (gameObject != null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (component.ElementID == element)
				{
					num += component.Mass;
				}
			}
		}
		return num;
	}

	public bool IsMaterialOnStorage(Tag tag, ref float amount)
	{
		foreach (GameObject item in items)
		{
			if (item != null)
			{
				Pickupable component = item.GetComponent<Pickupable>();
				if (component != null && component.GetComponent<KPrefabID>().HasTag(tag))
				{
					amount = component.TotalAmount;
					return true;
				}
			}
		}
		return false;
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		if (showDescriptor)
		{
			descriptors.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.STORAGECAPACITY, GameUtil.GetFormattedMass(Capacity())), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STORAGECAPACITY, GameUtil.GetFormattedMass(Capacity()))));
		}
		return descriptors;
	}

	public static void MakeItemTemperatureInsulated(GameObject go, bool is_stored, bool is_initializing)
	{
		SimTemperatureTransfer component = go.GetComponent<SimTemperatureTransfer>();
		if (!(component == null))
		{
			component.enabled = !is_stored;
		}
	}

	public static void MakeItemInvisible(GameObject go, bool is_stored, bool is_initializing)
	{
		if (!is_initializing)
		{
			bool flag = !is_stored;
			KAnimControllerBase component = go.GetComponent<KAnimControllerBase>();
			if (component != null && component.enabled != flag)
			{
				component.enabled = flag;
			}
			KSelectable component2 = go.GetComponent<KSelectable>();
			if (component2 != null && component2.enabled != flag)
			{
				component2.enabled = flag;
			}
		}
	}

	public static void MakeItemSealed(GameObject go, bool is_stored, bool is_initializing)
	{
		if (go != null)
		{
			if (is_stored)
			{
				go.GetComponent<KPrefabID>().AddTag(GameTags.Sealed);
			}
			else
			{
				go.GetComponent<KPrefabID>().RemoveTag(GameTags.Sealed);
			}
		}
	}

	public static void MakeItemPreserved(GameObject go, bool is_stored, bool is_initializing)
	{
		if (go != null)
		{
			if (is_stored)
			{
				go.GetComponent<KPrefabID>().AddTag(GameTags.Preserved);
			}
			else
			{
				go.GetComponent<KPrefabID>().RemoveTag(GameTags.Preserved);
			}
		}
	}

	private void ApplyStoredItemModifiers(GameObject go, bool is_stored, bool is_initializing)
	{
		List<StoredItemModifier> list = defaultStoredItemModifers;
		for (int i = 0; i < list.Count; i++)
		{
			StoredItemModifier storedItemModifier = list[i];
			for (int j = 0; j < StoredItemModifierHandlers.Count; j++)
			{
				StoredItemModifierInfo storedItemModifierInfo = StoredItemModifierHandlers[j];
				if (storedItemModifierInfo.modifier == storedItemModifier)
				{
					storedItemModifierInfo.toggleState(go, is_stored, is_initializing);
					break;
				}
			}
		}
	}

	private void OnCopySettings(object data)
	{
		Storage component = ((GameObject)data).GetComponent<Storage>();
		if (component != null)
		{
			SetOnlyFetchMarkedItems(component.onlyFetchMarkedItems);
		}
	}

	private void OnPriorityChanged(PrioritySetting priority)
	{
		foreach (GameObject item in items)
		{
			item.Trigger(-1626373771, this);
		}
	}

	private void OnReachableChanged(object data)
	{
		bool num = (bool)data;
		KSelectable component = GetComponent<KSelectable>();
		if (num)
		{
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.StorageUnreachable);
		}
		else
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.StorageUnreachable, this);
		}
	}

	private bool ShouldSaveItem(GameObject go)
	{
		bool result = false;
		if (go != null && go.GetComponent<SaveLoadRoot>() != null && go.GetComponent<PrimaryElement>().Mass > 0f)
		{
			result = true;
		}
		return result;
	}

	public void Serialize(BinaryWriter writer)
	{
		int num = 0;
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			if (ShouldSaveItem(items[i]))
			{
				num++;
			}
		}
		writer.Write(num);
		if (num == 0 || items == null || items.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < items.Count; j++)
		{
			GameObject gameObject = items[j];
			if (ShouldSaveItem(gameObject))
			{
				SaveLoadRoot component = gameObject.GetComponent<SaveLoadRoot>();
				if (component != null)
				{
					string str = gameObject.GetComponent<KPrefabID>().GetSaveLoadTag().Name;
					writer.WriteKleiString(str);
					component.Save(writer);
				}
				else
				{
					Debug.Log("Tried to save obj in storage but obj has no SaveLoadRoot", gameObject);
				}
			}
		}
	}

	public void Deserialize(IReader reader)
	{
		_ = Time.realtimeSinceStartup;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		ClearItems();
		int num4 = reader.ReadInt32();
		items = new List<GameObject>(num4);
		for (int i = 0; i < num4; i++)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			Tag tag = TagManager.Create(reader.ReadKleiString());
			SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(tag, reader);
			num += Time.realtimeSinceStartup - realtimeSinceStartup;
			if (saveLoadRoot != null)
			{
				KBatchedAnimController component = saveLoadRoot.GetComponent<KBatchedAnimController>();
				if (component != null)
				{
					component.enabled = false;
				}
				saveLoadRoot.SetRegistered(registered: false);
				float realtimeSinceStartup2 = Time.realtimeSinceStartup;
				GameObject gameObject = Store(saveLoadRoot.gameObject, hide_popups: true, block_events: true, do_disease_transfer: false, is_deserializing: true);
				num2 += Time.realtimeSinceStartup - realtimeSinceStartup2;
				if (gameObject != null)
				{
					Pickupable component2 = gameObject.GetComponent<Pickupable>();
					if (component2 != null)
					{
						float realtimeSinceStartup3 = Time.realtimeSinceStartup;
						component2.OnStore(this);
						num3 += Time.realtimeSinceStartup - realtimeSinceStartup3;
					}
					Storable component3 = gameObject.GetComponent<Storable>();
					if (component3 != null)
					{
						float realtimeSinceStartup4 = Time.realtimeSinceStartup;
						component3.OnStore(this);
						num3 += Time.realtimeSinceStartup - realtimeSinceStartup4;
					}
					if (dropOnLoad)
					{
						Drop(saveLoadRoot.gameObject);
					}
				}
			}
			else
			{
				Debug.LogWarning("Tried to deserialize " + tag.ToString() + " into storage but failed", base.gameObject);
			}
		}
	}

	private void ClearItems()
	{
		foreach (GameObject item in items)
		{
			item.DeleteObject();
		}
		items.Clear();
	}

	public void UpdateStoredItemCachedCells()
	{
		foreach (GameObject item in items)
		{
			Pickupable component = item.GetComponent<Pickupable>();
			if (component != null)
			{
				component.UpdateCachedCellFromStoragePosition();
			}
		}
	}
}
