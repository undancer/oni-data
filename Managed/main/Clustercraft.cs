using System;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using UnityEngine;

public class Clustercraft : ClusterGridEntity, ISim200ms
{
	public enum CraftStatus
	{
		Grounded,
		Launching,
		InFlight
	}

	[Serialize]
	private string m_name;

	[Serialize]
	private AxialI m_location;

	[MyCmpReq]
	private CraftModuleInterface m_moduleInterface;

	private Guid mainStatusHandle;

	private List<AxialI> m_cachedPath;

	private AxialI m_cachedPathDestination;

	[Serialize]
	private float movePotential = 0f;

	[Serialize]
	[Range(0f, 1f)]
	public float AutoPilotMultiplier = 1f;

	[Serialize]
	private bool m_launchRequested;

	[Serialize]
	private CraftStatus status;

	private static EventSystem.IntraObjectHandler<Clustercraft> ClusterDestinationChangedHandler = new EventSystem.IntraObjectHandler<Clustercraft>(delegate(Clustercraft cmp, object data)
	{
		cmp.ClusterDestinationChanged(data);
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

	public override AxialI Location => m_location;

	public override bool IsVisible => status == CraftStatus.InFlight;

	public CraftModuleInterface ModuleInterface => m_moduleInterface;

	public AxialI Destination => m_moduleInterface.GetClusterDestinationSelector().GetDestination();

	public List<AxialI> CurrentPath
	{
		get
		{
			if (m_cachedPath == null || Destination != m_cachedPathDestination)
			{
				ClusterDestinationSelector clusterDestinationSelector = m_moduleInterface.GetClusterDestinationSelector();
				m_cachedPathDestination = Destination;
				m_cachedPath = ClusterGrid.Instance.GetPath(Location, m_cachedPathDestination, clusterDestinationSelector);
			}
			return m_cachedPath;
		}
	}

	public float MovePotential => movePotential;

	public float Speed => EnginePower / TotalBurden * AutoPilotMultiplier;

	public float EnginePower
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModule> module in m_moduleInterface.Modules)
			{
				num += module.Get().performanceStats.EnginePower;
			}
			return num;
		}
	}

	public float FuelPerDistance
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModule> module in m_moduleInterface.Modules)
			{
				num += module.Get().performanceStats.FuelKilogramPerDistance;
			}
			return num;
		}
	}

	public float TotalBurden
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModule> module in m_moduleInterface.Modules)
			{
				num += module.Get().performanceStats.Burden;
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
		}
	}

	public CraftStatus Status
	{
		get
		{
			return status;
		}
		private set
		{
			status = value;
		}
	}

	public void SetCraftStatus(CraftStatus craft_status)
	{
		status = craft_status;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Clustercrafts.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(543433792, ClusterDestinationChangedHandler);
		SetRocketName(m_name);
	}

	public void Init(AxialI location, LaunchPad pad)
	{
		m_location = location;
		RocketClusterDestinationSelector component = GetComponent<RocketClusterDestinationSelector>();
		component.SetDestination(m_location);
		SetRocketName(GameUtil.GenerateRandomRocketName());
		if (pad != null)
		{
			Land(pad);
		}
	}

	protected override void OnCleanUp()
	{
		Components.Clustercrafts.Remove(this);
		base.OnCleanUp();
	}

	private void ClusterDestinationChanged(object data)
	{
		if (Destination == Location)
		{
			movePotential = 0f;
		}
		UpdateStatusItem();
	}

	public void SetRocketName(string newName)
	{
		m_name = newName;
		base.name = "Clustercraft: " + newName;
		foreach (Ref<RocketModule> module in m_moduleInterface.Modules)
		{
			CharacterOverlay component = module.Get().GetComponent<CharacterOverlay>();
			if (component != null)
			{
				NameDisplayScreen.Instance.UpdateName(component.gameObject);
				break;
			}
		}
	}

	public bool CheckReadyToLaunch()
	{
		return m_moduleInterface.CheckReadyToLaunch();
	}

	public bool CheckAbleToFly()
	{
		return m_moduleInterface.CheckAbleToFly();
	}

	public bool IsFlightInProgress()
	{
		return Status == CraftStatus.InFlight && !m_moduleInterface.GetClusterDestinationSelector().IsAtDestination();
	}

	public ClusterGridEntity GetStableOrbitAsteroid()
	{
		if (status != CraftStatus.InFlight || IsFlightInProgress())
		{
			return null;
		}
		return ClusterGrid.Instance.GetVisibleAsteroidAtAdjacentCell(m_location);
	}

	public ClusterGridEntity GetOrbitAsteroid()
	{
		if (status != CraftStatus.InFlight)
		{
			return null;
		}
		return ClusterGrid.Instance.GetVisibleAsteroidAtAdjacentCell(m_location);
	}

	public float TravelETA()
	{
		if (!IsFlightInProgress())
		{
			return 0f;
		}
		return RemainingTravelDistance() / Speed;
	}

	public float RemainingTravelDistance()
	{
		int num = RemainingTravelNodes();
		return (float)num * 600f - movePotential;
	}

	public int RemainingTravelNodes()
	{
		int num = CurrentPath.Count;
		if (ClusterGrid.Instance.HasVisibleAsteroidAtCell(m_location))
		{
			num--;
		}
		if (m_moduleInterface.GetClusterDestinationSelector().HasAsteroidDestination())
		{
			num--;
		}
		return Mathf.Max(0, num);
	}

	public bool CheckDesinationInRange()
	{
		return Speed * TravelETA() <= ModuleInterface.Range;
	}

	public bool HasResourcesToMove(int hexes = 1)
	{
		return m_moduleInterface.BurnableMassRemaining / FuelPerDistance >= 600f * (float)hexes - 0.001f;
	}

	public void Sim200ms(float dt)
	{
		if (!IsFlightInProgress() || !this.HasTag(GameTags.RocketInSpace))
		{
			return;
		}
		AxialI location = m_location;
		if (CurrentPath != null && CurrentPath.Count > 0)
		{
			if (!m_moduleInterface.GetClusterDestinationSelector().HasAsteroidDestination() || CurrentPath.Count > 1)
			{
				float num = dt * Speed;
				if (HasResourcesToMove())
				{
					movePotential += num;
					if (movePotential >= 600f)
					{
						movePotential -= 600f;
						TryTravel(600f);
						m_location = CurrentPath[0];
						CurrentPath.RemoveAt(0);
						Debug.Assert(ClusterGrid.Instance.GetVisibleAsteroidAtCell(m_location) == null, $"Somehow this clustercraft pathed through an asteroid at {m_location}");
					}
				}
			}
			else
			{
				LaunchPad destinationPad = m_moduleInterface.GetClusterDestinationSelector().GetDestinationPad();
				Land(CurrentPath[0], destinationPad);
			}
		}
		if (location != m_location)
		{
			SendClusterLocationChangedEvent(location, m_location);
		}
		UpdateStatusItem();
	}

	private float TryTravel(float targetTravelAmount)
	{
		float num = targetTravelAmount;
		foreach (Ref<RocketModule> module in m_moduleInterface.Modules)
		{
			RocketModule rocketModule = module.Get();
			RocketEngine component = rocketModule.GetComponent<RocketEngine>();
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
			foreach (Ref<RocketModule> module2 in m_moduleInterface.Modules)
			{
				FuelTank component2 = module2.Get().GetComponent<FuelTank>();
				if (component2 != null)
				{
					num -= BurnFromTank(num, component, fuelTag, component2.storage, ref totalOxidizerRemaining);
				}
				if (num <= 0f)
				{
					break;
				}
			}
		}
		return targetTravelAmount - num;
	}

	private float BurnFromTank(float attemptTravelAmount, RocketEngine engine, Tag fuelTag, Storage storage, ref float totalOxidizerRemaining)
	{
		float b = attemptTravelAmount * engine.GetComponent<RocketModule>().performanceStats.FuelKilogramPerDistance;
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
		return b / engine.GetComponent<RocketModule>().performanceStats.FuelKilogramPerDistance;
	}

	private void BurnOxidizer(float amount)
	{
		foreach (Ref<RocketModule> module in m_moduleInterface.Modules)
		{
			OxidizerTank component = module.Get().GetComponent<OxidizerTank>();
			if (component != null)
			{
				foreach (KeyValuePair<Tag, float> item in component.GetOxidizersAvailable())
				{
					float num = Mathf.Min(amount, item.Value);
					if (num > 0f)
					{
						component.storage.ConsumeIgnoringDisease(item.Key, num);
						amount -= num;
					}
				}
			}
			if (amount <= 0f)
			{
				break;
			}
		}
		Debug.Assert(amount <= 0f, "Tried to burn more oxidizer than was available in rocket " + amount + ":" + Name);
	}

	private void SendClusterLocationChangedEvent(AxialI oldLocation, AxialI newLocation)
	{
		Debug.Assert(oldLocation != m_location);
		ClusterLocationChangedEvent clusterLocationChangedEvent = default(ClusterLocationChangedEvent);
		clusterLocationChangedEvent.entity = this;
		clusterLocationChangedEvent.oldLocation = oldLocation;
		clusterLocationChangedEvent.newLocation = newLocation;
		ClusterLocationChangedEvent clusterLocationChangedEvent2 = clusterLocationChangedEvent;
		Trigger(-1298331547, clusterLocationChangedEvent2);
		Game.Instance.Trigger(-1298331547, clusterLocationChangedEvent2);
	}

	public void DestroyCraftAndModules()
	{
		List<RocketModule> list = m_moduleInterface.Modules.Select((Ref<RocketModule> x) => x.Get()).ToList();
		foreach (RocketModule item in list)
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
			LaunchRequested = false;
			Trigger(191901966, this);
		}
	}

	public void RequestLaunch(bool automated = false)
	{
		if (!this.HasTag(GameTags.RocketNotOnGround) && !m_moduleInterface.GetClusterDestinationSelector().IsAtDestination() && ((DebugHandler.InstantBuildMode && !automated) || (CheckReadyToLaunch() && CheckAbleToFly())))
		{
			LaunchRequested = true;
			Trigger(191901966, this);
		}
	}

	public void Launch(bool automated = false)
	{
		LaunchRequested = false;
		if (!this.HasTag(GameTags.RocketNotOnGround) && !m_moduleInterface.GetClusterDestinationSelector().IsAtDestination() && ((DebugHandler.InstantBuildMode && !automated) || (CheckReadyToLaunch() && CheckAbleToFly())))
		{
			Status = CraftStatus.Launching;
			m_moduleInterface.DoLaunch();
			Debug.Assert(CurrentPath.Count > 0, "Cannot launch a rocket if it has no destination");
			AxialI location = m_location;
			m_location = CurrentPath[0];
			CurrentPath.RemoveAt(0);
			SendClusterLocationChangedEvent(location, m_location);
			UpdateStatusItem();
		}
	}

	public void LandAtPad(LaunchPad pad)
	{
		m_moduleInterface.GetClusterDestinationSelector().SetDestinationPad(pad);
	}

	private void Land(LaunchPad pad)
	{
		if (!(pad == null) && (!pad.HasRocket() || !(pad.LandedRocket.CraftInterface != m_moduleInterface)))
		{
			m_location = pad.GetMyWorldLocation();
			if (CurrentPath != null)
			{
				CurrentPath.Clear();
			}
			Status = CraftStatus.Grounded;
			m_moduleInterface.DoLand(pad);
			UpdateStatusItem();
		}
	}

	private void Land(AxialI destination, LaunchPad pad)
	{
		if (pad == null)
		{
			foreach (LaunchPad launchPad in Components.LaunchPads)
			{
				if (launchPad.GetMyWorldLocation() == destination && !launchPad.HasRocket())
				{
					pad = launchPad;
					break;
				}
			}
		}
		Debug.Assert(pad == null || pad.GetMyWorldLocation().IsAdjacent(m_location), "Attempting to land on a pad that isn't adjacent to our current position");
		Land(pad);
	}

	private void UpdateStatusItem()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (mainStatusHandle != Guid.Empty)
		{
			component.RemoveStatusItem(mainStatusHandle);
		}
		ClusterGridEntity stableOrbitAsteroid = GetStableOrbitAsteroid();
		bool flag = false;
		if (stableOrbitAsteroid != null)
		{
			foreach (LaunchPad launchPad in Components.LaunchPads)
			{
				if (launchPad.GetMyWorldLocation() == stableOrbitAsteroid.Location)
				{
					flag = true;
					break;
				}
			}
		}
		if (!HasResourcesToMove() && !flag)
		{
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.RocketStranded, stableOrbitAsteroid);
		}
		else if (!m_moduleInterface.GetClusterDestinationSelector().IsAtDestination() && !CheckDesinationInRange())
		{
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.DestinationOutOfRange, this);
		}
		else if (IsFlightInProgress())
		{
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InFlight, this);
		}
		else if (stableOrbitAsteroid != null)
		{
			mainStatusHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InOrbit, stableOrbitAsteroid);
		}
	}
}
