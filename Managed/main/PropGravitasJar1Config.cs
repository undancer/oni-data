using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropGravitasJar1Config : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("PropGravitasJar1", STRINGS.BUILDINGS.PREFABS.PROPGRAVITASJAR1.NAME, STRINGS.BUILDINGS.PREFABS.PROPGRAVITASJAR1.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("gravitas_jar1_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 1, height: 2, element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Gravitas
		});
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Granite);
		component.Temperature = 294.15f;
		obj.AddOrGet<LoreBearer>();
		obj.AddOrGet<Demolishable>();
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
