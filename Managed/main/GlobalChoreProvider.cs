using System.Collections.Generic;

public class GlobalChoreProvider : ChoreProvider, IRender200ms
{
	public struct Fetch
	{
		public FetchChore chore;

		public int tagBitsHash;

		public int cost;

		public PrioritySetting priority;

		public Storage.FetchCategory category;

		public bool IsBetterThan(Fetch fetch)
		{
			if (category != fetch.category)
			{
				return false;
			}
			if (tagBitsHash != fetch.tagBitsHash)
			{
				return false;
			}
			if (chore.choreType != fetch.chore.choreType)
			{
				return false;
			}
			if (!chore.tagBits.AreEqual(ref fetch.chore.tagBits))
			{
				return false;
			}
			if (priority.priority_class > fetch.priority.priority_class)
			{
				return true;
			}
			if (priority.priority_class == fetch.priority.priority_class)
			{
				if (priority.priority_value > fetch.priority.priority_value)
				{
					return true;
				}
				if (priority.priority_value == fetch.priority.priority_value)
				{
					return cost <= fetch.cost;
				}
			}
			return false;
		}
	}

	private class FetchComparer : IComparer<Fetch>
	{
		public int Compare(Fetch a, Fetch b)
		{
			int num = b.priority.priority_class - a.priority.priority_class;
			if (num != 0)
			{
				return num;
			}
			int num2 = b.priority.priority_value - a.priority.priority_value;
			if (num2 != 0)
			{
				return num2;
			}
			return a.cost - b.cost;
		}
	}

	private struct FindTopPriorityTask : IWorkItem<object>
	{
		private int start;

		private int end;

		private List<Prioritizable> worldCollection;

		public bool found;

		public static bool abort;

		public FindTopPriorityTask(int start, int end, List<Prioritizable> worldCollection)
		{
			this.start = start;
			this.end = end;
			this.worldCollection = worldCollection;
			found = false;
		}

		public void Run(object context)
		{
			if (abort)
			{
				return;
			}
			for (int i = start; i != end && worldCollection.Count > i; i++)
			{
				if (!(worldCollection[i] == null) && worldCollection[i].IsTopPriority())
				{
					found = true;
					break;
				}
			}
			if (found)
			{
				abort = true;
			}
		}
	}

	public static GlobalChoreProvider Instance;

	public List<FetchChore> fetchChores = new List<FetchChore>();

	public List<Fetch> fetches = new List<Fetch>();

	private static readonly FetchComparer Comparer = new FetchComparer();

	private ClearableManager clearableManager;

	private TagBits storageFetchableBits;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		clearableManager = new ClearableManager();
	}

	public override void AddChore(Chore chore)
	{
		base.AddChore(chore);
		if (chore is FetchChore item)
		{
			fetchChores.Add(item);
		}
	}

	public override void RemoveChore(Chore chore)
	{
		base.RemoveChore(chore);
		if (chore is FetchChore item)
		{
			fetchChores.Remove(item);
		}
	}

	public void UpdateFetches(PathProber path_prober)
	{
		fetches.Clear();
		Navigator component = path_prober.GetComponent<Navigator>();
		foreach (FetchChore fetchChore in fetchChores)
		{
			if (fetchChore.driver != null || (fetchChore.automatable != null && fetchChore.automatable.GetAutomationOnly()))
			{
				continue;
			}
			Storage destination = fetchChore.destination;
			if (!(destination == null))
			{
				int navigationCost = component.GetNavigationCost(destination);
				if (navigationCost != -1)
				{
					fetches.Add(new Fetch
					{
						chore = fetchChore,
						tagBitsHash = fetchChore.tagBitsHash,
						cost = navigationCost,
						priority = fetchChore.masterPriority,
						category = destination.fetchCategory
					});
				}
			}
		}
		if (fetches.Count <= 0)
		{
			return;
		}
		fetches.Sort(Comparer);
		int i = 1;
		int num = 0;
		for (; i < fetches.Count; i++)
		{
			if (!fetches[num].IsBetterThan(fetches[i]))
			{
				num++;
				fetches[num] = fetches[i];
			}
		}
		fetches.RemoveRange(num + 1, fetches.Count - num - 1);
	}

	public override void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded, List<Chore.Precondition.Context> failed_contexts)
	{
		base.CollectChores(consumer_state, succeeded, failed_contexts);
		clearableManager.CollectChores(consumer_state, succeeded, failed_contexts);
		foreach (Fetch fetch in fetches)
		{
			fetch.chore.CollectChoresFromGlobalChoreProvider(consumer_state, succeeded, failed_contexts, is_attempting_override: false);
		}
	}

	public HandleVector<int>.Handle RegisterClearable(Clearable clearable)
	{
		return clearableManager.RegisterClearable(clearable);
	}

	public void UnregisterClearable(HandleVector<int>.Handle handle)
	{
		clearableManager.UnregisterClearable(handle);
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		Instance = null;
	}

	public void Render200ms(float dt)
	{
		UpdateStorageFetchableBits();
	}

	private void UpdateStorageFetchableBits()
	{
		ChoreType storageFetch = Db.Get().ChoreTypes.StorageFetch;
		ChoreType foodFetch = Db.Get().ChoreTypes.FoodFetch;
		storageFetchableBits.ClearAll();
		foreach (FetchChore fetchChore in fetchChores)
		{
			if ((fetchChore.choreType == storageFetch || fetchChore.choreType == foodFetch) && (bool)fetchChore.destination)
			{
				int cell = Grid.PosToCell(fetchChore.destination);
				if (MinionGroupProber.Get().IsReachable(cell, fetchChore.destination.GetOffsets(cell)))
				{
					storageFetchableBits.Or(ref fetchChore.tagBits);
				}
			}
		}
	}

	public bool ClearableHasDestination(Pickupable pickupable)
	{
		KPrefabID kPrefabID = pickupable.KPrefabID;
		kPrefabID.UpdateTagBits();
		return kPrefabID.HasAnyTags_AssumeLaundered(ref storageFetchableBits);
	}
}
