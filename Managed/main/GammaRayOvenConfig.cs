using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GammaRayOvenConfig : IBuildingConfig
{
	public const string ID = "GammaRayOven";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GammaRayOven", 2, 2, "kiln_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: TUNING.BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		GammaRayOven gammaRayOven = go.AddOrGet<GammaRayOven>();
		gammaRayOven.heatedTemperature = 368.15f;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_cookstation_kanim")
		};
		gammaRayOven.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		Prioritizable.AddRef(go);
		go.AddOrGet<DropAllWorkable>();
		ConfigureRecipes();
		go.AddOrGetDef<PoweredController.Def>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, gammaRayOven);
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("FriedMushBar", 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("GammaMush".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id = ComplexRecipeManager.MakeRecipeID("GammaRayOven", array, array2);
		GammaMushConfig.recipe = new ComplexRecipe(id, array, array2)
		{
			time = FOOD.RECIPES.SMALL_COOK_TIME,
			description = ITEMS.FOOD.GAMMAMUSH.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"GammaRayOven"
			},
			sortOrder = 1
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Lettuce", 1f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("MicrowavedLettuce", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("GammaRayOven", array3, array4);
		MicrowavedLettuceConfig.recipe = new ComplexRecipe(id2, array3, array4)
		{
			time = FOOD.RECIPES.SMALL_COOK_TIME,
			description = ITEMS.FOOD.MICROWAVEDLETTUCE.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"GammaRayOven"
			},
			sortOrder = 21
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("ColdWheatSeed", 2f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Popcorn", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id3 = ComplexRecipeManager.MakeRecipeID("GammaRayOven", array5, array6);
		PopcornConfig.recipe = new ComplexRecipe(id3, array5, array6)
		{
			time = FOOD.RECIPES.SMALL_COOK_TIME,
			description = ITEMS.FOOD.POPCORN.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"GammaRayOven"
			},
			sortOrder = 21
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Radiator radiator = go.AddOrGet<Radiator>();
		radiator.intensity = 25;
		radiator.projectionCount = 48;
		radiator.angle = 360;
		radiator.direction = 0;
	}
}
