using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CookingStationConfig : IBuildingConfig
{
	public const string ID = "CookingStation";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CookingStation", 3, 2, "cookstation_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: TUNING.BUILDINGS.DECOR.NONE);
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
		CookingStation cookingStation = go.AddOrGet<CookingStation>();
		cookingStation.heatedTemperature = 368.15f;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_cookstation_kanim")
		};
		cookingStation.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		Prioritizable.AddRef(go);
		go.AddOrGet<DropAllWorkable>();
		ConfigureRecipes();
		go.AddOrGetDef<PoweredController.Def>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, cookingStation);
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BasicPlantFood", 3f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("PickledMeal", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id = ComplexRecipeManager.MakeRecipeID("CookingStation", array, array2);
		PickledMealConfig.recipe = new ComplexRecipe(id, array, array2)
		{
			time = FOOD.RECIPES.SMALL_COOK_TIME,
			description = ITEMS.FOOD.PICKLEDMEAL.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 21
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("MushBar", 1f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("FriedMushBar".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("CookingStation", array3, array4);
		FriedMushBarConfig.recipe = new ComplexRecipe(id2, array3, array4)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.FRIEDMUSHBAR.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 1
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(MushroomConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("FriedMushroom", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id3 = ComplexRecipeManager.MakeRecipeID("CookingStation", array5, array6);
		FriedMushroomConfig.recipe = new ComplexRecipe(id3, array5, array6)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.FRIEDMUSHROOM.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 20
		};
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Meat", 2f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("CookedMeat", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id4 = ComplexRecipeManager.MakeRecipeID("CookingStation", array7, array8);
		CookedMeatConfig.recipe = new ComplexRecipe(id4, array7, array8)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.COOKEDMEAT.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 21
		};
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("FishMeat", 1f)
		};
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("CookedFish", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id5 = ComplexRecipeManager.MakeRecipeID("CookingStation", array9, array10);
		CookedMeatConfig.recipe = new ComplexRecipe(id5, array9, array10)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.COOKEDMEAT.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 21
		};
		ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(PrickleFruitConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("GrilledPrickleFruit", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id6 = ComplexRecipeManager.MakeRecipeID("CookingStation", array11, array12);
		GrilledPrickleFruitConfig.recipe = new ComplexRecipe(id6, array11, array12)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.GRILLEDPRICKLEFRUIT.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 20
		};
		ComplexRecipe.RecipeElement[] array13 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SwampFruitConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array14 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("SwampDelights", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id7 = ComplexRecipeManager.MakeRecipeID("CookingStation", array13, array14);
		CookedEggConfig.recipe = new ComplexRecipe(id7, array13, array14)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.SWAMPDELIGHTS.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 20
		};
		ComplexRecipe.RecipeElement[] array15 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("ColdWheatSeed", 3f)
		};
		ComplexRecipe.RecipeElement[] array16 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("ColdWheatBread", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id8 = ComplexRecipeManager.MakeRecipeID("CookingStation", array15, array16);
		ColdWheatBreadConfig.recipe = new ComplexRecipe(id8, array15, array16)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.COLDWHEATBREAD.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 50
		};
		ComplexRecipe.RecipeElement[] array17 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("RawEgg", 1f)
		};
		ComplexRecipe.RecipeElement[] array18 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("CookedEgg", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id9 = ComplexRecipeManager.MakeRecipeID("CookingStation", array17, array18);
		CookedEggConfig.recipe = new ComplexRecipe(id9, array17, array18)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.COOKEDEGG.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 1
		};
		ComplexRecipe.RecipeElement[] array19 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("WormBasicFruit", 1f)
		};
		ComplexRecipe.RecipeElement[] array20 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("WormBasicFood", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id10 = ComplexRecipeManager.MakeRecipeID("CookingStation", array19, array20);
		WormBasicFoodConfig.recipe = new ComplexRecipe(id10, array19, array20)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.WORMBASICFOOD.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 20
		};
		ComplexRecipe.RecipeElement[] array21 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("WormSuperFruit", 8f),
			new ComplexRecipe.RecipeElement("Sucrose".ToTag(), 4f)
		};
		ComplexRecipe.RecipeElement[] array22 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("WormSuperFood", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id11 = ComplexRecipeManager.MakeRecipeID("CookingStation", array21, array22);
		WormSuperFoodConfig.recipe = new ComplexRecipe(id11, array21, array22)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.WORMSUPERFOOD.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"CookingStation"
			},
			sortOrder = 20
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
