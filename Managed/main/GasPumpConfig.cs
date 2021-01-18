using TUNING;
using UnityEngine;

public class GasPumpConfig : IBuildingConfig
{
	public const string ID = "GasPump";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GasPump", 2, 2, "pumpgas_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 240f;
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.SelfHeatKilowattsWhenActive = 0f;
		obj.OutputConduitType = ConduitType.Gas;
		obj.Floodable = true;
		obj.ViewMode = OverlayModes.GasConduits.ID;
		obj.AudioCategory = "Metal";
		obj.PowerInputOffset = new CellOffset(0, 1);
		obj.UtilityOutputOffset = new CellOffset(1, 1);
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasPump");
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<EnergyConsumer>();
		go.AddOrGet<Pump>();
		go.AddOrGet<Storage>().capacityKg = 1f;
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.consumptionRate = 0.5f;
		elementConsumer.storeOnConsume = true;
		elementConsumer.showInStatusPanel = false;
		elementConsumer.consumptionRadius = 2;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Gas;
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.elementFilter = null;
		go.AddOrGetDef<OperationalController.Def>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
	}
}
