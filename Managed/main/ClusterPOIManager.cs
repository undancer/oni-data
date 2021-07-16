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
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			UIScheduler.Instance.ScheduleNextFrame("UpgradeOldSaves", delegate
			{
				UpgradeOldSaves();
			});
		}
	}

	public void RegisterTemporalTear(TemporalTear temporalTear)
	{
		m_temporalTear.Set(temporalTear);
	}

	public bool HasTemporalTear()
	{
		return m_temporalTear.Get() != null;
	}

	public TemporalTear GetTemporalTear()
	{
		return m_temporalTear.Get();
	}

	private void UpgradeOldSaves()
	{
		bool flag = false;
		foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in ClusterGrid.Instance.cellContents)
		{
			foreach (ClusterGridEntity item in cellContent.Value)
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
			ClusterManager.Instance.GetClusterPOIManager().SpawnSpacePOIsInLegacySave();
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
		string[] array;
		foreach (KeyValuePair<int[], string[]> item in dictionary)
		{
			int[] key = item.Key;
			string[] value = item.Value;
			int minRadius = Mathf.Min(key[0], ClusterGrid.Instance.numRings - 1);
			int maxRadius = Mathf.Min(key[1], ClusterGrid.Instance.numRings - 1);
			List<AxialI> rings = AxialUtil.GetRings(AxialI.ZERO, minRadius, maxRadius);
			List<AxialI> list2 = new List<AxialI>();
			foreach (AxialI item2 in rings)
			{
				_ = ClusterGrid.Instance;
				_ = ClusterGrid.Instance.cellContents;
				_ = ClusterGrid.Instance.cellContents[item2];
				if (ClusterGrid.Instance.cellContents[item2].Count == 0 && ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(item2, EntityLayer.Asteroid) == null)
				{
					list2.Add(item2);
				}
			}
			array = value;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(array[i]));
				AxialI axialI = list2[Random.Range(0, list2.Count - 1)];
				list2.Remove(axialI);
				list.Add(axialI);
				gameObject.GetComponent<ClusterGridEntity>().Location = axialI;
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
		int minRadius2 = Mathf.Min(2, ClusterGrid.Instance.numRings - 1);
		int maxRadius2 = Mathf.Min(11, ClusterGrid.Instance.numRings - 1);
		List<AxialI> rings2 = AxialUtil.GetRings(AxialI.ZERO, minRadius2, maxRadius2);
		List<AxialI> list3 = new List<AxialI>();
		foreach (AxialI item3 in rings2)
		{
			if (ClusterGrid.Instance.cellContents[item3].Count == 0 && ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(item3, EntityLayer.Asteroid) == null && !list.Contains(item3))
			{
				list3.Add(item3);
			}
		}
		array = array2;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject2 = Util.KInstantiate(Assets.GetPrefab(array[i]));
			AxialI axialI2 = list3[Random.Range(0, list3.Count - 1)];
			list3.Remove(axialI2);
			HarvestablePOIClusterGridEntity component = gameObject2.GetComponent<HarvestablePOIClusterGridEntity>();
			if (component != null)
			{
				component.Init(axialI2);
			}
			ArtifactPOIClusterGridEntity component2 = gameObject2.GetComponent<ArtifactPOIClusterGridEntity>();
			if (component2 != null)
			{
				component2.Init(axialI2);
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
			gameObject.GetComponent<ClusterGridEntity>().Location = poiPlacement.Key;
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
			ClusterManager.Instance.GetWorld(openerWorldId).GetSMI<GameplaySeasonManager.Instance>().StartNewSeason(Db.Get().GameplaySeasons.TemporalTearMeteorShowers);
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
