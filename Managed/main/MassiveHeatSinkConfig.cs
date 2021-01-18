using TUNING;
using UnityEngine;

public class MassiveHeatSinkConfig : IBuildingConfig
{
	public const string ID = "MassiveHeatSink";

	private const float CONSUMPTION_RATE = 0.01f;

	private const float STORAGE_CAPACITY = 0.099999994f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MassiveHeatSink", 4, 4, "massiveheatsink_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_METALS, 2400f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.BONUS.TIER2);
		buildingDef.ExhaustKilowattsWhenActive = -16f;
		buildingDef.SelfHeatKilowattsWhenActive = -64f;
		buildingDef.Floodable = true;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.InputConduitType = ConduitType.Gas;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<MassiveHeatSink>();
		MinimumOperatingTemperature minimumOperatingTemperature = go.AddOrGet<MinimumOperatingTemperature>();
		minimumOperatingTemperature.minimumTemperature = 100f;
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Iron);
		component.Temperature = 294.15f;
		go.AddOrGet<LoopingSounds>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 0.099999994f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Hydrogen);
		conduitConsumer.capacityKG = 0.099999994f;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(ElementLoader.FindElementByHash(SimHashes.Hydrogen).tag, 0.01f)
		};
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
