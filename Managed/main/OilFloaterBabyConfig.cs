using STRINGS;
using UnityEngine;

public class OilFloaterBabyConfig : IEntityConfig
{
	public const string ID = "OilfloaterBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = OilFloaterConfig.CreateOilFloater("OilfloaterBaby", CREATURES.SPECIES.OILFLOATER.BABY.NAME, CREATURES.SPECIES.OILFLOATER.BABY.DESC, "baby_oilfloater_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Oilfloater");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
