using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SolidLimitValveConfig : IBuildingConfig
{
	public const string ID = "SolidLimitValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Solid;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("SolidLimitValve", 1, 2, "limit_valve_solid_kanim", 30, 10f, new float[2]
		{
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, new string[2] { "RefinedMetal", "Plastic" }, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER1, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		obj.InputConduitType = ConduitType.Solid;
		obj.OutputConduitType = ConduitType.Solid;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.SolidConveyor.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.PermittedRotations = PermittedRotations.R360;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 1);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 10f;
		obj.PowerInputOffset = new CellOffset(0, 1);
		obj.LogicInputPorts = new List<LogicPorts.Port>
		{
			new LogicPorts.Port(LimitValve.RESET_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.LOGIC_PORT_RESET, STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.RESET_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.RESET_PORT_INACTIVE, show_wire_missing_icon: false, LogicPortSpriteType.ResetUpdate, display_custom_name: true)
		};
		obj.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(LimitValve.OUTPUT_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.LOGIC_PORT_OUTPUT, STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.OUTPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.OUTPUT_PORT_INACTIVE) };
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidLimitValve");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveTransitionController.Def>();
		go.AddOrGet<RequireOutputs>().ignoreFullPipe = true;
		go.AddOrGet<SolidConduitBridge>();
		LimitValve limitValve = go.AddOrGet<LimitValve>();
		limitValve.conduitType = ConduitType.Solid;
		limitValve.displayUnitsInsteadOfMass = true;
		limitValve.Limit = 0f;
		limitValve.maxLimitKg = 500f;
		limitValve.sliderRanges = new NonLinearSlider.Range[3]
		{
			new NonLinearSlider.Range(50f, 50f),
			new NonLinearSlider.Range(30f, 200f),
			new NonLinearSlider.Range(20f, limitValve.maxLimitKg)
		};
	}
}
