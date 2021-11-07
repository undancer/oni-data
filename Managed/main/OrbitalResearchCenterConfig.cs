using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class OrbitalResearchCenterConfig : IBuildingConfig
{
	public const string ID = "OrbitalResearchCenter";

	public const float BASE_SECONDS_PER_POINT = 33f;

	public const float MASS_PER_POINT = 5f;

	public static readonly Tag INPUT_MATERIAL = SimHashes.Polypropylene.CreateTag();

	public const float OUTPUT_TEMPERATURE = 308.15f;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("OrbitalResearchCenter", 2, 3, "orbital_research_station_kanim", 30, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.PLASTICS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: TUNING.BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 60f;
		obj.ExhaustKilowattsWhenActive = 0.125f;
		obj.SelfHeatKilowattsWhenActive = 0.5f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.RocketInteriorBuilding);
		go.AddOrGet<InOrbitRequired>();
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<Prioritizable>();
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.heatedTemperature = 308.15f;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_orbital_research_station_kanim") };
		Prioritizable.AddRef(go);
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		ConfigureRecipes();
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(INPUT_MATERIAL, 5f, inheritElement: true)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("OrbitalResearchDatabank".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("OrbitalResearchCenter", array, array2), array, array2)
		{
			time = 33f,
			description = ITEMS.INDUSTRIAL_PRODUCTS.ORBITAL_RESEARCH_DATABANK.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "OrbitalResearchCenter" }
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
