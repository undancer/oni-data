using TUNING;
using UnityEngine;

public class GasMiniPumpConfig : IBuildingConfig
{
	public const string ID = "GasMiniPump";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GasMiniPump", 1, 2, "minigaspump_kanim", 30, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.PLASTICS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.PermittedRotations = PermittedRotations.R360;
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 60f;
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.SelfHeatKilowattsWhenActive = 0f;
		obj.OutputConduitType = ConduitType.Gas;
		obj.Floodable = true;
		obj.ViewMode = OverlayModes.GasConduits.ID;
		obj.AudioCategory = "Metal";
		obj.PowerInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 1);
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasMiniPump");
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<EnergyConsumer>();
		go.AddOrGet<Pump>();
		go.AddOrGet<Storage>().capacityKg = 0.1f;
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.consumptionRate = 0.05f;
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
