using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay.Geo;
using Klei;
using KSerialization;
using ProcGen;
using ProcGenGame;
using TemplateClasses;
using TUNING;
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
	private bool isRoverVisited;

	[Serialize]
	public string worldName;

	[Serialize]
	public string[] nameTables;

	[Serialize]
	public string worldType;

	[Serialize]
	public string worldDescription;

	[Serialize]
	public int sunlight = FIXEDTRAITS.SUNLIGHT.DEFAULT_VALUE;

	[Serialize]
	public int cosmicRadiation = FIXEDTRAITS.COSMICRADIATION.DEFAULT_VALUE;

	[Serialize]
	public float currentSunlightIntensity;

	[Serialize]
	public float currentCosmicIntensity = FIXEDTRAITS.COSMICRADIATION.DEFAULT_VALUE;

	[Serialize]
	public string sunlightFixedTrait;

	[Serialize]
	public string cosmicRadiationFixedTrait;

	[Serialize]
	public int fixedTraitsUpdateVersion = 1;

	private Dictionary<string, int> sunlightFixedTraits = new Dictionary<string, int>
	{
		{
			FIXEDTRAITS.SUNLIGHT.NAME.NONE,
			FIXEDTRAITS.SUNLIGHT.NONE
		},
		{
			FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_LOW,
			FIXEDTRAITS.SUNLIGHT.VERY_VERY_LOW
		},
		{
			FIXEDTRAITS.SUNLIGHT.NAME.VERY_LOW,
			FIXEDTRAITS.SUNLIGHT.VERY_LOW
		},
		{
			FIXEDTRAITS.SUNLIGHT.NAME.LOW,
			FIXEDTRAITS.SUNLIGHT.LOW
		},
		{
			FIXEDTRAITS.SUNLIGHT.NAME.MED_LOW,
			FIXEDTRAITS.SUNLIGHT.MED_LOW
		},
		{
			FIXEDTRAITS.SUNLIGHT.NAME.MED,
			FIXEDTRAITS.SUNLIGHT.MED
		},
		{
			FIXEDTRAITS.SUNLIGHT.NAME.MED_HIGH,
			FIXEDTRAITS.SUNLIGHT.MED_HIGH
		},
		{
			FIXEDTRAITS.SUNLIGHT.NAME.HIGH,
			FIXEDTRAITS.SUNLIGHT.HIGH
		},
		{
			FIXEDTRAITS.SUNLIGHT.NAME.VERY_HIGH,
			FIXEDTRAITS.SUNLIGHT.VERY_HIGH
		},
		{
			FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_HIGH,
			FIXEDTRAITS.SUNLIGHT.VERY_VERY_HIGH
		},
		{
			FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_VERY_HIGH,
			FIXEDTRAITS.SUNLIGHT.VERY_VERY_VERY_HIGH
		}
	};

	private Dictionary<string, int> cosmicRadiationFixedTraits = new Dictionary<string, int>
	{
		{
			FIXEDTRAITS.COSMICRADIATION.NAME.NONE,
			FIXEDTRAITS.COSMICRADIATION.NONE
		},
		{
			FIXEDTRAITS.COSMICRADIATION.NAME.VERY_VERY_LOW,
			FIXEDTRAITS.COSMICRADIATION.VERY_VERY_LOW
		},
		{
			FIXEDTRAITS.COSMICRADIATION.NAME.VERY_LOW,
			FIXEDTRAITS.COSMICRADIATION.VERY_LOW
		},
		{
			FIXEDTRAITS.COSMICRADIATION.NAME.LOW,
			FIXEDTRAITS.COSMICRADIATION.LOW
		},
		{
			FIXEDTRAITS.COSMICRADIATION.NAME.MED_LOW,
			FIXEDTRAITS.COSMICRADIATION.MED_LOW
		},
		{
			FIXEDTRAITS.COSMICRADIATION.NAME.MED,
			FIXEDTRAITS.COSMICRADIATION.MED
		},
		{
			FIXEDTRAITS.COSMICRADIATION.NAME.MED_HIGH,
			FIXEDTRAITS.COSMICRADIATION.MED_HIGH
		},
		{
			FIXEDTRAITS.COSMICRADIATION.NAME.HIGH,
			FIXEDTRAITS.COSMICRADIATION.HIGH
		},
		{
			FIXEDTRAITS.COSMICRADIATION.NAME.VERY_HIGH,
			FIXEDTRAITS.COSMICRADIATION.VERY_HIGH
		},
		{
			FIXEDTRAITS.COSMICRADIATION.NAME.VERY_VERY_HIGH,
			FIXEDTRAITS.COSMICRADIATION.VERY_VERY_HIGH
		}
	};

	[Serialize]
	private List<string> m_seasonIds;

	[Serialize]
	private List<string> m_subworldNames;

	[Serialize]
	private List<string> m_worldTraitIds;

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

	public bool IsRoverVisted => isRoverVisited;

	public Dictionary<string, int> SunlightFixedTraits => sunlightFixedTraits;

	public Dictionary<string, int> CosmicRadiationFixedTraits => cosmicRadiationFixedTraits;

	public List<string> Biomes => m_subworldNames;

	public List<string> WorldTraitIds => m_worldTraitIds;

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
		UpgradeFixedTraits();
		RefreshFixedTraits();
		if (DlcManager.IsPureVanilla())
		{
			isStartWorld = true;
			isDupeVisited = true;
		}
	}

	protected override void OnCleanUp()
	{
		SaveGame.Instance.materialSelectorSerializer.WipeWorldSelectionData(id);
		ClusterManager.Instance.UnregisterWorldContainer(this);
		base.OnCleanUp();
	}

	private void UpgradeFixedTraits()
	{
		if (sunlightFixedTrait == null || sunlightFixedTrait == "")
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary.Add(160000, FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_HIGH);
			dictionary.Add(0, FIXEDTRAITS.SUNLIGHT.NAME.NONE);
			dictionary.Add(10000, FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_LOW);
			dictionary.Add(20000, FIXEDTRAITS.SUNLIGHT.NAME.VERY_LOW);
			dictionary.Add(30000, FIXEDTRAITS.SUNLIGHT.NAME.LOW);
			dictionary.Add(35000, FIXEDTRAITS.SUNLIGHT.NAME.MED_LOW);
			dictionary.Add(40000, FIXEDTRAITS.SUNLIGHT.NAME.MED);
			dictionary.Add(50000, FIXEDTRAITS.SUNLIGHT.NAME.MED_HIGH);
			dictionary.Add(60000, FIXEDTRAITS.SUNLIGHT.NAME.HIGH);
			dictionary.Add(80000, FIXEDTRAITS.SUNLIGHT.NAME.VERY_HIGH);
			dictionary.Add(120000, FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_HIGH);
			dictionary.TryGetValue(sunlight, out sunlightFixedTrait);
		}
		if (cosmicRadiationFixedTrait == null || cosmicRadiationFixedTrait == "")
		{
			Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
			dictionary2.Add(0, FIXEDTRAITS.COSMICRADIATION.NAME.NONE);
			dictionary2.Add(6, FIXEDTRAITS.COSMICRADIATION.NAME.VERY_VERY_LOW);
			dictionary2.Add(12, FIXEDTRAITS.COSMICRADIATION.NAME.VERY_LOW);
			dictionary2.Add(18, FIXEDTRAITS.COSMICRADIATION.NAME.LOW);
			dictionary2.Add(21, FIXEDTRAITS.COSMICRADIATION.NAME.MED_LOW);
			dictionary2.Add(25, FIXEDTRAITS.COSMICRADIATION.NAME.MED);
			dictionary2.Add(31, FIXEDTRAITS.COSMICRADIATION.NAME.MED_HIGH);
			dictionary2.Add(37, FIXEDTRAITS.COSMICRADIATION.NAME.HIGH);
			dictionary2.Add(50, FIXEDTRAITS.COSMICRADIATION.NAME.VERY_HIGH);
			dictionary2.Add(75, FIXEDTRAITS.COSMICRADIATION.NAME.VERY_VERY_HIGH);
			dictionary2.TryGetValue(cosmicRadiation, out cosmicRadiationFixedTrait);
		}
	}

	private void RefreshFixedTraits()
	{
		sunlight = GetSunlightValueFromFixedTrait();
		cosmicRadiation = GetCosmicRadiationValueFromFixedTrait();
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
		return GameUtil.GenerateRandomWorldName(nameTables);
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
			isDupeVisited = true;
			Game.Instance.Trigger(-434755240, this);
		}
	}

	public void SetRoverLanded()
	{
		isRoverVisited = true;
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

	private string GetSunlightFromFixedTraits(WorldGen world)
	{
		foreach (string fixedTrait in world.Settings.world.fixedTraits)
		{
			if (sunlightFixedTraits.ContainsKey(fixedTrait))
			{
				return fixedTrait;
			}
		}
		return FIXEDTRAITS.SUNLIGHT.NAME.DEFAULT;
	}

	private string GetCosmicRadiationFromFixedTraits(WorldGen world)
	{
		foreach (string fixedTrait in world.Settings.world.fixedTraits)
		{
			if (cosmicRadiationFixedTraits.ContainsKey(fixedTrait))
			{
				return fixedTrait;
			}
		}
		return FIXEDTRAITS.COSMICRADIATION.NAME.DEFAULT;
	}

	private int GetSunlightValueFromFixedTrait()
	{
		if (sunlightFixedTrait == null)
		{
			sunlightFixedTrait = FIXEDTRAITS.SUNLIGHT.NAME.DEFAULT;
		}
		if (sunlightFixedTraits.ContainsKey(sunlightFixedTrait))
		{
			return sunlightFixedTraits[sunlightFixedTrait];
		}
		return FIXEDTRAITS.SUNLIGHT.DEFAULT_VALUE;
	}

	private int GetCosmicRadiationValueFromFixedTrait()
	{
		if (cosmicRadiationFixedTrait == null)
		{
			cosmicRadiationFixedTrait = FIXEDTRAITS.COSMICRADIATION.NAME.DEFAULT;
		}
		if (cosmicRadiationFixedTraits.ContainsKey(cosmicRadiationFixedTrait))
		{
			return cosmicRadiationFixedTraits[cosmicRadiationFixedTrait];
		}
		return FIXEDTRAITS.COSMICRADIATION.DEFAULT_VALUE;
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
			nameTables = world.Settings.world.nameTables;
			worldDescription = world.Settings.world.description;
			worldType = world.Settings.world.name;
			isModuleInterior = world.Settings.world.moduleInterior;
			m_seasonIds = new List<string>(world.Settings.world.seasons);
			sunlightFixedTrait = GetSunlightFromFixedTraits(world);
			cosmicRadiationFixedTrait = GetCosmicRadiationFromFixedTraits(world);
			sunlight = GetSunlightValueFromFixedTrait();
			cosmicRadiation = GetCosmicRadiationValueFromFixedTrait();
			currentCosmicIntensity = cosmicRadiation;
			m_subworldNames = new List<string>();
			foreach (WeightedSubworldName subworldFile in world.Settings.world.subworldFiles)
			{
				string text = subworldFile.name;
				text = text.Substring(0, text.LastIndexOf('/'));
				text = text.Substring(text.LastIndexOf('/') + 1, text.Length - (text.LastIndexOf('/') + 1));
				m_subworldNames.Add(text);
			}
			m_worldTraitIds = new List<string>();
			m_worldTraitIds.AddRange(world.Settings.GetTraitIDs());
		}
		else
		{
			fullyEnclosedBorder = false;
			worldOffset = Vector2I.zero;
			worldSize = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
			isDiscovered = true;
			isStartWorld = true;
			isDupeVisited = true;
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

	public void SpacePodAllDupes(AxialI sourceLocation, SimHashes podElement)
	{
		foreach (MinionIdentity worldItem in Components.MinionIdentities.GetWorldItems(id))
		{
			if (!worldItem.HasTag(GameTags.Dead))
			{
				GameObject obj = Util.KInstantiate(position: new Vector3(-1f, -1f, 0f), original: Assets.GetPrefab("EscapePod"));
				obj.GetComponent<PrimaryElement>().SetElement(podElement);
				obj.SetActive(value: true);
				obj.GetComponent<MinionStorage>().SerializeMinion(worldItem.gameObject);
				TravellingCargoLander.StatesInstance sMI = obj.GetSMI<TravellingCargoLander.StatesInstance>();
				sMI.StartSM();
				sMI.Travel(sourceLocation, ClusterUtil.ClosestVisibleAsteroidToLocation(sourceLocation).Location);
			}
		}
	}

	public void DestroyWorldBuildings(out HashSet<int> noRefundTiles)
	{
		TransferBuildingMaterials(out noRefundTiles);
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

	public void TransferResourcesToDebris(AxialI sourceLocation, HashSet<int> noRefundTiles, SimHashes debrisContainerElement)
	{
		List<Storage> debrisObjects = new List<Storage>();
		TransferPickupablesToDebris(ref debrisObjects, debrisContainerElement);
		TransferLiquidsSolidsAndGasesToDebris(ref debrisObjects, noRefundTiles, debrisContainerElement);
		foreach (Storage item in debrisObjects)
		{
			RailGunPayload.StatesInstance sMI = item.GetSMI<RailGunPayload.StatesInstance>();
			sMI.StartSM();
			sMI.Travel(sourceLocation, ClusterUtil.ClosestVisibleAsteroidToLocation(sourceLocation).Location);
		}
	}

	private void TransferBuildingMaterials(out HashSet<int> noRefundTiles)
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
						element.substance.SpawnResource(buildingComplete.transform.GetPosition(), buildingComplete.Def.Mass[i], temperature, diseaseIdx, diseaseCount);
						continue;
					}
					GameObject prefab = Assets.GetPrefab(component.constructionElements[i]);
					for (int j = 0; (float)j < buildingComplete.Def.Mass[i]; j++)
					{
						GameUtil.KInstantiate(prefab, buildingComplete.transform.GetPosition(), Grid.SceneLayer.Ore).SetActive(value: true);
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
				component4.DropAll();
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

	private void TransferPickupablesToDebris(ref List<Storage> debrisObjects, SimHashes debrisContainerElement)
	{
		ListPool<ScenePartitionerEntry, ClusterManager>.PooledList pooledList = ListPool<ScenePartitionerEntry, ClusterManager>.Allocate();
		GameScenePartitioner.Instance.GatherEntries((int)minimumBounds.x, (int)minimumBounds.y, Width, Height, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		foreach (ScenePartitionerEntry item in pooledList)
		{
			if (item.obj == null)
			{
				continue;
			}
			Pickupable pickupable = item.obj as Pickupable;
			if (!(pickupable != null))
			{
				continue;
			}
			if (pickupable.HasTag(GameTags.Minion))
			{
				Util.KDestroyGameObject(pickupable.gameObject);
				continue;
			}
			pickupable.PrimaryElement.Units = Mathf.Max(1, Mathf.RoundToInt(pickupable.PrimaryElement.Units * 0.5f));
			if ((debrisObjects.Count == 0 || debrisObjects[debrisObjects.Count - 1].RemainingCapacity() == 0f) && pickupable.PrimaryElement.Mass > 0f)
			{
				debrisObjects.Add(CraftModuleInterface.SpawnRocketDebris(" from World Objects", debrisContainerElement));
			}
			Storage storage = debrisObjects[debrisObjects.Count - 1];
			while (pickupable.PrimaryElement.Mass > storage.RemainingCapacity())
			{
				Pickupable pickupable2 = pickupable.Take(storage.RemainingCapacity());
				storage.Store(pickupable2.gameObject);
				storage = CraftModuleInterface.SpawnRocketDebris(" from World Objects", debrisContainerElement);
				debrisObjects.Add(storage);
			}
			if (pickupable.PrimaryElement.Mass > 0f)
			{
				storage.Store(pickupable.gameObject);
			}
		}
		pooledList.Recycle();
	}

	private void TransferLiquidsSolidsAndGasesToDebris(ref List<Storage> debrisObjects, HashSet<int> noRefundTiles, SimHashes debrisContainerElement)
	{
		for (int i = (int)minimumBounds.x; (float)i <= maximumBounds.x; i++)
		{
			for (int j = (int)minimumBounds.y; (float)j <= maximumBounds.y; j++)
			{
				int num = Grid.XYToCell(i, j);
				if (noRefundTiles.Contains(num))
				{
					continue;
				}
				Element element = Grid.Element[num];
				if (element == null || element.IsVacuum)
				{
					continue;
				}
				float num2 = Grid.Mass[num];
				num2 *= 0.5f;
				if ((debrisObjects.Count == 0 || debrisObjects[debrisObjects.Count - 1].RemainingCapacity() == 0f) && num2 > 0f)
				{
					debrisObjects.Add(CraftModuleInterface.SpawnRocketDebris(" from World Tiles", debrisContainerElement));
				}
				Storage storage = debrisObjects[debrisObjects.Count - 1];
				while (num2 > 0f)
				{
					float num3 = Mathf.Min(num2, storage.RemainingCapacity());
					num2 -= num3;
					storage.AddOre(element.id, num3, Grid.Temperature[num], Grid.DiseaseIdx[num], Grid.DiseaseCount[num]);
					if (num2 > 0f)
					{
						storage = CraftModuleInterface.SpawnRocketDebris(" from World Tiles", debrisContainerElement);
						debrisObjects.Add(storage);
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
