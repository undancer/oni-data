using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

public class LaunchPad : KMonoBehaviour, ISim1000ms, IListableOption, IProcessConditionSet
{
	public class LaunchPadTower
	{
		private LaunchPad pad;

		private KAnimLink animLink;

		private Coroutine activeAnimationRoutine;

		private string[] towerBGAnimNames = new string[10] { "A1", "A2", "A3", "B", "C", "D", "E1", "E2", "F1", "F2" };

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
			kBatchedAnimController.AnimFiles = new KAnimFile[1] { Assets.GetAnim("rocket_launchpad_tower_kanim") };
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
				bool animComplete = false;
				towerAnimControllers[currentHeight].Queue(towerBGAnimNames[currentHeight % towerBGAnimNames.Length] + towerBGAnimSuffix_on_pre);
				towerAnimControllers[currentHeight].Queue(towerBGAnimNames[currentHeight % towerBGAnimNames.Length] + towerBGAnimSuffix_on);
				towerAnimControllers[currentHeight].onAnimComplete += delegate
				{
					animComplete = true;
				};
				float delay = 0.25f;
				while (!animComplete && delay > 0f)
				{
					delay -= Time.deltaTime;
					yield return 0;
				}
				currentHeight++;
			}
			while (currentHeight > targetHeight)
			{
				currentHeight--;
				bool animComplete2 = false;
				towerAnimControllers[currentHeight].Queue(towerBGAnimNames[currentHeight % towerBGAnimNames.Length] + towerBGAnimSuffix_off_pre);
				towerAnimControllers[currentHeight].Queue(towerBGAnimNames[currentHeight % towerBGAnimNames.Length] + towerBGAnimSuffix_off);
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
			}
			yield return 0;
		}
	}

	public HashedString triggerPort;

	public HashedString statusPort;

	public HashedString landedRocketPort;

	private CellOffset baseModulePosition = new CellOffset(0, 2);

	private SchedulerHandle RebuildLaunchTowerHeightHandler;

	private AttachableBuilding lastBaseAttachable;

	private LaunchPadTower tower;

	[Serialize]
	public int maxTowerHeight;

	private bool dirtyTowerHeight;

	private HandleVector<int>.Handle partitionerEntry;

	private Guid landedRocketPassengerModuleStatusItem = Guid.Empty;

	public RocketModuleCluster LandedRocket
	{
		get
		{
			GameObject value = null;
			Grid.ObjectLayers[1].TryGetValue(RocketBottomPosition, out value);
			if (value != null)
			{
				RocketModuleCluster component = value.GetComponent<RocketModuleCluster>();
				Clustercraft clustercraft = ((component != null && component.CraftInterface != null) ? component.CraftInterface.GetComponent<Clustercraft>() : null);
				if (clustercraft != null && (clustercraft.Status == Clustercraft.CraftStatus.Grounded || clustercraft.Status == Clustercraft.CraftStatus.Landing))
				{
					return component;
				}
			}
			return null;
		}
	}

	public int RocketBottomPosition => Grid.OffsetCell(Grid.PosToCell(this), baseModulePosition);

	[OnDeserialized]
	private void OnDeserialzed()
	{
		if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 24))
		{
			return;
		}
		Building component = GetComponent<Building>();
		if (!(component != null))
		{
			return;
		}
		component.RunOnArea(delegate(int cell)
		{
			if (Grid.IsValidCell(cell) && Grid.Solid[cell])
			{
				SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.LaunchpadDesolidify, 0f);
			}
		});
	}

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
		partitionerEntry = GameScenePartitioner.Instance.Add("LaunchPad.OnSpawn", base.gameObject, Extents.OneCell(RocketBottomPosition), GameScenePartitioner.Instance.objectLayers[1], OnRocketBuildingChanged);
		Components.LaunchPads.Add(this);
		CheckLandedRocketPassengerModuleStatus();
		int num = ConditionFlightPathIsClear.PadTopEdgeDistanceToCeilingEdge(base.gameObject);
		if (num < 35)
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
			attachableBuilding.onAttachmentNetworkChanged = (Action<object>)Delegate.Remove(attachableBuilding.onAttachmentNetworkChanged, new Action<object>(OnRocketLayoutChanged));
			lastBaseAttachable = null;
		}
		RebuildLaunchTowerHeightHandler.ClearScheduler();
		base.OnCleanUp();
	}

	private void CheckLandedRocketPassengerModuleStatus()
	{
		if (LandedRocket == null)
		{
			GetComponent<KSelectable>().RemoveStatusItem(landedRocketPassengerModuleStatusItem);
			landedRocketPassengerModuleStatusItem = Guid.Empty;
		}
		else if (LandedRocket.CraftInterface.GetPassengerModule() == null)
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
		int portCell = GetComponent<LogicPorts>().GetPortCell(triggerPort);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell) != null;
	}

	public void Sim1000ms(float dt)
	{
		LogicPorts component = base.gameObject.GetComponent<LogicPorts>();
		RocketModuleCluster landedRocket = LandedRocket;
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
		component.SendSignal(landedRocketPort, (landedRocket != null) ? 1 : 0);
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
		GameObject obj = Util.KInstantiate(Assets.GetPrefab("Clustercraft"));
		obj.SetActive(value: true);
		Clustercraft component = obj.GetComponent<Clustercraft>();
		component.GetComponent<CraftModuleInterface>().AddModule(gameObject.GetComponent<RocketModuleCluster>());
		component.Init(this.GetMyWorldLocation(), this);
		if (gameObject.GetComponent<BuildingUnderConstruction>() != null)
		{
			OnRocketBuildingChanged(gameObject);
		}
		Trigger(374403796);
		return gameObject;
	}

	private void OnRocketBuildingChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		RocketModuleCluster landedRocket = LandedRocket;
		Debug.Assert(gameObject == null || landedRocket == null || landedRocket.gameObject == gameObject, "Launch Pad had a rocket land or take off on it twice??");
		Clustercraft clustercraft = ((landedRocket != null && landedRocket.CraftInterface != null) ? landedRocket.CraftInterface.GetComponent<Clustercraft>() : null);
		if (clustercraft != null)
		{
			if (clustercraft.Status == Clustercraft.CraftStatus.Landing)
			{
				Trigger(-887025858, landedRocket);
			}
			else if (clustercraft.Status == Clustercraft.CraftStatus.Launching)
			{
				Trigger(-1277991738, landedRocket);
				AttachableBuilding component = landedRocket.CraftInterface.ClusterModules[0].Get().GetComponent<AttachableBuilding>();
				component.onAttachmentNetworkChanged = (Action<object>)Delegate.Remove(component.onAttachmentNetworkChanged, new Action<object>(OnRocketLayoutChanged));
			}
		}
		OnRocketLayoutChanged(null);
	}

	private void OnRocketLayoutChanged(object data)
	{
		if (lastBaseAttachable != null)
		{
			AttachableBuilding attachableBuilding = lastBaseAttachable;
			attachableBuilding.onAttachmentNetworkChanged = (Action<object>)Delegate.Remove(attachableBuilding.onAttachmentNetworkChanged, new Action<object>(OnRocketLayoutChanged));
			lastBaseAttachable = null;
		}
		GameObject rocketBaseModule = GetRocketBaseModule();
		if (rocketBaseModule != null)
		{
			lastBaseAttachable = rocketBaseModule.GetComponent<AttachableBuilding>();
			AttachableBuilding attachableBuilding2 = lastBaseAttachable;
			attachableBuilding2.onAttachmentNetworkChanged = (Action<object>)Delegate.Combine(attachableBuilding2.onAttachmentNetworkChanged, new Action<object>(OnRocketLayoutChanged));
		}
		DirtyTowerHeight();
	}

	public bool HasRocket()
	{
		return LandedRocket != null;
	}

	public bool HasRocketWithCommandModule()
	{
		if (HasRocket())
		{
			return LandedRocket.CraftInterface.FindLaunchableRocket() != null;
		}
		return false;
	}

	private GameObject GetRocketBaseModule()
	{
		GameObject gameObject = Grid.Objects[Grid.OffsetCell(Grid.PosToCell(base.gameObject), baseModulePosition), 1];
		if (!(gameObject != null) || !(gameObject.GetComponent<RocketModule>() != null))
		{
			return null;
		}
		return gameObject;
	}

	public void DirtyTowerHeight()
	{
		if (!dirtyTowerHeight)
		{
			dirtyTowerHeight = true;
			if (!RebuildLaunchTowerHeightHandler.IsValid)
			{
				RebuildLaunchTowerHeightHandler = GameScheduler.Instance.ScheduleNextFrame("RebuildLaunchTowerHeight", RebuildLaunchTowerHeight);
			}
		}
	}

	private void RebuildLaunchTowerHeight(object obj)
	{
		RocketModuleCluster landedRocket = LandedRocket;
		if (landedRocket != null)
		{
			tower.SetTowerHeight(landedRocket.CraftInterface.MaxHeight);
		}
		dirtyTowerHeight = false;
		RebuildLaunchTowerHeightHandler.ClearScheduler();
	}

	public string GetProperName()
	{
		return base.gameObject.GetProperName();
	}

	public List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		RocketProcessConditionDisplayTarget rocketProcessConditionDisplayTarget = null;
		RocketModuleCluster landedRocket = LandedRocket;
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
