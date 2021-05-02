using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class TemporalTear : ClusterGridEntity
{
	[Serialize]
	private bool m_open;

	[Serialize]
	private bool m_hasConsumedCraft;

	public override string Name => Db.Get().SpaceDestinationTypes.Wormhole.typeName;

	public override EntityLayer Layer => EntityLayer.POI;

	public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
	{
		new AnimConfig
		{
			animFile = Assets.GetAnim("temporal_tear_kanim"),
			initialAnim = "idle_loop"
		}
	};

	public override bool IsVisible => true;

	public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Peeked;

	public void Init(AxialI location)
	{
		base.Location = location;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.Subscribe(-1298331547, OnClusterLocationChanged);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.Unsubscribe(-1298331547, OnClusterLocationChanged);
		base.OnCleanUp();
	}

	public void OnClusterLocationChanged(object data)
	{
		Clustercraft clustercraft = ((ClusterLocationChangedEvent)data).entity as Clustercraft;
		Debug.Assert(clustercraft != null, $"ClusterLocationChanged sent for a non-Clustercraft object: {data}");
		if (m_open && clustercraft.Location == base.Location && !clustercraft.IsFlightInProgress())
		{
			clustercraft.DestroyCraftAndModules();
			m_hasConsumedCraft = true;
		}
	}

	public void Open()
	{
		m_open = true;
	}

	public bool IsOpen()
	{
		return m_open;
	}

	public bool HasConsumedCraft()
	{
		return m_hasConsumedCraft;
	}
}
