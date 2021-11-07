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
		if (!artifactSpawned)
		{
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(ArtifactSelector.Instance.GetUniqueArtifactID()), base.transform.position);
			gameObject.SetActive(value: true);
			gameObject.GetComponent<KPrefabID>().AddTag(GameTags.TerrestrialArtifact, serialize: true);
			storage.Store(gameObject);
			receptacle.ForceDeposit(gameObject);
			artifactSpawned = true;
		}
	}
}
