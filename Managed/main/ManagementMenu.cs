using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ManagementMenu : KIconToggleMenu
{
	public class ScreenData
	{
		public KScreen screen;

		public ToggleInfo toggleInfo;

		public int tabIdx;
	}

	[SerializeField]
	private KToggle smallPrefab;

	private ScreenData activeScreen;

	private KButton activeButton;

	public static ManagementMenu Instance;

	public KScreen researchScreen;

	private JobsTableScreen jobsScreen;

	public VitalsTableScreen vitalsScreen;

	public ScheduleScreen scheduleScreen;

	public KScreen reportsScreen;

	private ConsumablesTableScreen consumablesScreen;

	public CodexScreen codexScreen;

	private StarmapScreen starmapScreen;

	private SkillsScreen skillsScreen;

	public string colourSchemeDisabled;

	public InstantiateUIPrefabChild instantiator;

	private ToggleInfo jobsInfo;

	private ToggleInfo consumablesInfo;

	private ToggleInfo scheduleInfo;

	private ToggleInfo vitalsInfo;

	private ToggleInfo reportsInfo;

	private ToggleInfo researchInfo;

	private ToggleInfo codexInfo;

	private ToggleInfo starmapInfo;

	private ToggleInfo skillsInfo;

	private Dictionary<ToggleInfo, ScreenData> ScreenInfoMatch = new Dictionary<ToggleInfo, ScreenData>();

	public KButton[] CloseButtons;

	public KToggle PauseMenuButton;

	private string skillsTooltip;

	private string skillsTooltipDisabled;

	private string researchTooltip;

	private string researchTooltipDisabled;

	private string starmapTooltip;

	private string starmapTooltipDisabled;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		CodexCache.CodexCacheInit();
		ScheduledUIInstantiation component = GameScreenManager.Instance.ssOverlayCanvas.GetComponent<ScheduledUIInstantiation>();
		instantiator.Instantiate();
		jobsScreen = instantiator.GetComponentInChildren<JobsTableScreen>(includeInactive: true);
		consumablesScreen = instantiator.GetComponentInChildren<ConsumablesTableScreen>(includeInactive: true);
		vitalsScreen = instantiator.GetComponentInChildren<VitalsTableScreen>(includeInactive: true);
		starmapScreen = component.GetInstantiatedObject<StarmapScreen>();
		codexScreen = instantiator.GetComponentInChildren<CodexScreen>(includeInactive: true);
		scheduleScreen = instantiator.GetComponentInChildren<ScheduleScreen>(includeInactive: true);
		skillsScreen = component.GetInstantiatedObject<SkillsScreen>();
		Subscribe(Game.Instance.gameObject, 288942073, OnUIClear);
		consumablesInfo = new ToggleInfo(UI.CONSUMABLES, "OverviewUI_consumables_icon", null, Action.ManageConsumables, UI.TOOLTIPS.MANAGEMENTMENU_CONSUMABLES);
		vitalsInfo = new ToggleInfo(UI.VITALS, "OverviewUI_vitals_icon", null, Action.ManageVitals, UI.TOOLTIPS.MANAGEMENTMENU_VITALS);
		reportsInfo = new ToggleInfo(UI.REPORT, "OverviewUI_reports_icon", null, Action.ManageReport, UI.TOOLTIPS.MANAGEMENTMENU_DAILYREPORT);
		reportsInfo.prefabOverride = smallPrefab;
		researchInfo = new ToggleInfo(UI.RESEARCH, "OverviewUI_research_nav_icon", null, Action.ManageResearch, UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH);
		jobsInfo = new ToggleInfo(UI.JOBS, "OverviewUI_priority_icon", null, Action.ManagePriorities, UI.TOOLTIPS.MANAGEMENTMENU_JOBS);
		skillsInfo = new ToggleInfo(UI.SKILLS, "OverviewUI_jobs_icon", null, Action.ManageSkills, UI.TOOLTIPS.MANAGEMENTMENU_SKILLS);
		starmapInfo = new ToggleInfo(UI.STARMAP.MANAGEMENT_BUTTON, "OverviewUI_starmap_icon", null, Action.ManageStarmap, UI.TOOLTIPS.MANAGEMENTMENU_STARMAP);
		codexInfo = new ToggleInfo(UI.CODEX.MANAGEMENT_BUTTON, "OverviewUI_database_icon", null, Action.ManageDatabase, UI.TOOLTIPS.MANAGEMENTMENU_CODEX);
		codexInfo.prefabOverride = smallPrefab;
		scheduleInfo = new ToggleInfo(UI.SCHEDULE, "OverviewUI_schedule2_icon", null, Action.ManageSchedule, UI.TOOLTIPS.MANAGEMENTMENU_SCHEDULE);
		ScreenInfoMatch.Add(consumablesInfo, new ScreenData
		{
			screen = consumablesScreen,
			tabIdx = 3,
			toggleInfo = consumablesInfo
		});
		ScreenInfoMatch.Add(vitalsInfo, new ScreenData
		{
			screen = vitalsScreen,
			tabIdx = 2,
			toggleInfo = vitalsInfo
		});
		ScreenInfoMatch.Add(reportsInfo, new ScreenData
		{
			screen = reportsScreen,
			tabIdx = 4,
			toggleInfo = reportsInfo
		});
		ScreenInfoMatch.Add(jobsInfo, new ScreenData
		{
			screen = jobsScreen,
			tabIdx = 1,
			toggleInfo = jobsInfo
		});
		ScreenInfoMatch.Add(skillsInfo, new ScreenData
		{
			screen = skillsScreen,
			tabIdx = 0,
			toggleInfo = skillsInfo
		});
		ScreenInfoMatch.Add(codexInfo, new ScreenData
		{
			screen = codexScreen,
			tabIdx = 6,
			toggleInfo = codexInfo
		});
		ScreenInfoMatch.Add(scheduleInfo, new ScreenData
		{
			screen = scheduleScreen,
			tabIdx = 7,
			toggleInfo = scheduleInfo
		});
		ScreenInfoMatch.Add(starmapInfo, new ScreenData
		{
			screen = starmapScreen,
			tabIdx = 7,
			toggleInfo = starmapInfo
		});
		List<ToggleInfo> list = new List<ToggleInfo>();
		list.Add(vitalsInfo);
		list.Add(consumablesInfo);
		list.Add(scheduleInfo);
		list.Add(jobsInfo);
		list.Add(skillsInfo);
		list.Add(researchInfo);
		list.Add(starmapInfo);
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
		Components.Telescopes.OnAdd += CheckStarmap;
		Components.Telescopes.OnRemove += CheckStarmap;
		skillsTooltipDisabled = UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_SKILL_STATION;
		skillsTooltip = GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.MANAGEMENTMENU_SKILLS, Action.ManageSkills);
		researchTooltipDisabled = UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_RESEARCH;
		researchTooltip = GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH, Action.ManageResearch);
		starmapTooltipDisabled = UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_TELESCOPE;
		starmapTooltip = GameUtil.ReplaceHotkeyString(UI.TOOLTIPS.MANAGEMENTMENU_STARMAP, Action.ManageStarmap);
		CheckResearch(null);
		CheckSkills();
		CheckStarmap();
		researchInfo.toggle.soundPlayer.AcceptClickCondition = () => ResearchAvailable() || activeScreen == ScreenInfoMatch[Instance.researchInfo];
		KButton[] closeButtons = CloseButtons;
		foreach (KButton obj in closeButtons)
		{
			obj.onClick += CloseAll;
			obj.soundPlayer.Enabled = false;
		}
		foreach (KToggle toggle in toggles)
		{
			toggle.soundPlayer.toggle_widget_sound_events[0].PlaySound = false;
			toggle.soundPlayer.toggle_widget_sound_events[1].PlaySound = false;
		}
	}

	private void OnPauseMenuClicked()
	{
		PauseScreen.Instance.Show();
		PauseMenuButton.isOn = false;
	}

	public void AddResearchScreen(ResearchScreen researchScreen)
	{
		if (!(this.researchScreen != null))
		{
			this.researchScreen = researchScreen;
			this.researchScreen.gameObject.SetActive(value: false);
			ScreenInfoMatch.Add(researchInfo, new ScreenData
			{
				screen = this.researchScreen,
				tabIdx = 5,
				toggleInfo = researchInfo
			});
			this.researchScreen.Show(show: false);
		}
	}

	public void CheckResearch(object o)
	{
		if (!(researchInfo.toggle == null))
		{
			bool flag = Components.ResearchCenters.Count <= 0 && !DebugHandler.InstantBuildMode;
			bool active = !flag && activeScreen != null && activeScreen.toggleInfo == researchInfo;
			string tooltip = (flag ? researchTooltipDisabled : researchTooltip);
			ConfigureToggle(researchInfo.toggle, flag, active, tooltip, ToggleToolTipTextStyleSetting);
		}
	}

	public void CheckSkills(object o = null)
	{
		if (skillsInfo != null && !(skillsInfo.toggle == null))
		{
			bool flag = Components.RoleStations.Count <= 0 && !DebugHandler.InstantBuildMode;
			bool active = activeScreen != null && activeScreen.toggleInfo == skillsInfo;
			string tooltip = (flag ? skillsTooltipDisabled : skillsTooltip);
			ConfigureToggle(skillsInfo.toggle, flag, active, tooltip, ToggleToolTipTextStyleSetting);
		}
	}

	public void CheckStarmap(object o = null)
	{
		if (!(starmapInfo.toggle == null))
		{
			bool flag = Components.Telescopes.Count <= 0 && !DebugHandler.InstantBuildMode;
			bool active = activeScreen != null && activeScreen.toggleInfo == starmapInfo;
			string tooltip = (flag ? starmapTooltipDisabled : starmapTooltip);
			ConfigureToggle(starmapInfo.toggle, flag, active, tooltip, ToggleToolTipTextStyleSetting);
		}
	}

	private void ConfigureToggle(KToggle toggle, bool disabled, bool active, string tooltip, TextStyleSetting tooltip_style)
	{
		toggle.interactable = active;
		toggle.GetComponent<KToggle>().interactable = !disabled;
		if (disabled)
		{
			toggle.GetComponentInChildren<ImageToggleState>().SetDisabled();
		}
		else
		{
			toggle.GetComponentInChildren<ImageToggleState>().SetActiveState(active);
		}
		ToolTip component = toggle.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		component.AddMultiStringTooltip(tooltip, tooltip_style);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (activeScreen != null && e.TryConsume(Action.Escape))
		{
			ToggleScreen(activeScreen);
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
			ToggleScreen(activeScreen);
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	private bool ResearchAvailable()
	{
		if (Components.ResearchCenters.Count <= 0)
		{
			return DebugHandler.InstantBuildMode;
		}
		return true;
	}

	private bool SkillsAvailable()
	{
		if (Components.RoleStations.Count <= 0)
		{
			return DebugHandler.InstantBuildMode;
		}
		return true;
	}

	private bool StarmapAvailable()
	{
		if (Components.Telescopes.Count <= 0)
		{
			return DebugHandler.InstantBuildMode;
		}
		return true;
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
				activeScreen.screen.Show();
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
		ToggleScreen(ScreenInfoMatch[toggle_info]);
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
		if (researchScreen == null)
		{
			AddResearchScreen(Object.FindObjectOfType<ResearchScreen>());
		}
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

	public void OpenSkills(MinionIdentity minionIdentity)
	{
		if (activeScreen != ScreenInfoMatch[Instance.skillsInfo])
		{
			skillsScreen.CurrentlySelectedMinion = minionIdentity;
			ToggleScreen(ScreenInfoMatch[Instance.skillsInfo]);
		}
	}
}
