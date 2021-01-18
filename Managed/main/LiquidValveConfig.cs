using TUNING;
using UnityEngine;

public class LiquidValveConfig : IBuildingConfig
{
	public const string ID = "LiquidValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidValve", 1, 2, "valveliquid_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidValve");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		ValveBase valveBase = go.AddOrGet<ValveBase>();
		valveBase.conduitType = ConduitType.Liquid;
		valveBase.maxFlow = 10f;
		valveBase.animFlowRanges = new ValveBase.AnimRangeInfo[3]
		{
			new ValveBase.AnimRangeInfo(3f, "lo"),
			new ValveBase.AnimRangeInfo(7f, "med"),
			new ValveBase.AnimRangeInfo(10f, "hi")
		};
		go.AddOrGet<Valve>();
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 5f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Object.DestroyImmediate(go.GetComponent<RequireInputs>());
		Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
