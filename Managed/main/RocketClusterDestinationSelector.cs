using System.Collections.Generic;
using KSerialization;

public class RocketClusterDestinationSelector : ClusterDestinationSelector
{
	[Serialize]
	private Dictionary<int, Ref<LaunchPad>> m_launchPad = new Dictionary<int, Ref<LaunchPad>>();

	[Serialize]
	private bool m_repeat;

	[Serialize]
	private AxialI m_prevDestination;

	[Serialize]
	private Ref<LaunchPad> m_prevLaunchPad = new Ref<LaunchPad>();

	[Serialize]
	private bool isHarvesting;

	private EventSystem.IntraObjectHandler<RocketClusterDestinationSelector> OnLaunchDelegate = new EventSystem.IntraObjectHandler<RocketClusterDestinationSelector>(delegate(RocketClusterDestinationSelector cmp, object data)
	{
		cmp.OnLaunch(data);
	});

	public bool Repeat
	{
		get
		{
			return m_repeat;
		}
		set
		{
			m_repeat = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1277991738, OnLaunchDelegate);
	}

	protected override void OnSpawn()
	{
		if (isHarvesting)
		{
			WaitForPOIHarvest();
		}
	}

	public LaunchPad GetDestinationPad(AxialI destination)
	{
		int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(destination);
		if (m_launchPad.ContainsKey(asteroidWorldIdAtLocation))
		{
			return m_launchPad[asteroidWorldIdAtLocation].Get();
		}
		return null;
	}

	public LaunchPad GetDestinationPad()
	{
		return GetDestinationPad(m_destination);
	}

	public override void SetDestination(AxialI location)
	{
		base.SetDestination(location);
	}

	public void SetDestinationPad(LaunchPad pad)
	{
		Debug.Assert(pad == null || ClusterGrid.Instance.IsInRange(pad.GetMyWorldLocation(), m_destination), "Tried sending a rocket to a launchpad that wasn't its destination world.");
		if (pad != null)
		{
			AddDestinationPad(pad.GetMyWorldLocation(), pad);
			base.SetDestination(pad.GetMyWorldLocation());
		}
		GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.ClusterDestinationChanged, null);
	}

	private void AddDestinationPad(AxialI location, LaunchPad pad)
	{
		int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(location);
		if (asteroidWorldIdAtLocation >= 0)
		{
			if (!m_launchPad.ContainsKey(asteroidWorldIdAtLocation))
			{
				m_launchPad.Add(asteroidWorldIdAtLocation, new Ref<LaunchPad>());
			}
			m_launchPad[asteroidWorldIdAtLocation].Set(pad);
		}
	}

	protected override void OnClusterLocationChanged(object data)
	{
		ClusterLocationChangedEvent clusterLocationChangedEvent = (ClusterLocationChangedEvent)data;
		if (!(clusterLocationChangedEvent.newLocation == m_destination))
		{
			return;
		}
		GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.ClusterDestinationReached, null);
		if (m_repeat)
		{
			if (ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(clusterLocationChangedEvent.newLocation, EntityLayer.POI) != null && CanRocketHarvest())
			{
				WaitForPOIHarvest();
			}
			else
			{
				SetUpReturnTrip();
			}
		}
	}

	private void SetUpReturnTrip()
	{
		AddDestinationPad(m_prevDestination, m_prevLaunchPad.Get());
		m_destination = m_prevDestination;
		m_prevDestination = GetComponent<Clustercraft>().Location;
		m_prevLaunchPad.Set(GetComponent<CraftModuleInterface>().CurrentPad);
	}

	private bool CanRocketHarvest()
	{
		bool flag = false;
		List<ResourceHarvestModule.StatesInstance> allResourceHarvestModules = GetComponent<Clustercraft>().GetAllResourceHarvestModules();
		if (allResourceHarvestModules.Count > 0)
		{
			foreach (ResourceHarvestModule.StatesInstance item in allResourceHarvestModules)
			{
				if (item.CheckIfCanHarvest())
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			List<ArtifactHarvestModule.StatesInstance> allArtifactHarvestModules = GetComponent<Clustercraft>().GetAllArtifactHarvestModules();
			if (allArtifactHarvestModules.Count > 0)
			{
				foreach (ArtifactHarvestModule.StatesInstance item2 in allArtifactHarvestModules)
				{
					if (item2.CheckIfCanHarvest())
					{
						flag = true;
					}
				}
				return flag;
			}
		}
		return flag;
	}

	private void OnStorageChange(object data)
	{
		if (CanRocketHarvest())
		{
			return;
		}
		isHarvesting = false;
		foreach (Ref<RocketModuleCluster> clusterModule in GetComponent<Clustercraft>().ModuleInterface.ClusterModules)
		{
			if ((bool)clusterModule.Get().GetComponent<Storage>())
			{
				Unsubscribe(clusterModule.Get().gameObject, -1697596308, OnStorageChange);
			}
		}
		SetUpReturnTrip();
	}

	private void WaitForPOIHarvest()
	{
		isHarvesting = true;
		foreach (Ref<RocketModuleCluster> clusterModule in GetComponent<Clustercraft>().ModuleInterface.ClusterModules)
		{
			if ((bool)clusterModule.Get().GetComponent<Storage>())
			{
				Subscribe(clusterModule.Get().gameObject, -1697596308, OnStorageChange);
			}
		}
	}

	private void OnLaunch(object data)
	{
		CraftModuleInterface component = GetComponent<CraftModuleInterface>();
		m_prevLaunchPad.Set(component.CurrentPad);
		Clustercraft component2 = GetComponent<Clustercraft>();
		m_prevDestination = component2.Location;
	}
}
