using KSerialization;
using UnityEngine;

public class ClustercraftExteriorDoor : KMonoBehaviour
{
	public string interiorTemplateName;

	private ClustercraftInteriorDoor targetDoor;

	[Serialize]
	private int targetWorldId = -1;

	private static readonly EventSystem.IntraObjectHandler<ClustercraftExteriorDoor> OnLaunchDelegate = new EventSystem.IntraObjectHandler<ClustercraftExteriorDoor>(delegate(ClustercraftExteriorDoor component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ClustercraftExteriorDoor> OnLandDelegate = new EventSystem.IntraObjectHandler<ClustercraftExteriorDoor>(delegate(ClustercraftExteriorDoor component, object data)
	{
		component.OnLand(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (targetWorldId < 0)
		{
			GameObject gameObject = GetComponent<RocketModuleCluster>().CraftInterface.gameObject;
			WorldContainer worldContainer = ClusterManager.Instance.CreateRocketInteriorWorld(gameObject, interiorTemplateName, delegate
			{
				PairWithInteriorDoor();
			});
			if (worldContainer != null)
			{
				targetWorldId = worldContainer.id;
			}
		}
		else
		{
			PairWithInteriorDoor();
		}
		Subscribe(-1277991738, OnLaunchDelegate);
		Subscribe(-887025858, OnLandDelegate);
	}

	protected override void OnCleanUp()
	{
		ClusterManager.Instance.DestoryRocketInteriorWorld(targetWorldId, this);
		base.OnCleanUp();
	}

	private void PairWithInteriorDoor()
	{
		foreach (ClustercraftInteriorDoor clusterCraftInteriorDoor in Components.ClusterCraftInteriorDoors)
		{
			if (clusterCraftInteriorDoor.GetMyWorldId() == targetWorldId)
			{
				SetTarget(clusterCraftInteriorDoor);
				break;
			}
		}
		if (targetDoor == null)
		{
			Debug.LogWarning("No ClusterCraftInteriorDoor found on world");
		}
		WorldContainer targetWorld = GetTargetWorld();
		int myWorldId = this.GetMyWorldId();
		if (targetWorld != null && myWorldId != -1)
		{
			targetWorld.SetParentIdx(myWorldId);
		}
		if (base.gameObject.GetComponent<KSelectable>().IsSelected)
		{
			RocketModuleSideScreen.instance.UpdateButtonStates();
		}
	}

	public void SetTarget(ClustercraftInteriorDoor target)
	{
		targetDoor = target;
		target.GetComponent<AssignmentGroupController>().SetGroupID(GetComponent<AssignmentGroupController>().AssignmentGroupID);
		GetComponent<NavTeleporter>().TwoWayTarget(target.GetComponent<NavTeleporter>());
	}

	public bool HasTargetWorld()
	{
		return targetDoor != null;
	}

	public WorldContainer GetTargetWorld()
	{
		Debug.Assert(targetDoor != null, "Clustercraft Exterior Door has no targetDoor");
		return targetDoor.GetMyWorld();
	}

	public void FerryMinion(GameObject minion)
	{
		Vector3 b = Vector3.left * 3f;
		minion.transform.SetPosition(Grid.CellToPos(Grid.PosToCell(targetDoor.transform.position + b), CellAlignment.Bottom, Grid.SceneLayer.Move));
		ClusterManager.Instance.MigrateMinion(minion.GetComponent<MinionIdentity>(), targetDoor.GetMyWorldId());
	}

	private void OnLaunch(object data)
	{
		GetComponent<NavTeleporter>().EnableTwoWayTarget(enable: false);
		WorldContainer targetWorld = GetTargetWorld();
		if (targetWorld != null)
		{
			targetWorld.SetParentIdx(targetWorld.id);
		}
	}

	private void OnLand(object data)
	{
		GetComponent<NavTeleporter>().EnableTwoWayTarget(enable: true);
		WorldContainer targetWorld = GetTargetWorld();
		if (targetWorld != null)
		{
			int myWorldId = this.GetMyWorldId();
			targetWorld.SetParentIdx(myWorldId);
		}
	}

	public int TargetCell()
	{
		return targetDoor.GetComponent<NavTeleporter>().GetCell();
	}

	public ClustercraftInteriorDoor GetInteriorDoor()
	{
		return targetDoor;
	}
}
