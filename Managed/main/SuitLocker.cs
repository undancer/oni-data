using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SuitLocker : StateMachineComponent<SuitLocker.StatesInstance>
{
	[AddComponentMenu("KMonoBehaviour/Workable/ReturnSuitWorkable")]
	public class ReturnSuitWorkable : Workable
	{
		public static readonly Chore.Precondition DoesSuitNeedRechargingUrgent;

		public static readonly Chore.Precondition DoesSuitNeedRechargingIdle;

		public Chore.Precondition HasSuitMarker;

		public Chore.Precondition SuitTypeMatchesLocker;

		private WorkChore<ReturnSuitWorkable> urgentChore;

		private WorkChore<ReturnSuitWorkable> idleChore;

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			resetProgressOnStop = true;
			workTime = 0.25f;
			synchronizeAnims = false;
		}

		public void CreateChore()
		{
			if (urgentChore == null)
			{
				SuitLocker component = GetComponent<SuitLocker>();
				urgentChore = new WorkChore<ReturnSuitWorkable>(Db.Get().ChoreTypes.ReturnSuitUrgent, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.personalNeeds, 5, ignore_building_assignment: false, add_to_daily_report: false);
				urgentChore.AddPrecondition(DoesSuitNeedRechargingUrgent);
				urgentChore.AddPrecondition(HasSuitMarker, component);
				urgentChore.AddPrecondition(SuitTypeMatchesLocker, component);
				idleChore = new WorkChore<ReturnSuitWorkable>(Db.Get().ChoreTypes.ReturnSuitIdle, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.idle, 5, ignore_building_assignment: false, add_to_daily_report: false);
				idleChore.AddPrecondition(DoesSuitNeedRechargingIdle);
				idleChore.AddPrecondition(HasSuitMarker, component);
				idleChore.AddPrecondition(SuitTypeMatchesLocker, component);
			}
		}

		public void CancelChore()
		{
			if (urgentChore != null)
			{
				urgentChore.Cancel("ReturnSuitWorkable.CancelChore");
				urgentChore = null;
			}
			if (idleChore != null)
			{
				idleChore.Cancel("ReturnSuitWorkable.CancelChore");
				idleChore = null;
			}
		}

		protected override void OnStartWork(Worker worker)
		{
			ShowProgressBar(show: false);
		}

		protected override bool OnWorkTick(Worker worker, float dt)
		{
			return true;
		}

		protected override void OnCompleteWork(Worker worker)
		{
			Equipment equipment = worker.GetComponent<MinionIdentity>().GetEquipment();
			if (equipment.IsSlotOccupied(Db.Get().AssignableSlots.Suit))
			{
				if (GetComponent<SuitLocker>().CanDropOffSuit())
				{
					GetComponent<SuitLocker>().UnequipFrom(equipment);
				}
				else
				{
					equipment.GetAssignable(Db.Get().AssignableSlots.Suit).Unassign();
				}
			}
			if (urgentChore != null)
			{
				CancelChore();
				CreateChore();
			}
		}

		public override HashedString[] GetWorkAnims(Worker worker)
		{
			return new HashedString[1]
			{
				new HashedString("none")
			};
		}

		public ReturnSuitWorkable()
		{
			Chore.Precondition hasSuitMarker = new Chore.Precondition
			{
				id = "IsValid",
				description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_SUIT_MARKER,
				fn = delegate(ref Chore.Precondition.Context context, object data)
				{
					return ((SuitLocker)data).suitMarkerState == SuitMarkerState.HasMarker;
				}
			};
			HasSuitMarker = hasSuitMarker;
			hasSuitMarker = (SuitTypeMatchesLocker = new Chore.Precondition
			{
				id = "IsValid",
				description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_SUIT_MARKER,
				fn = delegate(ref Chore.Precondition.Context context, object data)
				{
					SuitLocker suitLocker = (SuitLocker)data;
					Equipment equipment = context.consumerState.equipment;
					if (equipment == null)
					{
						return false;
					}
					AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Suit);
					if (slot.assignable == null)
					{
						return false;
					}
					bool num = slot.assignable.GetComponent<JetSuitTank>() != null;
					bool flag = suitLocker.GetComponent<JetSuitLocker>() != null;
					return num == flag;
				}
			});
			base._002Ector();
		}

		static ReturnSuitWorkable()
		{
			Chore.Precondition precondition = new Chore.Precondition
			{
				id = "DoesSuitNeedRechargingUrgent",
				description = DUPLICANTS.CHORES.PRECONDITIONS.DOES_SUIT_NEED_RECHARGING_URGENT,
				fn = delegate(ref Chore.Precondition.Context context, object data)
				{
					Equipment equipment2 = context.consumerState.equipment;
					if (equipment2 == null)
					{
						return false;
					}
					AssignableSlotInstance slot2 = equipment2.GetSlot(Db.Get().AssignableSlots.Suit);
					if (slot2.assignable == null)
					{
						return false;
					}
					SuitTank component = slot2.assignable.GetComponent<SuitTank>();
					if (component != null && component.NeedsRecharging())
					{
						return true;
					}
					JetSuitTank component2 = slot2.assignable.GetComponent<JetSuitTank>();
					if (component2 != null && component2.NeedsRecharging())
					{
						return true;
					}
					LeadSuitTank component3 = slot2.assignable.GetComponent<LeadSuitTank>();
					return (component3 != null && component3.NeedsRecharging()) ? true : false;
				}
			};
			DoesSuitNeedRechargingUrgent = precondition;
			precondition = new Chore.Precondition
			{
				id = "DoesSuitNeedRechargingIdle",
				description = DUPLICANTS.CHORES.PRECONDITIONS.DOES_SUIT_NEED_RECHARGING_IDLE,
				fn = delegate(ref Chore.Precondition.Context context, object data)
				{
					Equipment equipment = context.consumerState.equipment;
					if (equipment == null)
					{
						return false;
					}
					AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Suit);
					if (slot.assignable == null)
					{
						return false;
					}
					if (slot.assignable.GetComponent<SuitTank>() != null)
					{
						return true;
					}
					if (slot.assignable.GetComponent<JetSuitTank>() != null)
					{
						return true;
					}
					return (slot.assignable.GetComponent<LeadSuitTank>() != null) ? true : false;
				}
			};
			DoesSuitNeedRechargingIdle = precondition;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, SuitLocker, object>.GameInstance
	{
		public StatesInstance(SuitLocker suit_locker)
			: base(suit_locker)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SuitLocker>
	{
		public class ChargingStates : State
		{
			public State pre;

			public State pst;

			public State operational;

			public State nooxygen;

			public State notoperational;
		}

		public class EmptyStates : State
		{
			public State configured;

			public State notconfigured;
		}

		public EmptyStates empty;

		public ChargingStates charging;

		public State waitingforsuit;

		public State suitfullycharged;

		public BoolParameter isWaitingForSuit;

		public BoolParameter isConfigured;

		public BoolParameter hasSuitMarker;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = empty;
			base.serializable = SerializeType.Both_DEPRECATED;
			root.Update("RefreshMeter", delegate(StatesInstance smi, float dt)
			{
				smi.master.RefreshMeter();
			}, UpdateRate.RENDER_200ms);
			empty.DefaultState(empty.notconfigured).EventTransition(GameHashes.OnStorageChange, charging, (StatesInstance smi) => smi.master.GetStoredOutfit() != null).ParamTransition(isWaitingForSuit, waitingforsuit, GameStateMachine<States, StatesInstance, SuitLocker, object>.IsTrue)
				.Enter("CreateReturnSuitChore", delegate(StatesInstance smi)
				{
					smi.master.returnSuitWorkable.CreateChore();
				})
				.RefreshUserMenuOnEnter()
				.Exit("CancelReturnSuitChore", delegate(StatesInstance smi)
				{
					smi.master.returnSuitWorkable.CancelChore();
				})
				.PlayAnim("no_suit_pre")
				.QueueAnim("no_suit");
			empty.notconfigured.ParamTransition(isConfigured, empty.configured, GameStateMachine<States, StatesInstance, SuitLocker, object>.IsTrue).ToggleStatusItem(BUILDING.STATUSITEMS.SUIT_LOCKER_NEEDS_CONFIGURATION.NAME, BUILDING.STATUSITEMS.SUIT_LOCKER_NEEDS_CONFIGURATION.TOOLTIP, "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
			empty.configured.RefreshUserMenuOnEnter().ToggleStatusItem(BUILDING.STATUSITEMS.SUIT_LOCKER.READY.NAME, BUILDING.STATUSITEMS.SUIT_LOCKER.READY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
			waitingforsuit.EventTransition(GameHashes.OnStorageChange, charging, (StatesInstance smi) => smi.master.GetStoredOutfit() != null).Enter("CreateFetchChore", delegate(StatesInstance smi)
			{
				smi.master.CreateFetchChore();
			}).ParamTransition(isWaitingForSuit, empty, GameStateMachine<States, StatesInstance, SuitLocker, object>.IsFalse)
				.RefreshUserMenuOnEnter()
				.PlayAnim("no_suit_pst")
				.QueueAnim("awaiting_suit")
				.Exit("ClearIsWaitingForSuit", delegate(StatesInstance smi)
				{
					isWaitingForSuit.Set(value: false, smi);
				})
				.Exit("CancelFetchChore", delegate(StatesInstance smi)
				{
					smi.master.CancelFetchChore();
				})
				.ToggleStatusItem(BUILDING.STATUSITEMS.SUIT_LOCKER.SUIT_REQUESTED.NAME, BUILDING.STATUSITEMS.SUIT_LOCKER.SUIT_REQUESTED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
			charging.DefaultState(charging.pre).RefreshUserMenuOnEnter().EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.master.GetStoredOutfit() == null)
				.ToggleStatusItem(Db.Get().MiscStatusItems.StoredItemDurability, (StatesInstance smi) => smi.master.GetStoredOutfit().gameObject);
			charging.pre.Enter(delegate(StatesInstance smi)
			{
				if (smi.master.IsSuitFullyCharged())
				{
					smi.GoTo(suitfullycharged);
				}
				else
				{
					smi.GetComponent<KBatchedAnimController>().Play("no_suit_pst");
					smi.GetComponent<KBatchedAnimController>().Queue("charging_pre");
				}
			}).OnAnimQueueComplete(charging.operational);
			charging.operational.TagTransition(GameTags.Operational, charging.notoperational, on_remove: true).Transition(charging.nooxygen, (StatesInstance smi) => !smi.master.HasOxygen()).PlayAnim("charging_loop", KAnim.PlayMode.Loop)
				.Enter("SetActive", delegate(StatesInstance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(value: true);
				})
				.Transition(charging.pst, (StatesInstance smi) => smi.master.IsSuitFullyCharged())
				.Update("ChargeSuit", delegate(StatesInstance smi, float dt)
				{
					smi.master.ChargeSuit(dt);
				})
				.Exit("ClearActive", delegate(StatesInstance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(value: false);
				})
				.ToggleStatusItem(BUILDING.STATUSITEMS.SUIT_LOCKER.CHARGING.NAME, BUILDING.STATUSITEMS.SUIT_LOCKER.CHARGING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
			charging.nooxygen.TagTransition(GameTags.Operational, charging.notoperational, on_remove: true).Transition(charging.operational, (StatesInstance smi) => smi.master.HasOxygen()).Transition(charging.pst, (StatesInstance smi) => smi.master.IsSuitFullyCharged())
				.PlayAnim("no_o2_loop", KAnim.PlayMode.Loop)
				.ToggleStatusItem(BUILDING.STATUSITEMS.SUIT_LOCKER.NO_OXYGEN.NAME, BUILDING.STATUSITEMS.SUIT_LOCKER.NO_OXYGEN.TOOLTIP, "status_item_suit_locker_no_oxygen", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
			charging.notoperational.TagTransition(GameTags.Operational, charging.operational).PlayAnim("not_charging_loop", KAnim.PlayMode.Loop).Transition(charging.pst, (StatesInstance smi) => smi.master.IsSuitFullyCharged())
				.ToggleStatusItem(BUILDING.STATUSITEMS.SUIT_LOCKER.NOT_OPERATIONAL.NAME, BUILDING.STATUSITEMS.SUIT_LOCKER.NOT_OPERATIONAL.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
			charging.pst.PlayAnim("charging_pst").OnAnimQueueComplete(suitfullycharged);
			suitfullycharged.EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.master.GetStoredOutfit() == null).PlayAnim("has_suit").RefreshUserMenuOnEnter()
				.ToggleStatusItem(Db.Get().MiscStatusItems.StoredItemDurability, (StatesInstance smi) => smi.master.GetStoredOutfit().gameObject)
				.ToggleStatusItem(BUILDING.STATUSITEMS.SUIT_LOCKER.FULLY_CHARGED.NAME, BUILDING.STATUSITEMS.SUIT_LOCKER.FULLY_CHARGED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		}
	}

	private enum SuitMarkerState
	{
		HasMarker,
		NoMarker,
		WrongSide,
		NotOperational
	}

	private struct SuitLockerEntry
	{
		public class Comparer : IComparer<SuitLockerEntry>
		{
			public int Compare(SuitLockerEntry a, SuitLockerEntry b)
			{
				return a.cell - b.cell;
			}
		}

		public SuitLocker suitLocker;

		public int cell;

		public static Comparer comparer = new Comparer();
	}

	private struct SuitMarkerEntry
	{
		public SuitMarker suitMarker;

		public int cell;
	}

	[MyCmpGet]
	private Building building;

	public Tag[] OutfitTags;

	private FetchChore fetchChore;

	[MyCmpAdd]
	public ReturnSuitWorkable returnSuitWorkable;

	private MeterController meter;

	private SuitMarkerState suitMarkerState;

	public float OxygenAvailable
	{
		get
		{
			KPrefabID storedOutfit = GetStoredOutfit();
			if (storedOutfit == null)
			{
				return 0f;
			}
			return storedOutfit.GetComponent<SuitTank>().PercentFull();
		}
	}

	public float BatteryAvailable
	{
		get
		{
			KPrefabID storedOutfit = GetStoredOutfit();
			if (storedOutfit == null)
			{
				return 0f;
			}
			return storedOutfit.GetComponent<LeadSuitTank>().batteryCharge;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_arrow", "meter_scale");
		UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
		base.smi.StartSM();
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits);
	}

	public KPrefabID GetStoredOutfit()
	{
		foreach (GameObject item in GetComponent<Storage>().items)
		{
			if (!(item == null))
			{
				KPrefabID component = item.GetComponent<KPrefabID>();
				if (!(component == null) && component.HasAnyTags(OutfitTags))
				{
					return component;
				}
			}
		}
		return null;
	}

	public float GetSuitScore()
	{
		float num = -1f;
		KPrefabID partiallyChargedOutfit = GetPartiallyChargedOutfit();
		if ((bool)partiallyChargedOutfit)
		{
			num = partiallyChargedOutfit.GetComponent<SuitTank>().PercentFull();
			JetSuitTank component = partiallyChargedOutfit.GetComponent<JetSuitTank>();
			if ((bool)component && component.PercentFull() < num)
			{
				num = component.PercentFull();
			}
		}
		return num;
	}

	public KPrefabID GetPartiallyChargedOutfit()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if ((bool)storedOutfit)
		{
			if (storedOutfit.GetComponent<SuitTank>().PercentFull() < TUNING.EQUIPMENT.SUITS.MINIMUM_USABLE_SUIT_CHARGE)
			{
				return null;
			}
			JetSuitTank component = storedOutfit.GetComponent<JetSuitTank>();
			if ((bool)component && component.PercentFull() < TUNING.EQUIPMENT.SUITS.MINIMUM_USABLE_SUIT_CHARGE)
			{
				return null;
			}
			return storedOutfit;
		}
		return null;
	}

	public KPrefabID GetFullyChargedOutfit()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if ((bool)storedOutfit)
		{
			if (!storedOutfit.GetComponent<SuitTank>().IsFull())
			{
				return null;
			}
			JetSuitTank component = storedOutfit.GetComponent<JetSuitTank>();
			if ((bool)component && !component.IsFull())
			{
				return null;
			}
			return storedOutfit;
		}
		return null;
	}

	private void CreateFetchChore()
	{
		fetchChore = new FetchChore(Db.Get().ChoreTypes.EquipmentFetch, GetComponent<Storage>(), 1f, OutfitTags, null, new Tag[1]
		{
			GameTags.Assigned
		}, null, run_until_complete: true, null, null, null, FetchOrder2.OperationalRequirement.None);
		fetchChore.allowMultifetch = false;
	}

	private void CancelFetchChore()
	{
		if (fetchChore != null)
		{
			fetchChore.Cancel("SuitLocker.CancelFetchChore");
			fetchChore = null;
		}
	}

	public bool HasOxygen()
	{
		GameObject oxygen = GetOxygen();
		if (oxygen != null)
		{
			return oxygen.GetComponent<PrimaryElement>().Mass > 0f;
		}
		return false;
	}

	private void RefreshMeter()
	{
		GameObject oxygen = GetOxygen();
		float positionPercent = 0f;
		if (oxygen != null)
		{
			positionPercent = oxygen.GetComponent<PrimaryElement>().Mass / GetComponent<ConduitConsumer>().capacityKG;
			positionPercent = Math.Min(positionPercent, 1f);
		}
		meter.SetPositionPercent(positionPercent);
	}

	public bool IsSuitFullyCharged()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (storedOutfit != null)
		{
			SuitTank component = storedOutfit.GetComponent<SuitTank>();
			if (component != null && component.PercentFull() < 1f)
			{
				return false;
			}
			JetSuitTank component2 = storedOutfit.GetComponent<JetSuitTank>();
			if (component2 != null && component2.PercentFull() < 1f)
			{
				return false;
			}
			LeadSuitTank leadSuitTank = ((storedOutfit != null) ? storedOutfit.GetComponent<LeadSuitTank>() : null);
			if (leadSuitTank != null && leadSuitTank.PercentFull() < 1f)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public bool IsOxygenTankFull()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (storedOutfit != null)
		{
			SuitTank component = storedOutfit.GetComponent<SuitTank>();
			if (component == null)
			{
				return true;
			}
			return component.PercentFull() >= 1f;
		}
		return false;
	}

	private void OnRequestOutfit()
	{
		base.smi.sm.isWaitingForSuit.Set(value: true, base.smi);
	}

	private void OnCancelRequest()
	{
		base.smi.sm.isWaitingForSuit.Set(value: false, base.smi);
	}

	public void DropSuit()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (!(storedOutfit == null))
		{
			GetComponent<Storage>().Drop(storedOutfit.gameObject);
		}
	}

	public void EquipTo(Equipment equipment)
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (!(storedOutfit == null))
		{
			GetComponent<Storage>().Drop(storedOutfit.gameObject);
			storedOutfit.GetComponent<Equippable>().Assign(equipment.GetComponent<IAssignableIdentity>());
			storedOutfit.GetComponent<EquippableWorkable>().CancelChore();
			equipment.Equip(storedOutfit.GetComponent<Equippable>());
			returnSuitWorkable.CreateChore();
		}
	}

	public void UnequipFrom(Equipment equipment)
	{
		Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
		assignable.Unassign();
		GetComponent<Storage>().Store(assignable.gameObject);
		Durability component = assignable.GetComponent<Durability>();
		if (component != null && component.IsWornOut())
		{
			ConfigRequestSuit();
		}
	}

	public void ConfigRequestSuit()
	{
		base.smi.sm.isConfigured.Set(value: true, base.smi);
		base.smi.sm.isWaitingForSuit.Set(value: true, base.smi);
	}

	public void ConfigNoSuit()
	{
		base.smi.sm.isConfigured.Set(value: true, base.smi);
		base.smi.sm.isWaitingForSuit.Set(value: false, base.smi);
	}

	public bool CanDropOffSuit()
	{
		if (base.smi.sm.isConfigured.Get(base.smi) && !base.smi.sm.isWaitingForSuit.Get(base.smi))
		{
			return GetStoredOutfit() == null;
		}
		return false;
	}

	private GameObject GetOxygen()
	{
		return GetComponent<Storage>().FindFirst(GameTags.Oxygen);
	}

	private void ChargeSuit(float dt)
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (storedOutfit == null)
		{
			return;
		}
		GameObject oxygen = GetOxygen();
		if (!(oxygen == null))
		{
			SuitTank component = storedOutfit.GetComponent<SuitTank>();
			float a = component.capacity * 15f * dt / 600f;
			a = Mathf.Min(a, component.capacity - component.GetTankAmount());
			a = Mathf.Min(oxygen.GetComponent<PrimaryElement>().Mass, a);
			if (a > 0f)
			{
				GetComponent<Storage>().Transfer(component.storage, component.elementTag, a, block_events: false, hide_popups: true);
			}
		}
	}

	public void SetSuitMarker(SuitMarker suit_marker)
	{
		SuitMarkerState suitMarkerState = SuitMarkerState.HasMarker;
		if (suit_marker == null)
		{
			suitMarkerState = SuitMarkerState.NoMarker;
		}
		else if (suit_marker.transform.GetPosition().x > base.transform.GetPosition().x && suit_marker.GetComponent<Rotatable>().IsRotated)
		{
			suitMarkerState = SuitMarkerState.WrongSide;
		}
		else if (suit_marker.transform.GetPosition().x < base.transform.GetPosition().x && !suit_marker.GetComponent<Rotatable>().IsRotated)
		{
			suitMarkerState = SuitMarkerState.WrongSide;
		}
		else if (!suit_marker.GetComponent<Operational>().IsOperational)
		{
			suitMarkerState = SuitMarkerState.NotOperational;
		}
		if (suitMarkerState != this.suitMarkerState)
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoSuitMarker);
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerWrongSide);
			switch (suitMarkerState)
			{
			case SuitMarkerState.NoMarker:
				GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSuitMarker);
				break;
			case SuitMarkerState.WrongSide:
				GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerWrongSide);
				break;
			}
			this.suitMarkerState = suitMarkerState;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), null);
	}

	private static void GatherSuitBuildings(int cell, int dir, List<SuitLockerEntry> suit_lockers, List<SuitMarkerEntry> suit_markers)
	{
		int num = dir;
		while (true)
		{
			int cell2 = Grid.OffsetCell(cell, num, 0);
			if (!Grid.IsValidCell(cell2) || GatherSuitBuildingsOnCell(cell2, suit_lockers, suit_markers))
			{
				num += dir;
				continue;
			}
			break;
		}
	}

	private static bool GatherSuitBuildingsOnCell(int cell, List<SuitLockerEntry> suit_lockers, List<SuitMarkerEntry> suit_markers)
	{
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject == null)
		{
			return false;
		}
		SuitMarker component = gameObject.GetComponent<SuitMarker>();
		if (component != null)
		{
			suit_markers.Add(new SuitMarkerEntry
			{
				suitMarker = component,
				cell = cell
			});
			return true;
		}
		SuitLocker component2 = gameObject.GetComponent<SuitLocker>();
		if (component2 != null)
		{
			suit_lockers.Add(new SuitLockerEntry
			{
				suitLocker = component2,
				cell = cell
			});
			return true;
		}
		return false;
	}

	private static SuitMarker FindSuitMarker(int cell, List<SuitMarkerEntry> suit_markers)
	{
		if (!Grid.IsValidCell(cell))
		{
			return null;
		}
		foreach (SuitMarkerEntry suit_marker in suit_markers)
		{
			if (suit_marker.cell == cell)
			{
				return suit_marker.suitMarker;
			}
		}
		return null;
	}

	public static void UpdateSuitMarkerStates(int cell, GameObject self)
	{
		ListPool<SuitLockerEntry, SuitLocker>.PooledList pooledList = ListPool<SuitLockerEntry, SuitLocker>.Allocate();
		ListPool<SuitMarkerEntry, SuitLocker>.PooledList pooledList2 = ListPool<SuitMarkerEntry, SuitLocker>.Allocate();
		if (self != null)
		{
			SuitLocker component = self.GetComponent<SuitLocker>();
			if (component != null)
			{
				pooledList.Add(new SuitLockerEntry
				{
					suitLocker = component,
					cell = cell
				});
			}
			SuitMarker component2 = self.GetComponent<SuitMarker>();
			if (component2 != null)
			{
				pooledList2.Add(new SuitMarkerEntry
				{
					suitMarker = component2,
					cell = cell
				});
			}
		}
		GatherSuitBuildings(cell, 1, pooledList, pooledList2);
		GatherSuitBuildings(cell, -1, pooledList, pooledList2);
		pooledList.Sort(SuitLockerEntry.comparer);
		for (int i = 0; i < pooledList.Count; i++)
		{
			SuitLockerEntry suitLockerEntry = pooledList[i];
			SuitLockerEntry suitLockerEntry2 = suitLockerEntry;
			ListPool<SuitLockerEntry, SuitLocker>.PooledList pooledList3 = ListPool<SuitLockerEntry, SuitLocker>.Allocate();
			pooledList3.Add(suitLockerEntry);
			for (int j = i + 1; j < pooledList.Count; j++)
			{
				SuitLockerEntry suitLockerEntry3 = pooledList[j];
				if (Grid.CellRight(suitLockerEntry2.cell) != suitLockerEntry3.cell)
				{
					break;
				}
				i++;
				suitLockerEntry2 = suitLockerEntry3;
				pooledList3.Add(suitLockerEntry3);
			}
			int cell2 = Grid.CellLeft(suitLockerEntry.cell);
			int cell3 = Grid.CellRight(suitLockerEntry2.cell);
			SuitMarker suitMarker = FindSuitMarker(cell2, pooledList2);
			if (suitMarker == null)
			{
				suitMarker = FindSuitMarker(cell3, pooledList2);
			}
			foreach (SuitLockerEntry item in pooledList3)
			{
				item.suitLocker.SetSuitMarker(suitMarker);
			}
			pooledList3.Recycle();
		}
		pooledList.Recycle();
		pooledList2.Recycle();
	}
}
