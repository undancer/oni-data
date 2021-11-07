using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class DiamondPressConfig : IBuildingConfig
{
	public const string ID = "DiamondPress";

	public const string PORT_ID = "HEP_STORAGE";

	private const int HEP_PER_DIAMOND_KG = 10;

	private const int RECIPE_MASS_KG = 100;

	private const int HEP_STORAGE_CAPACITY = 2000;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("DiamondPress", 3, 5, "diamond_press_kanim", 30, 60f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.ALL_METALS, 2400f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER6, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER2);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 240f;
		obj.SelfHeatKilowattsWhenActive = 16f;
		obj.UseHighEnergyParticleInputPort = true;
		obj.HighEnergyParticleInputOffset = new CellOffset(0, 2);
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "HollowMetal";
		obj.AudioSize = "large";
		obj.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort("HEP_STORAGE", new CellOffset(0, 2), STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE, STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_ACTIVE, STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_INACTIVE) };
		return obj;
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
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.capacity = 2000f;
		highEnergyParticleStorage.autoStore = true;
		highEnergyParticleStorage.PORT_ID = "HEP_STORAGE";
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_diamond_press_kanim") };
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SimHashes.RefinedCarbon.CreateTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Diamond.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
		};
		new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("DiamondPress", array, array2), array, array2, 1000, 0)
		{
			time = 80f,
			description = string.Format(STRINGS.BUILDINGS.PREFABS.DIAMONDPRESS.REFINED_CARBON_RECIPE_DESCRIPTION, SimHashes.Diamond.CreateTag().ProperName(), SimHashes.RefinedCarbon.CreateTag().ProperName()),
			nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
			fabricators = new List<Tag> { TagManager.Create("DiamondPress") }
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
			MeterController meter = new MeterController(component.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
			HighEnergyParticleStorage hepStorage = component.GetComponent<HighEnergyParticleStorage>();
			component.Subscribe(-1837862626, delegate
			{
				meter.SetPositionPercent(hepStorage.Particles / hepStorage.Capacity());
			});
			meter.SetPositionPercent(hepStorage.Particles / hepStorage.Capacity());
		};
	}
}
