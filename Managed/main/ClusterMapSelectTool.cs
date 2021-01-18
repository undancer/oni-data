using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClusterMapSelectTool : InterfaceTool
{
	private List<KSelectable> m_hoveredSelectables = new List<KSelectable>();

	private KSelectable m_selected;

	public static ClusterMapSelectTool Instance;

	private KSelectable delayedNextSelection;

	private bool delayedSkipSound;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
		ToolMenu.Instance.PriorityScreen.ResetPriority();
		Select(null);
	}

	public KSelectable GetSelected()
	{
		return m_selected;
	}

	public override bool ShowHoverUI()
	{
		return ClusterMapScreen.Instance.HasCurrentHover();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ClearHover();
		Select(null);
	}

	private void UpdateHoveredSelectables()
	{
		m_hoveredSelectables.Clear();
		if (ClusterMapScreen.Instance.HasCurrentHover())
		{
			AxialI currentHoverLocation = ClusterMapScreen.Instance.GetCurrentHoverLocation();
			List<KSelectable> collection = (from entity in ClusterGrid.Instance.GetVisibleEntitiesAtCell(currentHoverLocation)
				select entity.GetComponent<KSelectable>() into selectable
				where selectable != null && selectable.IsSelectable
				select selectable).ToList();
			m_hoveredSelectables.AddRange(collection);
		}
	}

	public override void LateUpdate()
	{
		UpdateHoveredSelectables();
		KSelectable kSelectable = ((m_hoveredSelectables.Count > 0) ? m_hoveredSelectables[0] : null);
		UpdateHoverElements(m_hoveredSelectables);
		if (!hasFocus)
		{
			ClearHover();
		}
		else if (kSelectable != hover)
		{
			ClearHover();
			hover = kSelectable;
			if (kSelectable != null)
			{
				Game.Instance.Trigger(2095258329, kSelectable.gameObject);
				kSelectable.Hover(!playedSoundThisFrame);
				playedSoundThisFrame = true;
			}
		}
		playedSoundThisFrame = false;
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
		if (new_selected == m_selected)
		{
			return;
		}
		if (m_selected != null)
		{
			m_selected.Unselect();
		}
		GameObject gameObject = null;
		if (new_selected != null && new_selected.GetMyWorldId() == -1)
		{
			if (new_selected == hover)
			{
				ClearHover();
			}
			new_selected.Select();
			gameObject = new_selected.gameObject;
		}
		m_selected = ((gameObject == null) ? null : new_selected);
		Game.Instance.Trigger(-1503271301, gameObject);
	}
}
