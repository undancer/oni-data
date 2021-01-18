using System.Collections.Generic;
using UnityEngine;

public class CodexTextWithTooltip : CodexWidget<CodexTextWithTooltip>
{
	public string text
	{
		get;
		set;
	}

	public string tooltip
	{
		get;
		set;
	}

	public CodexTextStyle style
	{
		get;
		set;
	}

	public string stringKey
	{
		get
		{
			return "--> " + (text ?? "NULL");
		}
		set
		{
			text = Strings.Get(value);
		}
	}

	public CodexTextWithTooltip()
	{
		style = CodexTextStyle.Body;
	}

	public CodexTextWithTooltip(string text, string tooltip, CodexTextStyle style = CodexTextStyle.Body)
	{
		this.text = text;
		this.style = style;
		this.tooltip = tooltip;
	}

	public void ConfigureLabel(LocText label, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		label.gameObject.SetActive(value: true);
		label.AllowLinks = style == CodexTextStyle.Body;
		label.textStyleSetting = textStyles[style];
		label.text = text;
		label.ApplySettings();
	}

	public void ConfigureTooltip(ToolTip tooltip)
	{
		tooltip.SetSimpleTooltip(this.tooltip);
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		ConfigureLabel(contentGameObject.GetComponent<LocText>(), textStyles);
		ConfigureTooltip(contentGameObject.GetComponent<ToolTip>());
		ConfigurePreferredLayout(contentGameObject);
	}
}
