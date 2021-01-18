using TUNING;
using UnityEngine;

public class SolidFilterConfig : IBuildingConfig
{
	public const string ID = "SolidFilter";

	private const ConduitType CONDUIT_TYPE = ConduitType.Solid;

	private ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(0, 0));

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolidFilter", 3, 1, "filter_material_conveyor_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.InputConduitType = ConduitType.Solid;
		buildingDef.OutputConduitType = ConduitType.Solid;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		buildingDef.PermittedRotations = PermittedRotations.R360;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidFilter");
		return buildingDef;
	}

	private void AttachPort(GameObject go)
	{
		ConduitSecondaryOutput conduitSecondaryOutput = go.AddComponent<ConduitSecondaryOutput>();
		conduitSecondaryOutput.portInfo = secondaryPort;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		AttachPort(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		AttachPort(go);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<Structure>();
		ElementFilter elementFilter = go.AddOrGet<ElementFilter>();
		elementFilter.portInfo = secondaryPort;
		Filterable filterable = go.AddOrGet<Filterable>();
		filterable.filterElementState = Filterable.ElementState.Solid;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
	}
}
