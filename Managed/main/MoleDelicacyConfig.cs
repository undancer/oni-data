using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class MoleDelicacyConfig : IEntityConfig
{
	public const string ID = "MoleDelicacy";

	public const string BASE_TRAIT_ID = "MoleDelicacyBaseTrait";

	public const string EGG_ID = "MoleDelicacyEgg";

	private static float MIN_POOP_SIZE_IN_CALORIES = 2400000f;

	private static float CALORIES_PER_KG_OF_DIRT = 1000f;

	public static int EGG_SORT_ORDER = 800;

	public static float GINGER_GROWTH_TIME_IN_CYCLES = 8f;

	public static float GINGER_PER_CYCLE = 1f;

	public static Tag SHEAR_DROP_ELEMENT = GingerConfig.ID;

	public static float MIN_GROWTH_TEMPERATURE = 343.15f;

	public static float MAX_GROWTH_TEMPERATURE = 353.15f;

	public static float EGG_CHANCES_TEMPERATURE_MIN = 333.15f;

	public static float EGG_CHANCES_TEMPERATURE_MAX = 373.15f;

	public static GameObject CreateMole(string id, string name, string desc, string anim_file, bool is_baby = false)
	{
		GameObject gameObject = BaseMoleConfig.BaseMole(id, name, desc, "MoleDelicacyBaseTrait", anim_file, is_baby, "del_", 5);
		gameObject.AddTag(GameTags.Creatures.Digger);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, MoleTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("MoleDelicacyBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, MoleTuning.DELICACY_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - MoleTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
		Diet diet = new Diet(BaseMoleConfig.SimpleOreDiet(new List<Tag>
		{
			SimHashes.Regolith.CreateTag(),
			SimHashes.Dirt.CreateTag(),
			SimHashes.IronOre.CreateTag()
		}, CALORIES_PER_KG_OF_DIRT, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL).ToArray());
		CreatureCalorieMonitor.Def def = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = MIN_POOP_SIZE_IN_CALORIES;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		gameObject.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = 0;
		gameObject.AddOrGet<LoopingSounds>();
		if (!is_baby)
		{
			ElementGrowthMonitor.Def def2 = gameObject.AddOrGetDef<ElementGrowthMonitor.Def>();
			def2.defaultGrowthRate = 1f / GINGER_GROWTH_TIME_IN_CYCLES / 600f;
			def2.dropMass = GINGER_PER_CYCLE * GINGER_GROWTH_TIME_IN_CYCLES;
			def2.itemDroppedOnShear = SHEAR_DROP_ELEMENT;
			def2.levelCount = 5;
			def2.minTemperature = MIN_GROWTH_TEMPERATURE;
			def2.maxTemperature = MAX_GROWTH_TEMPERATURE;
		}
		else
		{
			gameObject.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.ElementGrowth.Id);
		}
		return gameObject;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(CreateMole("MoleDelicacy", STRINGS.CREATURES.SPECIES.MOLE.VARIANT_DELICACY.NAME, STRINGS.CREATURES.SPECIES.MOLE.VARIANT_DELICACY.DESC, "driller_kanim"), "MoleDelicacyEgg", STRINGS.CREATURES.SPECIES.MOLE.VARIANT_DELICACY.EGG_NAME, STRINGS.CREATURES.SPECIES.MOLE.VARIANT_DELICACY.DESC, "egg_driller_kanim", MoleTuning.EGG_MASS, "MoleDelicacyBaby", 60.000004f, 20f, eggSortOrder: EGG_SORT_ORDER, egg_chances: MoleTuning.EGG_CHANCES_DELICACY);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		SetSpawnNavType(inst);
	}

	public static void SetSpawnNavType(GameObject inst)
	{
		int cell = Grid.PosToCell(inst);
		Navigator component = inst.GetComponent<Navigator>();
		if (component != null)
		{
			if (Grid.IsSolidCell(cell))
			{
				component.SetCurrentNavType(NavType.Solid);
				inst.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.FXFront));
				inst.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.FXFront);
			}
			else
			{
				inst.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
			}
		}
	}
}
