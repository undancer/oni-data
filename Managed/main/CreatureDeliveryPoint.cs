using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class CreatureDeliveryPoint : StateMachineComponent<CreatureDeliveryPoint.SMInstance>, IUserControlledCapacity
{
	public class SMInstance : GameStateMachine<States, SMInstance, CreatureDeliveryPoint, object>.GameInstance
	{
		public SMInstance(CreatureDeliveryPoint master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, CreatureDeliveryPoint>
	{
		public State waiting;

		public State interact_waiting;

		public State interact_delivery;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = waiting;
			root.Update("RefreshCreatureCount", delegate(SMInstance smi, float dt)
			{
				smi.master.RefreshCreatureCount();
			}, UpdateRate.SIM_1000ms).EventHandler(GameHashes.OnStorageChange, DropAllCreatures);
			waiting.EnterTransition(interact_waiting, (SMInstance smi) => smi.master.playAnimsOnFetch);
			interact_waiting.WorkableStartTransition((SMInstance smi) => smi.master.GetComponent<Storage>(), interact_delivery);
			interact_delivery.PlayAnim("working_pre").QueueAnim("working_pst").OnAnimQueueComplete(interact_waiting);
		}

		public static void DropAllCreatures(SMInstance smi)
		{
			Storage component = smi.master.GetComponent<Storage>();
			if (!component.IsEmpty())
			{
				List<GameObject> items = component.items;
				int count = items.Count;
				Vector3 position = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(smi.transform.GetPosition()), smi.master.spawnOffset), Grid.SceneLayer.Creatures);
				for (int num = count - 1; num >= 0; num--)
				{
					GameObject gameObject = items[num];
					component.Drop(gameObject);
					gameObject.transform.SetPosition(position);
					gameObject.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
				}
				smi.master.RefreshCreatureCount();
			}
		}
	}

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[Serialize]
	private int creatureLimit = 20;

	private int storedCreatureCount;

	public CellOffset[] deliveryOffsets = new CellOffset[1];

	public CellOffset spawnOffset = new CellOffset(0, 0);

	private List<FetchOrder2> fetches;

	private static StatusItem capacityStatusItem;

	public bool playAnimsOnFetch;

	private static readonly EventSystem.IntraObjectHandler<CreatureDeliveryPoint> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<CreatureDeliveryPoint>(delegate(CreatureDeliveryPoint component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CreatureDeliveryPoint> RefreshCreatureCountDelegate = new EventSystem.IntraObjectHandler<CreatureDeliveryPoint>(delegate(CreatureDeliveryPoint component, object data)
	{
		component.RefreshCreatureCount(data);
	});

	private Tag[] requiredFetchTags = new Tag[1]
	{
		GameTags.Creatures.Deliverable
	};

	float IUserControlledCapacity.UserMaxCapacity
	{
		get
		{
			return creatureLimit;
		}
		set
		{
			creatureLimit = Mathf.RoundToInt(value);
			RebalanceFetches();
		}
	}

	float IUserControlledCapacity.AmountStored => storedCreatureCount;

	float IUserControlledCapacity.MinCapacity => 0f;

	float IUserControlledCapacity.MaxCapacity => 20f;

	bool IUserControlledCapacity.WholeValues => true;

	LocString IUserControlledCapacity.CapacityUnits => UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.UNITS_SUFFIX;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		fetches = new List<FetchOrder2>();
		TreeFilterable component = GetComponent<TreeFilterable>();
		component.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(component.OnFilterChanged, new Action<Tag[]>(OnFilterChanged));
		GetComponent<Storage>().SetOffsets(deliveryOffsets);
		Prioritizable.AddRef(base.gameObject);
		if (capacityStatusItem == null)
		{
			capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			capacityStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				IUserControlledCapacity userControlledCapacity = (IUserControlledCapacity)data;
				string newValue = Util.FormatWholeNumber(Mathf.Floor(userControlledCapacity.AmountStored));
				string newValue2 = Util.FormatWholeNumber(userControlledCapacity.UserMaxCapacity);
				str = str.Replace("{Stored}", newValue).Replace("{Capacity}", newValue2).Replace("{Units}", userControlledCapacity.CapacityUnits);
				return str;
			};
		}
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, capacityStatusItem, this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Subscribe(-905833192, OnCopySettingsDelegate);
		Subscribe(643180843, RefreshCreatureCountDelegate);
		RefreshCreatureCount();
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (!(gameObject == null))
		{
			CreatureDeliveryPoint component = gameObject.GetComponent<CreatureDeliveryPoint>();
			if (!(component == null))
			{
				creatureLimit = component.creatureLimit;
				RebalanceFetches();
			}
		}
	}

	private void OnFilterChanged(Tag[] tags)
	{
		ClearFetches();
		RebalanceFetches();
	}

	private void RefreshCreatureCount(object data = null)
	{
		int cell = Grid.PosToCell(this);
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
		int num = storedCreatureCount;
		storedCreatureCount = 0;
		if (cavityForCell != null)
		{
			foreach (KPrefabID creature in cavityForCell.creatures)
			{
				if (!creature.HasTag(GameTags.Creatures.Bagged) && !creature.HasTag(GameTags.Trapped))
				{
					storedCreatureCount++;
				}
			}
		}
		if (storedCreatureCount != num)
		{
			RebalanceFetches();
		}
	}

	private void ClearFetches()
	{
		for (int num = fetches.Count - 1; num >= 0; num--)
		{
			fetches[num].Cancel("clearing all fetches");
		}
		fetches.Clear();
	}

	private void RebalanceFetches()
	{
		Tag[] tags = GetComponent<TreeFilterable>().GetTags();
		ChoreType creatureFetch = Db.Get().ChoreTypes.CreatureFetch;
		Storage component = GetComponent<Storage>();
		int num = creatureLimit - storedCreatureCount;
		_ = fetches.Count;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		for (int num6 = fetches.Count - 1; num6 >= 0; num6--)
		{
			if (fetches[num6].IsComplete())
			{
				fetches.RemoveAt(num6);
				num2++;
			}
		}
		int num7 = 0;
		for (int i = 0; i < fetches.Count; i++)
		{
			if (!fetches[i].InProgress)
			{
				num7++;
			}
		}
		if (num7 == 0 && fetches.Count < num)
		{
			FetchOrder2 fetchOrder = new FetchOrder2(creatureFetch, tags, requiredFetchTags, null, component, 1f, FetchOrder2.OperationalRequirement.Operational);
			fetchOrder.Submit(OnFetchComplete, check_storage_contents: false, OnFetchBegun);
			fetches.Add(fetchOrder);
			num3++;
		}
		int num8 = fetches.Count - num;
		int num9 = fetches.Count - 1;
		while (num9 >= 0 && num8 > 0)
		{
			if (!fetches[num9].InProgress)
			{
				fetches[num9].Cancel("fewer creatures in room");
				fetches.RemoveAt(num9);
				num8--;
				num4++;
			}
			num9--;
		}
		while (num8 > 0 && fetches.Count > 0)
		{
			fetches[fetches.Count - 1].Cancel("fewer creatures in room");
			fetches.RemoveAt(fetches.Count - 1);
			num8--;
			num5++;
		}
	}

	private void OnFetchComplete(FetchOrder2 fetchOrder, Pickupable fetchedItem)
	{
		RebalanceFetches();
	}

	private void OnFetchBegun(FetchOrder2 fetchOrder, Pickupable fetchedItem)
	{
		RebalanceFetches();
	}

	protected override void OnCleanUp()
	{
		base.smi.StopSM("OnCleanUp");
		TreeFilterable component = GetComponent<TreeFilterable>();
		component.OnFilterChanged = (Action<Tag[]>)Delegate.Remove(component.OnFilterChanged, new Action<Tag[]>(OnFilterChanged));
		base.OnCleanUp();
	}
}
