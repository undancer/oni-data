using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Constructable")]
public class Constructable : Workable, ISaveLoadable
{
	[MyCmpAdd]
	private Storage storage;

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private Rotatable rotatable;

	private Notification invalidLocation;

	private float initialTemperature = -1f;

	[Serialize]
	private bool isPrioritized;

	private FetchList2 fetchList;

	private Chore buildChore;

	private bool materialNeedsCleared;

	private bool hasUnreachableDigs;

	private bool finished;

	private bool unmarked;

	public bool isDiggingRequired = true;

	private bool waitForFetchesBeforeDigging;

	private bool hasLadderNearby;

	private Extents ladderDetectionExtents;

	[Serialize]
	public bool IsReplacementTile;

	private HandleVector<int>.Handle solidPartitionerEntry;

	private HandleVector<int>.Handle digPartitionerEntry;

	private HandleVector<int>.Handle ladderParititonerEntry;

	private LoggerFSS log = new LoggerFSS("Constructable");

	[Serialize]
	private Tag[] selectedElementsTags;

	private Element[] selectedElements;

	[Serialize]
	private int[] ids;

	private static readonly EventSystem.IntraObjectHandler<Constructable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Constructable>(delegate(Constructable component, object data)
	{
		component.OnReachableChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Constructable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Constructable>(delegate(Constructable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Constructable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Constructable>(delegate(Constructable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public Recipe Recipe => building.Def.CraftRecipe;

	public IList<Tag> SelectedElementsTags
	{
		get
		{
			return selectedElementsTags;
		}
		set
		{
			if (selectedElementsTags == null || selectedElementsTags.Length != value.Count)
			{
				selectedElementsTags = new Tag[value.Count];
			}
			value.CopyTo(selectedElementsTags, 0);
		}
	}

	public override string GetConversationTopic()
	{
		return building.Def.PrefabID;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		float num = 0f;
		float num2 = 0f;
		bool flag = true;
		foreach (GameObject item in storage.items)
		{
			if (!(item == null))
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (!(component == null))
				{
					num += component.Mass;
					num2 += component.Temperature * component.Mass;
					flag = flag && component.HasTag(GameTags.Liquifiable);
				}
			}
		}
		if (num <= 0f)
		{
			DebugUtil.LogWarningArgs(base.gameObject, "uhhh this constructable is about to generate a nan", "Item Count: ", storage.items.Count);
			return;
		}
		if (flag)
		{
			initialTemperature = Mathf.Min(num2 / num, 318.15f);
		}
		else
		{
			initialTemperature = Mathf.Clamp(num2 / num, 288.15f, 318.15f);
		}
		KAnimGraphTileVisualizer component2 = GetComponent<KAnimGraphTileVisualizer>();
		UtilityConnections connections = ((!(component2 == null)) ? component2.Connections : ((UtilityConnections)0));
		bool flag2 = true;
		if (IsReplacementTile)
		{
			int cell = Grid.PosToCell(base.transform.GetLocalPosition());
			GameObject replacementCandidate = building.Def.GetReplacementCandidate(cell);
			if (replacementCandidate != null)
			{
				flag2 = false;
				SimCellOccupier component3 = replacementCandidate.GetComponent<SimCellOccupier>();
				if (component3 != null)
				{
					component3.DestroySelf(delegate
					{
						if (this != null && base.gameObject != null)
						{
							FinishConstruction(connections);
						}
					});
				}
				else
				{
					Conduit component4 = replacementCandidate.GetComponent<Conduit>();
					if (component4 != null)
					{
						component4.GetFlowManager().MarkForReplacement(cell);
					}
					BuildingComplete component5 = replacementCandidate.GetComponent<BuildingComplete>();
					if (component5 != null)
					{
						component5.Subscribe(-21016276, delegate
						{
							FinishConstruction(connections);
						});
					}
					else
					{
						Debug.LogWarning("Why am I trying to replace a: " + replacementCandidate.name);
						FinishConstruction(connections);
					}
				}
				KAnimGraphTileVisualizer component6 = replacementCandidate.GetComponent<KAnimGraphTileVisualizer>();
				if (component6 != null)
				{
					component6.skipCleanup = true;
				}
				Deconstructable component7 = replacementCandidate.GetComponent<Deconstructable>();
				if (component7 != null)
				{
					component7.SpawnItemsFromConstruction();
				}
				replacementCandidate.Trigger(1606648047, building.Def.TileLayer);
				replacementCandidate.DeleteObject();
			}
		}
		if (flag2)
		{
			FinishConstruction(connections);
		}
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, GetComponent<KSelectable>().GetName(), base.transform);
	}

	private void FinishConstruction(UtilityConnections connections)
	{
		Rotatable component = GetComponent<Rotatable>();
		Orientation orientation = ((component != null) ? component.GetOrientation() : Orientation.Neutral);
		int cell = Grid.PosToCell(base.transform.GetLocalPosition());
		UnmarkArea();
		GameObject gameObject = building.Def.Build(cell, orientation, storage, selectedElementsTags, initialTemperature, playsound: true, GameClock.Instance.GetTime());
		gameObject.transform.rotation = base.transform.rotation;
		Rotatable component2 = gameObject.GetComponent<Rotatable>();
		if (component2 != null)
		{
			component2.SetOrientation(orientation);
		}
		KAnimGraphTileVisualizer component3 = GetComponent<KAnimGraphTileVisualizer>();
		if (component3 != null)
		{
			gameObject.GetComponent<KAnimGraphTileVisualizer>().Connections = connections;
			component3.skipCleanup = true;
		}
		KSelectable component4 = GetComponent<KSelectable>();
		if (component4 != null && component4.IsSelected && gameObject.GetComponent<KSelectable>() != null)
		{
			component4.Unselect();
			if (PlayerController.Instance.ActiveTool.name == "SelectTool")
			{
				((SelectTool)PlayerController.Instance.ActiveTool).SelectNextFrame(gameObject.GetComponent<KSelectable>());
			}
		}
		storage.ConsumeAllIgnoringDisease();
		finished = true;
		this.DeleteObject();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		invalidLocation = new Notification(MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => string.Concat(MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.TOOLTIP, notificationList.ReduceMessages(countNames: false)));
		CellOffset[][] table = OffsetGroups.InvertedStandardTable;
		if (building.Def.IsTilePiece)
		{
			table = OffsetGroups.InvertedStandardTableWithCorners;
		}
		CellOffset[][] offsetTable = OffsetGroups.BuildReachabilityTable(building.Def.PlacementOffsets, table, building.Def.ConstructionOffsetFilter);
		SetOffsetTable(offsetTable);
		storage.SetOffsetTable(offsetTable);
		faceTargetWhenWorking = true;
		Subscribe(-1432940121, OnReachableChangedDelegate);
		if (rotatable == null)
		{
			MarkArea();
		}
		workerStatusItem = Db.Get().DuplicantStatusItems.Building;
		workingStatusItem = null;
		attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		minimumAttributeMultiplier = 0.75f;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		Prioritizable.AddRef(base.gameObject);
		synchronizeAnims = false;
		multitoolContext = "build";
		multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		workingPstComplete = null;
		workingPstFailed = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(2127324410, OnCancelDelegate);
		if (rotatable != null)
		{
			MarkArea();
		}
		fetchList = new FetchList2(storage, Db.Get().ChoreTypes.BuildFetch);
		PrimaryElement component = GetComponent<PrimaryElement>();
		Element element = ElementLoader.GetElement(SelectedElementsTags[0]);
		Debug.Assert(element != null, "Missing primary element for Constructable");
		component.ElementID = element.id;
		float num3 = (component.Temperature = (component.Temperature = 293.15f));
		Recipe.Ingredient[] allIngredients = Recipe.GetAllIngredients(selectedElementsTags);
		foreach (Recipe.Ingredient ingredient in allIngredients)
		{
			fetchList.Add(ingredient.tag, null, null, ingredient.amount);
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, ingredient.amount);
		}
		if (!building.Def.IsTilePiece)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Construction");
		}
		building.RunOnArea(delegate(int offset_cell)
		{
			if (base.gameObject.GetComponent<ConduitBridge>() == null)
			{
				GameObject gameObject3 = Grid.Objects[offset_cell, 7];
				if (gameObject3 != null)
				{
					gameObject3.DeleteObject();
				}
			}
		});
		if (IsReplacementTile)
		{
			GameObject gameObject = null;
			if (building.Def.ReplacementLayer != ObjectLayer.NumLayers)
			{
				int cell = Grid.PosToCell(base.transform.GetPosition());
				gameObject = Grid.Objects[cell, (int)building.Def.ReplacementLayer];
				if (gameObject == null || gameObject == base.gameObject)
				{
					Grid.Objects[cell, (int)building.Def.ReplacementLayer] = base.gameObject;
					if (base.gameObject.GetComponent<SimCellOccupier>() != null)
					{
						int renderLayer = LayerMask.NameToLayer("Overlay");
						World.Instance.blockTileRenderer.AddBlock(renderLayer, building.Def, IsReplacementTile, SimHashes.Void, cell);
					}
					TileVisualizer.RefreshCell(cell, building.Def.TileLayer, building.Def.ReplacementLayer);
				}
				else
				{
					Debug.LogError("multiple replacement tiles on the same cell!");
					Util.KDestroyGameObject(base.gameObject);
				}
				GameObject gameObject2 = Grid.Objects[cell, (int)building.Def.ObjectLayer];
				if (gameObject2 != null)
				{
					Deconstructable component2 = gameObject2.GetComponent<Deconstructable>();
					if (component2 != null)
					{
						component2.CancelDeconstruction();
					}
				}
			}
		}
		bool flag = building.Def.BuildingComplete.GetComponent<Ladder>();
		waitForFetchesBeforeDigging = flag || (bool)building.Def.BuildingComplete.GetComponent<SimCellOccupier>() || (bool)building.Def.BuildingComplete.GetComponent<Door>() || (bool)building.Def.BuildingComplete.GetComponent<LiquidPumpingStation>();
		if (flag)
		{
			int x = 0;
			int y = 0;
			Grid.CellToXY(Grid.PosToCell(this), out x, out y);
			int y2 = y - 3;
			ladderDetectionExtents = new Extents(x, y2, 1, 5);
			ladderParititonerEntry = GameScenePartitioner.Instance.Add("Constructable.OnNearbyBuildingLayerChanged", base.gameObject, ladderDetectionExtents, GameScenePartitioner.Instance.objectLayers[1], OnNearbyBuildingLayerChanged);
			OnNearbyBuildingLayerChanged(null);
		}
		fetchList.Submit(OnFetchListComplete, check_storage_contents: true);
		PlaceDiggables();
		new ReachabilityMonitor.Instance(this).StartSM();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Prioritizable component3 = GetComponent<Prioritizable>();
		component3.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(component3.onPriorityChanged, new Action<PrioritySetting>(OnPriorityChanged));
		OnPriorityChanged(component3.GetMasterPriority());
	}

	private void OnPriorityChanged(PrioritySetting priority)
	{
		building.RunOnArea(delegate(int cell)
		{
			Diggable diggable = Diggable.GetDiggable(cell);
			if (diggable != null)
			{
				diggable.GetComponent<Prioritizable>().SetMasterPriority(priority);
			}
		});
	}

	private void MarkArea()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		BuildingDef def = building.Def;
		Orientation orientation = building.Orientation;
		ObjectLayer layer = (IsReplacementTile ? def.ReplacementLayer : def.ObjectLayer);
		def.MarkArea(num, orientation, layer, base.gameObject);
		if (!def.IsTilePiece)
		{
			return;
		}
		if (Grid.Objects[num, (int)def.TileLayer] == null)
		{
			def.MarkArea(num, orientation, def.TileLayer, base.gameObject);
			def.RunOnArea(num, orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, def.TileLayer, def.ReplacementLayer);
			});
		}
		Grid.IsTileUnderConstruction[num] = true;
	}

	private void UnmarkArea()
	{
		if (!unmarked)
		{
			unmarked = true;
			int num = Grid.PosToCell(base.transform.GetPosition());
			BuildingDef def = building.Def;
			def.UnmarkArea(layer: IsReplacementTile ? building.Def.ReplacementLayer : building.Def.ObjectLayer, cell: num, orientation: building.Orientation, go: base.gameObject);
			if (def.IsTilePiece)
			{
				Grid.IsTileUnderConstruction[num] = false;
			}
		}
	}

	private void OnNearbyBuildingLayerChanged(object data)
	{
		hasLadderNearby = false;
		for (int i = ladderDetectionExtents.y; i < ladderDetectionExtents.y + ladderDetectionExtents.height; i++)
		{
			int num = Grid.OffsetCell(0, ladderDetectionExtents.x, i);
			if (Grid.IsValidCell(num))
			{
				GameObject value = null;
				Grid.ObjectLayers[1].TryGetValue(num, out value);
				if (value != null && value.GetComponent<Ladder>() != null)
				{
					hasLadderNearby = true;
					break;
				}
			}
		}
	}

	private bool IsWire()
	{
		return building.Def.name.Contains("Wire");
	}

	public bool IconConnectionAnimation(float delay, int connectionCount, string defName, string soundName)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (this.building.Def.Name.Contains(defName))
		{
			Building building = null;
			GameObject gameObject = Grid.Objects[num, 1];
			if (gameObject != null)
			{
				building = gameObject.GetComponent<Building>();
			}
			if (building != null)
			{
				bool flag = IsWire();
				int num2 = (flag ? building.GetPowerInputCell() : building.GetUtilityInputCell());
				int num3 = (flag ? num2 : building.GetUtilityOutputCell());
				if (num == num2 || num == num3)
				{
					BuildingCellVisualizer component = building.gameObject.GetComponent<BuildingCellVisualizer>();
					if (component != null && (flag ? component.RequiresPower : component.RequiresUtilityConnection))
					{
						component.ConnectedEventWithDelay(delay, connectionCount, num, soundName);
						return true;
					}
				}
			}
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		if (IsReplacementTile && building.Def.isKAnimTile)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			GameObject gameObject = Grid.Objects[cell, (int)building.Def.ReplacementLayer];
			if (gameObject == base.gameObject && gameObject.GetComponent<SimCellOccupier>() != null)
			{
				World.Instance.blockTileRenderer.RemoveBlock(building.Def, IsReplacementTile, SimHashes.Void, cell);
			}
		}
		GameScenePartitioner.Instance.Free(ref solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref digPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref ladderParititonerEntry);
		SaveLoadRoot component = GetComponent<SaveLoadRoot>();
		if (component != null)
		{
			SaveLoader.Instance.saveManager.Unregister(component);
		}
		if (fetchList != null)
		{
			fetchList.Cancel("Constructable destroyed");
		}
		UnmarkArea();
		int[] placementCells = building.PlacementCells;
		for (int i = 0; i < placementCells.Length; i++)
		{
			Diggable diggable = Diggable.GetDiggable(placementCells[i]);
			if (diggable != null)
			{
				diggable.gameObject.DeleteObject();
			}
		}
		base.OnCleanUp();
	}

	private void OnDiggableReachabilityChanged(object data)
	{
		if (IsReplacementTile)
		{
			return;
		}
		int diggable_count = 0;
		int unreachable_count = 0;
		building.RunOnArea(delegate(int offset_cell)
		{
			Diggable diggable = Diggable.GetDiggable(offset_cell);
			if (diggable != null)
			{
				diggable_count++;
				if (!diggable.GetComponent<KPrefabID>().HasTag(GameTags.Reachable))
				{
					unreachable_count++;
				}
			}
		});
		bool flag = unreachable_count > 0 && unreachable_count == diggable_count;
		if (flag != hasUnreachableDigs)
		{
			if (flag)
			{
				GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ConstructableDigUnreachable);
			}
			else
			{
				GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ConstructableDigUnreachable);
			}
			hasUnreachableDigs = flag;
		}
	}

	private void PlaceDiggables()
	{
		if (waitForFetchesBeforeDigging && fetchList != null && !hasLadderNearby)
		{
			OnDiggableReachabilityChanged(null);
			return;
		}
		bool digs_complete = true;
		if (!solidPartitionerEntry.IsValid())
		{
			Extents validPlacementExtents = building.GetValidPlacementExtents();
			solidPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.OnFetchListComplete", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChangedOrDigDestroyed);
			digPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.OnFetchListComplete", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.digDestroyedLayer, OnSolidChangedOrDigDestroyed);
		}
		if (!IsReplacementTile)
		{
			building.RunOnArea(delegate(int offset_cell)
			{
				PrioritySetting masterPriority = GetComponent<Prioritizable>().GetMasterPriority();
				if (Diggable.IsDiggable(offset_cell))
				{
					digs_complete = false;
					Diggable diggable = Diggable.GetDiggable(offset_cell);
					if (diggable == null)
					{
						diggable = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")), Grid.SceneLayer.Move).GetComponent<Diggable>();
						diggable.gameObject.SetActive(value: true);
						diggable.transform.SetPosition(Grid.CellToPosCBC(offset_cell, Grid.SceneLayer.Move));
						diggable.Subscribe(-1432940121, OnDiggableReachabilityChanged);
						Grid.Objects[offset_cell, 7] = diggable.gameObject;
					}
					else
					{
						diggable.Unsubscribe(-1432940121, OnDiggableReachabilityChanged);
						diggable.Subscribe(-1432940121, OnDiggableReachabilityChanged);
					}
					diggable.choreTypeIdHash = Db.Get().ChoreTypes.BuildDig.IdHash;
					diggable.GetComponent<Prioritizable>().SetMasterPriority(masterPriority);
					RenderUtil.EnableRenderer(diggable.transform, is_enabled: false);
					SaveLoadRoot component = diggable.GetComponent<SaveLoadRoot>();
					if (component != null)
					{
						UnityEngine.Object.Destroy(component);
					}
				}
			});
			OnDiggableReachabilityChanged(null);
		}
		bool flag = building.Def.IsValidBuildLocation(base.gameObject, base.transform.GetPosition(), building.Orientation);
		if (flag)
		{
			notifier.Remove(invalidLocation);
		}
		else
		{
			notifier.Add(invalidLocation);
		}
		GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidBuildingLocation, !flag, this);
		bool flag2 = digs_complete && flag && fetchList == null;
		if (flag2 && buildChore == null)
		{
			buildChore = new WorkChore<Constructable>(Db.Get().ChoreTypes.Build, this, null, run_until_complete: true, UpdateBuildState, UpdateBuildState, UpdateBuildState, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: true);
			UpdateBuildState(buildChore);
		}
		else if (!flag2 && buildChore != null)
		{
			buildChore.Cancel("Need to dig");
			buildChore = null;
		}
	}

	private void OnFetchListComplete()
	{
		fetchList = null;
		PlaceDiggables();
		ClearMaterialNeeds();
	}

	private void ClearMaterialNeeds()
	{
		if (!materialNeedsCleared)
		{
			Recipe.Ingredient[] allIngredients = Recipe.GetAllIngredients(SelectedElementsTags);
			foreach (Recipe.Ingredient ingredient in allIngredients)
			{
				MaterialNeeds.Instance.UpdateNeed(ingredient.tag, 0f - ingredient.amount);
			}
			materialNeedsCleared = true;
		}
	}

	private void OnSolidChangedOrDigDestroyed(object data)
	{
		if (!(this == null) && !finished)
		{
			PlaceDiggables();
		}
	}

	private void UpdateBuildState(Chore chore)
	{
		KSelectable component = GetComponent<KSelectable>();
		if (chore.InProgress())
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.UnderConstruction);
		}
		else
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.UnderConstructionNoWorker);
		}
	}

	[OnDeserialized]
	internal void OnDeserialized()
	{
		if (ids == null)
		{
			return;
		}
		selectedElements = new Element[ids.Length];
		for (int i = 0; i < ids.Length; i++)
		{
			selectedElements[i] = ElementLoader.FindElementByHash((SimHashes)ids[i]);
		}
		if (selectedElementsTags == null)
		{
			selectedElementsTags = new Tag[ids.Length];
			for (int j = 0; j < ids.Length; j++)
			{
				selectedElementsTags[j] = ElementLoader.FindElementByHash((SimHashes)ids[j]).tag;
			}
		}
		Debug.Assert(selectedElements.Length == selectedElementsTags.Length);
		for (int k = 0; k < selectedElements.Length; k++)
		{
			Debug.Assert(selectedElements[k].tag == SelectedElementsTags[k]);
		}
	}

	private void OnReachableChanged(object data)
	{
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		if ((bool)data)
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ConstructionUnreachable);
			if (component != null)
			{
				component.TintColour = Game.Instance.uiColours.Build.validLocation;
			}
		}
		else
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ConstructionUnreachable, this);
			if (component != null)
			{
				component.TintColour = Game.Instance.uiColours.Build.unreachable;
			}
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_cancel", UI.USERMENUACTIONS.CANCELCONSTRUCTION.NAME, OnPressCancel, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCELCONSTRUCTION.TOOLTIP));
	}

	private void OnPressCancel()
	{
		base.gameObject.Trigger(2127324410);
	}

	private void OnCancel(object data = null)
	{
		DetailsScreen.Instance.Show(show: false);
		ClearMaterialNeeds();
	}
}
