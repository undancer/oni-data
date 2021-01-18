using System.Collections.Generic;
using UnityEngine;

public class ClusterMapSelectToolHoverTextCard : HoverTextConfiguration
{
	private Sprite m_iconWarning;

	private Sprite m_iconDash;

	private Sprite m_iconHighlighted;

	public override void ConfigureHoverScreen()
	{
		base.ConfigureHoverScreen();
		HoverTextScreen instance = HoverTextScreen.Instance;
		m_iconWarning = instance.GetSprite("iconWarning");
		m_iconDash = instance.GetSprite("dash");
		m_iconHighlighted = instance.GetSprite("dash_arrow");
	}

	public override void UpdateHoverElements(List<KSelectable> hoverObjects)
	{
		if (m_iconWarning == null)
		{
			ConfigureHoverScreen();
		}
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		foreach (KSelectable hoverObject in hoverObjects)
		{
			hoverTextDrawer.BeginShadowBar(ClusterMapSelectTool.Instance.GetSelected() == hoverObject);
			string unitFormattedName = GameUtil.GetUnitFormattedName(hoverObject.gameObject, upperName: true);
			hoverTextDrawer.DrawText(unitFormattedName, Styles_Title.Standard);
			foreach (StatusItemGroup.Entry item in hoverObject.GetStatusItemGroup())
			{
				if (item.category != null && item.category.Id == "Main")
				{
					TextStyleSetting style = (IsStatusItemWarning(item) ? Styles_Warning.Standard : Styles_BodyText.Standard);
					Sprite icon = ((item.item.sprite != null) ? item.item.sprite.sprite : m_iconWarning);
					Color color = (IsStatusItemWarning(item) ? Styles_Warning.Standard.textColor : Styles_BodyText.Standard.textColor);
					hoverTextDrawer.NewLine();
					hoverTextDrawer.DrawIcon(icon, color);
					hoverTextDrawer.DrawText(item.GetName(), style);
				}
			}
			foreach (StatusItemGroup.Entry item2 in hoverObject.GetStatusItemGroup())
			{
				if (item2.category == null || item2.category.Id != "Main")
				{
					TextStyleSetting style2 = (IsStatusItemWarning(item2) ? Styles_Warning.Standard : Styles_BodyText.Standard);
					Sprite icon2 = ((item2.item.sprite != null) ? item2.item.sprite.sprite : m_iconWarning);
					Color color2 = (IsStatusItemWarning(item2) ? Styles_Warning.Standard.textColor : Styles_BodyText.Standard.textColor);
					hoverTextDrawer.NewLine();
					hoverTextDrawer.DrawIcon(icon2, color2);
					hoverTextDrawer.DrawText(item2.GetName(), style2);
				}
			}
			hoverTextDrawer.EndShadowBar();
		}
		hoverTextDrawer.EndDrawing();
	}

	private bool IsStatusItemWarning(StatusItemGroup.Entry item)
	{
		if (item.item.notificationType == NotificationType.Bad || item.item.notificationType == NotificationType.BadMinor || item.item.notificationType == NotificationType.DuplicantThreatening)
		{
			return true;
		}
		return false;
	}
}
