using STRINGS;
using TUNING;
using UnityEngine;

public class PropTallPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("PropTallPlant", STRINGS.BUILDINGS.PREFABS.PROPFACILITYTALLPLANT.NAME, STRINGS.BUILDINGS.PREFABS.PROPFACILITYTALLPLANT.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("gravitas_tall_plant_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 1, height: 3);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Polypropylene);
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
