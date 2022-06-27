using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using FMOD.Studio;
using Klei;
using Klei.AI;
using Klei.CustomSettings;
using KSerialization;
using ProcGenGame;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[AddComponentMenu("KMonoBehaviour/scripts/Game")]
public class Game : KMonoBehaviour
{
	[Serializable]
	public struct SavedInfo
	{
		public bool discoveredSurface;

		public bool discoveredOilField;

		public bool curedDisease;

		public bool blockedCometWithBunkerDoor;

		public Dictionary<Tag, float> creaturePoopAmount;

		public Dictionary<Tag, float> powerCreatedbyGeneratorType;

		[OnDeserialized]
		private void OnDeserialized()
		{
			InitializeEmptyVariables();
		}

		public void InitializeEmptyVariables()
		{
			if (creaturePoopAmount == null)
			{
				creaturePoopAmount = new Dictionary<Tag, float>();
			}
			if (powerCreatedbyGeneratorType == null)
			{
				powerCreatedbyGeneratorType = new Dictionary<Tag, float>();
			}
		}
	}

	public struct CallbackInfo
	{
		public System.Action cb;

		public bool manuallyRelease;

		public CallbackInfo(System.Action cb, bool manually_release = false)
		{
			this.cb = cb;
			manuallyRelease = manually_release;
		}
	}

	public struct ComplexCallbackInfo<DataType>
	{
		public Action<DataType, object> cb;

		public object callbackData;

		public string debugInfo;

		public ComplexCallbackInfo(Action<DataType, object> cb, object callback_data, string debug_info)
		{
			this.cb = cb;
			debugInfo = debug_info;
			callbackData = callback_data;
		}
	}

	public class ComplexCallbackHandleVector<DataType>
	{
		private HandleVector<ComplexCallbackInfo<DataType>> baseMgr;

		private Dictionary<int, string> releaseInfo = new Dictionary<int, string>();

		public ComplexCallbackHandleVector(int initial_size)
		{
			baseMgr = new HandleVector<ComplexCallbackInfo<DataType>>(initial_size);
		}

		public HandleVector<ComplexCallbackInfo<DataType>>.Handle Add(Action<DataType, object> cb, object callback_data, string debug_info)
		{
			return baseMgr.Add(new ComplexCallbackInfo<DataType>(cb, callback_data, debug_info));
		}

		public ComplexCallbackInfo<DataType> GetItem(HandleVector<ComplexCallbackInfo<DataType>>.Handle handle)
		{
			try
			{
				return baseMgr.GetItem(handle);
			}
			catch (Exception ex)
			{
				baseMgr.UnpackHandleUnchecked(handle, out var _, out var index);
				string value = null;
				if (releaseInfo.TryGetValue(index, out value))
				{
					KCrashReporter.Assert(condition: false, "Trying to get data for handle that was already released by " + value);
				}
				else
				{
					KCrashReporter.Assert(condition: false, "Trying to get data for handle that was released ...... magically");
				}
				throw ex;
			}
		}

		public ComplexCallbackInfo<DataType> Release(HandleVector<ComplexCallbackInfo<DataType>>.Handle handle, string release_info)
		{
			byte version;
			int index;
			try
			{
				baseMgr.UnpackHandle(handle, out version, out index);
				releaseInfo[index] = release_info;
				return baseMgr.Release(handle);
			}
			catch (Exception ex)
			{
				baseMgr.UnpackHandleUnchecked(handle, out version, out index);
				string value = null;
				if (releaseInfo.TryGetValue(index, out value))
				{
					KCrashReporter.Assert(condition: false, release_info + "is trying to release handle but it was already released by " + value);
				}
				else
				{
					KCrashReporter.Assert(condition: false, release_info + "is trying to release a handle that was already released by some unknown thing");
				}
				throw ex;
			}
		}

		public void Clear()
		{
			baseMgr.Clear();
		}

		public bool IsVersionValid(HandleVector<ComplexCallbackInfo<DataType>>.Handle handle)
		{
			return baseMgr.IsVersionValid(handle);
		}
	}

	public enum TemperatureOverlayModes
	{
		AbsoluteTemperature,
		AdaptiveTemperature,
		HeatFlow,
		StateChange
	}

	[Serializable]
	public class ConduitVisInfo
	{
		public GameObject prefab;

		[Header("Main View")]
		public Color32 tint;

		public Color32 insulatedTint;

		public Color32 radiantTint;

		[Header("Overlay")]
		public string overlayTintName;

		public string overlayInsulatedTintName;

		public string overlayRadiantTintName;

		public Vector2 overlayMassScaleRange = new Vector2f(1f, 1000f);

		public Vector2 overlayMassScaleValues = new Vector2f(0.1f, 1f);
	}

	private class WorldRegion
	{
		private Vector2I min;

		private Vector2I max;

		public bool isActive;

		public Vector2I regionMin => min;

		public Vector2I regionMax => max;

		public void UpdateGameActiveRegion(int x0, int y0, int x1, int y1)
		{
			min.x = Mathf.Max(0, x0);
			min.y = Mathf.Max(0, y0);
			max.x = Mathf.Max(x1, regionMax.x);
			max.y = Mathf.Max(y1, regionMax.y);
		}

		public void UpdateGameActiveRegion(Vector2I simActiveRegionMin, Vector2I simActiveRegionMax)
		{
			min = simActiveRegionMin;
			max = simActiveRegionMax;
		}
	}

	public class SimActiveRegion
	{
		public Pair<Vector2I, Vector2I> region;

		public float currentSunlightIntensity;

		public float currentCosmicRadiationIntensity;

		public SimActiveRegion()
		{
			region = default(Pair<Vector2I, Vector2I>);
			currentSunlightIntensity = FIXEDTRAITS.SUNLIGHT.DEFAULT_VALUE;
			currentCosmicRadiationIntensity = FIXEDTRAITS.COSMICRADIATION.DEFAULT_VALUE;
		}
	}

	private enum SpawnRotationConfig
	{
		Normal,
		StringName
	}

	[Serializable]
	private struct SpawnRotationData
	{
		public string animName;

		public bool flip;
	}

	[Serializable]
	private struct SpawnPoolData
	{
		[HashedEnum]
		public SpawnFXHashes id;

		public int initialCount;

		public Color32 colour;

		public GameObject fxPrefab;

		public string initialAnim;

		public Vector3 spawnOffset;

		public Vector2 spawnRandomOffset;

		public SpawnRotationConfig rotationConfig;

		public SpawnRotationData[] rotationData;
	}

	[Serializable]
	private class Settings
	{
		public int nextUniqueID;

		public int gameID;

		public Settings(Game game)
		{
			nextUniqueID = KPrefabID.NextUniqueID;
			gameID = KleiMetrics.GameID();
		}

		public Settings()
		{
		}
	}

	public class GameSaveData
	{
		public ConduitFlow gasConduitFlow;

		public ConduitFlow liquidConduitFlow;

		public FallingWater fallingWater;

		public UnstableGroundManager unstableGround;

		public WorldDetailSave worldDetail;

		public CustomGameSettings customGameSettings;

		public bool debugWasUsed;

		public bool autoPrioritizeRoles;

		public bool advancedPersonalPriorities;

		public SavedInfo savedInfo;

		public string dateGenerated;

		public List<uint> changelistsPlayedOn;
	}

	public delegate void CansaveCB();

	public delegate void SavingPreCB(CansaveCB cb);

	public delegate void SavingActiveCB();

	public delegate void SavingPostCB();

	[Serializable]
	public struct LocationColours
	{
		public Color unreachable;

		public Color invalidLocation;

		public Color validLocation;

		public Color requiresRole;

		public Color unreachable_requiresRole;
	}

	[Serializable]
	public class UIColours
	{
		[SerializeField]
		private LocationColours digColours;

		[SerializeField]
		private LocationColours buildColours;

		public LocationColours Dig => digColours;

		public LocationColours Build => buildColours;
	}

	private static readonly string NextUniqueIDKey = "NextUniqueID";

	public static string clusterId = null;

	private PlayerController playerController;

	private CameraController cameraController;

	public Action<GameSaveData> OnSave;

	public Action<GameSaveData> OnLoad;

	[NonSerialized]
	public bool baseAlreadyCreated;

	[NonSerialized]
	public bool autoPrioritizeRoles;

	[NonSerialized]
	public bool advancedPersonalPriorities;

	public SavedInfo savedInfo;

	public static bool quitting = false;

	public AssignmentManager assignmentManager;

	public GameObject playerPrefab;

	public GameObject screenManagerPrefab;

	public GameObject cameraControllerPrefab;

	private Camera m_CachedCamera;

	public GameObject tempIntroScreenPrefab;

	public static int BlockSelectionLayerMask;

	public static int PickupableLayer;

	public Element VisualTunerElement;

	public float currentFallbackSunlightIntensity;

	public RoomProber roomProber;

	public FetchManager fetchManager;

	public EdiblesManager ediblesManager;

	public SpacecraftManager spacecraftManager;

	public UserMenu userMenu;

	public Unlocks unlocks;

	public Timelapser timelapser;

	private bool sandboxModeActive;

	public HandleVector<CallbackInfo> callbackManager = new HandleVector<CallbackInfo>(256);

	public List<int> callbackManagerManuallyReleasedHandles = new List<int>();

	public ComplexCallbackHandleVector<int> simComponentCallbackManager = new ComplexCallbackHandleVector<int>(256);

	public ComplexCallbackHandleVector<Sim.MassConsumedCallback> massConsumedCallbackManager = new ComplexCallbackHandleVector<Sim.MassConsumedCallback>(64);

	public ComplexCallbackHandleVector<Sim.MassEmittedCallback> massEmitCallbackManager = new ComplexCallbackHandleVector<Sim.MassEmittedCallback>(64);

	public ComplexCallbackHandleVector<Sim.DiseaseConsumptionCallback> diseaseConsumptionCallbackManager = new ComplexCallbackHandleVector<Sim.DiseaseConsumptionCallback>(64);

	public ComplexCallbackHandleVector<Sim.ConsumedRadiationCallback> radiationConsumedCallbackManager = new ComplexCallbackHandleVector<Sim.ConsumedRadiationCallback>(256);

	[NonSerialized]
	public Player LocalPlayer;

	[SerializeField]
	public TextAsset maleNamesFile;

	[SerializeField]
	public TextAsset femaleNamesFile;

	[NonSerialized]
	public World world;

	[NonSerialized]
	public CircuitManager circuitManager;

	[NonSerialized]
	public EnergySim energySim;

	[NonSerialized]
	public LogicCircuitManager logicCircuitManager;

	private GameScreenManager screenMgr;

	public UtilityNetworkManager<FlowUtilityNetwork, Vent> gasConduitSystem;

	public UtilityNetworkManager<FlowUtilityNetwork, Vent> liquidConduitSystem;

	public UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem;

	public UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem;

	public UtilityNetworkTubesManager travelTubeSystem;

	public UtilityNetworkManager<FlowUtilityNetwork, SolidConduit> solidConduitSystem;

	public ConduitFlow gasConduitFlow;

	public ConduitFlow liquidConduitFlow;

	public SolidConduitFlow solidConduitFlow;

	public Accumulators accumulators;

	public PlantElementAbsorbers plantElementAbsorbers;

	public TemperatureOverlayModes temperatureOverlayMode;

	public bool showExpandedTemperatures;

	public List<Tag> tileOverlayFilters = new List<Tag>();

	public bool showGasConduitDisease;

	public bool showLiquidConduitDisease;

	public ConduitFlowVisualizer gasFlowVisualizer;

	public ConduitFlowVisualizer liquidFlowVisualizer;

	public SolidConduitFlowVisualizer solidFlowVisualizer;

	public ConduitTemperatureManager conduitTemperatureManager;

	public ConduitDiseaseManager conduitDiseaseManager;

	public MingleCellTracker mingleCellTracker;

	private int simSubTick;

	private bool hasFirstSimTickRun;

	private float simDt;

	public string dateGenerated;

	public List<uint> changelistsPlayedOn;

	[SerializeField]
	public ConduitVisInfo liquidConduitVisInfo;

	[SerializeField]
	public ConduitVisInfo gasConduitVisInfo;

	[SerializeField]
	public ConduitVisInfo solidConduitVisInfo;

	[SerializeField]
	private Material liquidFlowMaterial;

	[SerializeField]
	private Material gasFlowMaterial;

	[SerializeField]
	private Color flowColour;

	private Vector3 gasFlowPos;

	private Vector3 liquidFlowPos;

	private Vector3 solidFlowPos;

	public bool drawStatusItems = true;

	private List<SolidInfo> solidInfo = new List<SolidInfo>();

	private List<Klei.CallbackInfo> callbackInfo = new List<Klei.CallbackInfo>();

	private List<SolidInfo> gameSolidInfo = new List<SolidInfo>();

	private bool IsPaused;

	private HashSet<int> solidChangedFilter = new HashSet<int>();

	private HashedString lastDrawnOverlayMode;

	private BuildingCellVisualizer previewVisualizer;

	public SafetyConditions safetyConditions = new SafetyConditions();

	public SimData simData = new SimData();

	[MyCmpGet]
	private GameScenePartitioner gameScenePartitioner;

	private bool gameStarted;

	private static readonly EventSystem.IntraObjectHandler<Game> MarkStatusItemRendererDirtyDelegate = new EventSystem.IntraObjectHandler<Game>(delegate(Game component, object data)
	{
		component.MarkStatusItemRendererDirty(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Game> ActiveWorldChangedDelegate = new EventSystem.IntraObjectHandler<Game>(delegate(Game component, object data)
	{
		component.ForceOverlayUpdate(clearLastMode: true);
	});

	private ushort[] activeFX;

	public bool debugWasUsed;

	private bool isLoading;

	private List<SimActiveRegion> simActiveRegions = new List<SimActiveRegion>();

	private HashedString previousOverlayMode = OverlayModes.None.ID;

	private float previousGasConduitFlowDiscreteLerpPercent = -1f;

	private float previousLiquidConduitFlowDiscreteLerpPercent = -1f;

	private float previousSolidConduitFlowDiscreteLerpPercent = -1f;

	[SerializeField]
	private SpawnPoolData[] fxSpawnData;

	private Dictionary<int, Action<Vector3, float>> fxSpawner = new Dictionary<int, Action<Vector3, float>>();

	private Dictionary<int, ObjectPool> fxPools = new Dictionary<int, ObjectPool>();

	private SavingPreCB activatePreCB;

	private SavingActiveCB activateActiveCB;

	private SavingPostCB activatePostCB;

	[SerializeField]
	public UIColours uiColours = new UIColours();

	private float lastTimeWorkStarted = float.NegativeInfinity;

	public KInputHandler inputHandler { get; set; }

	public static Game Instance { get; private set; }

	public Camera MainCamera
	{
		get
		{
			if (m_CachedCamera == null)
			{
				m_CachedCamera = Camera.main;
			}
			return m_CachedCamera;
		}
	}

	public bool SaveToCloudActive
	{
		get
		{
			return CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SaveToCloud).id == "Enabled";
		}
		set
		{
			string value2 = (value ? "Enabled" : "Disabled");
			CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.SaveToCloud, value2);
		}
	}

	public bool FastWorkersModeActive
	{
		get
		{
			return CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.FastWorkersMode).id == "Enabled";
		}
		set
		{
			string value2 = (value ? "Enabled" : "Disabled");
			CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.FastWorkersMode, value2);
		}
	}

	public bool SandboxModeActive
	{
		get
		{
			return sandboxModeActive;
		}
		set
		{
			sandboxModeActive = value;
			Trigger(-1948169901);
			if (PlanScreen.Instance != null)
			{
				PlanScreen.Instance.Refresh();
			}
			if (BuildMenu.Instance != null)
			{
				BuildMenu.Instance.Refresh();
			}
		}
	}

	public bool DebugOnlyBuildingsAllowed
	{
		get
		{
			if (DebugHandler.enabled)
			{
				if (!SandboxModeActive)
				{
					return DebugHandler.InstantBuildMode;
				}
				return true;
			}
			return false;
		}
	}

	public StatusItemRenderer statusItemRenderer { get; private set; }

	public PrioritizableRenderer prioritizableRenderer { get; private set; }

	public float LastTimeWorkStarted => lastTimeWorkStarted;

	public static bool IsQuitting()
	{
		return quitting;
	}

	protected override void OnPrefabInit()
	{
		DebugUtil.LogArgs(Time.realtimeSinceStartup, "Level Loaded....", SceneManager.GetActiveScene().name);
		Components.BuildingCellVisualizers.OnAdd += OnAddBuildingCellVisualizer;
		Components.BuildingCellVisualizers.OnRemove += OnRemoveBuildingCellVisualizer;
		Singleton<KBatchedAnimUpdater>.CreateInstance();
		Singleton<CellChangeMonitor>.CreateInstance();
		userMenu = new UserMenu();
		SimTemperatureTransfer.ClearInstanceMap();
		StructureTemperatureComponents.ClearInstanceMap();
		ElementConsumer.ClearInstanceMap();
		App.OnPreLoadScene = (System.Action)Delegate.Combine(App.OnPreLoadScene, new System.Action(StopBE));
		Instance = this;
		statusItemRenderer = new StatusItemRenderer();
		prioritizableRenderer = new PrioritizableRenderer();
		LoadEventHashes();
		savedInfo.InitializeEmptyVariables();
		gasFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.GasConduits) - 0.4f);
		liquidFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.LiquidConduits) - 0.4f);
		solidFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.SolidConduitContents) - 0.4f);
		Shader.WarmupAllShaders();
		Db.Get();
		quitting = false;
		PickupableLayer = LayerMask.NameToLayer("Pickupable");
		BlockSelectionLayerMask = LayerMask.GetMask("BlockSelection");
		world = World.Instance;
		KPrefabID.NextUniqueID = KPlayerPrefs.GetInt(NextUniqueIDKey, 0);
		circuitManager = new CircuitManager();
		energySim = new EnergySim();
		gasConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(Grid.WidthInCells, Grid.HeightInCells, 13);
		liquidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(Grid.WidthInCells, Grid.HeightInCells, 17);
		electricalConduitSystem = new UtilityNetworkManager<ElectricalUtilityNetwork, Wire>(Grid.WidthInCells, Grid.HeightInCells, 27);
		logicCircuitSystem = new UtilityNetworkManager<LogicCircuitNetwork, LogicWire>(Grid.WidthInCells, Grid.HeightInCells, 32);
		logicCircuitManager = new LogicCircuitManager(logicCircuitSystem);
		travelTubeSystem = new UtilityNetworkTubesManager(Grid.WidthInCells, Grid.HeightInCells, 35);
		solidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, SolidConduit>(Grid.WidthInCells, Grid.HeightInCells, 21);
		conduitTemperatureManager = new ConduitTemperatureManager();
		conduitDiseaseManager = new ConduitDiseaseManager(conduitTemperatureManager);
		gasConduitFlow = new ConduitFlow(ConduitType.Gas, Grid.CellCount, gasConduitSystem, 1f, 0.25f);
		liquidConduitFlow = new ConduitFlow(ConduitType.Liquid, Grid.CellCount, liquidConduitSystem, 10f, 0.75f);
		solidConduitFlow = new SolidConduitFlow(Grid.CellCount, solidConduitSystem, 0.75f);
		gasFlowVisualizer = new ConduitFlowVisualizer(gasConduitFlow, gasConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundGas, Lighting.Instance.Settings.GasConduit);
		liquidFlowVisualizer = new ConduitFlowVisualizer(liquidConduitFlow, liquidConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundLiquid, Lighting.Instance.Settings.LiquidConduit);
		solidFlowVisualizer = new SolidConduitFlowVisualizer(solidConduitFlow, solidConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundSolid, Lighting.Instance.Settings.SolidConduit);
		accumulators = new Accumulators();
		plantElementAbsorbers = new PlantElementAbsorbers();
		activeFX = new ushort[Grid.CellCount];
		UnsafePrefabInit();
		Shader.SetGlobalVector("_MetalParameters", new Vector4(0f, 0f, 0f, 0f));
		Shader.SetGlobalVector("_WaterParameters", new Vector4(0f, 0f, 0f, 0f));
		InitializeFXSpawners();
		PathFinder.Initialize();
		new GameNavGrids(Pathfinding.Instance);
		screenMgr = Util.KInstantiate(screenManagerPrefab).GetComponent<GameScreenManager>();
		roomProber = new RoomProber();
		fetchManager = base.gameObject.AddComponent<FetchManager>();
		ediblesManager = base.gameObject.AddComponent<EdiblesManager>();
		Singleton<CellChangeMonitor>.Instance.SetGridSize(Grid.WidthInCells, Grid.HeightInCells);
		unlocks = GetComponent<Unlocks>();
		changelistsPlayedOn = new List<uint>();
		changelistsPlayedOn.Add(512719u);
		dateGenerated = System.DateTime.UtcNow.ToString("U", CultureInfo.InvariantCulture);
	}

	public void SetGameStarted()
	{
		gameStarted = true;
	}

	public bool GameStarted()
	{
		return gameStarted;
	}

	private unsafe void UnsafePrefabInit()
	{
		StepTheSim(0f);
	}

	protected override void OnLoadLevel()
	{
		Unsubscribe(1798162660, MarkStatusItemRendererDirtyDelegate);
		Unsubscribe(1983128072, ActiveWorldChangedDelegate);
		base.OnLoadLevel();
	}

	private void MarkStatusItemRendererDirty(object data)
	{
		statusItemRenderer.MarkAllDirty();
	}

	protected override void OnForcedCleanUp()
	{
		if (prioritizableRenderer != null)
		{
			prioritizableRenderer.Cleanup();
			prioritizableRenderer = null;
		}
		if (statusItemRenderer != null)
		{
			statusItemRenderer.Destroy();
			statusItemRenderer = null;
		}
		if (conduitTemperatureManager != null)
		{
			conduitTemperatureManager.Shutdown();
		}
		gasFlowVisualizer.FreeResources();
		liquidFlowVisualizer.FreeResources();
		solidFlowVisualizer.FreeResources();
		LightGridManager.Shutdown();
		RadiationGridManager.Shutdown();
		App.OnPreLoadScene = (System.Action)Delegate.Remove(App.OnPreLoadScene, new System.Action(StopBE));
		base.OnForcedCleanUp();
	}

	protected override void OnSpawn()
	{
		Debug.Log("-- GAME --");
		PropertyTextures.FogOfWarScale = 0f;
		if (CameraController.Instance != null)
		{
			CameraController.Instance.EnableFreeCamera(enable: false);
		}
		LocalPlayer = SpawnPlayer();
		WaterCubes.Instance.Init();
		SpeedControlScreen.Instance.Pause(playSound: false);
		LightGridManager.Initialise();
		RadiationGridManager.Initialise();
		RefreshRadiationLoop();
		UnsafeOnSpawn();
		Time.timeScale = 0f;
		if (tempIntroScreenPrefab != null)
		{
			Util.KInstantiate(tempIntroScreenPrefab);
		}
		if (SaveLoader.Instance.ClusterLayout != null)
		{
			foreach (WorldGen world in SaveLoader.Instance.ClusterLayout.worlds)
			{
				Reset(world.data.gameSpawnData, world.WorldOffset);
			}
			NewBaseScreen.SetInitialCamera();
		}
		TagManager.FillMissingProperNames();
		CameraController.Instance.SetOrthographicsSize(20f);
		if (SaveLoader.Instance.loadedFromSave)
		{
			baseAlreadyCreated = true;
			Trigger(-1992507039);
			Trigger(-838649377);
		}
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(MeshRenderer));
		for (int i = 0; i < array.Length; i++)
		{
			((MeshRenderer)array[i]).reflectionProbeUsage = ReflectionProbeUsage.Off;
		}
		Subscribe(1798162660, MarkStatusItemRendererDirtyDelegate);
		Subscribe(1983128072, ActiveWorldChangedDelegate);
		solidConduitFlow.Initialize();
		SimAndRenderScheduler.instance.Add(roomProber);
		SimAndRenderScheduler.instance.Add(KComponentSpawn.instance);
		SimAndRenderScheduler.instance.RegisterBatchUpdate<ISim200ms, AmountInstance>(AmountInstance.BatchUpdate);
		SimAndRenderScheduler.instance.RegisterBatchUpdate<ISim1000ms, SolidTransferArm>(SolidTransferArm.BatchUpdate);
		if (!SaveLoader.Instance.loadedFromSave)
		{
			SettingConfig settingConfig = CustomGameSettings.Instance.QualitySettings[CustomGameSettingConfigs.SandboxMode.id];
			SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SandboxMode);
			SaveGame.Instance.sandboxEnabled = !settingConfig.IsDefaultLevel(currentQualitySetting.id);
		}
		mingleCellTracker = base.gameObject.AddComponent<MingleCellTracker>();
		if (Global.Instance != null)
		{
			Global.Instance.GetComponent<PerformanceMonitor>().Reset();
			Global.Instance.modManager.NotifyDialog(UI.FRONTEND.MOD_DIALOGS.SAVE_GAME_MODS_DIFFER.TITLE, UI.FRONTEND.MOD_DIALOGS.SAVE_GAME_MODS_DIFFER.MESSAGE, Global.Instance.globalCanvas);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		SimAndRenderScheduler.instance.Remove(KComponentSpawn.instance);
		SimAndRenderScheduler.instance.RegisterBatchUpdate<ISim200ms, AmountInstance>(null);
		SimAndRenderScheduler.instance.RegisterBatchUpdate<ISim1000ms, SolidTransferArm>(null);
		DestroyInstances();
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		DestroyInstances();
	}

	private unsafe void UnsafeOnSpawn()
	{
		world.UpdateCellInfo(gameSolidInfo, callbackInfo, 0, null, 0, null);
	}

	private void RefreshRadiationLoop()
	{
		GameScheduler.Instance.Schedule("UpdateRadiation", 1f, delegate
		{
			RadiationGridManager.Refresh();
			RefreshRadiationLoop();
		});
	}

	public void SetMusicEnabled(bool enabled)
	{
		if (enabled)
		{
			MusicManager.instance.PlaySong("Music_FrontEnd");
		}
		else
		{
			MusicManager.instance.StopSong("Music_FrontEnd");
		}
	}

	private Player SpawnPlayer()
	{
		Player component = Util.KInstantiate(playerPrefab, base.gameObject).GetComponent<Player>();
		component.ScreenManager = screenMgr;
		component.ScreenManager.StartScreen(ScreenPrefabs.Instance.HudScreen.gameObject);
		component.ScreenManager.StartScreen(ScreenPrefabs.Instance.HoverTextScreen.gameObject, null, GameScreenManager.UIRenderTarget.HoverTextScreen);
		component.ScreenManager.StartScreen(ScreenPrefabs.Instance.ToolTipScreen.gameObject, null, GameScreenManager.UIRenderTarget.HoverTextScreen);
		cameraController = Util.KInstantiate(cameraControllerPrefab).GetComponent<CameraController>();
		component.CameraController = cameraController;
		if (KInputManager.currentController != null)
		{
			KInputHandler.Add(KInputManager.currentController, cameraController, 1);
		}
		else
		{
			KInputHandler.Add(Global.Instance.GetInputManager().GetDefaultController(), cameraController, 1);
		}
		Global.Instance.GetInputManager().usedMenus.Add(cameraController);
		playerController = component.GetComponent<PlayerController>();
		if (KInputManager.currentController != null)
		{
			KInputHandler.Add(KInputManager.currentController, playerController, 20);
		}
		else
		{
			KInputHandler.Add(Global.Instance.GetInputManager().GetDefaultController(), playerController, 20);
		}
		Global.Instance.GetInputManager().usedMenus.Add(playerController);
		return component;
	}

	public void SetDupePassableSolid(int cell, bool passable, bool solid)
	{
		Grid.DupePassable[cell] = passable;
		gameSolidInfo.Add(new SolidInfo(cell, solid));
	}

	private unsafe Sim.GameDataUpdate* StepTheSim(float dt)
	{
		using (new KProfiler.Region("StepTheSim"))
		{
			IntPtr intPtr = IntPtr.Zero;
			using (new KProfiler.Region("WaitingForSim"))
			{
				if (Grid.Visible == null || Grid.Visible.Length == 0)
				{
					Debug.LogError("Invalid Grid.Visible, what have you done?!");
					return null;
				}
				intPtr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, Grid.Visible.Length, Grid.Visible);
			}
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			Sim.GameDataUpdate* ptr = (Sim.GameDataUpdate*)(void*)intPtr;
			Grid.elementIdx = ptr->elementIdx;
			Grid.temperature = ptr->temperature;
			Grid.mass = ptr->mass;
			Grid.radiation = ptr->radiation;
			Grid.properties = ptr->properties;
			Grid.strengthInfo = ptr->strengthInfo;
			Grid.insulation = ptr->insulation;
			Grid.diseaseIdx = ptr->diseaseIdx;
			Grid.diseaseCount = ptr->diseaseCount;
			Grid.AccumulatedFlowValues = ptr->accumulatedFlow;
			Grid.exposedToSunlight = (byte*)(void*)ptr->propertyTextureExposedToSunlight;
			PropertyTextures.externalFlowTex = ptr->propertyTextureFlow;
			PropertyTextures.externalLiquidTex = ptr->propertyTextureLiquid;
			PropertyTextures.externalExposedToSunlight = ptr->propertyTextureExposedToSunlight;
			List<Element> elements = ElementLoader.elements;
			simData.emittedMassEntries = ptr->emittedMassEntries;
			simData.elementChunks = ptr->elementChunkInfos;
			simData.buildingTemperatures = ptr->buildingTemperatures;
			simData.diseaseEmittedInfos = ptr->diseaseEmittedInfos;
			simData.diseaseConsumedInfos = ptr->diseaseConsumedInfos;
			for (int i = 0; i < ptr->numSubstanceChangeInfo; i++)
			{
				Sim.SubstanceChangeInfo substanceChangeInfo = ptr->substanceChangeInfo[i];
				Element element = elements[substanceChangeInfo.newElemIdx];
				Grid.Element[substanceChangeInfo.cellIdx] = element;
			}
			for (int j = 0; j < ptr->numSolidInfo; j++)
			{
				Sim.SolidInfo solidInfo = ptr->solidInfo[j];
				if (!solidChangedFilter.Contains(solidInfo.cellIdx))
				{
					this.solidInfo.Add(new SolidInfo(solidInfo.cellIdx, solidInfo.isSolid != 0));
					bool solid = solidInfo.isSolid != 0;
					Grid.SetSolid(solidInfo.cellIdx, solid, CellEventLogger.Instance.SimMessagesSolid);
				}
			}
			for (int k = 0; k < ptr->numCallbackInfo; k++)
			{
				Sim.CallbackInfo callbackInfo = ptr->callbackInfo[k];
				HandleVector<CallbackInfo>.Handle handle = default(HandleVector<CallbackInfo>.Handle);
				handle.index = callbackInfo.callbackIdx;
				HandleVector<CallbackInfo>.Handle handle2 = handle;
				if (!IsManuallyReleasedHandle(handle2))
				{
					this.callbackInfo.Add(new Klei.CallbackInfo(handle2));
				}
			}
			int numSpawnFallingLiquidInfo = ptr->numSpawnFallingLiquidInfo;
			for (int l = 0; l < numSpawnFallingLiquidInfo; l++)
			{
				Sim.SpawnFallingLiquidInfo spawnFallingLiquidInfo = ptr->spawnFallingLiquidInfo[l];
				FallingWater.instance.AddParticle(spawnFallingLiquidInfo.cellIdx, spawnFallingLiquidInfo.elemIdx, spawnFallingLiquidInfo.mass, spawnFallingLiquidInfo.temperature, spawnFallingLiquidInfo.diseaseIdx, spawnFallingLiquidInfo.diseaseCount);
			}
			int numDigInfo = ptr->numDigInfo;
			WorldDamage component = world.GetComponent<WorldDamage>();
			for (int m = 0; m < numDigInfo; m++)
			{
				Sim.SpawnOreInfo spawnOreInfo = ptr->digInfo[m];
				if (spawnOreInfo.temperature <= 0f && spawnOreInfo.mass > 0f)
				{
					Debug.LogError("Sim is telling us to spawn a zero temperature object. This shouldn't be possible because I have asserts in the dll about this....");
				}
				component.OnDigComplete(spawnOreInfo.cellIdx, spawnOreInfo.mass, spawnOreInfo.temperature, spawnOreInfo.elemIdx, spawnOreInfo.diseaseIdx, spawnOreInfo.diseaseCount);
			}
			int numSpawnOreInfo = ptr->numSpawnOreInfo;
			for (int n = 0; n < numSpawnOreInfo; n++)
			{
				Sim.SpawnOreInfo spawnOreInfo2 = ptr->spawnOreInfo[n];
				Vector3 position = Grid.CellToPosCCC(spawnOreInfo2.cellIdx, Grid.SceneLayer.Ore);
				Element element2 = ElementLoader.elements[spawnOreInfo2.elemIdx];
				if (spawnOreInfo2.temperature <= 0f && spawnOreInfo2.mass > 0f)
				{
					Debug.LogError("Sim is telling us to spawn a zero temperature object. This shouldn't be possible because I have asserts in the dll about this....");
				}
				element2.substance.SpawnResource(position, spawnOreInfo2.mass, spawnOreInfo2.temperature, spawnOreInfo2.diseaseIdx, spawnOreInfo2.diseaseCount);
			}
			int numSpawnFXInfo = ptr->numSpawnFXInfo;
			for (int num = 0; num < numSpawnFXInfo; num++)
			{
				Sim.SpawnFXInfo spawnFXInfo = ptr->spawnFXInfo[num];
				SpawnFX((SpawnFXHashes)spawnFXInfo.fxHash, spawnFXInfo.cellIdx, spawnFXInfo.rotation);
			}
			UnstableGroundManager component2 = world.GetComponent<UnstableGroundManager>();
			int numUnstableCellInfo = ptr->numUnstableCellInfo;
			for (int num2 = 0; num2 < numUnstableCellInfo; num2++)
			{
				Sim.UnstableCellInfo unstableCellInfo = ptr->unstableCellInfo[num2];
				if (unstableCellInfo.fallingInfo == 0)
				{
					component2.Spawn(unstableCellInfo.cellIdx, ElementLoader.elements[unstableCellInfo.elemIdx], unstableCellInfo.mass, unstableCellInfo.temperature, unstableCellInfo.diseaseIdx, unstableCellInfo.diseaseCount);
				}
			}
			int numWorldDamageInfo = ptr->numWorldDamageInfo;
			for (int num3 = 0; num3 < numWorldDamageInfo; num3++)
			{
				Sim.WorldDamageInfo damage_info = ptr->worldDamageInfo[num3];
				WorldDamage.Instance.ApplyDamage(damage_info);
			}
			for (int num4 = 0; num4 < ptr->numRemovedMassEntries; num4++)
			{
				ElementConsumer.AddMass(ptr->removedMassEntries[num4]);
			}
			int numMassConsumedCallbacks = ptr->numMassConsumedCallbacks;
			HandleVector<ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle3 = default(HandleVector<ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle);
			for (int num5 = 0; num5 < numMassConsumedCallbacks; num5++)
			{
				Sim.MassConsumedCallback arg = ptr->massConsumedCallbacks[num5];
				handle3.index = arg.callbackIdx;
				ComplexCallbackInfo<Sim.MassConsumedCallback> complexCallbackInfo = massConsumedCallbackManager.Release(handle3, "massConsumedCB");
				if (complexCallbackInfo.cb != null)
				{
					complexCallbackInfo.cb(arg, complexCallbackInfo.callbackData);
				}
			}
			int numMassEmittedCallbacks = ptr->numMassEmittedCallbacks;
			HandleVector<ComplexCallbackInfo<Sim.MassEmittedCallback>>.Handle handle4 = default(HandleVector<ComplexCallbackInfo<Sim.MassEmittedCallback>>.Handle);
			for (int num6 = 0; num6 < numMassEmittedCallbacks; num6++)
			{
				Sim.MassEmittedCallback arg2 = ptr->massEmittedCallbacks[num6];
				handle4.index = arg2.callbackIdx;
				if (massEmitCallbackManager.IsVersionValid(handle4))
				{
					ComplexCallbackInfo<Sim.MassEmittedCallback> item = massEmitCallbackManager.GetItem(handle4);
					if (item.cb != null)
					{
						item.cb(arg2, item.callbackData);
					}
				}
			}
			int numDiseaseConsumptionCallbacks = ptr->numDiseaseConsumptionCallbacks;
			HandleVector<ComplexCallbackInfo<Sim.DiseaseConsumptionCallback>>.Handle handle5 = default(HandleVector<ComplexCallbackInfo<Sim.DiseaseConsumptionCallback>>.Handle);
			for (int num7 = 0; num7 < numDiseaseConsumptionCallbacks; num7++)
			{
				Sim.DiseaseConsumptionCallback arg3 = ptr->diseaseConsumptionCallbacks[num7];
				handle5.index = arg3.callbackIdx;
				if (diseaseConsumptionCallbackManager.IsVersionValid(handle5))
				{
					ComplexCallbackInfo<Sim.DiseaseConsumptionCallback> item2 = diseaseConsumptionCallbackManager.GetItem(handle5);
					if (item2.cb != null)
					{
						item2.cb(arg3, item2.callbackData);
					}
				}
			}
			int numComponentStateChangedMessages = ptr->numComponentStateChangedMessages;
			HandleVector<ComplexCallbackInfo<int>>.Handle handle6 = default(HandleVector<ComplexCallbackInfo<int>>.Handle);
			for (int num8 = 0; num8 < numComponentStateChangedMessages; num8++)
			{
				Sim.ComponentStateChangedMessage componentStateChangedMessage = ptr->componentStateChangedMessages[num8];
				handle6.index = componentStateChangedMessage.callbackIdx;
				if (simComponentCallbackManager.IsVersionValid(handle6))
				{
					ComplexCallbackInfo<int> complexCallbackInfo2 = simComponentCallbackManager.Release(handle6, "component state changed cb");
					if (complexCallbackInfo2.cb != null)
					{
						complexCallbackInfo2.cb(componentStateChangedMessage.simHandle, complexCallbackInfo2.callbackData);
					}
				}
			}
			int numRadiationConsumedCallbacks = ptr->numRadiationConsumedCallbacks;
			HandleVector<ComplexCallbackInfo<Sim.ConsumedRadiationCallback>>.Handle handle7 = default(HandleVector<ComplexCallbackInfo<Sim.ConsumedRadiationCallback>>.Handle);
			for (int num9 = 0; num9 < numRadiationConsumedCallbacks; num9++)
			{
				Sim.ConsumedRadiationCallback arg4 = ptr->radiationConsumedCallbacks[num9];
				handle7.index = arg4.callbackIdx;
				ComplexCallbackInfo<Sim.ConsumedRadiationCallback> complexCallbackInfo3 = radiationConsumedCallbackManager.Release(handle7, "radiationConsumedCB");
				if (complexCallbackInfo3.cb != null)
				{
					complexCallbackInfo3.cb(arg4, complexCallbackInfo3.callbackData);
				}
			}
			int numElementChunkMeltedInfos = ptr->numElementChunkMeltedInfos;
			for (int num10 = 0; num10 < numElementChunkMeltedInfos; num10++)
			{
				SimTemperatureTransfer.DoOreMeltTransition(ptr->elementChunkMeltedInfos[num10].handle);
			}
			int numBuildingOverheatInfos = ptr->numBuildingOverheatInfos;
			for (int num11 = 0; num11 < numBuildingOverheatInfos; num11++)
			{
				StructureTemperatureComponents.DoOverheat(ptr->buildingOverheatInfos[num11].handle);
			}
			int numBuildingNoLongerOverheatedInfos = ptr->numBuildingNoLongerOverheatedInfos;
			for (int num12 = 0; num12 < numBuildingNoLongerOverheatedInfos; num12++)
			{
				StructureTemperatureComponents.DoNoLongerOverheated(ptr->buildingNoLongerOverheatedInfos[num12].handle);
			}
			int numBuildingMeltedInfos = ptr->numBuildingMeltedInfos;
			for (int num13 = 0; num13 < numBuildingMeltedInfos; num13++)
			{
				StructureTemperatureComponents.DoStateTransition(ptr->buildingMeltedInfos[num13].handle);
			}
			int numCellMeltedInfos = ptr->numCellMeltedInfos;
			for (int num14 = 0; num14 < numCellMeltedInfos; num14++)
			{
				int gameCell = ptr->cellMeltedInfos[num14].gameCell;
				GameObject gameObject = Grid.Objects[gameCell, 9];
				if (gameObject != null)
				{
					Util.KDestroyGameObject(gameObject);
				}
			}
			if (dt > 0f)
			{
				conduitTemperatureManager.Sim200ms(0.2f);
				conduitDiseaseManager.Sim200ms(0.2f);
				gasConduitFlow.Sim200ms(0.2f);
				liquidConduitFlow.Sim200ms(0.2f);
				solidConduitFlow.Sim200ms(0.2f);
				accumulators.Sim200ms(0.2f);
				plantElementAbsorbers.Sim200ms(0.2f);
			}
			Sim.DebugProperties debugProperties = default(Sim.DebugProperties);
			debugProperties.buildingTemperatureScale = 100f;
			debugProperties.contaminatedOxygenEmitProbability = 0.001f;
			debugProperties.contaminatedOxygenConversionPercent = 0.001f;
			debugProperties.biomeTemperatureLerpRate = 0.001f;
			debugProperties.isDebugEditing = (byte)((DebugPaintElementScreen.Instance != null && DebugPaintElementScreen.Instance.gameObject.activeSelf) ? 1u : 0u);
			debugProperties.pad0 = (debugProperties.pad1 = (debugProperties.pad2 = 0));
			SimMessages.SetDebugProperties(debugProperties);
			if (dt > 0f)
			{
				if (circuitManager != null)
				{
					circuitManager.Sim200msFirst(dt);
				}
				if (energySim != null)
				{
					energySim.EnergySim200ms(dt);
				}
				if (circuitManager != null)
				{
					circuitManager.Sim200msLast(dt);
				}
			}
			return ptr;
		}
	}

	public void AddSolidChangedFilter(int cell)
	{
		solidChangedFilter.Add(cell);
	}

	public void RemoveSolidChangedFilter(int cell)
	{
		solidChangedFilter.Remove(cell);
	}

	public void SetIsLoading()
	{
		isLoading = true;
	}

	public bool IsLoading()
	{
		return isLoading;
	}

	private void ShowDebugCellInfo()
	{
		int mouseCell = DebugHandler.GetMouseCell();
		int x = 0;
		int y = 0;
		Grid.CellToXY(mouseCell, out x, out y);
		string text = mouseCell + " (" + x + ", " + y + ")";
		DebugText.Instance.Draw(text, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
	}

	public void ForceSimStep()
	{
		DebugUtil.LogArgs("Force-stepping the sim");
		simDt = 0.2f;
	}

	private void Update()
	{
		if (!isLoading)
		{
			float deltaTime = Time.deltaTime;
			if (Debug.developerConsoleVisible)
			{
				Debug.developerConsoleVisible = false;
			}
			if (DebugHandler.DebugCellInfo)
			{
				ShowDebugCellInfo();
			}
			gasConduitSystem.Update();
			liquidConduitSystem.Update();
			solidConduitSystem.Update();
			circuitManager.RenderEveryTick(deltaTime);
			logicCircuitManager.RenderEveryTick(deltaTime);
			solidConduitFlow.RenderEveryTick(deltaTime);
			Pathfinding.Instance.RenderEveryTick();
			Singleton<CellChangeMonitor>.Instance.RenderEveryTick();
			SimEveryTick(deltaTime);
		}
	}

	private void SimEveryTick(float dt)
	{
		dt = Mathf.Min(dt, 0.2f);
		simDt += dt;
		if (simDt >= 1f / 60f)
		{
			do
			{
				simSubTick++;
				simSubTick %= 12;
				if (simSubTick == 0)
				{
					hasFirstSimTickRun = true;
					UnsafeSim200ms(0.2f);
				}
				if (hasFirstSimTickRun)
				{
					Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
				}
				simDt -= 1f / 60f;
			}
			while (simDt >= 1f / 60f);
		}
		else
		{
			UnsafeSim200ms(0f);
		}
	}

	private unsafe void UnsafeSim200ms(float dt)
	{
		simActiveRegions.Clear();
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			if (worldContainer.IsDiscovered)
			{
				SimActiveRegion simActiveRegion = new SimActiveRegion();
				simActiveRegion.region = new Pair<Vector2I, Vector2I>(worldContainer.WorldOffset, worldContainer.WorldOffset + worldContainer.WorldSize);
				simActiveRegion.currentSunlightIntensity = worldContainer.currentSunlightIntensity;
				simActiveRegion.currentCosmicRadiationIntensity = worldContainer.currentCosmicIntensity;
				simActiveRegions.Add(simActiveRegion);
			}
		}
		Debug.Assert(simActiveRegions.Count > 0, "Cannot send a frame to the sim with zero active regions");
		SimMessages.NewGameFrame(dt, simActiveRegions);
		Sim.GameDataUpdate* ptr = StepTheSim(dt);
		if (ptr == null)
		{
			Debug.LogError("UNEXPECTED!");
		}
		else if (ptr->numFramesProcessed > 0)
		{
			gameSolidInfo.AddRange(solidInfo);
			world.UpdateCellInfo(gameSolidInfo, callbackInfo, ptr->numSolidSubstanceChangeInfo, ptr->solidSubstanceChangeInfo, ptr->numLiquidChangeInfo, ptr->liquidChangeInfo);
			gameSolidInfo.Clear();
			solidInfo.Clear();
			callbackInfo.Clear();
			callbackManagerManuallyReleasedHandles.Clear();
			Pathfinding.Instance.UpdateNavGrids();
		}
	}

	private void LateUpdateComponents()
	{
		UpdateOverlayScreen();
	}

	private void OnAddBuildingCellVisualizer(BuildingCellVisualizer building_cell_visualizer)
	{
		lastDrawnOverlayMode = default(HashedString);
		if (PlayerController.Instance != null)
		{
			BuildTool buildTool = PlayerController.Instance.ActiveTool as BuildTool;
			if (buildTool != null && buildTool.visualizer == building_cell_visualizer.gameObject)
			{
				previewVisualizer = building_cell_visualizer;
			}
		}
	}

	private void OnRemoveBuildingCellVisualizer(BuildingCellVisualizer building_cell_visualizer)
	{
		if (previewVisualizer == building_cell_visualizer)
		{
			previewVisualizer = null;
		}
	}

	private void UpdateOverlayScreen()
	{
		if (OverlayScreen.Instance == null)
		{
			return;
		}
		HashedString mode = OverlayScreen.Instance.GetMode();
		if (previewVisualizer != null)
		{
			previewVisualizer.DisableIcons();
			previewVisualizer.DrawIcons(mode);
		}
		if (mode == lastDrawnOverlayMode)
		{
			return;
		}
		foreach (BuildingCellVisualizer item in Components.BuildingCellVisualizers.Items)
		{
			item.DisableIcons();
			item.DrawIcons(mode);
		}
		lastDrawnOverlayMode = mode;
	}

	public void ForceOverlayUpdate(bool clearLastMode = false)
	{
		previousOverlayMode = OverlayModes.None.ID;
		if (clearLastMode)
		{
			lastDrawnOverlayMode = OverlayModes.None.ID;
		}
	}

	private void LateUpdate()
	{
		if (Time.timeScale == 0f && !IsPaused)
		{
			IsPaused = true;
			Trigger(-1788536802, IsPaused);
		}
		else if (Time.timeScale != 0f && IsPaused)
		{
			IsPaused = false;
			Trigger(-1788536802, IsPaused);
		}
		if (Input.GetMouseButton(0))
		{
			VisualTunerElement = null;
			int mouseCell = DebugHandler.GetMouseCell();
			if (Grid.IsValidCell(mouseCell))
			{
				Element element = (VisualTunerElement = Grid.Element[mouseCell]);
			}
		}
		gasConduitSystem.Update();
		liquidConduitSystem.Update();
		solidConduitSystem.Update();
		HashedString mode = SimDebugView.Instance.GetMode();
		if (mode != previousOverlayMode)
		{
			previousOverlayMode = mode;
			if (mode == OverlayModes.LiquidConduits.ID)
			{
				liquidFlowVisualizer.ColourizePipeContents(show_contents: true, move_to_overlay_layer: true);
				gasFlowVisualizer.ColourizePipeContents(show_contents: false, move_to_overlay_layer: true);
				solidFlowVisualizer.ColourizePipeContents(show_contents: false, move_to_overlay_layer: true);
			}
			else if (mode == OverlayModes.GasConduits.ID)
			{
				liquidFlowVisualizer.ColourizePipeContents(show_contents: false, move_to_overlay_layer: true);
				gasFlowVisualizer.ColourizePipeContents(show_contents: true, move_to_overlay_layer: true);
				solidFlowVisualizer.ColourizePipeContents(show_contents: false, move_to_overlay_layer: true);
			}
			else if (mode == OverlayModes.SolidConveyor.ID)
			{
				liquidFlowVisualizer.ColourizePipeContents(show_contents: false, move_to_overlay_layer: true);
				gasFlowVisualizer.ColourizePipeContents(show_contents: false, move_to_overlay_layer: true);
				solidFlowVisualizer.ColourizePipeContents(show_contents: true, move_to_overlay_layer: true);
			}
			else
			{
				liquidFlowVisualizer.ColourizePipeContents(show_contents: false, move_to_overlay_layer: false);
				gasFlowVisualizer.ColourizePipeContents(show_contents: false, move_to_overlay_layer: false);
				solidFlowVisualizer.ColourizePipeContents(show_contents: false, move_to_overlay_layer: false);
			}
		}
		gasFlowVisualizer.Render(gasFlowPos.z, 0, gasConduitFlow.ContinuousLerpPercent, mode == OverlayModes.GasConduits.ID && gasConduitFlow.DiscreteLerpPercent != previousGasConduitFlowDiscreteLerpPercent);
		liquidFlowVisualizer.Render(liquidFlowPos.z, 0, liquidConduitFlow.ContinuousLerpPercent, mode == OverlayModes.LiquidConduits.ID && liquidConduitFlow.DiscreteLerpPercent != previousLiquidConduitFlowDiscreteLerpPercent);
		solidFlowVisualizer.Render(solidFlowPos.z, 0, solidConduitFlow.ContinuousLerpPercent, mode == OverlayModes.SolidConveyor.ID && solidConduitFlow.DiscreteLerpPercent != previousSolidConduitFlowDiscreteLerpPercent);
		previousGasConduitFlowDiscreteLerpPercent = ((mode == OverlayModes.GasConduits.ID) ? gasConduitFlow.DiscreteLerpPercent : (-1f));
		previousLiquidConduitFlowDiscreteLerpPercent = ((mode == OverlayModes.LiquidConduits.ID) ? liquidConduitFlow.DiscreteLerpPercent : (-1f));
		previousSolidConduitFlowDiscreteLerpPercent = ((mode == OverlayModes.SolidConveyor.ID) ? solidConduitFlow.DiscreteLerpPercent : (-1f));
		Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.GetPosition().z));
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.GetPosition().z));
		Shader.SetGlobalVector("_WsToCs", new Vector4(vector.x / (float)Grid.WidthInCells, vector.y / (float)Grid.HeightInCells, (vector2.x - vector.x) / (float)Grid.WidthInCells, (vector2.y - vector.y) / (float)Grid.HeightInCells));
		WorldContainer activeWorld = ClusterManager.Instance.activeWorld;
		Vector2I worldOffset = activeWorld.WorldOffset;
		Vector2I worldSize = activeWorld.WorldSize;
		Vector4 value = new Vector4((vector.x - (float)worldOffset.x) / (float)worldSize.x, (vector.y - (float)worldOffset.y) / (float)worldSize.y, (vector2.x - vector.x) / (float)worldSize.x, (vector2.y - vector.y) / (float)worldSize.y);
		Shader.SetGlobalVector("_WsToCcs", value);
		if (drawStatusItems)
		{
			statusItemRenderer.RenderEveryTick();
			prioritizableRenderer.RenderEveryTick();
		}
		LateUpdateComponents();
		Singleton<StateMachineUpdater>.Instance.Render(Time.unscaledDeltaTime);
		Singleton<StateMachineUpdater>.Instance.RenderEveryTick(Time.unscaledDeltaTime);
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null)
		{
			Navigator component = SelectTool.Instance.selected.GetComponent<Navigator>();
			if (component != null)
			{
				component.DrawPath();
			}
		}
		KFMOD.RenderEveryTick(Time.deltaTime);
		if (GenericGameSettings.instance.performanceCapture.waitTime != 0f)
		{
			UpdatePerformanceCapture();
		}
	}

	private void UpdatePerformanceCapture()
	{
		if (IsPaused && SpeedControlScreen.Instance != null)
		{
			SpeedControlScreen.Instance.Unpause();
		}
		if (Time.timeSinceLevelLoad < GenericGameSettings.instance.performanceCapture.waitTime)
		{
			return;
		}
		uint num = 512719u;
		string text = System.DateTime.Now.ToShortDateString();
		string text2 = System.DateTime.Now.ToShortTimeString();
		string fileName = Path.GetFileName(GenericGameSettings.instance.performanceCapture.saveGame);
		string text3 = "Version,Date,Time,SaveGame";
		string text4 = $"{num},{text},{text2},{fileName}";
		float num2 = 0.1f;
		if (GenericGameSettings.instance.performanceCapture.gcStats)
		{
			Debug.Log("Begin GC profiling...");
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			GC.Collect();
			num2 = Time.realtimeSinceStartup - realtimeSinceStartup;
			Debug.Log("\tGC.Collect() took " + num2 + " seconds");
			MemorySnapshot memorySnapshot = new MemorySnapshot();
			string format = "{0},{1},{2},{3}";
			string path = "./memory/GCTypeMetrics.csv";
			if (!File.Exists(path))
			{
				using StreamWriter streamWriter = new StreamWriter(path);
				streamWriter.WriteLine(string.Format(format, text3, "Type", "Instances", "References"));
			}
			using (StreamWriter streamWriter2 = new StreamWriter(path, append: true))
			{
				foreach (MemorySnapshot.TypeData value in memorySnapshot.types.Values)
				{
					streamWriter2.WriteLine(string.Format(format, text4, "\"" + value.type.ToString() + "\"", value.instanceCount, value.refCount));
				}
			}
			Debug.Log("...end GC profiling");
		}
		float fPS = Global.Instance.GetComponent<PerformanceMonitor>().FPS;
		Directory.CreateDirectory("./memory");
		string format2 = "{0},{1},{2}";
		string path2 = "./memory/GeneralMetrics.csv";
		if (!File.Exists(path2))
		{
			using StreamWriter streamWriter3 = new StreamWriter(path2);
			streamWriter3.WriteLine(string.Format(format2, text3, "GCDuration", "FPS"));
		}
		using (StreamWriter streamWriter4 = new StreamWriter(path2, append: true))
		{
			streamWriter4.WriteLine(string.Format(format2, text4, num2, fPS));
		}
		GenericGameSettings.instance.performanceCapture.waitTime = 0f;
		App.Quit();
	}

	public void Reset(GameSpawnData gsd, Vector2I world_offset)
	{
		using (new KProfiler.Region("World.Reset"))
		{
			if (gsd == null)
			{
				return;
			}
			foreach (KeyValuePair<Vector2I, bool> item in gsd.preventFoWReveal)
			{
				if (item.Value)
				{
					Vector2I vector2I = new Vector2I(item.Key.X + world_offset.X, item.Key.Y + world_offset.Y);
					Grid.PreventFogOfWarReveal[Grid.PosToCell(vector2I)] = item.Value;
				}
			}
		}
	}

	private void OnApplicationQuit()
	{
		quitting = true;
		Sim.Shutdown();
		AudioMixer.Destroy();
		if (screenMgr != null && screenMgr.gameObject != null)
		{
			UnityEngine.Object.Destroy(screenMgr.gameObject);
		}
		Console.WriteLine("Game.OnApplicationQuit()");
	}

	private void InitializeFXSpawners()
	{
		for (int i = 0; i < fxSpawnData.Length; i++)
		{
			int fx_idx = i;
			fxSpawnData[fx_idx].fxPrefab.SetActive(value: false);
			ushort fx_mask = (ushort)(1 << fx_idx);
			Action<SpawnFXHashes, GameObject> destroyer = delegate(SpawnFXHashes fxid, GameObject go)
			{
				if (!IsQuitting())
				{
					int num3 = Grid.PosToCell(go);
					activeFX[num3] &= (ushort)(~fx_mask);
					go.GetComponent<KAnimControllerBase>().enabled = false;
					fxPools[(int)fxid].ReleaseInstance(go);
				}
			};
			Func<GameObject> instantiator = delegate
			{
				GameObject obj2 = GameUtil.KInstantiate(fxSpawnData[fx_idx].fxPrefab, Grid.SceneLayer.Front);
				KBatchedAnimController component2 = obj2.GetComponent<KBatchedAnimController>();
				component2.enabled = false;
				obj2.SetActive(value: true);
				component2.onDestroySelf = delegate(GameObject go)
				{
					destroyer(fxSpawnData[fx_idx].id, go);
				};
				return obj2;
			};
			ObjectPool pool = new ObjectPool(instantiator, fxSpawnData[fx_idx].initialCount);
			fxPools[(int)fxSpawnData[fx_idx].id] = pool;
			fxSpawner[(int)fxSpawnData[fx_idx].id] = delegate(Vector3 pos, float rotation)
			{
				GameScheduler.Instance.Schedule("SpawnFX", 0f, delegate
				{
					int num = Grid.PosToCell(pos);
					if ((activeFX[num] & fx_mask) == 0)
					{
						activeFX[num] |= fx_mask;
						GameObject instance = pool.GetInstance();
						SpawnPoolData spawnPoolData = fxSpawnData[fx_idx];
						Quaternion quaternion = Quaternion.identity;
						bool flipX = false;
						string text = spawnPoolData.initialAnim;
						switch (spawnPoolData.rotationConfig)
						{
						case SpawnRotationConfig.Normal:
							quaternion = Quaternion.Euler(0f, 0f, rotation);
							break;
						case SpawnRotationConfig.StringName:
						{
							int num2 = (int)(rotation / 90f);
							if (num2 < 0)
							{
								num2 += spawnPoolData.rotationData.Length;
							}
							text = spawnPoolData.rotationData[num2].animName;
							flipX = spawnPoolData.rotationData[num2].flip;
							break;
						}
						}
						pos += spawnPoolData.spawnOffset;
						Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
						insideUnitCircle.x *= spawnPoolData.spawnRandomOffset.x;
						insideUnitCircle.y *= spawnPoolData.spawnRandomOffset.y;
						insideUnitCircle = quaternion * insideUnitCircle;
						pos.x += insideUnitCircle.x;
						pos.y += insideUnitCircle.y;
						instance.transform.SetPosition(pos);
						instance.transform.rotation = quaternion;
						KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
						component.FlipX = flipX;
						component.TintColour = spawnPoolData.colour;
						component.Play(text);
						component.enabled = true;
					}
				});
			};
		}
	}

	public void SpawnFX(SpawnFXHashes fx_id, int cell, float rotation)
	{
		Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.Front);
		if (CameraController.Instance.IsVisiblePos(vector))
		{
			fxSpawner[(int)fx_id](vector, rotation);
		}
	}

	public void SpawnFX(SpawnFXHashes fx_id, Vector3 pos, float rotation)
	{
		fxSpawner[(int)fx_id](pos, rotation);
	}

	public static void SaveSettings(BinaryWriter writer)
	{
		Serializer.Serialize(new Settings(Instance), writer);
	}

	public static void LoadSettings(Deserializer deserializer)
	{
		Settings settings = new Settings();
		deserializer.Deserialize(settings);
		KPlayerPrefs.SetInt(NextUniqueIDKey, settings.nextUniqueID);
		KleiMetrics.SetGameID(settings.gameID);
	}

	public void Save(BinaryWriter writer)
	{
		GameSaveData gameSaveData = new GameSaveData();
		gameSaveData.gasConduitFlow = gasConduitFlow;
		gameSaveData.liquidConduitFlow = liquidConduitFlow;
		gameSaveData.fallingWater = world.GetComponent<FallingWater>();
		gameSaveData.unstableGround = world.GetComponent<UnstableGroundManager>();
		gameSaveData.worldDetail = SaveLoader.Instance.clusterDetailSave;
		gameSaveData.debugWasUsed = debugWasUsed;
		gameSaveData.customGameSettings = CustomGameSettings.Instance;
		gameSaveData.autoPrioritizeRoles = autoPrioritizeRoles;
		gameSaveData.advancedPersonalPriorities = advancedPersonalPriorities;
		gameSaveData.savedInfo = savedInfo;
		Debug.Assert(gameSaveData.worldDetail != null, "World detail null");
		gameSaveData.dateGenerated = dateGenerated;
		if (!changelistsPlayedOn.Contains(512719u))
		{
			changelistsPlayedOn.Add(512719u);
		}
		gameSaveData.changelistsPlayedOn = changelistsPlayedOn;
		if (OnSave != null)
		{
			OnSave(gameSaveData);
		}
		Serializer.Serialize(gameSaveData, writer);
	}

	public void Load(Deserializer deserializer)
	{
		GameSaveData gameSaveData = new GameSaveData();
		gameSaveData.gasConduitFlow = gasConduitFlow;
		gameSaveData.liquidConduitFlow = liquidConduitFlow;
		gameSaveData.fallingWater = world.GetComponent<FallingWater>();
		gameSaveData.unstableGround = world.GetComponent<UnstableGroundManager>();
		gameSaveData.worldDetail = new WorldDetailSave();
		gameSaveData.customGameSettings = CustomGameSettings.Instance;
		deserializer.Deserialize(gameSaveData);
		gasConduitFlow = gameSaveData.gasConduitFlow;
		liquidConduitFlow = gameSaveData.liquidConduitFlow;
		debugWasUsed = gameSaveData.debugWasUsed;
		autoPrioritizeRoles = gameSaveData.autoPrioritizeRoles;
		advancedPersonalPriorities = gameSaveData.advancedPersonalPriorities;
		dateGenerated = gameSaveData.dateGenerated;
		changelistsPlayedOn = gameSaveData.changelistsPlayedOn ?? new List<uint>();
		if (gameSaveData.dateGenerated.IsNullOrWhiteSpace())
		{
			dateGenerated = "Before U41 (Feb 2022)";
		}
		DebugUtil.LogArgs("SAVEINFO");
		DebugUtil.LogArgs(" - Generated: " + dateGenerated);
		DebugUtil.LogArgs(" - Played on: " + string.Join(", ", changelistsPlayedOn));
		savedInfo = gameSaveData.savedInfo;
		savedInfo.InitializeEmptyVariables();
		CustomGameSettings.Instance.Print();
		KCrashReporter.debugWasUsed = debugWasUsed;
		SaveLoader.Instance.SetWorldDetail(gameSaveData.worldDetail);
		if (OnLoad != null)
		{
			OnLoad(gameSaveData);
		}
	}

	public void SetAutoSaveCallbacks(SavingPreCB activatePreCB, SavingActiveCB activateActiveCB, SavingPostCB activatePostCB)
	{
		this.activatePreCB = activatePreCB;
		this.activateActiveCB = activateActiveCB;
		this.activatePostCB = activatePostCB;
	}

	public void StartDelayedInitialSave()
	{
		StartCoroutine(DelayedInitialSave());
	}

	private IEnumerator DelayedInitialSave()
	{
		int i = 0;
		while (i < 1)
		{
			yield return null;
			int num = i + 1;
			i = num;
		}
		if (GenericGameSettings.instance.devAutoWorldGenActive)
		{
			foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
			{
				worldContainer.SetDiscovered(reveal_surface: true);
			}
			SaveGame.Instance.worldGenSpawner.SpawnEverything();
			SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>().DEBUG_REVEAL_ENTIRE_MAP();
			if (CameraController.Instance != null)
			{
				CameraController.Instance.EnableFreeCamera(enable: true);
			}
			for (int j = 0; j != Grid.WidthInCells * Grid.HeightInCells; j++)
			{
				Grid.Reveal(j);
			}
			GenericGameSettings.instance.devAutoWorldGenActive = false;
		}
		SaveLoader.Instance.InitialSave();
	}

	public void StartDelayedSave(string filename, bool isAutoSave = false, bool updateSavePointer = true)
	{
		if (activatePreCB != null)
		{
			activatePreCB(delegate
			{
				StartCoroutine(DelayedSave(filename, isAutoSave, updateSavePointer));
			});
		}
		else
		{
			StartCoroutine(DelayedSave(filename, isAutoSave, updateSavePointer));
		}
	}

	private IEnumerator DelayedSave(string filename, bool isAutoSave, bool updateSavePointer)
	{
		while (PlayerController.Instance.IsDragging())
		{
			yield return null;
		}
		PlayerController.Instance.CancelDragging();
		PlayerController.Instance.AllowDragging(allow: false);
		int k = 0;
		while (k < 1)
		{
			yield return null;
			int num = k + 1;
			k = num;
		}
		if (activateActiveCB != null)
		{
			activateActiveCB();
			k = 0;
			while (k < 1)
			{
				yield return null;
				int num = k + 1;
				k = num;
			}
		}
		SaveLoader.Instance.Save(filename, isAutoSave, updateSavePointer);
		if (activatePostCB != null)
		{
			activatePostCB();
		}
		k = 0;
		while (k < 5)
		{
			yield return null;
			int num = k + 1;
			k = num;
		}
		PlayerController.Instance.AllowDragging(allow: true);
	}

	public void StartDelayed(int tick_delay, System.Action action)
	{
		StartCoroutine(DelayedExecutor(tick_delay, action));
	}

	private IEnumerator DelayedExecutor(int tick_delay, System.Action action)
	{
		int i = 0;
		while (i < tick_delay)
		{
			yield return null;
			int num = i + 1;
			i = num;
		}
		action();
	}

	private void LoadEventHashes()
	{
		foreach (GameHashes value in Enum.GetValues(typeof(GameHashes)))
		{
			HashCache.Get().Add((int)value, value.ToString());
		}
		foreach (UtilHashes value2 in Enum.GetValues(typeof(UtilHashes)))
		{
			HashCache.Get().Add((int)value2, value2.ToString());
		}
		foreach (UIHashes value3 in Enum.GetValues(typeof(UIHashes)))
		{
			HashCache.Get().Add((int)value3, value3.ToString());
		}
	}

	public void StopFE()
	{
		if ((bool)SteamUGCService.Instance)
		{
			SteamUGCService.Instance.enabled = false;
		}
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot);
		if (MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MusicManager.instance.StopSong("Music_FrontEnd");
		}
		MainMenu.Instance.StopMainMenuMusic();
	}

	public void StartBE()
	{
		Resources.UnloadUnusedAssets();
		if (TimeOfDay.Instance != null && !MusicManager.instance.SongIsPlaying("Stinger_Loop_Night") && TimeOfDay.Instance.GetCurrentTimeRegion() == TimeOfDay.TimeRegion.Night)
		{
			MusicManager.instance.PlaySong("Stinger_Loop_Night");
			MusicManager.instance.SetSongParameter("Stinger_Loop_Night", "Music_PlayStinger", 0f);
		}
		AudioMixer.instance.Reset();
		AudioMixer.instance.StartPersistentSnapshots();
		if (MusicManager.instance.ShouldPlayDynamicMusicLoadedGame())
		{
			MusicManager.instance.PlayDynamicMusic();
		}
	}

	public void StopBE()
	{
		if ((bool)SteamUGCService.Instance)
		{
			SteamUGCService.Instance.enabled = true;
		}
		LoopingSoundManager loopingSoundManager = LoopingSoundManager.Get();
		if (loopingSoundManager != null)
		{
			loopingSoundManager.StopAllSounds();
		}
		MusicManager.instance.KillAllSongs(STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.StopPersistentSnapshots();
		foreach (List<SaveLoadRoot> value in SaveLoader.Instance.saveManager.GetLists().Values)
		{
			foreach (SaveLoadRoot item in value)
			{
				if (item.gameObject != null)
				{
					Util.KDestroyGameObject(item.gameObject);
				}
			}
		}
		GetComponent<EntombedItemVisualizer>().Clear();
		SimTemperatureTransfer.ClearInstanceMap();
		StructureTemperatureComponents.ClearInstanceMap();
		ElementConsumer.ClearInstanceMap();
		KComponentSpawn.instance.comps.Clear();
		KInputHandler.Remove(Global.Instance.GetInputManager().GetDefaultController(), cameraController);
		KInputHandler.Remove(Global.Instance.GetInputManager().GetDefaultController(), playerController);
		Sim.Shutdown();
		SimAndRenderScheduler.instance.Reset();
		Resources.UnloadUnusedAssets();
	}

	public void SetStatusItemOffset(Transform transform, Vector3 offset)
	{
		statusItemRenderer.SetOffset(transform, offset);
	}

	public void AddStatusItem(Transform transform, StatusItem status_item)
	{
		statusItemRenderer.Add(transform, status_item);
	}

	public void RemoveStatusItem(Transform transform, StatusItem status_item)
	{
		statusItemRenderer.Remove(transform, status_item);
	}

	public void StartedWork()
	{
		lastTimeWorkStarted = Time.time;
	}

	private void SpawnOxygenBubbles(Vector3 position, float angle)
	{
	}

	public void ManualReleaseHandle(HandleVector<CallbackInfo>.Handle handle)
	{
		if (handle.IsValid())
		{
			callbackManagerManuallyReleasedHandles.Add(handle.index);
			callbackManager.Release(handle);
		}
	}

	private bool IsManuallyReleasedHandle(HandleVector<CallbackInfo>.Handle handle)
	{
		if (!callbackManager.IsVersionValid(handle) && callbackManagerManuallyReleasedHandles.Contains(handle.index))
		{
			return true;
		}
		return false;
	}

	[ContextMenu("Print")]
	private void Print()
	{
		Console.WriteLine("This is a console writeline test");
		Debug.Log("This is a debug log test");
	}

	private void DestroyInstances()
	{
		KMonoBehaviour.lastGameObject = null;
		KMonoBehaviour.lastObj = null;
		GridSettings.ClearGrid();
		StateMachineManager.ResetParameters();
		ChoreTable.Instance.ResetParameters();
		BubbleManager.DestroyInstance();
		AmbientSoundManager.Destroy();
		AutoDisinfectableManager.DestroyInstance();
		BuildMenu.DestroyInstance();
		CancelTool.DestroyInstance();
		ClearTool.DestroyInstance();
		ChoreGroupManager.DestroyInstance();
		CO2Manager.DestroyInstance();
		ConsumerManager.DestroyInstance();
		CopySettingsTool.DestroyInstance();
		DateTime.DestroyInstance();
		DebugBaseTemplateButton.DestroyInstance();
		DebugPaintElementScreen.DestroyInstance();
		DetailsScreen.DestroyInstance();
		DietManager.DestroyInstance();
		DebugText.DestroyInstance();
		FactionManager.DestroyInstance();
		EmptyPipeTool.DestroyInstance();
		FetchListStatusItemUpdater.DestroyInstance();
		FishOvercrowingManager.DestroyInstance();
		FallingWater.DestroyInstance();
		GridCompositor.DestroyInstance();
		Infrared.DestroyInstance();
		KPrefabIDTracker.DestroyInstance();
		ManagementMenu.DestroyInstance();
		ClusterMapScreen.DestroyInstance();
		Messenger.DestroyInstance();
		LoopingSoundManager.DestroyInstance();
		MeterScreen.DestroyInstance();
		MinionGroupProber.DestroyInstance();
		NavPathDrawer.DestroyInstance();
		MinionIdentity.DestroyStatics();
		PathFinder.DestroyStatics();
		Pathfinding.DestroyInstance();
		PrebuildTool.DestroyInstance();
		PrioritizeTool.DestroyInstance();
		SelectTool.DestroyInstance();
		PopFXManager.DestroyInstance();
		ProgressBarsConfig.DestroyInstance();
		PropertyTextures.DestroyInstance();
		RationTracker.DestroyInstance();
		ReportManager.DestroyInstance();
		Research.DestroyInstance();
		RootMenu.DestroyInstance();
		SaveLoader.DestroyInstance();
		Scenario.DestroyInstance();
		SimDebugView.DestroyInstance();
		SpriteSheetAnimManager.DestroyInstance();
		ScheduleManager.DestroyInstance();
		Sounds.DestroyInstance();
		ToolMenu.DestroyInstance();
		WorldDamage.DestroyInstance();
		WaterCubes.DestroyInstance();
		WireBuildTool.DestroyInstance();
		VisibilityTester.DestroyInstance();
		Traces.DestroyInstance();
		TopLeftControlScreen.DestroyInstance();
		UtilityBuildTool.DestroyInstance();
		ReportScreen.DestroyInstance();
		ChorePreconditions.DestroyInstance();
		SandboxBrushTool.DestroyInstance();
		SandboxHeatTool.DestroyInstance();
		SandboxStressTool.DestroyInstance();
		SandboxCritterTool.DestroyInstance();
		SandboxClearFloorTool.DestroyInstance();
		GameScreenManager.DestroyInstance();
		GameScheduler.DestroyInstance();
		NavigationReservations.DestroyInstance();
		Tutorial.DestroyInstance();
		CameraController.DestroyInstance();
		CellEventLogger.DestroyInstance();
		GameFlowManager.DestroyInstance();
		Immigration.DestroyInstance();
		BuildTool.DestroyInstance();
		DebugTool.DestroyInstance();
		DeconstructTool.DestroyInstance();
		DigTool.DestroyInstance();
		DisinfectTool.DestroyInstance();
		HarvestTool.DestroyInstance();
		MopTool.DestroyInstance();
		MoveToLocationTool.DestroyInstance();
		PlaceTool.DestroyInstance();
		SpacecraftManager.DestroyInstance();
		GameplayEventManager.DestroyInstance();
		BuildingInventory.DestroyInstance();
		PlantSubSpeciesCatalog.DestroyInstance();
		SandboxDestroyerTool.DestroyInstance();
		SandboxFOWTool.DestroyInstance();
		SandboxFloodTool.DestroyInstance();
		SandboxSprinkleTool.DestroyInstance();
		StampTool.DestroyInstance();
		OnDemandUpdater.DestroyInstance();
		HoverTextScreen.DestroyInstance();
		ImmigrantScreen.DestroyInstance();
		OverlayMenu.DestroyInstance();
		NameDisplayScreen.DestroyInstance();
		PlanScreen.DestroyInstance();
		ResourceCategoryScreen.DestroyInstance();
		ResourceRemainingDisplayScreen.DestroyInstance();
		SandboxToolParameterMenu.DestroyInstance();
		SpeedControlScreen.DestroyInstance();
		Vignette.DestroyInstance();
		PlayerController.DestroyInstance();
		NotificationScreen.DestroyInstance();
		BuildingCellVisualizerResources.DestroyInstance();
		PauseScreen.DestroyInstance();
		SaveLoadRoot.DestroyStatics();
		KTime.DestroyInstance();
		DemoTimer.DestroyInstance();
		UIScheduler.DestroyInstance();
		SaveGame.DestroyInstance();
		GameClock.DestroyInstance();
		TimeOfDay.DestroyInstance();
		DeserializeWarnings.DestroyInstance();
		UISounds.DestroyInstance();
		RenderTextureDestroyer.DestroyInstance();
		HoverTextHelper.DestroyStatics();
		LoadScreen.DestroyInstance();
		LoadingOverlay.DestroyInstance();
		SimAndRenderScheduler.DestroyInstance();
		Singleton<CellChangeMonitor>.DestroyInstance();
		Singleton<StateMachineManager>.Instance.Clear();
		Singleton<StateMachineUpdater>.Instance.Clear();
		UpdateObjectCountParameter.Clear();
		MaterialSelectionPanel.ClearStatics();
		StarmapScreen.DestroyInstance();
		ClusterNameDisplayScreen.DestroyInstance();
		ClusterManager.DestroyInstance();
		ClusterGrid.DestroyInstance();
		PathFinderQueries.Reset();
		Singleton<KBatchedAnimUpdater>.Instance?.InitializeGrid();
		GlobalChoreProvider.DestroyInstance();
		WorldSelector.DestroyInstance();
		ColonyDiagnosticUtility.DestroyInstance();
		DiscoveredResources.DestroyInstance();
		ClusterMapSelectTool.DestroyInstance();
		Instance = null;
		Grid.OnReveal = null;
		VisualTunerElement = null;
		Assets.ClearOnAddPrefab();
		KMonoBehaviour.lastGameObject = null;
		KMonoBehaviour.lastObj = null;
		(KComponentSpawn.instance.comps as GameComps).Clear();
	}
}
