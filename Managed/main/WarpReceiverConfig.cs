using STRINGS;
using TUNING;
using UnityEngine;

public class WarpReceiverConfig : IEntityConfig
{
	public static string ID = "WarpReceiver";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity(ID, STRINGS.BUILDINGS.PREFABS.WARPRECEIVER.NAME, STRINGS.BUILDINGS.PREFABS.WARPRECEIVER.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("warp_portal_receiver_kanim"), initialAnim: "idle", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 3);
		obj.AddTag(GameTags.NotRoomAssignable);
		obj.AddTag(GameTags.WarpTech);
		obj.AddTag(GameTags.Gravitas);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		obj.AddOrGet<Operational>();
		obj.AddOrGet<Notifier>();
		obj.AddOrGet<WarpReceiver>();
		obj.AddOrGet<LoreBearer>();
		obj.AddOrGet<LoopingSounds>();
		obj.AddOrGet<Prioritizable>();
		KBatchedAnimController kBatchedAnimController = obj.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		kBatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		return obj;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<WarpReceiver>().workLayer = Grid.SceneLayer.Building;
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		inst.GetComponent<Deconstructable>();
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
