using STRINGS;
using TUNING;
using UnityEngine;

public class PropLightConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropLight", STRINGS.BUILDINGS.PREFABS.PROPLIGHT.NAME, STRINGS.BUILDINGS.PREFABS.PROPLIGHT.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("setpiece_light_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 1, height: 1);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Steel);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
