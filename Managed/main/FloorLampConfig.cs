using TUNING;
using UnityEngine;

public class FloorLampConfig : IBuildingConfig
{
	public const string ID = "FloorLamp";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("FloorLamp", 1, 2, "floorlamp_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER1);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 8f;
		obj.SelfHeatKilowattsWhenActive = 0.5f;
		obj.ViewMode = OverlayModes.Light.ID;
		obj.AudioCategory = "Metal";
		return obj;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
		lightShapePreview.lux = 1000;
		lightShapePreview.radius = 4f;
		lightShapePreview.shape = LightShape.Circle;
		lightShapePreview.offset = new CellOffset((int)def.BuildingComplete.GetComponent<Light2D>().Offset.x, (int)def.BuildingComplete.GetComponent<Light2D>().Offset.y);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.LightSource);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<EnergyConsumer>();
		go.AddOrGet<LoopingSounds>();
		Light2D light2D = go.AddOrGet<Light2D>();
		light2D.overlayColour = LIGHT2D.FLOORLAMP_OVERLAYCOLOR;
		light2D.Color = LIGHT2D.FLOORLAMP_COLOR;
		light2D.Range = 4f;
		light2D.Angle = 0f;
		light2D.Direction = LIGHT2D.FLOORLAMP_DIRECTION;
		light2D.Offset = LIGHT2D.FLOORLAMP_OFFSET;
		light2D.shape = LightShape.Circle;
		light2D.drawOverlay = true;
		go.AddOrGetDef<LightController.Def>();
	}
}
