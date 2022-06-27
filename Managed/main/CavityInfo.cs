using System.Collections.Generic;
using UnityEngine;

public class CavityInfo
{
	public HandleVector<int>.Handle handle;

	public bool dirty;

	public int numCells;

	public int maxX;

	public int maxY;

	public int minX;

	public int minY;

	public Room room;

	public List<KPrefabID> buildings = new List<KPrefabID>();

	public List<KPrefabID> plants = new List<KPrefabID>();

	public List<KPrefabID> creatures = new List<KPrefabID>();

	public List<KPrefabID> eggs = new List<KPrefabID>();

	public CavityInfo()
	{
		handle = HandleVector<int>.InvalidHandle;
		dirty = true;
	}

	public void AddBuilding(KPrefabID bc)
	{
		buildings.Add(bc);
		dirty = true;
	}

	public void AddPlants(KPrefabID plant)
	{
		plants.Add(plant);
		dirty = true;
	}

	public void RemoveFromCavity(KPrefabID id, List<KPrefabID> listToRemove)
	{
		int num = -1;
		for (int i = 0; i < listToRemove.Count; i++)
		{
			if (id.InstanceID == listToRemove[i].InstanceID)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			listToRemove.RemoveAt(num);
		}
	}

	public void OnEnter(object data)
	{
		foreach (KPrefabID building in buildings)
		{
			if (building != null)
			{
				building.Trigger(-832141045, data);
			}
		}
	}

	public Vector3 GetCenter()
	{
		return new Vector3(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);
	}
}
