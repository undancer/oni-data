using System;
using System.Collections;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class LaunchPad : KMonoBehaviour, ISim1000ms, IListableOption, IProcessConditionSet
{
	public class LaunchPadTower
	{
		private LaunchPad pad;

		private KAnimLink animLink;

		private Coroutine activeAnimationRoutine = null;

		private string[] towerBGAnimNames = new string[10]
		{
			"A1",
			"A2",
			"A3",
			"B",
			"C",
			"D",
			"E1",
			"E2",
			"F1",
			"F2"
		};

		private string towerBGAnimSuffix_on = "_on";

		private string towerBGAnimSuffix_on_pre = "_on_pre";

		private string towerBGAnimSuffix_off_pre = "_off_pre";

		private string towerBGAnimSuffix_off = "_off";

		private List<KBatchedAnimController> towerAnimControllers = new List<KBatchedAnimController>();

		private int targetHeight;

		private int currentHeight;

		public LaunchPadTower(LaunchPad pad, int startHeight)
		{
			this.pad = pad;
			SetTowerHeight(startHeight);
		}

		public void AddTowerRow()
		{
			GameObject gameObject = new GameObject("LaunchPadTowerRow");
			gameObject.SetActive(value: false);
			gameObject.transform.SetParent(pad.transform);
			gameObject.transform.SetLocalPosition(Grid.CellSizeInMeters * Vector3.up * (towerAnimControllers.Count + pad.baseModulePosition.y));
			gameObject.transform.SetPosition(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.Backwall)));
			KBatchedAnimController kBatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
			kBatchedAnimController.AnimFiles = new KAnimFile[1]
			{
				Assets.GetAnim("rocket_launchpad_tower_kanim")
			};
			gameObject.SetActive(value: true);
			towerAnimControllers.Add(kBatchedAnimController);
			kBatchedAnimController.initialAnim = towerBGAnimNames[towerAnimControllers.Count % towerBGAnimNames.Length] + towerBGAnimSuffix_off;
			animLink = new KAnimLink(pad.GetComponent<KAnimControllerBase>(), kBatchedAnimController);
		}

		public void RemoveTowerRow()
		{
		}

		public void SetTowerHeight(int height)
		{
			if (height < 8)
			{
				height = 0;
			}
			targetHeight = height;
			pad.maxTowerHeight = height;
			while (targetHeight > towerAnimControllers.Count)
			{
				AddTowerRow();
			}
			if (activeAnimationRoutine != null)
			{
				pad.StopCoroutine(activeAnimationRoutine);
			}
			activeAnimationRoutine = pad.StartCoroutine(TowerRoutine());
		}

		private IEnumerator TowerRoutine()
		{
			while (currentHeight < targetHeight)
			{
				bool animComplete2 = false;
				towerAnimControllers[currentHeight].Queue(towerBGAnimNames[currentHeight % towerBGAnimNames.Length] + towerBGAnimSuffix_on_pre);
				towerAnimControllers[currentHeight].Queue(towerBGAnimNames[currentHeight % towerBGAnimNames.Length] + towerBGAnimSuffix_on);
				towerAnimControllers[currentHeight].onAnimComplete += delegate
				{
					animComplete2 = true;
				};
				float delay = 0.25f;
				while (!animComplete2 && delay > 0f)
				{
					delay -= Time.deltaTime;
					yield return 0;
				}
				currentHeight++;
			}
			while (currentHeight > targetHeight)
			{
				currentHeight--;
				bool animComplete = false;
				towerAnimControllers[currentHeight].Queue(towerBGAnimNames[currentHeight % towerBGAnimNames.Length] + towerBGAnimSuffix_off_pre);
				towerAnimControllers[currentHeight].Queue(towerBGAnimNames[currentHeight % towerBGAnimNames.Length] + towerBGAnimSuffix_off);
				towerAnimControllers[currentHeight].onAnimComplete += delegate
				{
					animComplete = true;
				};
				float delay2 = 0.25f;
				while (!animComplete && delay2 > 0f)
				{
					delay2 -= Time.deltaTime;
					yield return 0;
				}
			}
			yield return 0;
		}
	}

	public HashedString triggerPort;

	public HashedString statusPort;

	private CellOffset baseModulePosition = new CellOffset(0, 2);

	private RocketModuleCluster landedRocket;

	private AttachableBuilding lastBaseAttachable;

	private LaunchPadTower tower;

	[Serialize]
	public int maxTowerHeight;

	private bool dirtyTowerHeight;

	private HandleVector<int>.Handle partitionerEntry;

	private Guid landedRocketPassengerModuleStatusItem = Guid.Empty;

	public RocketModuleCluster LandedRocket => landedRocket;

	public int PadPosition => Grid.OffsetCell(Grid.PosToCell(this), baseModulePosition);

	protected override void OnPrefabInit()
	{
		UserNameable component = GetComponent<UserNameable>();
		if (component != null)
		{
			component.SetName(GameUtil.GenerateRandomLaunchPadName());
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		tower = new LaunchPadTower(this, maxTowerHeight);
		OnRocketBuildingChanged(GetRocketBaseModule());
		partitionerEntry = GameScenePartitioner.Instance.Add("LaunchPad.OnSpawn", base.gameObject, Extents.OneCell(PadPosition), GameScenePartitioner.Instance.objectLayers[1], OnRocketBuildingChanged);
		Components.LaunchPads.Add(this);
		if (landedRocket != null && landedRocket.CraftInterface.CurrentPad == null)
		{
			landedRocket.CraftInterface.SetCurrentPad(this);
		}
		CheckLandedRocketPassengerModuleStatus();
		int num = ConditionFlightPathIsClear.PadPositionDistanceToCeiling(base.gameObject);
		if (num < ConditionFlightPathIsClear.maximumRocketHeight)
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.RocketPlatformCloseToCeiling, num);
		}
	}

	protected override void OnCleanUp()
	{
		Components.LaunchPads.Remove(this);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		if (lastBaseAttachable != null)
		{
			AttachableBuilding attachableBuilding = lastBaseAttachable;
			attachableBuilding.onAttachmentNetworkChanged = (Action<AttachableBuilding>)Delegate.Remove(attachableBuilding.onAttachmentNetworkChanged, new Action<AttachableBuilding>(OnRocketLayoutChanged));
			lastBaseAttachable = null;
		}
		base.OnCleanUp();
	}

	private void CheckLandedRocketPassengerModuleStatus()
	{
		if (landedRocket == null)
		{
			GetComponent<KSelectable>().RemoveStatusItem(landedRocketPassengerModuleStatusItem);
			landedRocketPassengerModuleStatusItem = Guid.Empty;
		}
		else if (landedRocket.CraftInterface.GetPassengerModule() == null)
		{
			if (landedRocketPassengerModuleStatusItem == Guid.Empty)
			{
				landedRocketPassengerModuleStatusItem = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.LandedRocketLacksPassengerModule);
			}
		}
		else if (landedRocketPassengerModuleStatusItem != Guid.Empty)
		{
			GetComponent<KSelectable>().RemoveStatusItem(landedRocketPassengerModuleStatusItem);
			landedRocketPassengerModuleStatusItem = Guid.Empty;
		}
	}

	public bool IsLogicInputConnected()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		int portCell = component.GetPortCell(triggerPort);
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(portCell);
		return networkForCell != null;
	}

	public void Sim1000ms(float dt)
	{
		LogicPorts component = base.gameObject.GetComponent<LogicPorts>();
		if (landedRocket != null && IsLogicInputConnected())
		{
			if (component.GetInputValue(triggerPort) == 1)
			{
				if (landedRocket.CraftInterface.CheckReadyForAutomatedLaunchCommand())
				{
					landedRocket.CraftInterface.TriggerLaunch(automated: true);
				}
				else
				{
					landedRocket.CraftInterface.CancelLaunch();
				}
			}
			else
			{
				landedRocket.CraftInterface.CancelLaunch();
			}
		}
		CheckLandedRocketPassengerModuleStatus();
		if (landedRocket != null)
		{
			component.SendSignal(statusPort, (landedRocket.CraftInterface.CheckReadyForAutomatedLaunch() || landedRocket.CraftInterface.HasTag(GameTags.RocketNotOnGround)) ? 1 : 0);
		}
		else
		{
			component.SendSignal(statusPort, 0);
		}
	}

	public GameObject AddBaseModule(BuildingDef moduleDefID, IList<Tag> elements)
	{
		int cell = Grid.OffsetCell(Grid.PosToCell(base.gameObject), baseModulePosition);
		GameObject gameObject = null;
		gameObject = ((!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive) ? moduleDefID.TryPlace(null, Grid.CellToPosCBC(cell, moduleDefID.SceneLayer), Orientation.Neutral, elements) : moduleDefID.Build(cell, Orientation.Neutral, null, elements, 293.15f, playsound: true, GameClock.Instance.GetTime()));
		GameObject gameObject2 = Util.KInstantiate(Assets.GetPrefab("Clustercraft"));
		gameObject2.SetActive(value: true);
		Clustercraft component = gameObject2.GetComponent<Clustercraft>();
		CraftModuleInterface component2 = component.GetComponent<CraftModuleInterface>();
		component2.AddModule(gameObject.GetComponent<RocketModuleCluster>());
		component.Init(this.GetMyWorldLocation(), this);
		if (gameObject.GetComponent<BuildingUnderConstruction>() != null)
		{
			OnRocketBuildingChanged(gameObject);
		}
		return gameObject;
	}

	private void OnRocketBuildingChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		Debug.Assert(gameObject == null || landedRocket == null || landedRocket.gameObject == gameObject, "Launch Pad had a rocket land or take off on it twice??");
		if (landedRocket == null && gameObject != null)
		{
			AttachableBuilding component = gameObject.GetComponent<AttachableBuilding>();
			BuildingAttachPoint component2 = gameObject.GetComponent<BuildingAttachPoint>();
			landedRocket = gameObject.GetComponent<RocketModuleCluster>();
			Trigger(-887025858, landedRocket);
		}
		else if (landedRocket != null && gameObject == null)
		{
			Trigger(-1277991738, landedRocket);
			AttachableBuilding component3 = landedRocket.CraftInterface.ClusterModules[0].Get().GetComponent<AttachableBuilding>();
			component3.onAttachmentNetworkChanged = (Action<AttachableBuilding>)Delegate.Remove(component3.onAttachmentNetworkChanged, new Action<AttachableBuilding>(OnRocketLayoutChanged));
			landedRocket = null;
		}
		OnRocketLayoutChanged(null);
	}

	private void OnRocketLayoutChanged(AttachableBuilding data)
	{
		if (lastBaseAttachable != null)
		{
			AttachableBuilding attachableBuilding = lastBaseAttachable;
			attachableBuilding.onAttachmentNetworkChanged = (Action<AttachableBuilding>)Delegate.Remove(attachableBuilding.onAttachmentNetworkChanged, new Action<AttachableBuilding>(OnRocketLayoutChanged));
			lastBaseAttachable = null;
		}
		GameObject rocketBaseModule = GetRocketBaseModule();
		if (rocketBaseModule != null)
		{
			lastBaseAttachable = rocketBaseModule.GetComponent<AttachableBuilding>();
			AttachableBuilding attachableBuilding2 = lastBaseAttachable;
			attachableBuilding2.onAttachmentNetworkChanged = (Action<AttachableBuilding>)Delegate.Combine(attachableBuilding2.onAttachmentNetworkChanged, new Action<AttachableBuilding>(OnRocketLayoutChanged));
		}
		DirtyTowerHeight();
	}

	public bool HasRocket()
	{
		return landedRocket != null;
	}

	public bool HasRocketWithCommandModule()
	{
		return HasRocket() && landedRocket.CraftInterface.FindLaunchableRocket() != null;
	}

	private GameObject GetRocketBaseModule()
	{
		GameObject gameObject = Grid.Objects[Grid.OffsetCell(Grid.PosToCell(base.gameObject), baseModulePosition), 1];
		return (gameObject != null && gameObject.GetComponent<RocketModule>() != null) ? gameObject : null;
	}

	public void DirtyTowerHeight()
	{
		if (!dirtyTowerHeight)
		{
			dirtyTowerHeight = true;
			GameScheduler.Instance.ScheduleNextFrame("RebuildLaunchTowerHeight", RebuildLaunchTowerHeight);
		}
	}

	private void RebuildLaunchTowerHeight(object obj)
	{
		if (landedRocket != null)
		{
			tower.SetTowerHeight(landedRocket.CraftInterface.MaxHeight);
		}
		dirtyTowerHeight = false;
	}

	public string GetProperName()
	{
		return base.gameObject.GetProperName();
	}

	public List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		RocketProcessConditionDisplayTarget rocketProcessConditionDisplayTarget = null;
		if (landedRocket != null)
		{
			for (int i = 0; i < landedRocket.CraftInterface.ClusterModules.Count; i++)
			{
				RocketProcessConditionDisplayTarget component = landedRocket.CraftInterface.ClusterModules[i].Get().GetComponent<RocketProcessConditionDisplayTarget>();
				if (component != null)
				{
					rocketProcessConditionDisplayTarget = component;
					break;
				}
			}
		}
		if (rocketProcessConditionDisplayTarget != null)
		{
			return ((IProcessConditionSet)rocketProcessConditionDisplayTarget).GetConditionSet(conditionType);
		}
		return new List<ProcessCondition>();
	}

	public static List<LaunchPad> GetLaunchPadsForDestination(AxialI destination)
	{
		List<LaunchPad> list = new List<LaunchPad>();
		foreach (LaunchPad launchPad in Components.LaunchPads)
		{
			if (launchPad.GetMyWorldLocation() == destination)
			{
				list.Add(launchPad);
			}
		}
		return list;
	}
}