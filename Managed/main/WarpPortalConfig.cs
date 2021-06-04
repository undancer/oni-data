using STRINGS;
using TUNING;
using UnityEngine;

public class WarpPortalConfig : IEntityConfig
{
	public const string ID = "WarpPortal";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("WarpPortal", STRINGS.BUILDINGS.PREFABS.WARPPORTAL.NAME, STRINGS.BUILDINGS.PREFABS.WARPPORTAL.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("warp_portal_sender_kanim"), initialAnim: "idle", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 3);
		gameObject.AddTag(GameTags.NotRoomAssignable);
		gameObject.AddTag(GameTags.WarpTech);
		gameObject.AddTag(GameTags.Gravitas);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<Operational>();
		gameObject.AddOrGet<Notifier>();
		gameObject.AddOrGet<WarpPortal>();
		gameObject.AddOrGet<LoreBearer>();
		gameObject.AddOrGet<LoopingSounds>();
		Ownable ownable = gameObject.AddOrGet<Ownable>();
		ownable.tintWhenUnassigned = false;
		gameObject.AddOrGet<Prioritizable>();
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		kBatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		WarpPortal component = inst.GetComponent<WarpPortal>();
		component.workLayer = Grid.SceneLayer.Building;
		Ownable component2 = inst.GetComponent<Ownable>();
		component2.slotID = Db.Get().AssignableSlots.WarpPortal.Id;
		OccupyArea component3 = inst.GetComponent<OccupyArea>();
		component3.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		inst.GetComponent<Deconstructable>();
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
