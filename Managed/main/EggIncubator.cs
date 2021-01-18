using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EggIncubator : SingleEntityReceptacle, ISaveLoadable, ISim1000ms
{
	[MyCmpAdd]
	private EggIncubatorWorkable workable;

	private Chore chore;

	private EggIncubatorStates.Instance smi;

	private KBatchedAnimTracker tracker;

	private MeterController meter;

	private static readonly EventSystem.IntraObjectHandler<EggIncubator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<EggIncubator>(delegate(EggIncubator component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<EggIncubator> OnOccupantChangedDelegate = new EventSystem.IntraObjectHandler<EggIncubator>(delegate(EggIncubator component, object data)
	{
		component.OnOccupantChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<EggIncubator> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<EggIncubator>(delegate(EggIncubator component, object data)
	{
		component.OnStorageChange(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		autoReplaceEntity = true;
		statusItemNeed = Db.Get().BuildingStatusItems.NeedEgg;
		statusItemNoneAvailable = Db.Get().BuildingStatusItems.NoAvailableEgg;
		statusItemAwaitingDelivery = Db.Get().BuildingStatusItems.AwaitingEggDelivery;
		requiredSkillPerk = Db.Get().SkillPerks.CanWrangleCreatures.Id;
		occupyingObjectRelativePosition = new Vector3(0.5f, 1f, -1f);
		synchronizeAnims = false;
		GetComponent<KBatchedAnimController>().SetSymbolVisiblity("egg_target", is_visible: false);
		meter = new MeterController(this, Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if ((bool)base.occupyingObject)
		{
			if (base.occupyingObject.HasTag(GameTags.Creature))
			{
				storage.allowItemRemoval = true;
			}
			storage.RenotifyAll();
			PositionOccupyingObject();
		}
		Subscribe(-592767678, OnOperationalChangedDelegate);
		Subscribe(-731304873, OnOccupantChangedDelegate);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		smi = new EggIncubatorStates.Instance(this);
		smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		smi.StopSM("cleanup");
		base.OnCleanUp();
	}

	protected override void SubscribeToOccupant()
	{
		base.SubscribeToOccupant();
		if (base.occupyingObject != null)
		{
			tracker = base.occupyingObject.AddComponent<KBatchedAnimTracker>();
			tracker.symbol = "egg_target";
			tracker.forceAlwaysVisible = true;
		}
		UpdateProgress();
	}

	protected override void UnsubscribeFromOccupant()
	{
		base.UnsubscribeFromOccupant();
		Object.Destroy(tracker);
		tracker = null;
		UpdateProgress();
	}

	private void OnOperationalChanged(object data = null)
	{
		if (!base.occupyingObject)
		{
			storage.DropAll();
		}
	}

	private void OnOccupantChanged(object data = null)
	{
		if (!base.occupyingObject)
		{
			storage.allowItemRemoval = false;
		}
	}

	private void OnStorageChange(object data = null)
	{
		if ((bool)base.occupyingObject && !storage.items.Contains(base.occupyingObject))
		{
			UnsubscribeFromOccupant();
			ClearOccupant();
		}
	}

	protected override void ClearOccupant()
	{
		bool flag = false;
		if (base.occupyingObject != null)
		{
			flag = !base.occupyingObject.HasTag(GameTags.Egg);
		}
		base.ClearOccupant();
		if (autoReplaceEntity && flag && requestedEntityTag.IsValid)
		{
			CreateOrder(requestedEntityTag);
		}
	}

	protected override void PositionOccupyingObject()
	{
		base.PositionOccupyingObject();
		base.occupyingObject.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingUse);
		KSelectable component = base.occupyingObject.GetComponent<KSelectable>();
		if (component != null)
		{
			component.IsSelectable = true;
		}
	}

	public override void OrderRemoveOccupant()
	{
		Object.Destroy(tracker);
		tracker = null;
		storage.DropAll();
		base.occupyingObject = null;
		ClearOccupant();
	}

	public float GetProgress()
	{
		float result = 0f;
		if ((bool)base.occupyingObject)
		{
			AmountInstance amountInstance = base.occupyingObject.GetAmounts().Get(Db.Get().Amounts.Incubation);
			result = ((amountInstance == null) ? 1f : (amountInstance.value / amountInstance.GetMax()));
		}
		return result;
	}

	private void UpdateProgress()
	{
		meter.SetPositionPercent(GetProgress());
	}

	public void Sim1000ms(float dt)
	{
		UpdateProgress();
		UpdateChore();
	}

	public void StoreBaby(GameObject baby)
	{
		UnsubscribeFromOccupant();
		storage.DropAll();
		storage.allowItemRemoval = true;
		storage.Store(baby);
		base.occupyingObject = baby;
		SubscribeToOccupant();
		Trigger(-731304873, base.occupyingObject);
	}

	private void UpdateChore()
	{
		if (operational.IsOperational && EggNeedsAttention())
		{
			if (chore == null)
			{
				chore = new WorkChore<EggIncubatorWorkable>(Db.Get().ChoreTypes.EggSing, workable);
			}
		}
		else if (chore != null)
		{
			chore.Cancel("now is not the time for song");
			chore = null;
		}
	}

	private bool EggNeedsAttention()
	{
		if (!base.Occupant)
		{
			return false;
		}
		IncubationMonitor.Instance sMI = base.Occupant.GetSMI<IncubationMonitor.Instance>();
		if (sMI == null)
		{
			return false;
		}
		return !sMI.HasSongBuff();
	}
}
