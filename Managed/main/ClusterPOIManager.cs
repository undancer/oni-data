using System.Collections.Generic;
using KSerialization;
using ProcGenGame;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ClusterPOIManager : KMonoBehaviour
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UIScheduler.Instance.ScheduleNextFrame("UpgradeOldSaves", delegate
		{
			UpgradeOldSaves();
		});
	}

	private void UpgradeOldSaves()
	{
		bool flag = false;
		foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in ClusterGrid.Instance.cellContents)
		{
			List<ClusterGridEntity> value = cellContent.Value;
			foreach (ClusterGridEntity item in value)
			{
				if ((bool)item.GetComponent<HarvestablePOIClusterGridEntity>() || (bool)item.GetComponent<ArtifactPOIClusterGridEntity>())
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (!flag)
		{
			ClusterPOIManager clusterPOIManager = ClusterManager.Instance.GetClusterPOIManager();
			clusterPOIManager.SpawnSpacePOIsInLegacySave();
		}
	}

	public void SpawnSpacePOIsInLegacySave()
	{
		Dictionary<int[], string[]> dictionary = new Dictionary<int[], string[]>();
		dictionary.Add(new int[2]
		{
			2,
			3
		}, new string[1]
		{
			"HarvestableSpacePOI_SandyOreField"
		});
		dictionary.Add(new int[2]
		{
			5,
			7
		}, new string[1]
		{
			"HarvestableSpacePOI_OrganicMassField"
		});
		dictionary.Add(new int[2]
		{
			8,
			11
		}, new string[5]
		{
			"HarvestableSpacePOI_GildedAsteroidField",
			"HarvestableSpacePOI_GlimmeringAsteroidField",
			"HarvestableSpacePOI_HeliumCloud",
			"HarvestableSpacePOI_OilyAsteroidField",
			"HarvestableSpacePOI_FrozenOreField"
		});
		dictionary.Add(new int[2]
		{
			10,
			11
		}, new string[2]
		{
			"HarvestableSpacePOI_RadioactiveGasCloud",
			"HarvestableSpacePOI_RadioactiveAsteroidField"
		});
		dictionary.Add(new int[2]
		{
			5,
			7
		}, new string[5]
		{
			"HarvestableSpacePOI_RockyAsteroidField",
			"HarvestableSpacePOI_InterstellarIceField",
			"HarvestableSpacePOI_InterstellarOcean",
			"HarvestableSpacePOI_SandyOreField",
			"HarvestableSpacePOI_SwampyOreField"
		});
		dictionary.Add(new int[2]
		{
			7,
			11
		}, new string[10]
		{
			"HarvestableSpacePOI_MetallicAsteroidField",
			"HarvestableSpacePOI_SatelliteField",
			"HarvestableSpacePOI_ChlorineCloud",
			"HarvestableSpacePOI_OxidizedAsteroidField",
			"HarvestableSpacePOI_OxygenRichAsteroidField",
			"HarvestableSpacePOI_GildedAsteroidField",
			"HarvestableSpacePOI_HeliumCloud",
			"HarvestableSpacePOI_OilyAsteroidField",
			"HarvestableSpacePOI_FrozenOreField",
			"HarvestableSpacePOI_RadioactiveAsteroidField"
		});
		List<AxialI> list = new List<AxialI>();
		foreach (KeyValuePair<int[], string[]> item in dictionary)
		{
			int[] key = item.Key;
			string[] value = item.Value;
			List<AxialI> rings = AxialUtil.GetRings(AxialI.ZERO, key[0], key[1]);
			List<AxialI> list2 = new List<AxialI>();
			foreach (AxialI item2 in rings)
			{
				if (ClusterGrid.Instance.cellContents[item2].Count == 0 && ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(item2, EntityLayer.Asteroid) == null)
				{
					list2.Add(item2);
				}
			}
			string[] array = value;
			foreach (string s in array)
			{
				GameObject prefab = Assets.GetPrefab(s);
				GameObject gameObject = Util.KInstantiate(prefab);
				AxialI axialI = list2[Random.Range(0, list2.Count - 1)];
				list2.Remove(axialI);
				list.Add(axialI);
				HarvestablePOIClusterGridEntity component = gameObject.GetComponent<HarvestablePOIClusterGridEntity>();
				if (component != null)
				{
					component.Init(axialI);
				}
				ArtifactPOIClusterGridEntity component2 = gameObject.GetComponent<ArtifactPOIClusterGridEntity>();
				if (component2 != null)
				{
					component2.Init(axialI);
				}
				gameObject.SetActive(value: true);
			}
		}
		string[] array2 = new string[6]
		{
			"ArtifactSpacePOI_GravitasSpaceStation1",
			"ArtifactSpacePOI_GravitasSpaceStation4",
			"ArtifactSpacePOI_GravitasSpaceStation5",
			"ArtifactSpacePOI_GravitasSpaceStation6",
			"ArtifactSpacePOI_GravitasSpaceStation8",
			"ArtifactSpacePOI_RussellsTeapot"
		};
		List<AxialI> rings2 = AxialUtil.GetRings(AxialI.ZERO, 2, 11);
		List<AxialI> list3 = new List<AxialI>();
		foreach (AxialI item3 in rings2)
		{
			if (ClusterGrid.Instance.cellContents[item3].Count == 0 && ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(item3, EntityLayer.Asteroid) == null && !list.Contains(item3))
			{
				list3.Add(item3);
			}
		}
		string[] array3 = array2;
		foreach (string s2 in array3)
		{
			GameObject gameObject2 = Util.KInstantiate(Assets.GetPrefab(s2));
			AxialI axialI2 = list3[Random.Range(0, list3.Count - 1)];
			list3.Remove(axialI2);
			HarvestablePOIClusterGridEntity component3 = gameObject2.GetComponent<HarvestablePOIClusterGridEntity>();
			if (component3 != null)
			{
				component3.Init(axialI2);
			}
			ArtifactPOIClusterGridEntity component4 = gameObject2.GetComponent<ArtifactPOIClusterGridEntity>();
			if (component4 != null)
			{
				component4.Init(axialI2);
			}
			gameObject2.SetActive(value: true);
		}
	}

	public void PopulatePOIsFromWorldGen(Cluster clusterLayout)
	{
		Debug.Log("PopulatePOIsFromWorldGen");
		foreach (KeyValuePair<AxialI, string> poiPlacement in clusterLayout.poiPlacements)
		{
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(poiPlacement.Value));
			HarvestablePOIClusterGridEntity component = gameObject.GetComponent<HarvestablePOIClusterGridEntity>();
			if (component != null)
			{
				component.Init(poiPlacement.Key);
			}
			ArtifactPOIClusterGridEntity component2 = gameObject.GetComponent<ArtifactPOIClusterGridEntity>();
			if (component2 != null)
			{
				component2.Init(poiPlacement.Key);
			}
			gameObject.SetActive(value: true);
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
