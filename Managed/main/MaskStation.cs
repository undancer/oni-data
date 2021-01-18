using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MaskStation : StateMachineComponent<MaskStation.SMInstance>, IBasicBuilding
{
	private class OxygenMaskReactable : Reactable
	{
		private MaskStation maskStation;

		private float startTime;

		public OxygenMaskReactable(MaskStation mask_station)
			: base(mask_station.gameObject, "OxygenMask", Db.Get().ChoreTypes.SuitMarker, 1, 1)
		{
			maskStation = mask_station;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (reactor != null)
			{
				return false;
			}
			if (maskStation == null)
			{
				Cleanup();
				return false;
			}
			Equipment equipment = new_reactor.GetComponent<MinionIdentity>().GetEquipment();
			bool flag = !equipment.IsSlotOccupied(Db.Get().AssignableSlots.Suit);
			int x = transition.navGridTransition.x;
			if (x == 0)
			{
				return false;
			}
			if (!flag)
			{
				if (x < 0 && maskStation.isRotated)
				{
					return false;
				}
				if (x > 0 && !maskStation.isRotated)
				{
					return false;
				}
				return true;
			}
			if (!maskStation.smi.IsReady())
			{
				return false;
			}
			if (x > 0 && maskStation.isRotated)
			{
				return false;
			}
			if (x < 0 && !maskStation.isRotated)
			{
				return false;
			}
			return true;
		}

		protected override void InternalBegin()
		{
			startTime = Time.time;
			KBatchedAnimController component = reactor.GetComponent<KBatchedAnimController>();
			component.AddAnimOverrides(maskStation.interactAnim, 1f);
			component.Play("working_pre");
			component.Queue("working_loop");
			component.Queue("working_pst");
			maskStation.CreateNewReactable();
		}

		public override void Update(float dt)
		{
			Facing facing = (reactor ? reactor.GetComponent<Facing>() : null);
			if ((bool)facing && (bool)maskStation)
			{
				facing.SetFacing(maskStation.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH);
			}
			if (Time.time - startTime > 2.8f)
			{
				Run();
				Cleanup();
			}
		}

		private void Run()
		{
			GameObject reactor = base.reactor;
			Equipment equipment = reactor.GetComponent<MinionIdentity>().GetEquipment();
			bool flag = !equipment.IsSlotOccupied(Db.Get().AssignableSlots.Suit);
			Navigator component = reactor.GetComponent<Navigator>();
			bool flag2 = component != null && (component.flags & maskStation.PathFlag) != 0;
			if (flag)
			{
				if (!maskStation.smi.IsReady())
				{
					return;
				}
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("Oxygen_Mask".ToTag()));
				gameObject.SetActive(value: true);
				List<GameObject> possibleMaterials = maskStation.GetPossibleMaterials();
				SimHashes elementID = possibleMaterials[0].GetComponent<PrimaryElement>().ElementID;
				gameObject.GetComponent<PrimaryElement>().SetElement(elementID, addTags: false);
				SuitTank component2 = gameObject.GetComponent<SuitTank>();
				maskStation.materialStorage.ConsumeIgnoringDisease(maskStation.materialTag, maskStation.materialConsumedPerMask);
				maskStation.oxygenStorage.Transfer(component2.storage, component2.elementTag, maskStation.oxygenConsumedPerMask, block_events: false, hide_popups: true);
				Equippable component3 = gameObject.GetComponent<Equippable>();
				component3.Assign(equipment.GetComponent<IAssignableIdentity>());
				component3.isEquipped = true;
			}
			if (flag)
			{
				return;
			}
			Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
			assignable.Unassign();
			if (!flag2)
			{
				Notification notification = new Notification(MISC.NOTIFICATIONS.SUIT_DROPPED.NAME, NotificationType.BadMinor, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.SUIT_DROPPED.TOOLTIP);
				assignable.GetComponent<Notifier>().Add(notification);
			}
		}

		protected override void InternalEnd()
		{
			if (reactor != null)
			{
				reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(maskStation.interactAnim);
			}
		}

		protected override void InternalCleanup()
		{
		}
	}

	public class SMInstance : GameStateMachine<States, SMInstance, MaskStation, object>.GameInstance
	{
		public SMInstance(MaskStation master)
			: base(master)
		{
		}

		private bool HasSufficientMaterials()
		{
			return base.master.GetTotalMaterialAmount() >= base.master.materialConsumedPerMask;
		}

		private bool HasSufficientOxygen()
		{
			return base.master.GetTotalOxygenAmount() >= base.master.oxygenConsumedPerMask;
		}

		public bool OxygenIsFull()
		{
			return base.master.GetTotalOxygenAmount() >= base.master.oxygenConsumedPerMask * (float)base.master.maxUses;
		}

		public bool IsReady()
		{
			if (!HasSufficientMaterials() || !HasSufficientOxygen())
			{
				return false;
			}
			return true;
		}
	}

	public class States : GameStateMachine<States, SMInstance, MaskStation>
	{
		public class ChargingStates : State
		{
			public State opening;

			public State open;

			public State closing;

			public State closed;

			public State openChargingPre;

			public State closedChargingPre;
		}

		public class NotChargingStates : State
		{
			public State opening;

			public State open;

			public State closing;

			public State closed;

			public State openChargingPst;

			public State closedChargingPst;
		}

		public State notOperational;

		public ChargingStates charging;

		public NotChargingStates notCharging;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = notOperational;
			notOperational.PlayAnim("off").TagTransition(GameTags.Operational, charging);
			charging.TagTransition(GameTags.Operational, notOperational, on_remove: true).EventTransition(GameHashes.OnStorageChange, notCharging, (SMInstance smi) => smi.OxygenIsFull() || !smi.master.shouldPump).Update(delegate(SMInstance smi, float dt)
			{
				smi.master.UpdateOperational();
			}, UpdateRate.SIM_1000ms)
				.Enter(delegate(SMInstance smi)
				{
					if (smi.OxygenIsFull() || !smi.master.shouldPump)
					{
						smi.GoTo(notCharging);
					}
					else if (smi.IsReady())
					{
						smi.GoTo(charging.openChargingPre);
					}
					else
					{
						smi.GoTo(charging.closedChargingPre);
					}
				});
			charging.opening.QueueAnim("opening_charging").OnAnimQueueComplete(charging.open);
			charging.open.PlayAnim("open_charging_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OnStorageChange, charging.closing, (SMInstance smi) => !smi.IsReady());
			charging.closing.QueueAnim("closing_charging").OnAnimQueueComplete(charging.closed);
			charging.closed.PlayAnim("closed_charging_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OnStorageChange, charging.opening, (SMInstance smi) => smi.IsReady());
			charging.openChargingPre.PlayAnim("open_charging_pre").OnAnimQueueComplete(charging.open);
			charging.closedChargingPre.PlayAnim("closed_charging_pre").OnAnimQueueComplete(charging.closed);
			notCharging.TagTransition(GameTags.Operational, notOperational, on_remove: true).EventTransition(GameHashes.OnStorageChange, charging, (SMInstance smi) => !smi.OxygenIsFull() && smi.master.shouldPump).Update(delegate(SMInstance smi, float dt)
			{
				smi.master.UpdateOperational();
			}, UpdateRate.SIM_1000ms)
				.Enter(delegate(SMInstance smi)
				{
					if (!smi.OxygenIsFull() && smi.master.shouldPump)
					{
						smi.GoTo(charging);
					}
					else if (smi.IsReady())
					{
						smi.GoTo(notCharging.openChargingPst);
					}
					else
					{
						smi.GoTo(notCharging.closedChargingPst);
					}
				});
			notCharging.opening.PlayAnim("opening_not_charging").OnAnimQueueComplete(notCharging.open);
			notCharging.open.PlayAnim("open_not_charging_loop").EventTransition(GameHashes.OnStorageChange, notCharging.closing, (SMInstance smi) => !smi.IsReady());
			notCharging.closing.PlayAnim("closing_not_charging").OnAnimQueueComplete(notCharging.closed);
			notCharging.closed.PlayAnim("closed_not_charging_loop").EventTransition(GameHashes.OnStorageChange, notCharging.opening, (SMInstance smi) => smi.IsReady());
			notCharging.openChargingPst.PlayAnim("open_charging_pst").OnAnimQueueComplete(notCharging.open);
			notCharging.closedChargingPst.PlayAnim("closed_charging_pst").OnAnimQueueComplete(notCharging.closed);
		}
	}

	private static readonly EventSystem.IntraObjectHandler<MaskStation> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<MaskStation>(delegate(MaskStation component, object data)
	{
		component.OnStorageChange(data);
	});

	private static readonly EventSystem.IntraObjectHandler<MaskStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<MaskStation>(delegate(MaskStation component, object data)
	{
		component.OnOperationalChanged((bool)data);
	});

	private static readonly EventSystem.IntraObjectHandler<MaskStation> OnRotatedDelegate = new EventSystem.IntraObjectHandler<MaskStation>(delegate(MaskStation component, object data)
	{
		component.isRotated = ((Rotatable)data).IsRotated;
	});

	public float materialConsumedPerMask = 1f;

	public float oxygenConsumedPerMask = 1f;

	public Tag materialTag = GameTags.Metal;

	public Tag oxygenTag = GameTags.Breathable;

	public int maxUses = 10;

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private KSelectable selectable;

	public Storage materialStorage;

	public Storage oxygenStorage;

	private bool shouldPump;

	private OxygenMaskReactable reactable;

	private MeterController materialsMeter;

	private MeterController oxygenMeter;

	public Meter.Offset materialsMeterOffset = Meter.Offset.Behind;

	public Meter.Offset oxygenMeterOffset = Meter.Offset.Infront;

	public string choreTypeID;

	protected FilteredStorage filteredStorage;

	public KAnimFile interactAnim = Assets.GetAnim("anim_equip_clothing_kanim");

	private int cell;

	public PathFinder.PotentialPath.Flags PathFlag;

	private Guid noElementStatusGuid;

	private Grid.SuitMarker.Flags gridFlags;

	private bool isRotated
	{
		get
		{
			return (gridFlags & Grid.SuitMarker.Flags.Rotated) != 0;
		}
		set
		{
			UpdateGridFlag(Grid.SuitMarker.Flags.Rotated, value);
		}
	}

	private bool isOperational
	{
		get
		{
			return (gridFlags & Grid.SuitMarker.Flags.Operational) != 0;
		}
		set
		{
			UpdateGridFlag(Grid.SuitMarker.Flags.Operational, value);
		}
	}

	public void UpdateOperational()
	{
		bool flag = GetTotalOxygenAmount() >= oxygenConsumedPerMask * (float)maxUses;
		shouldPump = IsPumpable();
		if (operational.IsOperational && shouldPump && !flag)
		{
			operational.SetActive(value: true);
		}
		else
		{
			operational.SetActive(value: false);
		}
		noElementStatusGuid = selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidMaskStationConsumptionState, noElementStatusGuid, !shouldPump);
	}

	private bool IsPumpable()
	{
		ElementConsumer[] components = GetComponents<ElementConsumer>();
		int num = Grid.PosToCell(base.transform.GetPosition());
		bool result = false;
		ElementConsumer[] array = components;
		foreach (ElementConsumer elementConsumer in array)
		{
			for (int j = 0; j < elementConsumer.consumptionRadius; j++)
			{
				for (int k = 0; k < elementConsumer.consumptionRadius; k++)
				{
					int num2 = num + k + Grid.WidthInCells * j;
					bool flag = Grid.Element[num2].IsState(Element.State.Gas);
					bool flag2 = Grid.Element[num2].id == elementConsumer.elementToConsume;
					if (flag && flag2)
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ChoreType fetch_chore_type = Db.Get().ChoreTypes.Get(choreTypeID);
		filteredStorage = new FilteredStorage(this, null, null, null, use_logic_meter: false, fetch_chore_type);
	}

	private List<GameObject> GetPossibleMaterials()
	{
		List<GameObject> result = new List<GameObject>();
		materialStorage.Find(materialTag, result);
		return result;
	}

	private float GetTotalMaterialAmount()
	{
		return materialStorage.GetMassAvailable(materialTag);
	}

	private float GetTotalOxygenAmount()
	{
		return oxygenStorage.GetMassAvailable(oxygenTag);
	}

	private void RefreshMeters()
	{
		float totalMaterialAmount = GetTotalMaterialAmount();
		totalMaterialAmount = Mathf.Clamp01(totalMaterialAmount / ((float)maxUses * materialConsumedPerMask));
		float totalOxygenAmount = GetTotalOxygenAmount();
		totalOxygenAmount = Mathf.Clamp01(totalOxygenAmount / ((float)maxUses * oxygenConsumedPerMask));
		materialsMeter.SetPositionPercent(totalMaterialAmount);
		oxygenMeter.SetPositionPercent(totalOxygenAmount);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		CreateNewReactable();
		cell = Grid.PosToCell(this);
		Grid.RegisterSuitMarker(cell);
		isOperational = GetComponent<Operational>().IsOperational;
		Subscribe(-592767678, OnOperationalChangedDelegate);
		isRotated = GetComponent<Rotatable>().IsRotated;
		Subscribe(-1643076535, OnRotatedDelegate);
		materialsMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_resources_target", "meter_resources", materialsMeterOffset, Grid.SceneLayer.BuildingBack, "meter_resources_target");
		oxygenMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_oxygen_target", "meter_oxygen", oxygenMeterOffset, Grid.SceneLayer.BuildingFront, "meter_oxygen_target");
		if (filteredStorage != null)
		{
			filteredStorage.FilterChanged();
		}
		Subscribe(-1697596308, OnStorageChangeDelegate);
		RefreshMeters();
	}

	private void Update()
	{
		float a = GetTotalMaterialAmount() / materialConsumedPerMask;
		float b = GetTotalOxygenAmount() / oxygenConsumedPerMask;
		int fullLockerCount = (int)Mathf.Min(a, b);
		int emptyLockerCount = 0;
		Grid.UpdateSuitMarker(cell, fullLockerCount, emptyLockerCount, gridFlags, PathFlag);
	}

	protected override void OnCleanUp()
	{
		if (filteredStorage != null)
		{
			filteredStorage.CleanUp();
		}
		if (base.isSpawned)
		{
			Grid.UnregisterSuitMarker(cell);
		}
		if (reactable != null)
		{
			reactable.Cleanup();
		}
		base.OnCleanUp();
	}

	private void OnOperationalChanged(bool isOperational)
	{
		this.isOperational = isOperational;
	}

	private void OnStorageChange(object data)
	{
		RefreshMeters();
	}

	private void UpdateGridFlag(Grid.SuitMarker.Flags flag, bool state)
	{
		if (state)
		{
			gridFlags |= flag;
		}
		else
		{
			gridFlags &= (Grid.SuitMarker.Flags)(byte)(~(int)flag);
		}
	}

	private void CreateNewReactable()
	{
		reactable = new OxygenMaskReactable(this);
	}
}
