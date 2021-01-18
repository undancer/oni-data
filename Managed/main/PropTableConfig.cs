using STRINGS;
using TUNING;
using UnityEngine;

public class PropTableConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("PropTable", STRINGS.BUILDINGS.PREFABS.PROPTABLE.NAME, STRINGS.BUILDINGS.PREFABS.PROPTABLE.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("table_breakroom_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 1);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		return obj;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
