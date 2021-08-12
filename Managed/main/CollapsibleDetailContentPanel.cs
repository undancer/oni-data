using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/CollapsibleDetailContentPanel")]
public class CollapsibleDetailContentPanel : KMonoBehaviour
{
	private class Label<T>
	{
		public T obj;

		public bool used;
	}

	[Serializable]
	public struct PanelColors
	{
		public Color FrameColor;

		public Color FrameColor_Hover;

		public Color FrameColor_Press;

		public Color ArrowColor;

		public Color TextColor;
	}

	public ImageToggleState ArrowIcon;

	public LocText HeaderLabel;

	public Button CollapseButton;

	public Transform Content;

	public ScalerMask scalerMask;

	[Space(10f)]
	public DetailLabel labelTemplate;

	public DetailLabelWithButton labelWithActionButtonTemplate;

	private Dictionary<string, Label<DetailLabel>> labels;

	private Dictionary<string, Label<DetailLabelWithButton>> buttonLabels;

	private LoggerFSS log;

	public PanelColors colors;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		CollapseButton.onClick.AddListener(ToggleOpen);
		SetColors(colors);
		ArrowIcon.SetActive();
		log = new LoggerFSS("detailpanel");
		labels = new Dictionary<string, Label<DetailLabel>>();
		buttonLabels = new Dictionary<string, Label<DetailLabelWithButton>>();
		Commit();
	}

	public void SetTitle(string title)
	{
		HeaderLabel.text = title;
	}

	public void Commit()
	{
		int num = 0;
		foreach (Label<DetailLabel> value in labels.Values)
		{
			if (value.used)
			{
				num++;
				if (!value.obj.gameObject.activeSelf)
				{
					value.obj.gameObject.SetActive(value: true);
				}
			}
			else if (!value.used && value.obj.gameObject.activeSelf)
			{
				value.obj.gameObject.SetActive(value: false);
			}
			value.used = false;
		}
		foreach (Label<DetailLabelWithButton> value2 in buttonLabels.Values)
		{
			if (value2.used)
			{
				num++;
				if (!value2.obj.gameObject.activeSelf)
				{
					value2.obj.gameObject.SetActive(value: true);
				}
			}
			else if (!value2.used && value2.obj.gameObject.activeSelf)
			{
				value2.obj.gameObject.SetActive(value: false);
			}
			value2.used = false;
		}
		if (base.gameObject.activeSelf && num == 0)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (!base.gameObject.activeSelf && num > 0)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	public void SetLabel(string id, string text, string tooltip)
	{
		if (!labels.TryGetValue(id, out var value))
		{
			value = new Label<DetailLabel>
			{
				used = true,
				obj = Util.KInstantiateUI(labelTemplate.gameObject, Content.gameObject).GetComponent<DetailLabel>()
			};
			value.obj.gameObject.name = id;
			labels[id] = value;
		}
		value.obj.label.AllowLinks = true;
		value.obj.label.text = text;
		value.obj.toolTip.toolTip = tooltip;
		value.used = true;
	}

	public void SetLabelWithButton(string id, string text, string tooltip, string buttonText, string buttonTooltip, System.Action buttonCb)
	{
		if (!buttonLabels.TryGetValue(id, out var value))
		{
			value = new Label<DetailLabelWithButton>
			{
				used = true,
				obj = Util.KInstantiateUI(labelWithActionButtonTemplate.gameObject, Content.gameObject).GetComponent<DetailLabelWithButton>()
			};
			value.obj.gameObject.name = id;
			buttonLabels[id] = value;
		}
		value.obj.label.AllowLinks = true;
		value.obj.label.text = text;
		value.obj.toolTip.toolTip = tooltip;
		value.obj.buttonLabel.text = buttonText;
		value.obj.buttonToolTip.toolTip = buttonTooltip;
		value.obj.button.ClearOnClick();
		value.obj.button.onClick += buttonCb;
		value.used = true;
	}

	public void SetColors(PanelColors newColors)
	{
		colors = newColors;
		HeaderLabel.color = colors.TextColor;
		ArrowIcon.ActiveColour = colors.ArrowColor;
		ArrowIcon.InactiveColour = colors.ArrowColor;
		CollapseButton.transition = Selectable.Transition.None;
		ColorBlock colorBlock = default(ColorBlock);
		colorBlock.normalColor = new Color(colors.FrameColor.r, colors.FrameColor.g, colors.FrameColor.b, colors.FrameColor.a);
		colorBlock.highlightedColor = colors.FrameColor_Hover;
		colorBlock.pressedColor = colors.FrameColor_Press;
		colorBlock.disabledColor = colorBlock.normalColor;
		colorBlock.colorMultiplier = 1f;
		CollapseButton.colors = colorBlock;
		CollapseButton.transition = Selectable.Transition.ColorTint;
	}

	private void ToggleOpen()
	{
		bool activeSelf = scalerMask.gameObject.activeSelf;
		activeSelf = !activeSelf;
		scalerMask.gameObject.SetActive(activeSelf);
		if (activeSelf)
		{
			ArrowIcon.SetActive();
			ForceLocTextsMeshRebuild();
		}
		else
		{
			ArrowIcon.SetInactive();
		}
	}

	public void SetCollapsible(bool bCollapsible)
	{
		ArrowIcon.gameObject.SetActive(bCollapsible);
		CollapseButton.interactable = bCollapsible;
	}

	public void ForceLocTextsMeshRebuild()
	{
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ForceMeshUpdate();
		}
	}
}
