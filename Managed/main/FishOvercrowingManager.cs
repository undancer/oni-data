using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FishOvercrowingManager")]
public class FishOvercrowingManager : KMonoBehaviour, ISim1000ms
{
	private struct Cell
	{
		public int version;

		public int cavityId;
	}

	private struct FishInfo
	{
		public int cell;

		public FishOvercrowdingMonitor.Instance fish;
	}

	private struct CavityInfo
	{
		public int fishCount;

		public int cellCount;
	}

	public static FishOvercrowingManager Instance;

	private List<FishOvercrowdingMonitor.Instance> fishes = new List<FishOvercrowdingMonitor.Instance>();

	private Dictionary<int, CavityInfo> cavityIdToCavityInfo = new Dictionary<int, CavityInfo>();

	private Dictionary<int, int> cellToFishCount = new Dictionary<int, int>();

	private Cell[] cells;

	private int versionCounter = 1;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		cells = new Cell[Grid.CellCount];
	}

	public void Add(FishOvercrowdingMonitor.Instance fish)
	{
		fishes.Add(fish);
	}

	public void Remove(FishOvercrowdingMonitor.Instance fish)
	{
		fishes.Remove(fish);
	}

	public void Sim1000ms(float dt)
	{
		int num = versionCounter++;
		int num2 = 1;
		cavityIdToCavityInfo.Clear();
		cellToFishCount.Clear();
		ListPool<FishInfo, FishOvercrowingManager>.PooledList pooledList = ListPool<FishInfo, FishOvercrowingManager>.Allocate();
		foreach (FishOvercrowdingMonitor.Instance fish in fishes)
		{
			int num3 = Grid.PosToCell(fish);
			if (Grid.IsValidCell(num3))
			{
				FishInfo fishInfo = default(FishInfo);
				fishInfo.cell = num3;
				fishInfo.fish = fish;
				FishInfo item = fishInfo;
				pooledList.Add(item);
				int value = 0;
				cellToFishCount.TryGetValue(num3, out value);
				value++;
				cellToFishCount[num3] = value;
			}
		}
		foreach (FishInfo item2 in pooledList)
		{
			ListPool<int, FishOvercrowingManager>.PooledList pooledList2 = ListPool<int, FishOvercrowingManager>.Allocate();
			pooledList2.Add(item2.cell);
			int num4 = 0;
			int num5 = num2++;
			while (num4 < pooledList2.Count)
			{
				int num6 = pooledList2[num4++];
				if (!Grid.IsValidCell(num6))
				{
					continue;
				}
				Cell cell = cells[num6];
				if (cell.version != num && Grid.IsLiquid(num6))
				{
					cell.cavityId = num5;
					cell.version = num;
					int value2 = 0;
					cellToFishCount.TryGetValue(num6, out value2);
					CavityInfo value3 = default(CavityInfo);
					if (!cavityIdToCavityInfo.TryGetValue(num5, out value3))
					{
						value3 = default(CavityInfo);
					}
					value3.fishCount += value2;
					value3.cellCount++;
					cavityIdToCavityInfo[num5] = value3;
					pooledList2.Add(Grid.CellLeft(num6));
					pooledList2.Add(Grid.CellRight(num6));
					pooledList2.Add(Grid.CellAbove(num6));
					pooledList2.Add(Grid.CellBelow(num6));
					cells[num6] = cell;
				}
			}
			pooledList2.Recycle();
		}
		foreach (FishInfo item3 in pooledList)
		{
			Cell cell2 = cells[item3.cell];
			CavityInfo value4 = default(CavityInfo);
			cavityIdToCavityInfo.TryGetValue(cell2.cavityId, out value4);
			item3.fish.SetOvercrowdingInfo(value4.cellCount, value4.fishCount);
		}
		pooledList.Recycle();
	}
}
