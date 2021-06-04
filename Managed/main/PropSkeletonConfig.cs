using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropSkeletonConfig : IEntityConfig
{
	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropSkeleton", STRINGS.BUILDINGS.PREFABS.PROPSKELETON.NAME, STRINGS.BUILDINGS.PREFABS.PROPSKELETON.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER5, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("skeleton_poi_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 1, height: 2, element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Gravitas
		});
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Creature);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<Demolishable>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
