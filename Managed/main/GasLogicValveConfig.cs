using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GasLogicValveConfig : IBuildingConfig
{
	public const string ID = "GasLogicValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GasLogicValve", 1, 2, "valvegas_logic_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER1, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
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
			LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.GASLOGICVALVE.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.GASLOGICVALVE.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.GASLOGICVALVE.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasLogicValve");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		OperationalValve operationalValve = go.AddOrGet<OperationalValve>();
		operationalValve.conduitType = ConduitType.Gas;
		operationalValve.maxFlow = 1f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		go.GetComponent<RequireInputs>().SetRequirements(power: true, conduit: false);
		go.AddOrGet<LogicOperationalController>().unNetworkedValue = 0;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
