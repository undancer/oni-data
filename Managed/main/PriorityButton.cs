using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/PriorityButton")]
public class PriorityButton : KMonoBehaviour
{
	public KToggle toggle;

	public LocText text;

	public ToolTip tooltip;

	[MyCmpGet]
	private ImageToggleState its;

	public ColorStyleSetting normalStyle;

	public ColorStyleSetting highStyle;

	public bool playSelectionSound = true;

	public Action<PrioritySetting> onClick;

	private PrioritySetting _priority;

	public PrioritySetting priority
	{
		get
		{
			return _priority;
		}
		set
		{
			_priority = value;
			if (its != null)
			{
				if (priority.priority_class == PriorityScreen.PriorityClass.high)
				{
					its.colorStyleSetting = highStyle;
				}
				else
				{
					its.colorStyleSetting = normalStyle;
				}
				its.RefreshColorStyle();
				its.ResetColor();
			}
		}
	}

	protected override void OnPrefabInit()
	{
		toggle.onClick += OnClick;
	}

	private void OnClick()
	{
		if (playSelectionSound)
		{
			PriorityScreen.PlayPriorityConfirmSound(priority);
		}
		if (onClick != null)
		{
			onClick(priority);
		}
	}
}
