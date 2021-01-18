using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KToggleMenu : KScreen
{
	public delegate void OnSelect(ToggleInfo toggleInfo);

	public class ToggleInfo
	{
		public string text;

		public object userData;

		public KToggle toggle;

		public Action hotKey;

		public ToggleInfo(string text, object user_data = null, Action hotKey = Action.NumActions)
		{
			this.text = text;
			userData = user_data;
			this.hotKey = hotKey;
		}
	}

	[SerializeField]
	private Transform toggleParent;

	[SerializeField]
	private KToggle prefab;

	[SerializeField]
	private ToggleGroup group;

	protected IList<ToggleInfo> toggleInfo;

	protected List<KToggle> toggles = new List<KToggle>();

	private static int selected = -1;

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

	private void RefreshButtons()
	{
		foreach (KToggle toggle in toggles)
		{
			if (toggle != null)
			{
				Object.Destroy(toggle.gameObject);
			}
		}
		toggles.Clear();
		if (this.toggleInfo == null)
		{
			return;
		}
		Transform parent = ((toggleParent != null) ? toggleParent : base.transform);
		for (int i = 0; i < this.toggleInfo.Count; i++)
		{
			int idx = i;
			ToggleInfo toggleInfo = this.toggleInfo[i];
			if (toggleInfo == null)
			{
				toggles.Add(null);
				continue;
			}
			KToggle kToggle = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
			kToggle.gameObject.name = "Toggle:" + toggleInfo.text;
			kToggle.transform.SetParent(parent, worldPositionStays: false);
			kToggle.group = group;
			kToggle.onClick += delegate
			{
				OnClick(idx);
			};
			kToggle.GetComponentsInChildren<Text>(includeInactive: true)[0].text = toggleInfo.text;
			toggleInfo.toggle = kToggle;
			toggles.Add(kToggle);
		}
	}

	public int GetSelected()
	{
		return selected;
	}

	private void OnClick(int i)
	{
		UISounds.PlaySound(UISounds.Sound.ClickObject);
		if (this.onSelect != null)
		{
			this.onSelect(toggleInfo[i]);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (toggles == null)
		{
			return;
		}
		for (int i = 0; i < toggleInfo.Count; i++)
		{
			Action hotKey = toggleInfo[i].hotKey;
			if (hotKey != Action.NumActions && e.TryConsume(hotKey))
			{
				toggles[i].Click();
				break;
			}
		}
	}
}
