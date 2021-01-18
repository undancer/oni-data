using UnityEngine;

public class ToxicSandConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.ToxicSand;

	public SimHashes SublimeElementID => SimHashes.ContaminatedOxygen;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(ElementID);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
		sublimates.info = new Sublimates.Info(0.00020000001f, 0.0025000002f, 1.8f, 0.5f, SublimeElementID);
		return gameObject;
	}
}
