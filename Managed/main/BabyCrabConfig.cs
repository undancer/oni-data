using STRINGS;
using UnityEngine;

public class BabyCrabConfig : IEntityConfig
{
	public const string ID = "CrabBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CrabConfig.CreateCrab("CrabBaby", CREATURES.SPECIES.CRAB.BABY.NAME, CREATURES.SPECIES.CRAB.BABY.DESC, "baby_pincher_kanim", is_baby: true, "BabyCrabShell");
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Crab", "BabyCrabShell");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
