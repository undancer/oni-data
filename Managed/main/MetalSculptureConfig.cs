using STRINGS;
using TUNING;
using UnityEngine;

public class MetalSculptureConfig : IBuildingConfig
{
	public const string ID = "MetalSculpture";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("MetalSculpture", 1, 3, "sculpture_metal_kanim", 10, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
		{
			amount = 20,
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
		sculpture.stages.Add(new Artable.Stage("Default", STRINGS.BUILDINGS.PREFABS.METALSCULPTURE.NAME, "slab", 0, cheer_on_complete: false, Artable.Status.Ready));
		sculpture.stages.Add(new Artable.Stage("Bad", STRINGS.BUILDINGS.PREFABS.METALSCULPTURE.POORQUALITYNAME, "crap_1", 5, cheer_on_complete: false, Artable.Status.Ugly));
		sculpture.stages.Add(new Artable.Stage("Average", STRINGS.BUILDINGS.PREFABS.METALSCULPTURE.AVERAGEQUALITYNAME, "good_1", 10, cheer_on_complete: false, Artable.Status.Okay));
		sculpture.stages.Add(new Artable.Stage("Good1", STRINGS.BUILDINGS.PREFABS.METALSCULPTURE.EXCELLENTQUALITYNAME, "amazing_1", 15, cheer_on_complete: true, Artable.Status.Great));
		sculpture.stages.Add(new Artable.Stage("Good2", STRINGS.BUILDINGS.PREFABS.METALSCULPTURE.EXCELLENTQUALITYNAME, "amazing_2", 15, cheer_on_complete: true, Artable.Status.Great));
		sculpture.stages.Add(new Artable.Stage("Good3", STRINGS.BUILDINGS.PREFABS.METALSCULPTURE.EXCELLENTQUALITYNAME, "amazing_3", 15, cheer_on_complete: true, Artable.Status.Great));
	}
}
