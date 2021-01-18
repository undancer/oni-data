using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GourmetCookingStationConfig : IBuildingConfig
{
	public const string ID = "GourmetCookingStation";

	private const float FUEL_STORE_CAPACITY = 10f;

	private const float FUEL_CONSUME_RATE = 0.1f;

	private const float CO2_EMIT_RATE = 0.025f;

	private Tag FUEL_TAG = new Tag("Methane");

	private static readonly List<Storage.StoredItemModifier> GourmetCookingStationStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GourmetCookingStation", 3, 3, "cookstation_gourmet_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: TUNING.BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateElectricalBuildingDef(obj);
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		obj.EnergyConsumptionWhenActive = 240f;
		obj.ExhaustKilowattsWhenActive = 1f;
		obj.SelfHeatKilowattsWhenActive = 8f;
		obj.InputConduitType = ConduitType.Gas;
		obj.UtilityInputOffset = new CellOffset(-1, 0);
		obj.PowerInputOffset = new CellOffset(1, 0);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		GourmetCookingStation gourmetCookingStation = go.AddOrGet<GourmetCookingStation>();
		gourmetCookingStation.resultState = ComplexFabricator.ResultState.Heated;
		gourmetCookingStation.heatedTemperature = 368.15f;
		gourmetCookingStation.duplicantOperated = true;
		gourmetCookingStation.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, gourmetCookingStation);
		gourmetCookingStation.fuelTag = FUEL_TAG;
		gourmetCookingStation.outStorage.capacityKg = 10f;
		gourmetCookingStation.inStorage.SetDefaultStoredItemModifiers(GourmetCookingStationStoredItemModifiers);
		gourmetCookingStation.buildStorage.SetDefaultStoredItemModifiers(GourmetCookingStationStoredItemModifiers);
		gourmetCookingStation.outStorage.SetDefaultStoredItemModifiers(GourmetCookingStationStoredItemModifiers);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.capacityTag = FUEL_TAG;
		conduitConsumer.capacityKG = 10f;
		conduitConsumer.alwaysConsume = true;
		conduitConsumer.storage = gourmetCookingStation.inStorage;
		conduitConsumer.forceAlwaysSatisfied = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(FUEL_TAG, 0.1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.025f, SimHashes.CarbonDioxide, 348.15f, useEntityTemperature: false, storeOutput: false, 0f, 3f)
		};
		ConfigureRecipes();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveStoppableController.Def>();
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		};
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("GrilledPrickleFruit", 2f),
			new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 2f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Salsa", 1f)
		};
		SalsaConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array, array2), array, array2)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.SALSA.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"GourmetCookingStation"
			},
			sortOrder = 300
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("FriedMushroom", 1f),
			new ComplexRecipe.RecipeElement("Lettuce", 4f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("MushroomWrap", 1f)
		};
		MushroomWrapConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array3, array4), array3, array4)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.MUSHROOMWRAP.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"GourmetCookingStation"
			},
			sortOrder = 400
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("CookedMeat", 1f),
			new ComplexRecipe.RecipeElement("CookedFish", 1f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("SurfAndTurf", 1f)
		};
		SurfAndTurfConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array5, array6), array5, array6)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.SURFANDTURF.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"GourmetCookingStation"
			},
			sortOrder = 500
		};
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("ColdWheatSeed", 10f),
			new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("SpiceBread", 1f)
		};
		SpiceBreadConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array7, array8), array7, array8)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.SPICEBREAD.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"GourmetCookingStation"
			},
			sortOrder = 600
		};
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("Tofu", 1f),
			new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("SpicyTofu", 1f)
		};
		SpicyTofuConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array9, array10), array9, array10)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.SPICYTOFU.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"GourmetCookingStation"
			},
			sortOrder = 800
		};
		ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[3]
		{
			new ComplexRecipe.RecipeElement("ColdWheatBread", 1f),
			new ComplexRecipe.RecipeElement("Lettuce", 1f),
			new ComplexRecipe.RecipeElement("CookedMeat", 1f)
		};
		ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Burger", 1f)
		};
		BurgerConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array11, array12), array11, array12)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.BURGER.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag>
			{
				"GourmetCookingStation"
			},
			sortOrder = 900
		};
	}
}
