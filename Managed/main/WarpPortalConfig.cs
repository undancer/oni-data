using STRINGS;
using TUNING;
using UnityEngine;

public class WarpPortalConfig : IEntityConfig
{
	public const string ID = "WarpPortal";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("WarpPortal", STRINGS.BUILDINGS.PREFABS.WARPPORTAL.NAME, STRINGS.BUILDINGS.PREFABS.WARPPORTAL.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("warp_portal_sender_kanim"), initialAnim: "idle", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 3);
		obj.AddTag(GameTags.NotRoomAssignable);
		obj.AddTag(GameTags.WarpTech);
		obj.AddTag(GameTags.Gravitas);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		obj.AddOrGet<Operational>();
		obj.AddOrGet<Notifier>();
		obj.AddOrGet<WarpPortal>();
		obj.AddOrGet<LoreBearer>();
		obj.AddOrGet<LoopingSounds>();
		obj.AddOrGet<Ownable>().tintWhenUnassigned = false;
		obj.AddOrGet<Prioritizable>();
		KBatchedAnimController kBatchedAnimController = obj.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		kBatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		return obj;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<WarpPortal>().workLayer = Grid.SceneLayer.Building;
		inst.GetComponent<Ownable>().slotID = Db.Get().AssignableSlots.WarpPortal.Id;
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[1] { ObjectLayer.Building };
		inst.GetComponent<Deconstructable>();
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
