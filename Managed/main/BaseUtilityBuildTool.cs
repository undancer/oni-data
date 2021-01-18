using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class BaseUtilityBuildTool : DragTool
{
	protected struct PathNode
	{
		public int cell;

		public bool valid;

		public GameObject visualizer;

		public void Play(string anim)
		{
			visualizer.GetComponent<KBatchedAnimController>().Play(anim);
		}
	}

	private IList<Tag> selectedElements;

	private BuildingDef def;

	protected List<PathNode> path = new List<PathNode>();

	protected IUtilityNetworkMgr conduitMgr;

	private Coroutine visUpdater;

	private int buildingCount;

	private int lastCell = -1;

	private BuildingCellVisualizer previousCellConnection;

	private int previousCell;

	protected override void OnPrefabInit()
	{
		buildingCount = Random.Range(1, 14);
		canChangeDragAxis = false;
	}

	private void Play(GameObject go, string anim)
	{
		go.GetComponent<KBatchedAnimController>().Play(anim);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		Vector3 cursorPos = PlayerController.GetCursorPos(KInputManager.GetMousePos());
		visualizer = GameUtil.KInstantiate(def.BuildingPreview, cursorPos, Grid.SceneLayer.Ore, null, LayerMask.NameToLayer("Place"));
		KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
			component.SetDirty();
		}
		visualizer.SetActive(value: true);
		Play(visualizer, "None_Place");
		GetComponent<BuildToolHoverTextCard>().currentDef = def;
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(visualizer);
		IHaveUtilityNetworkMgr component2 = def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>();
		conduitMgr = component2.GetNetworkManager();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		StopVisUpdater();
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
		if (visualizer != null)
		{
			Object.Destroy(visualizer);
		}
		base.OnDeactivateTool(new_tool);
	}

	public void Activate(BuildingDef def, IList<Tag> selected_elements)
	{
		selectedElements = selected_elements;
		this.def = def;
		viewMode = def.ViewMode;
		PlayerController.Instance.ActivateTool(this);
		ResourceRemainingDisplayScreen.instance.SetResources(selected_elements, def.CraftRecipe);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (path.Count == 0 || path[path.Count - 1].cell == cell)
		{
			return;
		}
		placeSound = GlobalAssets.GetSound("Place_building_" + def.AudioSize);
		Vector3 pos = Grid.CellToPos(cell);
		EventInstance instance = SoundEvent.BeginOneShot(placeSound, pos);
		if (path.Count > 1 && cell == path[path.Count - 2].cell)
		{
			if (previousCellConnection != null)
			{
				previousCellConnection.ConnectedEvent(previousCell);
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("OutletDisconnected"));
				previousCellConnection = null;
			}
			previousCell = cell;
			CheckForConnection(cell, def.PrefabID, "", ref previousCellConnection, fireEvents: false);
			Object.Destroy(path[path.Count - 1].visualizer);
			TileVisualizer.RefreshCell(path[path.Count - 1].cell, def.TileLayer, def.ReplacementLayer);
			path.RemoveAt(path.Count - 1);
			buildingCount = ((buildingCount == 1) ? (buildingCount = 14) : (buildingCount - 1));
			instance.setParameterByName("tileCount", buildingCount);
			SoundEvent.EndOneShot(instance);
		}
		else if (!path.Exists((PathNode n) => n.cell == cell))
		{
			bool valid = CheckValidPathPiece(cell);
			path.Add(new PathNode
			{
				cell = cell,
				visualizer = null,
				valid = valid
			});
			CheckForConnection(cell, def.PrefabID, "OutletConnected", ref previousCellConnection);
			buildingCount = buildingCount % 14 + 1;
			instance.setParameterByName("tileCount", buildingCount);
			SoundEvent.EndOneShot(instance);
		}
		visualizer.SetActive(path.Count < 2);
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(path.Count);
	}

	protected override int GetDragLength()
	{
		return path.Count;
	}

	private bool CheckValidPathPiece(int cell)
	{
		if (def.BuildLocationRule == BuildLocationRule.NotInTiles)
		{
			if (Grid.Objects[cell, 9] != null)
			{
				return false;
			}
			if (Grid.HasDoor[cell])
			{
				return false;
			}
		}
		GameObject gameObject = Grid.Objects[cell, (int)def.ObjectLayer];
		if (gameObject != null && gameObject.GetComponent<KAnimGraphTileVisualizer>() == null)
		{
			return false;
		}
		GameObject gameObject2 = Grid.Objects[cell, (int)def.TileLayer];
		if (gameObject2 != null && gameObject2.GetComponent<KAnimGraphTileVisualizer>() == null)
		{
			return false;
		}
		return true;
	}

	private bool CheckForConnection(int cell, string defName, string soundName, ref BuildingCellVisualizer outBcv, bool fireEvents = true)
	{
		outBcv = null;
		DebugUtil.Assert(defName != null, "defName was null");
		Building building = GetBuilding(cell);
		if (!building)
		{
			return false;
		}
		DebugUtil.Assert(building.gameObject, "targetBuilding.gameObject was null");
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		if (defName.Contains("LogicWire"))
		{
			LogicPorts component = building.gameObject.GetComponent<LogicPorts>();
			if (component != null)
			{
				if (component.inputPorts != null)
				{
					foreach (ILogicUIElement inputPort in component.inputPorts)
					{
						DebugUtil.Assert(inputPort != null, "input port was null");
						if (inputPort.GetLogicUICell() == cell)
						{
							num = cell;
							break;
						}
					}
				}
				if (num == -1 && component.outputPorts != null)
				{
					foreach (ILogicUIElement outputPort in component.outputPorts)
					{
						DebugUtil.Assert(outputPort != null, "output port was null");
						if (outputPort.GetLogicUICell() == cell)
						{
							num2 = cell;
							break;
						}
					}
				}
			}
		}
		else if (defName.Contains("Wire"))
		{
			num = building.GetPowerInputCell();
			num2 = building.GetPowerOutputCell();
		}
		else if (defName.Contains("Liquid"))
		{
			if (building.Def.InputConduitType == ConduitType.Liquid)
			{
				num = building.GetUtilityInputCell();
			}
			if (building.Def.OutputConduitType == ConduitType.Liquid)
			{
				num2 = building.GetUtilityOutputCell();
			}
			ElementFilter component2 = building.GetComponent<ElementFilter>();
			if (component2 != null)
			{
				DebugUtil.Assert(component2.portInfo != null, "elementFilter.portInfo was null A");
				if (component2.portInfo.conduitType == ConduitType.Liquid)
				{
					num3 = component2.GetFilteredCell();
				}
			}
		}
		else if (defName.Contains("Gas"))
		{
			if (building.Def.InputConduitType == ConduitType.Gas)
			{
				num = building.GetUtilityInputCell();
			}
			if (building.Def.OutputConduitType == ConduitType.Gas)
			{
				num2 = building.GetUtilityOutputCell();
			}
			ElementFilter component3 = building.GetComponent<ElementFilter>();
			if (component3 != null)
			{
				DebugUtil.Assert(component3.portInfo != null, "elementFilter.portInfo was null B");
				if (component3.portInfo.conduitType == ConduitType.Gas)
				{
					num3 = component3.GetFilteredCell();
				}
			}
		}
		if (cell == num || cell == num2 || cell == num3)
		{
			BuildingCellVisualizer buildingCellVisualizer = (outBcv = building.gameObject.GetComponent<BuildingCellVisualizer>());
			if ((buildingCellVisualizer != null) ? true : false)
			{
				if (fireEvents)
				{
					buildingCellVisualizer.ConnectedEvent(cell);
					string sound = GlobalAssets.GetSound(soundName);
					if (sound != null)
					{
						KMonoBehaviour.PlaySound(sound);
					}
				}
				return true;
			}
		}
		outBcv = null;
		return false;
	}

	private Building GetBuilding(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null)
		{
			return gameObject.GetComponent<Building>();
		}
		return null;
	}

	protected override Mode GetMode()
	{
		return Mode.Brush;
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		if (visualizer == null)
		{
			return;
		}
		path.Clear();
		int cell = Grid.PosToCell(cursor_pos);
		if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
		{
			bool valid = CheckValidPathPiece(cell);
			path.Add(new PathNode
			{
				cell = cell,
				visualizer = null,
				valid = valid
			});
			CheckForConnection(cell, def.PrefabID, "OutletConnected", ref previousCellConnection);
		}
		visUpdater = StartCoroutine(VisUpdater());
		visualizer.GetComponent<KBatchedAnimController>().StopAndClear();
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(1);
		placeSound = GlobalAssets.GetSound("Place_building_" + def.AudioSize);
		if (placeSound != null)
		{
			buildingCount = buildingCount % 14 + 1;
			Vector3 pos = Grid.CellToPos(cell);
			EventInstance instance = SoundEvent.BeginOneShot(placeSound, pos);
			if (def.AudioSize == "small")
			{
				instance.setParameterByName("tileCount", buildingCount);
			}
			SoundEvent.EndOneShot(instance);
		}
		base.OnLeftClickDown(cursor_pos);
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		if (!(visualizer == null))
		{
			BuildPath();
			StopVisUpdater();
			Play(visualizer, "None_Place");
			ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(0);
			base.OnLeftClickUp(cursor_pos);
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		int num = Grid.PosToCell(cursorPos);
		if (lastCell != num)
		{
			lastCell = num;
		}
		if (visualizer != null)
		{
			Color c = Color.white;
			float strength = 0f;
			if (!def.IsValidPlaceLocation(visualizer, num, Orientation.Neutral, out var _))
			{
				c = Color.red;
				strength = 1f;
			}
			SetColor(visualizer, c, strength);
		}
	}

	private void SetColor(GameObject root, Color c, float strength)
	{
		KBatchedAnimController component = root.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.TintColour = c;
		}
	}

	protected virtual void ApplyPathToConduitSystem()
	{
		DebugUtil.Assert(test: false, "I don't think this function ever runs");
	}

	private IEnumerator VisUpdater()
	{
		while (true)
		{
			conduitMgr.StashVisualGrids();
			if (path.Count == 1)
			{
				PathNode node = path[0];
				path[0] = CreateVisualizer(node);
			}
			ApplyPathToConduitSystem();
			for (int i = 0; i < path.Count; i++)
			{
				PathNode node2 = path[i];
				node2 = CreateVisualizer(node2);
				path[i] = node2;
				string text = conduitMgr.GetVisualizerString(node2.cell) + "_place";
				KBatchedAnimController component = node2.visualizer.GetComponent<KBatchedAnimController>();
				if (component.HasAnimation(text))
				{
					node2.Play(text);
				}
				else
				{
					node2.Play(conduitMgr.GetVisualizerString(node2.cell));
				}
				component.TintColour = (def.IsValidBuildLocation(null, node2.cell, Orientation.Neutral, out var _) ? Color.white : Color.red);
				TileVisualizer.RefreshCell(node2.cell, def.TileLayer, def.ReplacementLayer);
			}
			conduitMgr.UnstashVisualGrids();
			yield return null;
		}
	}

	private void BuildPath()
	{
		ApplyPathToConduitSystem();
		int num = 0;
		bool flag = false;
		for (int i = 0; i < path.Count; i++)
		{
			PathNode pathNode = path[i];
			Vector3 vector = Grid.CellToPosCBC(pathNode.cell, Grid.SceneLayer.Building);
			UtilityConnections utilityConnections = (UtilityConnections)0;
			GameObject gameObject = Grid.Objects[pathNode.cell, (int)def.TileLayer];
			if (gameObject == null)
			{
				utilityConnections = conduitMgr.GetConnections(pathNode.cell, is_physical_building: false);
				if ((DebugHandler.InstantBuildMode || (Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild)) && def.IsValidBuildLocation(visualizer, vector, Orientation.Neutral) && def.IsValidPlaceLocation(visualizer, vector, Orientation.Neutral, out var _))
				{
					gameObject = def.Build(pathNode.cell, Orientation.Neutral, null, selectedElements, 293.15f, playsound: true, GameClock.Instance.GetTime());
				}
				else
				{
					gameObject = def.TryPlace(null, vector, Orientation.Neutral, selectedElements);
					if (gameObject != null)
					{
						if (!def.MaterialsAvailable(selectedElements) && !DebugHandler.InstantBuildMode)
						{
							PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, UI.TOOLTIPS.NOMATERIAL, null, vector);
						}
						Constructable component = gameObject.GetComponent<Constructable>();
						if (component.IconConnectionAnimation(0.1f * (float)num, num, "Wire", "OutletConnected_release") || component.IconConnectionAnimation(0.1f * (float)num, num, "Pipe", "OutletConnected_release"))
						{
							num++;
						}
						flag = true;
					}
				}
			}
			else
			{
				IUtilityItem component2 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
				if (component2 != null)
				{
					utilityConnections = component2.Connections;
				}
				utilityConnections |= conduitMgr.GetConnections(pathNode.cell, is_physical_building: false);
				if (gameObject.GetComponent<BuildingComplete>() != null)
				{
					component2.UpdateConnections(utilityConnections);
				}
			}
			if (def.ReplacementLayer != ObjectLayer.NumLayers && !DebugHandler.InstantBuildMode && (!Game.Instance.SandboxModeActive || !SandboxToolParameterMenu.instance.settings.InstantBuild) && def.IsValidBuildLocation(null, vector, Orientation.Neutral))
			{
				GameObject gameObject2 = Grid.Objects[pathNode.cell, (int)def.TileLayer];
				GameObject x = Grid.Objects[pathNode.cell, (int)def.ReplacementLayer];
				if (gameObject2 != null && x == null)
				{
					BuildingComplete component3 = gameObject2.GetComponent<BuildingComplete>();
					bool flag2 = gameObject2.GetComponent<PrimaryElement>().Element.tag != selectedElements[0];
					if (component3 != null && (component3.Def != def || flag2))
					{
						Constructable component4 = def.BuildingUnderConstruction.GetComponent<Constructable>();
						component4.IsReplacementTile = true;
						gameObject = def.Instantiate(vector, Orientation.Neutral, selectedElements);
						component4.IsReplacementTile = false;
						if (!def.MaterialsAvailable(selectedElements) && !DebugHandler.InstantBuildMode)
						{
							PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, UI.TOOLTIPS.NOMATERIAL, null, vector);
						}
						Grid.Objects[pathNode.cell, (int)def.ReplacementLayer] = gameObject;
						IUtilityItem component5 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
						if (component5 != null)
						{
							utilityConnections = component5.Connections;
						}
						utilityConnections |= conduitMgr.GetConnections(pathNode.cell, is_physical_building: false);
						if (gameObject.GetComponent<BuildingComplete>() != null)
						{
							component5.UpdateConnections(utilityConnections);
						}
						string visualizerString = conduitMgr.GetVisualizerString(utilityConnections);
						string text = visualizerString;
						if (gameObject.GetComponent<KBatchedAnimController>().HasAnimation(visualizerString + "_place"))
						{
							text += "_place";
						}
						Play(gameObject, text);
						flag = true;
					}
				}
			}
			if (gameObject != null)
			{
				if (flag)
				{
					Prioritizable component6 = gameObject.GetComponent<Prioritizable>();
					if (component6 != null)
					{
						if (BuildMenu.Instance != null)
						{
							component6.SetMasterPriority(BuildMenu.Instance.GetBuildingPriority());
						}
						if (PlanScreen.Instance != null)
						{
							component6.SetMasterPriority(PlanScreen.Instance.GetBuildingPriority());
						}
					}
				}
				IUtilityItem component7 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
				if (component7 != null)
				{
					component7.Connections = utilityConnections;
				}
			}
			TileVisualizer.RefreshCell(pathNode.cell, def.TileLayer, def.ReplacementLayer);
		}
		ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(0);
	}

	private PathNode CreateVisualizer(PathNode node)
	{
		if (node.visualizer == null)
		{
			Vector3 position = Grid.CellToPosCBC(node.cell, def.SceneLayer);
			GameObject gameObject = Object.Instantiate(def.BuildingPreview, position, Quaternion.identity);
			gameObject.SetActive(value: true);
			node.visualizer = gameObject;
		}
		return node;
	}

	private void StopVisUpdater()
	{
		for (int i = 0; i < path.Count; i++)
		{
			Object.Destroy(path[i].visualizer);
		}
		path.Clear();
		if (visUpdater != null)
		{
			StopCoroutine(visUpdater);
			visUpdater = null;
		}
	}
}
