using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class PlaceToolHoverTextCard : HoverTextConfiguration
{
	public Placeable currentPlaceable;

	public override void UpdateHoverElements(List<KSelectable> hoverObjects)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		if (!Grid.IsValidCell(num) || Grid.WorldIdx[num] != ClusterManager.Instance.activeWorldId)
		{
			hoverTextDrawer.EndDrawing();
			return;
		}
		hoverTextDrawer.BeginShadowBar();
		ActionName = UI.TOOLS.PLACE.TOOLACTION;
		if (currentPlaceable != null && currentPlaceable.GetProperName() != null)
		{
			ToolName = string.Format(UI.TOOLS.PLACE.NAME, currentPlaceable.GetProperName());
		}
		DrawTitle(instance, hoverTextDrawer);
		DrawInstructions(instance, hoverTextDrawer);
		int min_height = 26;
		int width = 8;
		if (currentPlaceable != null && !currentPlaceable.IsValidPlaceLocation(num, out var reason))
		{
			hoverTextDrawer.NewLine(min_height);
			hoverTextDrawer.AddIndent(width);
			hoverTextDrawer.DrawText(reason, HoverTextStyleSettings[1]);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
