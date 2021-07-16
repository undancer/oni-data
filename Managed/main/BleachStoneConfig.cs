using UnityEngine;

public class BleachStoneConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.BleachStone;

	public SimHashes SublimeElementID => SimHashes.ChlorineGas;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(ElementID);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.BleachStoneEmissionBubbles;
		sublimates.info = new Sublimates.Info(0.00020000001f, 0.0025000002f, 1.8f, 0.5f, SublimeElementID);
		return gameObject;
	}
}
