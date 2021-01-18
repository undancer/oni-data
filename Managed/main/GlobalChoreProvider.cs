using System.Collections.Generic;

public class GlobalChoreProvider : ChoreProvider, ISim200ms, IRender200ms
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

		public bool found;

		public static bool abort;

		public FindTopPriorityTask(int start, int end)
		{
			this.start = start;
			this.end = end;
			found = false;
		}

		public void Run(object context)
		{
			if (abort)
			{
				return;
			}
			for (int i = start; i != end; i++)
			{
				if (Components.Prioritizables.Items[i].IsTopPriority())
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

	private static WorkItemCollection<FindTopPriorityTask, object> find_top_priority_job = new WorkItemCollection<FindTopPriorityTask, object>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		clearableManager = new ClearableManager();
	}

	public override void AddChore(Chore chore)
	{
		base.AddChore(chore);
		FetchChore fetchChore = chore as FetchChore;
		if (fetchChore != null)
		{
			fetchChores.Add(fetchChore);
		}
	}

	public override void RemoveChore(Chore chore)
	{
		base.RemoveChore(chore);
		FetchChore fetchChore = chore as FetchChore;
		if (fetchChore != null)
		{
			fetchChores.Remove(fetchChore);
		}
	}

	public void UpdateFetches(PathProber path_prober)
	{
		fetches.Clear();
		Navigator component = path_prober.GetComponent<Navigator>();
		Fetch item;
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
					List<Fetch> list = fetches;
					item = new Fetch
					{
						chore = fetchChore,
						tagBitsHash = fetchChore.tagBitsHash,
						cost = navigationCost,
						priority = fetchChore.masterPriority,
						category = destination.fetchCategory
					};
					list.Add(item);
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
			item = fetches[num];
			if (!item.IsBetterThan(fetches[i]))
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

	public void Sim200ms(float time_delta)
	{
		find_top_priority_job.Reset(null);
		FindTopPriorityTask.abort = false;
		int num = 512;
		for (int i = 0; i < Components.Prioritizables.Items.Count; i += num)
		{
			int num2 = i + num;
			if (Components.Prioritizables.Items.Count < num2)
			{
				num2 = Components.Prioritizables.Items.Count;
			}
			find_top_priority_job.Add(new FindTopPriorityTask(i, num2));
		}
		GlobalJobManager.Run(find_top_priority_job);
		bool on = false;
		for (int j = 0; j != find_top_priority_job.Count; j++)
		{
			if (find_top_priority_job.GetWorkItem(j).found)
			{
				on = true;
				break;
			}
		}
		VignetteManager.Instance.Get().HasTopPriorityChore(on);
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
