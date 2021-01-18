using System.Collections.Generic;
using Klei;
using KSerialization;
using ProcGenGame;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ClusterPOIManager
{
	[Serialize]
	private List<Ref<ResearchDestination>> m_researchDestinations = new List<Ref<ResearchDestination>>();

	[Serialize]
	private Ref<TemporalTear> m_temporalTear = new Ref<TemporalTear>();

	private ClusterFogOfWarManager.Instance m_fowManager;

	private ClusterFogOfWarManager.Instance GetFOWManager()
	{
		if (m_fowManager == null)
		{
			m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
		}
		return m_fowManager;
	}

	public void PopulatePOIsFromWorldGen(Cluster clusterLayout)
	{
		foreach (KeyValuePair<ClusterLayoutSave.POIType, List<AxialI>> poiLocation in clusterLayout.poiLocations)
		{
			IEnumerable<AxialI> value = poiLocation.Value;
			foreach (AxialI item in value)
			{
				switch (poiLocation.Key)
				{
				case ClusterLayoutSave.POIType.TemporalTear:
				{
					GameObject gameObject2 = Util.KInstantiate(Assets.GetPrefab("TemporalTear"));
					TemporalTear component2 = gameObject2.GetComponent<TemporalTear>();
					component2.Init(item);
					m_temporalTear.Set(component2);
					gameObject2.SetActive(value: true);
					break;
				}
				case ClusterLayoutSave.POIType.ResearchDestination:
				{
					GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("ResearchDestination"));
					ResearchDestination component = gameObject.GetComponent<ResearchDestination>();
					component.Init(item);
					m_researchDestinations.Add(new Ref<ResearchDestination>(component));
					gameObject.SetActive(value: true);
					break;
				}
				}
			}
		}
	}

	public void RevealTemporalTear()
	{
		if (m_temporalTear.Get() == null)
		{
			Debug.LogWarning("This cluster has no temporal tear, but has the poi mechanism to reveal it");
			return;
		}
		AxialI location = m_temporalTear.Get().Location;
		GetFOWManager().RevealLocation(location, 1);
	}

	public bool IsTemporalTearRevealed()
	{
		if (m_temporalTear.Get() == null)
		{
			Debug.LogWarning("This cluster has no temporal tear, but has the poi mechanism to reveal it");
			return false;
		}
		return GetFOWManager().IsLocationRevealed(m_temporalTear.Get().Location);
	}

	public void OpenTemporalTear(int openerWorldId)
	{
		if (m_temporalTear.Get() == null)
		{
			Debug.LogWarning("This cluster has no temporal tear, but has the poi mechanism to open it");
		}
		else if (!m_temporalTear.Get().IsOpen())
		{
			m_temporalTear.Get().Open();
			GameplaySeasonManager.Instance sMI = ClusterManager.Instance.GetWorld(openerWorldId).GetSMI<GameplaySeasonManager.Instance>();
			sMI.StartNewSeason(Db.Get().GameplaySeasons.TemporalTearMeteorShowers);
		}
	}

	public bool HasTemporalTearConsumedCraft()
	{
		if (m_temporalTear.Get() == null)
		{
			return false;
		}
		return m_temporalTear.Get().HasConsumedCraft();
	}

	public bool IsTemporalTearOpen()
	{
		if (m_temporalTear.Get() == null)
		{
			return false;
		}
		return m_temporalTear.Get().IsOpen();
	}
}
