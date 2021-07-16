using UnityEngine;

public class OxyRockConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.OxyRock;

	public SimHashes SublimeElementID => SimHashes.Oxygen;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(ElementID);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.OxygenEmissionBubbles;
		sublimates.info = new Sublimates.Info(0.010000001f, 0.0050000004f, 1.8f, 0.7f, SublimeElementID);
		return gameObject;
	}
}
