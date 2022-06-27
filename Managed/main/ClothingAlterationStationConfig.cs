using System.Collections.Generic;
using Database;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClothingAlterationStationConfig : IBuildingConfig
{
	public const string ID = "ClothingAlterationStation";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("ClothingAlterationStation", 4, 3, "super_snazzy_suit_alteration_station_kanim", 100, 240f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: TUNING.BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 240f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.PowerInputOffset = new CellOffset(0, 0);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.outputOffset = new Vector3(1f, 0f, 0f);
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		complexFabricatorWorkable.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_super_snazzy_suit_alteration_station_kanim") };
		complexFabricatorWorkable.workingPstComplete = new HashedString[1] { "working_pst_complete" };
		complexFabricatorWorkable.AnimOffset = new Vector3(-1f, 0f, 0f);
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ConfigureRecipes();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("Funky_Vest".ToTag(), 1f, inheritElement: false),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 3f)
		};
		foreach (EquippableFacadeResource item in Db.Get().EquippableFacades.resources.FindAll((EquippableFacadeResource match) => match.DefID == "CustomClothing"))
		{
			ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
			{
				new ComplexRecipe.RecipeElement("CustomClothing".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, item.Id)
			};
			string id = ComplexRecipeManager.MakeRecipeID("ClothingAlterationStation", array, array2, item.Id);
			CustomClothingConfig.facadeRecipes.Add(new ComplexRecipe(id, array, array2)
			{
				time = TUNING.EQUIPMENT.VESTS.CUSTOM_CLOTHING_FABTIME,
				description = STRINGS.EQUIPMENT.PREFABS.CUSTOMCLOTHING.RECIPE_DESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { "ClothingAlterationStation" },
				sortOrder = 1
			});
		}
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
			component.requiredSkillPerk = Db.Get().SkillPerks.CanClothingAlteration.Id;
		};
	}
}
