using TUNING;
using UnityEngine;

public class DevPumpGasConfig : IBuildingConfig
{
	public const string ID = "DevPumpGas";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	private ConduitPortInfo primaryPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(1, 1));

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("DevPumpGas", 2, 2, "dev_pump_gas_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.ALL_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.RequiresPowerInput = false;
		obj.OutputConduitType = ConduitType.Gas;
		obj.Floodable = false;
		obj.Invincible = true;
		obj.Entombable = false;
		obj.ViewMode = OverlayModes.GasConduits.ID;
		obj.AudioCategory = "Metal";
		obj.UtilityOutputOffset = primaryPort.offset;
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "DevPumpGas");
		obj.DebugOnly = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		base.ConfigureBuildingTemplate(go, prefab_tag);
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<DevPump>().elementState = Filterable.ElementState.Gas;
		go.AddOrGet<Storage>().capacityKg = 20f;
		go.AddTag(GameTags.CorrosionProof);
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Gas;
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.elementFilter = null;
		go.AddOrGetDef<OperationalController.Def>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
	}
}
