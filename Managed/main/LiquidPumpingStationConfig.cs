using TUNING;
using UnityEngine;

public class LiquidPumpingStationConfig : IBuildingConfig
{
	public const string ID = "LiquidPumpingStation";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LiquidPumpingStation", 2, 4, "waterpump_kanim", 100, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.Floodable = false;
		obj.Entombable = true;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 0);
		obj.DefaultAnimState = "on";
		obj.ShowInBuildMenu = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<LiquidPumpingStation>().overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_waterpump_kanim") };
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = false;
		storage.allowItemRemoval = true;
		storage.showDescriptor = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		go.AddTag(GameTags.CorrosionProof);
	}

	private static void AddGuide(GameObject go, bool occupy_tiles)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = go.transform;
		gameObject.transform.SetLocalPosition(Vector3.zero);
		KBatchedAnimController kBatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kBatchedAnimController.Offset = go.GetComponent<Building>().Def.GetVisualizerOffset();
		kBatchedAnimController.AnimFiles = new KAnimFile[1] { Assets.GetAnim(new HashedString("waterpump_kanim")) };
		kBatchedAnimController.initialAnim = "place_guide";
		kBatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		kBatchedAnimController.isMovable = true;
		PumpingStationGuide pumpingStationGuide = gameObject.AddComponent<PumpingStationGuide>();
		pumpingStationGuide.parent = go;
		pumpingStationGuide.occupyTiles = occupy_tiles;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		AddGuide(go.GetComponent<Building>().Def.BuildingPreview, occupy_tiles: false);
		AddGuide(go.GetComponent<Building>().Def.BuildingUnderConstruction, occupy_tiles: true);
		go.AddOrGet<FakeFloorAdder>().floorOffsets = new CellOffset[2]
		{
			new CellOffset(0, 0),
			new CellOffset(1, 0)
		};
	}
}
