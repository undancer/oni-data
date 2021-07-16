using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/scripts/PlayerController")]
public class PlayerController : KMonoBehaviour, IInputHandler
{
	public InterfaceTool[] tools;

	private InterfaceTool activeTool;

	private bool DebugHidingCursor;

	private Vector3 prevMousePos = new Vector3(float.PositiveInfinity, 0f, 0f);

	private const float MIN_DRAG_DIST = 6f;

	private const float MIN_DRAG_TIME = 0.3f;

	private Action dragAction;

	private bool draggingAllowed = true;

	private bool dragging;

	private bool queueStopDrag;

	private Vector3 startDragPos;

	private float startDragTime;

	private Vector3 dragDelta;

	private Vector3 worldDragDelta;

	public string handlerName => "PlayerController";

	public KInputHandler inputHandler
	{
		get;
		set;
	}

	public InterfaceTool ActiveTool => activeTool;

	public static PlayerController Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		for (int i = 0; i < tools.Length; i++)
		{
			if (DlcManager.IsDlcListValidForCurrentContent(tools[i].DlcIDs))
			{
				GameObject gameObject = Util.KInstantiate(tools[i].gameObject, base.gameObject);
				tools[i] = gameObject.GetComponent<InterfaceTool>();
				tools[i].gameObject.SetActive(value: true);
				tools[i].gameObject.SetActive(value: false);
			}
		}
	}

	protected override void OnSpawn()
	{
		ActivateTool(tools[0]);
	}

	private Vector3 GetCursorPos()
	{
		return GetCursorPos(KInputManager.GetMousePos());
	}

	public static Vector3 GetCursorPos(Vector3 mouse_pos)
	{
		Vector3 result;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(mouse_pos), out var hitInfo, float.PositiveInfinity, Game.BlockSelectionLayerMask))
		{
			result = hitInfo.point;
		}
		else
		{
			mouse_pos.z = 0f - Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
			result = Camera.main.ScreenToWorldPoint(mouse_pos);
		}
		float x = result.x;
		float y = result.y;
		x = Mathf.Max(x, 0f);
		x = Mathf.Min(x, Grid.WidthInMeters);
		y = Mathf.Max(y, 0f);
		y = Mathf.Min(y, Grid.HeightInMeters);
		result.x = x;
		result.y = y;
		return result;
	}

	private void UpdateHover()
	{
		UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
		if (current != null)
		{
			activeTool.OnFocus(!current.IsPointerOverGameObject());
		}
	}

	private void Update()
	{
		UpdateDrag();
		if ((bool)activeTool && activeTool.enabled)
		{
			UpdateHover();
			Vector3 cursorPos = GetCursorPos();
			if (cursorPos != prevMousePos)
			{
				prevMousePos = cursorPos;
				activeTool.OnMouseMove(cursorPos);
			}
		}
		if (Input.GetKeyDown(KeyCode.F12) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
		{
			DebugHidingCursor = !DebugHidingCursor;
			Cursor.visible = !DebugHidingCursor;
			HoverTextScreen.Instance.Show(!DebugHidingCursor);
		}
	}

	private void LateUpdate()
	{
		if (queueStopDrag)
		{
			queueStopDrag = false;
			dragging = false;
			dragAction = Action.Invalid;
			dragDelta = Vector3.zero;
			worldDragDelta = Vector3.zero;
		}
	}

	public void ActivateTool(InterfaceTool tool)
	{
		if (!(activeTool == tool))
		{
			DeactivateTool(tool);
			activeTool = tool;
			activeTool.enabled = true;
			activeTool.gameObject.SetActive(value: true);
			activeTool.ActivateTool();
			UpdateHover();
		}
	}

	public void ToolDeactivated(InterfaceTool tool)
	{
		if (activeTool == tool && activeTool != null)
		{
			DeactivateTool();
		}
		if (activeTool == null)
		{
			ActivateTool(SelectTool.Instance);
		}
	}

	private void DeactivateTool(InterfaceTool new_tool = null)
	{
		if (activeTool != null)
		{
			activeTool.enabled = false;
			activeTool.gameObject.SetActive(value: false);
			InterfaceTool interfaceTool = activeTool;
			activeTool = null;
			interfaceTool.DeactivateTool(new_tool);
		}
	}

	public bool IsUsingDefaultTool()
	{
		return activeTool == tools[0];
	}

	private void StartDrag(Action action)
	{
		if (dragAction == Action.Invalid)
		{
			dragAction = action;
			startDragPos = KInputManager.GetMousePos();
			startDragTime = Time.unscaledTime;
		}
	}

	private void UpdateDrag()
	{
		dragDelta = Vector2.zero;
		Vector3 mousePos = KInputManager.GetMousePos();
		if (!dragging && dragAction != 0 && ((mousePos - startDragPos).magnitude > 6f || Time.unscaledTime - startDragTime > 0.3f))
		{
			dragging = true;
		}
		if (dragging)
		{
			dragDelta = mousePos - startDragPos;
			worldDragDelta = Camera.main.ScreenToWorldPoint(mousePos) - Camera.main.ScreenToWorldPoint(startDragPos);
			startDragPos = mousePos;
		}
	}

	private void StopDrag(Action action)
	{
		if (dragAction == action)
		{
			queueStopDrag = true;
		}
	}

	public void CancelDragging()
	{
		queueStopDrag = true;
		if (activeTool != null)
		{
			DragTool dragTool = activeTool as DragTool;
			if (dragTool != null)
			{
				dragTool.CancelDragging();
			}
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.ToggleScreenshotMode))
		{
			DebugHandler.ToggleScreenshotMode();
			return;
		}
		if (DebugHandler.HideUI && e.TryConsume(Action.Escape))
		{
			DebugHandler.ToggleScreenshotMode();
			return;
		}
		if (e.IsAction(Action.MouseLeft) || e.IsAction(Action.ShiftMouseLeft))
		{
			StartDrag(Action.MouseLeft);
		}
		else if (e.IsAction(Action.MouseRight))
		{
			StartDrag(Action.MouseRight);
		}
		else if (e.IsAction(Action.MouseMiddle))
		{
			StartDrag(Action.MouseMiddle);
		}
		if (activeTool == null || !activeTool.enabled)
		{
			return;
		}
		List<RaycastResult> list = new List<RaycastResult>();
		PointerEventData pointerEventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
		pointerEventData.position = KInputManager.GetMousePos();
		UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
		if (current != null)
		{
			current.RaycastAll(pointerEventData, list);
			if (list.Count > 0)
			{
				return;
			}
		}
		if (e.TryConsume(Action.MouseLeft) || e.TryConsume(Action.ShiftMouseLeft))
		{
			activeTool.OnLeftClickDown(GetCursorPos());
		}
		else if (e.IsAction(Action.MouseRight))
		{
			activeTool.OnRightClickDown(GetCursorPos(), e);
		}
		else
		{
			activeTool.OnKeyDown(e);
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (e.IsAction(Action.MouseLeft) || e.IsAction(Action.ShiftMouseLeft))
		{
			StopDrag(Action.MouseLeft);
		}
		else if (e.IsAction(Action.MouseRight))
		{
			StopDrag(Action.MouseRight);
		}
		else if (e.IsAction(Action.MouseMiddle))
		{
			StopDrag(Action.MouseMiddle);
		}
		if (!(activeTool == null) && activeTool.enabled && activeTool.hasFocus)
		{
			if (e.TryConsume(Action.MouseLeft) || e.TryConsume(Action.ShiftMouseLeft))
			{
				activeTool.OnLeftClickUp(GetCursorPos());
			}
			else if (e.IsAction(Action.MouseRight))
			{
				activeTool.OnRightClickUp(GetCursorPos());
			}
			else
			{
				activeTool.OnKeyUp(e);
			}
		}
	}

	public bool ConsumeIfNotDragging(KButtonEvent e, Action action)
	{
		if (dragAction != action || !dragging)
		{
			return e.TryConsume(action);
		}
		return false;
	}

	public bool IsDragging()
	{
		if (draggingAllowed)
		{
			return dragAction != Action.Invalid;
		}
		return false;
	}

	public void AllowDragging(bool allow)
	{
		draggingAllowed = allow;
	}

	public Vector3 GetDragDelta()
	{
		return dragDelta;
	}

	public Vector3 GetWorldDragDelta()
	{
		if (!draggingAllowed)
		{
			return Vector3.zero;
		}
		return worldDragDelta;
	}
}
