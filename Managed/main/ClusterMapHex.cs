using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ClusterMapHex : MultiToggle, ICanvasRaycastFilter
{
	public enum ToggleState
	{
		Unselected,
		Selected,
		OrbitHighlight
	}

	private RectTransform rectTransform;

	public Color hoverColorValid;

	public Color hoverColorInvalid;

	public Image fogOfWar;

	public Image peekedTile;

	public TextStyleSetting invalidDestinationTooltipStyle;

	public TextStyleSetting informationTooltipStyle;

	[MyCmpGet]
	private ToolTip m_tooltip;

	private ClusterRevealLevel _revealLevel;

	public AxialI location { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		rectTransform = GetComponent<RectTransform>();
		onClick = TrySelect;
		onDoubleClick = TryGoTo;
		onEnter = OnHover;
		onExit = OnUnhover;
	}

	public void SetLocation(AxialI location)
	{
		this.location = location;
	}

	public void SetRevealed(ClusterRevealLevel level)
	{
		_revealLevel = level;
		switch (level)
		{
		case ClusterRevealLevel.Hidden:
			fogOfWar.gameObject.SetActive(value: true);
			peekedTile.gameObject.SetActive(value: false);
			break;
		case ClusterRevealLevel.Peeked:
			fogOfWar.gameObject.SetActive(value: false);
			peekedTile.gameObject.SetActive(value: true);
			break;
		case ClusterRevealLevel.Visible:
			fogOfWar.gameObject.SetActive(value: false);
			peekedTile.gameObject.SetActive(value: false);
			break;
		}
	}

	public void SetDestinationStatus(string fail_reason)
	{
		m_tooltip.ClearMultiStringTooltip();
		UpdateHoverColors(string.IsNullOrEmpty(fail_reason));
		if (!string.IsNullOrEmpty(fail_reason))
		{
			m_tooltip.AddMultiStringTooltip(fail_reason, invalidDestinationTooltipStyle);
		}
	}

	public void SetDestinationStatus(string fail_reason, int pathLength, int rocketRange, bool repeat)
	{
		m_tooltip.ClearMultiStringTooltip();
		if (pathLength > 0)
		{
			string format = (repeat ? UI.CLUSTERMAP.TOOLTIP_PATH_LENGTH_RETURN : UI.CLUSTERMAP.TOOLTIP_PATH_LENGTH);
			if (repeat)
			{
				pathLength *= 2;
			}
			format = string.Format(format, pathLength, GameUtil.GetFormattedRocketRange(rocketRange, GameUtil.TimeSlice.None));
			m_tooltip.AddMultiStringTooltip(format, informationTooltipStyle);
		}
		UpdateHoverColors(string.IsNullOrEmpty(fail_reason));
		if (!string.IsNullOrEmpty(fail_reason))
		{
			m_tooltip.AddMultiStringTooltip(fail_reason, invalidDestinationTooltipStyle);
		}
	}

	public void UpdateToggleState(ToggleState state)
	{
		int new_state_index = -1;
		switch (state)
		{
		case ToggleState.Unselected:
			new_state_index = 0;
			break;
		case ToggleState.Selected:
			new_state_index = 1;
			break;
		case ToggleState.OrbitHighlight:
			new_state_index = 2;
			break;
		}
		ChangeState(new_state_index);
	}

	private void TrySelect()
	{
		if (DebugHandler.InstantBuildMode)
		{
			SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(location);
		}
		ClusterMapScreen.Instance.SelectHex(this);
	}

	private bool TryGoTo()
	{
		List<WorldContainer> list = (from entity in ClusterGrid.Instance.GetVisibleEntitiesAtCell(location)
			select entity.GetComponent<WorldContainer>() into x
			where x != null
			select x).ToList();
		if (list.Count == 1)
		{
			CameraController.Instance.ActiveWorldStarWipe(list[0].id);
			return true;
		}
		return false;
	}

	private void OnHover()
	{
		m_tooltip.ClearMultiStringTooltip();
		string text = "";
		switch (_revealLevel)
		{
		case ClusterRevealLevel.Hidden:
			text = UI.CLUSTERMAP.TOOLTIP_HIDDEN_HEX;
			break;
		case ClusterRevealLevel.Peeked:
		{
			List<ClusterGridEntity> hiddenEntitiesOfLayerAtCell = ClusterGrid.Instance.GetHiddenEntitiesOfLayerAtCell(location, EntityLayer.Asteroid);
			List<ClusterGridEntity> hiddenEntitiesOfLayerAtCell2 = ClusterGrid.Instance.GetHiddenEntitiesOfLayerAtCell(location, EntityLayer.POI);
			text = ((hiddenEntitiesOfLayerAtCell.Count > 0 || hiddenEntitiesOfLayerAtCell2.Count > 0) ? UI.CLUSTERMAP.TOOLTIP_PEEKED_HEX_WITH_OBJECT : UI.CLUSTERMAP.TOOLTIP_HIDDEN_HEX);
			break;
		}
		case ClusterRevealLevel.Visible:
			if (ClusterGrid.Instance.GetEntitiesOnCell(location).Count == 0)
			{
				text = UI.CLUSTERMAP.TOOLTIP_EMPTY_HEX;
			}
			break;
		}
		if (!text.IsNullOrWhiteSpace())
		{
			m_tooltip.AddMultiStringTooltip(text, informationTooltipStyle);
		}
		UpdateHoverColors(validDestination: true);
		ClusterMapScreen.Instance.OnHoverHex(this);
	}

	private void OnUnhover()
	{
		ClusterMapScreen.Instance.OnUnhoverHex(this);
	}

	private void UpdateHoverColors(bool validDestination)
	{
		Color color_on_hover = (validDestination ? hoverColorValid : hoverColorInvalid);
		for (int i = 0; i < states.Length; i++)
		{
			states[i].color_on_hover = color_on_hover;
			for (int j = 0; j < states[i].additional_display_settings.Length; j++)
			{
				states[i].additional_display_settings[j].color_on_hover = color_on_hover;
			}
		}
		RefreshHoverColor();
	}

	public bool IsRaycastLocationValid(Vector2 inputPoint, Camera eventCamera)
	{
		Vector2 vector = rectTransform.position;
		float num = Mathf.Abs(inputPoint.x - vector.x);
		float num2 = Mathf.Abs(inputPoint.y - vector.y);
		Vector2 vector2 = rectTransform.lossyScale;
		if (num > vector2.x || num2 > vector2.y)
		{
			return false;
		}
		return vector2.y * vector2.x - vector2.y / 2f * num - vector2.x * num2 >= 0f;
	}
}
