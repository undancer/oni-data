using STRINGS;
using UnityEngine;

public class BabyStaterpillarConfig : IEntityConfig
{
	public const string ID = "StaterpillarBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = StaterpillarConfig.CreateStaterpillar("StaterpillarBaby", CREATURES.SPECIES.STATERPILLAR.BABY.NAME, CREATURES.SPECIES.STATERPILLAR.BABY.DESC, "baby_caterpillar_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Staterpillar");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
