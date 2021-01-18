using STRINGS;
using TUNING;
using UnityEngine;

public class WarpReceiverConfig : IEntityConfig
{
	public static string ID = "WarpReceiver";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(ID, STRINGS.BUILDINGS.PREFABS.WARPRECEIVER.NAME, STRINGS.BUILDINGS.PREFABS.WARPRECEIVER.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("warp_portal_receiver_kanim"), initialAnim: "idle", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 3);
		gameObject.AddTag(GameTags.NotRoomAssignable);
		gameObject.AddTag(GameTags.WarpTech);
		gameObject.AddTag(GameTags.Gravitas);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<Operational>();
		gameObject.AddOrGet<Notifier>();
		gameObject.AddOrGet<WarpReceiver>();
		gameObject.AddOrGet<LoreBearer>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<Prioritizable>();
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		kBatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		WarpReceiver component = inst.GetComponent<WarpReceiver>();
		component.workLayer = Grid.SceneLayer.Building;
		OccupyArea component2 = inst.GetComponent<OccupyArea>();
		component2.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		inst.GetComponent<Deconstructable>();
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
