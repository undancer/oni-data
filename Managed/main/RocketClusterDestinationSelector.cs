using System.Collections.Generic;
using KSerialization;

public class RocketClusterDestinationSelector : ClusterDestinationSelector
{
	[Serialize]
	private Ref<LaunchPad> m_launchPad = new Ref<LaunchPad>();

	[Serialize]
	private bool m_repeat;

	[Serialize]
	private AxialI m_prevDestination;

	[Serialize]
	private Ref<LaunchPad> m_prevLaunchPad = new Ref<LaunchPad>();

	private EventSystem.IntraObjectHandler<RocketClusterDestinationSelector> OnLaunchDelegate = new EventSystem.IntraObjectHandler<RocketClusterDestinationSelector>(delegate(RocketClusterDestinationSelector cmp, object data)
	{
		cmp.OnLaunch(data);
	});

	private EventSystem.IntraObjectHandler<RocketClusterDestinationSelector> OnClusterLocationChangedDelegate = new EventSystem.IntraObjectHandler<RocketClusterDestinationSelector>(delegate(RocketClusterDestinationSelector cmp, object data)
	{
		cmp.OnClusterLocationChanged(data);
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
		Subscribe(-1298331547, OnClusterLocationChangedDelegate);
	}

	public List<LaunchPad> GetLaunchPadsForDestination()
	{
		List<LaunchPad> list = new List<LaunchPad>();
		foreach (LaunchPad launchPad in Components.LaunchPads)
		{
			if (launchPad.GetMyWorldLocation() == m_destination)
			{
				list.Add(launchPad);
			}
		}
		return list;
	}

	public LaunchPad GetDestinationPad()
	{
		return m_launchPad.Get();
	}

	public override void SetDestination(AxialI location)
	{
		m_launchPad.Set(null);
		base.SetDestination(location);
	}

	public void SetDestinationPad(LaunchPad pad)
	{
		Debug.Assert(pad == null || ClusterGrid.Instance.IsInRange(pad.GetMyWorldLocation(), m_destination), "Tried sending a rocket to a launchpad that wasn't its destination world.");
		m_launchPad.Set(pad);
		if (pad != null)
		{
			base.SetDestination(pad.GetMyWorldLocation());
		}
		GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.ClusterDestinationChanged, null);
	}

	private void OnClusterLocationChanged(object data)
	{
		ClusterLocationChangedEvent clusterLocationChangedEvent = (ClusterLocationChangedEvent)data;
		if (clusterLocationChangedEvent.newLocation == m_destination)
		{
			GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.ClusterDestinationReached, null);
			if (m_repeat)
			{
				m_launchPad.Set(m_prevLaunchPad.Get());
				m_destination = m_prevDestination;
				m_prevDestination = clusterLocationChangedEvent.newLocation;
				CraftModuleInterface component = GetComponent<CraftModuleInterface>();
				m_prevLaunchPad.Set(component.CurrentPad);
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
