using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MoveToLocationToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> selected)
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
		DrawTitle(HoverTextScreen.Instance, hoverTextDrawer);
		DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		if (!MoveToLocationTool.Instance.CanMoveTo(num))
		{
			hoverTextDrawer.NewLine();
			hoverTextDrawer.DrawText(UI.TOOLS.MOVETOLOCATION.UNREACHABLE, HoverTextStyleSettings[1]);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
