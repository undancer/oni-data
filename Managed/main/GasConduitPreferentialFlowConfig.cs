using TUNING;
using UnityEngine;

public class GasConduitPreferentialFlowConfig : IBuildingConfig
{
	public const string ID = "GasConduitPreferentialFlow";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	private ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(0, 1));

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GasConduitPreferentialFlow", 2, 2, "valvegas_kanim", 10, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.Conduit, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		buildingDef.Deprecated = true;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.GasConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, buildingDef.PrefabID);
		return buildingDef;
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

	private void AttachPort(GameObject go)
	{
		ConduitSecondaryInput conduitSecondaryInput = go.AddComponent<ConduitSecondaryInput>();
		conduitSecondaryInput.portInfo = secondaryPort;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		ConduitPreferentialFlow conduitPreferentialFlow = go.AddOrGet<ConduitPreferentialFlow>();
		conduitPreferentialFlow.portInfo = secondaryPort;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Object.DestroyImmediate(go.GetComponent<RequireInputs>());
		Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
	}
}
