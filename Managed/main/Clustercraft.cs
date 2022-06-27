using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class Clustercraft : ClusterGridEntity, IClusterRange, ISim4000ms
{
	public enum CraftStatus
	{
		Grounded,
		Launching,
		InFlight,
		Landing
	}

	public enum CombustionResource
	{
		Fuel,
		Oxidizer,
		All
	}

	public enum PadLandingStatus
	{
		CanLandImmediately,
		CanLandEventually,
		CanNeverLand
	}

	[Serialize]
	private string m_name;

	[MyCmpReq]
	private ClusterTraveler m_clusterTraveler;

	[MyCmpReq]
	private CraftModuleInterface m_moduleInterface;

	private Guid mainStatusHandle;

	private Guid cargoStatusHandle;

	public static Dictionary<Tag, float> dlc1OxidizerEfficiencies = new Dictionary<Tag, float>
	{
		{
			SimHashes.OxyRock.CreateTag(),
			ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.LOW
		},
		{
			SimHashes.LiquidOxygen.CreateTag(),
			ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.HIGH
		},
		{
			SimHashes.Fertilizer.CreateTag(),
			ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.VERY_LOW
		}
	};

	[Serialize]
	[Range(0f, 1f)]
	public float AutoPilotMultiplier = 1f;

	[Serialize]
	[Range(0f, 2f)]
	public float PilotSkillMultiplier = 1f;

	[Serialize]
	private bool m_launchRequested;

	[Serialize]
	private CraftStatus status;

	private static EventSystem.IntraObjectHandler<Clustercraft> RocketModuleChangedHandler = new EventSystem.IntraObjectHandler<Clustercraft>(delegate(Clustercraft cmp, object data)
	{
		cmp.RocketModuleChanged(data);
	});

	private static EventSystem.IntraObjectHandler<Clustercraft> ClusterDestinationChangedHandler = new EventSystem.IntraObjectHandler<Clustercraft>(delegate(Clustercraft cmp, object data)
	{
		cmp.OnClusterDestinationChanged(data);
	});

	private static EventSystem.IntraObjectHandler<Clustercraft> ClusterDestinationReachedHandler = new EventSystem.IntraObjectHandler<Clustercraft>(delegate(Clustercraft cmp, object data)
	{
		cmp.OnClusterDestinationReached(data);
	});

	public override string Name => m_name;

	public bool Exploding { get; protected set; }

	public override EntityLayer Layer => EntityLayer.Craft;

	public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
	{
		new AnimConfig
		{
			animFile = Assets.GetAnim("rocket01_kanim"),
			initialAnim = "idle_loop"
		}
	};

	public override bool IsVisible => !Exploding;

	public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;

	public CraftModuleInterface ModuleInterface => m_moduleInterface;

	public AxialI Destination => m_moduleInterface.GetClusterDestinationSelector().GetDestination();

	public float Speed => EnginePower / TotalBurden * AutoPilotMultiplier * PilotSkillMultiplier;

	public float EnginePower
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModuleCluster> clusterModule in m_moduleInterface.ClusterModules)
			{
				num += clusterModule.Get().performanceStats.EnginePower;
			}
			return num;
		}
	}

	public float FuelPerDistance
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModuleCluster> clusterModule in m_moduleInterface.ClusterModules)
			{
				num += clusterModule.Get().performanceStats.FuelKilogramPerDistance;
			}
			return num;
		}
	}

	public float TotalBurden
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModuleCluster> clusterModule in m_moduleInterface.ClusterModules)
			{
				num += clusterModule.Get().performanceStats.Burden;
			}
			Debug.Assert(num > 0f);
			return num;
		}
	}

	public bool LaunchRequested
	{
		get
		{
			return m_launchRequested;
		}
		private set
		{
			m_launchRequested = value;
			m_moduleInterface.TriggerEventOnCraftAndRocket(GameHashes.RocketRequestLaunch, this);
		}
	}

	public CraftStatus Status => status;

	public override Sprite GetUISprite()
	{
		return Def.GetUISprite(m_moduleInterface.GetPassengerModule().gameObject).first;
	}

	public override bool SpaceOutInSameHex()
	{
		return true;
	}

	public void SetCraftStatus(CraftStatus craft_status)
	{
		status = craft_status;
		UpdateGroundTags();
	}

	public void SetExploding()
	{
		Exploding = true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Clustercrafts.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		m_clusterTraveler.getSpeedCB = GetSpeed;
		m_clusterTraveler.getCanTravelCB = CanTravel;
		m_clusterTraveler.onTravelCB = BurnFuelForTravel;
		m_clusterTraveler.validateTravelCB = CanTravelToCell;
		UpdateGroundTags();
		Subscribe(1512695988, RocketModuleChangedHandler);
		Subscribe(543433792, ClusterDestinationChangedHandler);
		Subscribe(1796608350, ClusterDestinationReachedHandler);
		Subscribe(-688990705, delegate
		{
			UpdateStatusItem();
		});
		SetRocketName(m_name);
		UpdateStatusItem();
	}

	public void Sim4000ms(float dt)
	{
		RocketClusterDestinationSelector clusterDestinationSelector = m_moduleInterface.GetClusterDestinationSelector();
		if (Status == CraftStatus.InFlight && m_location == clusterDestinationSelector.GetDestination())
		{
			OnClusterDestinationReached(null);
		}
	}

	public void Init(AxialI location, LaunchPad pad)
	{
		m_location = location;
		GetComponent<RocketClusterDestinationSelector>().SetDestination(m_location);
		SetRocketName(GameUtil.GenerateRandomRocketName());
		if (pad != null)
		{
			Land(pad, forceGrounded: true);
		}
		UpdateStatusItem();
	}

	protected override void OnCleanUp()
	{
		Components.Clustercrafts.Remove(this);
		base.OnCleanUp();
	}

	private bool CanTravel(bool tryingToLand)
	{
		if (this.HasTag(GameTags.RocketInSpace))
		{
			if (!tryingToLand)
			{
				return HasResourcesToMove();
			}
			return true;
		}
		return false;
	}

	private bool CanTravelToCell(AxialI location)
	{
		if (ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(location, EntityLayer.Asteroid) != null)
		{
			return CanLandAtAsteroid(location, mustLandImmediately: true);
		}
		return true;
	}

	private float GetSpeed()
	{
		return Speed;
	}

	private void RocketModuleChanged(object data)
	{
		RocketModuleCluster rocketModuleCluster = (RocketModuleCluster)data;
		if (rocketModuleCluster != null)
		{
			UpdateGroundTags(rocketModuleCluster.gameObject);
		}
	}

	private void OnClusterDestinationChanged(object data)
	{
		UpdateStatusItem();
	}

	private void OnClusterDestinationReached(object data)
	{
		RocketClusterDestinationSelector clusterDestinationSelector = m_moduleInterface.GetClusterDestinationSelector();
		Debug.Assert(base.Location == clusterDestinationSelector.GetDestination());
		if (clusterDestinationSelector.HasAsteroidDestination())
		{
			LaunchPad destinationPad = clusterDestinationSelector.GetDestinationPad();
			Land(base.Location, destinationPad);
		}
		UpdateStatusItem();
	}

	public void SetRocketName(string newName)
	{
		m_name = newName;
		base.name = "Clustercraft: " + newName;
		foreach (Ref<RocketModuleCluster> clusterModule in m_moduleInterface.ClusterModules)
		{
			CharacterOverlay component = clusterModule.Get().GetComponent<CharacterOverlay>();
			if (component != null)
			{
				NameDisplayScreen.Instance.UpdateName(component.gameObject);
				break;
			}
		}
	}

	public bool CheckPreppedForLaunch()
	{
		return m_moduleInterface.CheckPreppedForLaunch();
	}

	public bool CheckReadyToLaunch()
	{
		return m_moduleInterface.CheckReadyToLaunch();
	}

	public bool IsFlightInProgress()
	{
		if (Status == CraftStatus.InFlight)
		{
			return m_clusterTraveler.IsTraveling();
		}
		return false;
	}

	public ClusterGridEntity GetPOIAtCurrentLocation()
	{
		if (status != CraftStatus.InFlight || IsFlightInProgress())
		{
			return null;
		}
		return ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(m_location, EntityLayer.POI);
	}

	public ClusterGridEntity GetStableOrbitAsteroid()
	{
		if (status != CraftStatus.InFlight || IsFlightInProgress())
		{
			return null;
		}
		return ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(m_location, EntityLayer.Asteroid);
	}

	public ClusterGridEntity GetOrbitAsteroid()
	{
		if (status != CraftStatus.InFlight)
		{
			return null;
		}
		return ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(m_location, EntityLayer.Asteroid);
	}

	public ClusterGridEntity GetAdjacentAsteroid()
	{
		return ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(m_location, EntityLayer.Asteroid);
	}

	private bool CheckDesinationInRange()
	{
		if (m_clusterTraveler.CurrentPath == null)
		{
			return false;
		}
		return Speed * m_clusterTraveler.TravelETA() <= ModuleInterface.Range;
	}

	public bool HasResourcesToMove(int hexes = 1, CombustionResource combustionResource = CombustionResource.All)
	{
		return combustionResource switch
		{
			CombustionResource.All => m_moduleInterface.BurnableMassRemaining / FuelPerDistance >= 600f * (float)hexes - 0.001f, 
			CombustionResource.Fuel => m_moduleInterface.FuelRemaining / FuelPerDistance >= 600f * (float)hexes - 0.001f, 
			CombustionResource.Oxidizer => m_moduleInterface.OxidizerPowerRemaining / FuelPerDistance >= 600f * (float)hexes - 0.001f, 
			_ => false, 
		};
	}

	private void BurnFuelForTravel()
	{
		float num = 600f;
		foreach (Ref<RocketModuleCluster> clusterModule in m_moduleInterface.ClusterModules)
		{
			RocketEngineCluster component = clusterModule.Get().GetComponent<RocketEngineCluster>();
			if (!(component != null))
			{
				continue;
			}
			Tag fuelTag = component.fuelTag;
			float totalOxidizerRemaining = 0f;
			if (component.requireOxidizer)
			{
				totalOxidizerRemaining = ModuleInterface.OxidizerPowerRemaining;
			}
			if (!(num > 0f))
			{
				continue;
			}
			foreach (Ref<RocketModuleCluster> clusterModule2 in m_moduleInterface.ClusterModules)
			{
				IFuelTank component2 = clusterModule2.Get().GetComponent<IFuelTank>();
				if (!component2.IsNullOrDestroyed())
				{
					num -= BurnFromTank(num, component, fuelTag, component2.Storage, ref totalOxidizerRemaining);
				}
				if (num <= 0f)
				{
					break;
				}
			}
		}
		UpdateStatusItem();
	}

	private float BurnFromTank(float attemptTravelAmount, RocketEngineCluster engine, Tag fuelTag, IStorage storage, ref float totalOxidizerRemaining)
	{
		float b = attemptTravelAmount * engine.GetComponent<RocketModuleCluster>().performanceStats.FuelKilogramPerDistance;
		b = Mathf.Min(storage.GetAmountAvailable(fuelTag), b);
		if (engine.requireOxidizer)
		{
			b = Mathf.Min(b, totalOxidizerRemaining);
		}
		storage.ConsumeIgnoringDisease(fuelTag, b);
		if (engine.requireOxidizer)
		{
			BurnOxidizer(b);
			totalOxidizerRemaining -= b;
		}
		return b / engine.GetComponent<RocketModuleCluster>().performanceStats.FuelKilogramPerDistance;
	}

	private void BurnOxidizer(float fuelEquivalentKGs)
	{
		foreach (Ref<RocketModuleCluster> clusterModule in m_moduleInterface.ClusterModules)
		{
			OxidizerTank component = clusterModule.Get().GetComponent<OxidizerTank>();
			if (component != null)
			{
				foreach (KeyValuePair<Tag, float> item in component.GetOxidizersAvailable())
				{
					float num = dlc1OxidizerEfficiencies[item.Key];
					float num2 = Mathf.Min(fuelEquivalentKGs / num, item.Value);
					if (num2 > 0f)
					{
						component.storage.ConsumeIgnoringDisease(item.Key, num2);
						fuelEquivalentKGs -= num2 * num;
					}
				}
			}
			if (fuelEquivalentKGs <= 0f)
			{
				break;
			}
		}
	}

	public List<ResourceHarvestModule.StatesInstance> GetAllResourceHarvestModules()
	{
		List<ResourceHarvestModule.StatesInstance> list = new List<ResourceHarvestModule.StatesInstance>();
		foreach (Ref<RocketModuleCluster> clusterModule in m_moduleInterface.ClusterModules)
		{
			ResourceHarvestModule.StatesInstance sMI = clusterModule.Get().GetSMI<ResourceHarvestModule.StatesInstance>();
			if (sMI != null)
			{
				list.Add(sMI);
			}
		}
		return list;
	}

	public List<ArtifactHarvestModule.StatesInstance> GetAllArtifactHarvestModules()
	{
		List<ArtifactHarvestModule.StatesInstance> list = new List<ArtifactHarvestModule.StatesInstance>();
		foreach (Ref<RocketModuleCluster> clusterModule in m_moduleInterface.ClusterModules)
		{
			ArtifactHarvestModule.StatesInstance sMI = clusterModule.Get().GetSMI<ArtifactHarvestModule.StatesInstance>();
			if (sMI != null)
			{
				list.Add(sMI);
			}
		}
		return list;
	}

	public List<CargoBayCluster> GetAllCargoBays()
	{
		List<CargoBayCluster> list = new List<CargoBayCluster>();
		foreach (Ref<RocketModuleCluster> clusterModule in m_moduleInterface.ClusterModules)
		{
			CargoBayCluster component = clusterModule.Get().GetComponent<CargoBayCluster>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public List<CargoBayCluster> GetCargoBaysOfType(CargoBay.CargoType cargoType)
	{
		List<CargoBayCluster> list = new List<CargoBayCluster>();
		foreach (Ref<RocketModuleCluster> clusterModule in m_moduleInterface.ClusterModules)
		{
			CargoBayCluster component = clusterModule.Get().GetComponent<CargoBayCluster>();
			if (component != null && component.storageType == cargoType)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public void DestroyCraftAndModules()
	{
		List<RocketModuleCluster> list = m_moduleInterface.ClusterModules.Select((Ref<RocketModuleCluster> x) => x.Get()).ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			RocketModuleCluster rocketModuleCluster = list[num];
			Storage component = rocketModuleCluster.GetComponent<Storage>();
			if (component != null)
			{
				component.ConsumeAllIgnoringDisease();
			}
			MinionStorage component2 = rocketModuleCluster.GetComponent<MinionStorage>();
			if (component2 != null)
			{
				List<MinionStorage.Info> storedMinionInfo = component2.GetStoredMinionInfo();
				for (int num2 = storedMinionInfo.Count - 1; num2 >= 0; num2--)
				{
					component2.DeleteStoredMinion(storedMinionInfo[num2].id);
				}
			}
			Util.KDestroyGameObject(rocketModuleCluster.gameObject);
		}
		Util.KDestroyGameObject(base.gameObject);
	}

	public void CancelLaunch()
	{
		if (LaunchRequested)
		{
			Debug.Log("Cancelling launch!");
			LaunchRequested = false;
		}
	}

	public void RequestLaunch(bool automated = false)
	{
		if (!this.HasTag(GameTags.RocketNotOnGround) && !m_moduleInterface.GetClusterDestinationSelector().IsAtDestination())
		{
			if (DebugHandler.InstantBuildMode && !automated)
			{
				Launch();
			}
			if (!LaunchRequested && CheckPreppedForLaunch())
			{
				Debug.Log("Triggering launch!");
				LaunchRequested = true;
			}
		}
	}

	public void Launch(bool automated = false)
	{
		if (this.HasTag(GameTags.RocketNotOnGround) || m_moduleInterface.GetClusterDestinationSelector().IsAtDestination())
		{
			LaunchRequested = false;
		}
		else if ((DebugHandler.InstantBuildMode && !automated) || CheckReadyToLaunch())
		{
			if (automated && !m_moduleInterface.CheckReadyForAutomatedLaunchCommand())
			{
				LaunchRequested = false;
				return;
			}
			LaunchRequested = false;
			SetCraftStatus(CraftStatus.Launching);
			m_moduleInterface.DoLaunch();
			BurnFuelForTravel();
			m_clusterTraveler.AdvancePathOneStep();
			UpdateStatusItem();
		}
	}

	public void LandAtPad(LaunchPad pad)
	{
		m_moduleInterface.GetClusterDestinationSelector().SetDestinationPad(pad);
	}

	public PadLandingStatus CanLandAtPad(LaunchPad pad, out string failReason)
	{
		if (pad == null)
		{
			failReason = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.NONEAVAILABLE;
			return PadLandingStatus.CanNeverLand;
		}
		if (pad.HasRocket() && pad.LandedRocket.CraftInterface != m_moduleInterface)
		{
			failReason = "<TEMP>The pad already has a rocket on it!<TEMP>";
			return PadLandingStatus.CanLandEventually;
		}
		if (ConditionFlightPathIsClear.PadTopEdgeDistanceToCeilingEdge(pad.gameObject) < ModuleInterface.RocketHeight)
		{
			failReason = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_TOO_SHORT;
			return PadLandingStatus.CanNeverLand;
		}
		int obstruction = -1;
		if (!ConditionFlightPathIsClear.CheckFlightPathClear(ModuleInterface, pad.gameObject, out obstruction))
		{
			failReason = string.Format(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_PATH_OBSTRUCTED, pad.GetProperName());
			return PadLandingStatus.CanNeverLand;
		}
		if (!pad.GetComponent<Operational>().IsOperational)
		{
			failReason = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_PAD_DISABLED;
			return PadLandingStatus.CanNeverLand;
		}
		int rocketBottomPosition = pad.RocketBottomPosition;
		foreach (Ref<RocketModuleCluster> clusterModule in ModuleInterface.ClusterModules)
		{
			GameObject gameObject = clusterModule.Get().gameObject;
			int moduleRelativeVerticalPosition = ModuleInterface.GetModuleRelativeVerticalPosition(gameObject);
			Building component = gameObject.GetComponent<Building>();
			BuildingUnderConstruction component2 = gameObject.GetComponent<BuildingUnderConstruction>();
			BuildingDef buildingDef = ((component != null) ? component.Def : component2.Def);
			for (int i = 0; i < buildingDef.WidthInCells; i++)
			{
				for (int j = 0; j < buildingDef.HeightInCells; j++)
				{
					int cell = Grid.OffsetCell(rocketBottomPosition, 0, moduleRelativeVerticalPosition);
					cell = Grid.OffsetCell(cell, -(buildingDef.WidthInCells / 2) + i, j);
					if (Grid.Solid[cell])
					{
						obstruction = cell;
						failReason = string.Format(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_SITE_OBSTRUCTED, pad.GetProperName());
						return PadLandingStatus.CanNeverLand;
					}
				}
			}
		}
		failReason = null;
		return PadLandingStatus.CanLandImmediately;
	}

	private LaunchPad FindValidLandingPad(AxialI location, bool mustLandImmediately)
	{
		LaunchPad result = null;
		int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(location);
		LaunchPad preferredLaunchPadForWorld = m_moduleInterface.GetPreferredLaunchPadForWorld(asteroidWorldIdAtLocation);
		if (preferredLaunchPadForWorld != null && CanLandAtPad(preferredLaunchPadForWorld, out var _) == PadLandingStatus.CanLandImmediately)
		{
			return preferredLaunchPadForWorld;
		}
		foreach (LaunchPad launchPad in Components.LaunchPads)
		{
			if (launchPad.GetMyWorldLocation() == location)
			{
				string failReason2;
				PadLandingStatus padLandingStatus = CanLandAtPad(launchPad, out failReason2);
				if (padLandingStatus == PadLandingStatus.CanLandImmediately)
				{
					return launchPad;
				}
				if (!mustLandImmediately && padLandingStatus == PadLandingStatus.CanLandEventually)
				{
					result = launchPad;
				}
			}
		}
		return result;
	}

	public bool CanLandAtAsteroid(AxialI location, bool mustLandImmediately)
	{
		LaunchPad destinationPad = m_moduleInterface.GetClusterDestinationSelector().GetDestinationPad();
		Debug.Assert(destinationPad == null || destinationPad.GetMyWorldLocation() == location, "A rocket is trying to travel to an asteroid but has selected a landing pad at a different asteroid!");
		if (destinationPad != null)
		{
			string failReason;
			PadLandingStatus padLandingStatus = CanLandAtPad(destinationPad, out failReason);
			if (padLandingStatus != 0)
			{
				if (!mustLandImmediately)
				{
					return padLandingStatus == PadLandingStatus.CanLandEventually;
				}
				return false;
			}
			return true;
		}
		return FindValidLandingPad(location, mustLandImmediately) != null;
	}

	private void Land(LaunchPad pad, bool forceGrounded)
	{
		if (CanLandAtPad(pad, out var _) == PadLandingStatus.CanLandImmediately)
		{
			BurnFuelForTravel();
			m_location = pad.GetMyWorldLocation();
			SetCraftStatus((!forceGrounded) ? CraftStatus.Landing : CraftStatus.Grounded);
			m_moduleInterface.DoLand(pad);
			UpdateStatusItem();
		}
	}

	private void Land(AxialI destination, LaunchPad chosenPad)
	{
		if (chosenPad == null)
		{
			chosenPad = FindValidLandingPad(destination, mustLandImmediately: true);
		}
		Debug.Assert(chosenPad == null || chosenPad.GetMyWorldLocation() == m_location, "Attempting to land on a pad that isn't at our current position");
		Land(chosenPad, forceGrounded: false);
	}

	public void UpdateStatusItem()
	{
		if (ClusterGrid.Instance == null)
		{
			return;
		}
		KSelectable component = GetComponent<KSelectable>();
		if (mainStatusHandle != Guid.Empty)
		{
			component.RemoveStatusItem(mainStatusHandle);
		}
		ClusterGridEntity visibleEntityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(m_location, EntityLayer.Asteroid);
		ClusterGridEntity orbitAsteroid = GetOrbitAsteroid();
		bool flag = false;
		if (orbitAsteroid != null)
		{
			foreach (LaunchPad launchPad in Components.LaunchPads)
			{
				if (launchPad.GetMyWorldLocation() == orbitAsteroid.Location)
				{
					flag = true;
					break;
				}
			}
		}
		bool set = false;
		if (visibleEntityOfLayerAtCell != null)
		{
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InFlight, m_clusterTraveler);
		}
		else if (!HasResourcesToMove() && !flag)
		{
			set = true;
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.RocketStranded, orbitAsteroid);
		}
		else if (!m_moduleInterface.GetClusterDestinationSelector().IsAtDestination() && !CheckDesinationInRange())
		{
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.DestinationOutOfRange, m_clusterTraveler);
		}
		else if (IsFlightInProgress() || Status == CraftStatus.Launching)
		{
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InFlight, m_clusterTraveler);
		}
		else if (orbitAsteroid != null)
		{
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InOrbit, orbitAsteroid);
		}
		else
		{
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal);
		}
		GetComponent<KPrefabID>().SetTag(GameTags.RocketStranded, set);
		float num = 0f;
		float num2 = 0f;
		foreach (CargoBayCluster allCargoBay in GetAllCargoBays())
		{
			num += allCargoBay.MaxCapacity;
			num2 += allCargoBay.RemainingCapacity;
		}
		if (Status != 0 && num > 0f)
		{
			if (num2 == 0f)
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.FlightAllCargoFull);
				component.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining);
				return;
			}
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightAllCargoFull);
			if (cargoStatusHandle == Guid.Empty)
			{
				cargoStatusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, num2);
				return;
			}
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, immediate: true);
			cargoStatusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, num2);
		}
		else
		{
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining);
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightAllCargoFull);
		}
	}

	private void UpdateGroundTags()
	{
		foreach (Ref<RocketModuleCluster> clusterModule in ModuleInterface.ClusterModules)
		{
			if (clusterModule != null && !(clusterModule.Get() == null))
			{
				UpdateGroundTags(clusterModule.Get().gameObject);
			}
		}
		UpdateGroundTags(base.gameObject);
	}

	private void UpdateGroundTags(GameObject go)
	{
		SetTagOnGameObject(go, GameTags.RocketOnGround, status == CraftStatus.Grounded);
		SetTagOnGameObject(go, GameTags.RocketNotOnGround, status != CraftStatus.Grounded);
		SetTagOnGameObject(go, GameTags.RocketInSpace, status == CraftStatus.InFlight);
		SetTagOnGameObject(go, GameTags.EntityInSpace, status == CraftStatus.InFlight);
	}

	private void SetTagOnGameObject(GameObject go, Tag tag, bool set)
	{
		if (set)
		{
			go.AddTag(tag);
		}
		else
		{
			go.RemoveTag(tag);
		}
	}

	public override bool ShowName()
	{
		return status != CraftStatus.Grounded;
	}

	public override bool ShowPath()
	{
		return status != CraftStatus.Grounded;
	}

	public bool IsTravellingAndFueled()
	{
		if (HasResourcesToMove())
		{
			return m_clusterTraveler.IsTraveling();
		}
		return false;
	}

	public override bool ShowProgressBar()
	{
		return IsTravellingAndFueled();
	}

	public override float GetProgress()
	{
		return m_clusterTraveler.GetMoveProgress();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (Status == CraftStatus.Grounded || !SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 27))
		{
			return;
		}
		UIScheduler.Instance.ScheduleNextFrame("Check Fuel Costs", delegate
		{
			foreach (Ref<RocketModuleCluster> clusterModule in ModuleInterface.ClusterModules)
			{
				RocketModuleCluster rocketModuleCluster = clusterModule.Get();
				IFuelTank component = rocketModuleCluster.GetComponent<IFuelTank>();
				if (component != null && !component.Storage.IsEmpty())
				{
					component.DEBUG_FillTank();
				}
				OxidizerTank component2 = rocketModuleCluster.GetComponent<OxidizerTank>();
				if (component2 != null)
				{
					Dictionary<Tag, float> oxidizersAvailable = component2.GetOxidizersAvailable();
					if (oxidizersAvailable.Count > 0)
					{
						foreach (KeyValuePair<Tag, float> item in oxidizersAvailable)
						{
							if (item.Value > 0f)
							{
								component2.DEBUG_FillTank(ElementLoader.GetElementID(item.Key));
								break;
							}
						}
					}
				}
			}
		});
	}

	public float GetRange()
	{
		return ModuleInterface.Range;
	}
}
