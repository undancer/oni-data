using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SolidLogicValveConfig : IBuildingConfig
{
	public const string ID = "SolidLogicValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Solid;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolidLogicValve", 1, 2, "conveyor_shutoff_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER1, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.InputConduitType = ConduitType.Solid;
		buildingDef.OutputConduitType = ConduitType.Solid;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.PowerInputOffset = new CellOffset(0, 1);
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.SOLIDLOGICVALVE.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.SOLIDLOGICVALVE.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.SOLIDLOGICVALVE.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidLogicValve");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicOperationalController logicOperationalController = go.AddOrGet<LogicOperationalController>();
		logicOperationalController.unNetworkedValue = 0;
		RequireOutputs requireOutputs = go.AddOrGet<RequireOutputs>();
		requireOutputs.ignoreFullPipe = true;
		go.AddOrGet<SolidConduitBridge>();
		go.AddOrGet<SolidLogicValve>();
	}
}
