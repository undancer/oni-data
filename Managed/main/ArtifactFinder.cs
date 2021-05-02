using System.Collections.Generic;
using Database;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/ArtifactFinder")]
public class ArtifactFinder : KMonoBehaviour
{
	public const string ID = "ArtifactFinder";

	[MyCmpReq]
	private MinionStorage minionStorage;

	private static readonly EventSystem.IntraObjectHandler<ArtifactFinder> OnLandDelegate = new EventSystem.IntraObjectHandler<ArtifactFinder>(delegate(ArtifactFinder component, object data)
	{
		component.OnLand(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-887025858, OnLandDelegate);
	}

	public ArtifactTier GetArtifactDropTier(StoredMinionIdentity minionID, SpaceDestination destination)
	{
		ArtifactDropRate artifactDropTable = destination.GetDestinationType().artifactDropTable;
		bool flag = minionID.traitIDs.Contains("Archaeologist");
		if (artifactDropTable != null)
		{
			float num = artifactDropTable.totalWeight;
			if (flag)
			{
				num -= artifactDropTable.GetTierWeight(DECOR.SPACEARTIFACT.TIER_NONE);
			}
			float num2 = Random.value * num;
			foreach (Tuple<ArtifactTier, float> rate in artifactDropTable.rates)
			{
				if (!flag || (flag && rate.first != DECOR.SPACEARTIFACT.TIER_NONE))
				{
					num2 -= rate.second;
				}
				if (num2 <= 0f)
				{
					return rate.first;
				}
			}
		}
		return DECOR.SPACEARTIFACT.TIER0;
	}

	public List<string> GetArtifactsOfTier(ArtifactTier tier)
	{
		List<string> list = new List<string>();
		foreach (string artifactItem in ArtifactConfig.artifactItems)
		{
			GameObject prefab = Assets.GetPrefab(artifactItem.ToTag());
			ArtifactTier artifactTier = prefab.GetComponent<SpaceArtifact>().GetArtifactTier();
			if (artifactTier == tier)
			{
				list.Add(artifactItem);
			}
		}
		return list;
	}

	public string SearchForArtifact(StoredMinionIdentity minionID, SpaceDestination destination)
	{
		ArtifactTier artifactDropTier = GetArtifactDropTier(minionID, destination);
		if (artifactDropTier == DECOR.SPACEARTIFACT.TIER_NONE)
		{
			return null;
		}
		List<string> artifactsOfTier = GetArtifactsOfTier(artifactDropTier);
		return artifactsOfTier[Random.Range(0, artifactsOfTier.Count)];
	}

	public void OnLand(object data)
	{
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(SpacecraftManager.instance.GetSpacecraftID(GetComponent<RocketModule>().conditionManager.GetComponent<ILaunchableRocket>()));
		foreach (MinionStorage.Info item in minionStorage.GetStoredMinionInfo())
		{
			StoredMinionIdentity minionID = item.serializedMinion.Get<StoredMinionIdentity>();
			string text = SearchForArtifact(minionID, spacecraftDestination);
			if (text != null)
			{
				GameObject prefab = Assets.GetPrefab(text.ToTag());
				GameObject gameObject = GameUtil.KInstantiate(prefab, base.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore);
				gameObject.SetActive(value: true);
			}
		}
	}
}
