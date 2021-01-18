using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LiquidLogicValveConfig : IBuildingConfig
{
	public const string ID = "LiquidLogicValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidLogicValve", 1, 2, "valveliquid_logic_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER1, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.PowerInputOffset = new CellOffset(0, 1);
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LIQUIDLOGICVALVE.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LIQUIDLOGICVALVE.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LIQUIDLOGICVALVE.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidLogicValve");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		OperationalValve operationalValve = go.AddOrGet<OperationalValve>();
		operationalValve.conduitType = ConduitType.Liquid;
		operationalValve.maxFlow = 10f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		RequireInputs component = go.GetComponent<RequireInputs>();
		component.SetRequirements(power: true, conduit: false);
		LogicOperationalController logicOperationalController = go.AddOrGet<LogicOperationalController>();
		logicOperationalController.unNetworkedValue = 0;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
