using System;
using System.Collections.Generic;
using UnityEngine;

public class BrushTool : InterfaceTool
{
	private enum DragAxis
	{
		Invalid = -1,
		None,
		Horizontal,
		Vertical
	}

	[SerializeField]
	private Texture2D brushCursor;

	[SerializeField]
	private GameObject areaVisualizer;

	[SerializeField]
	private Color32 areaColour = new Color(1f, 1f, 1f, 0.5f);

	protected Color radiusIndicatorColor = new Color(0.5f, 0.7f, 0.5f, 0.2f);

	protected Vector3 placementPivot;

	protected bool interceptNumberKeysForPriority;

	protected List<Vector2> brushOffsets = new List<Vector2>();

	protected bool affectFoundation;

	private bool dragging;

	protected int brushRadius = -1;

	private DragAxis dragAxis = DragAxis.Invalid;

	protected Vector3 downPos;

	protected int currentCell;

	protected HashSet<int> cellsInRadius = new HashSet<int>();

	public bool Dragging => dragging;

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		dragging = false;
	}

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		foreach (int item in cellsInRadius)
		{
			colors.Add(new ToolMenu.CellColorData(item, radiusIndicatorColor));
		}
	}

	public virtual void SetBrushSize(int radius)
	{
		if (radius == brushRadius)
		{
			return;
		}
		brushRadius = radius;
		brushOffsets.Clear();
		for (int i = 0; i < brushRadius * 2; i++)
		{
			for (int j = 0; j < brushRadius * 2; j++)
			{
				if (Vector2.Distance(new Vector2(i, j), new Vector2(brushRadius, brushRadius)) < (float)brushRadius - 0.8f)
				{
					brushOffsets.Add(new Vector2(i - brushRadius, j - brushRadius));
				}
			}
		}
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		KScreenManager.Instance.SetEventSystemEnabled(state: true);
		base.OnDeactivateTool(new_tool);
	}

	protected override void OnPrefabInit()
	{
		Game.Instance.Subscribe(1634669191, OnTutorialOpened);
		base.OnPrefabInit();
		if (visualizer != null)
		{
			visualizer = Util.KInstantiate(visualizer);
		}
		if (areaVisualizer != null)
		{
			areaVisualizer = Util.KInstantiate(areaVisualizer);
			areaVisualizer.SetActive(value: false);
			areaVisualizer.GetComponent<RectTransform>().SetParent(base.transform);
			areaVisualizer.GetComponent<Renderer>().material.color = areaColour;
		}
	}

	protected override void OnCmpEnable()
	{
		dragging = false;
	}

	protected override void OnCmpDisable()
	{
		if (visualizer != null)
		{
			visualizer.SetActive(value: false);
		}
		if (areaVisualizer != null)
		{
			areaVisualizer.SetActive(value: false);
		}
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		cursor_pos -= placementPivot;
		dragging = true;
		downPos = cursor_pos;
		KScreenManager.Instance.SetEventSystemEnabled(state: false);
		Paint();
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		cursor_pos -= placementPivot;
		KScreenManager.Instance.SetEventSystemEnabled(state: true);
		if (dragging)
		{
			dragging = false;
			switch (dragAxis)
			{
			case DragAxis.Horizontal:
				cursor_pos.y = downPos.y;
				dragAxis = DragAxis.None;
				break;
			case DragAxis.Vertical:
				cursor_pos.x = downPos.x;
				dragAxis = DragAxis.None;
				break;
			}
		}
	}

	protected virtual string GetConfirmSound()
	{
		return "Tile_Confirm";
	}

	protected virtual string GetDragSound()
	{
		return "Tile_Drag";
	}

	public override string GetDeactivateSound()
	{
		return "Tile_Cancel";
	}

	private static int GetGridDistance(int cell, int center_cell)
	{
		Vector2I u = Grid.CellToXY(cell);
		Vector2I v = Grid.CellToXY(center_cell);
		Vector2I vector2I = u - v;
		return Math.Abs(vector2I.x) + Math.Abs(vector2I.y);
	}

	private void Paint()
	{
		foreach (int item in cellsInRadius)
		{
			if (Grid.IsValidCell(item) && Grid.WorldIdx[item] == ClusterManager.Instance.activeWorldId && (!Grid.Foundation[item] || affectFoundation))
			{
				OnPaintCell(item, Grid.GetCellDistance(currentCell, item));
			}
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		int num = (currentCell = Grid.PosToCell(cursorPos));
		base.OnMouseMove(cursorPos);
		cellsInRadius.Clear();
		int num2 = -1;
		foreach (Vector2 brushOffset in brushOffsets)
		{
			num2 = Grid.OffsetCell(Grid.PosToCell(cursorPos), new CellOffset((int)brushOffset.x, (int)brushOffset.y));
			if (Grid.IsValidCell(num2) && Grid.WorldIdx[num2] == ClusterManager.Instance.activeWorldId)
			{
				cellsInRadius.Add(Grid.OffsetCell(Grid.PosToCell(cursorPos), new CellOffset((int)brushOffset.x, (int)brushOffset.y)));
			}
		}
		if (dragging)
		{
			Paint();
		}
	}

	protected virtual void OnPaintCell(int cell, int distFromOrigin)
	{
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.DragStraight))
		{
			dragAxis = DragAxis.None;
		}
		else if (interceptNumberKeysForPriority)
		{
			HandlePriortyKeysDown(e);
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (e.TryConsume(Action.DragStraight))
		{
			dragAxis = DragAxis.Invalid;
		}
		else if (interceptNumberKeysForPriority)
		{
			HandlePriorityKeysUp(e);
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	private void HandlePriortyKeysDown(KButtonEvent e)
	{
		Action action = e.GetAction();
		if (Action.Plan1 <= action && action <= Action.Plan10 && e.TryConsume(action))
		{
			int num = (int)(action - 36 + 1);
			if (num <= 9)
			{
				ToolMenu.Instance.PriorityScreen.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, num), play_sound: true);
			}
			else
			{
				ToolMenu.Instance.PriorityScreen.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.topPriority, 1), play_sound: true);
			}
		}
	}

	private void HandlePriorityKeysUp(KButtonEvent e)
	{
		Action action = e.GetAction();
		if (Action.Plan1 <= action && action <= Action.Plan10)
		{
			e.TryConsume(action);
		}
	}

	public override void OnFocus(bool focus)
	{
		if (visualizer != null)
		{
			visualizer.SetActive(focus);
		}
		hasFocus = focus;
		base.OnFocus(focus);
	}

	private void OnTutorialOpened(object data)
	{
		dragging = false;
	}

	public override bool ShowHoverUI()
	{
		if (!dragging)
		{
			return base.ShowHoverUI();
		}
		return true;
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
	}
}
