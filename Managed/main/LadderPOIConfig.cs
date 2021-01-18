using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LadderPOIConfig : IEntityConfig
{
	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		int num = 1;
		int num2 = 1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropLadder", STRINGS.BUILDINGS.PREFABS.PROPLADDER.NAME, STRINGS.BUILDINGS.PREFABS.PROPLADDER.DESC, 50f, width: num, height: num2, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("ladder_poi_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Gravitas
		});
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Polypropylene);
		component.Temperature = 294.15f;
		Ladder ladder = gameObject.AddOrGet<Ladder>();
		ladder.upwardsMovementSpeedMultiplier = 1.5f;
		ladder.downwardsMovementSpeedMultiplier = 1.5f;
		gameObject.AddOrGet<AnimTileable>();
		OccupyArea obj = gameObject.AddOrGet<OccupyArea>();
		Object.DestroyImmediate(obj);
		obj = gameObject.AddOrGet<OccupyArea>();
		obj.OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(num, num2);
		obj.objectLayers = new ObjectLayer[1]
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
