using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMenu
{
	public class SliderInfo
	{
		public MinMaxSlider.LockingType lockType = MinMaxSlider.LockingType.Drag;

		public MinMaxSlider.Mode mode;

		public Slider.Direction direction;

		public bool interactable = true;

		public bool lockRange;

		public string toolTip;

		public string toolTipMin;

		public string toolTipMax;

		public float minLimit;

		public float maxLimit = 100f;

		public float currentMinValue = 10f;

		public float currentMaxValue = 90f;

		public GameObject sliderGO;

		public Action<MinMaxSlider> onMinChange;

		public Action<MinMaxSlider> onMaxChange;
	}

	public const float DECONSTRUCT_PRIORITY = 0f;

	public const float DRAWPATHS_PRIORITY = 0.1f;

	public const float FOLLOWCAM_PRIORITY = 0.3f;

	public const float SETDIRECTION_PRIORITY = 0.4f;

	public const float AUTOBOTTLE_PRIORITY = 0.4f;

	public const float AUTOREPAIR_PRIORITY = 0.5f;

	public const float DEFAULT_PRIORITY = 1f;

	public const float SUITEQUIP_PRIORITY = 2f;

	public const float AUTODISINFECT_PRIORITY = 10f;

	public const float ROCKETUSAGERESTRICTION_PRIORITY = 11f;

	private List<KeyValuePair<KIconButtonMenu.ButtonInfo, float>> buttons = new List<KeyValuePair<KIconButtonMenu.ButtonInfo, float>>();

	private List<SliderInfo> sliders = new List<SliderInfo>();

	private List<KIconButtonMenu.ButtonInfo> sortedButtons = new List<KIconButtonMenu.ButtonInfo>();

	public void Refresh(GameObject go)
	{
		Game.Instance.Trigger(1980521255, go);
	}

	public void AddButton(GameObject go, KIconButtonMenu.ButtonInfo button, float sort_order = 1f)
	{
		if (button.onClick != null)
		{
			System.Action callback = button.onClick;
			button.onClick = delegate
			{
				callback();
				Game.Instance.Trigger(1980521255, go);
			};
		}
		buttons.Add(new KeyValuePair<KIconButtonMenu.ButtonInfo, float>(button, sort_order));
	}

	public void AddSlider(GameObject go, SliderInfo slider)
	{
		sliders.Add(slider);
	}

	public void AppendToScreen(GameObject go, UserMenuScreen screen)
	{
		buttons.Clear();
		sliders.Clear();
		go.Trigger(493375141);
		if (buttons.Count > 0)
		{
			buttons.Sort(delegate(KeyValuePair<KIconButtonMenu.ButtonInfo, float> x, KeyValuePair<KIconButtonMenu.ButtonInfo, float> y)
			{
				if (x.Value == y.Value)
				{
					return 0;
				}
				return (x.Value > y.Value) ? 1 : (-1);
			});
			for (int i = 0; i < buttons.Count; i++)
			{
				sortedButtons.Add(buttons[i].Key);
			}
			screen.AddButtons(sortedButtons);
			sortedButtons.Clear();
		}
		if (sliders.Count > 0)
		{
			screen.AddSliders(sliders);
		}
	}
}
