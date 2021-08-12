using System.Collections.Generic;
using UnityEngine;

public class Room : IAssignableIdentity
{
	public CavityInfo cavity;

	public RoomType roomType;

	private List<KPrefabID> primary_buildings = new List<KPrefabID>();

	private List<Ownables> current_owners = new List<Ownables>();

	public List<KPrefabID> buildings => cavity.buildings;

	public List<KPrefabID> plants => cavity.plants;

	public string GetProperName()
	{
		return roomType.Name;
	}

	public List<Ownables> GetOwners()
	{
		current_owners.Clear();
		foreach (KPrefabID primaryEntity in GetPrimaryEntities())
		{
			if (!(primaryEntity != null))
			{
				continue;
			}
			Ownable component = primaryEntity.GetComponent<Ownable>();
			if (!(component != null) || component.assignee == null || component.assignee == this)
			{
				continue;
			}
			foreach (Ownables owner in component.assignee.GetOwners())
			{
				if (!current_owners.Contains(owner))
				{
					current_owners.Add(owner);
				}
			}
		}
		return current_owners;
	}

	public List<GameObject> GetBuildingsOnFloor()
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < buildings.Count; i++)
		{
			if (!Grid.Solid[Grid.PosToCell(buildings[i])] && Grid.Solid[Grid.CellBelow(Grid.PosToCell(buildings[i]))])
			{
				list.Add(buildings[i].gameObject);
			}
		}
		return list;
	}

	public Ownables GetSoleOwner()
	{
		List<Ownables> owners = GetOwners();
		if (owners.Count <= 0)
		{
			return null;
		}
		return owners[0];
	}

	public bool HasOwner(Assignables owner)
	{
		return GetOwners().Find((Ownables x) => x == owner) != null;
	}

	public int NumOwners()
	{
		return GetOwners().Count;
	}

	public List<KPrefabID> GetPrimaryEntities()
	{
		primary_buildings.Clear();
		RoomType roomType = this.roomType;
		if (roomType.primary_constraint != null)
		{
			foreach (KPrefabID building in buildings)
			{
				if (building != null && roomType.primary_constraint.building_criteria(building))
				{
					primary_buildings.Add(building);
				}
			}
			foreach (KPrefabID plant in plants)
			{
				if (plant != null && roomType.primary_constraint.building_criteria(plant))
				{
					primary_buildings.Add(plant);
				}
			}
		}
		return primary_buildings;
	}

	public void RetriggerBuildings()
	{
		foreach (KPrefabID building in buildings)
		{
			if (!(building == null))
			{
				building.Trigger(144050788, this);
			}
		}
		foreach (KPrefabID plant in plants)
		{
			if (!(plant == null))
			{
				plant.Trigger(144050788, this);
			}
		}
	}

	public bool IsNull()
	{
		return false;
	}

	public void CleanUp()
	{
		Game.Instance.assignmentManager.RemoveFromAllGroups(this);
	}
}
