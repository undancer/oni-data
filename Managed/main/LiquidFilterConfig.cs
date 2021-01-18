using TUNING;
using UnityEngine;

public class LiquidFilterConfig : IBuildingConfig
{
	public const string ID = "LiquidFilter";

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

	private ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 0));

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LiquidFilter", 3, 1, "filter_liquid_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 120f;
		obj.SelfHeatKilowattsWhenActive = 4f;
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.InputConduitType = ConduitType.Liquid;
		obj.OutputConduitType = ConduitType.Liquid;
		obj.Floodable = false;
		obj.ViewMode = OverlayModes.LiquidConduits.ID;
		obj.AudioCategory = "Metal";
		obj.UtilityInputOffset = new CellOffset(-1, 0);
		obj.UtilityOutputOffset = new CellOffset(1, 0);
		obj.PermittedRotations = PermittedRotations.R360;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidFilter");
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
		go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Liquid;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
