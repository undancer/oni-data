using STRINGS;
using TUNING;
using UnityEngine;

public class LandingPodConfig : IEntityConfig
{
	public const string ID = "LandingPod";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("LandingPod", STRINGS.BUILDINGS.PREFABS.LANDING_POD.NAME, STRINGS.BUILDINGS.PREFABS.LANDING_POD.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("rocket_puft_pod_kanim"), initialAnim: "grounded", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 3);
		obj.AddOrGet<PodLander>();
		obj.AddOrGet<MinionStorage>();
		return obj;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[1] { ObjectLayer.Building };
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
