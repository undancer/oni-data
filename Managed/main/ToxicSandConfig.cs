using UnityEngine;

public class ToxicSandConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.ToxicSand;

	public SimHashes SublimeElementID => SimHashes.ContaminatedOxygen;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(ElementID);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
		sublimates.info = new Sublimates.Info(2.0000001E-05f, 0.05f, 1.8f, 0.5f, SublimeElementID);
		return gameObject;
	}
}
