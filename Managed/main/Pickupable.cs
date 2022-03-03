using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using FMOD.Studio;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Pickupable")]
public class Pickupable : Workable, IHasSortOrder
{
	private struct Reservation
	{
		public GameObject reserver;

		public float amount;

		public int ticket;

		public Reservation(GameObject reserver, float amount, int ticket)
		{
			this.reserver = reserver;
			this.amount = amount;
			this.ticket = ticket;
		}

		public override string ToString()
		{
			return reserver.name + ", " + amount + ", " + ticket;
		}
	}

	public class PickupableStartWorkInfo : Worker.StartWorkInfo
	{
		public float amount { get; private set; }

		public Pickupable originalPickupable { get; private set; }

		public Action<GameObject> setResultCb { get; private set; }

		public PickupableStartWorkInfo(Pickupable pickupable, float amount, Action<GameObject> set_result_cb)
			: base(pickupable.targetWorkable)
		{
			originalPickupable = pickupable;
			this.amount = amount;
			setResultCb = set_result_cb;
		}
	}

	[MyCmpReq]
	private PrimaryElement primaryElement;

	public const float WorkTime = 1.5f;

	[SerializeField]
	private int _sortOrder;

	[NonSerialized]
	[MyCmpReq]
	public KPrefabID KPrefabID;

	[NonSerialized]
	[MyCmpAdd]
	public Clearable Clearable;

	[NonSerialized]
	[MyCmpAdd]
	public Prioritizable prioritizable;

	public bool absorbable;

	public Func<Pickupable, bool> CanAbsorb = (Pickupable other) => false;

	public Func<float, Pickupable> OnTake;

	public System.Action OnReservationsChanged;

	public ObjectLayerListItem objectLayerListItem;

	public Workable targetWorkable;

	public KAnimFile carryAnimOverride;

	private KBatchedAnimController lastCarrier;

	public bool useGunforPickup = true;

	private static CellOffset[] displacementOffsets = new CellOffset[8]
	{
		new CellOffset(0, 1),
		new CellOffset(0, -1),
		new CellOffset(1, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 1),
		new CellOffset(1, -1),
		new CellOffset(-1, 1),
		new CellOffset(-1, -1)
	};

	private bool isReachable;

	private bool isEntombed;

	private bool cleaningUp;

	public bool trackOnPickup = true;

	private int nextTicketNumber;

	[Serialize]
	public bool deleteOffGrid = true;

	private List<Reservation> reservations = new List<Reservation>();

	private HandleVector<int>.Handle solidPartitionerEntry;

	private HandleVector<int>.Handle partitionerEntry;

	private LoggerFSSF log;

	private static readonly EventSystem.IntraObjectHandler<Pickupable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.OnStore(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Pickupable> OnLandedDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.OnLanded(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Pickupable> OnOreSizeChangedDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.OnOreSizeChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Pickupable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.OnReachableChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Pickupable> RefreshStorageTagsDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.RefreshStorageTags(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Pickupable> OnTagsChangedDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.OnTagsChanged(data);
	});

	private int entombedCell = -1;

	public PrimaryElement PrimaryElement => primaryElement;

	public int sortOrder
	{
		get
		{
			return _sortOrder;
		}
		set
		{
			_sortOrder = value;
		}
	}

	public Storage storage { get; set; }

	public float MinTakeAmount => 0f;

	public bool prevent_absorb_until_stored { get; set; }

	public bool isKinematic { get; set; }

	public bool wasAbsorbed { get; private set; }

	public int cachedCell { get; private set; }

	public bool IsEntombed
	{
		get
		{
			return isEntombed;
		}
		set
		{
			if (value != isEntombed)
			{
				isEntombed = value;
				if (isEntombed)
				{
					GetComponent<KPrefabID>().AddTag(GameTags.Entombed);
				}
				else
				{
					GetComponent<KPrefabID>().RemoveTag(GameTags.Entombed);
				}
				Trigger(-1089732772);
				UpdateEntombedVisualizer();
			}
		}
	}

	public float UnreservedAmount => TotalAmount - ReservedAmount;

	public float ReservedAmount { get; private set; }

	public float TotalAmount
	{
		get
		{
			return primaryElement.Units;
		}
		set
		{
			DebugUtil.Assert(primaryElement != null);
			primaryElement.Units = value;
			if (value < PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT && !primaryElement.KeepZeroMassObject)
			{
				base.gameObject.DeleteObject();
			}
			NotifyChanged(Grid.PosToCell(this));
		}
	}

	private bool CouldBePickedUpCommon(GameObject carrier)
	{
		if (UnreservedAmount >= MinTakeAmount)
		{
			if (!(UnreservedAmount > 0f))
			{
				return FindReservedAmount(carrier) > 0f;
			}
			return true;
		}
		return false;
	}

	public bool CouldBePickedUpByMinion(GameObject carrier)
	{
		if (CouldBePickedUpCommon(carrier))
		{
			if (!(storage == null) && (bool)storage.automatable)
			{
				return !storage.automatable.GetAutomationOnly();
			}
			return true;
		}
		return false;
	}

	public bool CouldBePickedUpByTransferArm(GameObject carrier)
	{
		return CouldBePickedUpCommon(carrier);
	}

	public float FindReservedAmount(GameObject reserver)
	{
		for (int i = 0; i < reservations.Count; i++)
		{
			if (reservations[i].reserver == reserver)
			{
				return reservations[i].amount;
			}
		}
		return 0f;
	}

	private void RefreshReservedAmount()
	{
		ReservedAmount = 0f;
		for (int i = 0; i < reservations.Count; i++)
		{
			ReservedAmount += reservations[i].amount;
		}
	}

	[Conditional("UNITY_EDITOR")]
	private void Log(string evt, string param, float value)
	{
	}

	public void ClearReservations()
	{
		reservations.Clear();
		RefreshReservedAmount();
	}

	[ContextMenu("Print Reservations")]
	public void PrintReservations()
	{
		foreach (Reservation reservation in reservations)
		{
			Debug.Log(reservation.ToString());
		}
	}

	public int Reserve(string context, GameObject reserver, float amount)
	{
		int num = nextTicketNumber++;
		Reservation item = new Reservation(reserver, amount, num);
		reservations.Add(item);
		RefreshReservedAmount();
		if (OnReservationsChanged != null)
		{
			OnReservationsChanged();
		}
		return num;
	}

	public void Unreserve(string context, int ticket)
	{
		for (int i = 0; i < reservations.Count; i++)
		{
			if (reservations[i].ticket == ticket)
			{
				reservations.RemoveAt(i);
				RefreshReservedAmount();
				if (OnReservationsChanged != null)
				{
					OnReservationsChanged();
				}
				break;
			}
		}
	}

	private Pickupable()
	{
		showProgressBar = false;
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		shouldTransferDiseaseWithWorker = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workingPstComplete = null;
		workingPstFailed = null;
		log = new LoggerFSSF("Pickupable");
		workerStatusItem = Db.Get().DuplicantStatusItems.PickingUp;
		SetWorkTime(1.5f);
		targetWorkable = this;
		resetProgressOnStop = true;
		base.gameObject.layer = Game.PickupableLayer;
		Vector3 position = base.transform.GetPosition();
		UpdateCachedCell(Grid.PosToCell(position));
		Subscribe(856640610, OnStoreDelegate);
		Subscribe(1188683690, OnLandedDelegate);
		Subscribe(1807976145, OnOreSizeChangedDelegate);
		Subscribe(-1432940121, OnReachableChangedDelegate);
		Subscribe(-778359855, RefreshStorageTagsDelegate);
		KPrefabID.AddTag(GameTags.Pickupable);
		Components.Pickupables.Add(this);
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num) && deleteOffGrid)
		{
			base.gameObject.DeleteObject();
			return;
		}
		UpdateCachedCell(num);
		new ReachabilityMonitor.Instance(this).StartSM();
		new FetchableMonitor.Instance(this).StartSM();
		SetWorkTime(1.5f);
		faceTargetWhenWorking = true;
		KSelectable component = GetComponent<KSelectable>();
		if (component != null)
		{
			component.SetStatusIndicatorOffset(new Vector3(0f, -0.65f, 0f));
		}
		OnTagsChanged(null);
		TryToOffsetIfBuried();
		DecorProvider component2 = GetComponent<DecorProvider>();
		if (component2 != null && string.IsNullOrEmpty(component2.overrideName))
		{
			component2.overrideName = UI.OVERLAYS.DECOR.CLUTTER;
		}
		UpdateEntombedVisualizer();
		Subscribe(-1582839653, OnTagsChangedDelegate);
		NotifyChanged(num);
	}

	[OnDeserialized]
	public void OnDeserialize()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 28) && base.transform.position.z == 0f)
		{
			KBatchedAnimController component = base.transform.GetComponent<KBatchedAnimController>();
			component.SetSceneLayer(component.sceneLayer);
		}
	}

	public void RegisterListeners()
	{
		if (!cleaningUp && !solidPartitionerEntry.IsValid())
		{
			int num = Grid.PosToCell(this);
			objectLayerListItem = new ObjectLayerListItem(base.gameObject, ObjectLayer.Pickupables, num);
			solidPartitionerEntry = GameScenePartitioner.Instance.Add("Pickupable.RegisterSolidListener", base.gameObject, num, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
			partitionerEntry = GameScenePartitioner.Instance.Add("Pickupable.RegisterPickupable", this, num, GameScenePartitioner.Instance.pickupablesLayer, null);
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "Pickupable.OnCellChange");
			Singleton<CellChangeMonitor>.Instance.MarkDirty(base.transform);
		}
	}

	public void UnregisterListeners()
	{
		if (objectLayerListItem != null)
		{
			objectLayerListItem.Clear();
			objectLayerListItem = null;
		}
		GameScenePartitioner.Instance.Free(ref solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
	}

	private void OnSolidChanged(object data)
	{
		TryToOffsetIfBuried();
	}

	public void TryToOffsetIfBuried()
	{
		if (KPrefabID.HasTag(GameTags.Stored) || KPrefabID.HasTag(GameTags.Equipped))
		{
			return;
		}
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		if ((base.gameObject.GetSMI<DeathMonitor.Instance>()?.IsDead() ?? true) && ((Grid.Solid[num] && Grid.Foundation[num]) || Grid.Properties[num] != 0))
		{
			for (int i = 0; i < displacementOffsets.Length; i++)
			{
				int num2 = Grid.OffsetCell(num, displacementOffsets[i]);
				if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
				{
					Vector3 position = Grid.CellToPosCBC(num2, Grid.SceneLayer.Move);
					KCollider2D component = GetComponent<KCollider2D>();
					if (component != null)
					{
						position.y += base.transform.GetPosition().y - component.bounds.min.y;
					}
					base.transform.SetPosition(position);
					num = num2;
					RemoveFaller();
					AddFaller(Vector2.zero);
					break;
				}
			}
		}
		HandleSolidCell(num);
	}

	private bool HandleSolidCell(int cell)
	{
		bool flag = IsEntombed;
		bool flag2 = false;
		if (Grid.IsValidCell(cell) && Grid.Solid[cell] && (base.gameObject.GetSMI<DeathMonitor.Instance>()?.IsDead() ?? true))
		{
			Clearable.CancelClearing();
			flag2 = true;
		}
		if (flag2 != flag && !KPrefabID.HasTag(GameTags.Stored))
		{
			IsEntombed = flag2;
			GetComponent<KSelectable>().IsSelectable = !IsEntombed;
		}
		UpdateEntombedVisualizer();
		return IsEntombed;
	}

	private void OnCellChange()
	{
		Vector3 position = base.transform.GetPosition();
		int num = Grid.PosToCell(position);
		if (!Grid.IsValidCell(num))
		{
			Vector2 vector = new Vector2(-0.1f * (float)Grid.WidthInCells, 1.1f * (float)Grid.WidthInCells);
			Vector2 vector2 = new Vector2(-0.1f * (float)Grid.HeightInCells, 1.1f * (float)Grid.HeightInCells);
			if (deleteOffGrid && (position.x < vector.x || vector.y < position.x || position.y < vector2.x || vector2.y < position.y))
			{
				this.DeleteObject();
			}
			return;
		}
		ReleaseEntombedVisualizerAndAddFaller(add_faller_if_necessary: true);
		if (HandleSolidCell(num))
		{
			return;
		}
		objectLayerListItem.Update(num);
		bool flag = false;
		if (absorbable && !KPrefabID.HasTag(GameTags.Stored))
		{
			int num2 = Grid.CellBelow(num);
			if (Grid.IsValidCell(num2) && Grid.Solid[num2])
			{
				ObjectLayerListItem nextItem = objectLayerListItem.nextItem;
				while (nextItem != null)
				{
					GameObject gameObject = nextItem.gameObject;
					nextItem = nextItem.nextItem;
					Pickupable component = gameObject.GetComponent<Pickupable>();
					if (component != null)
					{
						flag = component.TryAbsorb(this, hide_effects: false);
						if (flag)
						{
							break;
						}
					}
				}
			}
		}
		GameScenePartitioner.Instance.UpdatePosition(solidPartitionerEntry, num);
		GameScenePartitioner.Instance.UpdatePosition(partitionerEntry, num);
		int num3 = cachedCell;
		UpdateCachedCell(num);
		if (!flag)
		{
			NotifyChanged(num);
		}
		if (Grid.IsValidCell(num3) && num != num3)
		{
			NotifyChanged(num3);
		}
	}

	private void OnTagsChanged(object data)
	{
		if (!KPrefabID.HasTag(GameTags.Stored) && !KPrefabID.HasTag(GameTags.Equipped))
		{
			RegisterListeners();
			AddFaller(Vector2.zero);
		}
		else
		{
			UnregisterListeners();
			RemoveFaller();
		}
	}

	private void NotifyChanged(int new_cell)
	{
		GameScenePartitioner.Instance.TriggerEvent(new_cell, GameScenePartitioner.Instance.pickupablesChangedLayer, this);
	}

	public bool TryAbsorb(Pickupable other, bool hide_effects, bool allow_cross_storage = false)
	{
		if (other == null)
		{
			return false;
		}
		if (other.wasAbsorbed)
		{
			return false;
		}
		if (wasAbsorbed)
		{
			return false;
		}
		if (!other.CanAbsorb(this))
		{
			return false;
		}
		if (prevent_absorb_until_stored)
		{
			return false;
		}
		if (!allow_cross_storage && storage == null != (other.storage == null))
		{
			return false;
		}
		Absorb(other);
		if (!hide_effects && EffectPrefabs.Instance != null && !storage)
		{
			Vector3 position = base.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
			Util.KInstantiate(Assets.GetPrefab(EffectConfigs.OreAbsorbId), position, Quaternion.identity).SetActive(value: true);
		}
		return true;
	}

	protected override void OnCleanUp()
	{
		cleaningUp = true;
		ReleaseEntombedVisualizerAndAddFaller(add_faller_if_necessary: false);
		RemoveFaller();
		if ((bool)storage)
		{
			storage.Remove(base.gameObject);
		}
		UnregisterListeners();
		Components.Pickupables.Remove(this);
		if (reservations.Count > 0)
		{
			reservations.Clear();
			if (OnReservationsChanged != null)
			{
				OnReservationsChanged();
			}
		}
		if (Grid.IsValidCell(cachedCell))
		{
			NotifyChanged(cachedCell);
		}
		base.OnCleanUp();
	}

	public Pickupable Take(float amount)
	{
		if (amount <= 0f)
		{
			return null;
		}
		if (OnTake != null)
		{
			if (amount >= TotalAmount && storage != null && !primaryElement.KeepZeroMassObject)
			{
				storage.Remove(base.gameObject);
			}
			float num = Math.Min(TotalAmount, amount);
			if (num <= 0f)
			{
				return null;
			}
			return OnTake(num);
		}
		if (storage != null)
		{
			storage.Remove(base.gameObject);
		}
		return this;
	}

	private void Absorb(Pickupable pickupable)
	{
		Debug.Assert(!wasAbsorbed);
		Debug.Assert(!pickupable.wasAbsorbed);
		Trigger(-2064133523, pickupable);
		pickupable.Trigger(-1940207677, base.gameObject);
		pickupable.wasAbsorbed = true;
		KSelectable component = GetComponent<KSelectable>();
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null && SelectTool.Instance.selected == pickupable.GetComponent<KSelectable>())
		{
			SelectTool.Instance.Select(component);
		}
		pickupable.gameObject.DeleteObject();
		NotifyChanged(Grid.PosToCell(this));
	}

	private void RefreshStorageTags(object data = null)
	{
		bool flag = data is Storage || (data != null && (bool)data);
		if (flag && data is Storage && ((Storage)data).gameObject == base.gameObject)
		{
			return;
		}
		if (flag)
		{
			KPrefabID.AddTag(GameTags.Stored);
			if ((object)storage == null || !storage.allowItemRemoval)
			{
				KPrefabID.AddTag(GameTags.StoredPrivate);
			}
			else
			{
				KPrefabID.RemoveTag(GameTags.StoredPrivate);
			}
		}
		else
		{
			KPrefabID.RemoveTag(GameTags.Stored);
			KPrefabID.RemoveTag(GameTags.StoredPrivate);
		}
	}

	public void OnStore(object data)
	{
		storage = data as Storage;
		bool flag = data is Storage || (data != null && (bool)data);
		SaveLoadRoot component = GetComponent<SaveLoadRoot>();
		if (carryAnimOverride != null && lastCarrier != null)
		{
			lastCarrier.RemoveAnimOverrides(carryAnimOverride);
			lastCarrier = null;
		}
		KSelectable component2 = GetComponent<KSelectable>();
		if ((bool)component2)
		{
			component2.IsSelectable = !flag;
		}
		if (flag)
		{
			int new_cell = cachedCell;
			RefreshStorageTags(data);
			if ((object)storage != null)
			{
				if (carryAnimOverride != null && storage.GetComponent<Navigator>() != null)
				{
					lastCarrier = storage.GetComponent<KBatchedAnimController>();
					if (lastCarrier != null)
					{
						lastCarrier.AddAnimOverrides(carryAnimOverride);
					}
				}
				UpdateCachedCell(Grid.PosToCell(storage));
			}
			NotifyChanged(new_cell);
			if (component != null)
			{
				component.SetRegistered(registered: false);
			}
		}
		else
		{
			if (component != null)
			{
				component.SetRegistered(registered: true);
			}
			RemovedFromStorage();
		}
	}

	private void RemovedFromStorage()
	{
		storage = null;
		UpdateCachedCell(Grid.PosToCell(this));
		RefreshStorageTags();
		AddFaller(Vector2.zero);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.enabled = true;
		base.gameObject.transform.rotation = Quaternion.identity;
		RegisterListeners();
		component.GetBatchInstanceData().ClearOverrideTransformMatrix();
	}

	public void UpdateCachedCellFromStoragePosition()
	{
		Debug.Assert(storage != null, "Only call UpdateCachedCellFromStoragePosition on pickupables in storage!");
		UpdateCachedCell(Grid.PosToCell(storage));
	}

	private void UpdateCachedCell(int cell)
	{
		cachedCell = cell;
		GetOffsets(cachedCell);
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		if (useGunforPickup && worker.usesMultiTool)
		{
			AnimInfo anim = base.GetAnim(worker);
			anim.smi = new MultitoolController.Instance(this, worker, "pickup", Assets.GetPrefab(EffectConfigs.OreAbsorbId));
			return anim;
		}
		return base.GetAnim(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage component = worker.GetComponent<Storage>();
		PickupableStartWorkInfo pickupableStartWorkInfo = (PickupableStartWorkInfo)worker.startWorkInfo;
		float amount = pickupableStartWorkInfo.amount;
		if (this != null)
		{
			Pickupable pickupable = Take(amount);
			if (pickupable != null)
			{
				component.Store(pickupable.gameObject);
				worker.workCompleteData = pickupable;
				pickupableStartWorkInfo.setResultCb(pickupable.gameObject);
			}
			else
			{
				pickupableStartWorkInfo.setResultCb(null);
			}
		}
		else
		{
			pickupableStartWorkInfo.setResultCb(null);
		}
	}

	public override bool InstantlyFinish(Worker worker)
	{
		return false;
	}

	public override Vector3 GetTargetPoint()
	{
		return base.transform.GetPosition();
	}

	public bool IsReachable()
	{
		return isReachable;
	}

	private void OnReachableChanged(object data)
	{
		isReachable = (bool)data;
		KSelectable component = GetComponent<KSelectable>();
		if (isReachable)
		{
			component.RemoveStatusItem(Db.Get().MiscStatusItems.PickupableUnreachable);
		}
		else
		{
			component.AddStatusItem(Db.Get().MiscStatusItems.PickupableUnreachable, this);
		}
	}

	private void AddFaller(Vector2 initial_velocity)
	{
		if (!(GetComponent<Health>() != null) && !GameComps.Fallers.Has(base.gameObject))
		{
			GameComps.Fallers.Add(base.gameObject, initial_velocity);
		}
	}

	private void RemoveFaller()
	{
		if (!(GetComponent<Health>() != null) && GameComps.Fallers.Has(base.gameObject))
		{
			GameComps.Fallers.Remove(base.gameObject);
		}
	}

	private void OnOreSizeChanged(object data)
	{
		Vector3 vector = Vector3.zero;
		HandleVector<int>.Handle handle = GameComps.Gravities.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			vector = GameComps.Gravities.GetData(handle).velocity;
		}
		RemoveFaller();
		if (!KPrefabID.HasTag(GameTags.Stored))
		{
			AddFaller(vector);
		}
	}

	private void OnLanded(object data)
	{
		if (CameraController.Instance == null)
		{
			return;
		}
		Vector3 position = base.transform.GetPosition();
		Vector2I vector2I = Grid.PosToXY(position);
		if (vector2I.x < 0 || Grid.WidthInCells <= vector2I.x || vector2I.y < 0 || Grid.HeightInCells <= vector2I.y)
		{
			this.DeleteObject();
			return;
		}
		Vector2 vector = (Vector2)data;
		if (vector.sqrMagnitude <= 0.2f || SpeedControlScreen.Instance.IsPaused)
		{
			return;
		}
		Element element = primaryElement.Element;
		if (element.substance == null)
		{
			return;
		}
		string text = element.substance.GetOreBumpSound();
		if (text == null)
		{
			text = (element.HasTag(GameTags.RefinedMetal) ? "RefinedMetal" : ((!element.HasTag(GameTags.Metal)) ? "Rock" : "RawMetal"));
		}
		text = ((!(element.tag.ToString() == "Creature") || base.gameObject.HasTag(GameTags.Seed)) ? ("Ore_bump_" + text) : "Bodyfall_rock");
		string sound = GlobalAssets.GetSound(text, force_no_warning: true);
		sound = ((sound != null) ? sound : GlobalAssets.GetSound("Ore_bump_rock"));
		if (CameraController.Instance.IsAudibleSound(base.transform.GetPosition(), sound))
		{
			int num = Grid.PosToCell(position);
			bool isLiquid = Grid.Element[num].IsLiquid;
			float value = 0f;
			if (isLiquid)
			{
				value = SoundUtil.GetLiquidDepth(num);
			}
			FMOD.Studio.EventInstance instance = KFMOD.BeginOneShot(sound, CameraController.Instance.GetVerticallyScaledPosition(base.transform.GetPosition()));
			instance.setParameterByName("velocity", vector.magnitude);
			instance.setParameterByName("liquidDepth", value);
			KFMOD.EndOneShot(instance);
		}
	}

	private void UpdateEntombedVisualizer()
	{
		if (IsEntombed)
		{
			if (entombedCell != -1)
			{
				return;
			}
			int cell = Grid.PosToCell(this);
			if (EntombedItemManager.CanEntomb(this))
			{
				SaveGame.Instance.entombedItemManager.Add(this);
			}
			if (Grid.Objects[cell, 1] == null)
			{
				KBatchedAnimController component = GetComponent<KBatchedAnimController>();
				if (component != null && Game.Instance.GetComponent<EntombedItemVisualizer>().AddItem(cell))
				{
					entombedCell = cell;
					component.enabled = false;
					RemoveFaller();
				}
			}
		}
		else
		{
			ReleaseEntombedVisualizerAndAddFaller(add_faller_if_necessary: true);
		}
	}

	private void ReleaseEntombedVisualizerAndAddFaller(bool add_faller_if_necessary)
	{
		if (entombedCell != -1)
		{
			Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(entombedCell);
			entombedCell = -1;
			GetComponent<KBatchedAnimController>().enabled = true;
			if (add_faller_if_necessary)
			{
				AddFaller(Vector2.zero);
			}
		}
	}
}
