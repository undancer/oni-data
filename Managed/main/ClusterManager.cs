using System;
using System.Collections.Generic;
using KSerialization;
using ProcGenGame;
using TUNING;
using UnityEngine;

public class ClusterManager : KMonoBehaviour, ISaveLoadable
{
	public static int MAX_ROCKET_INTERIOR_COUNT = 16;

	public static ClusterManager Instance;

	private ClusterGrid m_grid;

	[Serialize]
	private int m_numRings = 9;

	[Serialize]
	private int activeWorldIdx;

	public static byte INVALID_WORLD_IDX = byte.MaxValue;

	public static Color[] worldColors = new Color[6]
	{
		Color.HSVToRGB(0.15f, 0.3f, 0.5f),
		Color.HSVToRGB(0.3f, 0.3f, 0.5f),
		Color.HSVToRGB(0.45f, 0.3f, 0.5f),
		Color.HSVToRGB(0.6f, 0.3f, 0.5f),
		Color.HSVToRGB(0.75f, 0.3f, 0.5f),
		Color.HSVToRGB(0.9f, 0.3f, 0.5f)
	};

	private List<WorldContainer> m_worldContainers = new List<WorldContainer>();

	[MyCmpGet]
	private ClusterPOIManager m_clusterPOIsManager;

	private Dictionary<int, List<IAssignableIdentity>> minionsByWorld = new Dictionary<int, List<IAssignableIdentity>>();

	private List<int> _worldIDs = new List<int>();

	public int worldCount => m_worldContainers.Count;

	public int activeWorldId => activeWorldIdx;

	public IList<WorldContainer> WorldContainers => m_worldContainers.AsReadOnly();

	public Dictionary<int, List<IAssignableIdentity>> MinionsByWorld
	{
		get
		{
			minionsByWorld.Clear();
			for (int i = 0; i < Components.MinionAssignablesProxy.Count; i++)
			{
				if (!Components.MinionAssignablesProxy[i].GetTargetGameObject().HasTag(GameTags.Dead))
				{
					int id = Components.MinionAssignablesProxy[i].GetTargetGameObject().GetComponent<KMonoBehaviour>().GetMyWorld()
						.id;
					if (!minionsByWorld.ContainsKey(id))
					{
						minionsByWorld.Add(id, new List<IAssignableIdentity>());
					}
					minionsByWorld[id].Add(Components.MinionAssignablesProxy[i]);
				}
			}
			return minionsByWorld;
		}
	}

	public WorldContainer activeWorld => GetWorld(activeWorldId);

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public ClusterPOIManager GetClusterPOIManager()
	{
		return m_clusterPOIsManager;
	}

	public void RegisterWorldContainer(WorldContainer worldContainer)
	{
		m_worldContainers.Add(worldContainer);
	}

	public void UnregisterWorldContainer(WorldContainer worldContainer)
	{
		Trigger(-1078710002, worldContainer.id);
		m_worldContainers.Remove(worldContainer);
	}

	public List<int> GetWorldIDsSorted()
	{
		m_worldContainers.Sort((WorldContainer a, WorldContainer b) => a.DiscoveryTimestamp.CompareTo(b.DiscoveryTimestamp));
		_worldIDs.Clear();
		foreach (WorldContainer worldContainer in m_worldContainers)
		{
			_worldIDs.Add(worldContainer.id);
		}
		return _worldIDs;
	}

	public List<int> GetDiscoveredAsteroidIDsSorted()
	{
		m_worldContainers.Sort((WorldContainer a, WorldContainer b) => a.DiscoveryTimestamp.CompareTo(b.DiscoveryTimestamp));
		List<int> list = new List<int>();
		for (int i = 0; i < m_worldContainers.Count; i++)
		{
			if (m_worldContainers[i].IsDiscovered && !m_worldContainers[i].IsModuleInterior)
			{
				list.Add(m_worldContainers[i].id);
			}
		}
		return list;
	}

	public WorldContainer GetStartWorld()
	{
		foreach (WorldContainer worldContainer in WorldContainers)
		{
			if (worldContainer.IsStartWorld)
			{
				return worldContainer;
			}
		}
		return WorldContainers[0];
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		SaveLoader instance = SaveLoader.Instance;
		instance.OnWorldGenComplete = (Action<Cluster>)Delegate.Combine(instance.OnWorldGenComplete, new Action<Cluster>(OnWorldGenComplete));
	}

	protected override void OnSpawn()
	{
		if (m_grid == null)
		{
			m_grid = new ClusterGrid(m_numRings);
		}
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void OnWorldGenComplete(Cluster clusterLayout)
	{
		m_numRings = clusterLayout.numRings;
		m_grid = new ClusterGrid(m_numRings);
		foreach (WorldGen world in clusterLayout.worlds)
		{
			int id = CreateAsteroidWorldContainer(world).id;
			Vector2I position = world.GetPosition();
			Vector2I vector2I = position + world.GetSize();
			for (int i = position.y; i < vector2I.y; i++)
			{
				for (int j = position.x; j < vector2I.x; j++)
				{
					int num = Grid.XYToCell(j, i);
					Grid.WorldIdx[num] = (byte)id;
					Pathfinding.Instance.AddDirtyNavGridCell(num);
				}
			}
			if (world.isStartingWorld)
			{
				activeWorldIdx = id;
			}
		}
		this.GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(AxialI.ZERO, 1);
		m_clusterPOIsManager.PopulatePOIsFromWorldGen(clusterLayout);
	}

	private int GetNextWorldId()
	{
		HashSetPool<int, ClusterManager>.PooledHashSet pooledHashSet = HashSetPool<int, ClusterManager>.Allocate();
		foreach (WorldContainer worldContainer in m_worldContainers)
		{
			pooledHashSet.Add(worldContainer.id);
		}
		Debug.Assert(m_worldContainers.Count < 255, "Oh no! We're trying to generate our 255th world in this save, things are going to start going badly...");
		for (int i = 0; i < 255; i++)
		{
			if (!pooledHashSet.Contains(i))
			{
				pooledHashSet.Recycle();
				return i;
			}
		}
		pooledHashSet.Recycle();
		return INVALID_WORLD_IDX;
	}

	private WorldContainer CreateAsteroidWorldContainer(WorldGen world)
	{
		int nextWorldId = GetNextWorldId();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("Asteroid"));
		WorldContainer component = gameObject.GetComponent<WorldContainer>();
		component.SetID(nextWorldId);
		component.SetWorldDetails(world);
		AsteroidGridEntity component2 = gameObject.GetComponent<AsteroidGridEntity>();
		if (world != null)
		{
			AxialI clusterLocation = world.GetClusterLocation();
			Debug.Assert(clusterLocation != AxialI.ZERO || world.isStartingWorld, "Only starting world should be at zero");
			component2.Init(component.GetRandomName(), clusterLocation, world.Settings.world.asteroidIcon);
		}
		else
		{
			component2.Init("", AxialI.ZERO, "");
		}
		if (component.IsStartWorld)
		{
			OrbitalMechanics component3 = gameObject.GetComponent<OrbitalMechanics>();
			if (component3 != null)
			{
				component3.CreateOrbitalObject(Db.Get().OrbitalTypeCategories.backgroundEarth.Id);
			}
		}
		gameObject.SetActive(value: true);
		return component;
	}

	private void CreateDefaultAsteroidWorldContainer()
	{
		if (m_worldContainers.Count != 0)
		{
			return;
		}
		Debug.LogWarning("Cluster manager has no world containers, create a default using Grid settings.");
		WorldContainer worldContainer = CreateAsteroidWorldContainer(null);
		int id = worldContainer.id;
		for (int i = (int)worldContainer.minimumBounds.y; (float)i <= worldContainer.maximumBounds.y; i++)
		{
			for (int j = (int)worldContainer.minimumBounds.x; (float)j <= worldContainer.maximumBounds.x; j++)
			{
				int num = Grid.XYToCell(j, i);
				Grid.WorldIdx[num] = (byte)id;
				Pathfinding.Instance.AddDirtyNavGridCell(num);
			}
		}
	}

	public void InitializeWorldGrid()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 20))
		{
			CreateDefaultAsteroidWorldContainer();
		}
		bool flag = false;
		foreach (WorldContainer worldContainer in m_worldContainers)
		{
			Vector2I worldOffset = worldContainer.WorldOffset;
			Vector2I vector2I = worldOffset + worldContainer.WorldSize;
			for (int i = worldOffset.y; i < vector2I.y; i++)
			{
				for (int j = worldOffset.x; j < vector2I.x; j++)
				{
					int num = Grid.XYToCell(j, i);
					Grid.WorldIdx[num] = (byte)worldContainer.id;
					Pathfinding.Instance.AddDirtyNavGridCell(num);
				}
			}
			flag |= worldContainer.IsDiscovered;
		}
		if (!flag)
		{
			Debug.LogWarning("No worlds have been discovered. Setting the active world to discovered");
			activeWorld.SetDiscovered();
		}
	}

	public void SetActiveWorld(int worldIdx)
	{
		int num = activeWorldIdx;
		if (num != worldIdx)
		{
			activeWorldIdx = worldIdx;
			Game.Instance.Trigger(1983128072, new Tuple<int, int>(activeWorldIdx, num));
		}
	}

	public void TimelapseModeOverrideActiveWorld(int overrideValue)
	{
		activeWorldIdx = overrideValue;
	}

	public WorldContainer GetWorld(int id)
	{
		for (int i = 0; i < m_worldContainers.Count; i++)
		{
			if (m_worldContainers[i].id == id)
			{
				return m_worldContainers[i];
			}
		}
		return null;
	}

	public WorldContainer GetWorldFromPosition(Vector3 position)
	{
		foreach (WorldContainer worldContainer in m_worldContainers)
		{
			if (worldContainer.ContainsPoint(position))
			{
				return worldContainer;
			}
		}
		return null;
	}

	public float CountAllRations()
	{
		float result = 0f;
		foreach (WorldContainer worldContainer in m_worldContainers)
		{
			RationTracker.Get().CountRations(null, worldContainer.worldInventory);
		}
		return result;
	}

	public Dictionary<Tag, float> GetAllWorldsAccessibleAmounts()
	{
		Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
		foreach (WorldContainer worldContainer in m_worldContainers)
		{
			foreach (KeyValuePair<Tag, float> accessibleAmount in worldContainer.worldInventory.GetAccessibleAmounts())
			{
				if (dictionary.ContainsKey(accessibleAmount.Key))
				{
					dictionary[accessibleAmount.Key] += accessibleAmount.Value;
				}
				else
				{
					dictionary.Add(accessibleAmount.Key, accessibleAmount.Value);
				}
			}
		}
		return dictionary;
	}

	public void MigrateMinion(MinionIdentity minion, int targetID)
	{
		if (!Instance.GetWorld(targetID).IsDiscovered)
		{
			Instance.GetWorld(targetID).SetDiscovered();
		}
		if (!Instance.GetWorld(targetID).IsDupeVisited)
		{
			Instance.GetWorld(targetID).SetDupeVisited();
		}
		Game.Instance.assignmentManager.RemoveFromWorld(minion, minion.GetMyWorldId());
		Game.Instance.Trigger(586301400, minion);
	}

	public int GetLandingBeaconLocation(int worldId)
	{
		foreach (LandingBeacon.Instance landingBeacon in Components.LandingBeacons)
		{
			if (landingBeacon.GetMyWorldId() == worldId && landingBeacon.CanBeTargeted())
			{
				return Grid.PosToCell(landingBeacon);
			}
		}
		return Grid.InvalidCell;
	}

	public int GetRandomClearCell(int worldId)
	{
		bool flag = false;
		int num = 0;
		while (!flag && num < 1000)
		{
			num++;
			int num2 = UnityEngine.Random.Range(0, Grid.CellCount);
			if (!Grid.Solid[num2] && !Grid.IsLiquid(num2) && Grid.WorldIdx[num2] == worldId)
			{
				return num2;
			}
		}
		num = 0;
		while (!flag && num < 1000)
		{
			num++;
			int num3 = UnityEngine.Random.Range(0, Grid.CellCount);
			if (!Grid.Solid[num3] && Grid.WorldIdx[num3] == worldId)
			{
				return num3;
			}
		}
		return Grid.InvalidCell;
	}

	private bool NotObstructedCell(int x, int y)
	{
		int cell = Grid.XYToCell(x, y);
		if (Grid.IsValidCell(cell))
		{
			return Grid.Objects[cell, 1] == null;
		}
		return false;
	}

	private int LowestYThatSeesSky(int topCellYPos, int x)
	{
		int num = topCellYPos;
		while (!ValidSurfaceCell(x, num))
		{
			num--;
		}
		return num;
	}

	private bool ValidSurfaceCell(int x, int y)
	{
		int i = Grid.XYToCell(x, y - 1);
		if (!Grid.Solid[i])
		{
			return Grid.Foundation[i];
		}
		return true;
	}

	public int GetRandomSurfaceCell(int worldID, int width = 1, bool excludeTopBorderHeight = true)
	{
		WorldContainer worldContainer = m_worldContainers.Find((WorldContainer match) => match.id == worldID);
		int num = Mathf.RoundToInt(UnityEngine.Random.Range(worldContainer.minimumBounds.x + (float)(worldContainer.Width / 10), worldContainer.maximumBounds.x - (float)(worldContainer.Width / 10)));
		int num2 = Mathf.RoundToInt(worldContainer.maximumBounds.y);
		if (excludeTopBorderHeight)
		{
			num2 -= Grid.TopBorderHeight;
		}
		int num3 = num;
		int num4 = LowestYThatSeesSky(num2, num3);
		int num5 = 0;
		num5 = (NotObstructedCell(num3, num4) ? 1 : 0);
		while (num3 + 1 != num && num5 < width)
		{
			num3++;
			if ((float)num3 > worldContainer.maximumBounds.x)
			{
				num5 = 0;
				num3 = (int)worldContainer.minimumBounds.x;
			}
			int num6 = LowestYThatSeesSky(num2, num3);
			bool flag = NotObstructedCell(num3, num6);
			num5 = ((!(num6 == num4 && flag)) ? (flag ? 1 : 0) : (num5 + 1));
			num4 = num6;
		}
		if (num5 < width)
		{
			return -1;
		}
		return Grid.XYToCell(num3, num4);
	}

	public bool IsPositionInActiveWorld(Vector3 pos)
	{
		if (activeWorld != null && !CameraController.Instance.ignoreClusterFX)
		{
			Vector2 vector = activeWorld.maximumBounds * Grid.CellSizeInMeters;
			Vector2 vector2 = activeWorld.minimumBounds * Grid.CellSizeInMeters;
			if (pos.x < vector2.x || pos.x > vector.x || pos.y < vector2.y || pos.y > vector.y)
			{
				return false;
			}
		}
		return true;
	}

	public WorldContainer CreateRocketInteriorWorld(GameObject craft_go, string interiorTemplateName, System.Action callback)
	{
		Vector2I rOCKET_INTERIOR_SIZE = ROCKETRY.ROCKET_INTERIOR_SIZE;
		if (Grid.GetFreeGridSpace(rOCKET_INTERIOR_SIZE, out var offset))
		{
			int nextWorldId = GetNextWorldId();
			craft_go.AddComponent<WorldInventory>();
			WorldContainer worldContainer = craft_go.AddComponent<WorldContainer>();
			worldContainer.SetRocketInteriorWorldDetails(nextWorldId, rOCKET_INTERIOR_SIZE, offset);
			Vector2I vector2I = offset + rOCKET_INTERIOR_SIZE;
			for (int i = offset.y; i < vector2I.y; i++)
			{
				for (int j = offset.x; j < vector2I.x; j++)
				{
					int num = Grid.XYToCell(j, i);
					Grid.WorldIdx[num] = (byte)nextWorldId;
					Pathfinding.Instance.AddDirtyNavGridCell(num);
				}
			}
			Debug.Log($"Created new rocket interior id: {nextWorldId}, at {offset} with size {rOCKET_INTERIOR_SIZE}");
			worldContainer.PlaceInteriorTemplate(interiorTemplateName, delegate
			{
				if (callback != null)
				{
					callback();
				}
				craft_go.GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.RocketInteriorComplete, null);
			});
			craft_go.AddComponent<OrbitalMechanics>().CreateOrbitalObject(Db.Get().OrbitalTypeCategories.landed.Id);
			Trigger(-1280433810, worldContainer.id);
			return worldContainer;
		}
		Debug.LogError("Failed to create rocket interior.");
		return null;
	}

	public void DestoryRocketInteriorWorld(int world_id, ClustercraftExteriorDoor door)
	{
		WorldContainer world = GetWorld(world_id);
		if (world == null || !world.IsModuleInterior)
		{
			Debug.LogError($"Attempting to destroy world id {world_id}. The world is not a valid rocket interior");
			return;
		}
		GameObject craft_go = door.GetComponent<RocketModuleCluster>().CraftInterface.gameObject;
		if (activeWorldId == world_id)
		{
			if (craft_go.GetComponent<WorldContainer>().ParentWorldId == world_id)
			{
				SetActiveWorld(Instance.GetStartWorld().id);
			}
			else
			{
				SetActiveWorld(craft_go.GetComponent<WorldContainer>().ParentWorldId);
			}
		}
		Vector3 spawn_pos = door.transform.position;
		world.EjectAllDupes(spawn_pos);
		world.CancelChores();
		world.DestroyWorldBuildings(spawn_pos, out var noRefundTiles);
		GameScheduler.Instance.ScheduleNextFrame("ClusterManager.DeleteWorldObjects", delegate
		{
			DeleteWorldObjects(world, craft_go, spawn_pos, noRefundTiles);
		});
	}

	private void DeleteWorldObjects(WorldContainer world, GameObject craft_go, Vector3 spawn_pos, HashSet<int> noRefundTiles)
	{
		world.TransferResourcesToParentWorld(spawn_pos, noRefundTiles);
		Grid.FreeGridSpace(world.WorldSize, world.WorldOffset);
		WorldInventory worldInventory = null;
		if (world != null)
		{
			worldInventory = world.GetComponent<WorldInventory>();
		}
		if (worldInventory != null)
		{
			UnityEngine.Object.Destroy(worldInventory);
		}
		if (world != null)
		{
			UnityEngine.Object.Destroy(world);
		}
	}
}
