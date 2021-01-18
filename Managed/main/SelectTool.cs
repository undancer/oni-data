using FMOD.Studio;
using UnityEngine;

public class SelectTool : InterfaceTool
{
	public KSelectable selected;

	protected int cell_new;

	private int selectedCell;

	protected int defaultLayerMask;

	public static SelectTool Instance;

	private KSelectable delayedNextSelection;

	private bool delayedSkipSound;

	private KSelectable previousSelection = null;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		defaultLayerMask = 1 | LayerMask.GetMask("World", "Pickupable", "Place", "PlaceWithDepth", "BlockSelection", "Construction", "Selection");
		layerMask = defaultLayerMask;
		selectMarker = Util.KInstantiateUI<SelectMarker>(EntityPrefabs.Instance.SelectMarker, GameScreenManager.Instance.worldSpaceCanvas);
		selectMarker.gameObject.SetActive(value: false);
		populateHitsList = true;
		Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
		ToolMenu.Instance.PriorityScreen.ResetPriority();
		Select(null);
	}

	public void SetLayerMask(int mask)
	{
		layerMask = mask;
		ClearHover();
		LateUpdate();
	}

	public void ClearLayerMask()
	{
		layerMask = defaultLayerMask;
	}

	public int GetDefaultLayerMask()
	{
		return defaultLayerMask;
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ClearHover();
		Select(null);
	}

	public void Focus(Vector3 pos, KSelectable selectable, Vector3 offset)
	{
		if (selectable != null)
		{
			pos = selectable.transform.GetPosition();
		}
		pos.z = -40f;
		pos += offset;
		WorldContainer worldFromPosition = ClusterManager.Instance.GetWorldFromPosition(pos);
		CameraController.Instance.ActiveWorldStarWipe(worldFromPosition.id, pos);
	}

	public void SelectAndFocus(Vector3 pos, KSelectable selectable, Vector3 offset)
	{
		Focus(pos, selectable, offset);
		Select(selectable);
	}

	public void SelectAndFocus(Vector3 pos, KSelectable selectable)
	{
		SelectAndFocus(pos, selectable, Vector3.zero);
	}

	public void SelectNextFrame(KSelectable new_selected, bool skipSound = false)
	{
		delayedNextSelection = new_selected;
		delayedSkipSound = skipSound;
		UIScheduler.Instance.ScheduleNextFrame("DelayedSelect", DoSelectNextFrame);
	}

	private void DoSelectNextFrame(object data)
	{
		Select(delayedNextSelection, delayedSkipSound);
		delayedNextSelection = null;
	}

	public void Select(KSelectable new_selected, bool skipSound = false)
	{
		if (new_selected == previousSelection)
		{
			return;
		}
		previousSelection = new_selected;
		if (selected != null)
		{
			selected.Unselect();
		}
		GameObject gameObject = null;
		if (new_selected != null && new_selected.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
		{
			SelectToolHoverTextCard component = GetComponent<SelectToolHoverTextCard>();
			if (component != null)
			{
				int currentSelectedSelectableIndex = component.currentSelectedSelectableIndex;
				int recentNumberOfDisplayedSelectables = component.recentNumberOfDisplayedSelectables;
				if (recentNumberOfDisplayedSelectables != 0)
				{
					currentSelectedSelectableIndex = (currentSelectedSelectableIndex + 1) % recentNumberOfDisplayedSelectables;
					if (!skipSound)
					{
						if (recentNumberOfDisplayedSelectables == 1)
						{
							KFMOD.PlayUISound(GlobalAssets.GetSound("Select_empty"));
						}
						else
						{
							EventInstance instance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Select_full"), Vector3.zero);
							instance.setParameterByName("selection", currentSelectedSelectableIndex);
							SoundEvent.EndOneShot(instance);
						}
						playedSoundThisFrame = true;
					}
				}
			}
			if (new_selected == hover)
			{
				ClearHover();
			}
			new_selected.Select();
			gameObject = new_selected.gameObject;
			selectMarker.SetTargetTransform(gameObject.transform);
			selectMarker.gameObject.SetActive(!new_selected.DisableSelectMarker);
		}
		else if (selectMarker != null)
		{
			selectMarker.gameObject.SetActive(value: false);
		}
		selected = ((gameObject == null) ? null : new_selected);
		Game.Instance.Trigger(-1503271301, gameObject);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		KSelectable objectUnderCursor = GetObjectUnderCursor(cycleSelection: true, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable, selected);
		selectedCell = Grid.PosToCell(cursor_pos);
		Select(objectUnderCursor);
	}

	public int GetSelectedCell()
	{
		return selectedCell;
	}
}
