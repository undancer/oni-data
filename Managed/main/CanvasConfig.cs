using STRINGS;
using TUNING;
using UnityEngine;

public class CanvasConfig : IBuildingConfig
{
	public const string ID = "Canvas";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("Canvas", 2, 2, "painting_kanim", 30, 120f, new float[2] { 400f, 1f }, new string[2] { "Metal", "BuildingFiber" }, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
		{
			amount = 10,
			radius = 6
		});
		obj.Floodable = false;
		obj.SceneLayer = Grid.SceneLayer.InteriorWall;
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		obj.BaseTimeUntilRepair = -1f;
		obj.ViewMode = OverlayModes.Decor.ID;
		obj.DefaultAnimState = "off";
		obj.PermittedRotations = PermittedRotations.FlipH;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isArtable = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Painting painting = go.AddComponent<Painting>();
		painting.stages.Add(new Artable.Stage("Default", STRINGS.BUILDINGS.PREFABS.CANVAS.NAME, "off", 0, cheer_on_complete: false, Artable.Status.Ready));
		painting.stages.Add(new Artable.Stage("Bad", STRINGS.BUILDINGS.PREFABS.CANVAS.POORQUALITYNAME, "art_a", 5, cheer_on_complete: false, Artable.Status.Ugly));
		painting.stages.Add(new Artable.Stage("Average", STRINGS.BUILDINGS.PREFABS.CANVAS.AVERAGEQUALITYNAME, "art_b", 10, cheer_on_complete: false, Artable.Status.Okay));
		painting.stages.Add(new Artable.Stage("Good", STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_c", 15, cheer_on_complete: true, Artable.Status.Great));
		painting.stages.Add(new Artable.Stage("Good2", STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_d", 15, cheer_on_complete: true, Artable.Status.Great));
		painting.stages.Add(new Artable.Stage("Good3", STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_e", 15, cheer_on_complete: true, Artable.Status.Great));
	}
}
