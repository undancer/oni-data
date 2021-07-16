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
			initialAnim = "closed_loop"
		}
	};

	public override bool IsVisible => true;

	public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Peeked;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ClusterManager.Instance.GetComponent<ClusterPOIManager>().RegisterTemporalTear(this);
		UpdateStatus();
	}

	public void UpdateStatus()
	{
		KSelectable component = GetComponent<KSelectable>();
		ClusterMapVisualizer clusterMapVisualizer = null;
		if (ClusterMapScreen.Instance != null)
		{
			clusterMapVisualizer = ClusterMapScreen.Instance.GetEntityVisAnim(this);
		}
		if (IsOpen())
		{
			if (clusterMapVisualizer != null)
			{
				clusterMapVisualizer.PlayAnim("open_loop", KAnim.PlayMode.Loop);
			}
			component.RemoveStatusItem(Db.Get().MiscStatusItems.TearClosed);
			component.AddStatusItem(Db.Get().MiscStatusItems.TearOpen);
		}
		else
		{
			if (clusterMapVisualizer != null)
			{
				clusterMapVisualizer.PlayAnim("closed_loop", KAnim.PlayMode.Loop);
			}
			component.RemoveStatusItem(Db.Get().MiscStatusItems.TearOpen);
			GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.TearClosed);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void ConsumeCraft(Clustercraft craft)
	{
		if (!m_open || !(craft.Location == base.Location) || craft.IsFlightInProgress())
		{
			return;
		}
		for (int i = 0; i < Components.MinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = Components.MinionIdentities[i];
			if (minionIdentity.GetMyWorldId() == craft.ModuleInterface.GetInteriorWorld().id)
			{
				Util.KDestroyGameObject(minionIdentity.gameObject);
			}
		}
		craft.DestroyCraftAndModules();
		m_hasConsumedCraft = true;
	}

	public void Open()
	{
		m_open = true;
		UpdateStatus();
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
