using TUNING;
using UnityEngine;

public class GasFilterConfig : IBuildingConfig
{
	public const string ID = "GasFilter";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	private ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(0, 0));

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GasFilter", 3, 1, "filter_gas_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 120f;
		obj.SelfHeatKilowattsWhenActive = 0f;
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.InputConduitType = ConduitType.Gas;
		obj.OutputConduitType = ConduitType.Gas;
		obj.Floodable = false;
		obj.ViewMode = OverlayModes.GasConduits.ID;
		obj.AudioCategory = "Metal";
		obj.UtilityInputOffset = new CellOffset(-1, 0);
		obj.UtilityOutputOffset = new CellOffset(1, 0);
		obj.PermittedRotations = PermittedRotations.R360;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasFilter");
		return obj;
	}

	private void AttachPort(GameObject go)
	{
		go.AddComponent<ConduitSecondaryOutput>().portInfo = secondaryPort;
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
		go.AddOrGet<ElementFilter>().portInfo = secondaryPort;
		go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Gas;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
