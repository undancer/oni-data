using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipScreen : KScreen
{
	public GameObject ToolTipPrefab;

	public RectTransform anchorRoot;

	private GameObject toolTipWidget;

	private ToolTip prevTooltip;

	private ToolTip tooltipSetting;

	public GameObject labelPrefab;

	private GameObject multiTooltipContainer;

	public TextStyleSetting defaultTooltipHeaderStyle;

	public TextStyleSetting defaultTooltipBodyStyle;

	private bool toolTipIsBlank;

	private Vector2 ScreenEdgePadding = new Vector2(8f, 8f);

	private ToolTip dirtyHoverTooltip;

	private bool tooltipIncubating = true;

	public static ToolTipScreen Instance { get; private set; }

	protected override void OnActivate()
	{
		Instance = this;
		toolTipWidget = Util.KInstantiate(ToolTipPrefab, base.gameObject);
		toolTipWidget.transform.SetParent(base.gameObject.transform, worldPositionStays: false);
		Util.Reset(toolTipWidget.transform);
		toolTipWidget.SetActive(value: false);
	}

	protected override void OnForcedCleanUp()
	{
		Instance = null;
	}

	public void SetToolTip(ToolTip tool_tip)
	{
		tooltipSetting = tool_tip;
		multiTooltipContainer = toolTipWidget.transform.Find("MultitooltipContainer").gameObject;
		ConfigureTooltip();
	}

	private void ConfigureTooltip()
	{
		if (tooltipSetting == null)
		{
			prevTooltip = null;
		}
		if (tooltipSetting != null && dirtyHoverTooltip != null && tooltipSetting == dirtyHoverTooltip)
		{
			ClearToolTip(dirtyHoverTooltip);
		}
		if (tooltipSetting != null)
		{
			tooltipSetting.RebuildDynamicTooltip();
			if (tooltipSetting.multiStringCount == 0)
			{
				clearMultiStringTooltip();
			}
			else if (prevTooltip != tooltipSetting || !multiTooltipContainer.activeInHierarchy)
			{
				prepareMultiStringTooltip(tooltipSetting);
				prevTooltip = tooltipSetting;
			}
			bool flag = multiTooltipContainer.transform.childCount != 0;
			toolTipWidget.SetActive(flag);
			if (flag)
			{
				RectTransform rectTransform = ((!(tooltipSetting.overrideParentObject == null)) ? tooltipSetting.overrideParentObject : tooltipSetting.GetComponent<RectTransform>());
				RectTransform component = toolTipWidget.GetComponent<RectTransform>();
				component.transform.SetParent(anchorRoot.transform);
				if (!tooltipSetting.worldSpace)
				{
					anchorRoot.anchoredPosition = rectTransform.transform.GetPosition();
				}
				else
				{
					anchorRoot.anchoredPosition = WorldToScreen(rectTransform.transform.GetPosition()) + new Vector3(Screen.width / 2, Screen.height / 2, 0f);
				}
				anchorRoot.anchoredPosition -= Vector2.up * (rectTransform.rectTransform().pivot.y * rectTransform.rectTransform().sizeDelta.y);
				anchorRoot.anchoredPosition -= Vector2.right * (rectTransform.rectTransform().pivot.x * rectTransform.rectTransform().sizeDelta.x);
				anchorRoot.anchoredPosition += Vector2.right * (rectTransform.sizeDelta.x * tooltipSetting.parentPositionAnchor.x);
				anchorRoot.anchoredPosition += Vector2.up * (rectTransform.sizeDelta.y * tooltipSetting.parentPositionAnchor.y);
				float num = 1f;
				CanvasScaler component2 = base.transform.parent.GetComponent<CanvasScaler>();
				if (component2 == null)
				{
					component2 = base.transform.parent.parent.GetComponent<CanvasScaler>();
				}
				if (component2 != null)
				{
					num = component2.scaleFactor;
				}
				anchorRoot.anchoredPosition = new Vector2(anchorRoot.anchoredPosition.x / num, anchorRoot.anchoredPosition.y / num);
				component.pivot = tooltipSetting.tooltipPivot;
				Vector2 vector3 = (component.anchorMin = (component.anchorMax = new Vector2(0f, 0f)));
				component.anchoredPosition = tooltipSetting.tooltipPositionOffset * num;
				if (!tooltipSetting.worldSpace)
				{
					Rect rect = ((RectTransform)base.transform).rect;
					Vector2 vector4 = new Vector2(base.transform.GetPosition().x, base.transform.GetPosition().y) + ScreenEdgePadding;
					Vector2 vector5 = new Vector2(base.transform.GetPosition().x, base.transform.GetPosition().y) + rect.width * Vector2.right + rect.height * Vector2.up - ScreenEdgePadding * Mathf.Max(1f, num);
					vector5.x *= num;
					vector5.y *= num;
					Vector2 vector6 = default(Vector2);
					vector6.x = component.GetPosition().x - component.pivot.x * (component.sizeDelta.x * num);
					vector6.y = component.GetPosition().y - component.pivot.y * (component.sizeDelta.y * num);
					Vector2 vector7 = default(Vector2);
					vector7.x = component.GetPosition().x + (1f - component.pivot.x) * (component.sizeDelta.x * num);
					vector7.y = component.GetPosition().y + (1f - component.pivot.y) * (component.sizeDelta.y * num);
					Vector2 zero = Vector2.zero;
					if (vector6.x < vector4.x)
					{
						zero.x = vector4.x - vector6.x;
					}
					if (vector7.x > vector5.x)
					{
						zero.x = vector5.x - vector7.x;
					}
					if (vector6.y < vector4.y)
					{
						zero.y = vector4.y - vector6.y;
					}
					if (vector7.y > vector5.y)
					{
						zero.y = vector5.y - vector7.y;
					}
					zero /= num;
					component.anchoredPosition += zero;
				}
			}
		}
		if (((RectTransform)base.transform).GetSiblingIndex() != base.transform.parent.childCount - 1)
		{
			((RectTransform)base.transform).SetAsLastSibling();
		}
	}

	private void prepareMultiStringTooltip(ToolTip setting)
	{
		int multiStringCount = tooltipSetting.multiStringCount;
		clearMultiStringTooltip();
		for (int i = 0; i < multiStringCount; i++)
		{
			Util.KInstantiateUI(labelPrefab, null, force_active: true).transform.SetParent(multiTooltipContainer.transform);
		}
		for (int j = 0; j < tooltipSetting.multiStringCount; j++)
		{
			Transform child = multiTooltipContainer.transform.GetChild(j);
			LayoutElement component = child.GetComponent<LayoutElement>();
			TextMeshProUGUI component2 = child.GetComponent<TextMeshProUGUI>();
			component2.text = tooltipSetting.GetMultiString(j);
			child.GetComponent<SetTextStyleSetting>().SetStyle(tooltipSetting.GetStyleSetting(j));
			if (setting.SizingSetting == ToolTip.ToolTipSizeSetting.MaxWidthWrapContent)
			{
				float num2 = (component.minWidth = (component.preferredWidth = setting.WrapWidth));
				component.rectTransform().sizeDelta = new Vector2(setting.WrapWidth, 1000f);
				num2 = (component.minHeight = (component.preferredHeight = component2.preferredHeight));
				num2 = (component.minHeight = (component.preferredHeight = component2.preferredHeight));
				component.rectTransform().sizeDelta = new Vector2(setting.WrapWidth, component.minHeight);
				GetComponentInChildren<ContentSizeFitter>(includeInactive: true).horizontalFit = ContentSizeFitter.FitMode.MinSize;
				multiTooltipContainer.GetComponent<LayoutElement>().minWidth = setting.WrapWidth + 2f * ScreenEdgePadding.x;
			}
			else if (setting.SizingSetting == ToolTip.ToolTipSizeSetting.DynamicWidthNoWrap)
			{
				GetComponentInChildren<ContentSizeFitter>(includeInactive: true).horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
				Vector2 preferredValues = component2.GetPreferredValues();
				LayoutElement component3 = multiTooltipContainer.GetComponent<LayoutElement>();
				float num5 = (component.preferredWidth = preferredValues.x);
				float num2 = (component3.minWidth = (component.minWidth = num5));
				num2 = (component.minHeight = (component.preferredHeight = preferredValues.y));
				GetComponentInChildren<ContentSizeFitter>(includeInactive: true).SetLayoutHorizontal();
				GetComponentInChildren<ContentSizeFitter>(includeInactive: true).SetLayoutVertical();
				multiTooltipContainer.rectTransform().sizeDelta = new Vector2(component.minWidth, component.minHeight);
				multiTooltipContainer.transform.parent.rectTransform().sizeDelta = multiTooltipContainer.rectTransform().sizeDelta;
			}
			component2.ForceMeshUpdate();
		}
		tooltipIncubating = true;
	}

	private void Update()
	{
		if (tooltipSetting != null)
		{
			tooltipSetting.UpdateWhileHovered();
		}
		if (multiTooltipContainer == null || anchorRoot == null)
		{
			return;
		}
		if (dirtyHoverTooltip != null)
		{
			ToolTip tt = dirtyHoverTooltip;
			MakeDirtyTooltipClean(tt);
			ClearToolTip(tt);
		}
		if (tooltipIncubating)
		{
			tooltipIncubating = false;
			if (anchorRoot.GetComponentInChildren<Image>() != null)
			{
				anchorRoot.GetComponentInChildren<Image>(includeInactive: true).enabled = false;
			}
			multiTooltipContainer.transform.localScale = Vector3.zero;
			toolTipIsBlank = true;
			for (int i = 0; i < multiTooltipContainer.transform.childCount; i++)
			{
				if (multiTooltipContainer.transform.GetChild(i).transform.localScale != Vector3.one)
				{
					multiTooltipContainer.transform.GetChild(i).transform.localScale = Vector3.one;
				}
				LayoutElement component = multiTooltipContainer.transform.GetChild(i).GetComponent<LayoutElement>();
				TextMeshProUGUI component2 = component.GetComponent<TextMeshProUGUI>();
				toolTipIsBlank = component2.text == "" && toolTipIsBlank;
				if (component.minHeight != component2.preferredHeight)
				{
					component.minHeight = component2.preferredHeight;
				}
			}
		}
		else if (multiTooltipContainer.transform.localScale != Vector3.one && !toolTipIsBlank)
		{
			if (anchorRoot.GetComponentInChildren<Image>() != null)
			{
				anchorRoot.GetComponentInChildren<Image>(includeInactive: true).enabled = true;
			}
			multiTooltipContainer.transform.localScale = Vector3.one;
		}
	}

	public void HotSwapTooltipString(string newString, int lineIndex)
	{
		if (multiTooltipContainer.transform.childCount > lineIndex)
		{
			multiTooltipContainer.transform.GetChild(lineIndex).GetComponent<TextMeshProUGUI>().text = newString;
		}
	}

	private void clearMultiStringTooltip()
	{
		for (int num = multiTooltipContainer.transform.childCount - 1; num >= 0; num--)
		{
			Object.DestroyImmediate(multiTooltipContainer.transform.GetChild(num).gameObject);
		}
	}

	public void ClearToolTip(ToolTip tt)
	{
		if (tt == tooltipSetting)
		{
			tooltipSetting = null;
			if (toolTipWidget != null)
			{
				clearMultiStringTooltip();
				toolTipWidget.SetActive(value: false);
			}
		}
	}

	public void MarkTooltipDirty(ToolTip tt)
	{
		if (tt == tooltipSetting)
		{
			dirtyHoverTooltip = tt;
		}
	}

	public void MakeDirtyTooltipClean(ToolTip tt)
	{
		if (tt == dirtyHoverTooltip)
		{
			dirtyHoverTooltip = null;
		}
	}
}
