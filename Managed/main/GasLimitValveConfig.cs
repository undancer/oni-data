using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GasLimitValveConfig : IBuildingConfig
{
	public const string ID = "GasLimitValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GasLimitValve", 1, 2, "limit_valve_gas_kanim", 30, 10f, new float[2]
		{
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, new string[2]
		{
			"RefinedMetal",
			"Plastic"
		}, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER1, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.PowerInputOffset = new CellOffset(0, 1);
		buildingDef.ViewMode = OverlayModes.GasConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			new LogicPorts.Port(LimitValve.RESET_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.LOGIC_PORT_RESET, STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.RESET_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.RESET_PORT_INACTIVE, show_wire_missing_icon: false, LogicPortSpriteType.ResetUpdate, display_custom_name: true)
		};
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LimitValve.OUTPUT_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.LOGIC_PORT_OUTPUT, STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.OUTPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.OUTPUT_PORT_INACTIVE)
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasLimitValve");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGetDef<PoweredActiveTransitionController.Def>();
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		ConduitBridge conduitBridge = go.AddOrGet<ConduitBridge>();
		conduitBridge.type = ConduitType.Gas;
		LimitValve limitValve = go.AddOrGet<LimitValve>();
		limitValve.conduitType = ConduitType.Gas;
		limitValve.maxLimitKg = 500f;
		limitValve.Limit = 0f;
		limitValve.sliderRanges = LimitValveTuning.GetDefaultSlider();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		RequireInputs component = go.GetComponent<RequireInputs>();
		component.SetRequirements(power: true, conduit: false);
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
