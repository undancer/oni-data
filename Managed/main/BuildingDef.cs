using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Klei;
using Klei.AI;
using ProcGen;
using STRINGS;
using UnityEngine;

[Serializable]
public class BuildingDef : Def
{
	public string[] RequiredDlcIds;

	public float EnergyConsumptionWhenActive;

	public float GeneratorWattageRating;

	public float GeneratorBaseCapacity;

	public float MassForTemperatureModification;

	public float ExhaustKilowattsWhenActive;

	public float SelfHeatKilowattsWhenActive;

	public float BaseMeltingPoint;

	public float ConstructionTime;

	public float WorkTime;

	public float ThermalConductivity = 1f;

	public int WidthInCells;

	public int HeightInCells;

	public int HitPoints;

	public bool RequiresPowerInput;

	public bool AddLogicPowerPort = true;

	public bool RequiresPowerOutput;

	public bool UseWhitePowerOutputConnectorColour;

	public CellOffset ElectricalArrowOffset;

	public ConduitType InputConduitType;

	public ConduitType OutputConduitType;

	public bool ModifiesTemperature;

	public bool Floodable = true;

	public bool Disinfectable = true;

	public bool Entombable = true;

	public bool Replaceable = true;

	public bool Invincible;

	public bool Overheatable = true;

	public bool Repairable = true;

	public float OverheatTemperature = 348.15f;

	public float FatalHot = 533.15f;

	public bool Breakable;

	public bool ContinuouslyCheckFoundation;

	public bool IsFoundation;

	[Obsolete]
	public bool isSolidTile;

	public bool DragBuild;

	public bool UseStructureTemperature = true;

	public Action HotKey = Action.NumActions;

	public CellOffset attachablePosition = new CellOffset(0, 0);

	public bool CanMove;

	public bool Cancellable = true;

	public bool OnePerWorld;

	public bool PlayConstructionSounds = true;

	public List<Tag> ReplacementTags;

	public List<ObjectLayer> ReplacementCandidateLayers;

	public List<ObjectLayer> EquivalentReplacementLayers;

	[NonSerialized]
	[HashedEnum]
	public HashedString ViewMode = OverlayModes.None.ID;

	public BuildLocationRule BuildLocationRule;

	public ObjectLayer ObjectLayer = ObjectLayer.Building;

	public ObjectLayer TileLayer = ObjectLayer.NumLayers;

	public ObjectLayer ReplacementLayer = ObjectLayer.NumLayers;

	public string DiseaseCellVisName;

	public string[] MaterialCategory;

	public string AudioCategory = "Metal";

	public string AudioSize = "medium";

	public float[] Mass;

	public bool AlwaysOperational;

	public List<LogicPorts.Port> LogicInputPorts;

	public List<LogicPorts.Port> LogicOutputPorts;

	public bool Upgradeable;

	public float BaseTimeUntilRepair = 600f;

	public bool ShowInBuildMenu = true;

	public bool DebugOnly;

	public PermittedRotations PermittedRotations;

	public Orientation InitialOrientation;

	public bool Deprecated;

	public bool UseHighEnergyParticleInputPort;

	public bool UseHighEnergyParticleOutputPort;

	public CellOffset HighEnergyParticleInputOffset;

	public CellOffset HighEnergyParticleOutputOffset;

	public CellOffset PowerInputOffset;

	public CellOffset PowerOutputOffset;

	public CellOffset UtilityInputOffset = new CellOffset(0, 1);

	public CellOffset UtilityOutputOffset = new CellOffset(1, 0);

	public Grid.SceneLayer SceneLayer = Grid.SceneLayer.Building;

	public Grid.SceneLayer ForegroundLayer = Grid.SceneLayer.BuildingFront;

	public string RequiredAttribute = "";

	public int RequiredAttributeLevel;

	public List<Descriptor> EffectDescription;

	public float MassTier;

	public float HeatTier;

	public float ConstructionTimeTier;

	public string PrimaryUse;

	public string SecondaryUse;

	public string PrimarySideEffect;

	public string SecondarySideEffect;

	public Recipe CraftRecipe;

	public Sprite UISprite;

	public bool isKAnimTile;

	public bool isUtility;

	public KAnimFile[] AnimFiles;

	public string DefaultAnimState = "off";

	public bool BlockTileIsTransparent;

	public TextureAtlas BlockTileAtlas;

	public TextureAtlas BlockTilePlaceAtlas;

	public TextureAtlas BlockTileShineAtlas;

	public Material BlockTileMaterial;

	public BlockTileDecorInfo DecorBlockTileInfo;

	public BlockTileDecorInfo DecorPlaceBlockTileInfo;

	public List<Klei.AI.Attribute> attributes = new List<Klei.AI.Attribute>();

	public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();

	public Tag AttachmentSlotTag;

	public bool PreventIdleTraversalPastBuilding;

	public GameObject BuildingComplete;

	public GameObject BuildingPreview;

	public GameObject BuildingUnderConstruction;

	public CellOffset[] PlacementOffsets;

	public CellOffset[] ConstructionOffsetFilter;

	public static CellOffset[] ConstructionOffsetFilter_OneDown = new CellOffset[1]
	{
		new CellOffset(0, -1)
	};

	public float BaseDecor;

	public float BaseDecorRadius;

	public int BaseNoisePollution;

	public int BaseNoisePollutionRadius;

	private static Dictionary<CellOffset, CellOffset[]> placementOffsetsCache = new Dictionary<CellOffset, CellOffset[]>();

	public override string Name => Strings.Get("STRINGS.BUILDINGS.PREFABS." + PrefabID.ToUpper() + ".NAME");

	public string Desc => Strings.Get("STRINGS.BUILDINGS.PREFABS." + PrefabID.ToUpper() + ".DESC");

	public string Flavor => string.Concat("\"", Strings.Get("STRINGS.BUILDINGS.PREFABS." + PrefabID.ToUpper() + ".FLAVOR"), "\"");

	public string Effect => Strings.Get("STRINGS.BUILDINGS.PREFABS." + PrefabID.ToUpper() + ".EFFECT");

	public bool IsTilePiece => TileLayer != ObjectLayer.NumLayers;

	public bool CanReplace(GameObject go)
	{
		if (ReplacementTags == null)
		{
			return false;
		}
		foreach (Tag replacementTag in ReplacementTags)
		{
			if (go.GetComponent<KPrefabID>().HasTag(replacementTag))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAvailable()
	{
		if (!Deprecated)
		{
			if (DebugOnly)
			{
				return Game.Instance.DebugOnlyBuildingsAllowed;
			}
			return true;
		}
		return false;
	}

	public bool ShouldShowInBuildMenu()
	{
		return ShowInBuildMenu;
	}

	public bool IsReplacementLayerOccupied(int cell)
	{
		if (Grid.Objects[cell, (int)ReplacementLayer] != null)
		{
			return true;
		}
		if (EquivalentReplacementLayers != null)
		{
			foreach (ObjectLayer equivalentReplacementLayer in EquivalentReplacementLayers)
			{
				if (Grid.Objects[cell, (int)equivalentReplacementLayer] != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	public GameObject GetReplacementCandidate(int cell)
	{
		if (ReplacementCandidateLayers != null)
		{
			foreach (ObjectLayer replacementCandidateLayer in ReplacementCandidateLayers)
			{
				if (Grid.ObjectLayers[(int)replacementCandidateLayer].ContainsKey(cell))
				{
					GameObject gameObject = Grid.ObjectLayers[(int)replacementCandidateLayer][cell];
					if (gameObject != null && gameObject.GetComponent<BuildingComplete>() != null)
					{
						return gameObject;
					}
				}
			}
		}
		else if (Grid.ObjectLayers[(int)TileLayer].ContainsKey(cell))
		{
			return Grid.ObjectLayers[(int)TileLayer][cell];
		}
		return null;
	}

	public GameObject Create(Vector3 pos, Storage resource_storage, IList<Tag> selected_elements, Recipe recipe, float temperature, GameObject obj)
	{
		SimUtil.DiseaseInfo a = SimUtil.DiseaseInfo.Invalid;
		if (resource_storage != null)
		{
			Recipe.Ingredient[] allIngredients = recipe.GetAllIngredients(selected_elements);
			if (allIngredients != null)
			{
				Recipe.Ingredient[] array = allIngredients;
				foreach (Recipe.Ingredient ingredient in array)
				{
					resource_storage.ConsumeAndGetDisease(ingredient.tag, ingredient.amount, out var _, out var disease_info, out var _);
					a = SimUtil.CalculateFinalDiseaseInfo(a, disease_info);
				}
			}
		}
		GameObject gameObject = GameUtil.KInstantiate(obj, pos, SceneLayer);
		Element element = ElementLoader.GetElement(selected_elements[0]);
		Debug.Assert(element != null);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.ElementID = element.id;
		component.Temperature = temperature;
		component.AddDisease(a.idx, a.count, "BuildingDef.Create");
		gameObject.name = obj.name;
		gameObject.SetActive(value: true);
		return gameObject;
	}

	public List<Tag> DefaultElements()
	{
		List<Tag> list = new List<Tag>();
		string[] materialCategory = MaterialCategory;
		foreach (string text in materialCategory)
		{
			foreach (Element element in ElementLoader.elements)
			{
				if (element.IsSolid && (element.tag.Name == text || element.HasTag(text)))
				{
					list.Add(element.tag);
					break;
				}
			}
		}
		return list;
	}

	public GameObject Build(int cell, Orientation orientation, Storage resource_storage, IList<Tag> selected_elements, float temperature, bool playsound = true, float timeBuilt = -1f)
	{
		Vector3 pos = Grid.CellToPosCBC(cell, SceneLayer);
		GameObject gameObject = Create(pos, resource_storage, selected_elements, CraftRecipe, temperature, BuildingComplete);
		Rotatable component = gameObject.GetComponent<Rotatable>();
		if (component != null)
		{
			component.SetOrientation(orientation);
		}
		MarkArea(cell, orientation, ObjectLayer, gameObject);
		if (IsTilePiece)
		{
			MarkArea(cell, orientation, TileLayer, gameObject);
			RunOnArea(cell, orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, TileLayer, ReplacementLayer);
			});
		}
		if (PlayConstructionSounds)
		{
			string sound = GlobalAssets.GetSound("Finish_Building_" + AudioSize);
			if (playsound && sound != null)
			{
				Vector3 position = gameObject.transform.GetPosition();
				position.z = 0f;
				KFMOD.PlayOneShot(sound, position);
			}
		}
		Deconstructable component2 = gameObject.GetComponent<Deconstructable>();
		if (component2 != null)
		{
			component2.constructionElements = new Tag[selected_elements.Count];
			for (int i = 0; i < selected_elements.Count; i++)
			{
				component2.constructionElements[i] = selected_elements[i];
			}
		}
		BuildingComplete component3 = gameObject.GetComponent<BuildingComplete>();
		if ((bool)component3)
		{
			component3.SetCreationTime(timeBuilt);
		}
		Game.Instance.Trigger(-1661515756, gameObject);
		gameObject.Trigger(-1661515756, gameObject);
		return gameObject;
	}

	public GameObject TryPlace(GameObject src_go, Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer = 0)
	{
		GameObject result = null;
		if (IsValidPlaceLocation(src_go, pos, orientation, replace_tile: false, out var _))
		{
			result = Instantiate(pos, orientation, selected_elements, layer);
		}
		return result;
	}

	public GameObject TryReplaceTile(GameObject src_go, Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer = 0)
	{
		GameObject result = null;
		if (IsValidPlaceLocation(src_go, pos, orientation, replace_tile: true, out var _))
		{
			Constructable component = BuildingUnderConstruction.GetComponent<Constructable>();
			component.IsReplacementTile = true;
			result = Instantiate(pos, orientation, selected_elements, layer);
			component.IsReplacementTile = false;
		}
		return result;
	}

	public GameObject Instantiate(Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer = 0)
	{
		float num = -0.15f;
		pos.z += num;
		GameObject gameObject = GameUtil.KInstantiate(BuildingUnderConstruction, pos, Grid.SceneLayer.Front, null, layer);
		Element element = ElementLoader.GetElement(selected_elements[0]);
		Debug.Assert(element != null, "Missing primary element for BuildingDef");
		gameObject.GetComponent<PrimaryElement>().ElementID = element.id;
		gameObject.GetComponent<Constructable>().SelectedElementsTags = selected_elements;
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private bool IsAreaClear(GameObject source_go, int cell, Orientation orientation, ObjectLayer layer, ObjectLayer tile_layer, bool replace_tile, out string fail_reason)
	{
		bool flag = true;
		fail_reason = null;
		for (int i = 0; i < PlacementOffsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(PlacementOffsets[i], orientation);
			if (!Grid.IsCellOffsetValid(cell, rotatedCellOffset))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				flag = false;
				break;
			}
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			if (Grid.WorldIdx[num] != ClusterManager.Instance.activeWorldId)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				return false;
			}
			if (!Grid.IsValidBuildingCell(num))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				flag = false;
				break;
			}
			if (Grid.Element[num].id == SimHashes.Unobtanium)
			{
				fail_reason = null;
				flag = false;
				break;
			}
			bool flag2 = BuildLocationRule == BuildLocationRule.LogicBridge || BuildLocationRule == BuildLocationRule.Conduit || BuildLocationRule == BuildLocationRule.WireBridge;
			if (!replace_tile && !flag2)
			{
				GameObject gameObject = Grid.Objects[num, (int)layer];
				if (gameObject != null && gameObject != source_go)
				{
					if (gameObject.GetComponent<Wire>() == null || BuildingComplete.GetComponent<Wire>() == null)
					{
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
						flag = false;
					}
					break;
				}
				if (tile_layer != ObjectLayer.NumLayers && Grid.Objects[num, (int)tile_layer] != null && Grid.Objects[num, (int)tile_layer].GetComponent<BuildingPreview>() == null)
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
					flag = false;
					break;
				}
			}
			if (layer == ObjectLayer.Building && AttachmentSlotTag != GameTags.Rocket && Grid.Objects[num, 39] != null)
			{
				if (BuildingComplete.GetComponent<Wire>() == null)
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
					flag = false;
				}
				break;
			}
			if (layer == ObjectLayer.Gantry)
			{
				bool flag3 = false;
				MakeBaseSolid.Def def = source_go.GetDef<MakeBaseSolid.Def>();
				for (int j = 0; j < def.solidOffsets.Length; j++)
				{
					CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(def.solidOffsets[j], orientation);
					flag3 |= rotatedCellOffset2 == rotatedCellOffset;
				}
				if (flag3 && !IsValidTileLocation(source_go, num, ref fail_reason))
				{
					flag = false;
					break;
				}
				GameObject gameObject2 = Grid.Objects[num, 1];
				if (gameObject2 != null && gameObject2.GetComponent<BuildingPreview>() == null)
				{
					Building component = gameObject2.GetComponent<Building>();
					if (flag3 || component == null || component.Def.AttachmentSlotTag != GameTags.Rocket)
					{
						fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED;
						flag = false;
						break;
					}
				}
			}
			if (BuildLocationRule == BuildLocationRule.Tile)
			{
				if (!IsValidTileLocation(source_go, num, ref fail_reason))
				{
					flag = false;
					break;
				}
			}
			else if (BuildLocationRule == BuildLocationRule.OnFloorOverSpace && World.Instance.zoneRenderData.GetSubWorldZoneType(num) != SubWorld.ZoneType.Space)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SPACE;
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		switch (BuildLocationRule)
		{
		case BuildLocationRule.WireBridge:
			return IsValidWireBridgeLocation(source_go, cell, orientation, out fail_reason);
		case BuildLocationRule.HighWattBridgeTile:
			flag = IsValidTileLocation(source_go, cell, ref fail_reason) && IsValidHighWattBridgeLocation(source_go, cell, orientation, out fail_reason);
			break;
		case BuildLocationRule.BuildingAttachPoint:
		{
			flag = false;
			for (int k = 0; k < Components.BuildingAttachPoints.Count; k++)
			{
				if (flag)
				{
					break;
				}
				for (int l = 0; l < Components.BuildingAttachPoints[k].points.Length; l++)
				{
					if (Components.BuildingAttachPoints[k].AcceptsAttachment(AttachmentSlotTag, Grid.OffsetCell(cell, attachablePosition)))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				fail_reason = string.Format(UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, AttachmentSlotTag);
			}
			break;
		}
		case BuildLocationRule.NotInTiles:
		{
			GameObject gameObject3 = Grid.Objects[cell, 9];
			if (gameObject3 != null && gameObject3 != source_go)
			{
				flag = false;
			}
			else if (Grid.HasDoor[cell])
			{
				flag = false;
			}
			else
			{
				GameObject gameObject4 = Grid.Objects[cell, (int)ObjectLayer];
				if (gameObject4 != null)
				{
					if (ReplacementLayer == ObjectLayer.NumLayers)
					{
						if (gameObject4 != source_go)
						{
							flag = false;
						}
					}
					else
					{
						Building component2 = gameObject4.GetComponent<Building>();
						if (component2 != null && component2.Def.ReplacementLayer != ReplacementLayer)
						{
							flag = false;
						}
					}
				}
			}
			if (!flag)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_NOT_IN_TILES;
			}
			break;
		}
		}
		return flag && ArePowerPortsInValidPositions(source_go, cell, orientation, out fail_reason) && AreConduitPortsInValidPositions(source_go, cell, orientation, out fail_reason) && AreLogicPortsInValidPositions(source_go, cell, out fail_reason);
	}

	private bool IsValidTileLocation(GameObject source_go, int cell, ref string fail_reason)
	{
		GameObject gameObject = Grid.Objects[cell, 27];
		if (gameObject != null && gameObject != source_go && gameObject.GetComponent<Building>().Def.BuildLocationRule == BuildLocationRule.NotInTiles)
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION;
			return false;
		}
		gameObject = Grid.Objects[cell, 29];
		if (gameObject != null && gameObject != source_go && gameObject.GetComponent<Building>().Def.BuildLocationRule == BuildLocationRule.HighWattBridgeTile)
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION;
			return false;
		}
		gameObject = Grid.Objects[cell, 2];
		if (gameObject != null && gameObject != source_go)
		{
			Building component = gameObject.GetComponent<Building>();
			if (component != null && component.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_BACK_WALL;
				return false;
			}
		}
		return true;
	}

	public void RunOnArea(int cell, Orientation orientation, Action<int> callback)
	{
		for (int i = 0; i < PlacementOffsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(PlacementOffsets[i], orientation);
			int obj = Grid.OffsetCell(cell, rotatedCellOffset);
			callback(obj);
		}
	}

	public void MarkArea(int cell, Orientation orientation, ObjectLayer layer, GameObject go)
	{
		if (BuildLocationRule != BuildLocationRule.Conduit && BuildLocationRule != BuildLocationRule.WireBridge && BuildLocationRule != BuildLocationRule.LogicBridge)
		{
			for (int i = 0; i < PlacementOffsets.Length; i++)
			{
				CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(PlacementOffsets[i], orientation);
				int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
				Grid.Objects[cell2, (int)layer] = go;
			}
		}
		if (InputConduitType != 0)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(UtilityInputOffset, orientation);
			int cell3 = Grid.OffsetCell(cell, rotatedCellOffset2);
			ObjectLayer objectLayerForConduitType = Grid.GetObjectLayerForConduitType(InputConduitType);
			MarkOverlappingPorts(Grid.Objects[cell3, (int)objectLayerForConduitType], go);
			Grid.Objects[cell3, (int)objectLayerForConduitType] = go;
		}
		if (OutputConduitType != 0)
		{
			CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(UtilityOutputOffset, orientation);
			int cell4 = Grid.OffsetCell(cell, rotatedCellOffset3);
			ObjectLayer objectLayerForConduitType2 = Grid.GetObjectLayerForConduitType(OutputConduitType);
			MarkOverlappingPorts(Grid.Objects[cell4, (int)objectLayerForConduitType2], go);
			Grid.Objects[cell4, (int)objectLayerForConduitType2] = go;
		}
		if (RequiresPowerInput)
		{
			CellOffset rotatedCellOffset4 = Rotatable.GetRotatedCellOffset(PowerInputOffset, orientation);
			int cell5 = Grid.OffsetCell(cell, rotatedCellOffset4);
			MarkOverlappingPorts(Grid.Objects[cell5, 29], go);
			Grid.Objects[cell5, 29] = go;
		}
		if (RequiresPowerOutput)
		{
			CellOffset rotatedCellOffset5 = Rotatable.GetRotatedCellOffset(PowerOutputOffset, orientation);
			int cell6 = Grid.OffsetCell(cell, rotatedCellOffset5);
			MarkOverlappingPorts(Grid.Objects[cell6, 29], go);
			Grid.Objects[cell6, 29] = go;
		}
		if (BuildLocationRule == BuildLocationRule.WireBridge || BuildLocationRule == BuildLocationRule.HighWattBridgeTile)
		{
			go.GetComponent<UtilityNetworkLink>().GetCells(cell, orientation, out var linked_cell, out var linked_cell2);
			MarkOverlappingPorts(Grid.Objects[linked_cell, 29], go);
			MarkOverlappingPorts(Grid.Objects[linked_cell2, 29], go);
			Grid.Objects[linked_cell, 29] = go;
			Grid.Objects[linked_cell2, 29] = go;
		}
		if (BuildLocationRule == BuildLocationRule.LogicBridge)
		{
			LogicPorts component = go.GetComponent<LogicPorts>();
			if (component != null && component.inputPortInfo != null)
			{
				LogicPorts.Port[] inputPortInfo = component.inputPortInfo;
				for (int j = 0; j < inputPortInfo.Length; j++)
				{
					CellOffset rotatedCellOffset6 = Rotatable.GetRotatedCellOffset(inputPortInfo[j].cellOffset, orientation);
					int cell7 = Grid.OffsetCell(cell, rotatedCellOffset6);
					MarkOverlappingPorts(Grid.Objects[cell7, (int)layer], go);
					Grid.Objects[cell7, (int)layer] = go;
				}
			}
		}
		ISecondaryInput[] components = BuildingComplete.GetComponents<ISecondaryInput>();
		if (components != null)
		{
			ISecondaryInput[] array = components;
			foreach (ISecondaryInput secondaryInput in array)
			{
				for (int k = 0; k < 4; k++)
				{
					ConduitType conduitType = (ConduitType)k;
					if (conduitType != 0 && secondaryInput.HasSecondaryConduitType(conduitType))
					{
						ObjectLayer objectLayerForConduitType3 = Grid.GetObjectLayerForConduitType(conduitType);
						CellOffset rotatedCellOffset7 = Rotatable.GetRotatedCellOffset(secondaryInput.GetSecondaryConduitOffset(conduitType), orientation);
						int cell8 = Grid.OffsetCell(cell, rotatedCellOffset7);
						MarkOverlappingPorts(Grid.Objects[cell8, (int)objectLayerForConduitType3], go);
						Grid.Objects[cell8, (int)objectLayerForConduitType3] = go;
					}
				}
			}
		}
		ISecondaryOutput[] components2 = BuildingComplete.GetComponents<ISecondaryOutput>();
		if (components2 == null)
		{
			return;
		}
		ISecondaryOutput[] array2 = components2;
		foreach (ISecondaryOutput secondaryOutput in array2)
		{
			for (int l = 0; l < 4; l++)
			{
				ConduitType conduitType2 = (ConduitType)l;
				if (conduitType2 != 0 && secondaryOutput.HasSecondaryConduitType(conduitType2))
				{
					ObjectLayer objectLayerForConduitType4 = Grid.GetObjectLayerForConduitType(conduitType2);
					CellOffset rotatedCellOffset8 = Rotatable.GetRotatedCellOffset(secondaryOutput.GetSecondaryConduitOffset(conduitType2), orientation);
					int cell9 = Grid.OffsetCell(cell, rotatedCellOffset8);
					MarkOverlappingPorts(Grid.Objects[cell9, (int)objectLayerForConduitType4], go);
					Grid.Objects[cell9, (int)objectLayerForConduitType4] = go;
				}
			}
		}
	}

	public void MarkOverlappingPorts(GameObject existing, GameObject replaced)
	{
		if (existing == null)
		{
			if (replaced != null)
			{
				replaced.RemoveTag(GameTags.HasInvalidPorts);
			}
		}
		else if (existing != replaced)
		{
			existing.AddTag(GameTags.HasInvalidPorts);
		}
	}

	public void UnmarkArea(int cell, Orientation orientation, ObjectLayer layer, GameObject go)
	{
		if (cell == Grid.InvalidCell)
		{
			return;
		}
		for (int i = 0; i < PlacementOffsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(PlacementOffsets[i], orientation);
			int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
			if (Grid.Objects[cell2, (int)layer] == go)
			{
				Grid.Objects[cell2, (int)layer] = null;
			}
		}
		if (InputConduitType != 0)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(UtilityInputOffset, orientation);
			int cell3 = Grid.OffsetCell(cell, rotatedCellOffset2);
			ObjectLayer objectLayerForConduitType = Grid.GetObjectLayerForConduitType(InputConduitType);
			if (Grid.Objects[cell3, (int)objectLayerForConduitType] == go)
			{
				Grid.Objects[cell3, (int)objectLayerForConduitType] = null;
			}
		}
		if (OutputConduitType != 0)
		{
			CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(UtilityOutputOffset, orientation);
			int cell4 = Grid.OffsetCell(cell, rotatedCellOffset3);
			ObjectLayer objectLayerForConduitType2 = Grid.GetObjectLayerForConduitType(OutputConduitType);
			if (Grid.Objects[cell4, (int)objectLayerForConduitType2] == go)
			{
				Grid.Objects[cell4, (int)objectLayerForConduitType2] = null;
			}
		}
		if (RequiresPowerInput)
		{
			CellOffset rotatedCellOffset4 = Rotatable.GetRotatedCellOffset(PowerInputOffset, orientation);
			int cell5 = Grid.OffsetCell(cell, rotatedCellOffset4);
			if (Grid.Objects[cell5, 29] == go)
			{
				Grid.Objects[cell5, 29] = null;
			}
		}
		if (RequiresPowerOutput)
		{
			CellOffset rotatedCellOffset5 = Rotatable.GetRotatedCellOffset(PowerOutputOffset, orientation);
			int cell6 = Grid.OffsetCell(cell, rotatedCellOffset5);
			if (Grid.Objects[cell6, 29] == go)
			{
				Grid.Objects[cell6, 29] = null;
			}
		}
		if (BuildLocationRule == BuildLocationRule.HighWattBridgeTile)
		{
			go.GetComponent<UtilityNetworkLink>().GetCells(cell, orientation, out var linked_cell, out var linked_cell2);
			if (Grid.Objects[linked_cell, 29] == go)
			{
				Grid.Objects[linked_cell, 29] = null;
			}
			if (Grid.Objects[linked_cell2, 29] == go)
			{
				Grid.Objects[linked_cell2, 29] = null;
			}
		}
		ISecondaryInput[] components = BuildingComplete.GetComponents<ISecondaryInput>();
		if (components != null)
		{
			ISecondaryInput[] array = components;
			foreach (ISecondaryInput secondaryInput in array)
			{
				for (int k = 0; k < 4; k++)
				{
					ConduitType conduitType = (ConduitType)k;
					if (conduitType != 0 && secondaryInput.HasSecondaryConduitType(conduitType))
					{
						ObjectLayer objectLayerForConduitType3 = Grid.GetObjectLayerForConduitType(conduitType);
						CellOffset rotatedCellOffset6 = Rotatable.GetRotatedCellOffset(secondaryInput.GetSecondaryConduitOffset(conduitType), orientation);
						int cell7 = Grid.OffsetCell(cell, rotatedCellOffset6);
						if (Grid.Objects[cell7, (int)objectLayerForConduitType3] == go)
						{
							Grid.Objects[cell7, (int)objectLayerForConduitType3] = null;
						}
					}
				}
			}
		}
		ISecondaryOutput[] components2 = BuildingComplete.GetComponents<ISecondaryOutput>();
		if (components2 == null)
		{
			return;
		}
		ISecondaryOutput[] array2 = components2;
		foreach (ISecondaryOutput secondaryOutput in array2)
		{
			for (int l = 0; l < 4; l++)
			{
				ConduitType conduitType2 = (ConduitType)l;
				if (conduitType2 != 0 && secondaryOutput.HasSecondaryConduitType(conduitType2))
				{
					ObjectLayer objectLayerForConduitType4 = Grid.GetObjectLayerForConduitType(conduitType2);
					CellOffset rotatedCellOffset7 = Rotatable.GetRotatedCellOffset(secondaryOutput.GetSecondaryConduitOffset(conduitType2), orientation);
					int cell8 = Grid.OffsetCell(cell, rotatedCellOffset7);
					if (Grid.Objects[cell8, (int)objectLayerForConduitType4] == go)
					{
						Grid.Objects[cell8, (int)objectLayerForConduitType4] = null;
					}
				}
			}
		}
	}

	public int GetBuildingCell(int cell)
	{
		return cell + (WidthInCells - 1) / 2;
	}

	public Vector3 GetVisualizerOffset()
	{
		return Vector3.right * (0.5f * (float)((WidthInCells + 1) % 2));
	}

	public bool IsValidPlaceLocation(GameObject source_go, Vector3 pos, Orientation orientation, out string fail_reason)
	{
		int cell = Grid.PosToCell(pos);
		return IsValidPlaceLocation(source_go, cell, orientation, replace_tile: false, out fail_reason);
	}

	public bool IsValidPlaceLocation(GameObject source_go, Vector3 pos, Orientation orientation, bool replace_tile, out string fail_reason)
	{
		int cell = Grid.PosToCell(pos);
		return IsValidPlaceLocation(source_go, cell, orientation, replace_tile, out fail_reason);
	}

	public bool IsValidPlaceLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		return IsValidPlaceLocation(source_go, cell, orientation, replace_tile: false, out fail_reason);
	}

	public bool IsValidPlaceLocation(GameObject source_go, int cell, Orientation orientation, bool replace_tile, out string fail_reason)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
			return false;
		}
		if (Grid.WorldIdx[cell] != ClusterManager.Instance.activeWorldId)
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
			return false;
		}
		if (BuildLocationRule == BuildLocationRule.OnRocketEnvelope)
		{
			if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells, GameTags.RocketEnvelopeTile))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_ONROCKETENVELOPE;
				return false;
			}
		}
		else if (BuildLocationRule == BuildLocationRule.OnWall)
		{
			if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WALL;
				return false;
			}
		}
		else if (BuildLocationRule == BuildLocationRule.InCorner)
		{
			if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER;
				return false;
			}
		}
		else if (BuildLocationRule == BuildLocationRule.WallFloor)
		{
			if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER_FLOOR;
				return false;
			}
		}
		else if (BuildLocationRule == BuildLocationRule.BelowRocketCeiling)
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(Grid.WorldIdx[cell]);
			if ((float)(Grid.CellToXY(cell).y + 35 + source_go.GetComponent<Building>().Def.HeightInCells) >= world.maximumBounds.y - (float)Grid.TopBorderHeight)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_BELOWROCKETCEILING;
				return false;
			}
		}
		return IsAreaClear(source_go, cell, orientation, ObjectLayer, TileLayer, replace_tile, out fail_reason);
	}

	public bool IsValidReplaceLocation(Vector3 pos, Orientation orientation, ObjectLayer replace_layer, ObjectLayer obj_layer)
	{
		if (replace_layer == ObjectLayer.NumLayers)
		{
			return false;
		}
		bool result = true;
		int cell = Grid.PosToCell(pos);
		for (int i = 0; i < PlacementOffsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(PlacementOffsets[i], orientation);
			int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
			if (!Grid.IsValidBuildingCell(cell2))
			{
				return false;
			}
			if (Grid.Objects[cell2, (int)obj_layer] == null || Grid.Objects[cell2, (int)replace_layer] != null)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public bool IsValidBuildLocation(GameObject source_go, Vector3 pos, Orientation orientation)
	{
		string reason = "";
		return IsValidBuildLocation(source_go, pos, orientation, out reason);
	}

	public bool IsValidBuildLocation(GameObject source_go, Vector3 pos, Orientation orientation, out string reason)
	{
		int cell = Grid.PosToCell(pos);
		return IsValidBuildLocation(source_go, cell, orientation, out reason);
	}

	public bool IsValidBuildLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
			return false;
		}
		if (!IsAreaValid(cell, orientation, out fail_reason))
		{
			return false;
		}
		bool flag = true;
		fail_reason = null;
		switch (BuildLocationRule)
		{
		case BuildLocationRule.OnFloor:
		case BuildLocationRule.OnCeiling:
		case BuildLocationRule.OnFoundationRotatable:
			if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR;
			}
			break;
		case BuildLocationRule.OnWall:
			if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WALL;
			}
			break;
		case BuildLocationRule.InCorner:
			if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER;
			}
			break;
		case BuildLocationRule.WallFloor:
			if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_CORNER_FLOOR;
			}
			break;
		case BuildLocationRule.OnFloorOverSpace:
			if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_FLOOR;
			}
			else if (!AreAllCellsValid(cell, orientation, WidthInCells, HeightInCells, (int check_cell) => World.Instance.zoneRenderData.GetSubWorldZoneType(check_cell) == SubWorld.ZoneType.Space))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SPACE;
			}
			break;
		case BuildLocationRule.OnRocketEnvelope:
			if (!CheckFoundation(cell, orientation, BuildLocationRule, WidthInCells, HeightInCells, GameTags.RocketEnvelopeTile))
			{
				flag = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_ONROCKETENVELOPE;
			}
			break;
		case BuildLocationRule.NotInTiles:
		{
			GameObject gameObject2 = Grid.Objects[cell, 9];
			flag = (gameObject2 == null || gameObject2 == source_go) && !Grid.HasDoor[cell];
			if (flag)
			{
				GameObject gameObject3 = Grid.Objects[cell, (int)ObjectLayer];
				if (gameObject3 != null)
				{
					if (ReplacementLayer == ObjectLayer.NumLayers)
					{
						flag = flag && (gameObject3 == null || gameObject3 == source_go);
					}
					else
					{
						Building component3 = gameObject3.GetComponent<Building>();
						flag = component3 == null || component3.Def.ReplacementLayer == ReplacementLayer;
					}
				}
			}
			fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_NOT_IN_TILES;
			break;
		}
		case BuildLocationRule.Tile:
		{
			flag = true;
			GameObject gameObject = Grid.Objects[cell, 27];
			if (gameObject != null)
			{
				Building component = gameObject.GetComponent<Building>();
				if (component != null && component.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
				{
					flag = false;
				}
			}
			gameObject = Grid.Objects[cell, 2];
			if (gameObject != null)
			{
				Building component2 = gameObject.GetComponent<Building>();
				if (component2 != null && component2.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
				{
					flag = false;
				}
			}
			break;
		}
		case BuildLocationRule.BuildingAttachPoint:
		{
			flag = false;
			for (int i = 0; i < Components.BuildingAttachPoints.Count; i++)
			{
				if (flag)
				{
					break;
				}
				for (int j = 0; j < Components.BuildingAttachPoints[i].points.Length; j++)
				{
					if (Components.BuildingAttachPoints[i].AcceptsAttachment(AttachmentSlotTag, Grid.OffsetCell(cell, attachablePosition)))
					{
						flag = true;
						break;
					}
				}
			}
			fail_reason = string.Format(UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, AttachmentSlotTag);
			break;
		}
		case BuildLocationRule.Anywhere:
		case BuildLocationRule.Conduit:
		case BuildLocationRule.OnFloorOrBuildingAttachPoint:
			flag = true;
			break;
		}
		return flag && ArePowerPortsInValidPositions(source_go, cell, orientation, out fail_reason) && AreConduitPortsInValidPositions(source_go, cell, orientation, out fail_reason);
	}

	private bool IsAreaValid(int cell, Orientation orientation, out string fail_reason)
	{
		bool result = true;
		fail_reason = null;
		for (int i = 0; i < PlacementOffsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(PlacementOffsets[i], orientation);
			if (!Grid.IsCellOffsetValid(cell, rotatedCellOffset))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				result = false;
				break;
			}
			int num = Grid.OffsetCell(cell, rotatedCellOffset);
			if (!Grid.IsValidBuildingCell(num))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				result = false;
				break;
			}
			if (Grid.Element[num].id == SimHashes.Unobtanium)
			{
				fail_reason = null;
				result = false;
				break;
			}
		}
		return result;
	}

	private bool ArePowerPortsInValidPositions(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		fail_reason = null;
		if (source_go == null)
		{
			return true;
		}
		if (RequiresPowerInput)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(PowerInputOffset, orientation);
			int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
			GameObject gameObject = Grid.Objects[cell2, 29];
			if (gameObject != null && gameObject != source_go)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
				return false;
			}
		}
		if (RequiresPowerOutput)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(PowerOutputOffset, orientation);
			int cell3 = Grid.OffsetCell(cell, rotatedCellOffset2);
			GameObject gameObject2 = Grid.Objects[cell3, 29];
			if (gameObject2 != null && gameObject2 != source_go)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
				return false;
			}
		}
		return true;
	}

	private bool AreConduitPortsInValidPositions(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		fail_reason = null;
		if (source_go == null)
		{
			return true;
		}
		bool flag = true;
		if (InputConduitType != 0)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(UtilityInputOffset, orientation);
			int utility_cell = Grid.OffsetCell(cell, rotatedCellOffset);
			flag = IsValidConduitConnection(source_go, InputConduitType, utility_cell, ref fail_reason);
		}
		if (flag && OutputConduitType != 0)
		{
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(UtilityOutputOffset, orientation);
			int utility_cell2 = Grid.OffsetCell(cell, rotatedCellOffset2);
			flag = IsValidConduitConnection(source_go, OutputConduitType, utility_cell2, ref fail_reason);
		}
		Building component = source_go.GetComponent<Building>();
		if (flag && (bool)component)
		{
			ISecondaryInput[] components = component.Def.BuildingComplete.GetComponents<ISecondaryInput>();
			if (components != null)
			{
				ISecondaryInput[] array = components;
				foreach (ISecondaryInput secondaryInput in array)
				{
					for (int j = 0; j < 4; j++)
					{
						ConduitType conduitType = (ConduitType)j;
						if (conduitType != 0 && secondaryInput.HasSecondaryConduitType(conduitType))
						{
							CellOffset rotatedCellOffset3 = Rotatable.GetRotatedCellOffset(secondaryInput.GetSecondaryConduitOffset(conduitType), orientation);
							int utility_cell3 = Grid.OffsetCell(cell, rotatedCellOffset3);
							flag = IsValidConduitConnection(source_go, conduitType, utility_cell3, ref fail_reason);
						}
					}
				}
			}
		}
		if (flag)
		{
			ISecondaryOutput[] components2 = component.Def.BuildingComplete.GetComponents<ISecondaryOutput>();
			if (components2 != null)
			{
				ISecondaryOutput[] array2 = components2;
				foreach (ISecondaryOutput secondaryOutput in array2)
				{
					for (int k = 0; k < 4; k++)
					{
						ConduitType conduitType2 = (ConduitType)k;
						if (conduitType2 != 0 && secondaryOutput.HasSecondaryConduitType(conduitType2))
						{
							CellOffset rotatedCellOffset4 = Rotatable.GetRotatedCellOffset(secondaryOutput.GetSecondaryConduitOffset(conduitType2), orientation);
							int utility_cell4 = Grid.OffsetCell(cell, rotatedCellOffset4);
							flag = IsValidConduitConnection(source_go, conduitType2, utility_cell4, ref fail_reason);
						}
					}
				}
			}
		}
		return flag;
	}

	private bool IsValidWireBridgeLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		UtilityNetworkLink component = source_go.GetComponent<UtilityNetworkLink>();
		if (component != null)
		{
			component.GetCells(out var linked_cell, out var linked_cell2);
			if (Grid.Objects[linked_cell, 29] != null || Grid.Objects[linked_cell2, 29] != null)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
				return false;
			}
		}
		fail_reason = null;
		return true;
	}

	private bool IsValidHighWattBridgeLocation(GameObject source_go, int cell, Orientation orientation, out string fail_reason)
	{
		UtilityNetworkLink component = source_go.GetComponent<UtilityNetworkLink>();
		if (component != null)
		{
			if (!component.AreCellsValid(cell, orientation))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_INVALID_CELL;
				return false;
			}
			component.GetCells(out var linked_cell, out var linked_cell2);
			if (Grid.Objects[linked_cell, 29] != null || Grid.Objects[linked_cell2, 29] != null)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP;
				return false;
			}
			if (Grid.Objects[linked_cell, 9] != null || Grid.Objects[linked_cell2, 9] != null)
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_HIGHWATT_NOT_IN_TILE;
				return false;
			}
			if (Grid.HasDoor[linked_cell] || Grid.HasDoor[linked_cell2])
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_HIGHWATT_NOT_IN_TILE;
				return false;
			}
			GameObject gameObject = Grid.Objects[linked_cell, 1];
			GameObject gameObject2 = Grid.Objects[linked_cell2, 1];
			if (gameObject != null || gameObject2 != null)
			{
				BuildingUnderConstruction buildingUnderConstruction = (gameObject ? gameObject.GetComponent<BuildingUnderConstruction>() : null);
				BuildingUnderConstruction buildingUnderConstruction2 = (gameObject2 ? gameObject2.GetComponent<BuildingUnderConstruction>() : null);
				if (((bool)buildingUnderConstruction && (bool)buildingUnderConstruction.Def.BuildingComplete.GetComponent<Door>()) || ((bool)buildingUnderConstruction2 && (bool)buildingUnderConstruction2.Def.BuildingComplete.GetComponent<Door>()))
				{
					fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_HIGHWATT_NOT_IN_TILE;
					return false;
				}
			}
		}
		fail_reason = null;
		return true;
	}

	private bool AreLogicPortsInValidPositions(GameObject source_go, int cell, out string fail_reason)
	{
		fail_reason = null;
		if (source_go == null)
		{
			return true;
		}
		ReadOnlyCollection<ILogicUIElement> visElements = Game.Instance.logicCircuitManager.GetVisElements();
		LogicPorts component = source_go.GetComponent<LogicPorts>();
		if (component != null)
		{
			component.HackRefreshVisualizers();
			if (DoLogicPortsConflict(component.inputPorts, visElements) || DoLogicPortsConflict(component.outputPorts, visElements))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LOGIC_PORTS_OBSTRUCTED;
				return false;
			}
		}
		else
		{
			LogicGateBase component2 = source_go.GetComponent<LogicGateBase>();
			if (component2 != null && (IsLogicPortObstructed(component2.InputCellOne, visElements) || IsLogicPortObstructed(component2.OutputCellOne, visElements) || ((component2.RequiresTwoInputs || component2.RequiresFourInputs) && IsLogicPortObstructed(component2.InputCellTwo, visElements)) || (component2.RequiresFourInputs && (IsLogicPortObstructed(component2.InputCellThree, visElements) || IsLogicPortObstructed(component2.InputCellFour, visElements))) || (component2.RequiresFourOutputs && (IsLogicPortObstructed(component2.OutputCellTwo, visElements) || IsLogicPortObstructed(component2.OutputCellThree, visElements) || IsLogicPortObstructed(component2.OutputCellFour, visElements)))))
			{
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LOGIC_PORTS_OBSTRUCTED;
				return false;
			}
		}
		return true;
	}

	private bool DoLogicPortsConflict(IList<ILogicUIElement> ports_a, IList<ILogicUIElement> ports_b)
	{
		if (ports_a == null || ports_b == null)
		{
			return false;
		}
		foreach (ILogicUIElement item in ports_a)
		{
			int logicUICell = item.GetLogicUICell();
			foreach (ILogicUIElement item2 in ports_b)
			{
				if (item != item2 && logicUICell == item2.GetLogicUICell())
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsLogicPortObstructed(int cell, IList<ILogicUIElement> ports)
	{
		int num = 0;
		foreach (ILogicUIElement port in ports)
		{
			if (port.GetLogicUICell() == cell)
			{
				num++;
			}
		}
		return num > 0;
	}

	private bool IsValidConduitConnection(GameObject source_go, ConduitType conduit_type, int utility_cell, ref string fail_reason)
	{
		bool result = true;
		switch (conduit_type)
		{
		case ConduitType.Gas:
		{
			GameObject gameObject3 = Grid.Objects[utility_cell, 15];
			if (gameObject3 != null && gameObject3 != source_go)
			{
				result = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_GASPORTS_OVERLAP;
			}
			break;
		}
		case ConduitType.Liquid:
		{
			GameObject gameObject2 = Grid.Objects[utility_cell, 19];
			if (gameObject2 != null && gameObject2 != source_go)
			{
				result = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_LIQUIDPORTS_OVERLAP;
			}
			break;
		}
		case ConduitType.Solid:
		{
			GameObject gameObject = Grid.Objects[utility_cell, 23];
			if (gameObject != null && gameObject != source_go)
			{
				result = false;
				fail_reason = UI.TOOLTIPS.HELP_BUILDLOCATION_SOLIDPORTS_OVERLAP;
			}
			break;
		}
		}
		return result;
	}

	public static int GetXOffset(int width)
	{
		return -(width - 1) / 2;
	}

	public static bool CheckFoundation(int cell, Orientation orientation, BuildLocationRule location_rule, int width, int height, Tag optionalFoundationRequiredTag = default(Tag))
	{
		switch (location_rule)
		{
		case BuildLocationRule.OnWall:
			return CheckWallFoundation(cell, width, height, orientation != Orientation.FlipH);
		case BuildLocationRule.InCorner:
			if (CheckBaseFoundation(cell, orientation, BuildLocationRule.OnCeiling, width, height, optionalFoundationRequiredTag))
			{
				return CheckWallFoundation(cell, width, height, orientation != Orientation.FlipH);
			}
			return false;
		case BuildLocationRule.WallFloor:
			if (CheckBaseFoundation(cell, orientation, BuildLocationRule.OnFloor, width, height, optionalFoundationRequiredTag))
			{
				return CheckWallFoundation(cell, width, height, orientation != Orientation.FlipH);
			}
			return false;
		default:
			return CheckBaseFoundation(cell, orientation, location_rule, width, height, optionalFoundationRequiredTag);
		}
	}

	public static bool CheckBaseFoundation(int cell, Orientation orientation, BuildLocationRule location_rule, int width, int height, Tag optionalFoundationRequiredTag = default(Tag))
	{
		int num = -(width - 1) / 2;
		int num2 = width / 2;
		for (int i = num; i <= num2; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset((location_rule == BuildLocationRule.OnCeiling) ? new CellOffset(i, height) : new CellOffset(i, -1), orientation);
			int num3 = Grid.OffsetCell(cell, rotatedCellOffset);
			if (!Grid.IsValidBuildingCell(num3) || !Grid.Solid[num3])
			{
				return false;
			}
			if (optionalFoundationRequiredTag.IsValid && (!Grid.ObjectLayers[9].ContainsKey(num3) || !Grid.ObjectLayers[9][num3].HasTag(optionalFoundationRequiredTag)))
			{
				return false;
			}
		}
		return true;
	}

	public static bool CheckWallFoundation(int cell, int width, int height, bool leftWall)
	{
		for (int i = 0; i < height; i++)
		{
			CellOffset offset = new CellOffset(leftWall ? (-(width - 1) / 2 - 1) : (width / 2 + 1), i);
			int num = Grid.OffsetCell(cell, offset);
			GameObject gameObject = Grid.Objects[num, 1];
			bool flag = false;
			if (gameObject != null)
			{
				BuildingUnderConstruction component = gameObject.GetComponent<BuildingUnderConstruction>();
				if (component != null && component.Def.IsFoundation)
				{
					flag = true;
				}
			}
			if (!Grid.IsValidBuildingCell(num) || !(Grid.Solid[num] || flag))
			{
				return false;
			}
		}
		return true;
	}

	public static bool AreAllCellsValid(int base_cell, Orientation orientation, int width, int height, Func<int, bool> valid_cell_check)
	{
		int num = -(width - 1) / 2;
		int num2 = width / 2;
		if (orientation == Orientation.FlipH)
		{
			int num3 = num;
			num = -num2;
			num2 = -num3;
		}
		for (int i = 0; i < height; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				int arg = Grid.OffsetCell(base_cell, j, i);
				if (!valid_cell_check(arg))
				{
					return false;
				}
			}
		}
		return true;
	}

	public Sprite GetUISprite(string animName = "ui", bool centered = false)
	{
		return Def.GetUISpriteFromMultiObjectAnim(AnimFiles[0], animName, centered);
	}

	public void GenerateOffsets()
	{
		GenerateOffsets(WidthInCells, HeightInCells);
	}

	public void GenerateOffsets(int width, int height)
	{
		if (placementOffsetsCache.TryGetValue(new CellOffset(width, height), out PlacementOffsets))
		{
			return;
		}
		int num = width / 2 - width + 1;
		PlacementOffsets = new CellOffset[width * height];
		for (int i = 0; i != height; i++)
		{
			int num2 = i * width;
			for (int j = 0; j != width; j++)
			{
				int num3 = num2 + j;
				PlacementOffsets[num3].x = j + num;
				PlacementOffsets[num3].y = i;
			}
		}
		placementOffsetsCache.Add(new CellOffset(width, height), PlacementOffsets);
	}

	public void PostProcess()
	{
		CraftRecipe = new Recipe(BuildingComplete.PrefabID().Name, 1f, (SimHashes)0, Name);
		CraftRecipe.Icon = UISprite;
		for (int i = 0; i < MaterialCategory.Length; i++)
		{
			Recipe.Ingredient item = new Recipe.Ingredient(MaterialCategory[i], (int)Mass[i]);
			CraftRecipe.Ingredients.Add(item);
		}
		if (DecorBlockTileInfo != null)
		{
			DecorBlockTileInfo.PostProcess();
		}
		if (DecorPlaceBlockTileInfo != null)
		{
			DecorPlaceBlockTileInfo.PostProcess();
		}
		if (!Deprecated)
		{
			Db.Get().TechItems.AddTechItem(PrefabID, Name, Effect, GetUISprite, RequiredDlcIds);
		}
	}

	public bool MaterialsAvailable(IList<Tag> selected_elements, WorldContainer world)
	{
		bool result = true;
		Recipe.Ingredient[] allIngredients = CraftRecipe.GetAllIngredients(selected_elements);
		foreach (Recipe.Ingredient ingredient in allIngredients)
		{
			if (world.worldInventory.GetAmount(ingredient.tag, includeRelatedWorlds: true) < ingredient.amount)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public bool CheckRequiresBuildingCellVisualizer()
	{
		if (!CheckRequiresPowerInput() && !CheckRequiresPowerOutput() && !CheckRequiresGasInput() && !CheckRequiresGasOutput() && !CheckRequiresLiquidInput() && !CheckRequiresLiquidOutput() && !CheckRequiresSolidInput() && !CheckRequiresSolidOutput() && !CheckRequiresHighEnergyParticleInput() && !CheckRequiresHighEnergyParticleOutput())
		{
			return DiseaseCellVisName != null;
		}
		return true;
	}

	public bool CheckRequiresPowerInput()
	{
		return RequiresPowerInput;
	}

	public bool CheckRequiresPowerOutput()
	{
		return RequiresPowerOutput;
	}

	public bool CheckRequiresGasInput()
	{
		return InputConduitType == ConduitType.Gas;
	}

	public bool CheckRequiresGasOutput()
	{
		return OutputConduitType == ConduitType.Gas;
	}

	public bool CheckRequiresLiquidInput()
	{
		return InputConduitType == ConduitType.Liquid;
	}

	public bool CheckRequiresLiquidOutput()
	{
		return OutputConduitType == ConduitType.Liquid;
	}

	public bool CheckRequiresSolidInput()
	{
		return InputConduitType == ConduitType.Solid;
	}

	public bool CheckRequiresSolidOutput()
	{
		return OutputConduitType == ConduitType.Solid;
	}

	public bool CheckRequiresHighEnergyParticleInput()
	{
		return UseHighEnergyParticleInputPort;
	}

	public bool CheckRequiresHighEnergyParticleOutput()
	{
		return UseHighEnergyParticleOutputPort;
	}
}
