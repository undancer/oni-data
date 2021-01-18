using System;
using System.Collections.Generic;
using Database;
using KSerialization;

public class AsteroidGridEntity : ClusterGridEntity
{
	[MyCmpReq]
	private WorldContainer m_worldContainer;

	[Serialize]
	private string m_name;

	[Serialize]
	private string m_asteroidTypeId;

	[Serialize]
	private AxialI m_location;

	private Action<object> m_onClusterLocationChangedDelegate;

	private Action<object> m_onFogOfWarRevealedDelegate;

	public override string Name => m_name;

	public override EntityLayer Layer => EntityLayer.Asteroid;

	public override List<AnimConfig> AnimConfigs
	{
		get
		{
			AsteroidType typeOrDefault = Db.Get().AsteroidTypes.GetTypeOrDefault(m_asteroidTypeId);
			List<AnimConfig> list = new List<AnimConfig>();
			AnimConfig item = new AnimConfig
			{
				animFile = Assets.GetAnim(typeOrDefault.animName),
				initialAnim = "idle_loop"
			};
			list.Add(item);
			item = new AnimConfig
			{
				animFile = Assets.GetAnim("orbit_kanim"),
				initialAnim = "orbit"
			};
			list.Add(item);
			return list;
		}
	}

	public override AxialI Location => m_location;

	public override bool IsVisible => true;

	public void Init(string name, AxialI location, string asteroidTypeId)
	{
		m_name = name;
		m_location = location;
		m_asteroidTypeId = asteroidTypeId;
	}

	protected override void OnSpawn()
	{
		m_onClusterLocationChangedDelegate = OnClusterLocationChanged;
		m_onFogOfWarRevealedDelegate = OnFogOfWarRevealed;
		Game.Instance.Subscribe(-1298331547, m_onClusterLocationChangedDelegate);
		Game.Instance.Subscribe(-1991583975, m_onFogOfWarRevealedDelegate);
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.Unsubscribe(-1298331547, m_onClusterLocationChangedDelegate);
		Game.Instance.Unsubscribe(-1991583975, m_onFogOfWarRevealedDelegate);
		base.OnCleanUp();
	}

	public void OnClusterLocationChanged(object data)
	{
		if (!m_worldContainer.IsDiscovered && ClusterGrid.Instance.IsCellVisible(Location))
		{
			Clustercraft component = ((ClusterLocationChangedEvent)data).entity.GetComponent<Clustercraft>();
			if (!(component == null) && component.GetStableOrbitAsteroid() == this)
			{
				m_worldContainer.SetDiscovered(reveal_surface: true);
			}
		}
	}

	public void OnFogOfWarRevealed(object data = null)
	{
		if (m_worldContainer.IsDiscovered || !ClusterGrid.Instance.IsCellVisible(Location))
		{
			return;
		}
		foreach (Clustercraft clustercraft in Components.Clustercrafts)
		{
			if (clustercraft.GetStableOrbitAsteroid() == this)
			{
				m_worldContainer.SetDiscovered(reveal_surface: true);
				break;
			}
		}
		if (data != null && (AxialI)data == m_location)
		{
			WorldDetectedMessage message = new WorldDetectedMessage(m_worldContainer);
			MusicManager.instance.PlaySong("Stinger_ResearchComplete");
			Messenger.Instance.QueueMessage(message);
		}
	}
}
