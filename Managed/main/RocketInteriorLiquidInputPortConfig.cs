using TUNING;
using UnityEngine;

public class RocketInteriorLiquidInputPortConfig : IBuildingConfig
{
	public const string ID = "RocketInteriorLiquidInputPort";

	private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 0));

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("RocketInteriorLiquidInputPort", 1, 1, "rocket_interior_port_liquid_in_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.REFINED_METALS, 9999f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER0);
		obj.DefaultAnimState = "liquid_in";
		obj.Floodable = false;
		obj.Entombable = false;
		obj.Overheatable = false;
		obj.UseStructureTemperature = false;
		obj.Replaceable = false;
		obj.Invincible = true;
		obj.BaseTimeUntilRepair = -1f;
		obj.AudioCategory = "Metal";
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		return obj;
	}

	private void AttachPort(GameObject go)
	{
		go.AddComponent<ConduitSecondaryInput>().portInfo = liquidInputPort;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<SimCellOccupier>().notifyOnMelt = true;
		go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
		Storage storage = go.AddComponent<Storage>();
		storage.showInUI = false;
		storage.capacityKg = 10f;
		RocketConduitSender rocketConduitSender = go.AddComponent<RocketConduitSender>();
		rocketConduitSender.conduitStorage = storage;
		rocketConduitSender.conduitPortInfo = liquidInputPort;
		AutoStorageDropper.Def def = go.AddOrGetDef<AutoStorageDropper.Def>();
		def.elementFilter = new SimHashes[1] { SimHashes.Unobtanium };
		def.dropOffset = new CellOffset(0, 1);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RemoveLoopingSounds(go);
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Bunker);
		component.AddTag(GameTags.FloorTiles);
		component.AddTag(GameTags.NoRocketRefund);
		go.AddOrGetDef<MakeBaseSolid.Def>().solidOffsets = new CellOffset[1]
		{
			new CellOffset(0, 0)
		};
		go.AddOrGet<BuildingCellVisualizer>();
		go.GetComponent<Deconstructable>().allowDeconstruction = false;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		go.AddOrGet<BuildingCellVisualizer>();
		AttachPort(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.AddOrGet<BuildingCellVisualizer>();
		AttachPort(go);
	}
}
