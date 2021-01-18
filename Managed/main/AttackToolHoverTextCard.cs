using System.Collections.Generic;

public class AttackToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> hover_objects)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar();
		DrawTitle(instance, hoverTextDrawer);
		DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		hoverTextDrawer.EndShadowBar();
		if (hover_objects != null)
		{
			foreach (KSelectable hover_object in hover_objects)
			{
				if (hover_object.GetComponent<AttackableBase>() != null)
				{
					hoverTextDrawer.BeginShadowBar();
					hoverTextDrawer.DrawText(hover_object.GetProperName().ToUpper(), Styles_Title.Standard);
					hoverTextDrawer.EndShadowBar();
					break;
				}
			}
		}
		hoverTextDrawer.EndDrawing();
	}
}
