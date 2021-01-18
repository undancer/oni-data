using STRINGS;
using TUNING;
using UnityEngine;

public class PropSkeletonConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("PropSkeleton", STRINGS.BUILDINGS.PREFABS.PROPSKELETON.NAME, STRINGS.BUILDINGS.PREFABS.PROPSKELETON.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER5, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("skeleton_poi_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 1, height: 2);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Creature);
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
