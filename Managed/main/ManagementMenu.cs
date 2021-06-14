using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ManagementMenu : KIconToggleMenu
{
	public class ScreenData
	{
		public KScreen screen;

		public ManagementMenuToggleInfo toggleInfo;

		public Func<bool> cancelHandler;

		public int tabIdx;
	}

	public class ManagementMenuToggleInfo : ToggleInfo
	{
		public ImageToggleState alertImage;

		public ImageToggleState glowImage;

		private ColorStyleSetting originalButtonSetting;

		public ManagementMenuToggleInfo(string text, string icon, object user_data = null, Action hotkey = Action.NumActions, string tooltip = "", string tooltip_header = "")
			: base(text, icon, user_data, hotkey, tooltip, tooltip_header)
		{
			base.tooltip = GameUtil.ReplaceHotkeyString(base.tooltip, hotKey);
		}

		public void SetNotificationDisplay(bool showAlertImage, bool showGlow, ColorStyleSetting buttonColorStyle, ColorStyleSetting alertColorStyle)
		{
			ImageToggleState component = toggle.GetComponent<ImageToggleState>();
			if (component != null)
			{
				if (buttonColorStyle != null)
				{
					component.SetColorStyle(buttonColorStyle);
				}
				else
				{
					component.SetColorStyle(originalButtonSetting);
				}
			}
			if (alertImage != null)
			{
				alertImage.gameObject.SetActive(showAlertImage);
				alertImage.SetColorStyle(alertColorStyle);
			}
			if (glowImage != null)
			{
				glowImage.gameObject.SetActive(showGlow);
				if (buttonColorStyle != null)
				{
					glowImage.SetColorStyle(buttonColorStyle);
				}
			}
		}

		public override void SetToggle(KToggle toggle)
		{
			base.SetToggle(toggle);
			ImageToggleState component = toggle.GetComponent<ImageToggleState>();
			if (component != null)
			{
				originalButtonSetting = component.colorStyleSetting;
			}
			HierarchyReferences component2 = toggle.GetComponent<HierarchyReferences>();
			if (component2 != null)
			{
				alertImage = component2.GetReference<ImageToggleState>("AlertImage");
				glowImage = component2.GetReference<ImageToggleState>("GlowImage");
			}
		}
	}

	[MyCmpReq]
	public ManagementMenuNotificationDisplayer notificationDisplayer;

	public static ManagementMenu Instance;

	[Header("Management Menu Specific")]
	[SerializeField]
	private KToggle smallPrefab;

	public KToggle PauseMenuButton;

	[Header("Top Right Screen References")]
	public JobsTableScreen jobsScreen;

	public VitalsTableScreen vitalsScreen;

	public ScheduleScreen scheduleScreen;

	public ReportScreen reportsScreen;

	public CodexScreen codexScreen;

	public ConsumablesTableScreen consumablesScreen;

	private StarmapScreen starmapScreen;

	private ClusterMapScreen clusterMapScreen;

	private SkillsScreen skillsScreen;

	private ResearchScreen researchScreen;

	[Header("Notification Styles")]
	public ColorStyleSetting noAlertColorStyle;

	public List<ColorStyleSetting> alertColorStyle;

	public List<TextStyleSetting> alertTextStyle;

	private ManagementMenuToggleInfo jobsInfo;

	private ManagementMenuToggleInfo consumablesInfo;

	private ManagementMenuToggleInfo scheduleInfo;

	private ManagementMenuToggleInfo vitalsInfo;

	private ManagementMenuToggleInfo reportsInfo;

	private ManagementMenuToggleInfo researchInfo;

	private ManagementMenuToggleInfo codexInfo;

	private ManagementMenuToggleInfo starmapInfo;

	private ManagementMenuToggleInfo clusterMapInfo;

	private ManagementMenuToggleInfo skillsInfo;

	private Dictionary<ManagementMenuToggleInfo, ScreenData> ScreenInfoMatch = new Dictionary<ManagementMenuToggleInfo, ScreenData>();

	private ScreenData activeScreen;

	private KButton activeButton;

	private string skillsTooltip;

	private string skillsTooltipDisabled;

	private string researchTooltip;

	private string researchTooltipDisabled;

	private string starmapTooltip;

	private string starmapTooltipDisabled;

	private string clusterMapTooltip;

	private string clusterMapTooltipDisabled;

	private List<KScreen> mutuallyExclusiveScreens = new List<KScreen>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public override float GetSortKey()
	{
		return 21f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		notificationDisplayer.onNotificationsChanged += OnNotificationsChanged;
		CodexCache.CodexCacheInit();
		ScheduledUIInstantiation component = GameScreenManager.Instance.ssOverlayCanvas.GetComponent<ScheduledUIInstantiation>();
		starmapScreen = component.GetInstantiatedObject<StarmapScreen>();
		clusterMapScreen = component.GetInstantiatedObject<ClusterMapScreen>();
		skillsScreen = component.GetInstantiatedObject<SkillsScreen>();
		researchScreen = component.GetInstantiatedObject<ResearchScreen>();
		Subscribe(Game.Instance.gameObject, 288942073, OnUIClear);
		consumablesInfo = new ManagementMenuToggleInfo(UI.CONSUMABLES, "OverviewUI_consumables_icon", null, Action.ManageConsumables, UI.TOOLTIPS.MANAGEMENTMENU_CONSUMABLES);
		AddToggleTooltip(consumablesInfo);
		vitalsInfo = new ManagementMenuToggleInfo(UI.VITALS, "OverviewUI_vitals_icon", null, Action.ManageVitals, UI.TOOLTIPS.MANAGEMENTMENU_VITALS);
		AddToggleTooltip(vitalsInfo);
		researchInfo = new ManagementMenuToggleInfo(UI.RESEARCH, "OverviewUI_research_nav_icon", null, Action.ManageResearch, UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH);
		AddToggleTooltip(researchInfo, UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_RESEARCH);
		jobsInfo = new ManagementMenuToggleInfo(UI.JOBS, "OverviewUI_priority_icon", null, Action.ManagePriorities, UI.TOOLTIPS.MANAGEMENTMENU_JOBS);
		AddToggleTooltip(jobsInfo);
		skillsInfo = new ManagementMenuToggleInfo(UI.SKILLS, "OverviewUI_jobs_icon", null, Action.ManageSkills, UI.TOOLTIPS.MANAGEMENTMENU_SKILLS);
		AddToggleTooltip(skillsInfo, UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_SKILL_STATION);
		starmapInfo = new ManagementMenuToggleInfo(UI.STARMAP.MANAGEMENT_BUTTON, "OverviewUI_starmap_icon", null, Action.ManageStarmap, UI.TOOLTIPS.MANAGEMENTMENU_STARMAP);
		AddToggleTooltip(starmapInfo, UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_TELESCOPE);
		clusterMapInfo = new ManagementMenuToggleInfo(UI.STARMAP.MANAGEMENT_BUTTON, "OverviewUI_starmap_icon", null, Action.ManageStarmap, UI.TOOLTIPS.MANAGEMENTMENU_STARMAP);
		AddToggleTooltip(clusterMapInfo);
		scheduleInfo = new ManagementMenuToggleInfo(UI.SCHEDULE, "OverviewUI_schedule2_icon", null, Action.ManageSchedule, UI.TOOLTIPS.MANAGEMENTMENU_SCHEDULE);
		AddToggleTooltip(scheduleInfo);
		reportsInfo = new ManagementMenuToggleInfo(UI.REPORT, "OverviewUI_reports_icon", null, Action.ManageReport, UI.TOOLTIPS.MANAGEMENTMENU_DAILYREPORT);
		AddToggleTooltip(reportsInfo);
		reportsInfo.prefabOverride = smallPrefab;
		codexInfo = new ManagementMenuToggleInfo(UI.CODEX.MANAGEMENT_BUTTON, "OverviewUI_database_icon", null, Action.ManageDatabase, UI.TOOLTIPS.MANAGEMENTMENU_CODEX);
		AddToggleTooltip(codexInfo);
		codexInfo.prefabOverride = smallPrefab;
		ScreenInfoMatch.Add(consumablesInfo, new ScreenData
		{
			screen = consumablesScreen,
			tabIdx = 3,
			toggleInfo = consumablesInfo,
			cancelHandler = null
		});
		ScreenInfoMatch.Add(vitalsInfo, new ScreenData
		{
			screen = vitalsScreen,
			tabIdx = 2,
			toggleInfo = vitalsInfo,
			cancelHandler = null
		});
		ScreenInfoMatch.Add(reportsInfo, new ScreenData
		{
			screen = reportsScreen,
			tabIdx = 4,
			toggleInfo = reportsInfo,
			cancelHandler = null
		});
		ScreenInfoMatch.Add(jobsInfo, new ScreenData
		{
			screen = jobsScreen,
			tabIdx = 1,
			toggleInfo = jobsInfo,
			cancelHandler = null
		});
		ScreenInfoMatch.Add(skillsInfo, new ScreenData
		{
			screen = skillsScreen,
			tabIdx = 0,
			toggleInfo = skillsInfo,
			cancelHandler = null
		});
		ScreenInfoMatch.Add(codexInfo, new ScreenData
		{
			screen = codexScreen,
			tabIdx = 6,
			toggleInfo = codexInfo,
			cancelHandler = null
		});
		ScreenInfoMatch.Add(scheduleInfo, new ScreenData
		{
			screen = scheduleScreen,
			tabIdx = 7,
			toggleInfo = scheduleInfo,
			cancelHandler = null
		});
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			ScreenInfoMatch.Add(clusterMapInfo, new ScreenData
			{
				screen = clusterMapScreen,
				tabIdx = 7,
				toggleInfo = clusterMapInfo,
				cancelHandler = clusterMapScreen.TryHandleCancel
			});
		}
		else
		{
			ScreenInfoMatch.Add(starmapInfo, new ScreenData
			{
				screen = starmapScreen,
				tabIdx = 7,
				toggleInfo = starmapInfo,
				cancelHandler = null
			});
		}
		ScreenInfoMatch.Add(researchInfo, new ScreenData
		{
			screen = researchScreen,
			tabIdx = 5,
			toggleInfo = researchInfo,
			cancelHandler = null
		});
		List<ToggleInfo> list = new List<ToggleInfo>();
		list.Add(vitalsInfo);
		list.Add(consumablesInfo);
		list.Add(scheduleInfo);
		list.Add(jobsInfo);
		list.Add(skillsInfo);
		list.Add(researchInfo);
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			list.Add(clusterMapInfo);
		}
		else
		{
			list.Add(starmapInfo);
		}
		list.Add(reportsInfo);
		list.Add(codexInfo);
		Setup(list);
		base.onSelect += OnButtonClick;
		PauseMenuButton.onClick += OnPauseMenuClicked;
		PauseMenuButton.transform.SetAsLastSibling();
		PauseMenuButton.GetComponent<ToolTip>().toolTip = GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.MANAGEMENTMENU_PAUSEMENU, Action.Escape);
		Components.ResearchCenters.OnAdd += CheckResearch;
		Components.ResearchCenters.OnRemove += CheckResearch;
		Components.RoleStations.OnAdd += CheckSkills;
		Components.RoleStations.OnRemove += CheckSkills;
		Game.Instance.Subscribe(-809948329, CheckResearch);
		Game.Instance.Subscribe(-809948329, CheckSkills);
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			Components.Telescopes.OnAdd += CheckStarmap;
			Components.Telescopes.OnRemove += CheckStarmap;
		}
		CheckResearch(null);
		CheckSkills();
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			CheckStarmap();
		}
		researchInfo.toggle.soundPlayer.AcceptClickCondition = () => ResearchAvailable() || activeScreen == ScreenInfoMatch[Instance.researchInfo];
		foreach (KToggle toggle in toggles)
		{
			toggle.soundPlayer.toggle_widget_sound_events[0].PlaySound = false;
			toggle.soundPlayer.toggle_widget_sound_events[1].PlaySound = false;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		mutuallyExclusiveScreens.Add(AllResourcesScreen.Instance);
		mutuallyExclusiveScreens.Add(AllDiagnosticsScreen.Instance);
		OnNotificationsChanged();
	}

	private void OnNotificationsChanged()
	{
		foreach (KeyValuePair<ManagementMenuToggleInfo, ScreenData> item in ScreenInfoMatch)
		{
			ManagementMenuToggleInfo key = item.Key;
			key.SetNotificationDisplay(showAlertImage: false, showGlow: false, null, noAlertColorStyle);
		}
	}

	private void AddToggleTooltip(ManagementMenuToggleInfo toggleInfo, string disabledTooltip = null)
	{
		toggleInfo.getTooltipText = delegate
		{
			List<Tuple<string, TextStyleSetting>> list = new List<Tuple<string, TextStyleSetting>>();
			if (disabledTooltip != null && !toggleInfo.toggle.interactable)
			{
				list.Add(new Tuple<string, TextStyleSetting>(disabledTooltip, ToolTipScreen.Instance.defaultTooltipBodyStyle));
				return list;
			}
			if (toggleInfo.tooltipHeader != null)
			{
				list.Add(new Tuple<string, TextStyleSetting>(toggleInfo.tooltipHeader, ToolTipScreen.Instance.defaultTooltipHeaderStyle));
			}
			list.Add(new Tuple<string, TextStyleSetting>(toggleInfo.tooltip, ToolTipScreen.Instance.defaultTooltipBodyStyle));
			return list;
		};
	}

	private void OnPauseMenuClicked()
	{
		PauseScreen.Instance.Show();
		PauseMenuButton.isOn = false;
	}

	public void CheckResearch(object o)
	{
		if (!(researchInfo.toggle == null))
		{
			bool flag = Components.ResearchCenters.Count <= 0 && !DebugHandler.InstantBuildMode;
			bool active = !flag && activeScreen != null && activeScreen.toggleInfo == researchInfo;
			ConfigureToggle(researchInfo.toggle, flag, active);
		}
	}

	public void CheckSkills(object o = null)
	{
		if (!(skillsInfo.toggle == null))
		{
			bool disabled = Components.RoleStations.Count <= 0 && !DebugHandler.InstantBuildMode;
			bool active = activeScreen != null && activeScreen.toggleInfo == skillsInfo;
			ConfigureToggle(skillsInfo.toggle, disabled, active);
		}
	}

	public void CheckStarmap(object o = null)
	{
		if (!(starmapInfo.toggle == null))
		{
			bool disabled = Components.Telescopes.Count <= 0 && !DebugHandler.InstantBuildMode;
			bool active = activeScreen != null && activeScreen.toggleInfo == starmapInfo;
			ConfigureToggle(starmapInfo.toggle, disabled, active);
		}
	}

	private void ConfigureToggle(KToggle toggle, bool disabled, bool active)
	{
		toggle.interactable = !disabled;
		if (disabled)
		{
			toggle.GetComponentInChildren<ImageToggleState>().SetDisabled();
		}
		else
		{
			toggle.GetComponentInChildren<ImageToggleState>().SetActiveState(active);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (activeScreen != null && e.TryConsume(Action.Escape))
		{
			ToggleIfCancelUnhandled(activeScreen);
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (activeScreen != null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			ToggleIfCancelUnhandled(activeScreen);
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	private void ToggleIfCancelUnhandled(ScreenData screenData)
	{
		if (screenData.cancelHandler == null || !screenData.cancelHandler())
		{
			ToggleScreen(screenData);
		}
	}

	private bool ResearchAvailable()
	{
		return Components.ResearchCenters.Count > 0 || DebugHandler.InstantBuildMode;
	}

	private bool SkillsAvailable()
	{
		return Components.RoleStations.Count > 0 || DebugHandler.InstantBuildMode;
	}

	public static bool StarmapAvailable()
	{
		return Components.Telescopes.Count > 0 || DebugHandler.InstantBuildMode;
	}

	public void CloseAll()
	{
		if (activeScreen != null)
		{
			if (activeScreen.toggleInfo != null)
			{
				ToggleScreen(activeScreen);
			}
			CloseActive();
			ClearSelection();
		}
	}

	private void OnUIClear(object data)
	{
		CloseAll();
	}

	public void ToggleScreen(ScreenData screenData)
	{
		if (screenData == null)
		{
			return;
		}
		if (screenData.toggleInfo == researchInfo && !ResearchAvailable())
		{
			CheckResearch(null);
			CloseActive();
		}
		else if (screenData.toggleInfo == skillsInfo && !SkillsAvailable())
		{
			CheckSkills();
			CloseActive();
		}
		else if (screenData.toggleInfo == starmapInfo && !StarmapAvailable())
		{
			CheckStarmap();
			CloseActive();
		}
		else
		{
			if (screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().IsDisabled)
			{
				return;
			}
			if (activeScreen != null)
			{
				activeScreen.toggleInfo.toggle.isOn = false;
				activeScreen.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
			}
			if (activeScreen != screenData)
			{
				OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
				if (activeScreen != null)
				{
					activeScreen.toggleInfo.toggle.ActivateFlourish(state: false);
				}
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open"));
				AudioMixer.instance.Start(AudioMixerSnapshots.Get().MenuOpenMigrated);
				screenData.toggleInfo.toggle.ActivateFlourish(state: true);
				screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetActive();
				CloseActive();
				activeScreen = screenData;
				if (!activeScreen.screen.IsActive())
				{
					activeScreen.screen.Activate();
				}
				activeScreen.screen.Show();
				List<ManagementMenuNotification> notificationsForAction = notificationDisplayer.GetNotificationsForAction(screenData.toggleInfo.hotKey);
				foreach (ManagementMenuNotification item in notificationsForAction)
				{
					if (item.customClickCallback != null)
					{
						item.customClickCallback(item.customClickData);
						break;
					}
				}
				foreach (KScreen mutuallyExclusiveScreen in mutuallyExclusiveScreens)
				{
					mutuallyExclusiveScreen.Show(show: false);
				}
			}
			else
			{
				activeScreen.screen.Show(show: false);
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
				AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MenuOpenMigrated);
				activeScreen.toggleInfo.toggle.ActivateFlourish(state: false);
				activeScreen = null;
				screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
			}
		}
	}

	public void OnButtonClick(ToggleInfo toggle_info)
	{
		ToggleScreen(ScreenInfoMatch[(ManagementMenuToggleInfo)toggle_info]);
	}

	private void CloseActive()
	{
		if (activeScreen != null)
		{
			activeScreen.toggleInfo.toggle.isOn = false;
			activeScreen.screen.Show(show: false);
			activeScreen = null;
		}
	}

	public void ToggleResearch()
	{
		if ((ResearchAvailable() || activeScreen == ScreenInfoMatch[Instance.researchInfo]) && researchInfo != null)
		{
			ToggleScreen(ScreenInfoMatch[Instance.researchInfo]);
		}
	}

	public void ToggleCodex()
	{
		ToggleScreen(ScreenInfoMatch[Instance.codexInfo]);
	}

	public void OpenCodexToEntry(string id)
	{
		if (!codexScreen.gameObject.activeInHierarchy)
		{
			ToggleCodex();
		}
		codexScreen.ChangeArticle(id);
	}

	public void ToggleSkills()
	{
		if ((SkillsAvailable() || activeScreen == ScreenInfoMatch[Instance.skillsInfo]) && skillsInfo != null)
		{
			ToggleScreen(ScreenInfoMatch[Instance.skillsInfo]);
		}
	}

	public void ToggleStarmap()
	{
		if (starmapInfo != null)
		{
			ToggleScreen(ScreenInfoMatch[Instance.starmapInfo]);
		}
	}

	public void ToggleClusterMap()
	{
		ToggleScreen(ScreenInfoMatch[Instance.clusterMapInfo]);
	}

	public void TogglePriorities()
	{
		ToggleScreen(ScreenInfoMatch[Instance.jobsInfo]);
	}

	public void OpenReports(int day)
	{
		if (activeScreen != ScreenInfoMatch[Instance.reportsInfo])
		{
			ToggleScreen(ScreenInfoMatch[Instance.reportsInfo]);
		}
		ReportScreen.Instance.ShowReport(day);
	}

	public void OpenResearch()
	{
		if (activeScreen != ScreenInfoMatch[Instance.researchInfo])
		{
			ToggleScreen(ScreenInfoMatch[Instance.researchInfo]);
		}
	}

	public void OpenStarmap()
	{
		if (activeScreen != ScreenInfoMatch[Instance.starmapInfo])
		{
			ToggleScreen(ScreenInfoMatch[Instance.starmapInfo]);
		}
	}

	public void OpenClusterMap()
	{
		if (activeScreen != ScreenInfoMatch[Instance.clusterMapInfo])
		{
			ToggleScreen(ScreenInfoMatch[Instance.clusterMapInfo]);
		}
	}

	public void OpenSkills(MinionIdentity minionIdentity)
	{
		skillsScreen.CurrentlySelectedMinion = minionIdentity;
		if (activeScreen != ScreenInfoMatch[Instance.skillsInfo])
		{
			ToggleScreen(ScreenInfoMatch[Instance.skillsInfo]);
		}
	}
}
