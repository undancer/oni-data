using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KIconToggleMenu : KScreen
{
	public delegate void OnSelect(ToggleInfo toggleInfo);

	public class ToggleInfo
	{
		public string text;

		public object userData;

		public string icon;

		public string tooltip;

		public string tooltipHeader;

		public KToggle toggle;

		public Action hotKey;

		public ToolTip.ComplexTooltipDelegate getTooltipText;

		public Func<Sprite> getSpriteCB;

		public KToggle prefabOverride;

		public KToggle instanceOverride;

		public ToggleInfo(string text, string icon, object user_data = null, Action hotkey = Action.NumActions, string tooltip = "", string tooltip_header = "")
		{
			this.text = text;
			userData = user_data;
			this.icon = icon;
			hotKey = hotkey;
			this.tooltip = tooltip;
			tooltipHeader = tooltip_header;
			getTooltipText = DefaultGetTooltipText;
		}

		public ToggleInfo(string text, object user_data, Action hotkey, Func<Sprite> get_sprite_cb)
		{
			this.text = text;
			userData = user_data;
			hotKey = hotkey;
			getSpriteCB = get_sprite_cb;
		}

		public virtual void SetToggle(KToggle toggle)
		{
			this.toggle = toggle;
			toggle.GetComponent<ToolTip>().OnComplexToolTip = getTooltipText;
		}

		protected virtual List<Tuple<string, TextStyleSetting>> DefaultGetTooltipText()
		{
			List<Tuple<string, TextStyleSetting>> list = new List<Tuple<string, TextStyleSetting>>();
			if (tooltipHeader != null)
			{
				list.Add(new Tuple<string, TextStyleSetting>(tooltipHeader, ToolTipScreen.Instance.defaultTooltipHeaderStyle));
			}
			list.Add(new Tuple<string, TextStyleSetting>(tooltip, ToolTipScreen.Instance.defaultTooltipBodyStyle));
			return list;
		}
	}

	[SerializeField]
	private Transform toggleParent;

	[SerializeField]
	private KToggle prefab;

	[SerializeField]
	private ToggleGroup group;

	[SerializeField]
	private Sprite[] icons;

	[SerializeField]
	public TextStyleSetting ToggleToolTipTextStyleSetting;

	[SerializeField]
	public TextStyleSetting ToggleToolTipHeaderTextStyleSetting;

	[SerializeField]
	protected bool repeatKeyDownToggles = true;

	protected KToggle currentlySelectedToggle;

	protected IList<ToggleInfo> toggleInfo;

	protected List<KToggle> toggles = new List<KToggle>();

	private List<KToggle> dontDestroyToggles = new List<KToggle>();

	protected int selected = -1;

	public event OnSelect onSelect;

	public void Setup(IList<ToggleInfo> toggleInfo)
	{
		this.toggleInfo = toggleInfo;
		RefreshButtons();
	}

	protected void Setup()
	{
		RefreshButtons();
	}

	protected virtual void RefreshButtons()
	{
		foreach (KToggle toggle in toggles)
		{
			if (toggle != null)
			{
				if (!dontDestroyToggles.Contains(toggle))
				{
					UnityEngine.Object.Destroy(toggle.gameObject);
				}
				else
				{
					toggle.ClearOnClick();
				}
			}
		}
		toggles.Clear();
		dontDestroyToggles.Clear();
		if (this.toggleInfo == null)
		{
			return;
		}
		Transform transform = ((toggleParent != null) ? toggleParent : base.transform);
		for (int i = 0; i < this.toggleInfo.Count; i++)
		{
			int idx = i;
			ToggleInfo toggleInfo = this.toggleInfo[i];
			KToggle kToggle;
			if (!(toggleInfo.instanceOverride != null))
			{
				kToggle = ((!toggleInfo.prefabOverride) ? Util.KInstantiateUI<KToggle>(prefab.gameObject, transform.gameObject, force_active: true) : Util.KInstantiateUI<KToggle>(toggleInfo.prefabOverride.gameObject, transform.gameObject, force_active: true));
			}
			else
			{
				kToggle = toggleInfo.instanceOverride;
				dontDestroyToggles.Add(kToggle);
			}
			kToggle.Deselect();
			kToggle.gameObject.name = "Toggle:" + toggleInfo.text;
			kToggle.group = group;
			kToggle.onClick += delegate
			{
				OnClick(idx);
			};
			Transform transform2 = kToggle.transform.Find("Text");
			if (transform2 != null)
			{
				LocText component = transform2.GetComponent<LocText>();
				if (component != null)
				{
					component.text = toggleInfo.text;
				}
			}
			if (toggleInfo.getSpriteCB != null)
			{
				kToggle.fgImage.sprite = toggleInfo.getSpriteCB();
			}
			else if (toggleInfo.icon != null)
			{
				kToggle.fgImage.sprite = Assets.GetSprite(toggleInfo.icon);
			}
			toggleInfo.SetToggle(kToggle);
			toggles.Add(kToggle);
		}
	}

	public Sprite GetIcon(string name)
	{
		Sprite[] array = icons;
		foreach (Sprite sprite in array)
		{
			if (sprite.name == name)
			{
				return sprite;
			}
		}
		return null;
	}

	public virtual void ClearSelection()
	{
		if (toggles == null)
		{
			return;
		}
		foreach (KToggle toggle in toggles)
		{
			toggle.Deselect();
			toggle.ClearAnimState();
		}
		selected = -1;
	}

	private void OnClick(int i)
	{
		if (this.onSelect == null)
		{
			return;
		}
		selected = i;
		this.onSelect(toggleInfo[i]);
		if (!toggles[i].isOn)
		{
			selected = -1;
		}
		for (int j = 0; j < toggles.Count; j++)
		{
			if (j != selected)
			{
				toggles[j].isOn = false;
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (toggles == null || toggleInfo == null)
		{
			return;
		}
		for (int i = 0; i < toggleInfo.Count; i++)
		{
			if (!toggles[i].isActiveAndEnabled)
			{
				continue;
			}
			Action hotKey = toggleInfo[i].hotKey;
			if (hotKey == Action.NumActions || !e.TryConsume(hotKey))
			{
				continue;
			}
			if (selected != i || repeatKeyDownToggles)
			{
				toggles[i].Click();
				if (selected == i)
				{
					toggles[i].Deselect();
				}
				selected = i;
			}
			break;
		}
	}

	public virtual void Close()
	{
		ClearSelection();
		Show(show: false);
	}
}
