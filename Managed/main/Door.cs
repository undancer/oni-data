using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Door")]
public class Door : Workable, ISaveLoadable, ISim200ms, INavDoor
{
	public enum DoorType
	{
		Pressure,
		ManualPressure,
		Internal,
		Sealed
	}

	public enum ControlState
	{
		Auto,
		Opened,
		Locked,
		NumStates
	}

	public class Controller : GameStateMachine<Controller, Controller.Instance, Door>
	{
		public class SealedStates : State
		{
			public class AwaitingUnlock : State
			{
				public State awaiting_arrival;

				public State unlocking;
			}

			public State closed;

			public AwaitingUnlock awaiting_unlock;

			public State chore_pst;
		}

		public new class Instance : GameInstance
		{
			public Instance(Door door)
				: base(door)
			{
			}

			public void RefreshIsBlocked()
			{
				bool value = false;
				int[] placementCells = base.master.GetComponent<Building>().PlacementCells;
				foreach (int cell in placementCells)
				{
					if (Grid.Objects[cell, 40] != null)
					{
						value = true;
						break;
					}
				}
				base.sm.isBlocked.Set(value, base.smi);
			}
		}

		public State open;

		public State opening;

		public State closed;

		public State closing;

		public State closedelay;

		public State closeblocked;

		public State locking;

		public State locked;

		public State unlocking;

		public SealedStates Sealed;

		public BoolParameter isOpen;

		public BoolParameter isLocked;

		public BoolParameter isBlocked;

		public BoolParameter isSealed;

		public BoolParameter sealDirectionRight;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = SerializeType.Both_DEPRECATED;
			default_state = closed;
			root.Update("RefreshIsBlocked", delegate(Instance smi, float dt)
			{
				smi.RefreshIsBlocked();
			}).ParamTransition(isSealed, Sealed.closed, GameStateMachine<Controller, Instance, Door, object>.IsTrue);
			closeblocked.PlayAnim("open").ParamTransition(isOpen, open, GameStateMachine<Controller, Instance, Door, object>.IsTrue).ParamTransition(isBlocked, closedelay, GameStateMachine<Controller, Instance, Door, object>.IsFalse);
			closedelay.PlayAnim("open").ScheduleGoTo(0.5f, closing).ParamTransition(isOpen, open, GameStateMachine<Controller, Instance, Door, object>.IsTrue)
				.ParamTransition(isBlocked, closeblocked, GameStateMachine<Controller, Instance, Door, object>.IsTrue);
			closing.ParamTransition(isBlocked, closeblocked, GameStateMachine<Controller, Instance, Door, object>.IsTrue).ToggleTag(GameTags.Transition).ToggleLoopingSound("Closing loop", (Instance smi) => smi.master.doorClosingSound, (Instance smi) => !string.IsNullOrEmpty(smi.master.doorClosingSound))
				.Enter("SetParams", delegate(Instance smi)
				{
					smi.master.UpdateAnimAndSoundParams(smi.master.on);
				})
				.Update(delegate(Instance smi, float dt)
				{
					if (smi.master.doorClosingSound != null)
					{
						smi.master.loopingSounds.UpdateSecondParameter(smi.master.doorClosingSound, SOUND_PROGRESS_PARAMETER, smi.Get<KBatchedAnimController>().GetPositionPercent());
					}
				}, UpdateRate.SIM_33ms)
				.Enter("SetActive", delegate(Instance smi)
				{
					smi.master.SetActive(active: true);
				})
				.Exit("SetActive", delegate(Instance smi)
				{
					smi.master.SetActive(active: false);
				})
				.PlayAnim("closing")
				.OnAnimQueueComplete(closed);
			open.PlayAnim("open").ParamTransition(isOpen, closeblocked, GameStateMachine<Controller, Instance, Door, object>.IsFalse).Enter("SetWorldStateOpen", delegate(Instance smi)
			{
				smi.master.SetWorldState();
			});
			closed.PlayAnim("closed").ParamTransition(isOpen, opening, GameStateMachine<Controller, Instance, Door, object>.IsTrue).ParamTransition(isLocked, locking, GameStateMachine<Controller, Instance, Door, object>.IsTrue)
				.Enter("SetWorldStateClosed", delegate(Instance smi)
				{
					smi.master.SetWorldState();
				});
			locking.PlayAnim("locked_pre").OnAnimQueueComplete(locked).Enter("SetWorldStateClosed", delegate(Instance smi)
			{
				smi.master.SetWorldState();
			});
			locked.PlayAnim("locked").ParamTransition(isLocked, unlocking, GameStateMachine<Controller, Instance, Door, object>.IsFalse);
			unlocking.PlayAnim("locked_pst").OnAnimQueueComplete(closed);
			opening.ToggleTag(GameTags.Transition).ToggleLoopingSound("Opening loop", (Instance smi) => smi.master.doorOpeningSound, (Instance smi) => !string.IsNullOrEmpty(smi.master.doorOpeningSound)).Enter("SetParams", delegate(Instance smi)
			{
				smi.master.UpdateAnimAndSoundParams(smi.master.on);
			})
				.Update(delegate(Instance smi, float dt)
				{
					if (smi.master.doorOpeningSound != null)
					{
						smi.master.loopingSounds.UpdateSecondParameter(smi.master.doorOpeningSound, SOUND_PROGRESS_PARAMETER, smi.Get<KBatchedAnimController>().GetPositionPercent());
					}
				}, UpdateRate.SIM_33ms)
				.Enter("SetActive", delegate(Instance smi)
				{
					smi.master.SetActive(active: true);
				})
				.Exit("SetActive", delegate(Instance smi)
				{
					smi.master.SetActive(active: false);
				})
				.PlayAnim("opening")
				.OnAnimQueueComplete(open);
			Sealed.Enter(delegate(Instance smi)
			{
				OccupyArea component = smi.master.GetComponent<OccupyArea>();
				for (int i = 0; i < component.OccupiedCellsOffsets.Length; i++)
				{
					Grid.PreventFogOfWarReveal[Grid.OffsetCell(Grid.PosToCell(smi.master.gameObject), component.OccupiedCellsOffsets[i])] = false;
				}
				smi.sm.isLocked.Set(value: true, smi);
				smi.master.controlState = ControlState.Locked;
				smi.master.RefreshControlState();
				if (smi.master.GetComponent<Unsealable>().facingRight)
				{
					KBatchedAnimController component2 = smi.master.GetComponent<KBatchedAnimController>();
					component2.FlipX = true;
				}
			}).Enter("SetWorldStateClosed", delegate(Instance smi)
			{
				smi.master.SetWorldState();
			}).Exit(delegate(Instance smi)
			{
				smi.sm.isLocked.Set(value: false, smi);
				smi.master.GetComponent<AccessControl>().controlEnabled = true;
				smi.master.controlState = ControlState.Opened;
				smi.master.RefreshControlState();
				smi.sm.isOpen.Set(value: true, smi);
				smi.sm.isLocked.Set(value: false, smi);
				smi.sm.isSealed.Set(value: false, smi);
			});
			Sealed.closed.PlayAnim("sealed", KAnim.PlayMode.Once);
			Sealed.awaiting_unlock.ToggleChore((Instance smi) => CreateUnsealChore(smi, approach_right: true), Sealed.chore_pst);
			Sealed.chore_pst.Enter(delegate(Instance smi)
			{
				smi.master.hasBeenUnsealed = true;
				if (smi.master.GetComponent<Unsealable>().unsealed)
				{
					smi.GoTo(opening);
					FogOfWarMask.ClearMask(Grid.CellRight(Grid.PosToCell(smi.master.gameObject)));
					FogOfWarMask.ClearMask(Grid.CellLeft(Grid.PosToCell(smi.master.gameObject)));
				}
				else
				{
					smi.GoTo(Sealed.closed);
				}
			});
		}

		private Chore CreateUnsealChore(Instance smi, bool approach_right)
		{
			return new WorkChore<Unsealable>(Db.Get().ChoreTypes.Toggle, smi.master);
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpReq]
	private KBatchedAnimController animController;

	[MyCmpReq]
	public Building building;

	[MyCmpGet]
	private EnergyConsumer consumer;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	public Orientation verticalOrientation = Orientation.Neutral;

	[SerializeField]
	public bool hasComplexUserControls = false;

	[SerializeField]
	public float unpoweredAnimSpeed = 0.25f;

	[SerializeField]
	public float poweredAnimSpeed = 1f;

	[SerializeField]
	public DoorType doorType;

	[SerializeField]
	public bool allowAutoControl = true;

	[SerializeField]
	public string doorClosingSoundEventName;

	[SerializeField]
	public string doorOpeningSoundEventName;

	private string doorClosingSound;

	private string doorOpeningSound;

	private static readonly HashedString SOUND_POWERED_PARAMETER = "doorPowered";

	private static readonly HashedString SOUND_PROGRESS_PARAMETER = "doorProgress";

	[Serialize]
	private bool hasBeenUnsealed = false;

	[Serialize]
	private ControlState controlState = ControlState.Auto;

	private bool on = false;

	private bool do_melt_check = false;

	private int openCount = 0;

	private ControlState requestedState;

	private Chore changeStateChore;

	private Controller.Instance controller;

	private LoggerFSS log;

	private const float REFRESH_HACK_DELAY = 1f;

	private bool doorOpenLiquidRefreshHack;

	private float doorOpenLiquidRefreshTime;

	private static readonly EventSystem.IntraObjectHandler<Door> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Door>(delegate(Door component, object data)
	{
		component.OnCopySettings(data);
	});

	public static readonly HashedString OPEN_CLOSE_PORT_ID = new HashedString("DoorOpenClose");

	private static readonly KAnimFile[] OVERRIDE_ANIMS = new KAnimFile[1]
	{
		Assets.GetAnim("anim_use_remote_kanim")
	};

	private static readonly EventSystem.IntraObjectHandler<Door> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Door>(delegate(Door component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Door> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<Door>(delegate(Door component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private bool applyLogicChange = false;

	public ControlState CurrentState => controlState;

	public ControlState RequestedState => requestedState;

	public bool ShouldBlockFallingSand => rotatable.GetOrientation() != verticalOrientation;

	public bool isSealed => controller.sm.isSealed.Get(controller);

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Door component = gameObject.GetComponent<Door>();
		if (component != null)
		{
			QueueStateChange(component.requestedState);
		}
	}

	public Door()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = OVERRIDE_ANIMS;
		synchronizeAnims = false;
		SetWorkTime(3f);
		if (!string.IsNullOrEmpty(doorClosingSoundEventName))
		{
			doorClosingSound = GlobalAssets.GetSound(doorClosingSoundEventName);
		}
		if (!string.IsNullOrEmpty(doorOpeningSoundEventName))
		{
			doorOpeningSound = GlobalAssets.GetSound(doorOpeningSoundEventName);
		}
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private ControlState GetNextState(ControlState wantedState)
	{
		return (ControlState)((int)(wantedState + 1) % 3);
	}

	private static bool DisplacesGas(DoorType type)
	{
		return type != DoorType.Internal;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KPrefabID component = GetComponent<KPrefabID>();
		if (component != null)
		{
			log = new LoggerFSS("Door");
		}
		if (!allowAutoControl && controlState == ControlState.Auto)
		{
			controlState = ControlState.Locked;
		}
		StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
		HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
		if (DisplacesGas(doorType))
		{
			structureTemperatures.Bypass(handle);
		}
		controller = new Controller.Instance(this);
		controller.StartSM();
		if (doorType == DoorType.Sealed && !hasBeenUnsealed)
		{
			Seal();
		}
		UpdateDoorSpeed(operational.IsOperational);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		Subscribe(824508782, OnOperationalChangedDelegate);
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		requestedState = CurrentState;
		ApplyRequestedControlState(force: true);
		int num = ((rotatable.GetOrientation() == Orientation.Neutral) ? (building.Def.WidthInCells * (building.Def.HeightInCells - 1)) : 0);
		int num2 = ((rotatable.GetOrientation() == Orientation.Neutral) ? building.Def.WidthInCells : building.Def.HeightInCells);
		for (int i = 0; i != num2; i++)
		{
			int num3 = building.PlacementCells[num + i];
			Grid.FakeFloor[num3] = true;
			Pathfinding.Instance.AddDirtyNavGridCell(num3);
		}
		List<int> list = new List<int>();
		int[] placementCells = building.PlacementCells;
		foreach (int num4 in placementCells)
		{
			Grid.HasDoor[num4] = true;
			if (rotatable.IsRotated)
			{
				list.Add(Grid.CellAbove(num4));
				list.Add(Grid.CellBelow(num4));
			}
			else
			{
				list.Add(Grid.CellLeft(num4));
				list.Add(Grid.CellRight(num4));
			}
			SimMessages.SetCellProperties(num4, 8);
			if (DisplacesGas(doorType))
			{
				Grid.RenderedByWorld[num4] = false;
			}
		}
	}

	protected override void OnCleanUp()
	{
		UpdateDoorState(cleaningUp: true);
		List<int> list = new List<int>();
		int[] placementCells = building.PlacementCells;
		foreach (int num in placementCells)
		{
			SimMessages.ClearCellProperties(num, 12);
			Grid.RenderedByWorld[num] = Grid.Element[num].substance.renderedByWorld;
			Grid.FakeFloor[num] = false;
			if (Grid.Element[num].IsSolid)
			{
				SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.DoorOpen, 0f);
			}
			Pathfinding.Instance.AddDirtyNavGridCell(num);
			if (rotatable.IsRotated)
			{
				list.Add(Grid.CellAbove(num));
				list.Add(Grid.CellBelow(num));
			}
			else
			{
				list.Add(Grid.CellLeft(num));
				list.Add(Grid.CellRight(num));
			}
		}
		int[] placementCells2 = building.PlacementCells;
		foreach (int num2 in placementCells2)
		{
			Grid.HasDoor[num2] = false;
			Game.Instance.SetDupePassableSolid(num2, passable: false, Grid.Solid[num2]);
			Grid.CritterImpassable[num2] = false;
			Grid.DupeImpassable[num2] = false;
			Pathfinding.Instance.AddDirtyNavGridCell(num2);
		}
		base.OnCleanUp();
	}

	public void Seal()
	{
		controller.sm.isSealed.Set(value: true, controller);
	}

	public void OrderUnseal()
	{
		controller.GoTo(controller.sm.Sealed.awaiting_unlock);
	}

	private void RefreshControlState()
	{
		switch (controlState)
		{
		case ControlState.Auto:
			controller.sm.isLocked.Set(value: false, controller);
			break;
		case ControlState.Opened:
			controller.sm.isLocked.Set(value: false, controller);
			break;
		case ControlState.Locked:
			controller.sm.isLocked.Set(value: true, controller);
			break;
		}
		Trigger(279163026, controlState);
		SetWorldState();
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CurrentDoorControlState, this);
	}

	private void OnOperationalChanged(object data)
	{
		bool isOperational = operational.IsOperational;
		if (isOperational != on)
		{
			UpdateDoorSpeed(isOperational);
			if (on && GetComponent<KPrefabID>().HasTag(GameTags.Transition))
			{
				SetActive(active: true);
			}
			else
			{
				SetActive(active: false);
			}
		}
	}

	private void UpdateDoorSpeed(bool powered)
	{
		on = powered;
		UpdateAnimAndSoundParams(powered);
		float positionPercent = animController.GetPositionPercent();
		animController.Play(animController.CurrentAnim.hash, animController.PlayMode);
		animController.SetPositionPercent(positionPercent);
	}

	private void UpdateAnimAndSoundParams(bool powered)
	{
		if (powered)
		{
			animController.PlaySpeedMultiplier = poweredAnimSpeed;
			if (doorClosingSound != null)
			{
				loopingSounds.UpdateFirstParameter(doorClosingSound, SOUND_POWERED_PARAMETER, 1f);
			}
			if (doorOpeningSound != null)
			{
				loopingSounds.UpdateFirstParameter(doorOpeningSound, SOUND_POWERED_PARAMETER, 1f);
			}
		}
		else
		{
			animController.PlaySpeedMultiplier = unpoweredAnimSpeed;
			if (doorClosingSound != null)
			{
				loopingSounds.UpdateFirstParameter(doorClosingSound, SOUND_POWERED_PARAMETER, 0f);
			}
			if (doorOpeningSound != null)
			{
				loopingSounds.UpdateFirstParameter(doorOpeningSound, SOUND_POWERED_PARAMETER, 0f);
			}
		}
	}

	private void SetActive(bool active)
	{
		if (operational.IsOperational)
		{
			operational.SetActive(active);
		}
	}

	private void SetWorldState()
	{
		int[] placementCells = building.PlacementCells;
		bool is_door_open = IsOpen();
		SetPassableState(is_door_open, placementCells);
		SetSimState(is_door_open, placementCells);
	}

	private void SetPassableState(bool is_door_open, IList<int> cells)
	{
		for (int i = 0; i < cells.Count; i++)
		{
			int num = cells[i];
			switch (doorType)
			{
			case DoorType.Pressure:
			case DoorType.ManualPressure:
			case DoorType.Sealed:
			{
				Grid.CritterImpassable[num] = controlState != ControlState.Opened;
				bool solid = !is_door_open;
				bool passable = controlState != ControlState.Locked;
				Game.Instance.SetDupePassableSolid(num, passable, solid);
				if (controlState == ControlState.Opened)
				{
					doorOpenLiquidRefreshHack = true;
					doorOpenLiquidRefreshTime = 1f;
				}
				break;
			}
			case DoorType.Internal:
				Grid.CritterImpassable[num] = controlState != ControlState.Opened;
				Grid.DupeImpassable[num] = controlState == ControlState.Locked;
				break;
			}
			Pathfinding.Instance.AddDirtyNavGridCell(num);
		}
	}

	private void SetSimState(bool is_door_open, IList<int> cells)
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		float mass = component.Mass / (float)cells.Count;
		for (int i = 0; i < cells.Count; i++)
		{
			int num = cells[i];
			DoorType doorType = this.doorType;
			DoorType doorType2 = doorType;
			if ((uint)doorType2 > 1u && doorType2 != DoorType.Sealed)
			{
				continue;
			}
			World.Instance.groundRenderer.MarkDirty(num);
			if (is_door_open)
			{
				SimMessages.Dig(num, Game.Instance.callbackManager.Add(new Game.CallbackInfo(OnSimDoorOpened)).index, skipEvent: true);
				if (ShouldBlockFallingSand)
				{
					SimMessages.ClearCellProperties(num, 4);
				}
				else
				{
					SimMessages.SetCellProperties(num, 4);
				}
				continue;
			}
			HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(OnSimDoorClosed));
			float temperature = component.Temperature;
			if (temperature <= 0f)
			{
				temperature = component.Temperature;
			}
			SimMessages.ReplaceAndDisplaceElement(num, component.ElementID, CellEventLogger.Instance.DoorClose, mass, temperature, byte.MaxValue, 0, handle.index);
			SimMessages.SetCellProperties(num, 4);
		}
	}

	private void UpdateDoorState(bool cleaningUp)
	{
		int[] placementCells = building.PlacementCells;
		foreach (int num in placementCells)
		{
			if (Grid.IsValidCell(num))
			{
				Grid.Foundation[num] = !cleaningUp;
			}
		}
	}

	public void QueueStateChange(ControlState nextState)
	{
		if (requestedState != nextState)
		{
			requestedState = nextState;
		}
		else
		{
			requestedState = controlState;
		}
		if (requestedState == controlState)
		{
			if (changeStateChore != null)
			{
				changeStateChore.Cancel("Change state");
				changeStateChore = null;
				GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState);
			}
		}
		else if (DebugHandler.InstantBuildMode)
		{
			controlState = requestedState;
			RefreshControlState();
			OnOperationalChanged(null);
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState);
			Open();
			Close();
		}
		else
		{
			if (changeStateChore != null)
			{
				changeStateChore.Cancel("Change state");
			}
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, this);
			changeStateChore = new WorkChore<Door>(Db.Get().ChoreTypes.Toggle, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
		}
	}

	private void OnSimDoorOpened()
	{
		if (!(this == null) && DisplacesGas(doorType))
		{
			StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
			HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
			structureTemperatures.UnBypass(handle);
			do_melt_check = false;
		}
	}

	private void OnSimDoorClosed()
	{
		if (!(this == null) && DisplacesGas(doorType))
		{
			StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
			HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
			structureTemperatures.Bypass(handle);
			do_melt_check = true;
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		changeStateChore = null;
		ApplyRequestedControlState();
	}

	public void Open()
	{
		if (openCount == 0 && DisplacesGas(doorType))
		{
			StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
			HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
			if (handle.IsValid() && structureTemperatures.IsBypassed(handle))
			{
				int[] placementCells = building.PlacementCells;
				float num = 0f;
				int num2 = 0;
				foreach (int i2 in placementCells)
				{
					if (Grid.Mass[i2] > 0f)
					{
						num2++;
						num += Grid.Temperature[i2];
					}
				}
				if (num2 > 0)
				{
					num /= (float)placementCells.Length;
					PrimaryElement component = GetComponent<PrimaryElement>();
					KCrashReporter.Assert(num > 0f, "Door has calculated an invalid temperature");
					component.Temperature = num;
				}
			}
		}
		openCount++;
		switch (controlState)
		{
		case ControlState.Auto:
		case ControlState.Opened:
			controller.sm.isOpen.Set(value: true, controller);
			break;
		}
	}

	public void Close()
	{
		openCount = Mathf.Max(0, openCount - 1);
		if (openCount == 0 && DisplacesGas(doorType))
		{
			StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
			HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
			PrimaryElement component = GetComponent<PrimaryElement>();
			if (handle.IsValid() && !structureTemperatures.IsBypassed(handle))
			{
				float num = (component.Temperature = structureTemperatures.GetPayload(handle).Temperature);
			}
		}
		switch (controlState)
		{
		case ControlState.Opened:
			break;
		case ControlState.Locked:
			controller.sm.isOpen.Set(value: false, controller);
			break;
		case ControlState.Auto:
			if (openCount == 0)
			{
				controller.sm.isOpen.Set(value: false, controller);
				Game.Instance.userMenu.Refresh(base.gameObject);
			}
			break;
		}
	}

	public bool IsOpen()
	{
		return controller.IsInsideState(controller.sm.open) || controller.IsInsideState(controller.sm.closedelay) || controller.IsInsideState(controller.sm.closeblocked);
	}

	private void ApplyRequestedControlState(bool force = false)
	{
		if (requestedState != controlState || force)
		{
			controlState = requestedState;
			RefreshControlState();
			OnOperationalChanged(null);
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState);
			Trigger(1734268753, this);
			if (!force)
			{
				Open();
				Close();
			}
		}
	}

	public void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (!(logicValueChanged.portID != OPEN_CLOSE_PORT_ID))
		{
			int newValue = logicValueChanged.newValue;
			if (changeStateChore != null)
			{
				changeStateChore.Cancel("Change state");
				changeStateChore = null;
			}
			bool flag = LogicCircuitNetwork.IsBitActive(0, newValue);
			requestedState = (flag ? ControlState.Opened : ControlState.Locked);
			applyLogicChange = true;
		}
	}

	public void Sim200ms(float dt)
	{
		if (this == null)
		{
			return;
		}
		if (doorOpenLiquidRefreshHack)
		{
			doorOpenLiquidRefreshTime -= dt;
			if (doorOpenLiquidRefreshTime <= 0f)
			{
				doorOpenLiquidRefreshHack = false;
				int[] placementCells = building.PlacementCells;
				foreach (int cell in placementCells)
				{
					Pathfinding.Instance.AddDirtyNavGridCell(cell);
				}
			}
		}
		if (applyLogicChange)
		{
			applyLogicChange = false;
			ApplyRequestedControlState();
		}
		if (!do_melt_check)
		{
			return;
		}
		StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
		HandleVector<int>.Handle handle = structureTemperatures.GetHandle(base.gameObject);
		if (!handle.IsValid() || !structureTemperatures.IsBypassed(handle))
		{
			return;
		}
		int[] placementCells2 = building.PlacementCells;
		foreach (int i2 in placementCells2)
		{
			if (!Grid.Solid[i2])
			{
				Util.KDestroyGameObject(this);
				break;
			}
		}
	}

	bool INavDoor.get_isSpawned()
	{
		return base.isSpawned;
	}
}
