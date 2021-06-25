using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SludgePressConfig : IBuildingConfig
{
	public const string ID = "SludgePress";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SludgePress", 4, 3, "sludge_press_kanim", 100, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
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
			Assets.GetAnim("anim_interacts_sludge_press_kanim")
		};
		complexFabricatorWorkable.workingPstComplete = new HashedString[1]
		{
			"working_pst_complete"
		};
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.elementFilter = null;
		conduitDispenser.storage = go.GetComponent<ComplexFabricator>().outStorage;
		AddRecipes(go);
		Prioritizable.AddRef(go);
	}

	private void AddRecipes(GameObject go)
	{
		float num = 150f;
		List<Element> list = ElementLoader.elements.FindAll((Element e) => e.elementComposition != null);
		foreach (Element item in list)
		{
			ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
			{
				new ComplexRecipe.RecipeElement(item.tag, num)
			};
			ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[item.elementComposition.Length];
			for (int i = 0; i < item.elementComposition.Length; i++)
			{
				ElementLoader.ElementComposition elementComposition = item.elementComposition[i];
				Element element = ElementLoader.FindElementByName(elementComposition.elementID);
				bool isLiquid = element.IsLiquid;
				array2[i] = new ComplexRecipe.RecipeElement(element.tag, num * elementComposition.percentage, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, isLiquid);
			}
			string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("SludgePress", item.tag);
			string text = ComplexRecipeManager.MakeRecipeID("SludgePress", array, array2);
			new ComplexRecipe(text, array, array2)
			{
				time = 20f,
				description = string.Format(STRINGS.BUILDINGS.PREFABS.SLUDGEPRESS.RECIPE_DESCRIPTION, item.name),
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Composite,
				fabricators = new List<Tag>
				{
					TagManager.Create("SludgePress")
				}
			};
			ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, text);
		}
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
