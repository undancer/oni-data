using STRINGS;
using TUNING;
using UnityEngine;

public class GeneShufflerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("GeneShuffler", STRINGS.BUILDINGS.PREFABS.GENESHUFFLER.NAME, STRINGS.BUILDINGS.PREFABS.GENESHUFFLER.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("geneshuffler_kanim"), initialAnim: "on", sceneLayer: Grid.SceneLayer.Building, width: 4, height: 3);
		obj.AddTag(GameTags.NotRoomAssignable);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		obj.AddOrGet<Operational>();
		obj.AddOrGet<Notifier>();
		obj.AddOrGet<GeneShuffler>();
		obj.AddOrGet<LoreBearer>();
		obj.AddOrGet<LoopingSounds>();
		obj.AddOrGet<Ownable>();
		obj.AddOrGet<Prioritizable>();
		Storage storage = obj.AddOrGet<Storage>();
		storage.dropOnLoad = true;
		ManualDeliveryKG manualDeliveryKG = obj.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG.requestedItemTag = new Tag("GeneShufflerRecharge");
		manualDeliveryKG.refillMass = 1f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.capacity = 1f;
		KBatchedAnimController kBatchedAnimController = obj.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		kBatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		return obj;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<GeneShuffler>().workLayer = Grid.SceneLayer.Building;
		inst.GetComponent<Ownable>().slotID = Db.Get().AssignableSlots.GeneShuffler.Id;
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
