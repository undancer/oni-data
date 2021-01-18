using System.Collections.Generic;
using STRINGS;

public class PrioritizeToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		if (!(ToolMenu.Instance.PriorityScreen == null))
		{
			HoverTextScreen instance = HoverTextScreen.Instance;
			HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
			hoverTextDrawer.BeginShadowBar();
			DrawTitle(instance, hoverTextDrawer);
			DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
			hoverTextDrawer.NewLine();
			hoverTextDrawer.DrawText(string.Format(UI.TOOLS.PRIORITIZE.SPECIFIC_PRIORITY, ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority().priority_value.ToString()), Styles_Title.Standard);
			string lastEnabledFilter = ToolMenu.Instance.toolParameterMenu.GetLastEnabledFilter();
			if (lastEnabledFilter != null && lastEnabledFilter != "ALL")
			{
				ConfigureTitle(instance);
			}
			hoverTextDrawer.EndShadowBar();
			hoverTextDrawer.EndDrawing();
		}
	}

	protected override void ConfigureTitle(HoverTextScreen screen)
	{
		string lastEnabledFilter = ToolMenu.Instance.toolParameterMenu.GetLastEnabledFilter();
		if (string.IsNullOrEmpty(ToolName) || lastEnabledFilter == "ALL")
		{
			ToolName = Strings.Get(ToolNameStringKey).String.ToUpper();
		}
		if (lastEnabledFilter != null && lastEnabledFilter != "ALL")
		{
			ToolName = Strings.Get(ToolNameStringKey).String.ToUpper() + string.Format(UI.TOOLS.FILTER_HOVERCARD_HEADER, Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + lastEnabledFilter).String.ToUpper());
		}
	}
}
