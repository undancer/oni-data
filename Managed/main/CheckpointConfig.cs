using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CheckpointConfig : IBuildingConfig
{
	public const string ID = "Checkpoint";

	public override BuildingDef CreateBuildingDef()
	{
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, construction_materials: rEFINED_METALS, melting_point: 1600f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER0, id: "Checkpoint", width: 1, height: 3, anim: "checkpoint_kanim", hitpoints: 30, construction_time: 30f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER1);
		buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.PreventIdleTraversalPastBuilding = true;
		buildingDef.Floodable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 2);
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(Checkpoint.PORT_ID, new CellOffset(0, 2), STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT_INACTIVE, show_wire_missing_icon: true)
		};
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Checkpoint>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
