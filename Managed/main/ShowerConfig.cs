using TUNING;
using UnityEngine;

public class ShowerConfig : IBuildingConfig
{
	public static string ID = "Shower";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 2, 4, "shower_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.BONUS.TIER1);
		obj.Overheatable = false;
		obj.ExhaustKilowattsWhenActive = 0.25f;
		obj.InputConduitType = ConduitType.Liquid;
		obj.OutputConduitType = ConduitType.Liquid;
		obj.ViewMode = OverlayModes.LiquidConduits.ID;
		obj.AudioCategory = "Metal";
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(1, 1);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.WashStation);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.AdvancedWashStation);
		Shower shower = go.AddOrGet<Shower>();
		shower.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_shower_kanim")
		};
		shower.workTime = 15f;
		shower.outputTargetElement = SimHashes.DirtyWater;
		shower.fractionalDiseaseRemoval = 0.95f;
		shower.absoluteDiseaseRemoval = -2000;
		shower.workLayer = Grid.SceneLayer.BuildingFront;
		shower.trackUses = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		conduitConsumer.capacityKG = 5f;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[1]
		{
			SimHashes.Water
		};
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(new Tag("Water"), 1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(1f, SimHashes.DirtyWater, 0f, useEntityTemperature: false, storeOutput: true)
		};
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 10f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<RequireOutputs>().ignoreFullPipe = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
