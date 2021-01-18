using STRINGS;
using TUNING;
using UnityEngine;

public class LandingPodConfig : IEntityConfig
{
	public const string ID = "LandingPod";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("LandingPod", STRINGS.BUILDINGS.PREFABS.LANDING_POD.NAME, STRINGS.BUILDINGS.PREFABS.LANDING_POD.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("rocket_puft_pod_kanim"), initialAnim: "grounded", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 3);
		gameObject.AddOrGet<PodLander>();
		gameObject.AddOrGet<MinionStorage>();
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
