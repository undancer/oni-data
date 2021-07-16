using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GasLimitValveConfig : IBuildingConfig
{
	public const string ID = "GasLimitValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GasLimitValve", 1, 2, "limit_valve_gas_kanim", 30, 10f, new float[2]
		{
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, new string[2]
		{
			"RefinedMetal",
			"Plastic"
		}, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER1, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		obj.InputConduitType = ConduitType.Gas;
		obj.OutputConduitType = ConduitType.Gas;
		obj.Floodable = false;
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 10f;
		obj.PowerInputOffset = new CellOffset(0, 1);
		obj.ViewMode = OverlayModes.GasConduits.ID;
		obj.AudioCategory = "Metal";
		obj.PermittedRotations = PermittedRotations.R360;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 1);
		obj.LogicInputPorts = new List<LogicPorts.Port>
		{
			new LogicPorts.Port(LimitValve.RESET_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.LOGIC_PORT_RESET, STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.RESET_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.RESET_PORT_INACTIVE, show_wire_missing_icon: false, LogicPortSpriteType.ResetUpdate, display_custom_name: true)
		};
		obj.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort(LimitValve.OUTPUT_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.LOGIC_PORT_OUTPUT, STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.OUTPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.GASLIMITVALVE.OUTPUT_PORT_INACTIVE)
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasLimitValve");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGetDef<PoweredActiveTransitionController.Def>();
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<ConduitBridge>().type = ConduitType.Gas;
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
		go.GetComponent<RequireInputs>().SetRequirements(power: true, conduit: false);
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
