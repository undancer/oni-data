using System;
using Klei.CustomSettings;
using ProcGen;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ColonyDestinationSelectScreen : NewGameFlowScreen
{
	[SerializeField]
	private GameObject destinationMap;

	[SerializeField]
	private GameObject customSettings;

	[SerializeField]
	private KButton backButton;

	[SerializeField]
	private KButton customizeButton;

	[SerializeField]
	private KButton launchButton;

	[SerializeField]
	private KButton shuffleButton;

	[SerializeField]
	private HierarchyReferences locationIcons;

	[SerializeField]
	private RectTransform worldsScrollPanel;

	private const int DESTINATION_HEADER_BUTTON_HEIGHT_CLUSTER = 134;

	private const int DESTINATION_HEADER_BUTTON_HEIGHT_BASE = 76;

	private const int WORLDS_SCROLL_PANEL_HEIGHT_CLUSTER = 466;

	private const int WORLDS_SCROLL_PANEL_HEIGHT_BASE = 524;

	[SerializeField]
	private AsteroidDescriptorPanel destinationProperties;

	[SerializeField]
	private AsteroidDescriptorPanel selectedLocationProperties;

	[SerializeField]
	private KInputTextField coordinate;

	[SerializeField]
	private RectTransform destinationInfoPanel;

	[MyCmpReq]
	private NewGameSettingsPanel newGameSettings;

	[MyCmpReq]
	private DestinationSelectPanel destinationMapPanel;

	private KRandom random;

	private bool isEditingCoordinate;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		backButton.onClick += BackClicked;
		customizeButton.onClick += CustomizeClicked;
		launchButton.onClick += LaunchClicked;
		shuffleButton.onClick += ShuffleClicked;
		destinationMapPanel.OnAsteroidClicked += OnAsteroidClicked;
		KInputTextField kInputTextField = coordinate;
		kInputTextField.onFocus = (System.Action)Delegate.Combine(kInputTextField.onFocus, new System.Action(CoordinateEditStarted));
		coordinate.onEndEdit.AddListener(CoordinateEditFinished);
		if (locationIcons != null)
		{
			bool cloudSavesAvailable = SaveLoader.GetCloudSavesAvailable();
			locationIcons.gameObject.SetActive(cloudSavesAvailable);
		}
		random = new KRandom();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RefreshCloudSavePref();
		RefreshCloudLocalIcon();
		newGameSettings.Init();
		newGameSettings.SetCloseAction(CustomizeClose);
		destinationMapPanel.Init();
		CustomGameSettings.Instance.OnSettingChanged += SettingChanged;
		ShuffleClicked();
		ResizeLayout();
	}

	private void ResizeLayout()
	{
		Vector2 sizeDelta = destinationProperties.clusterDetailsButton.rectTransform().sizeDelta;
		destinationProperties.clusterDetailsButton.rectTransform().sizeDelta = new Vector2(sizeDelta.x, DlcManager.FeatureClusterSpaceEnabled() ? 134 : 76);
		Vector2 sizeDelta2 = worldsScrollPanel.rectTransform().sizeDelta;
		Vector2 anchoredPosition = worldsScrollPanel.rectTransform().anchoredPosition;
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			worldsScrollPanel.rectTransform().anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y + 58f);
		}
		float a = (DlcManager.FeatureClusterSpaceEnabled() ? 466 : 524);
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.gameObject.rectTransform());
		a = Mathf.Min(a, destinationInfoPanel.sizeDelta.y - (float)(DlcManager.FeatureClusterSpaceEnabled() ? 134 : 76) - 22f);
		worldsScrollPanel.rectTransform().sizeDelta = new Vector2(sizeDelta2.x, a);
	}

	protected override void OnCleanUp()
	{
		CustomGameSettings.Instance.OnSettingChanged -= SettingChanged;
		base.OnCleanUp();
	}

	private void RefreshCloudLocalIcon()
	{
		if (locationIcons == null || !SaveLoader.GetCloudSavesAvailable())
		{
			return;
		}
		HierarchyReferences component = locationIcons.GetComponent<HierarchyReferences>();
		LocText component2 = component.GetReference<RectTransform>("LocationText").GetComponent<LocText>();
		KButton component3 = component.GetReference<RectTransform>("CloudButton").GetComponent<KButton>();
		KButton component4 = component.GetReference<RectTransform>("LocalButton").GetComponent<KButton>();
		ToolTip component5 = component3.GetComponent<ToolTip>();
		ToolTip component6 = component4.GetComponent<ToolTip>();
		component5.toolTip = $"{UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP}\n{UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP_EXTRA}";
		component6.toolTip = $"{UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP_LOCAL}\n{UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP_EXTRA}";
		bool flag = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SaveToCloud).id == "Enabled";
		component2.text = (flag ? UI.FRONTEND.LOADSCREEN.CLOUD_SAVE : UI.FRONTEND.LOADSCREEN.LOCAL_SAVE);
		component3.gameObject.SetActive(flag);
		component3.ClearOnClick();
		if (flag)
		{
			component3.onClick += delegate
			{
				CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.SaveToCloud, "Disabled");
				RefreshCloudLocalIcon();
			};
		}
		component4.gameObject.SetActive(!flag);
		component4.ClearOnClick();
		if (!flag)
		{
			component4.onClick += delegate
			{
				CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.SaveToCloud, "Enabled");
				RefreshCloudLocalIcon();
			};
		}
	}

	private void RefreshCloudSavePref()
	{
		if (SaveLoader.GetCloudSavesAvailable())
		{
			string cloudSavesDefaultPref = SaveLoader.GetCloudSavesDefaultPref();
			CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.SaveToCloud, cloudSavesDefaultPref);
		}
	}

	private void BackClicked()
	{
		newGameSettings.Cancel();
		NavigateBackward();
	}

	private void CustomizeClicked()
	{
		newGameSettings.Refresh();
		customSettings.SetActive(value: true);
	}

	private void CustomizeClose()
	{
		customSettings.SetActive(value: false);
	}

	private void LaunchClicked()
	{
		NavigateForward();
	}

	private void ShuffleClicked()
	{
		int num = random.Next();
		newGameSettings.SetSetting(CustomGameSettingConfigs.WorldgenSeed, num.ToString());
	}

	private void CoordinateChanged(string text)
	{
		string[] array = CustomGameSettings.ParseSettingCoordinate(text);
		if (array.Length != 4 || !int.TryParse(array[2], out var _))
		{
			return;
		}
		ClusterLayout clusterLayout = null;
		foreach (string clusterName in SettingsCache.GetClusterNames())
		{
			ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(clusterName);
			if (clusterData.coordinatePrefix == array[1])
			{
				clusterLayout = clusterData;
			}
		}
		if (clusterLayout != null)
		{
			newGameSettings.SetSetting(CustomGameSettingConfigs.ClusterLayout, clusterLayout.filePath);
		}
		newGameSettings.SetSetting(CustomGameSettingConfigs.WorldgenSeed, array[2]);
		newGameSettings.ConsumeSettingsCode(array[3]);
	}

	private void CoordinateEditStarted()
	{
		isEditingCoordinate = true;
	}

	private void CoordinateEditFinished(string text)
	{
		CoordinateChanged(text);
		isEditingCoordinate = false;
		coordinate.text = CustomGameSettings.Instance.GetSettingsCoordinate();
	}

	private void SettingChanged(SettingConfig config, SettingLevel level)
	{
		if (config == CustomGameSettingConfigs.SaveToCloud)
		{
			RefreshCloudLocalIcon();
		}
		if (!isEditingCoordinate)
		{
			coordinate.text = CustomGameSettings.Instance.GetSettingsCoordinate();
		}
		string setting = newGameSettings.GetSetting(CustomGameSettingConfigs.ClusterLayout);
		string setting2 = newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
		destinationMapPanel.UpdateDisplayedClusters();
		int.TryParse(setting2, out var result);
		ColonyDestinationAsteroidBeltData cluster;
		try
		{
			cluster = destinationMapPanel.SelectCluster(setting, result);
		}
		catch
		{
			string defaultAsteroid = destinationMapPanel.GetDefaultAsteroid();
			newGameSettings.SetSetting(CustomGameSettingConfigs.ClusterLayout, defaultAsteroid);
			cluster = destinationMapPanel.SelectCluster(defaultAsteroid, result);
		}
		if (DlcManager.IsContentActive("EXPANSION1_ID"))
		{
			destinationProperties.EnableClusterLocationLabels(enable: true);
			destinationProperties.RefreshAsteroidLines(cluster, selectedLocationProperties);
			destinationProperties.EnableClusterDetails(setActive: true);
			destinationProperties.SetClusterDetailLabels(cluster);
			selectedLocationProperties.headerLabel.SetText(UI.FRONTEND.COLONYDESTINATIONSCREEN.SELECTED_CLUSTER_TRAITS_HEADER);
			destinationProperties.clusterDetailsButton.onClick = delegate
			{
				destinationProperties.SelectWholeClusterDetails(cluster, selectedLocationProperties);
			};
		}
		else
		{
			destinationProperties.EnableClusterDetails(setActive: false);
			destinationProperties.EnableClusterLocationLabels(enable: false);
			destinationProperties.SetParameterDescriptors(cluster.GetParamDescriptors());
			selectedLocationProperties.SetTraitDescriptors(cluster.GetTraitDescriptors());
		}
	}

	private void OnAsteroidClicked(ColonyDestinationAsteroidBeltData cluster)
	{
		newGameSettings.SetSetting(CustomGameSettingConfigs.ClusterLayout, cluster.beltPath);
		ShuffleClicked();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!isEditingCoordinate)
		{
			if (!e.Consumed && e.TryConsume(Action.PanLeft))
			{
				destinationMapPanel.ScrollLeft();
			}
			else if (!e.Consumed && e.TryConsume(Action.PanRight))
			{
				destinationMapPanel.ScrollRight();
			}
			else if (customSettings.activeSelf && !e.Consumed && (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight)))
			{
				CustomizeClose();
			}
			base.OnKeyDown(e);
		}
	}
}
