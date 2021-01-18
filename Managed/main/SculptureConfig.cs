using STRINGS;
using TUNING;
using UnityEngine;

public class SculptureConfig : IBuildingConfig
{
	public const string ID = "Sculpture";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("Sculpture", 1, 3, "sculpture_kanim", 30, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
		{
			amount = 10,
			radius = 8
		});
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		obj.BaseTimeUntilRepair = -1f;
		obj.ViewMode = OverlayModes.Decor.ID;
		obj.DefaultAnimState = "slab";
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
		Sculpture sculpture = go.AddComponent<Sculpture>();
		sculpture.stages.Add(new Artable.Stage("Default", STRINGS.BUILDINGS.PREFABS.SCULPTURE.NAME, "slab", 0, cheer_on_complete: false, Artable.Status.Ready));
		sculpture.stages.Add(new Artable.Stage("Bad", STRINGS.BUILDINGS.PREFABS.SCULPTURE.POORQUALITYNAME, "crap_1", 5, cheer_on_complete: false, Artable.Status.Ugly));
		sculpture.stages.Add(new Artable.Stage("Average", STRINGS.BUILDINGS.PREFABS.SCULPTURE.AVERAGEQUALITYNAME, "good_1", 10, cheer_on_complete: false, Artable.Status.Okay));
		sculpture.stages.Add(new Artable.Stage("Good1", STRINGS.BUILDINGS.PREFABS.SCULPTURE.EXCELLENTQUALITYNAME, "amazing_1", 15, cheer_on_complete: true, Artable.Status.Great));
		sculpture.stages.Add(new Artable.Stage("Good2", STRINGS.BUILDINGS.PREFABS.SCULPTURE.EXCELLENTQUALITYNAME, "amazing_2", 15, cheer_on_complete: true, Artable.Status.Great));
		sculpture.stages.Add(new Artable.Stage("Good3", STRINGS.BUILDINGS.PREFABS.SCULPTURE.EXCELLENTQUALITYNAME, "amazing_3", 15, cheer_on_complete: true, Artable.Status.Great));
	}
}
