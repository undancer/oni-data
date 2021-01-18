using TUNING;
using UnityEngine;

public class DesalinatorConfig : IBuildingConfig
{
	public const string ID = "Desalinator";

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

	private const float INPUT_RATE = 5f;

	private const float SALT_WATER_TO_SALT_OUTPUT_RATE = 0.35f;

	private const float SALT_WATER_TO_CLEAN_WATER_OUTPUT_RATE = 4.65f;

	private const float BRINE_TO_SALT_OUTPUT_RATE = 1.5f;

	private const float BRINE_TO_CLEAN_WATER_OUTPUT_RATE = 3.5f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Desalinator", 4, 3, "desalinator_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.SelfHeatKilowattsWhenActive = 8f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		Desalinator desalinator = go.AddOrGet<Desalinator>();
		desalinator.maxSalt = 945f;
		ElementConverter elementConverter = go.AddComponent<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(new Tag("SaltWater"), 5f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[2]
		{
			new ElementConverter.OutputElement(4.65f, SimHashes.Water, 0f, useEntityTemperature: false, storeOutput: true, 0f, 0.5f, 0.75f),
			new ElementConverter.OutputElement(0.35f, SimHashes.Salt, 0f, useEntityTemperature: false, storeOutput: true, 0f, 0.5f, 0.25f)
		};
		ElementConverter elementConverter2 = go.AddComponent<ElementConverter>();
		elementConverter2.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(new Tag("Brine"), 5f)
		};
		elementConverter2.outputElements = new ElementConverter.OutputElement[2]
		{
			new ElementConverter.OutputElement(3.5f, SimHashes.Water, 313.15f, useEntityTemperature: false, storeOutput: true, 0f, 0.5f, 0.75f),
			new ElementConverter.OutputElement(1.5f, SimHashes.Salt, 313.15f, useEntityTemperature: false, storeOutput: true, 0f, 0.5f, 0.25f)
		};
		DesalinatorWorkableEmpty desalinatorWorkableEmpty = go.AddOrGet<DesalinatorWorkableEmpty>();
		desalinatorWorkableEmpty.workTime = 90f;
		desalinatorWorkableEmpty.workLayer = Grid.SceneLayer.BuildingFront;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityKG = 20f;
		conduitConsumer.capacityTag = GameTags.AnyWater;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[2]
		{
			SimHashes.SaltWater,
			SimHashes.Brine
		};
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
	}
}
