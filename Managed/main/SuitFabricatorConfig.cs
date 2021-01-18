using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SuitFabricatorConfig : IBuildingConfig
{
	public const string ID = "SuitFabricator";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("SuitFabricator", 4, 3, "suit_maker_kanim", 100, 240f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: TUNING.BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 480f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.PowerInputOffset = new CellOffset(1, 0);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<Prioritizable>();
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.resultState = ComplexFabricator.ResultState.Heated;
		complexFabricator.heatedTemperature = 318.15f;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_suit_fabricator_kanim")
		};
		Prioritizable.AddRef(go);
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		ConfigureRecipes();
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Copper.CreateTag(), 300f),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", array, array2), array, array2)
		{
			time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag>
			{
				"SuitFabricator"
			},
			requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), 300f),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", array3, array4), array3, array4)
		{
			time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag>
			{
				"SuitFabricator"
			},
			requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 300f),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", array5, array6), array5, array6)
		{
			time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag>
			{
				"SuitFabricator"
			},
			requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id
		};
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Steel.ToString(), 200f),
			new ComplexRecipe.RecipeElement(SimHashes.Petroleum.ToString(), 25f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Jet_Suit".ToTag(), 1f)
		};
		JetSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", array7, array8), array7, array8)
		{
			time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag>
			{
				"SuitFabricator"
			},
			requiredTech = Db.Get().TechItems.jetSuit.parentTech.Id
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabInitFn += delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits);
		};
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
