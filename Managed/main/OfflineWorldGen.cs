using System;
using System.Collections.Generic;
using System.Threading;
using Klei.CustomSettings;
using ProcGenGame;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OfflineWorldGen")]
public class OfflineWorldGen : KMonoBehaviour
{
	public struct ErrorInfo
	{
		public string errorDesc;

		public Exception exception;
	}

	[Serializable]
	private struct ValidDimensions
	{
		public int width;

		public int height;

		public StringKey name;
	}

	[SerializeField]
	private RectTransform buttonRoot;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private RectTransform chooseLocationPanel;

	[SerializeField]
	private GameObject locationButtonPrefab;

	private const float baseScale = 0.005f;

	private Mutex errorMutex = new Mutex();

	private List<ErrorInfo> errors = new List<ErrorInfo>();

	private ValidDimensions[] validDimensions = new ValidDimensions[1]
	{
		new ValidDimensions
		{
			width = 256,
			height = 384,
			name = UI.FRONTEND.WORLDGENSCREEN.SIZES.STANDARD.key
		}
	};

	public string frontendGameLevel = "frontend";

	public string mainGameLevel = "backend";

	private bool shouldStop;

	private StringKey currentConvertedCurrentStage;

	private float currentPercent;

	public bool debug;

	private bool trackProgress = true;

	private bool doWorldGen;

	[SerializeField]
	private LocText titleText;

	[SerializeField]
	private LocText mainText;

	[SerializeField]
	private LocText updateText;

	[SerializeField]
	private LocText percentText;

	[SerializeField]
	private LocText seedText;

	[SerializeField]
	private KBatchedAnimController meterAnim;

	[SerializeField]
	private KBatchedAnimController asteriodAnim;

	private Cluster clusterLayout;

	private StringKey currentStringKeyRoot;

	private List<LocString> convertList = new List<LocString>
	{
		UI.WORLDGEN.SETTLESIM,
		UI.WORLDGEN.BORDERS,
		UI.WORLDGEN.PROCESSING,
		UI.WORLDGEN.COMPLETELAYOUT,
		UI.WORLDGEN.WORLDLAYOUT,
		UI.WORLDGEN.GENERATENOISE,
		UI.WORLDGEN.BUILDNOISESOURCE,
		UI.WORLDGEN.GENERATESOLARSYSTEM
	};

	private WorldGenProgressStages.Stages currentStage;

	private bool loadTriggered;

	private bool startedExitFlow;

	private int seed;

	private void TrackProgress(string text)
	{
		if (trackProgress)
		{
			Debug.Log(text);
		}
	}

	public static bool CanLoadSave()
	{
		bool flag = true;
		flag = WorldGen.CanLoad(SaveLoader.GetActiveSaveFilePath());
		if (!flag)
		{
			SaveLoader.SetActiveSaveFilePath(null);
			flag = WorldGen.CanLoad(WorldGen.GetSIMSaveFilename(0));
		}
		return flag;
	}

	public void Generate()
	{
		doWorldGen = !CanLoadSave();
		updateText.gameObject.SetActive(value: false);
		percentText.gameObject.SetActive(value: false);
		doWorldGen |= debug;
		if (doWorldGen)
		{
			seedText.text = string.Format(UI.WORLDGEN.USING_PLAYER_SEED, seed);
			titleText.text = UI.FRONTEND.WORLDGENSCREEN.TITLE.ToString();
			mainText.text = UI.WORLDGEN.CHOOSEWORLDSIZE.ToString();
			for (int i = 0; i < this.validDimensions.Length; i++)
			{
				GameObject obj = UnityEngine.Object.Instantiate(buttonPrefab);
				obj.SetActive(value: true);
				RectTransform component = obj.GetComponent<RectTransform>();
				component.SetParent(buttonRoot);
				component.localScale = Vector3.one;
				LocText componentInChildren = obj.GetComponentInChildren<LocText>();
				ValidDimensions validDimensions = this.validDimensions[i];
				componentInChildren.text = validDimensions.name.ToString();
				int idx = i;
				obj.GetComponent<KButton>().onClick += delegate
				{
					DoWorldGen(idx);
					ToggleGenerationUI();
				};
			}
			if (this.validDimensions.Length == 1)
			{
				DoWorldGen(0);
				ToggleGenerationUI();
			}
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
			OnResize();
		}
		else
		{
			titleText.text = UI.FRONTEND.WORLDGENSCREEN.LOADINGGAME.ToString();
			mainText.gameObject.SetActive(value: false);
			currentConvertedCurrentStage = UI.WORLDGEN.COMPLETE.key;
			currentPercent = 1f;
			updateText.gameObject.SetActive(value: false);
			percentText.gameObject.SetActive(value: false);
			RemoveButtons();
		}
		buttonPrefab.SetActive(value: false);
	}

	private void OnResize()
	{
		float num = 1f;
		num = GetComponentInParent<KCanvasScaler>().GetCanvasScale();
		if (asteriodAnim != null)
		{
			asteriodAnim.animScale = 0.005f * (1f / num);
		}
	}

	private void ToggleGenerationUI()
	{
		percentText.gameObject.SetActive(value: false);
		updateText.gameObject.SetActive(value: true);
		titleText.text = UI.FRONTEND.WORLDGENSCREEN.GENERATINGWORLD.ToString();
		if (titleText != null && titleText.gameObject != null)
		{
			titleText.gameObject.SetActive(value: false);
		}
		if (buttonRoot != null && buttonRoot.gameObject != null)
		{
			buttonRoot.gameObject.SetActive(value: false);
		}
	}

	private bool UpdateProgress(StringKey stringKeyRoot, float completePercent, WorldGenProgressStages.Stages stage)
	{
		if (currentStage != stage)
		{
			currentStage = stage;
		}
		if (currentStringKeyRoot.Hash != stringKeyRoot.Hash)
		{
			currentConvertedCurrentStage = stringKeyRoot;
			currentStringKeyRoot = stringKeyRoot;
		}
		else
		{
			int num = (int)completePercent * 10;
			LocString locString = convertList.Find((LocString s) => s.key.Hash == stringKeyRoot.Hash);
			if (num != 0 && locString != null)
			{
				currentConvertedCurrentStage = new StringKey(locString.key.String + num);
			}
		}
		float num2 = 0f;
		float num3 = 0f;
		float num4 = WorldGenProgressStages.StageWeights[(int)stage].Value * completePercent;
		for (int i = 0; i < WorldGenProgressStages.StageWeights.Length; i++)
		{
			num3 += WorldGenProgressStages.StageWeights[i].Value;
			if (i < (int)currentStage)
			{
				num2 += WorldGenProgressStages.StageWeights[i].Value;
			}
		}
		float num5 = (currentPercent = (num2 + num4) / num3);
		return !shouldStop;
	}

	private void Update()
	{
		if (loadTriggered || currentConvertedCurrentStage.String == null)
		{
			return;
		}
		errorMutex.WaitOne();
		int count = errors.Count;
		errorMutex.ReleaseMutex();
		if (count > 0)
		{
			DoExitFlow();
			return;
		}
		updateText.text = Strings.Get(currentConvertedCurrentStage.String);
		if (!debug && currentConvertedCurrentStage.Hash == UI.WORLDGEN.COMPLETE.key.Hash && currentPercent >= 1f && clusterLayout.IsGenerationComplete)
		{
			if (!KCrashReporter.terminateOnError || !KCrashReporter.hasCrash)
			{
				percentText.text = "";
				loadTriggered = true;
				App.LoadScene(mainGameLevel);
			}
		}
		else if (currentPercent < 0f)
		{
			DoExitFlow();
		}
		else
		{
			if (currentPercent > 0f && !percentText.gameObject.activeSelf)
			{
				percentText.gameObject.SetActive(value: false);
			}
			percentText.text = GameUtil.GetFormattedPercent(currentPercent * 100f);
			meterAnim.SetPositionPercent(currentPercent);
		}
	}

	private void DisplayErrors()
	{
		errorMutex.WaitOne();
		if (errors.Count > 0)
		{
			foreach (ErrorInfo error in errors)
			{
				Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, FrontEndManager.Instance.gameObject, force_active: true).PopupConfirmDialog(error.errorDesc, OnConfirmExit, null);
			}
		}
		errorMutex.ReleaseMutex();
	}

	private void DoExitFlow()
	{
		if (!startedExitFlow)
		{
			startedExitFlow = true;
			percentText.text = UI.WORLDGEN.RESTARTING.ToString();
			loadTriggered = true;
			Sim.Shutdown();
			DisplayErrors();
		}
	}

	private void OnConfirmExit()
	{
		App.LoadScene(frontendGameLevel);
	}

	private void RemoveButtons()
	{
		for (int num = buttonRoot.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(buttonRoot.GetChild(num).gameObject);
		}
	}

	private void DoWorldGen(int selectedDimension)
	{
		RemoveButtons();
		DoWorldGenInitialize();
	}

	private void DoWorldGenInitialize()
	{
		string text = "";
		Func<int, WorldGen, bool> shouldSkipWorldCallback = null;
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		seed = int.Parse(currentQualitySetting.id);
		text = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout).id;
		clusterLayout = new Cluster(text, seed, assertMissingTraits: true, skipWorldTraits: false);
		clusterLayout.ShouldSkipWorldCallback = shouldSkipWorldCallback;
		clusterLayout.Generate(UpdateProgress, OnError, seed, seed, seed, seed);
	}

	private void OnError(ErrorInfo error)
	{
		errorMutex.WaitOne();
		errors.Add(error);
		errorMutex.ReleaseMutex();
	}
}
