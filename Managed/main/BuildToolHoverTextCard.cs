using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BuildToolHoverTextCard : HoverTextConfiguration
{
	public BuildingDef currentDef;

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
		ActionName = ((currentDef != null && currentDef.DragBuild) ? UI.TOOLS.BUILD.TOOLACTION_DRAG : UI.TOOLS.BUILD.TOOLACTION);
		if (currentDef != null && currentDef.Name != null)
		{
			ToolName = string.Format(UI.TOOLS.BUILD.NAME, currentDef.Name);
		}
		DrawTitle(instance, hoverTextDrawer);
		DrawInstructions(instance, hoverTextDrawer);
		int min_height = 26;
		int width = 8;
		if (currentDef != null)
		{
			Orientation orientation = Orientation.Neutral;
			if (PlayerController.Instance.ActiveTool != null)
			{
				Type type = PlayerController.Instance.ActiveTool.GetType();
				if (typeof(BuildTool).IsAssignableFrom(type) || typeof(BaseUtilityBuildTool).IsAssignableFrom(type))
				{
					if (currentDef.BuildingComplete.GetComponent<Rotatable>() != null)
					{
						hoverTextDrawer.NewLine(min_height);
						hoverTextDrawer.AddIndent(width);
						string text = UI.TOOLTIPS.HELP_ROTATE_KEY.ToString();
						text = text.Replace("{Key}", GameUtil.GetActionString(Action.RotateBuilding));
						hoverTextDrawer.DrawText(text, Styles_Instruction.Standard);
					}
					orientation = BuildTool.Instance.GetBuildingOrientation;
					string fail_reason = "Unknown reason";
					Vector3 pos = Grid.CellToPosCCC(num, Grid.SceneLayer.Building);
					if (!currentDef.IsValidPlaceLocation(BuildTool.Instance.visualizer, pos, orientation, out fail_reason))
					{
						hoverTextDrawer.NewLine(min_height);
						hoverTextDrawer.AddIndent(width);
						hoverTextDrawer.DrawText(fail_reason, HoverTextStyleSettings[1]);
					}
					RoomTracker component = currentDef.BuildingComplete.GetComponent<RoomTracker>();
					if (component != null && !component.SufficientBuildLocation(num))
					{
						hoverTextDrawer.NewLine(min_height);
						hoverTextDrawer.AddIndent(width);
						hoverTextDrawer.DrawText(UI.TOOLTIPS.HELP_REQUIRES_ROOM, HoverTextStyleSettings[1]);
					}
				}
			}
			hoverTextDrawer.NewLine(min_height);
			hoverTextDrawer.AddIndent(width);
			hoverTextDrawer.DrawText(ResourceRemainingDisplayScreen.instance.GetString(), Styles_BodyText.Standard);
			hoverTextDrawer.EndShadowBar();
			HashedString mode = SimDebugView.Instance.GetMode();
			if (mode == OverlayModes.Logic.ID && hoverObjects != null)
			{
				SelectToolHoverTextCard component2 = SelectTool.Instance.GetComponent<SelectToolHoverTextCard>();
				foreach (KSelectable hoverObject in hoverObjects)
				{
					LogicPorts component3 = hoverObject.GetComponent<LogicPorts>();
					if (component3 != null && component3.TryGetPortAtCell(num, out var port, out var isInput))
					{
						bool flag = component3.IsPortConnected(port.id);
						hoverTextDrawer.BeginShadowBar();
						int num2;
						if (isInput)
						{
							string replacement = (port.displayCustomName ? port.description : UI.LOGIC_PORTS.PORT_INPUT_DEFAULT_NAME.text);
							num2 = component3.GetInputValue(port.id);
							hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_INPUT_HOVER_FMT.Replace("{Port}", replacement).Replace("{Name}", hoverObject.GetProperName().ToUpper()), component2.Styles_Title.Standard);
						}
						else
						{
							string replacement2 = (port.displayCustomName ? port.description : UI.LOGIC_PORTS.PORT_OUTPUT_DEFAULT_NAME.text);
							num2 = component3.GetOutputValue(port.id);
							hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_OUTPUT_HOVER_FMT.Replace("{Port}", replacement2).Replace("{Name}", hoverObject.GetProperName().ToUpper()), component2.Styles_Title.Standard);
						}
						hoverTextDrawer.NewLine();
						TextStyleSetting style = ((!flag) ? component2.Styles_LogicActive.Standard : ((num2 == 1) ? component2.Styles_LogicActive.Selected : component2.Styles_LogicSignalInactive));
						component2.DrawLogicIcon(hoverTextDrawer, (num2 == 1 && flag) ? component2.iconActiveAutomationPort : component2.iconDash, style);
						component2.DrawLogicText(hoverTextDrawer, port.activeDescription, style);
						hoverTextDrawer.NewLine();
						TextStyleSetting style2 = ((!flag) ? component2.Styles_LogicStandby.Standard : ((num2 == 0) ? component2.Styles_LogicStandby.Selected : component2.Styles_LogicSignalInactive));
						component2.DrawLogicIcon(hoverTextDrawer, (num2 == 0 && flag) ? component2.iconActiveAutomationPort : component2.iconDash, style2);
						component2.DrawLogicText(hoverTextDrawer, port.inactiveDescription, style2);
						hoverTextDrawer.EndShadowBar();
					}
					LogicGate component4 = hoverObject.GetComponent<LogicGate>();
					if (component4 != null && component4.TryGetPortAtCell(num, out var port2))
					{
						int portValue = component4.GetPortValue(port2);
						bool portConnected = component4.GetPortConnected(port2);
						LogicGate.LogicGateDescriptions.Description portDescription = component4.GetPortDescription(port2);
						hoverTextDrawer.BeginShadowBar();
						if (port2 == LogicGateBase.PortId.OutputOne)
						{
							hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_MULTI_OUTPUT_HOVER_FMT.Replace("{Port}", portDescription.name).Replace("{Name}", hoverObject.GetProperName().ToUpper()), component2.Styles_Title.Standard);
						}
						else
						{
							hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_MULTI_INPUT_HOVER_FMT.Replace("{Port}", portDescription.name).Replace("{Name}", hoverObject.GetProperName().ToUpper()), component2.Styles_Title.Standard);
						}
						hoverTextDrawer.NewLine();
						TextStyleSetting style3 = ((!portConnected) ? component2.Styles_LogicActive.Standard : ((portValue == 1) ? component2.Styles_LogicActive.Selected : component2.Styles_LogicSignalInactive));
						component2.DrawLogicIcon(hoverTextDrawer, (portValue == 1 && portConnected) ? component2.iconActiveAutomationPort : component2.iconDash, style3);
						component2.DrawLogicText(hoverTextDrawer, portDescription.active, style3);
						hoverTextDrawer.NewLine();
						TextStyleSetting style4 = ((!portConnected) ? component2.Styles_LogicStandby.Standard : ((portValue == 0) ? component2.Styles_LogicStandby.Selected : component2.Styles_LogicSignalInactive));
						component2.DrawLogicIcon(hoverTextDrawer, (portValue == 0 && portConnected) ? component2.iconActiveAutomationPort : component2.iconDash, style4);
						component2.DrawLogicText(hoverTextDrawer, portDescription.inactive, style4);
						hoverTextDrawer.EndShadowBar();
					}
				}
			}
			else if (mode == OverlayModes.Power.ID)
			{
				CircuitManager circuitManager = Game.Instance.circuitManager;
				ushort circuitID = circuitManager.GetCircuitID(num);
				if (circuitID != ushort.MaxValue)
				{
					hoverTextDrawer.BeginShadowBar();
					float wattsNeededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitID);
					wattsNeededWhenActive += currentDef.EnergyConsumptionWhenActive;
					float maxSafeWattageForCircuit = circuitManager.GetMaxSafeWattageForCircuit(circuitID);
					Color color = ((wattsNeededWhenActive >= maxSafeWattageForCircuit + POWER.FLOAT_FUDGE_FACTOR) ? Color.red : Color.white);
					hoverTextDrawer.AddIndent(width);
					hoverTextDrawer.DrawText(string.Format(UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(wattsNeededWhenActive)), Styles_BodyText.Standard, color);
					hoverTextDrawer.EndShadowBar();
				}
			}
		}
		hoverTextDrawer.EndDrawing();
	}
}
