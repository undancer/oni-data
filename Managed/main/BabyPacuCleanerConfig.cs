using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class BabyPacuCleanerConfig : IEntityConfig
{
	public const string ID = "PacuCleanerBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = PacuCleanerConfig.CreatePacu("PacuCleanerBaby", CREATURES.SPECIES.PACU.VARIANT_CLEANER.BABY.NAME, CREATURES.SPECIES.PACU.VARIANT_CLEANER.BABY.DESC, "baby_pacu_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "PacuCleaner");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
