using System;
using Klei.CustomSettings;
using ProcGen;
using STRINGS;
using TMPro;
using UnityEngine;

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
	private AsteroidDescriptorPanel destinationProperties;

	[SerializeField]
	private AsteroidDescriptorPanel startLocationProperties;

	[SerializeField]
	private TMP_InputField coordinate;

	[MyCmpReq]
	private NewGameSettingsPanel newGameSettings;

	[MyCmpReq]
	private DestinationSelectPanel destinationMapPanel;

	private System.Random random;

	private bool isEditingCoordinate = false;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		backButton.onClick += BackClicked;
		customizeButton.onClick += CustomizeClicked;
		launchButton.onClick += LaunchClicked;
		shuffleButton.onClick += ShuffleClicked;
		destinationMapPanel.OnAsteroidClicked += OnAsteroidClicked;
		TMP_InputField tMP_InputField = coordinate;
		tMP_InputField.onFocus = (System.Action)Delegate.Combine(tMP_InputField.onFocus, new System.Action(CoordinateEditStarted));
		coordinate.onEndEdit.AddListener(CoordinateEditFinished);
		if (locationIcons != null)
		{
			bool cloudSavesAvailable = SaveLoader.GetCloudSavesAvailable();
			locationIcons.gameObject.SetActive(cloudSavesAvailable);
		}
		random = new System.Random();
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
		string id = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SaveToCloud).id;
		bool flag = id == "Enabled";
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
		ColonyDestinationAsteroidBeltData colonyDestinationAsteroidBeltData;
		try
		{
			colonyDestinationAsteroidBeltData = destinationMapPanel.SelectAsteroid(setting, result);
		}
		catch
		{
			string defaultAsteroid = destinationMapPanel.GetDefaultAsteroid();
			newGameSettings.SetSetting(CustomGameSettingConfigs.ClusterLayout, defaultAsteroid);
			colonyDestinationAsteroidBeltData = destinationMapPanel.SelectAsteroid(defaultAsteroid, result);
		}
		destinationProperties.SetDescriptors(colonyDestinationAsteroidBeltData.GetParamDescriptors());
		startLocationProperties.SetDescriptors(colonyDestinationAsteroidBeltData.GetTraitDescriptors());
	}

	private void OnAsteroidClicked(ColonyDestinationAsteroidBeltData asteroid)
	{
		newGameSettings.SetSetting(CustomGameSettingConfigs.ClusterLayout, asteroid.beltPath);
		newGameSettings.SetSetting(CustomGameSettingConfigs.World, asteroid.startWorldPath);
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
			else if (customSettings.activeSelf && !e.Consumed && e.TryConsume(Action.Escape))
			{
				CustomizeClose();
			}
			base.OnKeyDown(e);
		}
	}
}
