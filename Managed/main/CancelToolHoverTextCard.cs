using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CancelToolHoverTextCard : HoverTextConfiguration
{
	private string lastUpdatedFilter;

	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		ToolParameterMenu toolParameterMenu = ToolMenu.Instance.toolParameterMenu;
		string lastEnabledFilter = toolParameterMenu.GetLastEnabledFilter();
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		if (!Grid.IsValidCell(num) || Grid.WorldIdx[num] != ClusterManager.Instance.activeWorldId)
		{
			hoverTextDrawer.EndDrawing();
			return;
		}
		hoverTextDrawer.BeginShadowBar();
		DrawTitle(instance, hoverTextDrawer);
		DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		if (lastEnabledFilter != null && lastEnabledFilter != lastUpdatedFilter)
		{
			ConfigureTitle(instance);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}

	protected override void ConfigureTitle(HoverTextScreen screen)
	{
		ToolParameterMenu toolParameterMenu = ToolMenu.Instance.toolParameterMenu;
		string lastEnabledFilter = toolParameterMenu.GetLastEnabledFilter();
		if (string.IsNullOrEmpty(ToolName) || lastEnabledFilter == "ALL")
		{
			ToolName = Strings.Get(ToolNameStringKey).String.ToUpper();
		}
		if (lastEnabledFilter != null && lastEnabledFilter != "ALL")
		{
			ToolName = Strings.Get(ToolNameStringKey).String.ToUpper() + string.Format(UI.TOOLS.FILTER_HOVERCARD_HEADER, Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + lastEnabledFilter).String.ToUpper());
		}
		lastUpdatedFilter = lastEnabledFilter;
	}
}
