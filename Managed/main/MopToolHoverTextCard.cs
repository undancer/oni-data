using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MopToolHoverTextCard : HoverTextConfiguration
{
	private struct HoverScreenFields
	{
		public GameObject UnknownAreaLine;

		public Image ElementStateIcon;

		public LocText ElementCategory;

		public LocText ElementName;

		public LocText[] ElementMass;

		public LocText ElementHardness;

		public LocText ElementHardnessDescription;
	}

	private HoverScreenFields hoverScreenElements;

	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		if (!Grid.IsValidCell(num) || Grid.WorldIdx[num] != ClusterManager.Instance.activeWorldId)
		{
			hoverTextDrawer.EndDrawing();
			return;
		}
		hoverTextDrawer.BeginShadowBar();
		if (Grid.IsVisible(num))
		{
			DrawTitle(instance, hoverTextDrawer);
			DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
			Element element = Grid.Element[num];
			if (element.IsLiquid)
			{
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawText(element.nameUpperCase, Styles_Title.Standard);
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawIcon(instance.GetSprite("dash"));
				hoverTextDrawer.DrawText(element.GetMaterialCategoryTag().ProperName(), Styles_BodyText.Standard);
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawIcon(instance.GetSprite("dash"));
				string[] array = HoverTextHelper.MassStringsReadOnly(num);
				hoverTextDrawer.DrawText(array[0], Styles_Values.Property.Standard);
				hoverTextDrawer.DrawText(array[1], Styles_Values.Property_Decimal.Standard);
				hoverTextDrawer.DrawText(array[2], Styles_Values.Property.Standard);
				hoverTextDrawer.DrawText(array[3], Styles_Values.Property.Standard);
			}
		}
		else
		{
			hoverTextDrawer.DrawIcon(instance.GetSprite("iconWarning"));
			hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.UNKNOWN.ToString().ToUpper(), Styles_BodyText.Standard);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
