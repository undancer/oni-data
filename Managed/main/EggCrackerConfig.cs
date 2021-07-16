using TUNING;
using UnityEngine;

[EntityConfigOrder(2)]
public class EggCrackerConfig : IBuildingConfig
{
	public const string ID = "EggCracker";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("EggCracker", 2, 2, "egg_cracker_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER0);
		obj.AudioCategory = "Metal";
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity("snapto_egg", is_visible: false);
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.labelByResult = false;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		complexFabricator.duplicantOperated = true;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_egg_cracker_kanim")
		};
		complexFabricator.outputOffset = new Vector3(1f, 1f, 0f);
		Prioritizable.AddRef(go);
		go.AddOrGet<EggCracker>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
