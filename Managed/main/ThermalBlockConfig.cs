using TUNING;
using UnityEngine;

public class ThermalBlockConfig : IBuildingConfig
{
	public const string ID = "ThermalBlock";

	private static readonly CellOffset[] overrideOffsets = new CellOffset[4]
	{
		new CellOffset(-1, -1),
		new CellOffset(1, -1),
		new CellOffset(-1, 1),
		new CellOffset(1, 1)
	};

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ThermalBlock", 1, 1, "thermalblock_kanim", 30, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.ANY_BUILDABLE, 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: DECOR.NONE);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.ViewMode = OverlayModes.Temperature.ID;
		buildingDef.DefaultAnimState = "off";
		buildingDef.ObjectLayer = ObjectLayer.Backwall;
		buildingDef.SceneLayer = Grid.SceneLayer.Backwall;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		AnimTileable animTileable = go.AddOrGet<AnimTileable>();
		animTileable.objectLayer = ObjectLayer.Backwall;
		go.AddComponent<ZoneTile>();
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(game_object);
			StructureTemperaturePayload new_data = GameComps.StructureTemperatures.GetPayload(handle);
			int cell = Grid.PosToCell(game_object);
			new_data.OverrideExtents(new Extents(cell, overrideOffsets));
			GameComps.StructureTemperatures.SetPayload(handle, ref new_data);
		};
	}
}
