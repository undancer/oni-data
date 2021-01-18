using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PixelPackConfig : IBuildingConfig
{
	public static string ID = "PixelPack";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 4, 1, "pixel_pack_kanim", 30, 10f, new float[2]
		{
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0]
		}, new string[2]
		{
			"Glass",
			"RefinedMetal"
		}, 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER3);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.PermittedRotations = PermittedRotations.R360;
		obj.RequiresPowerInput = true;
		obj.PowerInputOffset = new CellOffset(1, 0);
		obj.EnergyConsumptionWhenActive = 10f;
		obj.SelfHeatKilowattsWhenActive = 0f;
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.RibbonInputPort(PixelPack.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.PIXELPACK.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.PIXELPACK.INPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.PIXELPACK.INPUT_PORT_INACTIVE)
		};
		obj.ViewMode = OverlayModes.Logic.ID;
		obj.AudioCategory = "Metal";
		obj.ObjectLayer = ObjectLayer.Backwall;
		obj.SceneLayer = Grid.SceneLayer.InteriorWall;
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Backwall;
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddComponent<ZoneTile>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<PixelPack>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
	}
}
