using STRINGS;
using TUNING;
using UnityEngine;

public class IceSculptureConfig : IBuildingConfig
{
	public const string ID = "IceSculpture";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("IceSculpture", 2, 2, "icesculpture_kanim", 10, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, new string[1] { "Ice" }, 273.15f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
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
		sculpture.stages.Add(new Artable.Stage("Default", STRINGS.BUILDINGS.PREFABS.ICESCULPTURE.NAME, "slab", 0, cheer_on_complete: false, Artable.Status.Ready));
		sculpture.stages.Add(new Artable.Stage("Bad", STRINGS.BUILDINGS.PREFABS.ICESCULPTURE.POORQUALITYNAME, "crap", 5, cheer_on_complete: false, Artable.Status.Ugly));
		sculpture.stages.Add(new Artable.Stage("Average", STRINGS.BUILDINGS.PREFABS.ICESCULPTURE.AVERAGEQUALITYNAME, "idle", 10, cheer_on_complete: true, Artable.Status.Okay));
	}
}
