using UnityEngine;

public class NuclearWasteConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.NuclearWaste;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLiquidOreEntity(ElementID);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.decayStorage = true;
		sublimates.spawnFXHash = SpawnFXHashes.NuclearWasteDrip;
		sublimates.info = new Sublimates.Info(0.066f, 6.6f, 1000f, 0f, ElementID);
		return gameObject;
	}
}
