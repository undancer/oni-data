using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GlassForgeConfig : IBuildingConfig
{
	public const string ID = "GlassForge";

	private const float INPUT_KG = 100f;

	public static readonly CellOffset outPipeOffset = new CellOffset(1, 3);

	private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve
	};

	public static readonly List<Storage.StoredItemModifier> OutputItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Insulate
	};

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GlassForge", 5, 4, "glassrefinery_kanim", 30, 60f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.ALL_MINERALS, 2400f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER6, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER2);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 1200f;
		obj.SelfHeatKilowattsWhenActive = 16f;
		obj.OutputConduitType = ConduitType.Liquid;
		obj.UtilityOutputOffset = outPipeOffset;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "HollowMetal";
		obj.AudioSize = "large";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		GlassForge glassForge = go.AddOrGet<GlassForge>();
		glassForge.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		glassForge.duplicantOperated = true;
		BuildingTemplates.CreateComplexFabricatorStorage(go, glassForge);
		glassForge.outStorage.capacityKg = 2000f;
		glassForge.storeProduced = true;
		glassForge.inStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
		glassForge.buildStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
		glassForge.outStorage.SetDefaultStoredItemModifiers(OutputItemModifiers);
		glassForge.outputOffset = new Vector3(1f, 0.5f);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_metalrefinery_kanim") };
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.storage = glassForge.outStorage;
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = null;
		conduitDispenser.alwaysDispense = true;
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Sand).tag, 100f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.MoltenGlass).tag, 25f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted)
		};
		string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("GlassForge", array[0].material);
		string text = ComplexRecipeManager.MakeRecipeID("GlassForge", array, array2);
		new ComplexRecipe(text, array, array2)
		{
			time = 40f,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			description = string.Format(STRINGS.BUILDINGS.PREFABS.GLASSFORGE.RECIPE_DESCRIPTION, ElementLoader.GetElement(array2[0].material).name, ElementLoader.GetElement(array[0].material).name),
			fabricators = new List<Tag> { TagManager.Create("GlassForge") }
		};
		ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, text);
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SymbolOverrideControllerUtil.AddToPrefab(go);
		go.AddOrGetDef<PoweredActiveStoppableController.Def>();
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
