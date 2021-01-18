using STRINGS;
using TUNING;
using UnityEngine;

public class CanvasConfig : IBuildingConfig
{
	public const string ID = "Canvas";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Canvas", 2, 2, "painting_kanim", 30, 120f, new float[2]
		{
			400f,
			1f
		}, new string[2]
		{
			"Metal",
			"BuildingFiber"
		}, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
		{
			amount = 10,
			radius = 6
		});
		buildingDef.Floodable = false;
		buildingDef.SceneLayer = Grid.SceneLayer.InteriorWall;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.DefaultAnimState = "off";
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isArtable = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Artable artable = go.AddComponent<Painting>();
		artable.stages.Add(new Artable.Stage("Default", STRINGS.BUILDINGS.PREFABS.CANVAS.NAME, "off", 0, cheer_on_complete: false, Artable.Status.Ready));
		artable.stages.Add(new Artable.Stage("Bad", STRINGS.BUILDINGS.PREFABS.CANVAS.POORQUALITYNAME, "art_a", 5, cheer_on_complete: false, Artable.Status.Ugly));
		artable.stages.Add(new Artable.Stage("Average", STRINGS.BUILDINGS.PREFABS.CANVAS.AVERAGEQUALITYNAME, "art_b", 10, cheer_on_complete: false, Artable.Status.Okay));
		artable.stages.Add(new Artable.Stage("Good", STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_c", 15, cheer_on_complete: true, Artable.Status.Great));
		artable.stages.Add(new Artable.Stage("Good2", STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_d", 15, cheer_on_complete: true, Artable.Status.Great));
		artable.stages.Add(new Artable.Stage("Good3", STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_e", 15, cheer_on_complete: true, Artable.Status.Great));
	}
}
