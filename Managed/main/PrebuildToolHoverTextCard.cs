using System.Collections.Generic;
using STRINGS;

public class PrebuildToolHoverTextCard : HoverTextConfiguration
{
	public PlanScreen.RequirementsState currentReqState;

	public BuildingDef currentDef;

	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar();
		switch (currentReqState)
		{
		case PlanScreen.RequirementsState.Materials:
		case PlanScreen.RequirementsState.Complete:
			hoverTextDrawer.DrawText(UI.TOOLTIPS.NOMATERIAL.text.ToUpper(), HoverTextStyleSettings[0]);
			hoverTextDrawer.NewLine();
			hoverTextDrawer.DrawText(UI.TOOLTIPS.SELECTAMATERIAL, HoverTextStyleSettings[1]);
			break;
		case PlanScreen.RequirementsState.Tech:
		{
			Tech parentTech = Db.Get().TechItems.Get(currentDef.PrefabID).parentTech;
			hoverTextDrawer.DrawText(string.Format(UI.PRODUCTINFO_RESEARCHREQUIRED, parentTech.Name).ToUpper(), HoverTextStyleSettings[0]);
			break;
		}
		}
		hoverTextDrawer.NewLine();
		hoverTextDrawer.DrawIcon(instance.GetSprite("icon_mouse_right"));
		hoverTextDrawer.DrawText(backStr, Styles_Instruction.Standard);
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
