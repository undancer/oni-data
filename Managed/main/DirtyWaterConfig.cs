using UnityEngine;

public class DirtyWaterConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.DirtyWater;

	public SimHashes SublimeElementID => SimHashes.ContaminatedOxygen;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLiquidOreEntity(ElementID);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
		sublimates.info = new Sublimates.Info(4.0000006E-05f, 0.025f, 1.8f, 1f, SublimeElementID);
		return gameObject;
	}
}
