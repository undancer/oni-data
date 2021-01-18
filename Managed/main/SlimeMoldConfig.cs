using UnityEngine;

public class SlimeMoldConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.SlimeMold;

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
		sublimates.info = new Sublimates.Info(0.025f, 0.125f, 1.8f, 0f, SublimeElementID);
		return gameObject;
	}
}
