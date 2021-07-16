using System.Collections.Generic;
using STRINGS;

public class RailgunPayloadClusterGridEntity : ClusterGridEntity
{
	[MyCmpReq]
	private ClusterDestinationSelector m_destionationSelector;

	[MyCmpReq]
	private KSelectable m_selectable;

	[MyCmpReq]
	private ClusterTraveler m_clusterTraveler;

	public bool NoWaitInOrbit;

	public override string Name => ITEMS.RAILGUNPAYLOAD.NAME;

	public override EntityLayer Layer => EntityLayer.Payload;

	public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
	{
		new AnimConfig
		{
			animFile = Assets.GetAnim("payload01_kanim"),
			initialAnim = "idle_loop"
		}
	};

	public override bool IsVisible => m_clusterTraveler.IsTraveling();

	public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Hidden;

	public override bool SpaceOutInSameHex()
	{
		return true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		m_clusterTraveler.getSpeedCB = GetSpeed;
		m_clusterTraveler.getCanTravelCB = CanTravel;
		m_clusterTraveler.onTravelCB = null;
	}

	private float GetSpeed()
	{
		return 10f;
	}

	private bool CanTravel(bool tryingToLand)
	{
		return this.HasTag(GameTags.EntityInSpace);
	}

	public void Configure(AxialI source, AxialI destination)
	{
		m_location = source;
		m_destionationSelector.SetDestination(destination);
	}

	public override bool ShowPath()
	{
		return m_selectable.IsSelected;
	}

	public override bool ShowProgressBar()
	{
		if (m_selectable.IsSelected)
		{
			return m_clusterTraveler.IsTraveling();
		}
		return false;
	}

	public override float GetProgress()
	{
		return m_clusterTraveler.GetMoveProgress();
	}
}
