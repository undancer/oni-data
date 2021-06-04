using System;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class DragTool : InterfaceTool
{
	private enum DragAxis
	{
		Invalid = -1,
		None,
		Horizontal,
		Vertical
	}

	public enum Mode
	{
		Brush,
		Box
	}

	[SerializeField]
	private Texture2D boxCursor;

	[SerializeField]
	private GameObject areaVisualizer;

	[SerializeField]
	private GameObject areaVisualizerTextPrefab;

	[SerializeField]
	private Color32 areaColour = new Color(1f, 1f, 1f, 0.5f);

	protected SpriteRenderer areaVisualizerSpriteRenderer;

	protected Guid areaVisualizerText;

	protected Vector3 placementPivot;

	protected bool interceptNumberKeysForPriority = false;

	private bool dragging = false;

	private Vector3 previousCursorPos;

	private Mode mode = Mode.Box;

	private DragAxis dragAxis = DragAxis.Invalid;

	protected bool canChangeDragAxis = true;

	protected Vector3 downPos;

	public bool Dragging => dragging;

	protected virtual Mode GetMode()
	{
		return mode;
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		dragging = false;
		SetMode(mode);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		KScreenManager.Instance.SetEventSystemEnabled(state: true);
		if (areaVisualizerText != Guid.Empty)
		{
			NameDisplayScreen.Instance.RemoveWorldText(areaVisualizerText);
			areaVisualizerText = Guid.Empty;
		}
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
			areaVisualizerSpriteRenderer = areaVisualizer.GetComponent<SpriteRenderer>();
			areaVisualizer.transform.SetParent(base.transform);
			Renderer component = areaVisualizer.GetComponent<Renderer>();
			component.material.color = areaColour;
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
		cursor_pos = ClampPositionToWorld(cursor_pos, ClusterManager.Instance.activeWorld);
		dragging = true;
		downPos = cursor_pos;
		previousCursorPos = cursor_pos;
		KScreenManager.Instance.SetEventSystemEnabled(state: false);
		hasFocus = true;
		if (areaVisualizerTextPrefab != null)
		{
			areaVisualizerText = NameDisplayScreen.Instance.AddWorldText("", areaVisualizerTextPrefab);
			GameObject worldText = NameDisplayScreen.Instance.GetWorldText(areaVisualizerText);
			LocText component = worldText.GetComponent<LocText>();
			component.color = areaColour;
		}
		switch (GetMode())
		{
		case Mode.Brush:
			if (visualizer != null)
			{
				AddDragPoint(cursor_pos);
			}
			break;
		case Mode.Box:
			if (visualizer != null)
			{
				visualizer.SetActive(value: false);
			}
			if (areaVisualizer != null)
			{
				areaVisualizer.SetActive(value: true);
				areaVisualizer.transform.SetPosition(cursor_pos);
				areaVisualizerSpriteRenderer.size = new Vector2(0.01f, 0.01f);
			}
			break;
		}
	}

	public void CancelDragging()
	{
		KScreenManager.Instance.SetEventSystemEnabled(state: true);
		dragAxis = DragAxis.Invalid;
		if (dragging)
		{
			dragging = false;
			if (areaVisualizerText != Guid.Empty)
			{
				NameDisplayScreen.Instance.RemoveWorldText(areaVisualizerText);
				areaVisualizerText = Guid.Empty;
			}
			Mode mode = GetMode();
			if (mode == Mode.Box && areaVisualizer != null)
			{
				areaVisualizer.SetActive(value: false);
			}
		}
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		cursor_pos = ClampPositionToWorld(cursor_pos, ClusterManager.Instance.activeWorld);
		KScreenManager.Instance.SetEventSystemEnabled(state: true);
		dragAxis = DragAxis.Invalid;
		if (!dragging)
		{
			return;
		}
		dragging = false;
		if (areaVisualizerText != Guid.Empty)
		{
			NameDisplayScreen.Instance.RemoveWorldText(areaVisualizerText);
			areaVisualizerText = Guid.Empty;
		}
		Mode mode = GetMode();
		if (mode != Mode.Box || !(areaVisualizer != null))
		{
			return;
		}
		areaVisualizer.SetActive(value: false);
		Grid.PosToXY(downPos, out var x, out var y);
		int num = x;
		int num2 = y;
		Grid.PosToXY(cursor_pos, out var x2, out var y2);
		if (x2 < x)
		{
			Util.Swap(ref x, ref x2);
		}
		if (y2 < y)
		{
			Util.Swap(ref y, ref y2);
		}
		for (int i = y; i <= y2; i++)
		{
			for (int j = x; j <= x2; j++)
			{
				int cell = Grid.XYToCell(j, i);
				if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
				{
					int value = i - num2;
					int value2 = j - num;
					value = Mathf.Abs(value);
					value2 = Mathf.Abs(value2);
					OnDragTool(cell, value + value2);
				}
			}
		}
		string sound = GlobalAssets.GetSound(GetConfirmSound());
		KMonoBehaviour.PlaySound(sound);
		OnDragComplete(downPos, cursor_pos);
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

	protected Vector3 ClampPositionToWorld(Vector3 position, WorldContainer world)
	{
		position.x = Mathf.Clamp(position.x, world.minimumBounds.x, world.maximumBounds.x);
		position.y = Mathf.Clamp(position.y, world.minimumBounds.y, world.maximumBounds.y);
		return position;
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		cursorPos = ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
		if (dragging)
		{
			if (Input.GetKey((KeyCode)Global.Instance.GetInputManager().GetDefaultController().GetInputForAction(Action.DragStraight)))
			{
				Vector3 vector = cursorPos - downPos;
				if ((canChangeDragAxis || dragAxis == DragAxis.Invalid) && vector.sqrMagnitude > 0.707f)
				{
					if (Mathf.Abs(vector.x) < Mathf.Abs(vector.y))
					{
						dragAxis = DragAxis.Vertical;
					}
					else
					{
						dragAxis = DragAxis.Horizontal;
					}
				}
			}
			else
			{
				dragAxis = DragAxis.Invalid;
			}
			switch (dragAxis)
			{
			case DragAxis.Horizontal:
				cursorPos.y = downPos.y;
				break;
			case DragAxis.Vertical:
				cursorPos.x = downPos.x;
				break;
			}
		}
		base.OnMouseMove(cursorPos);
		if (!dragging)
		{
			return;
		}
		switch (GetMode())
		{
		case Mode.Brush:
			AddDragPoints(cursorPos, previousCursorPos);
			if (areaVisualizerText != Guid.Empty)
			{
				int dragLength = GetDragLength();
				GameObject worldText2 = NameDisplayScreen.Instance.GetWorldText(areaVisualizerText);
				LocText component2 = worldText2.GetComponent<LocText>();
				component2.text = string.Format(UI.TOOLS.TOOL_LENGTH_FMT, dragLength);
				Vector3 position2 = Grid.CellToPos(Grid.PosToCell(cursorPos));
				position2 += new Vector3(0f, 1f, 0f);
				component2.transform.SetPosition(position2);
			}
			break;
		case Mode.Box:
		{
			Vector2 input = Vector3.Max(downPos, cursorPos);
			Vector2 input2 = Vector3.Min(downPos, cursorPos);
			input = GetWorldRestrictedPosition(input);
			input2 = GetWorldRestrictedPosition(input2);
			input = GetRegularizedPos(input, minimize: false);
			input2 = GetRegularizedPos(input2, minimize: true);
			Vector2 vector2 = input - input2;
			Vector2 vector3 = (input + input2) * 0.5f;
			areaVisualizer.transform.SetPosition(new Vector2(vector3.x, vector3.y));
			int num = (int)(input.x - input2.x + (input.y - input2.y) - 1f);
			if (areaVisualizerSpriteRenderer.size != vector2)
			{
				string sound = GlobalAssets.GetSound(GetDragSound());
				if (sound != null)
				{
					Vector3 position = areaVisualizer.transform.GetPosition();
					position.z = 0f;
					EventInstance instance = SoundEvent.BeginOneShot(sound, position);
					instance.setParameterByName("tileCount", num);
					SoundEvent.EndOneShot(instance);
				}
			}
			areaVisualizerSpriteRenderer.size = vector2;
			if (areaVisualizerText != Guid.Empty)
			{
				Vector2I vector2I = new Vector2I(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
				GameObject worldText = NameDisplayScreen.Instance.GetWorldText(areaVisualizerText);
				LocText component = worldText.GetComponent<LocText>();
				component.text = string.Format(UI.TOOLS.TOOL_AREA_FMT, vector2I.x, vector2I.y, vector2I.x * vector2I.y);
				Vector2 v = vector3;
				component.transform.SetPosition(v);
			}
			break;
		}
		}
		previousCursorPos = cursorPos;
	}

	protected virtual void OnDragTool(int cell, int distFromOrigin)
	{
	}

	protected virtual void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp)
	{
	}

	protected virtual int GetDragLength()
	{
		return 0;
	}

	private void AddDragPoint(Vector3 cursorPos)
	{
		cursorPos = ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
		int cell = Grid.PosToCell(cursorPos);
		if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
		{
			OnDragTool(cell, 0);
		}
	}

	private void AddDragPoints(Vector3 cursorPos, Vector3 previousCursorPos)
	{
		cursorPos = ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
		Vector3 a = cursorPos - previousCursorPos;
		float magnitude = a.magnitude;
		float num = Grid.CellSizeInMeters * 0.25f;
		int num2 = 1 + (int)(magnitude / num);
		a.Normalize();
		for (int i = 0; i < num2; i++)
		{
			Vector3 cursorPos2 = previousCursorPos + a * ((float)i * num);
			AddDragPoint(cursorPos2);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (interceptNumberKeysForPriority)
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
		if (interceptNumberKeysForPriority)
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

	protected void SetMode(Mode newMode)
	{
		mode = newMode;
		switch (mode)
		{
		case Mode.Brush:
			if (areaVisualizer != null)
			{
				areaVisualizer.SetActive(value: false);
			}
			if (visualizer != null)
			{
				visualizer.SetActive(value: true);
			}
			SetCursor(cursor, cursorOffset, CursorMode.Auto);
			break;
		case Mode.Box:
			if (visualizer != null)
			{
				visualizer.SetActive(value: true);
			}
			mode = Mode.Box;
			SetCursor(boxCursor, cursorOffset, CursorMode.Auto);
			break;
		}
	}

	public override void OnFocus(bool focus)
	{
		switch (GetMode())
		{
		case Mode.Brush:
			if (visualizer != null)
			{
				visualizer.SetActive(focus);
			}
			hasFocus = focus;
			break;
		case Mode.Box:
			if (visualizer != null && !dragging)
			{
				visualizer.SetActive(focus);
			}
			hasFocus = focus || dragging;
			break;
		}
		base.OnFocus(focus);
	}

	private void OnTutorialOpened(object data)
	{
		dragging = false;
	}

	public override bool ShowHoverUI()
	{
		return dragging || base.ShowHoverUI();
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
	}
}
