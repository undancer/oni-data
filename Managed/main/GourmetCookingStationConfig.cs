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
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GourmetCookingStation", 3, 3, "cookstation_gourmet_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: TUNING.BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 1f;
		buildingDef.SelfHeatKilowattsWhenActive = 8f;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		GourmetCookingStation gourmetCookingStation = go.AddOrGet<GourmetCookingStation>();
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
			new ComplexRecipe.RecipeElement("Salsa", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array, array2);
		SalsaConfig.recipe = new ComplexRecipe(id, array, array2)
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
			new ComplexRecipe.RecipeElement("MushroomWrap", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array3, array4);
		MushroomWrapConfig.recipe = new ComplexRecipe(id2, array3, array4)
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
			new ComplexRecipe.RecipeElement("SurfAndTurf", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id3 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array5, array6);
		SurfAndTurfConfig.recipe = new ComplexRecipe(id3, array5, array6)
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
			new ComplexRecipe.RecipeElement("SpiceBread", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id4 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array7, array8);
		SpiceBreadConfig.recipe = new ComplexRecipe(id4, array7, array8)
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
			new ComplexRecipe.RecipeElement("SpicyTofu", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id5 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array9, array10);
		SpicyTofuConfig.recipe = new ComplexRecipe(id5, array9, array10)
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
			new ComplexRecipe.RecipeElement("Burger", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id6 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array11, array12);
		BurgerConfig.recipe = new ComplexRecipe(id6, array11, array12)
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
		if (DlcManager.IsExpansion1Active())
		{
			ComplexRecipe.RecipeElement[] array13 = new ComplexRecipe.RecipeElement[3]
			{
				new ComplexRecipe.RecipeElement("ColdWheatSeed", 3f),
				new ComplexRecipe.RecipeElement("WormSuperFruit", 4f),
				new ComplexRecipe.RecipeElement("GrilledPrickleFruit", 1f)
			};
			ComplexRecipe.RecipeElement[] array14 = new ComplexRecipe.RecipeElement[1]
			{
				new ComplexRecipe.RecipeElement("BerryPie", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			};
			string id7 = ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", array13, array14);
			BerryPieConfig.recipe = new ComplexRecipe(id7, array13, array14)
			{
				time = FOOD.RECIPES.STANDARD_COOK_TIME,
				description = ITEMS.FOOD.BERRYPIE.RECIPEDESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag>
				{
					"GourmetCookingStation"
				},
				sortOrder = 900
			};
		}
	}
}
