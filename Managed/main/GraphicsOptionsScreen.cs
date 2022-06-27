using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

internal class GraphicsOptionsScreen : KModalScreen
{
	private struct Settings
	{
		public bool fullscreen;

		public Resolution resolution;

		public int lowRes;

		public int colorSetId;
	}

	[SerializeField]
	private Dropdown resolutionDropdown;

	[SerializeField]
	private MultiToggle lowResToggle;

	[SerializeField]
	private MultiToggle fullscreenToggle;

	[SerializeField]
	private KButton applyButton;

	[SerializeField]
	private KButton doneButton;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private ConfirmDialogScreen confirmPrefab;

	[SerializeField]
	private ConfirmDialogScreen feedbackPrefab;

	[SerializeField]
	private KSlider uiScaleSlider;

	[SerializeField]
	private LocText sliderLabel;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private Dropdown colorModeDropdown;

	[SerializeField]
	private KImage colorExampleLogicOn;

	[SerializeField]
	private KImage colorExampleLogicOff;

	[SerializeField]
	private KImage colorExampleCropHalted;

	[SerializeField]
	private KImage colorExampleCropGrowing;

	[SerializeField]
	private KImage colorExampleCropGrown;

	public static readonly string ResolutionWidthKey = "ResolutionWidth";

	public static readonly string ResolutionHeightKey = "ResolutionHeight";

	public static readonly string RefreshRateKey = "RefreshRate";

	public static readonly string FullScreenKey = "FullScreen";

	public static readonly string LowResKey = "LowResTextures";

	public static readonly string ColorModeKey = "ColorModeID";

	private KCanvasScaler[] CanvasScalers;

	private ConfirmDialogScreen confirmDialog;

	private ConfirmDialogScreen feedbackDialog;

	private List<Resolution> resolutions = new List<Resolution>();

	private List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

	private List<Dropdown.OptionData> colorModeOptions = new List<Dropdown.OptionData>();

	private int colorModeId;

	private bool colorModeChanged;

	private Settings originalSettings;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		title.SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.TITLE);
		originalSettings = CaptureSettings();
		applyButton.isInteractable = false;
		applyButton.onClick += OnApply;
		applyButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.APPLYBUTTON);
		doneButton.onClick += OnDone;
		closeButton.onClick += OnDone;
		doneButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.DONE_BUTTON);
		bool flag = QualitySettings.GetQualityLevel() == 1;
		lowResToggle.ChangeState(flag ? 1 : 0);
		MultiToggle multiToggle = lowResToggle;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, new System.Action(OnLowResToggle));
		lowResToggle.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.LOWRES);
		resolutionDropdown.ClearOptions();
		BuildOptions();
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, (System.Action)delegate
		{
			BuildOptions();
			resolutionDropdown.options = options;
		});
		resolutionDropdown.options = options;
		resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
		fullscreenToggle.ChangeState(Screen.fullScreen ? 1 : 0);
		MultiToggle multiToggle2 = fullscreenToggle;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, new System.Action(OnFullscreenToggle));
		fullscreenToggle.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.FULLSCREEN);
		resolutionDropdown.transform.parent.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.RESOLUTION);
		if (fullscreenToggle.CurrentState == 1)
		{
			int resolutionIndex = GetResolutionIndex(originalSettings.resolution);
			if (resolutionIndex != -1)
			{
				resolutionDropdown.value = resolutionIndex;
			}
		}
		CanvasScalers = UnityEngine.Object.FindObjectsOfType<KCanvasScaler>(includeInactive: true);
		UpdateSliderLabel();
		uiScaleSlider.onValueChanged.AddListener(delegate
		{
			sliderLabel.text = uiScaleSlider.value + "%";
		});
		uiScaleSlider.onReleaseHandle += delegate
		{
			UpdateUIScale(uiScaleSlider.value);
		};
		BuildColorModeOptions();
		colorModeDropdown.options = colorModeOptions;
		colorModeDropdown.onValueChanged.AddListener(OnColorModeChanged);
		int value = 0;
		if (KPlayerPrefs.HasKey(ColorModeKey))
		{
			value = KPlayerPrefs.GetInt(ColorModeKey);
		}
		colorModeDropdown.value = value;
		RefreshColorExamples(originalSettings.colorSetId);
	}

	public static void SetSettingsFromPrefs()
	{
		SetResolutionFromPrefs();
		SetLowResFromPrefs();
	}

	public static void SetLowResFromPrefs()
	{
		int num = 0;
		if (KPlayerPrefs.HasKey(LowResKey))
		{
			num = KPlayerPrefs.GetInt(LowResKey);
			QualitySettings.SetQualityLevel(num, applyExpensiveChanges: true);
		}
		else
		{
			QualitySettings.SetQualityLevel(num, applyExpensiveChanges: true);
		}
		DebugUtil.LogArgs(string.Format("Low Res Textures? {0}", (num == 1) ? "Yes" : "No"));
	}

	public static void SetResolutionFromPrefs()
	{
		int num = Screen.currentResolution.width;
		int num2 = Screen.currentResolution.height;
		int num3 = Screen.currentResolution.refreshRate;
		bool flag = Screen.fullScreen;
		if (KPlayerPrefs.HasKey(ResolutionWidthKey) && KPlayerPrefs.HasKey(ResolutionHeightKey))
		{
			int @int = KPlayerPrefs.GetInt(ResolutionWidthKey);
			int int2 = KPlayerPrefs.GetInt(ResolutionHeightKey);
			int int3 = KPlayerPrefs.GetInt(RefreshRateKey, Screen.currentResolution.refreshRate);
			bool flag2 = KPlayerPrefs.GetInt(FullScreenKey, Screen.fullScreen ? 1 : 0) == 1;
			if (int2 <= 1 || @int <= 1)
			{
				DebugUtil.LogArgs("Saved resolution was invalid, ignoring...");
			}
			else
			{
				num = @int;
				num2 = int2;
				num3 = int3;
				flag = flag2;
			}
		}
		if (num <= 1 || num2 <= 1)
		{
			DebugUtil.LogWarningArgs("Detected a degenerate resolution, attempting to fix...");
			Resolution[] array = Screen.resolutions;
			for (int i = 0; i < array.Length; i++)
			{
				Resolution resolution = array[i];
				if (resolution.width == 1920)
				{
					num = resolution.width;
					num2 = resolution.height;
					num3 = 0;
				}
			}
			if (num <= 1 || num2 <= 1)
			{
				array = Screen.resolutions;
				for (int i = 0; i < array.Length; i++)
				{
					Resolution resolution2 = array[i];
					if (resolution2.width == 1280)
					{
						num = resolution2.width;
						num2 = resolution2.height;
						num3 = 0;
					}
				}
			}
			if (num <= 1 || num2 <= 1)
			{
				array = Screen.resolutions;
				for (int i = 0; i < array.Length; i++)
				{
					Resolution resolution3 = array[i];
					if (resolution3.width > 1 && resolution3.height > 1 && resolution3.refreshRate > 0)
					{
						num = resolution3.width;
						num2 = resolution3.height;
						num3 = 0;
					}
				}
			}
			if (num <= 1 || num2 <= 1)
			{
				string text = "Could not find a suitable resolution for this screen! Reported available resolutions are:";
				array = Screen.resolutions;
				for (int i = 0; i < array.Length; i++)
				{
					Resolution resolution4 = array[i];
					text += $"\n{resolution4.width}x{resolution4.height} @ {resolution4.refreshRate}hz";
				}
				Debug.LogError(text);
				num = 1280;
				num2 = 720;
				flag = false;
				num3 = 0;
			}
		}
		DebugUtil.LogArgs($"Applying resolution {num}x{num2} @{num3}hz (fullscreen: {flag})");
		Screen.SetResolution(num, num2, flag, num3);
	}

	public static void SetColorModeFromPrefs()
	{
		int num = 0;
		if (KPlayerPrefs.HasKey(ColorModeKey))
		{
			num = KPlayerPrefs.GetInt(ColorModeKey);
		}
		GlobalAssets.Instance.colorSet = GlobalAssets.Instance.colorSetOptions[num];
	}

	public static void OnResize()
	{
		Settings settings = default(Settings);
		settings.resolution = Screen.currentResolution;
		settings.resolution.width = Screen.width;
		settings.resolution.height = Screen.height;
		settings.fullscreen = Screen.fullScreen;
		settings.lowRes = QualitySettings.GetQualityLevel();
		settings.colorSetId = Array.IndexOf(GlobalAssets.Instance.colorSetOptions, GlobalAssets.Instance.colorSet);
		SaveSettingsToPrefs(settings);
	}

	private static void SaveSettingsToPrefs(Settings settings)
	{
		KPlayerPrefs.SetInt(LowResKey, settings.lowRes);
		Debug.LogFormat("Screen resolution updated, saving values to prefs: {0}x{1} @ {2}, fullscreen: {3}", settings.resolution.width, settings.resolution.height, settings.resolution.refreshRate, settings.fullscreen);
		KPlayerPrefs.SetInt(ResolutionWidthKey, settings.resolution.width);
		KPlayerPrefs.SetInt(ResolutionHeightKey, settings.resolution.height);
		KPlayerPrefs.SetInt(RefreshRateKey, settings.resolution.refreshRate);
		KPlayerPrefs.SetInt(FullScreenKey, settings.fullscreen ? 1 : 0);
		KPlayerPrefs.SetInt(ColorModeKey, settings.colorSetId);
	}

	private void UpdateUIScale(float value)
	{
		CanvasScalers = UnityEngine.Object.FindObjectsOfType<KCanvasScaler>(includeInactive: true);
		KCanvasScaler[] canvasScalers = CanvasScalers;
		foreach (KCanvasScaler obj in canvasScalers)
		{
			float userScale = value / 100f;
			obj.SetUserScale(userScale);
			KPlayerPrefs.SetFloat(KCanvasScaler.UIScalePrefKey, value);
		}
		ScreenResize.Instance.TriggerResize();
		UpdateSliderLabel();
	}

	private void UpdateSliderLabel()
	{
		if (CanvasScalers != null && CanvasScalers.Length != 0 && CanvasScalers[0] != null)
		{
			uiScaleSlider.value = CanvasScalers[0].GetUserScale() * 100f;
			sliderLabel.text = uiScaleSlider.value + "%";
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			resolutionDropdown.Hide();
			Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void BuildOptions()
	{
		options.Clear();
		resolutions.Clear();
		Resolution item = default(Resolution);
		item.width = Screen.width;
		item.height = Screen.height;
		item.refreshRate = Screen.currentResolution.refreshRate;
		options.Add(new Dropdown.OptionData(item.ToString()));
		resolutions.Add(item);
		Resolution[] array = Screen.resolutions;
		for (int i = 0; i < array.Length; i++)
		{
			Resolution item2 = array[i];
			if (item2.height >= 720)
			{
				options.Add(new Dropdown.OptionData(item2.ToString()));
				resolutions.Add(item2);
			}
		}
	}

	private void BuildColorModeOptions()
	{
		colorModeOptions.Clear();
		for (int i = 0; i < GlobalAssets.Instance.colorSetOptions.Length; i++)
		{
			colorModeOptions.Add(new Dropdown.OptionData(Strings.Get(GlobalAssets.Instance.colorSetOptions[i].settingName)));
		}
	}

	private void RefreshColorExamples(int idx)
	{
		Color32 logicOn = GlobalAssets.Instance.colorSetOptions[idx].logicOn;
		Color32 logicOff = GlobalAssets.Instance.colorSetOptions[idx].logicOff;
		Color32 cropHalted = GlobalAssets.Instance.colorSetOptions[idx].cropHalted;
		Color32 cropGrowing = GlobalAssets.Instance.colorSetOptions[idx].cropGrowing;
		Color32 cropGrown = GlobalAssets.Instance.colorSetOptions[idx].cropGrown;
		logicOn.a = byte.MaxValue;
		logicOff.a = byte.MaxValue;
		cropHalted.a = byte.MaxValue;
		cropGrowing.a = byte.MaxValue;
		cropGrown.a = byte.MaxValue;
		colorExampleLogicOn.color = logicOn;
		colorExampleLogicOff.color = logicOff;
		colorExampleCropHalted.color = cropHalted;
		colorExampleCropGrowing.color = cropGrowing;
		colorExampleCropGrown.color = cropGrown;
	}

	private int GetResolutionIndex(Resolution resolution)
	{
		int num = -1;
		int result = -1;
		for (int i = 0; i < resolutions.Count; i++)
		{
			Resolution resolution2 = resolutions[i];
			if (resolution2.width == resolution.width && resolution2.height == resolution.height && resolution2.refreshRate == 0)
			{
				result = i;
			}
			if (resolution2.width == resolution.width && resolution2.height == resolution.height && Math.Abs(resolution2.refreshRate - resolution.refreshRate) <= 1)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			return num;
		}
		return result;
	}

	private Settings CaptureSettings()
	{
		Settings result = default(Settings);
		result.fullscreen = Screen.fullScreen;
		Resolution resolution = default(Resolution);
		resolution.width = Screen.width;
		resolution.height = Screen.height;
		resolution.refreshRate = Screen.currentResolution.refreshRate;
		result.resolution = resolution;
		result.lowRes = QualitySettings.GetQualityLevel();
		result.colorSetId = Array.IndexOf(GlobalAssets.Instance.colorSetOptions, GlobalAssets.Instance.colorSet);
		return result;
	}

	private void OnApply()
	{
		try
		{
			Settings new_settings = default(Settings);
			new_settings.resolution = resolutions[resolutionDropdown.value];
			new_settings.fullscreen = ((fullscreenToggle.CurrentState != 0) ? true : false);
			new_settings.lowRes = lowResToggle.CurrentState;
			new_settings.colorSetId = colorModeId;
			if (GlobalAssets.Instance.colorSetOptions[colorModeId] != GlobalAssets.Instance.colorSet)
			{
				colorModeChanged = true;
			}
			ApplyConfirmSettings(new_settings, delegate
			{
				applyButton.isInteractable = false;
				if (colorModeChanged)
				{
					feedbackDialog = Util.KInstantiateUI(confirmPrefab.gameObject, base.transform.gameObject).GetComponent<ConfirmDialogScreen>();
					feedbackDialog.PopupConfirmDialog(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.COLORBLIND_FEEDBACK.text, null, null, UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.COLORBLIND_FEEDBACK_BUTTON.text, delegate
					{
						App.OpenWebURL("https://forums.kleientertainment.com/forums/topic/117325-color-blindness-feedback/");
					});
					feedbackDialog.gameObject.SetActive(value: true);
				}
				colorModeChanged = false;
				SaveSettingsToPrefs(new_settings);
			});
		}
		catch (Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Failed to apply graphics options!\nResolutions:");
			foreach (Resolution resolution in resolutions)
			{
				stringBuilder.Append("\t" + resolution.ToString() + "\n");
			}
			stringBuilder.Append("Selected Resolution Idx: " + resolutionDropdown.value);
			stringBuilder.Append("FullScreen: " + fullscreenToggle.CurrentState);
			Debug.LogError(stringBuilder.ToString());
			throw ex;
		}
	}

	public void OnDone()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void RefreshApplyButton()
	{
		Settings settings = CaptureSettings();
		if (settings.fullscreen && fullscreenToggle.CurrentState == 0)
		{
			applyButton.isInteractable = true;
			return;
		}
		if (!settings.fullscreen && fullscreenToggle.CurrentState == 1)
		{
			applyButton.isInteractable = true;
			return;
		}
		if (settings.lowRes != lowResToggle.CurrentState)
		{
			applyButton.isInteractable = true;
			return;
		}
		if (settings.colorSetId != colorModeId)
		{
			applyButton.isInteractable = true;
			return;
		}
		int resolutionIndex = GetResolutionIndex(settings.resolution);
		applyButton.isInteractable = resolutionDropdown.value != resolutionIndex;
	}

	private void OnFullscreenToggle()
	{
		fullscreenToggle.ChangeState((fullscreenToggle.CurrentState == 0) ? 1 : 0);
		RefreshApplyButton();
	}

	private void OnResolutionChanged(int idx)
	{
		RefreshApplyButton();
	}

	private void OnColorModeChanged(int idx)
	{
		colorModeId = idx;
		RefreshApplyButton();
		RefreshColorExamples(colorModeId);
	}

	private void OnLowResToggle()
	{
		lowResToggle.ChangeState((lowResToggle.CurrentState == 0) ? 1 : 0);
		RefreshApplyButton();
	}

	private void ApplyConfirmSettings(Settings new_settings, System.Action on_confirm)
	{
		Settings current_settings = CaptureSettings();
		ApplySettings(new_settings);
		confirmDialog = Util.KInstantiateUI(confirmPrefab.gameObject, base.transform.gameObject).GetComponent<ConfirmDialogScreen>();
		System.Action action = delegate
		{
			ApplySettings(current_settings);
		};
		Coroutine timer = StartCoroutine(Timer(15f, action));
		confirmDialog.onDeactivateCB = delegate
		{
			StopCoroutine(timer);
		};
		confirmDialog.PopupConfirmDialog(colorModeChanged ? UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.ACCEPT_CHANGES_STRING_COLOR.text : UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.ACCEPT_CHANGES.text, on_confirm, action);
		confirmDialog.gameObject.SetActive(value: true);
	}

	private void ApplySettings(Settings new_settings)
	{
		Resolution resolution = new_settings.resolution;
		Screen.SetResolution(resolution.width, resolution.height, new_settings.fullscreen, resolution.refreshRate);
		Screen.fullScreen = new_settings.fullscreen;
		int resolutionIndex = GetResolutionIndex(new_settings.resolution);
		if (resolutionIndex != -1)
		{
			resolutionDropdown.value = resolutionIndex;
		}
		GlobalAssets.Instance.colorSet = GlobalAssets.Instance.colorSetOptions[new_settings.colorSetId];
		Debug.Log("Applying low res settings " + new_settings.lowRes + " / existing is " + QualitySettings.GetQualityLevel());
		if (QualitySettings.GetQualityLevel() != new_settings.lowRes)
		{
			QualitySettings.SetQualityLevel(new_settings.lowRes, applyExpensiveChanges: true);
		}
	}

	private IEnumerator Timer(float time, System.Action revert)
	{
		yield return new WaitForSeconds(time);
		if (confirmDialog != null)
		{
			confirmDialog.Deactivate();
			revert();
		}
	}

	private void Update()
	{
		Debug.developerConsoleVisible = false;
	}
}
