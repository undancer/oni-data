using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClothingFabricatorConfig : IBuildingConfig
{
	public const string ID = "ClothingFabricator";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("ClothingFabricator", 4, 3, "clothingfactory_kanim", 100, 240f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: TUNING.BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 240f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.PowerInputOffset = new CellOffset(2, 0);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_clothingfactory_kanim")
		};
		go.AddOrGet<ComplexFabricatorWorkable>().AnimOffset = new Vector3(-1f, 0f, 0f);
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ConfigureRecipes();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), TUNING.EQUIPMENT.VESTS.WARM_VEST_MASS)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Warm_Vest".ToTag(), 1f)
		};
		WarmVestConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingFabricator", array, array2), array, array2)
		{
			time = TUNING.EQUIPMENT.VESTS.WARM_VEST_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.WARM_VEST.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"ClothingFabricator"
			},
			sortOrder = 1
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), TUNING.EQUIPMENT.VESTS.COOL_VEST_MASS)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Cool_Vest".ToTag(), 1f)
		};
		CoolVestConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingFabricator", array3, array4), array3, array4)
		{
			time = TUNING.EQUIPMENT.VESTS.COOL_VEST_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.COOL_VEST.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"ClothingFabricator"
			},
			sortOrder = 1
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Funky_Vest".ToTag(), 1f)
		};
		FunkyVestConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingFabricator", array5, array6), array5, array6)
		{
			time = TUNING.EQUIPMENT.VESTS.FUNKY_VEST_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.FUNKY_VEST.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"ClothingFabricator"
			},
			sortOrder = 1
		};
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
		};
	}
}
