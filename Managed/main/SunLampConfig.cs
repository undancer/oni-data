using TUNING;
using UnityEngine;

public class SunLampConfig : IBuildingConfig
{
	public const string ID = "SunLamp";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SunLamp", 2, 4, "sun_lamp_kanim", 10, 60f, new float[2]
		{
			200f,
			50f
		}, new string[2]
		{
			"RefinedMetal",
			"Glass"
		}, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER3);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 960f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.ExhaustKilowattsWhenActive = 1f;
		buildingDef.ViewMode = OverlayModes.Light.ID;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
		lightShapePreview.lux = 40000;
		lightShapePreview.radius = 16f;
		lightShapePreview.shape = LightShape.Cone;
		lightShapePreview.offset = new CellOffset((int)LIGHT2D.SUNLAMP_OFFSET.x, (int)LIGHT2D.SUNLAMP_OFFSET.y);
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
		light2D.Lux = 40000;
		light2D.overlayColour = LIGHT2D.SUNLAMP_OVERLAYCOLOR;
		light2D.Color = LIGHT2D.SUNLAMP_COLOR;
		light2D.Range = 16f;
		light2D.Angle = 5.2f;
		light2D.Direction = LIGHT2D.SUNLAMP_DIRECTION;
		light2D.Offset = LIGHT2D.SUNLAMP_OFFSET;
		light2D.shape = LightShape.Cone;
		light2D.drawOverlay = true;
		go.AddOrGetDef<LightController.Def>();
	}
}
