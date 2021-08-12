using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class BabyPacuTropicalConfig : IEntityConfig
{
	public const string ID = "PacuTropicalBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = PacuTropicalConfig.CreatePacu("PacuTropicalBaby", CREATURES.SPECIES.PACU.VARIANT_TROPICAL.BABY.NAME, CREATURES.SPECIES.PACU.VARIANT_TROPICAL.BABY.DESC, "baby_pacu_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "PacuTropical");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
