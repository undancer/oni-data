using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/scripts/InterfaceTool")]
public class InterfaceTool : KMonoBehaviour
{
	public struct Intersection
	{
		public MonoBehaviour component;

		public float distance;
	}

	public const float MaxClickDistance = 0.02f;

	public const float DepthBias = -0.15f;

	public GameObject visualizer;

	public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;

	public string placeSound;

	protected bool populateHitsList;

	[NonSerialized]
	public bool hasFocus;

	[SerializeField]
	protected Texture2D cursor;

	public Vector2 cursorOffset = new Vector2(2f, 2f);

	public System.Action OnDeactivate;

	private static Texture2D activeCursor = null;

	private static HashedString toolActivatedViewMode = OverlayModes.None.ID;

	protected HashedString viewMode = OverlayModes.None.ID;

	private HoverTextConfiguration hoverTextConfiguration;

	private KSelectable hoverOverride;

	public KSelectable hover;

	protected int layerMask;

	protected SelectMarker selectMarker;

	private List<RaycastResult> castResults = new List<RaycastResult>();

	private bool isAppFocused = true;

	private List<KSelectable> hits = new List<KSelectable>();

	protected bool playedSoundThisFrame;

	private List<Intersection> intersections = new List<Intersection>();

	private HashSet<Component> prevIntersectionGroup = new HashSet<Component>();

	private HashSet<Component> curIntersectionGroup = new HashSet<Component>();

	private int hitCycleCount;

	public HashedString ViewMode => viewMode;

	public virtual string[] DlcIDs => DlcManager.AVAILABLE_ALL_VERSIONS;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		hoverTextConfiguration = GetComponent<HoverTextConfiguration>();
	}

	public void ActivateTool()
	{
		OnActivateTool();
		OnMouseMove(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
		Game.Instance.Trigger(1174281782, this);
	}

	public virtual bool ShowHoverUI()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
		if (OverlayScreen.Instance == null || !ClusterManager.Instance.IsPositionInActiveWorld(pos) || pos.x < 0f || pos.x > Grid.WidthInMeters || pos.y < 0f || pos.y > Grid.HeightInMeters)
		{
			return false;
		}
		bool result = false;
		UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
		if (current != null)
		{
			Vector3 vector = new Vector3(KInputManager.GetMousePos().x, KInputManager.GetMousePos().y, 0f);
			PointerEventData pointerEventData = new PointerEventData(current);
			pointerEventData.position = vector;
			current.RaycastAll(pointerEventData, castResults);
			result = castResults.Count == 0;
		}
		return result;
	}

	protected virtual void OnActivateTool()
	{
		if (OverlayScreen.Instance != null && viewMode != OverlayModes.None.ID && OverlayScreen.Instance.mode != viewMode)
		{
			OverlayScreen.Instance.ToggleOverlay(viewMode);
			toolActivatedViewMode = viewMode;
		}
		SetCursor(cursor, cursorOffset, CursorMode.Auto);
	}

	public void DeactivateTool(InterfaceTool new_tool = null)
	{
		OnDeactivateTool(new_tool);
		if ((new_tool == null || new_tool == SelectTool.Instance) && toolActivatedViewMode != OverlayModes.None.ID && toolActivatedViewMode == SimDebugView.Instance.GetMode())
		{
			OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
			toolActivatedViewMode = OverlayModes.None.ID;
		}
	}

	public virtual void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = null;
	}

	protected virtual void OnDeactivateTool(InterfaceTool new_tool)
	{
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		isAppFocused = focusStatus;
	}

	public virtual string GetDeactivateSound()
	{
		return "Tile_Cancel";
	}

	public virtual void OnMouseMove(Vector3 cursor_pos)
	{
		if (!(visualizer == null) && isAppFocused)
		{
			cursor_pos = Grid.CellToPosCBC(Grid.PosToCell(cursor_pos), visualizerLayer);
			cursor_pos.z += -0.15f;
			visualizer.transform.SetLocalPosition(cursor_pos);
		}
	}

	public virtual void OnKeyDown(KButtonEvent e)
	{
	}

	public virtual void OnKeyUp(KButtonEvent e)
	{
	}

	public virtual void OnLeftClickDown(Vector3 cursor_pos)
	{
	}

	public virtual void OnLeftClickUp(Vector3 cursor_pos)
	{
	}

	public virtual void OnRightClickDown(Vector3 cursor_pos, KButtonEvent e)
	{
	}

	public virtual void OnRightClickUp(Vector3 cursor_pos)
	{
	}

	public virtual void OnFocus(bool focus)
	{
		if (visualizer != null)
		{
			visualizer.SetActive(focus);
		}
		hasFocus = focus;
	}

	protected Vector2 GetRegularizedPos(Vector2 input, bool minimize)
	{
		Vector3 vector = new Vector3(Grid.HalfCellSizeInMeters, Grid.HalfCellSizeInMeters, 0f);
		return Grid.CellToPosCCC(Grid.PosToCell(input), Grid.SceneLayer.Background) + (minimize ? (-vector) : vector);
	}

	protected Vector2 GetWorldRestrictedPosition(Vector2 input)
	{
		input.x = Mathf.Clamp(input.x, ClusterManager.Instance.activeWorld.minimumBounds.x, ClusterManager.Instance.activeWorld.maximumBounds.x);
		input.y = Mathf.Clamp(input.y, ClusterManager.Instance.activeWorld.minimumBounds.y, ClusterManager.Instance.activeWorld.maximumBounds.y);
		return input;
	}

	protected void SetCursor(Texture2D new_cursor, Vector2 offset, CursorMode mode)
	{
		if (new_cursor != activeCursor)
		{
			activeCursor = new_cursor;
			Cursor.SetCursor(new_cursor, offset, mode);
		}
	}

	protected void UpdateHoverElements(List<KSelectable> hits)
	{
		if (hoverTextConfiguration != null)
		{
			hoverTextConfiguration.UpdateHoverElements(hits);
		}
	}

	public virtual void LateUpdate()
	{
		if (populateHitsList)
		{
			if (!isAppFocused || !Grid.IsValidCell(Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()))))
			{
				return;
			}
			hits.Clear();
			GetSelectablesUnderCursor(hits);
			KSelectable objectUnderCursor = GetObjectUnderCursor(cycleSelection: false, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable);
			UpdateHoverElements(hits);
			if (!hasFocus && hoverOverride == null)
			{
				ClearHover();
			}
			else if (objectUnderCursor != hover)
			{
				ClearHover();
				hover = objectUnderCursor;
				if (objectUnderCursor != null)
				{
					Game.Instance.Trigger(2095258329, objectUnderCursor.gameObject);
					objectUnderCursor.Hover(!playedSoundThisFrame);
					playedSoundThisFrame = true;
				}
			}
			playedSoundThisFrame = false;
		}
		else
		{
			UpdateHoverElements(null);
		}
	}

	public void GetSelectablesUnderCursor(List<KSelectable> hits)
	{
		if (hoverOverride != null)
		{
			hits.Add(hoverOverride);
		}
		Camera main = Camera.main;
		Vector3 position = new Vector3(KInputManager.GetMousePos().x, KInputManager.GetMousePos().y, 0f - main.transform.GetPosition().z);
		Vector3 pos = main.ScreenToWorldPoint(position);
		Vector2 pos2 = new Vector2(pos.x, pos.y);
		int cell = Grid.PosToCell(pos);
		if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
		{
			return;
		}
		Game.Instance.statusItemRenderer.GetIntersections(pos2, hits);
		ListPool<ScenePartitionerEntry, SelectTool>.PooledList pooledList = ListPool<ScenePartitionerEntry, SelectTool>.Allocate();
		GameScenePartitioner.Instance.GatherEntries((int)pos2.x, (int)pos2.y, 1, 1, GameScenePartitioner.Instance.collisionLayer, pooledList);
		pooledList.Sort((ScenePartitionerEntry x, ScenePartitionerEntry y) => SortHoverCards(x, y));
		foreach (ScenePartitionerEntry item in pooledList)
		{
			KCollider2D kCollider2D = item.obj as KCollider2D;
			if (!(kCollider2D == null) && kCollider2D.Intersects(new Vector2(pos2.x, pos2.y)))
			{
				KSelectable kSelectable = kCollider2D.GetComponent<KSelectable>();
				if (kSelectable == null)
				{
					kSelectable = kCollider2D.GetComponentInParent<KSelectable>();
				}
				if (!(kSelectable == null) && kSelectable.isActiveAndEnabled && !hits.Contains(kSelectable) && kSelectable.IsSelectable)
				{
					hits.Add(kSelectable);
				}
			}
		}
		pooledList.Recycle();
	}

	public void SetLinkCursor(bool set)
	{
		SetCursor(set ? Assets.GetTexture("cursor_hand") : cursor, set ? Vector2.zero : cursorOffset, CursorMode.Auto);
	}

	protected T GetObjectUnderCursor<T>(bool cycleSelection, Func<T, bool> condition = null, Component previous_selection = null) where T : MonoBehaviour
	{
		intersections.Clear();
		GetObjectUnderCursor2D(intersections, condition, layerMask);
		intersections.RemoveAll(is_component_null);
		if (intersections.Count <= 0)
		{
			prevIntersectionGroup.Clear();
			return null;
		}
		curIntersectionGroup.Clear();
		foreach (Intersection intersection in intersections)
		{
			curIntersectionGroup.Add(intersection.component);
		}
		if (!prevIntersectionGroup.Equals(curIntersectionGroup))
		{
			hitCycleCount = 0;
			prevIntersectionGroup = curIntersectionGroup;
		}
		intersections.Sort((Intersection a, Intersection b) => SortSelectables(a.component as KMonoBehaviour, b.component as KMonoBehaviour));
		int index = 0;
		if (cycleSelection)
		{
			index = hitCycleCount % intersections.Count;
			if (intersections[index].component != previous_selection || previous_selection == null)
			{
				index = 0;
				hitCycleCount = 0;
			}
			else
			{
				index = ++hitCycleCount % intersections.Count;
			}
		}
		return intersections[index].component as T;
	}

	private void GetObjectUnderCursor2D<T>(List<Intersection> intersections, Func<T, bool> condition, int layer_mask) where T : MonoBehaviour
	{
		Camera main = Camera.main;
		Vector3 position = new Vector3(KInputManager.GetMousePos().x, KInputManager.GetMousePos().y, 0f - main.transform.GetPosition().z);
		Vector3 pos = main.ScreenToWorldPoint(position);
		Vector2 pos2 = new Vector2(pos.x, pos.y);
		Intersection item;
		if (hoverOverride != null)
		{
			item = new Intersection
			{
				component = hoverOverride,
				distance = -100f
			};
			intersections.Add(item);
		}
		int cell = Grid.PosToCell(pos);
		if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
		{
			return;
		}
		Game.Instance.statusItemRenderer.GetIntersections(pos2, intersections);
		ListPool<ScenePartitionerEntry, SelectTool>.PooledList pooledList = ListPool<ScenePartitionerEntry, SelectTool>.Allocate();
		int x = 0;
		int y = 0;
		Grid.CellToXY(cell, out x, out y);
		GameScenePartitioner.Instance.GatherEntries(x, y, 1, 1, GameScenePartitioner.Instance.collisionLayer, pooledList);
		foreach (ScenePartitionerEntry item2 in pooledList)
		{
			KCollider2D kCollider2D = item2.obj as KCollider2D;
			if (kCollider2D == null || !kCollider2D.Intersects(new Vector2(pos.x, pos.y)))
			{
				continue;
			}
			T val = kCollider2D.GetComponent<T>();
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = kCollider2D.GetComponentInParent<T>();
			}
			if ((UnityEngine.Object)val == (UnityEngine.Object)null || ((1 << val.gameObject.layer) & layer_mask) == 0 || (UnityEngine.Object)val == (UnityEngine.Object)null || (condition != null && !condition(val)))
			{
				continue;
			}
			float num = val.transform.GetPosition().z - pos.z;
			bool flag = false;
			for (int i = 0; i < intersections.Count; i++)
			{
				Intersection value = intersections[i];
				if (value.component.gameObject == val.gameObject)
				{
					value.distance = Mathf.Min(value.distance, num);
					intersections[i] = value;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				item = new Intersection
				{
					component = val,
					distance = num
				};
				intersections.Add(item);
			}
		}
		pooledList.Recycle();
	}

	private int SortSelectables(KMonoBehaviour x, KMonoBehaviour y)
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return -1;
		}
		if (y == null)
		{
			return 1;
		}
		int num = x.transform.GetPosition().z.CompareTo(y.transform.GetPosition().z);
		if (num != 0)
		{
			return num;
		}
		return x.GetInstanceID().CompareTo(y.GetInstanceID());
	}

	public void SetHoverOverride(KSelectable hover_override)
	{
		hoverOverride = hover_override;
	}

	private int SortHoverCards(ScenePartitionerEntry x, ScenePartitionerEntry y)
	{
		KMonoBehaviour x2 = x.obj as KMonoBehaviour;
		KMonoBehaviour y2 = y.obj as KMonoBehaviour;
		return SortSelectables(x2, y2);
	}

	private static bool is_component_null(Intersection intersection)
	{
		return !intersection.component;
	}

	protected void ClearHover()
	{
		if (hover != null)
		{
			KSelectable kSelectable = hover;
			hover = null;
			kSelectable.Unhover();
			Game.Instance.Trigger(-1201923725);
		}
	}
}
