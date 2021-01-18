using System.Collections.Generic;
using UnityEngine;

public class PrebuildToolHoverTextCard : HoverTextConfiguration
{
	public string errorMessage;

	public BuildingDef currentDef;

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
		if (!errorMessage.IsNullOrWhiteSpace())
		{
			bool flag = true;
			string[] array = errorMessage.Split('\n');
			foreach (string text in array)
			{
				if (!flag)
				{
					hoverTextDrawer.NewLine();
				}
				hoverTextDrawer.DrawText(text.ToUpper(), HoverTextStyleSettings[(!flag) ? 1 : 0]);
				flag = false;
			}
		}
		hoverTextDrawer.NewLine();
		hoverTextDrawer.DrawIcon(instance.GetSprite("icon_mouse_right"));
		hoverTextDrawer.DrawText(backStr, Styles_Instruction.Standard);
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
