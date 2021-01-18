using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropGravitasDecorativeWindowConfig : IEntityConfig
{
	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropGravitasDecorativeWindow", STRINGS.BUILDINGS.PREFABS.PROPGRAVITASDECORATIVEWINDOW.NAME, STRINGS.BUILDINGS.PREFABS.PROPGRAVITASDECORATIVEWINDOW.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER2, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("gravitas_top_window_kanim"), initialAnim: "on", sceneLayer: Grid.SceneLayer.Building, width: 2, height: 3, element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Gravitas
		});
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Glass);
		component.Temperature = 294.15f;
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
