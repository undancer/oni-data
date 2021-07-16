using TUNING;
using UnityEngine;

public class CeilingLightConfig : IBuildingConfig
{
	public const string ID = "CeilingLight";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("CeilingLight", 1, 1, "ceilinglight_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnCeiling, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 10f;
		obj.SelfHeatKilowattsWhenActive = 0.5f;
		obj.ViewMode = OverlayModes.Light.ID;
		obj.AudioCategory = "Metal";
		return obj;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
		lightShapePreview.lux = 1800;
		lightShapePreview.radius = 8f;
		lightShapePreview.shape = LightShape.Cone;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.LightSource);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Light2D light2D = go.AddOrGet<Light2D>();
		light2D.overlayColour = LIGHT2D.CEILINGLIGHT_OVERLAYCOLOR;
		light2D.Color = LIGHT2D.CEILINGLIGHT_COLOR;
		light2D.Range = 8f;
		light2D.Angle = 2.6f;
		light2D.Direction = LIGHT2D.CEILINGLIGHT_DIRECTION;
		light2D.Offset = LIGHT2D.CEILINGLIGHT_OFFSET;
		light2D.shape = LightShape.Cone;
		light2D.drawOverlay = true;
		light2D.Lux = 1800;
		go.AddOrGetDef<LightController.Def>();
	}
}
