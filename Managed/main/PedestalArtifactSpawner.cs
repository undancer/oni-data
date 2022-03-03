using KSerialization;
using UnityEngine;

public class PedestalArtifactSpawner : KMonoBehaviour
{
	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private SingleEntityReceptacle receptacle;

	[Serialize]
	private bool artifactSpawned;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (GameObject item in storage.items)
		{
			if (ArtifactSelector.Instance.GetArtifactType(item.name) == ArtifactType.Terrestrial)
			{
				item.GetComponent<KPrefabID>().AddTag(GameTags.TerrestrialArtifact, serialize: true);
			}
		}
		if (!artifactSpawned)
		{
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(ArtifactSelector.Instance.GetUniqueArtifactID(ArtifactType.Terrestrial)), base.transform.position);
			gameObject.SetActive(value: true);
			gameObject.GetComponent<KPrefabID>().AddTag(GameTags.TerrestrialArtifact, serialize: true);
			storage.Store(gameObject);
			receptacle.ForceDeposit(gameObject);
			artifactSpawned = true;
		}
	}
}
