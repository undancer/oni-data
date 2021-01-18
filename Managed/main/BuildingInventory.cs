using System.Collections.Generic;

public class BuildingInventory : KMonoBehaviour
{
	public static BuildingInventory Instance;

	private Dictionary<Tag, HashSet<BuildingComplete>> Buildings = new Dictionary<Tag, HashSet<BuildingComplete>>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	public HashSet<BuildingComplete> GetBuildings(Tag tag)
	{
		return Buildings[tag];
	}

	public int BuildingCount(Tag tag)
	{
		if (!Buildings.ContainsKey(tag))
		{
			return 0;
		}
		return Buildings[tag].Count;
	}

	public int BuildingCountForWorld_BAD_PERF(Tag tag, int worldId)
	{
		if (!Buildings.ContainsKey(tag))
		{
			return 0;
		}
		int num = 0;
		foreach (BuildingComplete item in Buildings[tag])
		{
			if (item.GetMyWorldId() == worldId)
			{
				num++;
			}
		}
		return num;
	}

	public void RegisterBuilding(BuildingComplete building)
	{
		Tag prefabTag = building.prefabid.PrefabTag;
		if (!Buildings.TryGetValue(prefabTag, out var value))
		{
			value = new HashSet<BuildingComplete>();
			Buildings[prefabTag] = value;
		}
		value.Add(building);
	}

	public void UnregisterBuilding(BuildingComplete building)
	{
		Tag prefabTag = building.prefabid.PrefabTag;
		if (!Buildings.TryGetValue(prefabTag, out var value))
		{
			DebugUtil.DevLogError($"Unregistering building {prefabTag} before it was registered.");
			return;
		}
		bool test = value.Remove(building);
		DebugUtil.DevAssert(test, $"Building {prefabTag} was not found to be removed");
	}
}
