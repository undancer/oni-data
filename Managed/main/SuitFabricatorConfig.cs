using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SuitFabricatorConfig : IBuildingConfig
{
	public const string ID = "SuitFabricator";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SuitFabricator", 4, 3, "suit_maker_kanim", 100, 240f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: TUNING.BUILDINGS.DECOR.NONE);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<Prioritizable>();
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
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
			new ComplexRecipe.RecipeElement(SimHashes.Copper.CreateTag(), 300f, inheritElement: true),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array, array2);
		AtmoSuitConfig.recipe = new ComplexRecipe(id, array, array2)
		{
			time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag>
			{
				"SuitFabricator"
			},
			requiredTech = Db.Get().TechItems.atmoSuit.parentTechId
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), 300f, inheritElement: true),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array3, array4);
		AtmoSuitConfig.recipe = new ComplexRecipe(id2, array3, array4)
		{
			time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag>
			{
				"SuitFabricator"
			},
			requiredTech = Db.Get().TechItems.atmoSuit.parentTechId
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 300f, inheritElement: true),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id3 = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array5, array6);
		AtmoSuitConfig.recipe = new ComplexRecipe(id3, array5, array6)
		{
			time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag>
			{
				"SuitFabricator"
			},
			requiredTech = Db.Get().TechItems.atmoSuit.parentTechId
		};
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("Worn_Atmo_Suit".ToTag(), 1f, inheritElement: true),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 1f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id4 = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array7, array8);
		AtmoSuitConfig.recipe = new ComplexRecipe(id4, array7, array8)
		{
			time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag>
			{
				"SuitFabricator"
			},
			requiredTech = Db.Get().TechItems.atmoSuit.parentTechId
		};
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Steel.ToString(), 200f),
			new ComplexRecipe.RecipeElement(SimHashes.Petroleum.ToString(), 25f)
		};
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Jet_Suit".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id5 = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array9, array10);
		JetSuitConfig.recipe = new ComplexRecipe(id5, array9, array10)
		{
			time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag>
			{
				"SuitFabricator"
			},
			requiredTech = Db.Get().TechItems.jetSuit.parentTechId
		};
		ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("Worn_Jet_Suit".ToTag(), 1f),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 1f)
		};
		ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Jet_Suit".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
		};
		string id6 = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array11, array12);
		JetSuitConfig.recipe = new ComplexRecipe(id6, array11, array12)
		{
			time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag>
			{
				"SuitFabricator"
			},
			requiredTech = Db.Get().TechItems.jetSuit.parentTechId
		};
		if (Sim.IsRadiationEnabled())
		{
			ComplexRecipe.RecipeElement[] array13 = new ComplexRecipe.RecipeElement[2]
			{
				new ComplexRecipe.RecipeElement("Worn_Lead_Suit".ToTag(), 1f),
				new ComplexRecipe.RecipeElement(SimHashes.Glass.ToString(), 5f)
			};
			ComplexRecipe.RecipeElement[] array14 = new ComplexRecipe.RecipeElement[1]
			{
				new ComplexRecipe.RecipeElement("Lead_Suit".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			};
			string id7 = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array13, array14);
			LeadSuitConfig.recipe = new ComplexRecipe(id7, array13, array14)
			{
				time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
				description = STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.RECIPE_DESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
				fabricators = new List<Tag>
				{
					"SuitFabricator"
				},
				requiredTech = Db.Get().TechItems.leadSuit.parentTechId
			};
		}
		if (Sim.IsRadiationEnabled())
		{
			ComplexRecipe.RecipeElement[] array15 = new ComplexRecipe.RecipeElement[2]
			{
				new ComplexRecipe.RecipeElement(SimHashes.Lead.ToString(), 200f),
				new ComplexRecipe.RecipeElement(SimHashes.Glass.ToString(), 10f)
			};
			ComplexRecipe.RecipeElement[] array16 = new ComplexRecipe.RecipeElement[1]
			{
				new ComplexRecipe.RecipeElement("Lead_Suit".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			};
			string id8 = ComplexRecipeManager.MakeRecipeID("SuitFabricator", array15, array16);
			LeadSuitConfig.recipe = new ComplexRecipe(id8, array15, array16)
			{
				time = TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
				description = STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.RECIPE_DESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
				fabricators = new List<Tag>
				{
					"SuitFabricator"
				},
				requiredTech = Db.Get().TechItems.leadSuit.parentTechId
			};
		}
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
