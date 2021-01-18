using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SelectToolHoverTextCard : HoverTextConfiguration
{
	public static int maxNumberOfDisplayedSelectableWarnings = 10;

	private Dictionary<HashedString, Func<bool>> overlayFilterMap = new Dictionary<HashedString, Func<bool>>();

	public int recentNumberOfDisplayedSelectables = 0;

	public int currentSelectedSelectableIndex = -1;

	[NonSerialized]
	public Sprite iconWarning;

	[NonSerialized]
	public Sprite iconDash;

	[NonSerialized]
	public Sprite iconHighlighted;

	[NonSerialized]
	public Sprite iconActiveAutomationPort;

	public TextStylePair Styles_LogicActive;

	public TextStylePair Styles_LogicStandby;

	public TextStyleSetting Styles_LogicSignalInactive;

	public static List<GameObject> highlightedObjects = new List<GameObject>();

	private static readonly List<Type> hiddenChoreConsumerTypes = new List<Type>
	{
		typeof(KSelectableHealthBar)
	};

	private int maskOverlay;

	private string cachedTemperatureString;

	private float cachedTemperature = float.MinValue;

	private List<KSelectable> overlayValidHoverObjects = new List<KSelectable>();

	private Dictionary<HashedString, Func<KSelectable, bool>> modeFilters = new Dictionary<HashedString, Func<KSelectable, bool>>
	{
		{
			OverlayModes.Oxygen.ID,
			ShouldShowOxygenOverlay
		},
		{
			OverlayModes.Light.ID,
			ShouldShowLightOverlay
		},
		{
			OverlayModes.Radiation.ID,
			ShouldShowRadiationOverlay
		},
		{
			OverlayModes.GasConduits.ID,
			ShouldShowGasConduitOverlay
		},
		{
			OverlayModes.LiquidConduits.ID,
			ShouldShowLiquidConduitOverlay
		},
		{
			OverlayModes.SolidConveyor.ID,
			ShouldShowSolidConveyorOverlay
		},
		{
			OverlayModes.Power.ID,
			ShouldShowPowerOverlay
		},
		{
			OverlayModes.Logic.ID,
			ShouldShowLogicOverlay
		},
		{
			OverlayModes.TileMode.ID,
			ShouldShowTileOverlay
		},
		{
			OverlayModes.Disease.ID,
			ShowOverlayIfHasComponent<PrimaryElement>
		},
		{
			OverlayModes.Decor.ID,
			ShowOverlayIfHasComponent<DecorProvider>
		},
		{
			OverlayModes.Crop.ID,
			ShouldShowCropOverlay
		},
		{
			OverlayModes.Temperature.ID,
			ShouldShowTemperatureOverlay
		}
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		overlayFilterMap.Add(OverlayModes.Oxygen.ID, delegate
		{
			int num4 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			return Grid.Element[num4].IsGas;
		});
		overlayFilterMap.Add(OverlayModes.GasConduits.ID, delegate
		{
			int num3 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			return Grid.Element[num3].IsGas;
		});
		overlayFilterMap.Add(OverlayModes.Radiation.ID, delegate
		{
			int i = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			return Grid.Radiation[i] > 0f;
		});
		overlayFilterMap.Add(OverlayModes.LiquidConduits.ID, delegate
		{
			int num2 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			return Grid.Element[num2].IsLiquid;
		});
		overlayFilterMap.Add(OverlayModes.Decor.ID, () => false);
		overlayFilterMap.Add(OverlayModes.Rooms.ID, () => false);
		overlayFilterMap.Add(OverlayModes.Logic.ID, () => false);
		overlayFilterMap.Add(OverlayModes.TileMode.ID, delegate
		{
			int num = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			Element element = Grid.Element[num];
			foreach (Tag tileOverlayFilter in Game.Instance.tileOverlayFilters)
			{
				if (element.HasTag(tileOverlayFilter))
				{
					return true;
				}
			}
			return false;
		});
	}

	public override void ConfigureHoverScreen()
	{
		base.ConfigureHoverScreen();
		HoverTextScreen instance = HoverTextScreen.Instance;
		iconWarning = instance.GetSprite("iconWarning");
		iconDash = instance.GetSprite("dash");
		iconHighlighted = instance.GetSprite("dash_arrow");
		iconActiveAutomationPort = instance.GetSprite("current_automation_state_arrow");
		maskOverlay = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
	}

	private bool IsStatusItemWarning(StatusItemGroup.Entry item)
	{
		if (item.item.notificationType == NotificationType.Bad || item.item.notificationType == NotificationType.BadMinor || item.item.notificationType == NotificationType.DuplicantThreatening)
		{
			return true;
		}
		return false;
	}

	public override void UpdateHoverElements(List<KSelectable> hoverObjects)
	{
		if (iconWarning == null)
		{
			ConfigureHoverScreen();
		}
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		if (OverlayScreen.Instance == null || !Grid.IsValidCell(num))
		{
			return;
		}
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		overlayValidHoverObjects.Clear();
		foreach (KSelectable hoverObject in hoverObjects)
		{
			if (ShouldShowSelectableInCurrentOverlay(hoverObject))
			{
				overlayValidHoverObjects.Add(hoverObject);
			}
		}
		currentSelectedSelectableIndex = -1;
		if (highlightedObjects.Count > 0)
		{
			highlightedObjects.Clear();
		}
		HashedString mode = SimDebugView.Instance.GetMode();
		bool flag = mode == OverlayModes.Disease.ID;
		bool flag2 = true;
		if (Grid.DupePassable[num] && Grid.Solid[num])
		{
			flag2 = false;
		}
		bool flag3 = Grid.IsVisible(num);
		if (Grid.WorldIdx[num] != ClusterManager.Instance.activeWorldId)
		{
			flag3 = false;
		}
		if (!flag3)
		{
			flag2 = false;
		}
		foreach (KeyValuePair<HashedString, Func<bool>> item in overlayFilterMap)
		{
			if (OverlayScreen.Instance.GetMode() == item.Key)
			{
				if (!item.Value())
				{
					flag2 = false;
				}
				break;
			}
		}
		string text = "";
		string text2 = "";
		if (mode == OverlayModes.Temperature.ID && Game.Instance.temperatureOverlayMode == Game.TemperatureOverlayModes.HeatFlow)
		{
			if (!Grid.Solid[num] && flag3)
			{
				float thermalComfort = GameUtil.GetThermalComfort(num, 0f);
				float thermalComfort2 = GameUtil.GetThermalComfort(num);
				float num2 = 0f;
				if (thermalComfort2 * 0.001f > -0.27893335f - num2 && thermalComfort2 * 0.001f < 0.27893335f + num2)
				{
					text = UI.OVERLAYS.HEATFLOW.NEUTRAL;
				}
				else if (thermalComfort2 <= ExternalTemperatureMonitor.GetExternalColdThreshold(null))
				{
					text = UI.OVERLAYS.HEATFLOW.COOLING;
				}
				else if (thermalComfort2 >= ExternalTemperatureMonitor.GetExternalWarmThreshold(null))
				{
					text = UI.OVERLAYS.HEATFLOW.HEATING;
				}
				float dtu_s = 1f * thermalComfort;
				text = text + " (" + GameUtil.GetFormattedHeatEnergyRate(dtu_s) + ")";
				hoverTextDrawer.BeginShadowBar();
				hoverTextDrawer.DrawText(UI.OVERLAYS.HEATFLOW.HOVERTITLE, Styles_Title.Standard);
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawText(text, Styles_BodyText.Standard);
				hoverTextDrawer.EndShadowBar();
			}
		}
		else if (mode == OverlayModes.Decor.ID)
		{
			List<DecorProvider> list = new List<DecorProvider>();
			GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.decorProviderLayer, list);
			float decorAtCell = GameUtil.GetDecorAtCell(num);
			hoverTextDrawer.BeginShadowBar();
			hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.HOVERTITLE, Styles_Title.Standard);
			hoverTextDrawer.NewLine();
			hoverTextDrawer.DrawText(string.Concat(UI.OVERLAYS.DECOR.TOTAL, GameUtil.GetFormattedDecor(decorAtCell, enforce_max: true)), Styles_BodyText.Standard);
			if (!Grid.Solid[num] && flag3)
			{
				List<EffectorEntry> list2 = new List<EffectorEntry>();
				List<EffectorEntry> list3 = new List<EffectorEntry>();
				foreach (DecorProvider item2 in list)
				{
					float decorForCell = item2.GetDecorForCell(num);
					if (decorForCell == 0f)
					{
						continue;
					}
					string text3 = item2.GetName();
					KMonoBehaviour component = item2.GetComponent<KMonoBehaviour>();
					if (component != null && component.gameObject != null)
					{
						highlightedObjects.Add(component.gameObject);
						if (component.GetComponent<MonumentPart>() != null && component.GetComponent<MonumentPart>().IsMonumentCompleted())
						{
							text3 = MISC.MONUMENT_COMPLETE.NAME;
							List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(component.GetComponent<AttachableBuilding>());
							foreach (GameObject item3 in attachedNetwork)
							{
								highlightedObjects.Add(item3);
							}
						}
					}
					bool flag4 = false;
					if (decorForCell > 0f)
					{
						for (int i = 0; i < list2.Count; i++)
						{
							if (list2[i].name == text3)
							{
								EffectorEntry value = list2[i];
								value.count++;
								value.value += decorForCell;
								list2[i] = value;
								flag4 = true;
								break;
							}
						}
						if (!flag4)
						{
							list2.Add(new EffectorEntry(text3, decorForCell));
						}
						continue;
					}
					for (int j = 0; j < list3.Count; j++)
					{
						if (list3[j].name == text3)
						{
							EffectorEntry value2 = list3[j];
							value2.count++;
							value2.value += decorForCell;
							list3[j] = value2;
							flag4 = true;
							break;
						}
					}
					if (!flag4)
					{
						list3.Add(new EffectorEntry(text3, decorForCell));
					}
				}
				int lightDecorBonus = DecorProvider.GetLightDecorBonus(num);
				if (lightDecorBonus > 0)
				{
					list2.Add(new EffectorEntry(UI.OVERLAYS.DECOR.LIGHTING, lightDecorBonus));
				}
				list2.Sort((EffectorEntry x, EffectorEntry y) => y.value.CompareTo(x.value));
				if (list2.Count > 0)
				{
					hoverTextDrawer.NewLine();
					hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.HEADER_POSITIVE, Styles_BodyText.Standard);
				}
				foreach (EffectorEntry item4 in list2)
				{
					hoverTextDrawer.NewLine(18);
					hoverTextDrawer.DrawIcon(iconDash);
					hoverTextDrawer.DrawText(item4.ToString(), Styles_BodyText.Standard);
				}
				list3.Sort((EffectorEntry x, EffectorEntry y) => Mathf.Abs(y.value).CompareTo(Mathf.Abs(x.value)));
				if (list3.Count > 0)
				{
					hoverTextDrawer.NewLine();
					hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.HEADER_NEGATIVE, Styles_BodyText.Standard);
				}
				foreach (EffectorEntry item5 in list3)
				{
					hoverTextDrawer.NewLine(18);
					hoverTextDrawer.DrawIcon(iconDash);
					hoverTextDrawer.DrawText(item5.ToString(), Styles_BodyText.Standard);
				}
			}
			hoverTextDrawer.EndShadowBar();
		}
		else if (mode == OverlayModes.Rooms.ID)
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			if (cavityForCell != null)
			{
				Room room = cavityForCell.room;
				RoomType roomType = null;
				if (room != null)
				{
					roomType = room.roomType;
					text2 = roomType.Name;
				}
				else
				{
					text2 = UI.OVERLAYS.ROOMS.NOROOM.HEADER;
				}
				hoverTextDrawer.BeginShadowBar();
				hoverTextDrawer.DrawText(text2, Styles_Title.Standard);
				text = "";
				if (room != null)
				{
					string text4 = "";
					text4 = RoomDetails.EFFECT.resolve_string_function(room);
					string text5 = "";
					text5 = RoomDetails.ASSIGNED_TO.resolve_string_function(room);
					string text6 = "";
					text6 = RoomConstraints.RoomCriteriaString(room);
					string text7 = "";
					text7 = RoomDetails.EFFECTS.resolve_string_function(room);
					if (text4 != "")
					{
						hoverTextDrawer.NewLine();
						hoverTextDrawer.DrawText(text4, Styles_BodyText.Standard);
					}
					if (text5 != "" && roomType != Db.Get().RoomTypes.Neutral)
					{
						hoverTextDrawer.NewLine();
						hoverTextDrawer.DrawText(text5, Styles_BodyText.Standard);
					}
					hoverTextDrawer.NewLine(22);
					hoverTextDrawer.DrawText(RoomDetails.RoomDetailString(room), Styles_BodyText.Standard);
					if (text6 != "")
					{
						hoverTextDrawer.NewLine();
						hoverTextDrawer.DrawText(text6, Styles_BodyText.Standard);
					}
					if (text7 != "")
					{
						hoverTextDrawer.NewLine();
						hoverTextDrawer.DrawText(text7, Styles_BodyText.Standard);
					}
				}
				else
				{
					string text8 = UI.OVERLAYS.ROOMS.NOROOM.DESC;
					int maxRoomSize = TuningData<RoomProber.Tuning>.Get().maxRoomSize;
					if (cavityForCell.numCells > maxRoomSize)
					{
						text8 = text8 + "\n" + string.Format(UI.OVERLAYS.ROOMS.NOROOM.TOO_BIG, cavityForCell.numCells, maxRoomSize);
					}
					hoverTextDrawer.NewLine();
					hoverTextDrawer.DrawText(text8, Styles_BodyText.Standard);
				}
				hoverTextDrawer.EndShadowBar();
			}
		}
		else if (mode == OverlayModes.Light.ID)
		{
			if (flag3)
			{
				text = text + string.Format(UI.OVERLAYS.LIGHTING.DESC, Grid.LightIntensity[num]) + " (" + GameUtil.GetLightDescription(Grid.LightIntensity[num]) + ")";
				hoverTextDrawer.BeginShadowBar();
				hoverTextDrawer.DrawText(UI.OVERLAYS.LIGHTING.HOVERTITLE, Styles_Title.Standard);
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawText(text, Styles_BodyText.Standard);
				hoverTextDrawer.EndShadowBar();
			}
		}
		else if (mode == OverlayModes.Radiation.ID)
		{
			if (flag3)
			{
				text += UI.OVERLAYS.RADIATION.DESC.Replace("{rads}", GameUtil.GetFormattedRads(Grid.Radiation[num])).Replace("{description}", GameUtil.GetRadiationDescription(Grid.Radiation[num]));
				hoverTextDrawer.BeginShadowBar();
				hoverTextDrawer.DrawText(UI.OVERLAYS.RADIATION.HOVERTITLE, Styles_Title.Standard);
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawText(text, Styles_BodyText.Standard);
				hoverTextDrawer.EndShadowBar();
			}
		}
		else if (mode == OverlayModes.Logic.ID)
		{
			foreach (KSelectable hoverObject2 in hoverObjects)
			{
				LogicPorts component2 = hoverObject2.GetComponent<LogicPorts>();
				if (component2 != null && component2.TryGetPortAtCell(num, out var port, out var isInput))
				{
					bool flag5 = component2.IsPortConnected(port.id);
					hoverTextDrawer.BeginShadowBar();
					int num3;
					if (isInput)
					{
						string text9 = (port.displayCustomName ? port.description : UI.LOGIC_PORTS.PORT_INPUT_DEFAULT_NAME.text);
						num3 = component2.GetInputValue(port.id);
						hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_INPUT_HOVER_FMT.Replace("{Port}", text9.ToUpper()).Replace("{Name}", hoverObject2.GetProperName().ToUpper()), Styles_Title.Standard);
					}
					else
					{
						string text10 = (port.displayCustomName ? port.description : UI.LOGIC_PORTS.PORT_OUTPUT_DEFAULT_NAME.text);
						num3 = component2.GetOutputValue(port.id);
						hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_OUTPUT_HOVER_FMT.Replace("{Port}", text10.ToUpper()).Replace("{Name}", hoverObject2.GetProperName().ToUpper()), Styles_Title.Standard);
					}
					hoverTextDrawer.NewLine();
					TextStyleSetting style = ((!flag5) ? Styles_LogicActive.Standard : ((num3 == 1) ? Styles_LogicActive.Selected : Styles_LogicSignalInactive));
					DrawLogicIcon(hoverTextDrawer, (num3 == 1 && flag5) ? iconActiveAutomationPort : iconDash, style);
					DrawLogicText(hoverTextDrawer, port.activeDescription, style);
					hoverTextDrawer.NewLine();
					TextStyleSetting style2 = ((!flag5) ? Styles_LogicStandby.Standard : ((num3 == 0) ? Styles_LogicStandby.Selected : Styles_LogicSignalInactive));
					DrawLogicIcon(hoverTextDrawer, (num3 == 0 && flag5) ? iconActiveAutomationPort : iconDash, style2);
					DrawLogicText(hoverTextDrawer, port.inactiveDescription, style2);
					hoverTextDrawer.EndShadowBar();
				}
				LogicGate component3 = hoverObject2.GetComponent<LogicGate>();
				if (component3 != null && component3.TryGetPortAtCell(num, out var port2))
				{
					int portValue = component3.GetPortValue(port2);
					bool portConnected = component3.GetPortConnected(port2);
					LogicGate.LogicGateDescriptions.Description portDescription = component3.GetPortDescription(port2);
					hoverTextDrawer.BeginShadowBar();
					if (port2 == LogicGateBase.PortId.OutputOne)
					{
						hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_MULTI_OUTPUT_HOVER_FMT.Replace("{Port}", portDescription.name.ToUpper()).Replace("{Name}", hoverObject2.GetProperName().ToUpper()), Styles_Title.Standard);
					}
					else
					{
						hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_MULTI_INPUT_HOVER_FMT.Replace("{Port}", portDescription.name.ToUpper()).Replace("{Name}", hoverObject2.GetProperName().ToUpper()), Styles_Title.Standard);
					}
					hoverTextDrawer.NewLine();
					TextStyleSetting style3 = ((!portConnected) ? Styles_LogicActive.Standard : ((portValue == 1) ? Styles_LogicActive.Selected : Styles_LogicSignalInactive));
					DrawLogicIcon(hoverTextDrawer, (portValue == 1 && portConnected) ? iconActiveAutomationPort : iconDash, style3);
					DrawLogicText(hoverTextDrawer, portDescription.active, style3);
					hoverTextDrawer.NewLine();
					TextStyleSetting style4 = ((!portConnected) ? Styles_LogicStandby.Standard : ((portValue == 0) ? Styles_LogicStandby.Selected : Styles_LogicSignalInactive));
					DrawLogicIcon(hoverTextDrawer, (portValue == 0 && portConnected) ? iconActiveAutomationPort : iconDash, style4);
					DrawLogicText(hoverTextDrawer, portDescription.inactive, style4);
					hoverTextDrawer.EndShadowBar();
				}
			}
		}
		int num4 = 0;
		ChoreConsumer choreConsumer = null;
		if (SelectTool.Instance.selected != null)
		{
			choreConsumer = SelectTool.Instance.selected.GetComponent<ChoreConsumer>();
		}
		for (int k = 0; k < overlayValidHoverObjects.Count; k++)
		{
			if (!(overlayValidHoverObjects[k] != null) || !(overlayValidHoverObjects[k].GetComponent<CellSelectionObject>() == null))
			{
				continue;
			}
			KSelectable kSelectable = overlayValidHoverObjects[k];
			if ((OverlayScreen.Instance != null && OverlayScreen.Instance.mode != OverlayModes.None.ID && (kSelectable.gameObject.layer & maskOverlay) != 0) || !flag3)
			{
				continue;
			}
			PrimaryElement component4 = kSelectable.GetComponent<PrimaryElement>();
			bool flag6 = SelectTool.Instance.selected == overlayValidHoverObjects[k];
			if (flag6)
			{
				currentSelectedSelectableIndex = k;
			}
			num4++;
			hoverTextDrawer.BeginShadowBar(flag6);
			string text11 = GameUtil.GetUnitFormattedName(overlayValidHoverObjects[k].gameObject, upperName: true);
			if (component4 != null && kSelectable.GetComponent<Building>() != null)
			{
				text11 = StringFormatter.Replace(StringFormatter.Replace(UI.TOOLS.GENERIC.BUILDING_HOVER_NAME_FMT, "{Name}", text11), "{Element}", component4.Element.nameUpperCase);
			}
			hoverTextDrawer.DrawText(text11, Styles_Title.Standard);
			bool flag7 = false;
			string text12 = UI.OVERLAYS.DISEASE.NO_DISEASE;
			if (flag)
			{
				if (component4 != null && component4.DiseaseIdx != byte.MaxValue)
				{
					text12 = GameUtil.GetFormattedDisease(component4.DiseaseIdx, component4.DiseaseCount, color: true);
				}
				flag7 = true;
				Storage component5 = kSelectable.GetComponent<Storage>();
				if (component5 != null && component5.showInUI)
				{
					List<GameObject> items = component5.items;
					for (int l = 0; l < items.Count; l++)
					{
						GameObject gameObject = items[l];
						if (gameObject != null)
						{
							PrimaryElement component6 = gameObject.GetComponent<PrimaryElement>();
							if (component6.DiseaseIdx != byte.MaxValue)
							{
								text12 += string.Format(UI.OVERLAYS.DISEASE.CONTAINER_FORMAT, gameObject.GetComponent<KSelectable>().GetProperName(), GameUtil.GetFormattedDisease(component6.DiseaseIdx, component6.DiseaseCount, color: true));
							}
						}
					}
				}
			}
			if (flag7)
			{
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawIcon(iconDash);
				hoverTextDrawer.DrawText(text12, Styles_Values.Property.Standard);
			}
			int num5 = 0;
			foreach (StatusItemGroup.Entry item6 in overlayValidHoverObjects[k].GetStatusItemGroup())
			{
				if (ShowStatusItemInCurrentOverlay(item6.item))
				{
					if (num5 >= maxNumberOfDisplayedSelectableWarnings)
					{
						break;
					}
					if (item6.category != null && item6.category.Id == "Main" && num5 < maxNumberOfDisplayedSelectableWarnings)
					{
						TextStyleSetting style5 = (IsStatusItemWarning(item6) ? HoverTextStyleSettings[1] : Styles_BodyText.Standard);
						Sprite icon = ((item6.item.sprite != null) ? item6.item.sprite.sprite : iconWarning);
						Color color = (IsStatusItemWarning(item6) ? HoverTextStyleSettings[1].textColor : Styles_BodyText.Standard.textColor);
						hoverTextDrawer.NewLine();
						hoverTextDrawer.DrawIcon(icon, color);
						hoverTextDrawer.DrawText(item6.GetName(), style5);
						num5++;
					}
				}
			}
			foreach (StatusItemGroup.Entry item7 in overlayValidHoverObjects[k].GetStatusItemGroup())
			{
				if (ShowStatusItemInCurrentOverlay(item7.item))
				{
					if (num5 >= maxNumberOfDisplayedSelectableWarnings)
					{
						break;
					}
					if ((item7.category == null || item7.category.Id != "Main") && num5 < maxNumberOfDisplayedSelectableWarnings)
					{
						TextStyleSetting style6 = (IsStatusItemWarning(item7) ? HoverTextStyleSettings[1] : Styles_BodyText.Standard);
						Sprite icon2 = ((item7.item.sprite != null) ? item7.item.sprite.sprite : iconWarning);
						Color color2 = (IsStatusItemWarning(item7) ? HoverTextStyleSettings[1].textColor : Styles_BodyText.Standard.textColor);
						hoverTextDrawer.NewLine();
						hoverTextDrawer.DrawIcon(icon2, color2);
						hoverTextDrawer.DrawText(item7.GetName(), style6);
						num5++;
					}
				}
			}
			float temp = 0f;
			bool flag8 = true;
			bool flag9 = OverlayModes.Temperature.ID == SimDebugView.Instance.GetMode() && Game.Instance.temperatureOverlayMode != Game.TemperatureOverlayModes.HeatFlow;
			if ((bool)kSelectable.GetComponent<Constructable>())
			{
				flag8 = false;
			}
			else if (flag9 && (bool)component4)
			{
				temp = component4.Temperature;
			}
			else if ((bool)kSelectable.GetComponent<Building>() && (bool)component4)
			{
				temp = component4.Temperature;
			}
			else if (kSelectable.GetComponent<CellSelectionObject>() != null)
			{
				temp = kSelectable.GetComponent<CellSelectionObject>().temperature;
			}
			else
			{
				flag8 = false;
			}
			if (mode != OverlayModes.None.ID && mode != OverlayModes.Temperature.ID)
			{
				flag8 = false;
			}
			if (flag8)
			{
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawIcon(iconDash);
				hoverTextDrawer.DrawText(GameUtil.GetFormattedTemperature(temp), Styles_BodyText.Standard);
			}
			BuildingComplete component7 = kSelectable.GetComponent<BuildingComplete>();
			if (component7 != null && component7.Def.IsFoundation && Grid.Element[num].IsSolid)
			{
				flag2 = false;
			}
			if (mode == OverlayModes.Light.ID && choreConsumer != null)
			{
				bool flag10 = false;
				foreach (Type hiddenChoreConsumerType in hiddenChoreConsumerTypes)
				{
					if (choreConsumer.gameObject.GetComponent(hiddenChoreConsumerType) != null)
					{
						flag10 = true;
						break;
					}
				}
				if (!flag10)
				{
					choreConsumer.ShowHoverTextOnHoveredItem(kSelectable, hoverTextDrawer, this);
				}
			}
			hoverTextDrawer.EndShadowBar();
		}
		if (flag2)
		{
			CellSelectionObject cellSelectionObject = null;
			if (SelectTool.Instance.selected != null)
			{
				cellSelectionObject = SelectTool.Instance.selected.GetComponent<CellSelectionObject>();
			}
			bool flag11 = cellSelectionObject != null && cellSelectionObject.mouseCell == cellSelectionObject.alternateSelectionObject.mouseCell;
			if (flag11)
			{
				currentSelectedSelectableIndex = recentNumberOfDisplayedSelectables - 1;
			}
			Element element = Grid.Element[num];
			hoverTextDrawer.BeginShadowBar(flag11);
			hoverTextDrawer.DrawText(element.nameUpperCase, Styles_Title.Standard);
			if (Grid.DiseaseCount[num] > 0 || flag)
			{
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawIcon(iconDash);
				hoverTextDrawer.DrawText(GameUtil.GetFormattedDisease(Grid.DiseaseIdx[num], Grid.DiseaseCount[num], color: true), Styles_Values.Property.Standard);
			}
			if (!element.IsVacuum)
			{
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawIcon(iconDash);
				hoverTextDrawer.DrawText(ElementLoader.elements[Grid.ElementIdx[num]].GetMaterialCategoryTag().ProperName(), Styles_BodyText.Standard);
			}
			string[] array = HoverTextHelper.MassStringsReadOnly(num);
			hoverTextDrawer.NewLine();
			hoverTextDrawer.DrawIcon(iconDash);
			for (int m = 0; m < array.Length; m++)
			{
				if (m >= 3 || !element.IsVacuum)
				{
					hoverTextDrawer.DrawText(array[m], Styles_BodyText.Standard);
				}
			}
			if (!element.IsVacuum)
			{
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawIcon(iconDash);
				Element element2 = Grid.Element[num];
				string text13 = cachedTemperatureString;
				float num6 = Grid.Temperature[num];
				if (num6 != cachedTemperature)
				{
					cachedTemperature = num6;
					text13 = (cachedTemperatureString = GameUtil.GetFormattedTemperature(Grid.Temperature[num]));
				}
				string text14 = ((element2.specificHeatCapacity == 0f) ? "N/A" : text13);
				hoverTextDrawer.DrawText(text14, Styles_BodyText.Standard);
			}
			if (CellSelectionObject.IsExposedToSpace(num))
			{
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawIcon(iconDash);
				hoverTextDrawer.DrawText(MISC.STATUSITEMS.SPACE.NAME, Styles_BodyText.Standard);
			}
			if (Game.Instance.GetComponent<EntombedItemVisualizer>().IsEntombedItem(num))
			{
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawIcon(iconDash);
				hoverTextDrawer.DrawText(MISC.STATUSITEMS.BURIEDITEM.NAME, Styles_BodyText.Standard);
			}
			int num7 = Grid.CellAbove(num);
			bool flag12 = element.IsLiquid && Grid.IsValidCell(num7) && (Grid.Element[num7].IsGas || Grid.Element[num7].IsVacuum);
			if (element.sublimateId != 0 && (element.IsSolid || flag12))
			{
				float mass = Grid.AccumulatedFlow[num] / 3f;
				string elementNameByElementHash = GameUtil.GetElementNameByElementHash(element.id);
				string elementNameByElementHash2 = GameUtil.GetElementNameByElementHash(element.sublimateId);
				string text15 = BUILDING.STATUSITEMS.EMITTINGGASAVG.NAME;
				text15 = text15.Replace("{FlowRate}", GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.PerSecond));
				text15 = text15.Replace("{Element}", elementNameByElementHash2);
				hoverTextDrawer.NewLine();
				hoverTextDrawer.DrawIcon(iconDash);
				hoverTextDrawer.DrawText(text15, Styles_BodyText.Standard);
				GameUtil.IsEmissionBlocked(num, out var all_not_gaseous, out var all_over_pressure);
				string text16 = null;
				if (all_not_gaseous)
				{
					text16 = MISC.STATUSITEMS.SUBLIMATIONBLOCKED.NAME;
				}
				else if (all_over_pressure)
				{
					text16 = MISC.STATUSITEMS.SUBLIMATIONOVERPRESSURE.NAME;
				}
				if (text16 != null)
				{
					text16 = text16.Replace("{Element}", elementNameByElementHash);
					text16 = text16.Replace("{SubElement}", elementNameByElementHash2);
					hoverTextDrawer.NewLine();
					hoverTextDrawer.DrawIcon(iconDash);
					hoverTextDrawer.DrawText(text16, Styles_BodyText.Standard);
				}
			}
			hoverTextDrawer.EndShadowBar();
		}
		else if (!flag3 && Grid.WorldIdx[num] == ClusterManager.Instance.activeWorldId)
		{
			hoverTextDrawer.BeginShadowBar();
			hoverTextDrawer.DrawIcon(iconWarning);
			hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.UNKNOWN, Styles_BodyText.Standard);
			hoverTextDrawer.EndShadowBar();
		}
		recentNumberOfDisplayedSelectables = num4 + 1;
		hoverTextDrawer.EndDrawing();
	}

	public void DrawLogicIcon(HoverTextDrawer drawer, Sprite icon, TextStyleSetting style)
	{
		drawer.DrawIcon(icon, GetLogicColorFromStyle(style));
	}

	public void DrawLogicText(HoverTextDrawer drawer, string text, TextStyleSetting style)
	{
		drawer.DrawText(text, style, GetLogicColorFromStyle(style));
	}

	private Color GetLogicColorFromStyle(TextStyleSetting style)
	{
		ColorSet colorSet = GlobalAssets.Instance.colorSet;
		if (style == Styles_LogicActive.Selected)
		{
			return colorSet.logicOnText;
		}
		if (style == Styles_LogicStandby.Selected)
		{
			return colorSet.logicOffText;
		}
		return style.textColor;
	}

	private bool ShowStatusItemInCurrentOverlay(StatusItem status)
	{
		if (OverlayScreen.Instance == null)
		{
			return false;
		}
		return ((uint)status.status_overlays & (uint)StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode())) == (uint)StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode());
	}

	private bool ShouldShowSelectableInCurrentOverlay(KSelectable selectable)
	{
		bool result = true;
		if (OverlayScreen.Instance == null)
		{
			return result;
		}
		if (selectable == null)
		{
			return false;
		}
		if (selectable.GetComponent<KPrefabID>() == null)
		{
			return result;
		}
		HashedString mode = OverlayScreen.Instance.GetMode();
		if (modeFilters.TryGetValue(mode, out var value))
		{
			result = value(selectable);
		}
		return result;
	}

	private static bool ShouldShowOxygenOverlay(KSelectable selectable)
	{
		return selectable.GetComponent<AlgaeHabitat>() != null || selectable.GetComponent<Electrolyzer>() != null || selectable.GetComponent<AirFilter>() != null;
	}

	private static bool ShouldShowLightOverlay(KSelectable selectable)
	{
		return selectable.GetComponent<Light2D>() != null;
	}

	private static bool ShouldShowRadiationOverlay(KSelectable selectable)
	{
		return selectable.GetComponent<HighEnergyParticle>() != null || (bool)selectable.GetComponent<HighEnergyParticlePort>();
	}

	private static bool ShouldShowGasConduitOverlay(KSelectable selectable)
	{
		return (selectable.GetComponent<Conduit>() != null && selectable.GetComponent<Conduit>().type == ConduitType.Gas) || (selectable.GetComponent<Filterable>() != null && selectable.GetComponent<Filterable>().filterElementState == Filterable.ElementState.Gas) || (selectable.GetComponent<Vent>() != null && selectable.GetComponent<Vent>().conduitType == ConduitType.Gas) || (selectable.GetComponent<Pump>() != null && selectable.GetComponent<Pump>().conduitType == ConduitType.Gas) || (selectable.GetComponent<ValveBase>() != null && selectable.GetComponent<ValveBase>().conduitType == ConduitType.Gas);
	}

	private static bool ShouldShowLiquidConduitOverlay(KSelectable selectable)
	{
		return (selectable.GetComponent<Conduit>() != null && selectable.GetComponent<Conduit>().type == ConduitType.Liquid) || (selectable.GetComponent<Filterable>() != null && selectable.GetComponent<Filterable>().filterElementState == Filterable.ElementState.Liquid) || (selectable.GetComponent<Vent>() != null && selectable.GetComponent<Vent>().conduitType == ConduitType.Liquid) || (selectable.GetComponent<Pump>() != null && selectable.GetComponent<Pump>().conduitType == ConduitType.Liquid) || (selectable.GetComponent<ValveBase>() != null && selectable.GetComponent<ValveBase>().conduitType == ConduitType.Liquid);
	}

	private static bool ShouldShowPowerOverlay(KSelectable selectable)
	{
		Tag prefabTag = selectable.GetComponent<KPrefabID>().PrefabTag;
		return OverlayScreen.WireIDs.Contains(prefabTag) || selectable.GetComponent<Battery>() != null || selectable.GetComponent<PowerTransformer>() != null || selectable.GetComponent<EnergyConsumer>() != null || selectable.GetComponent<EnergyGenerator>() != null;
	}

	private static bool ShouldShowTileOverlay(KSelectable selectable)
	{
		bool result = false;
		PrimaryElement component = selectable.GetComponent<PrimaryElement>();
		if (component != null)
		{
			Element element = component.Element;
			foreach (Tag tileOverlayFilter in Game.Instance.tileOverlayFilters)
			{
				if (element.HasTag(tileOverlayFilter))
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	private static bool ShouldShowTemperatureOverlay(KSelectable selectable)
	{
		return selectable.GetComponent<PrimaryElement>() != null;
	}

	private static bool ShouldShowLogicOverlay(KSelectable selectable)
	{
		Tag prefabTag = selectable.GetComponent<KPrefabID>().PrefabTag;
		return OverlayModes.Logic.HighlightItemIDs.Contains(prefabTag) || selectable.GetComponent<LogicPorts>() != null;
	}

	private static bool ShouldShowSolidConveyorOverlay(KSelectable selectable)
	{
		Tag prefabTag = selectable.GetComponent<KPrefabID>().PrefabTag;
		return OverlayScreen.SolidConveyorIDs.Contains(prefabTag);
	}

	private static bool HideInOverlay(KSelectable selectable)
	{
		return false;
	}

	private static bool ShowOverlayIfHasComponent<T>(KSelectable selectable)
	{
		return selectable.GetComponent<T>() != null;
	}

	private static bool ShouldShowCropOverlay(KSelectable selectable)
	{
		return selectable.GetComponent<Uprootable>() != null || selectable.GetComponent<PlanterBox>() != null;
	}
}
