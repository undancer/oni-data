using STRINGS;
using UnityEngine;

public class BabySquirrelConfig : IEntityConfig
{
	public const string ID = "SquirrelBaby";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = SquirrelConfig.CreateSquirrel("SquirrelBaby", CREATURES.SPECIES.SQUIRREL.BABY.NAME, CREATURES.SPECIES.SQUIRREL.BABY.DESC, "baby_squirrel_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Squirrel");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
