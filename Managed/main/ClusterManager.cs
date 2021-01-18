using System;
using System.Collections;
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
	private int nextWorldId = 0;

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

	[Serialize]
	private ClusterPOIManager m_clusterPOIsManager = new ClusterPOIManager();

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

	public List<int> GetWorldIDs()
	{
		_worldIDs.Clear();
		foreach (WorldContainer worldContainer in m_worldContainers)
		{
			_worldIDs.Add(worldContainer.id);
		}
		return _worldIDs;
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
			WorldContainer worldContainer = CreateAsteroidWorldContainer(world);
			int id = worldContainer.id;
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
		return nextWorldId++;
	}

	private WorldContainer CreateAsteroidWorldContainer(WorldGen world)
	{
		int iD = GetNextWorldId();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("Asteroid"));
		WorldContainer component = gameObject.GetComponent<WorldContainer>();
		component.SetID(iD);
		component.SetWorldDetails(world);
		AsteroidGridEntity component2 = gameObject.GetComponent<AsteroidGridEntity>();
		if (world != null)
		{
			AxialI clusterLocation = world.GetClusterLocation();
			Debug.Assert(clusterLocation != AxialI.ZERO || world.isStartingWorld, "Only starting world should be at zero");
			component2.Init(component.GetRandomName(), clusterLocation, world.Settings.world.asteroidType);
		}
		else
		{
			component2.Init("", AxialI.ZERO, "");
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
		for (int i = (int)worldContainer.minimumBounds.y; (float)i < worldContainer.maximumBounds.y; i++)
		{
			for (int j = (int)worldContainer.minimumBounds.x; (float)j < worldContainer.maximumBounds.x; j++)
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
		foreach (WorldContainer worldContainer2 in m_worldContainers)
		{
			WorldContainer worldContainer = worldContainer2;
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
		foreach (LandingBeacon landingBeacon in Components.LandingBeacons)
		{
			if (landingBeacon.GetMyWorldId() == worldId && !landingBeacon.isInUse)
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
		return Grid.IsValidCell(cell) && Grid.Objects[cell, 1] == null;
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
		return Grid.Solid[i] || Grid.Foundation[i];
	}

	public int GetRandomSurfaceCell(int worldID, int width = 1)
	{
		WorldContainer worldContainer = m_worldContainers.Find((WorldContainer match) => match.id == worldID);
		int num = Mathf.RoundToInt(UnityEngine.Random.Range(worldContainer.minimumBounds.x + (float)(worldContainer.Width / 10), worldContainer.maximumBounds.x - (float)(worldContainer.Width / 10)));
		int topCellYPos = Mathf.RoundToInt(worldContainer.maximumBounds.y - 1f);
		int num2 = num;
		int num3 = LowestYThatSeesSky(topCellYPos, num2);
		int num4 = 0;
		num4 = (NotObstructedCell(num2, num3) ? 1 : 0);
		while (num2 + 1 != num && num4 < width)
		{
			num2++;
			if ((float)num2 >= worldContainer.maximumBounds.x)
			{
				num4 = 0;
				num2 = (int)worldContainer.minimumBounds.x;
			}
			int num5 = LowestYThatSeesSky(topCellYPos, num2);
			bool flag = NotObstructedCell(num2, num5);
			num4 = ((!(num5 == num3 && flag)) ? (flag ? 1 : 0) : (num4 + 1));
			num3 = num5;
		}
		if (num4 < width)
		{
			return -1;
		}
		return Grid.XYToCell(num2, num3);
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
			int num = GetNextWorldId();
			craft_go.AddComponent<WorldInventory>();
			WorldContainer worldContainer = craft_go.AddComponent<WorldContainer>();
			worldContainer.SetRocketInteriorWorldDetails(num, rOCKET_INTERIOR_SIZE, offset);
			Vector2I vector2I = offset + rOCKET_INTERIOR_SIZE;
			for (int i = offset.y; i < vector2I.y; i++)
			{
				for (int j = offset.x; j < vector2I.x; j++)
				{
					int num2 = Grid.XYToCell(j, i);
					Grid.WorldIdx[num2] = (byte)num;
					Pathfinding.Instance.AddDirtyNavGridCell(num2);
				}
			}
			worldContainer.PlaceInteriorTemplate(interiorTemplateName, delegate
			{
				if (callback != null)
				{
					callback();
				}
				craft_go.GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.RocketInteriorComplete, null);
			});
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
		GameObject gameObject = door.GetComponent<RocketModule>().CraftInterface.gameObject;
		if (activeWorldId == world_id)
		{
			int parentWorldId = gameObject.GetComponent<WorldContainer>().ParentWorldId;
			if (parentWorldId != INVALID_WORLD_IDX)
			{
				Debug.LogWarning($"Destroying rocket interior while it is the active world id {world_id}. Setting parent world {parentWorldId}");
				SetActiveWorld(gameObject.GetComponent<WorldContainer>().ParentWorldId);
			}
			else
			{
				Debug.LogError($"Destroying a rocket world {world_id} with no parent world.");
			}
		}
		Vector3 position = door.transform.position;
		world.EjectAllDupes(position);
		world.CancelChores();
		world.DestroyWorldBuildings(position);
		StartCoroutine(DeleteWorldObjects(world, gameObject, position));
	}

	private IEnumerator DeleteWorldObjects(WorldContainer world, GameObject craft_go, Vector3 spawn_pos)
	{
		yield return new WaitForEndOfFrame();
		world.TransferResourcesToParentWorld(spawn_pos);
		Grid.FreeGridSpace(world.WorldSize, world.WorldOffset);
		WorldInventory inventory = null;
		if (world != null)
		{
			inventory = world.GetComponent<WorldInventory>();
		}
		if (inventory != null)
		{
			UnityEngine.Object.Destroy(inventory);
		}
		if (world != null)
		{
			UnityEngine.Object.Destroy(world);
		}
	}
}
