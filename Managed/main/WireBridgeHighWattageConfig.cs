using TUNING;
using UnityEngine;

public class WireBridgeHighWattageConfig : IBuildingConfig
{
	public const string ID = "WireBridgeHighWattage";

	protected virtual string GetID()
	{
		return "WireBridgeHighWattage";
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(GetID(), 1, 1, "heavywatttile_kanim", 100, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.HighWattBridgeTile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER5);
		BuildingTemplates.CreateFoundationTileDef(obj);
		obj.Overheatable = false;
		obj.UseStructureTemperature = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.PermittedRotations = PermittedRotations.R360;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 2);
		obj.ObjectLayer = ObjectLayer.Building;
		obj.SceneLayer = Grid.SceneLayer.WireBridgesFront;
		obj.ForegroundLayer = Grid.SceneLayer.TileMain;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, "WireBridgeHighWattage");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.PENALTY_3;
		simCellOccupier.notifyOnMelt = true;
		go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
		go.AddOrGet<TileTemperature>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		AddNetworkLink(go).visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		AddNetworkLink(go).visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		AddNetworkLink(go).visualizeOnly = false;
		go.GetComponent<KPrefabID>().AddTag(GameTags.WireBridges);
		go.AddOrGet<BuildingCellVisualizer>();
	}

	protected virtual WireUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = go.AddOrGet<WireUtilityNetworkLink>();
		wireUtilityNetworkLink.maxWattageRating = Wire.WattageRating.Max20000;
		wireUtilityNetworkLink.link1 = new CellOffset(-1, 0);
		wireUtilityNetworkLink.link2 = new CellOffset(1, 0);
		return wireUtilityNetworkLink;
	}
}
