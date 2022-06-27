using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackerTool : KMonoBehaviour
{
	public static TrackerTool Instance;

	private List<WorldTracker> worldTrackers = new List<WorldTracker>();

	private Dictionary<MinionIdentity, List<MinionTracker>> minionTrackers = new Dictionary<MinionIdentity, List<MinionTracker>>();

	private int updatingWorldTracker;

	private int updatingMinionTracker;

	public bool trackerActive = true;

	private int numUpdatesPerFrame = 50;

	protected override void OnSpawn()
	{
		Instance = this;
		base.OnSpawn();
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			AddNewWorldTrackers(worldContainer.id);
		}
		foreach (object liveMinionIdentity in Components.LiveMinionIdentities)
		{
			AddMinionTrackers((MinionIdentity)liveMinionIdentity);
		}
		Components.LiveMinionIdentities.OnAdd += AddMinionTrackers;
		ClusterManager.Instance.Subscribe(-1280433810, Refresh);
		ClusterManager.Instance.Subscribe(-1078710002, RemoveWorld);
	}

	protected override void OnForcedCleanUp()
	{
		Instance = null;
		base.OnForcedCleanUp();
	}

	private void AddMinionTrackers(MinionIdentity identity)
	{
		minionTrackers.Add(identity, new List<MinionTracker>());
		identity.Subscribe(1969584890, delegate
		{
			minionTrackers.Remove(identity);
		});
	}

	private void Refresh(object data)
	{
		int worldID = (int)data;
		AddNewWorldTrackers(worldID);
	}

	private void RemoveWorld(object data)
	{
		int world_id = (int)data;
		worldTrackers.RemoveAll((WorldTracker match) => match.WorldID == world_id);
	}

	public bool IsRocketInterior(int worldID)
	{
		return ClusterManager.Instance.GetWorld(worldID).IsModuleInterior;
	}

	private void AddNewWorldTrackers(int worldID)
	{
		worldTrackers.Add(new StressTracker(worldID));
		worldTrackers.Add(new KCalTracker(worldID));
		worldTrackers.Add(new IdleTracker(worldID));
		worldTrackers.Add(new BreathabilityTracker(worldID));
		worldTrackers.Add(new PowerUseTracker(worldID));
		worldTrackers.Add(new BatteryTracker(worldID));
		worldTrackers.Add(new CropTracker(worldID));
		worldTrackers.Add(new WorkingToiletTracker(worldID));
		worldTrackers.Add(new RadiationTracker(worldID));
		if (ClusterManager.Instance.GetWorld(worldID).IsModuleInterior)
		{
			worldTrackers.Add(new RocketFuelTracker(worldID));
			worldTrackers.Add(new RocketOxidizerTracker(worldID));
		}
		for (int i = 0; i < Db.Get().ChoreGroups.Count; i++)
		{
			worldTrackers.Add(new WorkTimeTracker(worldID, Db.Get().ChoreGroups[i]));
			worldTrackers.Add(new ChoreCountTracker(worldID, Db.Get().ChoreGroups[i]));
		}
		worldTrackers.Add(new AllChoresCountTracker(worldID));
		worldTrackers.Add(new AllWorkTimeTracker(worldID));
		foreach (Tag calorieCategory in GameTags.CalorieCategories)
		{
			worldTrackers.Add(new ResourceTracker(worldID, calorieCategory));
			foreach (GameObject item in Assets.GetPrefabsWithTag(calorieCategory))
			{
				AddResourceTracker(worldID, item.GetComponent<KPrefabID>().PrefabTag);
			}
		}
		foreach (Tag unitCategory in GameTags.UnitCategories)
		{
			worldTrackers.Add(new ResourceTracker(worldID, unitCategory));
			foreach (GameObject item2 in Assets.GetPrefabsWithTag(unitCategory))
			{
				AddResourceTracker(worldID, item2.GetComponent<KPrefabID>().PrefabTag);
			}
		}
		foreach (Tag materialCategory in GameTags.MaterialCategories)
		{
			worldTrackers.Add(new ResourceTracker(worldID, materialCategory));
			foreach (GameObject item3 in Assets.GetPrefabsWithTag(materialCategory))
			{
				AddResourceTracker(worldID, item3.GetComponent<KPrefabID>().PrefabTag);
			}
		}
		foreach (Tag otherEntityTag in GameTags.OtherEntityTags)
		{
			worldTrackers.Add(new ResourceTracker(worldID, otherEntityTag));
			foreach (GameObject item4 in Assets.GetPrefabsWithTag(otherEntityTag))
			{
				AddResourceTracker(worldID, item4.GetComponent<KPrefabID>().PrefabTag);
			}
		}
		foreach (GameObject item5 in Assets.GetPrefabsWithTag(GameTags.CookingIngredient))
		{
			AddResourceTracker(worldID, item5.GetComponent<KPrefabID>().PrefabTag);
		}
		foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
		{
			AddResourceTracker(worldID, allFoodType.Id);
		}
		foreach (Element element in ElementLoader.elements)
		{
			AddResourceTracker(worldID, element.tag);
		}
	}

	private void AddResourceTracker(int worldID, Tag tag)
	{
		if (worldTrackers.Find((WorldTracker match) => match is ResourceTracker && ((ResourceTracker)match).WorldID == worldID && ((ResourceTracker)match).tag == tag) == null)
		{
			worldTrackers.Add(new ResourceTracker(worldID, tag));
		}
	}

	public ResourceTracker GetResourceStatistic(int worldID, Tag tag)
	{
		return (ResourceTracker)worldTrackers.Find((WorldTracker match) => match is ResourceTracker && ((ResourceTracker)match).WorldID == worldID && ((ResourceTracker)match).tag == tag);
	}

	public WorldTracker GetWorldTracker<T>(int worldID) where T : WorldTracker
	{
		return (T)worldTrackers.Find((WorldTracker match) => match is T && ((T)match).WorldID == worldID);
	}

	public ChoreCountTracker GetChoreGroupTracker(int worldID, ChoreGroup choreGroup)
	{
		return (ChoreCountTracker)worldTrackers.Find((WorldTracker match) => match is ChoreCountTracker && ((ChoreCountTracker)match).WorldID == worldID && ((ChoreCountTracker)match).choreGroup == choreGroup);
	}

	public WorkTimeTracker GetWorkTimeTracker(int worldID, ChoreGroup choreGroup)
	{
		return (WorkTimeTracker)worldTrackers.Find((WorldTracker match) => match is WorkTimeTracker && ((WorkTimeTracker)match).WorldID == worldID && ((WorkTimeTracker)match).choreGroup == choreGroup);
	}

	public MinionTracker GetMinionTracker<T>(MinionIdentity identity) where T : MinionTracker
	{
		return (T)minionTrackers[identity].Find((MinionTracker match) => match is T);
	}

	public void Update()
	{
		if (SpeedControlScreen.Instance.IsPaused || !trackerActive)
		{
			return;
		}
		if (minionTrackers.Count > 0)
		{
			updatingMinionTracker++;
			if (updatingMinionTracker >= minionTrackers.Count)
			{
				updatingMinionTracker = 0;
			}
			KeyValuePair<MinionIdentity, List<MinionTracker>> keyValuePair = minionTrackers.ElementAt(updatingMinionTracker);
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				keyValuePair.Value[i].UpdateData();
			}
		}
		if (worldTrackers.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < numUpdatesPerFrame; j++)
		{
			updatingWorldTracker++;
			if (updatingWorldTracker >= worldTrackers.Count)
			{
				updatingWorldTracker = 0;
			}
			worldTrackers[updatingWorldTracker].UpdateData();
		}
	}
}
