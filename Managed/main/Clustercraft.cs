using System;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Clustercraft : ClusterGridEntity
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

	[Serialize]
	[Range(0f, 1f)]
	public float AutoPilotMultiplier = 1f;

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

	public override EntityLayer Layer => EntityLayer.Craft;

	public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
	{
		new AnimConfig
		{
			animFile = Assets.GetAnim("rocket01_kanim"),
			initialAnim = "idle_loop"
		}
	};

	public override bool IsVisible => status == CraftStatus.InFlight;

	public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Hidden;

	public CraftModuleInterface ModuleInterface => m_moduleInterface;

	public AxialI Destination => m_moduleInterface.GetClusterDestinationSelector().GetDestination();

	public float Speed => EnginePower / TotalBurden * AutoPilotMultiplier;

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
		return Assets.GetSprite("ic_rocket");
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

	public void Init(AxialI location, LaunchPad pad)
	{
		m_location = location;
		RocketClusterDestinationSelector component = GetComponent<RocketClusterDestinationSelector>();
		component.SetDestination(m_location);
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
		return this.HasTag(GameTags.RocketInSpace) && (tryingToLand || HasResourcesToMove());
	}

	private bool CanTravelToCell(AxialI location)
	{
		ClusterGridEntity visibleEntityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(location, EntityLayer.Asteroid);
		if (visibleEntityOfLayerAtCell != null)
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
		return Status == CraftStatus.InFlight && m_clusterTraveler.IsTraveling();
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

	private bool CheckDesinationInRange()
	{
		return m_clusterTraveler.CurrentPath != null && Speed * m_clusterTraveler.TravelETA() <= ModuleInterface.Range;
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
			RocketModuleCluster rocketModuleCluster = clusterModule.Get();
			RocketEngineCluster component = rocketModuleCluster.GetComponent<RocketEngineCluster>();
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
					float num = RocketStats.oxidizerEfficiencies[item.Key];
					float a = fuelEquivalentKGs / num;
					float num2 = Mathf.Min(a, item.Value);
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
		foreach (RocketModuleCluster item in list)
		{
			Storage component = item.GetComponent<Storage>();
			if (component != null)
			{
				component.ConsumeAllIgnoringDisease();
			}
			MinionStorage component2 = item.GetComponent<MinionStorage>();
			if (component2 != null)
			{
				List<MinionStorage.Info> storedMinionInfo = component2.GetStoredMinionInfo();
				for (int num = storedMinionInfo.Count - 1; num >= 0; num--)
				{
					component2.DeleteStoredMinion(storedMinionInfo[num].id);
				}
			}
			Util.KDestroyGameObject(item.gameObject);
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
		int num = ConditionFlightPathIsClear.PadTopEdgeDistanceToCeilingEdge(pad.gameObject);
		if (num < ModuleInterface.RocketHeight)
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
		int rocketBottomPosition = pad.RocketBottomPosition;
		foreach (Ref<RocketModuleCluster> clusterModule in ModuleInterface.ClusterModules)
		{
			RocketModuleCluster rocketModuleCluster = clusterModule.Get();
			GameObject gameObject = rocketModuleCluster.gameObject;
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
		foreach (LaunchPad launchPad in Components.LaunchPads)
		{
			if (launchPad.GetMyWorldLocation() == location)
			{
				string failReason;
				PadLandingStatus padLandingStatus = CanLandAtPad(launchPad, out failReason);
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

	private bool CanLandAtAsteroid(AxialI location, bool mustLandImmediately)
	{
		LaunchPad destinationPad = m_moduleInterface.GetClusterDestinationSelector().GetDestinationPad();
		Debug.Assert(destinationPad == null || destinationPad.GetMyWorldLocation() == location, "A rocket is trying to travel to an asteroid but has selected a landing pad at a different asteroid!");
		if (destinationPad != null)
		{
			string failReason;
			PadLandingStatus padLandingStatus = CanLandAtPad(destinationPad, out failReason);
			return padLandingStatus == PadLandingStatus.CanLandImmediately || (!mustLandImmediately && padLandingStatus == PadLandingStatus.CanLandEventually);
		}
		return FindValidLandingPad(location, mustLandImmediately) != null;
	}

	private void Land(LaunchPad pad, bool forceGrounded)
	{
		if (CanLandAtPad(pad, out var _) == PadLandingStatus.CanLandImmediately)
		{
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
		if (visibleEntityOfLayerAtCell != null)
		{
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InFlight, m_clusterTraveler);
		}
		else if (!HasResourcesToMove() && !flag)
		{
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
		return true;
	}

	public override bool ShowProgressBar()
	{
		return HasResourcesToMove() && m_clusterTraveler.IsTraveling();
	}

	public override float GetProgress()
	{
		return m_clusterTraveler.GetMoveProgress();
	}
}
