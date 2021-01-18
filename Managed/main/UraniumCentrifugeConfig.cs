using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class UraniumCentrifugeConfig : IBuildingConfig
{
	public const string ID = "UraniumCentrifuge";

	public const float OUTPUT_TEMP = 1573.15f;

	public const float REFILL_RATE = 2400f;

	public static readonly CellOffset outPipeOffset = new CellOffset(1, 3);

	private static readonly List<Storage.StoredItemModifier> storedItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Insulate
	};

	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[2]
		{
			"RefinedMetal",
			"Plastic"
		};
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
			TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
		}, construction_materials: array, melting_point: 2400f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, id: "UraniumCentrifuge", width: 3, height: 4, anim: "enrichmentCentrifuge_kanim", hitpoints: 100, construction_time: 480f, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityOutputOffset = outPipeOffset;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.Deprecated = !Sim.IsRadiationEnabled();
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		UraniumCentrifuge uraniumCentrifuge = go.AddOrGet<UraniumCentrifuge>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, uraniumCentrifuge);
		uraniumCentrifuge.outStorage.capacityKg = 2000f;
		uraniumCentrifuge.storeProduced = true;
		uraniumCentrifuge.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		uraniumCentrifuge.duplicantOperated = false;
		uraniumCentrifuge.inStorage.SetDefaultStoredItemModifiers(storedItemModifiers);
		uraniumCentrifuge.buildStorage.SetDefaultStoredItemModifiers(storedItemModifiers);
		uraniumCentrifuge.outStorage.SetDefaultStoredItemModifiers(storedItemModifiers);
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.storage = uraniumCentrifuge.outStorage;
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.UraniumOre).tag, 100f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.EnrichedUranium).tag, 2f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.MoltenUranium).tag, 98f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted)
		};
		string id = ComplexRecipeManager.MakeRecipeID("UraniumCentrifuge", array, array2);
		new ComplexRecipe(id, array, array2)
		{
			time = 40f,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			description = STRINGS.BUILDINGS.PREFABS.URANIUMCENTRIFUGE.RECIPE_DESCRIPTION,
			fabricators = new List<Tag>
			{
				TagManager.Create("UraniumCentrifuge")
			}
		};
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
