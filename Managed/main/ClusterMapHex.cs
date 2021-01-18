using System.Collections.Generic;
using System.Linq;
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

	public TextStyleSetting invalidDestinationTooltipStyle;

	[MyCmpGet]
	private ToolTip m_tooltip;

	public AxialI location
	{
		get;
		private set;
	}

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

	public void SetRevealed(bool revealed)
	{
		fogOfWar.gameObject.SetActive(!revealed);
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
