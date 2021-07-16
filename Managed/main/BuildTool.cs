using System.Collections.Generic;
using FMOD.Studio;
using Rendering;
using STRINGS;
using UnityEngine;

public class BuildTool : DragTool
{
	[SerializeField]
	private TextStyleSetting tooltipStyle;

	private int lastCell = -1;

	private int lastDragCell = -1;

	private Orientation lastDragOrientation;

	private IList<Tag> selectedElements;

	private BuildingDef def;

	private Orientation buildingOrientation;

	private ToolTip tooltip;

	public static BuildTool Instance;

	private bool active;

	private int buildingCount;

	public int GetLastCell => lastCell;

	public Orientation GetBuildingOrientation => buildingOrientation;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		tooltip = GetComponent<ToolTip>();
		buildingCount = Random.Range(1, 14);
		canChangeDragAxis = false;
	}

	protected override void OnActivateTool()
	{
		lastDragCell = -1;
		if (visualizer != null)
		{
			ClearTilePreview();
			Object.Destroy(visualizer);
		}
		active = true;
		base.OnActivateTool();
		Vector3 vector = ClampPositionToWorld(PlayerController.GetCursorPos(KInputManager.GetMousePos()), ClusterManager.Instance.activeWorld);
		visualizer = GameUtil.KInstantiate(def.BuildingPreview, vector, Grid.SceneLayer.Ore, null, LayerMask.NameToLayer("Place"));
		KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
			component.Offset = def.GetVisualizerOffset();
			component.name = component.GetComponent<KPrefabID>().GetDebugName() + "_visualizer";
		}
		Rotatable component2 = visualizer.GetComponent<Rotatable>();
		if (component2 != null)
		{
			buildingOrientation = def.InitialOrientation;
			component2.SetOrientation(buildingOrientation);
		}
		visualizer.SetActive(value: true);
		UpdateVis(vector);
		GetComponent<BuildToolHoverTextCard>().currentDef = def;
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(visualizer);
		if (component == null)
		{
			visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
		}
		else
		{
			component.SetLayer(LayerMask.NameToLayer("Place"));
		}
		GridCompositor.Instance.ToggleMajor(on: true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		lastDragCell = -1;
		if (active)
		{
			active = false;
			GridCompositor.Instance.ToggleMajor(on: false);
			buildingOrientation = Orientation.Neutral;
			HideToolTip();
			ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
			ClearTilePreview();
			Object.Destroy(visualizer);
			if (new_tool == SelectTool.Instance)
			{
				Game.Instance.Trigger(-1190690038);
			}
			base.OnDeactivateTool(new_tool);
		}
	}

	public void Activate(BuildingDef def, IList<Tag> selected_elements)
	{
		selectedElements = selected_elements;
		this.def = def;
		viewMode = def.ViewMode;
		ResourceRemainingDisplayScreen.instance.SetResources(selected_elements, def.CraftRecipe);
		PlayerController.Instance.ActivateTool(this);
		OnActivateTool();
	}

	public void Deactivate()
	{
		selectedElements = null;
		SelectTool.Instance.Activate();
		def = null;
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
	}

	private void ClearTilePreview()
	{
		if (!Grid.IsValidBuildingCell(lastCell) || !def.IsTilePiece)
		{
			return;
		}
		GameObject gameObject = Grid.Objects[lastCell, (int)def.TileLayer];
		if (visualizer == gameObject)
		{
			Grid.Objects[lastCell, (int)def.TileLayer] = null;
		}
		if (def.isKAnimTile)
		{
			GameObject x = null;
			if (def.ReplacementLayer != ObjectLayer.NumLayers)
			{
				x = Grid.Objects[lastCell, (int)def.ReplacementLayer];
			}
			if ((gameObject == null || gameObject.GetComponent<Constructable>() == null) && (x == null || x == visualizer))
			{
				World.Instance.blockTileRenderer.RemoveBlock(def, isReplacement: false, SimHashes.Void, lastCell);
				World.Instance.blockTileRenderer.RemoveBlock(def, isReplacement: true, SimHashes.Void, lastCell);
				TileVisualizer.RefreshCell(lastCell, def.TileLayer, def.ReplacementLayer);
			}
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		cursorPos = ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
		UpdateVis(cursorPos);
	}

	private void UpdateVis(Vector3 pos)
	{
		bool flag = def.IsValidPlaceLocation(visualizer, pos, buildingOrientation, out var _);
		bool flag2 = def.IsValidReplaceLocation(pos, buildingOrientation, def.ReplacementLayer, def.ObjectLayer);
		flag = flag || flag2;
		if (visualizer != null)
		{
			Color c = Color.white;
			float strength = 0f;
			if (!flag)
			{
				c = Color.red;
				strength = 1f;
			}
			SetColor(visualizer, c, strength);
		}
		int num = Grid.PosToCell(pos);
		if (!(def != null))
		{
			return;
		}
		Vector3 vector = Grid.CellToPosCBC(num, def.SceneLayer);
		visualizer.transform.SetPosition(vector);
		base.transform.SetPosition(vector - Vector3.up * 0.5f);
		if (def.IsTilePiece)
		{
			ClearTilePreview();
			if (Grid.IsValidBuildingCell(num))
			{
				GameObject gameObject = Grid.Objects[num, (int)def.TileLayer];
				if (gameObject == null)
				{
					Grid.Objects[num, (int)def.TileLayer] = visualizer;
				}
				if (def.isKAnimTile)
				{
					GameObject x = null;
					if (def.ReplacementLayer != ObjectLayer.NumLayers)
					{
						x = Grid.Objects[num, (int)def.ReplacementLayer];
					}
					if (gameObject == null || (gameObject.GetComponent<Constructable>() == null && x == null))
					{
						TileVisualizer.RefreshCell(num, def.TileLayer, def.ReplacementLayer);
						if (def.BlockTileAtlas != null)
						{
							int renderLayer = LayerMask.NameToLayer("Overlay");
							BlockTileRenderer blockTileRenderer = World.Instance.blockTileRenderer;
							blockTileRenderer.SetInvalidPlaceCell(num, !flag);
							if (lastCell != num)
							{
								blockTileRenderer.SetInvalidPlaceCell(lastCell, enabled: false);
							}
							blockTileRenderer.AddBlock(renderLayer, def, flag2, SimHashes.Void, num);
						}
					}
				}
			}
		}
		if (lastCell != num)
		{
			lastCell = num;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.RotateBuilding))
		{
			if (!(visualizer != null))
			{
				return;
			}
			Rotatable component = visualizer.GetComponent<Rotatable>();
			if (component != null)
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Rotate"));
				buildingOrientation = component.Rotate();
				if (Grid.IsValidBuildingCell(lastCell))
				{
					Vector3 pos = Grid.CellToPosCCC(lastCell, Grid.SceneLayer.Building);
					UpdateVis(pos);
				}
				if (base.Dragging && lastDragCell != -1)
				{
					TryBuild(lastDragCell);
				}
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		TryBuild(cell);
	}

	private void TryBuild(int cell)
	{
		if (visualizer == null || (cell == lastDragCell && buildingOrientation == lastDragOrientation) || (Grid.PosToCell(visualizer) != cell && ((bool)def.BuildingComplete.GetComponent<LogicPorts>() || (bool)def.BuildingComplete.GetComponent<LogicGateBase>())))
		{
			return;
		}
		lastDragCell = cell;
		lastDragOrientation = buildingOrientation;
		ClearTilePreview();
		Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
		GameObject gameObject = null;
		if (DebugHandler.InstantBuildMode || (Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild))
		{
			if (def.IsValidBuildLocation(visualizer, vector, buildingOrientation) && def.IsValidPlaceLocation(visualizer, vector, buildingOrientation, out var _))
			{
				gameObject = def.Build(cell, buildingOrientation, null, selectedElements, 293.15f, playsound: false, GameClock.Instance.GetTime());
			}
		}
		else
		{
			gameObject = def.TryPlace(visualizer, vector, buildingOrientation, selectedElements);
			if (gameObject == null && def.ReplacementLayer != ObjectLayer.NumLayers)
			{
				GameObject replacementCandidate = def.GetReplacementCandidate(cell);
				if (replacementCandidate != null && !def.IsReplacementLayerOccupied(cell))
				{
					BuildingComplete component = replacementCandidate.GetComponent<BuildingComplete>();
					if (component != null && component.Def.Replaceable && def.CanReplace(replacementCandidate) && (component.Def != def || selectedElements[0] != replacementCandidate.GetComponent<PrimaryElement>().Element.tag))
					{
						gameObject = def.TryReplaceTile(visualizer, vector, buildingOrientation, selectedElements);
						Grid.Objects[cell, (int)def.ReplacementLayer] = gameObject;
					}
				}
			}
			if (gameObject != null)
			{
				Prioritizable component2 = gameObject.GetComponent<Prioritizable>();
				if (component2 != null)
				{
					if (BuildMenu.Instance != null)
					{
						component2.SetMasterPriority(BuildMenu.Instance.GetBuildingPriority());
					}
					if (PlanScreen.Instance != null)
					{
						component2.SetMasterPriority(PlanScreen.Instance.GetBuildingPriority());
					}
				}
			}
		}
		if (!(gameObject != null))
		{
			return;
		}
		if (def.MaterialsAvailable(selectedElements, ClusterManager.Instance.activeWorld) || DebugHandler.InstantBuildMode)
		{
			placeSound = GlobalAssets.GetSound("Place_Building_" + def.AudioSize);
			if (placeSound != null)
			{
				buildingCount = buildingCount % 14 + 1;
				Vector3 pos = vector;
				pos.z = 0f;
				EventInstance instance = SoundEvent.BeginOneShot(placeSound, pos);
				if (def.AudioSize == "small")
				{
					instance.setParameterByName("tileCount", buildingCount);
				}
				SoundEvent.EndOneShot(instance);
			}
		}
		else
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, UI.TOOLTIPS.NOMATERIAL, null, vector);
		}
		Rotatable component3 = gameObject.GetComponent<Rotatable>();
		if (component3 != null)
		{
			component3.SetOrientation(buildingOrientation);
		}
		if (def.OnePerWorld)
		{
			PlayerController.Instance.ActivateTool(SelectTool.Instance);
		}
	}

	protected override Mode GetMode()
	{
		return Mode.Brush;
	}

	private void SetColor(GameObject root, Color c, float strength)
	{
		KBatchedAnimController component = root.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.TintColour = c;
		}
	}

	private void ShowToolTip()
	{
		ToolTipScreen.Instance.SetToolTip(tooltip);
	}

	private void HideToolTip()
	{
		ToolTipScreen.Instance.ClearToolTip(tooltip);
	}

	public void Update()
	{
		if (active)
		{
			KBatchedAnimController component = visualizer.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				component.SetLayer(LayerMask.NameToLayer("Place"));
			}
		}
	}

	public override string GetDeactivateSound()
	{
		return "HUD_Click_Deselect";
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		base.OnLeftClickUp(cursor_pos);
	}

	public void SetToolOrientation(Orientation orientation)
	{
		if (!(visualizer != null))
		{
			return;
		}
		Rotatable component = visualizer.GetComponent<Rotatable>();
		if (component != null)
		{
			buildingOrientation = orientation;
			component.SetOrientation(orientation);
			if (Grid.IsValidBuildingCell(lastCell))
			{
				Vector3 pos = Grid.CellToPosCCC(lastCell, Grid.SceneLayer.Building);
				UpdateVis(pos);
			}
			if (base.Dragging && lastDragCell != -1)
			{
				TryBuild(lastDragCell);
			}
		}
	}
}
