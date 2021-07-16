using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/Plugins/KTabMenuHeader")]
public class KTabMenuHeader : KMonoBehaviour
{
	public delegate void OnClick(int id);

	[SerializeField]
	private RectTransform prefab;

	public TextStyleSetting TextStyle_Active;

	public TextStyleSetting TextStyle_Inactive;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ActivateTabArtwork(0);
	}

	public void Add(string name, OnClick onClick, int id)
	{
		GameObject gameObject = Util.KInstantiateUI(prefab.gameObject);
		gameObject.SetActive(value: true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.transform.SetParent(base.transform, worldPositionStays: false);
		component.name = name;
		Text componentInChildren = component.GetComponentInChildren<Text>();
		if (componentInChildren != null)
		{
			componentInChildren.text = name.ToUpper();
		}
		ActivateTabArtwork(id);
		gameObject.GetComponent<KButton>().onClick += delegate
		{
			onClick(id);
		};
	}

	public void Add(Sprite icon, string name, OnClick onClick, int id, string tooltip = "")
	{
		GameObject gameObject = Util.KInstantiateUI(prefab.gameObject);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.transform.SetParent(base.transform, worldPositionStays: false);
		component.name = name;
		if (tooltip == "")
		{
			component.GetComponent<ToolTip>().toolTip = name;
		}
		else
		{
			component.GetComponent<ToolTip>().toolTip = tooltip;
		}
		ActivateTabArtwork(id);
		TabHeaderIcon componentInChildren = component.GetComponentInChildren<TabHeaderIcon>();
		if ((bool)componentInChildren)
		{
			componentInChildren.TitleText.text = name;
		}
		KToggle component2 = gameObject.GetComponent<KToggle>();
		if ((bool)component2 && (bool)component2.fgImage)
		{
			component2.fgImage.sprite = icon;
		}
		component2.group = GetComponent<ToggleGroup>();
		component2.onClick += delegate
		{
			onClick(id);
		};
	}

	public void Activate(int itemIdx, int previouslyActiveTabIdx)
	{
		int childCount = base.transform.childCount;
		if (itemIdx >= childCount)
		{
			return;
		}
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (!child.gameObject.activeSelf)
			{
				continue;
			}
			KButton componentInChildren = child.GetComponentInChildren<KButton>();
			if (componentInChildren != null && componentInChildren.GetComponentInChildren<Text>() != null && i == itemIdx)
			{
				ActivateTabArtwork(itemIdx);
			}
			KToggle component = child.GetComponent<KToggle>();
			if (component != null)
			{
				ActivateTabArtwork(itemIdx);
				if (i == itemIdx)
				{
					component.Select();
				}
				else
				{
					component.Deselect();
				}
			}
		}
	}

	public void SetTabEnabled(int tabIdx, bool enabled)
	{
		if (tabIdx < base.transform.childCount)
		{
			base.transform.GetChild(tabIdx).gameObject.SetActive(enabled);
		}
	}

	public virtual void ActivateTabArtwork(int tabIdx)
	{
		if (tabIdx >= base.transform.childCount)
		{
			return;
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			ImageToggleState component = base.transform.GetChild(i).GetComponent<ImageToggleState>();
			if (component != null)
			{
				if (i == tabIdx)
				{
					component.SetActive();
				}
				else
				{
					component.SetInactive();
				}
			}
			Canvas componentInChildren = base.transform.GetChild(i).GetComponentInChildren<Canvas>(includeInactive: true);
			if (componentInChildren != null)
			{
				componentInChildren.overrideSorting = tabIdx == i;
			}
			SetTextStyleSetting componentInChildren2 = base.transform.GetChild(i).GetComponentInChildren<SetTextStyleSetting>();
			if (componentInChildren2 != null && TextStyle_Active != null && TextStyle_Inactive != null)
			{
				if (i == tabIdx)
				{
					componentInChildren2.SetStyle(TextStyle_Active);
				}
				else
				{
					componentInChildren2.SetStyle(TextStyle_Inactive);
				}
			}
		}
	}
}
