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
	private bool isDiscovered;

	[Serialize]
	private bool isDupeVisited;

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
	public WorldInventory worldInventory
	{
		get;
		private set;
	}

	public Dictionary<Tag, float> materialNeeds
	{
		get;
		private set;
	}

	public bool IsModuleInterior => isModuleInterior;

	public bool IsDiscovered => isDiscovered || DebugHandler.RevealFogOfWar;

	public bool IsDupeVisited => isDupeVisited;

	public List<string> Biomes => m_subworldNames;

	public AlertStateManager.Instance AlertManager => m_alertManager;

	public int ParentWorldId
	{
		get;
		private set;
	}

	public Vector2 minimumBounds => new Vector2(worldOffset.x, worldOffset.y);

	public Vector2 maximumBounds => new Vector2(worldOffset.x - 1 + worldSize.x, worldOffset.y - 1 + worldSize.y);

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
		InfoDescription infoDescription = base.gameObject.AddOrGet<InfoDescription>();
		infoDescription.DescriptionLocString = worldDescription;
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
		isDiscovered = true;
		if (reveal_surface)
		{
			RevealSurface();
			SetSurfaceCameraPos();
		}
		Game.Instance.Trigger(-521212405, this);
	}

	public void SetDupeVisited()
	{
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
		return (vector.sqrMagnitude < vector2.sqrMagnitude) ? 1 : (-1);
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
		WorldDetailSave.OverworldCell overworldCell = new WorldDetailSave.OverworldCell();
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
		clusterDetailSave.overworldCells.Add(overworldCell);
		Rect rect = new Rect(pos.x - num + 1f, pos.y - num2 + 1f, template.info.size.X, template.info.size.Y);
		for (int i = (int)rect.yMin; (float)i < rect.yMax; i++)
		{
			for (int j = (int)rect.xMin; (float)j < rect.xMax; j++)
			{
				int cell = Grid.XYToCell(j, i);
				SimMessages.ModifyCellWorldZone(cell, 0);
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
			nameTable = world.Settings.world.nameTable;
			worldDescription = world.Settings.world.description;
			worldType = world.Settings.world.name;
			isModuleInterior = world.Settings.world.moduleInterior;
			m_seasonIds = new List<string>(world.Settings.world.seasons);
			m_subworldNames = new List<string>();
			foreach (WeightedSubworldName subworldFile in world.Settings.world.subworldFiles)
			{
				string name = subworldFile.name;
				name = name.Substring(0, name.LastIndexOf('/'));
				name = name.Substring(name.LastIndexOf('/') + 1, name.Length - (name.LastIndexOf('/') + 1));
				m_subworldNames.Add(name);
			}
		}
		else
		{
			fullyEnclosedBorder = false;
			worldOffset = Vector2I.zero;
			worldSize = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
			isDiscovered = true;
			m_seasonIds = new List<string>
			{
				Db.Get().GameplaySeasons.MeteorShowers.Id
			};
		}
	}

	public bool ContainsPoint(Vector2 point)
	{
		return point.x >= (float)worldOffset.x && point.y >= (float)worldOffset.y && point.x < (float)(worldOffset.x + worldSize.x) && point.y < (float)(worldOffset.y + worldSize.y);
	}

	private void RevealSurface()
	{
		for (int i = 0; i < worldSize.x; i++)
		{
			for (int num = worldSize.y; num >= 0; num--)
			{
				int num2 = Grid.XYToCell(i + worldOffset.x, num + worldOffset.y);
				if (Grid.IsValidCell(num2) && Grid.ExposedToSunlight[num2] >= 253)
				{
					GridVisibility.Reveal(i + worldOffset.X, num + worldOffset.y, 7, 1f);
				}
			}
		}
	}

	private void SetSurfaceCameraPos()
	{
		if (!(SaveGame.Instance != null))
		{
			return;
		}
		int num = (int)maximumBounds.y;
		for (int i = 0; i < worldSize.X; i++)
		{
			for (int num2 = worldSize.Y; num2 >= 0; num2--)
			{
				int num3 = num2 + worldOffset.y;
				int num4 = Grid.XYToCell(i + worldOffset.x, num3);
				if (Grid.IsValidCell(num4) && Grid.Solid[num4])
				{
					num = Math.Min(num, num3);
					break;
				}
			}
		}
		int num5 = (num + worldOffset.y + worldSize.y) / 2;
		Vector3 start_pos = new Vector3(WorldOffset.x + Width / 2, num5, 0f);
		SaveGame.Instance.GetComponent<UserNavigation>().SetWorldCameraStartPosition(id, start_pos);
	}

	public void EjectAllDupes(Vector3 spawn_pos)
	{
		List<MinionIdentity> worldItems = Components.MinionIdentities.GetWorldItems(id);
		foreach (MinionIdentity item in worldItems)
		{
			item.transform.SetLocalPosition(spawn_pos);
		}
	}

	public void DestroyWorldBuildings(Vector3 spawn_pos)
	{
		TransferBuildingMaterials(spawn_pos);
		List<ClustercraftInteriorDoor> worldItems = Components.ClusterCraftInteriorDoors.GetWorldItems(id);
		foreach (ClustercraftInteriorDoor item in worldItems)
		{
			item.DeleteObject();
		}
	}

	public void TransferResourcesToParentWorld(Vector3 spawn_pos)
	{
		TransferPickupables(spawn_pos);
		TransferLiquidsSolidsAndGases(spawn_pos);
	}

	private void TransferBuildingMaterials(Vector3 pos)
	{
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
			if (component != null && component.allowDeconstruction)
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
						GameObject gameObject = GameUtil.KInstantiate(prefab, pos + Vector3.up * 0.5f, Grid.SceneLayer.Ore);
						gameObject.SetActive(value: true);
					}
				}
			}
			Storage component3 = buildingComplete.GetComponent<Storage>();
			if (component3 != null)
			{
				component3.DropAll(pos);
			}
			PlantablePlot component4 = buildingComplete.GetComponent<PlantablePlot>();
			if (component4 != null)
			{
				SeedProducer seedProducer = ((component4.Occupant != null) ? component4.Occupant.GetComponent<SeedProducer>() : null);
				if (seedProducer != null)
				{
					seedProducer.DropSeed();
				}
			}
			buildingComplete.DeleteObject();
		}
		pooledList.Clear();
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

	private void TransferLiquidsSolidsAndGases(Vector3 pos)
	{
		for (int i = (int)minimumBounds.x; (float)i < maximumBounds.x; i++)
		{
			for (int j = (int)minimumBounds.y; (float)j < maximumBounds.y; j++)
			{
				int num = Grid.XYToCell(i, j);
				Element element = Grid.Element[num];
				if (element != null && !element.IsVacuum)
				{
					element.substance.SpawnResource(pos, Grid.Mass[num], Grid.Temperature[num], Grid.DiseaseIdx[num], Grid.DiseaseCount[num]);
				}
			}
		}
	}

	public void CancelChores()
	{
		for (int i = 0; i < 40; i++)
		{
			for (int j = (int)minimumBounds.x; (float)j < maximumBounds.x; j++)
			{
				for (int k = (int)minimumBounds.y; (float)k < maximumBounds.y; k++)
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
}
