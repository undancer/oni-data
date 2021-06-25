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
		gameObject.AddOrGetDef<NuclearWaste.Def>();
		return gameObject;
	}
}
