using UnityEngine;

public class NuclearWasteConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.NuclearWaste;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLiquidOreEntity(ElementID);
		gameObject.AddOrGetDef<NuclearWaste.Def>();
		return gameObject;
	}
}
