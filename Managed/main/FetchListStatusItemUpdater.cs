using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FetchListStatusItemUpdater")]
public class FetchListStatusItemUpdater : KMonoBehaviour, IRender200ms
{
	public static FetchListStatusItemUpdater instance;

	private List<FetchList2> fetchLists = new List<FetchList2>();

	private int currentIteratingIndex = 0;

	private int maxIteratingCount = 100;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
	}

	public void AddFetchList(FetchList2 fetch_list)
	{
		fetchLists.Add(fetch_list);
	}

	public void RemoveFetchList(FetchList2 fetch_list)
	{
		fetchLists.Remove(fetch_list);
	}

	public void Render200ms(float dt)
	{
		DictionaryPool<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary = DictionaryPool<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList, FetchListStatusItemUpdater>.Allocate();
		int num = Math.Min(maxIteratingCount, fetchLists.Count - currentIteratingIndex);
		for (int i = currentIteratingIndex; i < num; i++)
		{
			FetchList2 fetchList = fetchLists[i];
			if (!(fetchList.Destination == null))
			{
				ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList value = null;
				int instanceID = fetchList.Destination.GetInstanceID();
				if (!pooledDictionary.TryGetValue(instanceID, out value))
				{
					value = (pooledDictionary[instanceID] = ListPool<FetchList2, FetchListStatusItemUpdater>.Allocate());
				}
				value.Add(fetchList);
			}
		}
		currentIteratingIndex += num;
		if (currentIteratingIndex >= fetchLists.Count)
		{
			currentIteratingIndex = 0;
		}
		DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary2 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
		DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary3 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
		foreach (KeyValuePair<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList> item in pooledDictionary)
		{
			ListPool<Tag, FetchListStatusItemUpdater>.PooledList pooledList2 = ListPool<Tag, FetchListStatusItemUpdater>.Allocate();
			Storage destination = item.Value[0].Destination;
			foreach (FetchList2 item2 in item.Value)
			{
				item2.UpdateRemaining();
				Dictionary<Tag, float> remaining = item2.GetRemaining();
				foreach (KeyValuePair<Tag, float> item3 in remaining)
				{
					if (!pooledList2.Contains(item3.Key))
					{
						pooledList2.Add(item3.Key);
					}
				}
			}
			ListPool<Pickupable, FetchListStatusItemUpdater>.PooledList pooledList3 = ListPool<Pickupable, FetchListStatusItemUpdater>.Allocate();
			foreach (GameObject item4 in destination.items)
			{
				if (!(item4 == null))
				{
					Pickupable component = item4.GetComponent<Pickupable>();
					if (!(component == null))
					{
						pooledList3.Add(component);
					}
				}
			}
			DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary4 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
			foreach (Tag item5 in pooledList2)
			{
				float num2 = 0f;
				foreach (Pickupable item6 in pooledList3)
				{
					if (item6.KPrefabID.HasTag(item5))
					{
						num2 += item6.TotalAmount;
					}
				}
				pooledDictionary4[item5] = num2;
			}
			foreach (Tag item7 in pooledList2)
			{
				if (!pooledDictionary2.ContainsKey(item7))
				{
					pooledDictionary2[item7] = destination.GetMyWorld().worldInventory.GetTotalAmount(item7, includeRelatedWorlds: true);
				}
				if (!pooledDictionary3.ContainsKey(item7))
				{
					pooledDictionary3[item7] = destination.GetMyWorld().worldInventory.GetAmount(item7, includeRelatedWorlds: true);
				}
			}
			foreach (FetchList2 item8 in item.Value)
			{
				bool should_add = false;
				bool should_add2 = true;
				bool should_add3 = false;
				Dictionary<Tag, float> remaining2 = item8.GetRemaining();
				foreach (KeyValuePair<Tag, float> item9 in remaining2)
				{
					Tag key = item9.Key;
					float value2 = item9.Value;
					float num3 = pooledDictionary4[key];
					float b = pooledDictionary2[key];
					float num4 = pooledDictionary3[key];
					float num5 = Mathf.Min(value2, b);
					float num6 = num4 + num5;
					float minimumAmount = item8.GetMinimumAmount(key);
					if (num3 + num6 < minimumAmount)
					{
						should_add = true;
					}
					if (num6 < value2)
					{
						should_add2 = false;
					}
					if (num3 + num6 > value2 && value2 > num6)
					{
						should_add3 = true;
					}
				}
				item8.UpdateStatusItem(Db.Get().BuildingStatusItems.WaitingForMaterials, ref item8.waitingForMaterialsHandle, should_add2);
				item8.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, ref item8.materialsUnavailableHandle, should_add);
				item8.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailableForRefill, ref item8.materialsUnavailableForRefillHandle, should_add3);
			}
			pooledDictionary4.Recycle();
			pooledList3.Recycle();
			pooledList2.Recycle();
			item.Value.Recycle();
		}
		pooledDictionary3.Recycle();
		pooledDictionary2.Recycle();
		pooledDictionary.Recycle();
	}
}
