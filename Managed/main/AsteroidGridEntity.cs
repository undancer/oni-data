using System.Collections.Generic;
using KSerialization;

public class AsteroidGridEntity : ClusterGridEntity
{
	public static string DEFAULT_ASTEROID_ICON_ANIM = "asteroid_sandstone_start_kanim";

	[MyCmpReq]
	private WorldContainer m_worldContainer;

	[Serialize]
	private string m_name;

	[Serialize]
	private string m_asteroidAnim;

	public override string Name => m_name;

	public override EntityLayer Layer => EntityLayer.Asteroid;

	public override List<AnimConfig> AnimConfigs
	{
		get
		{
			List<AnimConfig> list = new List<AnimConfig>();
			AnimConfig item = new AnimConfig
			{
				animFile = Assets.GetAnim(m_asteroidAnim.IsNullOrWhiteSpace() ? DEFAULT_ASTEROID_ICON_ANIM : m_asteroidAnim),
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

	public override bool IsVisible => true;

	public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Peeked;

	public override bool ShowName()
	{
		return true;
	}

	public void Init(string name, AxialI location, string asteroidTypeId)
	{
		m_name = name;
		m_location = location;
		m_asteroidAnim = asteroidTypeId;
	}

	protected override void OnSpawn()
	{
		Game.Instance.Subscribe(-1298331547, OnClusterLocationChanged);
		Game.Instance.Subscribe(-1991583975, OnFogOfWarRevealed);
		if (ClusterGrid.Instance.IsCellVisible(m_location))
		{
			SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(m_location, 1);
		}
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.Unsubscribe(-1298331547, OnClusterLocationChanged);
		Game.Instance.Unsubscribe(-1991583975, OnFogOfWarRevealed);
		base.OnCleanUp();
	}

	public void OnClusterLocationChanged(object data)
	{
		if (!m_worldContainer.IsDiscovered && ClusterGrid.Instance.IsCellVisible(base.Location))
		{
			Clustercraft component = ((ClusterLocationChangedEvent)data).entity.GetComponent<Clustercraft>();
			if (!(component == null) && component.GetOrbitAsteroid() == this)
			{
				m_worldContainer.SetDiscovered(reveal_surface: true);
			}
		}
	}

	public void OnFogOfWarRevealed(object data = null)
	{
		if (data == null || (AxialI)data != m_location || !ClusterGrid.Instance.IsCellVisible(base.Location))
		{
			return;
		}
		WorldDetectedMessage message = new WorldDetectedMessage(m_worldContainer);
		MusicManager.instance.PlaySong("Stinger_WorldDetected");
		Messenger.Instance.QueueMessage(message);
		if (m_worldContainer.IsDiscovered)
		{
			return;
		}
		foreach (Clustercraft clustercraft in Components.Clustercrafts)
		{
			if (clustercraft.GetOrbitAsteroid() == this)
			{
				m_worldContainer.SetDiscovered(reveal_surface: true);
				break;
			}
		}
	}
}
