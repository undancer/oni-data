using System.Collections.Generic;
using KSerialization;

public class PassengerRocketModule : KMonoBehaviour
{
	public enum RequestCrewState
	{
		Release,
		Auto,
		Request
	}

	[Serialize]
	private RequestCrewState passengersRequested = RequestCrewState.Release;

	private static readonly EventSystem.IntraObjectHandler<PassengerRocketModule> OnRocketOnGroundTagDelegate = GameUtil.CreateHasTagHandler(GameTags.RocketOnGround, delegate(PassengerRocketModule component, object data)
	{
		component.RefreshOrders();
	});

	private static EventSystem.IntraObjectHandler<PassengerRocketModule> RefreshDelegate = new EventSystem.IntraObjectHandler<PassengerRocketModule>(delegate(PassengerRocketModule cmp, object data)
	{
		cmp.RefreshOrders();
	});

	private static EventSystem.IntraObjectHandler<PassengerRocketModule> OnLaunchDelegate = new EventSystem.IntraObjectHandler<PassengerRocketModule>(delegate(PassengerRocketModule component, object data)
	{
		component.ClearMinionAssignments(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PassengerRocketModule> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<PassengerRocketModule>(delegate(PassengerRocketModule component, object data)
	{
		component.OnReachableChanged(data);
	});

	public RequestCrewState PassengersRequested => passengersRequested;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.Subscribe(-1123234494, OnAssignmentGroupChanged);
		GameUtil.SubscribeToTags(this, OnRocketOnGroundTagDelegate, triggerImmediately: false);
		Subscribe(1655598572, RefreshDelegate);
		Subscribe(-71801987, RefreshDelegate);
		Subscribe(-1277991738, OnLaunchDelegate);
		Subscribe(-1432940121, OnReachableChangedDelegate);
		ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(GetComponent<Workable>());
		instance.StartSM();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.Unsubscribe(-1123234494, OnAssignmentGroupChanged);
		base.OnCleanUp();
	}

	private void OnAssignmentGroupChanged(object data)
	{
		RefreshOrders();
	}

	private void OnReachableChanged(object data)
	{
		bool flag = (bool)data;
		KSelectable component = GetComponent<KSelectable>();
		if (flag)
		{
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.PassengerModuleUnreachable);
		}
		else
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.PassengerModuleUnreachable, this);
		}
	}

	public void RequestCrewBoard(RequestCrewState requestBoard)
	{
		passengersRequested = requestBoard;
		RefreshOrders();
	}

	public bool ShouldCrewGetIn()
	{
		return passengersRequested == RequestCrewState.Request || (passengersRequested == RequestCrewState.Auto && GetComponent<RocketModule>().CraftInterface.CheckCrewShouldBoard());
	}

	private void RefreshOrders()
	{
		if (this.HasTag(GameTags.RocketNotOnGround) || !GetComponent<ClustercraftExteriorDoor>().HasTargetWorld())
		{
			return;
		}
		int cell = GetComponent<NavTeleporter>().GetCell();
		int num = GetComponent<ClustercraftExteriorDoor>().TargetCell();
		bool flag = ShouldCrewGetIn();
		if (flag)
		{
			foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
			{
				bool flag2 = Game.Instance.assignmentManager.assignment_groups[GetComponent<AssignmentGroupController>().AssignmentGroupID].HasMember(item.assignableProxy.Get());
				bool flag3 = item.GetMyWorldId() == Grid.WorldIdx[num];
				if (!flag3 && flag2)
				{
					item.GetSMI<RocketPassengerMonitor.Instance>().SetMoveTarget(num);
				}
				else if (flag3 && !flag2)
				{
					item.GetSMI<RocketPassengerMonitor.Instance>().SetMoveTarget(cell);
				}
				else
				{
					item.GetSMI<RocketPassengerMonitor.Instance>().ClearMoveTarget(num);
				}
			}
		}
		else
		{
			foreach (MinionIdentity item2 in Components.LiveMinionIdentities.Items)
			{
				item2.GetSMI<RocketPassengerMonitor.Instance>().ClearMoveTarget(cell);
				item2.GetSMI<RocketPassengerMonitor.Instance>().ClearMoveTarget(num);
			}
		}
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			RefreshAccessStatus(Components.LiveMinionIdentities[i], flag);
		}
	}

	private void RefreshAccessStatus(MinionIdentity minion, bool restrict)
	{
		ClustercraftInteriorDoor interiorDoor = GetComponent<ClustercraftExteriorDoor>().GetInteriorDoor();
		AccessControl component = GetComponent<AccessControl>();
		AccessControl component2 = interiorDoor.GetComponent<AccessControl>();
		if (restrict)
		{
			if (Game.Instance.assignmentManager.assignment_groups[GetComponent<AssignmentGroupController>().AssignmentGroupID].HasMember(minion.assignableProxy.Get()))
			{
				component.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Both);
				component2.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Neither);
			}
			else
			{
				component.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Neither);
				component2.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Both);
			}
		}
		else
		{
			component.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Both);
			component2.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Both);
		}
	}

	public bool CheckPassengersBoarded()
	{
		ICollection<IAssignableIdentity> members = GetComponent<AssignmentGroupController>().GetMembers();
		if (members.Count == 0)
		{
			return false;
		}
		bool flag = false;
		foreach (IAssignableIdentity item in members)
		{
			MinionAssignablesProxy minionAssignablesProxy = (MinionAssignablesProxy)item;
			if (minionAssignablesProxy != null)
			{
				MinionResume component = minionAssignablesProxy.GetTargetGameObject().GetComponent<MinionResume>();
				if (component != null && component.HasPerk(Db.Get().SkillPerks.CanUseRocketControlStation))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			return false;
		}
		foreach (IAssignableIdentity item2 in members)
		{
			if (((MinionAssignablesProxy)item2).GetTargetGameObject().GetMyWorldId() != Grid.WorldIdx[GetComponent<ClustercraftExteriorDoor>().TargetCell()])
			{
				return false;
			}
		}
		return true;
	}

	public bool CheckExtraPassengers()
	{
		ClustercraftExteriorDoor component = GetComponent<ClustercraftExteriorDoor>();
		if (component.HasTargetWorld())
		{
			byte worldId = Grid.WorldIdx[component.TargetCell()];
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(worldId);
			for (int i = 0; i < worldItems.Count; i++)
			{
				if (!Game.Instance.assignmentManager.assignment_groups[GetComponent<AssignmentGroupController>().AssignmentGroupID].HasMember(worldItems[i].assignableProxy.Get()))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void RemoveRocketPassenger(MinionIdentity minion)
	{
		if (minion != null)
		{
			string assignmentGroupID = GetComponent<AssignmentGroupController>().AssignmentGroupID;
			MinionAssignablesProxy member = minion.assignableProxy.Get();
			if (Game.Instance.assignmentManager.assignment_groups[assignmentGroupID].HasMember(member))
			{
				Game.Instance.assignmentManager.assignment_groups[assignmentGroupID].RemoveMember(member);
			}
			RefreshOrders();
		}
	}

	public void ClearMinionAssignments(object data)
	{
		string assignmentGroupID = GetComponent<AssignmentGroupController>().AssignmentGroupID;
		foreach (IAssignableIdentity member in Game.Instance.assignmentManager.assignment_groups[assignmentGroupID].GetMembers())
		{
			Game.Instance.assignmentManager.RemoveFromWorld(member, this.GetMyWorldId());
		}
	}
}
