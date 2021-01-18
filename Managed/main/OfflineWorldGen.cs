using System;
using System.Collections.Generic;
using System.Threading;
using Klei.CustomSettings;
using ProcGen;
using ProcGenGame;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;
using VoronoiTree;

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

	private WorldGen worldGen;

	private List<VoronoiTree.Node> startNodes;

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

	private bool shownStartingLocations;

	private bool startedExitFlow;

	private bool generateThreadComplete;

	private bool renderThreadComplete;

	private bool firstPassGeneration;

	private bool secondPassGeneration;

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
			flag = WorldGen.CanLoad(WorldGen.SIM_SAVE_FILENAME);
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
				GameObject gameObject = UnityEngine.Object.Instantiate(buttonPrefab);
				gameObject.SetActive(value: true);
				RectTransform component = gameObject.GetComponent<RectTransform>();
				component.SetParent(buttonRoot);
				component.localScale = Vector3.one;
				LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
				ValidDimensions validDimensions = this.validDimensions[i];
				componentInChildren.text = validDimensions.name.ToString();
				int idx = i;
				gameObject.GetComponent<KButton>().onClick += delegate
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
			currentPercent = 100f;
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

	private void ChooseBaseLocation(VoronoiTree.Node startNode)
	{
		worldGen.ChooseBaseLocation(startNode);
		DoRenderWorld();
		RemoveLocationButtons();
	}

	private void ShowStartingLocationChoices()
	{
		if (titleText != null)
		{
			titleText.text = "Choose Starting Location";
		}
		startNodes = worldGen.WorldLayout.GetStartNodes();
		startNodes.Shuffle();
		if (startNodes.Count == 0)
		{
			DoRenderWorld();
			RemoveLocationButtons();
			return;
		}
		if (startNodes.Count > 0)
		{
			ChooseBaseLocation(startNodes[0]);
			return;
		}
		List<SubWorld> list = new List<SubWorld>();
		for (int i = 0; i < startNodes.Count; i++)
		{
			Tree tree = startNodes[i] as Tree;
			if (tree == null)
			{
				tree = worldGen.GetOverworldForNode(startNodes[i] as Leaf);
				if (tree == null)
				{
					continue;
				}
			}
			SubWorld subWorldForNode = worldGen.GetSubWorldForNode(tree);
			if (subWorldForNode == null || list.Contains(subWorldForNode))
			{
				continue;
			}
			list.Add(subWorldForNode);
			GameObject gameObject = UnityEngine.Object.Instantiate(locationButtonPrefab);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.SetParent(chooseLocationPanel);
			component.localScale = Vector3.one;
			Text componentInChildren = gameObject.GetComponentInChildren<Text>();
			SubWorld subWorld = null;
			Tree parent = startNodes[i].parent;
			while (subWorld == null && parent != null)
			{
				subWorld = worldGen.GetSubWorldForNode(parent);
				if (subWorld == null)
				{
					parent = parent.parent;
				}
			}
			TagSet tagSet = new TagSet(startNodes[i].tags);
			tagSet.Remove(WorldGenTags.Feature);
			tagSet.Remove(WorldGenTags.StartLocation);
			tagSet.Remove(WorldGenTags.IgnoreCaveOverride);
			componentInChildren.text = tagSet.ToString();
			int idx = i;
			Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
			buttonClickedEvent.AddListener(delegate
			{
				ChooseBaseLocation(startNodes[idx]);
			});
			gameObject.GetComponent<Button>().onClick = buttonClickedEvent;
		}
	}

	private void RemoveLocationButtons()
	{
		for (int num = chooseLocationPanel.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(chooseLocationPanel.GetChild(num).gameObject);
		}
		if (titleText != null && titleText.gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(titleText.gameObject);
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
			int num = (int)completePercent / 10;
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
			num3 += WorldGenProgressStages.StageWeights[i].Value * 100f;
			if (i < (int)currentStage)
			{
				num2 += WorldGenProgressStages.StageWeights[i].Value * 100f;
			}
		}
		float num5 = (currentPercent = 100f * ((num2 + num4) / num3));
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
		if (!debug && currentConvertedCurrentStage.Hash == UI.WORLDGEN.COMPLETE.key.Hash && currentPercent >= 100f)
		{
			if (!KCrashReporter.terminateOnError || !KCrashReporter.hasCrash)
			{
				percentText.text = "";
				loadTriggered = true;
				App.LoadScene(mainGameLevel);
			}
			return;
		}
		if (currentPercent < 0f)
		{
			DoExitFlow();
			return;
		}
		if (currentPercent > 0f && !percentText.gameObject.activeSelf)
		{
			percentText.gameObject.SetActive(value: false);
		}
		percentText.text = GameUtil.GetFormattedPercent(currentPercent);
		meterAnim.SetPositionPercent(currentPercent / 100f);
		if (firstPassGeneration)
		{
			generateThreadComplete = worldGen.IsGenerateComplete();
			if (!generateThreadComplete)
			{
				renderThreadComplete = false;
			}
		}
		if (secondPassGeneration)
		{
			renderThreadComplete = worldGen.IsRenderComplete();
		}
		if (!shownStartingLocations && firstPassGeneration && generateThreadComplete)
		{
			shownStartingLocations = true;
			ShowStartingLocationChoices();
		}
		if (renderThreadComplete)
		{
			_ = 0 + 1;
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
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World);
		SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		seed = int.Parse(currentQualitySetting2.id);
		List<string> randomTraits = SettingsCache.GetRandomTraits(seed);
		worldGen = new WorldGen(currentQualitySetting.id, randomTraits, assertMissingTraits: true);
		Vector2I worldsize = worldGen.Settings.world.worldsize;
		GridSettings.Reset(worldsize.x, worldsize.y);
		worldGen.Initialise(UpdateProgress, OnError, seed, seed, seed, seed);
		firstPassGeneration = true;
		worldGen.GenerateOfflineThreaded();
	}

	private void DoRenderWorld()
	{
		firstPassGeneration = false;
		secondPassGeneration = true;
		worldGen.RenderWorldThreaded();
	}

	private void OnError(ErrorInfo error)
	{
		errorMutex.WaitOne();
		errors.Add(error);
		errorMutex.ReleaseMutex();
	}
}
