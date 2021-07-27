using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using Klei;
using KSerialization;
using ProcGen;
using ProcGenGame;
using TemplateClasses;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class WorldContainer : KMonoBehaviour
{
	[Serialize]
	public int id = -1;

	[Serialize]
	public Tag prefabTag;

	[Serialize]
	private Vector2I worldOffset;

	[Serialize]
	private Vector2I worldSize;

	[Serialize]
	private bool fullyEnclosedBorder;

	[Serialize]
	private bool isModuleInterior;

	[Serialize]
	private WorldDetailSave.OverworldCell overworldCell;

	[Serialize]
	private bool isDiscovered;

	[Serialize]
	private bool isStartWorld;

	[Serialize]
	private bool isDupeVisited;

	[Serialize]
	private float dupeVisitedTimestamp;

	[Serialize]
	private float discoveryTimestamp;

	[Serialize]
	public string worldName;

	[Serialize]
	public string nameTable;

	[Serialize]
	public string worldType;

	[Serialize]
	public string worldDescription;

	[Serialize]
	private List<string> m_seasonIds;

	[Serialize]
	private List<string> m_subworldNames;

	[MySmiReq]
	private AlertStateManager.Instance m_alertManager;

	private List<Prioritizable> yellowAlertTasks = new List<Prioritizable>();

	[Serialize]
	public WorldInventory worldInventory { get; private set; }

	public Dictionary<Tag, float> materialNeeds { get; private set; }

	public bool IsModuleInterior => isModuleInterior;

	public bool IsDiscovered
	{
		get
		{
			if (!isDiscovered)
			{
				return DebugHandler.RevealFogOfWar;
			}
			return true;
		}
	}

	public bool IsStartWorld => isStartWorld;

	public bool IsDupeVisited => isDupeVisited;

	public float DupeVisitedTimestamp => dupeVisitedTimestamp;

	public float DiscoveryTimestamp => discoveryTimestamp;

	public List<string> Biomes => m_subworldNames;

	public AlertStateManager.Instance AlertManager
	{
		get
		{
			if (m_alertManager == null)
			{
				StateMachineController component = GetComponent<StateMachineController>();
				m_alertManager = component.GetSMI<AlertStateManager.Instance>();
			}
			Debug.Assert(m_alertManager != null, "AlertStateManager should never be null.");
			return m_alertManager;
		}
	}

	public int ParentWorldId { get; private set; }

	public Vector2 minimumBounds => new Vector2(worldOffset.x, worldOffset.y);

	public Vector2 maximumBounds => new Vector2(worldOffset.x + (worldSize.x - 1), worldOffset.y + (worldSize.y - 1));

	public Vector2I WorldSize => worldSize;

	public Vector2I WorldOffset => worldOffset;

	public bool FullyEnclosedBorder => fullyEnclosedBorder;

	public int Height => worldSize.y;

	public int Width => worldSize.x;

	public void AddTopPriorityPrioritizable(Prioritizable prioritizable)
	{
		if (!yellowAlertTasks.Contains(prioritizable))
		{
			yellowAlertTasks.Add(prioritizable);
		}
		RefreshHasTopPriorityChore();
	}

	public void RemoveTopPriorityPrioritizable(Prioritizable prioritizable)
	{
		for (int num = yellowAlertTasks.Count - 1; num >= 0; num--)
		{
			if (yellowAlertTasks[num] == prioritizable || yellowAlertTasks[num].Equals(null))
			{
				yellowAlertTasks.RemoveAt(num);
			}
		}
		RefreshHasTopPriorityChore();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		ParentWorldId = id;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		worldInventory = GetComponent<WorldInventory>();
		materialNeeds = new Dictionary<Tag, float>();
		ClusterManager.Instance.RegisterWorldContainer(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.gameObject.AddOrGet<InfoDescription>().DescriptionLocString = worldDescription;
		RefreshHasTopPriorityChore();
	}

	protected override void OnCleanUp()
	{
		ClusterManager.Instance.UnregisterWorldContainer(this);
		base.OnCleanUp();
	}

	private void RefreshHasTopPriorityChore()
	{
		if (AlertManager != null)
		{
			AlertManager.SetHasTopPriorityChore(yellowAlertTasks.Count > 0);
		}
	}

	public List<string> GetSeasonIds()
	{
		return m_seasonIds;
	}

	public bool IsRedAlert()
	{
		return m_alertManager.IsRedAlert();
	}

	public bool IsYellowAlert()
	{
		return m_alertManager.IsYellowAlert();
	}

	public string GetRandomName()
	{
		return GameUtil.GenerateRandomWorldName(nameTable);
	}

	public void SetID(int id)
	{
		this.id = id;
		ParentWorldId = id;
	}

	public void SetParentIdx(int parentIdx)
	{
		ParentWorldId = parentIdx;
		Game.Instance.Trigger(880851192, this);
	}

	public void SetDiscovered(bool reveal_surface = false)
	{
		if (!isDiscovered)
		{
			discoveryTimestamp = GameUtil.GetCurrentTimeInCycles();
		}
		isDiscovered = true;
		if (reveal_surface)
		{
			LookAtSurface();
		}
		Game.Instance.Trigger(-521212405, this);
	}

	public void SetDupeVisited()
	{
		if (!isDupeVisited)
		{
			dupeVisitedTimestamp = GameUtil.GetCurrentTimeInCycles();
		}
		isDupeVisited = true;
		Game.Instance.Trigger(-434755240, this);
	}

	public void SetRocketInteriorWorldDetails(int world_id, Vector2I size, Vector2I offset)
	{
		SetID(world_id);
		fullyEnclosedBorder = true;
		worldOffset = offset;
		worldSize = size;
		isDiscovered = true;
		isModuleInterior = true;
		m_seasonIds = new List<string>();
	}

	private static int IsClockwise(Vector2 first, Vector2 second, Vector2 origin)
	{
		if (first == second)
		{
			return 0;
		}
		Vector2 vector = first - origin;
		Vector2 vector2 = second - origin;
		float num = Mathf.Atan2(vector.x, vector.y);
		float num2 = Mathf.Atan2(vector2.x, vector2.y);
		if (num < num2)
		{
			return 1;
		}
		if (num > num2)
		{
			return -1;
		}
		if (!(vector.sqrMagnitude < vector2.sqrMagnitude))
		{
			return -1;
		}
		return 1;
	}

	public void PlaceInteriorTemplate(string template_name, System.Action callback)
	{
		TemplateContainer template = TemplateCache.GetTemplate(template_name);
		Vector2 pos = new Vector2(worldSize.x / 2 + worldOffset.x, worldSize.y / 2 + worldOffset.y);
		TemplateLoader.Stamp(template, pos, callback);
		float num = template.info.size.X / 2f;
		float num2 = template.info.size.Y / 2f;
		float num3 = Math.Max(num, num2);
		GridVisibility.Reveal((int)pos.x, (int)pos.y, (int)num3 + 3 + 5, num3 + 3f);
		WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
		overworldCell = new WorldDetailSave.OverworldCell();
		List<Vector2> list = new List<Vector2>(template.cells.Count);
		foreach (Prefab building in template.buildings)
		{
			if (building.id == "RocketWallTile")
			{
				Vector2 item = new Vector2((float)building.location_x + pos.x, (float)building.location_y + pos.y);
				if (item.x > pos.x)
				{
					item.x += 0.5f;
				}
				if (item.y > pos.y)
				{
					item.y += 0.5f;
				}
				list.Add(item);
			}
		}
		list.Sort((Vector2 v1, Vector2 v2) => IsClockwise(v1, v2, pos));
		overworldCell.poly = new Polygon(list);
		overworldCell.zoneType = SubWorld.ZoneType.RocketInterior;
		overworldCell.tags = new TagSet { WorldGenTags.RocketInterior };
		clusterDetailSave.overworldCells.Add(overworldCell);
		Rect rect = new Rect(pos.x - num + 1f, pos.y - num2 + 1f, template.info.size.X, template.info.size.Y);
		for (int i = (int)rect.yMin; (float)i < rect.yMax; i++)
		{
			for (int j = (int)rect.xMin; (float)j < rect.xMax; j++)
			{
				SimMessages.ModifyCellWorldZone(Grid.XYToCell(j, i), 0);
			}
		}
	}

	public void SetWorldDetails(WorldGen world)
	{
		if (world != null)
		{
			fullyEnclosedBorder = world.Settings.GetBoolSetting("DrawWorldBorder") && world.Settings.GetBoolSetting("DrawWorldBorderOverVacuum");
			worldOffset = world.GetPosition();
			worldSize = world.GetSize();
			isDiscovered = world.isStartingWorld;
			isStartWorld = world.isStartingWorld;
			worldName = world.Settings.world.filePath;
			nameTable = world.Settings.world.nameTable;
			worldDescription = world.Settings.world.description;
			worldType = world.Settings.world.name;
			isModuleInterior = world.Settings.world.moduleInterior;
			m_seasonIds = new List<string>(world.Settings.world.seasons);
			m_subworldNames = new List<string>();
			foreach (WeightedSubworldName subworldFile in world.Settings.world.subworldFiles)
			{
				string text = subworldFile.name;
				text = text.Substring(0, text.LastIndexOf('/'));
				text = text.Substring(text.LastIndexOf('/') + 1, text.Length - (text.LastIndexOf('/') + 1));
				m_subworldNames.Add(text);
			}
		}
		else
		{
			fullyEnclosedBorder = false;
			worldOffset = Vector2I.zero;
			worldSize = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
			isDiscovered = true;
			m_seasonIds = new List<string> { Db.Get().GameplaySeasons.MeteorShowers.Id };
		}
	}

	public bool ContainsPoint(Vector2 point)
	{
		if (point.x >= (float)worldOffset.x && point.y >= (float)worldOffset.y && point.x < (float)(worldOffset.x + worldSize.x))
		{
			return point.y < (float)(worldOffset.y + worldSize.y);
		}
		return false;
	}

	public void LookAtSurface()
	{
		if (!IsDupeVisited)
		{
			RevealSurface();
		}
		Vector3? vector = SetSurfaceCameraPos();
		if (ClusterManager.Instance.activeWorldId == id && vector.HasValue)
		{
			CameraController.Instance.SnapTo(vector.Value);
		}
	}

	private void RevealSurface()
	{
		for (int i = 0; i < worldSize.x; i++)
		{
			for (int num = worldSize.y - 1; num >= 0; num--)
			{
				int cell = Grid.XYToCell(i + worldOffset.x, num + worldOffset.y);
				if (!Grid.IsValidCell(cell) || Grid.IsSolidCell(cell) || Grid.IsLiquid(cell))
				{
					break;
				}
				GridVisibility.Reveal(i + worldOffset.X, num + worldOffset.y, 7, 1f);
			}
		}
	}

	private Vector3? SetSurfaceCameraPos()
	{
		if (SaveGame.Instance != null)
		{
			int num = (int)maximumBounds.y;
			for (int i = 0; i < worldSize.X; i++)
			{
				for (int num2 = worldSize.y - 1; num2 >= 0; num2--)
				{
					int num3 = num2 + worldOffset.y;
					int num4 = Grid.XYToCell(i + worldOffset.x, num3);
					if (Grid.IsValidCell(num4) && (Grid.Solid[num4] || Grid.IsLiquid(num4)))
					{
						num = Math.Min(num, num3);
						break;
					}
				}
			}
			int num5 = (num + worldOffset.y + worldSize.y) / 2;
			Vector3 vector = new Vector3(WorldOffset.x + Width / 2, num5, 0f);
			SaveGame.Instance.GetComponent<UserNavigation>().SetWorldCameraStartPosition(id, vector);
			return vector;
		}
		return null;
	}

	public void EjectAllDupes(Vector3 spawn_pos)
	{
		foreach (MinionIdentity worldItem in Components.MinionIdentities.GetWorldItems(id))
		{
			worldItem.transform.SetLocalPosition(spawn_pos);
		}
	}

	public void DestroyWorldBuildings(Vector3 spawn_pos, out HashSet<int> noRefundTiles)
	{
		TransferBuildingMaterials(spawn_pos, out noRefundTiles);
		foreach (ClustercraftInteriorDoor worldItem in Components.ClusterCraftInteriorDoors.GetWorldItems(id))
		{
			worldItem.DeleteObject();
		}
		ClearWorldZones();
	}

	public void TransferResourcesToParentWorld(Vector3 spawn_pos, HashSet<int> noRefundTiles)
	{
		TransferPickupables(spawn_pos);
		TransferLiquidsSolidsAndGases(spawn_pos, noRefundTiles);
	}

	private void TransferBuildingMaterials(Vector3 pos, out HashSet<int> noRefundTiles)
	{
		HashSet<int> retTemplateFoundationCells = new HashSet<int>();
		ListPool<ScenePartitionerEntry, ClusterManager>.PooledList pooledList = ListPool<ScenePartitionerEntry, ClusterManager>.Allocate();
		GameScenePartitioner.Instance.GatherEntries((int)minimumBounds.x, (int)minimumBounds.y, Width, Height, GameScenePartitioner.Instance.completeBuildings, pooledList);
		foreach (ScenePartitionerEntry item in pooledList)
		{
			BuildingComplete buildingComplete = item.obj as BuildingComplete;
			if (!(buildingComplete != null))
			{
				continue;
			}
			Deconstructable component = buildingComplete.GetComponent<Deconstructable>();
			if (component != null && !buildingComplete.HasTag(GameTags.NoRocketRefund))
			{
				PrimaryElement component2 = buildingComplete.GetComponent<PrimaryElement>();
				float temperature = component2.Temperature;
				byte diseaseIdx = component2.DiseaseIdx;
				int diseaseCount = component2.DiseaseCount;
				for (int i = 0; i < component.constructionElements.Length && buildingComplete.Def.Mass.Length > i; i++)
				{
					Element element = ElementLoader.GetElement(component.constructionElements[i]);
					if (element != null)
					{
						element.substance.SpawnResource(pos + Vector3.up * 0.5f, buildingComplete.Def.Mass[i], temperature, diseaseIdx, diseaseCount);
						continue;
					}
					GameObject prefab = Assets.GetPrefab(component.constructionElements[i]);
					for (int j = 0; (float)j < buildingComplete.Def.Mass[i]; j++)
					{
						GameUtil.KInstantiate(prefab, pos + Vector3.up * 0.5f, Grid.SceneLayer.Ore).SetActive(value: true);
					}
				}
			}
			SimCellOccupier component3 = buildingComplete.GetComponent<SimCellOccupier>();
			if (component3 != null && component3.doReplaceElement)
			{
				buildingComplete.RunOnArea(delegate(int cell)
				{
					retTemplateFoundationCells.Add(cell);
				});
			}
			Storage component4 = buildingComplete.GetComponent<Storage>();
			if (component4 != null)
			{
				component4.DropAll(pos);
			}
			PlantablePlot component5 = buildingComplete.GetComponent<PlantablePlot>();
			if (component5 != null)
			{
				SeedProducer seedProducer = ((component5.Occupant != null) ? component5.Occupant.GetComponent<SeedProducer>() : null);
				if (seedProducer != null)
				{
					seedProducer.DropSeed();
				}
			}
			buildingComplete.DeleteObject();
		}
		pooledList.Clear();
		noRefundTiles = retTemplateFoundationCells;
	}

	private void TransferPickupables(Vector3 pos)
	{
		int cell = Grid.PosToCell(pos);
		ListPool<ScenePartitionerEntry, ClusterManager>.PooledList pooledList = ListPool<ScenePartitionerEntry, ClusterManager>.Allocate();
		GameScenePartitioner.Instance.GatherEntries((int)minimumBounds.x, (int)minimumBounds.y, Width, Height, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		foreach (ScenePartitionerEntry item in pooledList)
		{
			if (item.obj != null)
			{
				Pickupable pickupable = item.obj as Pickupable;
				if (pickupable != null)
				{
					pickupable.gameObject.transform.SetLocalPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
				}
			}
		}
		pooledList.Recycle();
	}

	private void TransferLiquidsSolidsAndGases(Vector3 pos, HashSet<int> noRefundTiles)
	{
		for (int i = (int)minimumBounds.x; (float)i <= maximumBounds.x; i++)
		{
			for (int j = (int)minimumBounds.y; (float)j <= maximumBounds.y; j++)
			{
				int num = Grid.XYToCell(i, j);
				if (!noRefundTiles.Contains(num))
				{
					Element element = Grid.Element[num];
					if (element != null && !element.IsVacuum)
					{
						element.substance.SpawnResource(pos, Grid.Mass[num], Grid.Temperature[num], Grid.DiseaseIdx[num], Grid.DiseaseCount[num]);
					}
				}
			}
		}
	}

	public void CancelChores()
	{
		for (int i = 0; i < 42; i++)
		{
			for (int j = (int)minimumBounds.x; (float)j <= maximumBounds.x; j++)
			{
				for (int k = (int)minimumBounds.y; (float)k <= maximumBounds.y; k++)
				{
					int cell = Grid.XYToCell(j, k);
					GameObject gameObject = Grid.Objects[cell, i];
					if (gameObject != null)
					{
						gameObject.Trigger(2127324410, true);
					}
				}
			}
		}
		List<Chore> list = new List<Chore>();
		foreach (Chore chore in GlobalChoreProvider.Instance.chores)
		{
			if (chore != null && chore.target != null && !chore.isNull && chore.gameObject.GetMyWorldId() == id)
			{
				list.Add(chore);
			}
		}
		foreach (Chore item in list)
		{
			item.Cancel("World destroyed");
		}
	}

	public void ClearWorldZones()
	{
		if (this.overworldCell != null)
		{
			WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
			int num = -1;
			for (int i = 0; i < SaveLoader.Instance.clusterDetailSave.overworldCells.Count; i++)
			{
				WorldDetailSave.OverworldCell overworldCell = SaveLoader.Instance.clusterDetailSave.overworldCells[i];
				if (overworldCell.zoneType == this.overworldCell.zoneType && overworldCell.tags != null && this.overworldCell.tags != null && overworldCell.tags.ContainsAll(this.overworldCell.tags) && overworldCell.poly.bounds == this.overworldCell.poly.bounds)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				clusterDetailSave.overworldCells.RemoveAt(num);
			}
		}
		for (int j = (int)minimumBounds.y; (float)j <= maximumBounds.y; j++)
		{
			for (int k = (int)minimumBounds.x; (float)k <= maximumBounds.x; k++)
			{
				SimMessages.ModifyCellWorldZone(Grid.XYToCell(k, j), byte.MaxValue);
			}
		}
	}

	public int GetSafeCell()
	{
		if (IsModuleInterior)
		{
			foreach (RocketControlStation item in Components.RocketControlStations.Items)
			{
				if (item.GetMyWorldId() == id)
				{
					return Grid.PosToCell(item);
				}
			}
		}
		else
		{
			foreach (Telepad item2 in Components.Telepads.Items)
			{
				if (item2.GetMyWorldId() == id)
				{
					return Grid.PosToCell(item2);
				}
			}
		}
		return Grid.XYToCell(worldOffset.x + worldSize.x / 2, worldOffset.y + worldSize.y / 2);
	}

	public string GetStatus()
	{
		return ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResultStatus(id);
	}
}
