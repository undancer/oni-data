using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ManualHighEnergyParticleSpawnerConfig : IBuildingConfig
{
	public const string ID = "ManualHighEnergyParticleSpawner";

	public const float MIN_LAUNCH_INTERVAL = 2f;

	public const int MIN_SLIDER = 1;

	public const int MAX_SLIDER = 100;

	public const float RADBOLTS_PER_KG = 5f;

	public const float MASS_PER_CRAFT = 1f;

	public const float REFINED_BONUS = 5f;

	public const int RADBOLTS_PER_CRAFT = 5;

	public static readonly Tag WASTE_MATERIAL = SimHashes.DepletedUranium.CreateTag();

	private const float ORE_FUEL_TO_WASTE_RATIO = 0.5f;

	private const float REFINED_FUEL_TO_WASTE_RATIO = 0.8f;

	private short RAD_LIGHT_SIZE = 3;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("ManualHighEnergyParticleSpawner", 1, 3, "manual_radbolt_generator_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER1);
		obj.Floodable = false;
		obj.AudioCategory = "Metal";
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.Radiation.ID;
		obj.UseHighEnergyParticleOutputPort = true;
		obj.HighEnergyParticleOutputOffset = new CellOffset(0, 2);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.RadiationIDs, "ManualHighEnergyParticleSpawner");
		obj.Deprecated = !Sim.IsRadiationEnabled();
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
		go.AddOrGet<HighEnergyParticleStorage>();
		go.AddOrGet<LoopingSounds>();
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		complexFabricator.duplicantOperated = true;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_manual_radbolt_generator_kanim") };
		complexFabricatorWorkable.workLayer = Grid.SceneLayer.BuildingUse;
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SimHashes.UraniumOre.CreateTag(), 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(WASTE_MATERIAL, 0.5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ManualHighEnergyParticleSpawner", array, array2), array, array2, 0, 5)
		{
			time = 40f,
			description = string.Format(STRINGS.BUILDINGS.PREFABS.MANUALHIGHENERGYPARTICLESPAWNER.RECIPE_DESCRIPTION, SimHashes.UraniumOre.CreateTag().ProperName(), WASTE_MATERIAL.ProperName()),
			nameDisplay = ComplexRecipe.RecipeNameDisplay.HEP,
			fabricators = new List<Tag> { TagManager.Create("ManualHighEnergyParticleSpawner") }
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SimHashes.EnrichedUranium.CreateTag(), 1f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(WASTE_MATERIAL, 0.8f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ManualHighEnergyParticleSpawner", array3, array4), array3, array4, 0, 25)
		{
			time = 40f,
			description = string.Format(STRINGS.BUILDINGS.PREFABS.MANUALHIGHENERGYPARTICLESPAWNER.RECIPE_DESCRIPTION, SimHashes.EnrichedUranium.CreateTag().ProperName(), WASTE_MATERIAL.ProperName()),
			nameDisplay = ComplexRecipe.RecipeNameDisplay.HEP,
			fabricators = new List<Tag> { TagManager.Create("ManualHighEnergyParticleSpawner") }
		};
		go.AddOrGet<ManualHighEnergyParticleSpawner>();
		RadiationEmitter radiationEmitter = go.AddComponent<RadiationEmitter>();
		radiationEmitter.emissionOffset = new Vector3(0f, 2f);
		radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
		radiationEmitter.emitRadiusX = RAD_LIGHT_SIZE;
		radiationEmitter.emitRadiusY = RAD_LIGHT_SIZE;
		radiationEmitter.emitRads = 120f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
