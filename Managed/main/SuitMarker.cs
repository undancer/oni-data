using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SuitMarker")]
public class SuitMarker : KMonoBehaviour
{
	private class SuitMarkerReactable : Reactable
	{
		private SuitMarker suitMarker;

		private float startTime;

		public SuitMarkerReactable(SuitMarker suit_marker)
			: base(suit_marker.gameObject, "SuitMarkerReactable", Db.Get().ChoreTypes.SuitMarker, 1, 1)
		{
			suitMarker = suit_marker;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (reactor != null)
			{
				return false;
			}
			if (suitMarker == null)
			{
				Cleanup();
				return false;
			}
			if (!suitMarker.isOperational)
			{
				return false;
			}
			int x = transition.navGridTransition.x;
			if (x == 0)
			{
				return false;
			}
			MinionIdentity component = new_reactor.GetComponent<MinionIdentity>();
			if (component.GetEquipment().IsSlotOccupied(Db.Get().AssignableSlots.Suit))
			{
				if (x < 0 && suitMarker.isRotated)
				{
					return false;
				}
				if (x > 0 && !suitMarker.isRotated)
				{
					return false;
				}
				return true;
			}
			if (x > 0 && suitMarker.isRotated)
			{
				return false;
			}
			if (x < 0 && !suitMarker.isRotated)
			{
				return false;
			}
			return Grid.HasSuit(Grid.PosToCell(suitMarker), new_reactor.GetComponent<KPrefabID>().InstanceID);
		}

		protected override void InternalBegin()
		{
			startTime = Time.time;
			KBatchedAnimController component = reactor.GetComponent<KBatchedAnimController>();
			component.AddAnimOverrides(suitMarker.interactAnim, 1f);
			component.Play("working_pre");
			component.Queue("working_loop");
			component.Queue("working_pst");
			if (suitMarker.HasTag(GameTags.JetSuitBlocker))
			{
				KBatchedAnimController component2 = suitMarker.GetComponent<KBatchedAnimController>();
				component2.Play("working_pre");
				component2.Queue("working_loop");
				component2.Queue("working_pst");
			}
			suitMarker.CreateNewReactable();
		}

		public override void Update(float dt)
		{
			Facing facing = (reactor ? reactor.GetComponent<Facing>() : null);
			if ((bool)facing && (bool)suitMarker)
			{
				facing.SetFacing(suitMarker.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH);
			}
			if (Time.time - startTime > 2.8f)
			{
				Run();
				Cleanup();
			}
		}

		private void Run()
		{
			if (base.reactor == null || suitMarker == null)
			{
				return;
			}
			GameObject reactor = base.reactor;
			Equipment equipment = reactor.GetComponent<MinionIdentity>().GetEquipment();
			bool flag = !equipment.IsSlotOccupied(Db.Get().AssignableSlots.Suit);
			reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(suitMarker.interactAnim);
			bool flag2 = false;
			Navigator component = reactor.GetComponent<Navigator>();
			bool flag3 = component != null && (component.flags & suitMarker.PathFlag) != 0;
			if (flag || flag3)
			{
				ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
				suitMarker.GetAttachedLockers(pooledList);
				foreach (SuitLocker item in pooledList)
				{
					KPrefabID fullyChargedOutfit = item.GetFullyChargedOutfit();
					if (fullyChargedOutfit != null && flag)
					{
						item.EquipTo(equipment);
						flag2 = true;
						break;
					}
					if (!flag && item.CanDropOffSuit())
					{
						item.UnequipFrom(equipment);
						flag2 = true;
						break;
					}
				}
				if (flag && !flag2)
				{
					SuitLocker suitLocker = null;
					float num = 0f;
					foreach (SuitLocker item2 in pooledList)
					{
						if (item2.GetSuitScore() > num)
						{
							suitLocker = item2;
							num = item2.GetSuitScore();
						}
					}
					if (suitLocker != null)
					{
						suitLocker.EquipTo(equipment);
						flag2 = true;
					}
				}
				pooledList.Recycle();
			}
			if (!flag2 && !flag)
			{
				Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
				assignable.Unassign();
				Notification notification = new Notification(MISC.NOTIFICATIONS.SUIT_DROPPED.NAME, NotificationType.BadMinor, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.SUIT_DROPPED.TOOLTIP);
				assignable.GetComponent<Notifier>().Add(notification);
			}
		}

		protected override void InternalEnd()
		{
			if (reactor != null)
			{
				reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(suitMarker.interactAnim);
			}
		}

		protected override void InternalCleanup()
		{
		}
	}

	[MyCmpGet]
	private Building building;

	private ScenePartitionerEntry partitionerEntry;

	private SuitMarkerReactable reactable;

	private bool hasAvailableSuit;

	[Serialize]
	private bool onlyTraverseIfUnequipAvailable;

	private Grid.SuitMarker.Flags gridFlags;

	private int cell;

	public Tag[] LockerTags;

	public PathFinder.PotentialPath.Flags PathFlag;

	public KAnimFile interactAnim = Assets.GetAnim("anim_equip_clothing_kanim");

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.OnOperationalChanged((bool)data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnRotatedDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.isRotated = ((Rotatable)data).IsRotated;
	});

	private bool OnlyTraverseIfUnequipAvailable
	{
		get
		{
			DebugUtil.Assert(onlyTraverseIfUnequipAvailable == ((gridFlags & Grid.SuitMarker.Flags.OnlyTraverseIfUnequipAvailable) != 0));
			return onlyTraverseIfUnequipAvailable;
		}
		set
		{
			onlyTraverseIfUnequipAvailable = value;
			UpdateGridFlag(Grid.SuitMarker.Flags.OnlyTraverseIfUnequipAvailable, onlyTraverseIfUnequipAvailable);
		}
	}

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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		OnlyTraverseIfUnequipAvailable = onlyTraverseIfUnequipAvailable;
		Debug.Assert(interactAnim != null, "interactAnim is null");
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		isOperational = GetComponent<Operational>().IsOperational;
		Subscribe(-592767678, OnOperationalChangedDelegate);
		isRotated = GetComponent<Rotatable>().IsRotated;
		Subscribe(-1643076535, OnRotatedDelegate);
		CreateNewReactable();
		cell = Grid.PosToCell(this);
		Grid.RegisterSuitMarker(cell);
		GetComponent<KAnimControllerBase>().Play("no_suit");
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits);
		RefreshTraverseIfUnequipStatusItem();
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
	}

	private void CreateNewReactable()
	{
		reactable = new SuitMarkerReactable(this);
	}

	public void GetAttachedLockers(List<SuitLocker> suit_lockers)
	{
		int num = (isRotated ? 1 : (-1));
		int num2 = 1;
		while (true)
		{
			int num3 = Grid.OffsetCell(cell, num2 * num, 0);
			GameObject gameObject = Grid.Objects[num3, 1];
			if (gameObject == null)
			{
				break;
			}
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (!(component == null))
			{
				if (!component.HasAnyTags(LockerTags))
				{
					break;
				}
				SuitLocker component2 = gameObject.GetComponent<SuitLocker>();
				if (component2 == null)
				{
					break;
				}
				if (!suit_lockers.Contains(component2))
				{
					suit_lockers.Add(component2);
				}
			}
			num2++;
		}
	}

	public static bool DoesTraversalDirectionRequireSuit(int source_cell, int dest_cell, Grid.SuitMarker.Flags flags)
	{
		bool flag = Grid.CellColumn(dest_cell) > Grid.CellColumn(source_cell);
		return flag == ((flags & Grid.SuitMarker.Flags.Rotated) == 0);
	}

	public bool DoesTraversalDirectionRequireSuit(int source_cell, int dest_cell)
	{
		return DoesTraversalDirectionRequireSuit(source_cell, dest_cell, gridFlags);
	}

	private void Update()
	{
		ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
		GetAttachedLockers(pooledList);
		int num = 0;
		int num2 = 0;
		KPrefabID x = null;
		foreach (SuitLocker item in pooledList)
		{
			if (item.CanDropOffSuit())
			{
				num++;
			}
			if (item.GetPartiallyChargedOutfit() != null)
			{
				num2++;
			}
			if (x == null)
			{
				x = item.GetStoredOutfit();
			}
		}
		pooledList.Recycle();
		bool flag = x != null;
		if (flag != hasAvailableSuit)
		{
			GetComponent<KAnimControllerBase>().Play(flag ? "off" : "no_suit");
			hasAvailableSuit = flag;
		}
		Grid.UpdateSuitMarker(cell, num2, num, gridFlags, PathFlag);
	}

	private void RefreshTraverseIfUnequipStatusItem()
	{
		if (OnlyTraverseIfUnequipAvailable)
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalOnlyWhenRoomAvailable);
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalAnytime);
		}
		else
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalOnlyWhenRoomAvailable);
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalAnytime);
		}
	}

	private void OnEnableTraverseIfUnequipAvailable()
	{
		OnlyTraverseIfUnequipAvailable = true;
		RefreshTraverseIfUnequipStatusItem();
	}

	private void OnDisableTraverseIfUnequipAvailable()
	{
		OnlyTraverseIfUnequipAvailable = false;
		RefreshTraverseIfUnequipStatusItem();
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

	private void OnOperationalChanged(bool isOperational)
	{
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
		this.isOperational = isOperational;
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo button = ((!OnlyTraverseIfUnequipAvailable) ? new KIconButtonMenu.ButtonInfo("action_clearance", UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ONLY_WHEN_ROOM_AVAILABLE.NAME, OnEnableTraverseIfUnequipAvailable, Action.NumActions, null, null, null, UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ONLY_WHEN_ROOM_AVAILABLE.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_clearance", UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ALWAYS.NAME, OnDisableTraverseIfUnequipAvailable, Action.NumActions, null, null, null, UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ALWAYS.TOOLTIP));
		Game.Instance.userMenu.AddButton(base.gameObject, button);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (base.isSpawned)
		{
			Grid.UnregisterSuitMarker(cell);
		}
		if (partitionerEntry != null)
		{
			partitionerEntry.Release();
			partitionerEntry = null;
		}
		if (reactable != null)
		{
			reactable.Cleanup();
		}
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), null);
	}
}
