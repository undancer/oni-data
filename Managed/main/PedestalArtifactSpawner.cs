using KSerialization;
using UnityEngine;

public class PedestalArtifactSpawner : KMonoBehaviour
{
	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private SingleEntityReceptacle receptacle;

	[Serialize]
	private bool artifactSpawned = false;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!artifactSpawned)
		{
			string uniqueArtifactID = ArtifactSelector.Instance.GetUniqueArtifactID();
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(uniqueArtifactID), base.transform.position);
			gameObject.SetActive(value: true);
			storage.Store(gameObject);
			receptacle.ForceDeposit(gameObject);
			artifactSpawned = true;
		}
	}
}
