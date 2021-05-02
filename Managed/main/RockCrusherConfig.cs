using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class RockCrusherConfig : IBuildingConfig
{
	public const string ID = "RockCrusher";

	private const float INPUT_KG = 100f;

	private const float METAL_ORE_EFFICIENCY = 0.5f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RockCrusher", 4, 4, "rockrefinery_kanim", 30, 60f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.ALL_METALS, 2400f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER6, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER2);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.SelfHeatKilowattsWhenActive = 16f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		complexFabricator.duplicantOperated = true;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_rockrefinery_kanim")
		};
		complexFabricatorWorkable.workingPstComplete = new HashedString[1]
		{
			"working_pst_complete"
		};
		Tag tag = SimHashes.Sand.CreateTag();
		List<Element> list = ElementLoader.elements.FindAll((Element e) => e.HasTag(GameTags.Crushable));
		foreach (Element item in list)
		{
			ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
			{
				new ComplexRecipe.RecipeElement(item.tag, 100f)
			};
			ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
			{
				new ComplexRecipe.RecipeElement(tag, 100f)
			};
			string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", item.tag);
			string text = ComplexRecipeManager.MakeRecipeID("RockCrusher", array, array2);
			new ComplexRecipe(text, array, array2)
			{
				time = 40f,
				description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, item.name, tag.ProperName()),
				nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
				fabricators = new List<Tag>
				{
					TagManager.Create("RockCrusher")
				}
			};
			ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, text);
		}
		List<Element> list2 = ElementLoader.elements.FindAll((Element e) => e.IsSolid && e.HasTag(GameTags.Metal));
		foreach (Element item2 in list2)
		{
			if (!item2.HasTag(GameTags.Noncrushable))
			{
				Element highTempTransition = item2.highTempTransition;
				Element lowTempTransition = highTempTransition.lowTempTransition;
				if (lowTempTransition != item2)
				{
					ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[1]
					{
						new ComplexRecipe.RecipeElement(item2.tag, 100f)
					};
					ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[2]
					{
						new ComplexRecipe.RecipeElement(lowTempTransition.tag, 50f),
						new ComplexRecipe.RecipeElement(tag, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
					};
					string obsolete_id2 = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", lowTempTransition.tag);
					string text2 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array3, array4);
					new ComplexRecipe(text2, array3, array4)
					{
						time = 40f,
						description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.METAL_RECIPE_DESCRIPTION, lowTempTransition.name, item2.name),
						nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
						fabricators = new List<Tag>
						{
							TagManager.Create("RockCrusher")
						}
					};
					ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id2, text2);
				}
			}
		}
		Element element = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("EggShell", 5f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		string obsolete_id3 = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", element.tag);
		string text3 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array5, array6);
		new ComplexRecipe(text3, array5, array6)
		{
			time = 40f,
			description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), MISC.TAGS.EGGSHELL),
			nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
			fabricators = new List<Tag>
			{
				TagManager.Create("RockCrusher")
			}
		};
		ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id3, text3);
		Element element2 = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BabyCrabShell", 1f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(element2.tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		string id = ComplexRecipeManager.MakeRecipeID("RockCrusher", array7, array8);
		new ComplexRecipe(id, array7, array8)
		{
			time = 40f,
			description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME),
			nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
			fabricators = new List<Tag>
			{
				TagManager.Create("RockCrusher")
			}
		};
		Element element3 = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("CrabShell", 1f)
		};
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(element3.tag, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		string id2 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array9, array10);
		new ComplexRecipe(id2, array9, array10)
		{
			time = 40f,
			description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME),
			nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
			fabricators = new List<Tag>
			{
				TagManager.Create("RockCrusher")
			}
		};
		ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Fossil).tag, 100f)
		};
		ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.SedimentaryRock).tag, 95f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		string id3 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array11, array12);
		new ComplexRecipe(id3, array11, array12)
		{
			time = 40f,
			description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_FROM_LIMESTONE_RECIPE_DESCRIPTION, SimHashes.Fossil.CreateTag().ProperName(), SimHashes.SedimentaryRock.CreateTag().ProperName(), SimHashes.Lime.CreateTag().ProperName()),
			nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
			fabricators = new List<Tag>
			{
				TagManager.Create("RockCrusher")
			}
		};
		float num = 5E-05f;
		ComplexRecipe.RecipeElement[] array13 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array14 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(TableSaltConfig.ID.ToTag(), 100f * num, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
			new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 100f * (1f - num), ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		string id4 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array13, array14);
		new ComplexRecipe(id4, array13, array14)
		{
			time = 40f,
			description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, SimHashes.Salt.CreateTag().ProperName(), ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.NAME),
			nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
			fabricators = new List<Tag>
			{
				TagManager.Create("RockCrusher")
			}
		};
		float num2 = 0.9f;
		ComplexRecipe.RecipeElement[] array15 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Fullerene.CreateTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array16 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Graphite.CreateTag(), 100f * num2, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
			new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 100f * (1f - num2), ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		string id5 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array15, array16);
		new ComplexRecipe(id5, array15, array16)
		{
			time = 40f,
			description = string.Format(STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, SimHashes.Fullerene.CreateTag().ProperName(), SimHashes.Graphite.CreateTag().ProperName()),
			nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
			fabricators = new List<Tag>
			{
				TagManager.Create("RockCrusher")
			}
		};
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SymbolOverrideControllerUtil.AddToPrefab(go);
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		};
	}
}
