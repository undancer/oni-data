using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Tinkerable")]
public class Tinkerable : Workable
{
	private Chore chore;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Effects effects;

	[MyCmpGet]
	private RoomTracker roomTracker;

	public Tag tinkerMaterialTag;

	public float tinkerMaterialAmount;

	public string addedEffect;

	public string effectAttributeId;

	public float effectMultiplier;

	public HashedString choreTypeTinker = Db.Get().ChoreTypes.PowerTinker.IdHash;

	public HashedString choreTypeFetch = Db.Get().ChoreTypes.PowerFetch.IdHash;

	private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnEffectRemovedDelegate = new EventSystem.IntraObjectHandler<Tinkerable>(delegate(Tinkerable component, object data)
	{
		component.OnEffectRemoved(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<Tinkerable>(delegate(Tinkerable component, object data)
	{
		component.OnStorageChange(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnUpdateRoomDelegate = new EventSystem.IntraObjectHandler<Tinkerable>(delegate(Tinkerable component, object data)
	{
		component.OnUpdateRoom(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Tinkerable>(delegate(Tinkerable component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private SchedulerHandle updateHandle;

	private bool hasReservedMaterial = false;

	public static Tinkerable MakePowerTinkerable(GameObject prefab)
	{
		RoomTracker roomTracker = prefab.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.PowerPlant.Id;
		roomTracker.requirement = RoomTracker.Requirement.TrackingOnly;
		Tinkerable tinkerable = prefab.AddOrGet<Tinkerable>();
		tinkerable.tinkerMaterialTag = PowerControlStationConfig.TINKER_TOOLS;
		tinkerable.tinkerMaterialAmount = 1f;
		tinkerable.requiredSkillPerk = PowerControlStationConfig.ROLE_PERK;
		tinkerable.SetWorkTime(180f);
		tinkerable.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
		tinkerable.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		tinkerable.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		tinkerable.choreTypeTinker = Db.Get().ChoreTypes.PowerTinker.IdHash;
		tinkerable.choreTypeFetch = Db.Get().ChoreTypes.PowerFetch.IdHash;
		tinkerable.addedEffect = "PowerTinker";
		tinkerable.effectAttributeId = Db.Get().Attributes.Machinery.Id;
		tinkerable.effectMultiplier = 0.025f;
		tinkerable.multitoolContext = "powertinker";
		tinkerable.multitoolHitEffectTag = "fx_powertinker_splash";
		tinkerable.shouldShowSkillPerkStatusItem = false;
		prefab.AddOrGet<Storage>();
		prefab.AddOrGet<Effects>();
		KPrefabID component = prefab.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetComponent<Tinkerable>().SetOffsetTable(OffsetGroups.InvertedStandardTable);
		};
		return tinkerable;
	}

	public static Tinkerable MakeFarmTinkerable(GameObject prefab)
	{
		RoomTracker roomTracker = prefab.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Farm.Id;
		roomTracker.requirement = RoomTracker.Requirement.TrackingOnly;
		Tinkerable tinkerable = prefab.AddOrGet<Tinkerable>();
		tinkerable.tinkerMaterialTag = FarmStationConfig.TINKER_TOOLS;
		tinkerable.tinkerMaterialAmount = 1f;
		tinkerable.requiredSkillPerk = Db.Get().SkillPerks.CanFarmTinker.Id;
		tinkerable.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
		tinkerable.addedEffect = "FarmTinker";
		tinkerable.effectAttributeId = Db.Get().Attributes.Botanist.Id;
		tinkerable.effectMultiplier = 0.1f;
		tinkerable.SetWorkTime(15f);
		tinkerable.attributeConverter = Db.Get().AttributeConverters.PlantTendSpeed;
		tinkerable.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		tinkerable.choreTypeTinker = Db.Get().ChoreTypes.CropTend.IdHash;
		tinkerable.choreTypeFetch = Db.Get().ChoreTypes.FarmFetch.IdHash;
		tinkerable.multitoolContext = "tend";
		tinkerable.multitoolHitEffectTag = "fx_tend_splash";
		tinkerable.shouldShowSkillPerkStatusItem = false;
		prefab.AddOrGet<Storage>();
		prefab.AddOrGet<Effects>();
		KPrefabID component = prefab.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetComponent<Tinkerable>().SetOffsetTable(OffsetGroups.InvertedStandardTable);
		};
		return tinkerable;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_use_machine_kanim")
		};
		workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		Subscribe(-1157678353, OnEffectRemovedDelegate);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		Subscribe(144050788, OnUpdateRoomDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	protected override void OnCleanUp()
	{
		UpdateMaterialReservation(shouldReserve: false);
		if (updateHandle.IsValid)
		{
			updateHandle.ClearScheduler();
		}
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		QueueUpdateChore();
	}

	private void OnEffectRemoved(object data)
	{
		QueueUpdateChore();
	}

	private void OnUpdateRoom(object data)
	{
		QueueUpdateChore();
	}

	private void OnStorageChange(object data)
	{
		GameObject go = (GameObject)data;
		if (go.HasTag(tinkerMaterialTag))
		{
			QueueUpdateChore();
		}
	}

	private void QueueUpdateChore()
	{
		if (updateHandle.IsValid)
		{
			updateHandle.ClearScheduler();
		}
		updateHandle = GameScheduler.Instance.Schedule("UpdateTinkerChore", 1.2f, UpdateChoreCallback);
	}

	private void UpdateChoreCallback(object obj)
	{
		UpdateChore();
	}

	private void UpdateChore()
	{
		Operational component = GetComponent<Operational>();
		bool flag = component == null || component.IsFunctional;
		bool flag2 = HasEffect();
		bool flag3 = RoomHasActiveTinkerstation();
		bool flag4 = !flag2 && flag3 && flag;
		bool flag5 = flag2 || !flag3;
		if (chore == null && flag4)
		{
			UpdateMaterialReservation(shouldReserve: true);
			if (HasMaterial())
			{
				chore = new WorkChore<Tinkerable>(Db.Get().ChoreTypes.GetByHash(choreTypeTinker), this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
				if (component != null)
				{
					chore.AddPrecondition(ChorePreconditions.instance.IsFunctional, component);
				}
			}
			else
			{
				chore = new FetchChore(Db.Get().ChoreTypes.GetByHash(choreTypeFetch), storage, tinkerMaterialAmount, new Tag[1]
				{
					tinkerMaterialTag
				}, null, null, null, run_until_complete: true, OnFetchComplete, null, null, FetchOrder2.OperationalRequirement.Functional);
			}
			chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, requiredSkillPerk);
			RoomTracker component2 = GetComponent<RoomTracker>();
			if (!string.IsNullOrEmpty(component2.requiredRoomType))
			{
				chore.AddPrecondition(ChorePreconditions.instance.IsInMyRoom, Grid.PosToCell(base.transform.GetPosition()));
			}
		}
		else if (chore != null && flag5)
		{
			UpdateMaterialReservation(shouldReserve: false);
			chore.Cancel("No longer needed");
			chore = null;
		}
	}

	private bool RoomHasActiveTinkerstation()
	{
		if (!roomTracker.IsInCorrectRoom())
		{
			return false;
		}
		if (roomTracker.room == null)
		{
			return false;
		}
		foreach (KPrefabID building in roomTracker.room.buildings)
		{
			if (building == null)
			{
				continue;
			}
			TinkerStation component = building.GetComponent<TinkerStation>();
			if (component != null && component.outputPrefab == tinkerMaterialTag)
			{
				Operational component2 = building.GetComponent<Operational>();
				if (component2.IsOperational)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void UpdateMaterialReservation(bool shouldReserve)
	{
		if (shouldReserve && !hasReservedMaterial)
		{
			MaterialNeeds.UpdateNeed(tinkerMaterialTag, tinkerMaterialAmount, base.gameObject.GetMyWorldId());
			hasReservedMaterial = shouldReserve;
		}
		else if (!shouldReserve && hasReservedMaterial)
		{
			MaterialNeeds.UpdateNeed(tinkerMaterialTag, 0f - tinkerMaterialAmount, base.gameObject.GetMyWorldId());
			hasReservedMaterial = shouldReserve;
		}
	}

	private void OnFetchComplete(Chore data)
	{
		UpdateMaterialReservation(shouldReserve: false);
		chore = null;
		UpdateChore();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		storage.ConsumeIgnoringDisease(tinkerMaterialTag, tinkerMaterialAmount);
		float totalValue = worker.GetAttributes().Get(Db.Get().Attributes.Get(effectAttributeId)).GetTotalValue();
		effects.Add(addedEffect, should_save: true).timeRemaining *= 1f + totalValue * effectMultiplier;
		UpdateMaterialReservation(shouldReserve: false);
		chore = null;
		UpdateChore();
	}

	private bool HasMaterial()
	{
		return storage.GetAmountAvailable(tinkerMaterialTag) >= tinkerMaterialAmount;
	}

	private bool HasEffect()
	{
		return effects.HasEffect(addedEffect);
	}
}
