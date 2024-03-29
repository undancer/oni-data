using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class MoleConfig : IEntityConfig
{
	public const string ID = "Mole";

	public const string BASE_TRAIT_ID = "MoleBaseTrait";

	public const string EGG_ID = "MoleEgg";

	private static float MIN_POOP_SIZE_IN_CALORIES = 2400000f;

	private static float CALORIES_PER_KG_OF_DIRT = 1000f;

	public static int EGG_SORT_ORDER = 800;

	public static GameObject CreateMole(string id, string name, string desc, string anim_file, bool is_baby = false)
	{
		GameObject gameObject = BaseMoleConfig.BaseMole(id, name, STRINGS.CREATURES.SPECIES.MOLE.DESC, "MoleBaseTrait", anim_file, is_baby);
		gameObject.AddTag(GameTags.Creatures.Digger);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, MoleTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("MoleBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, MoleTuning.STANDARD_STOMACH_SIZE, name));
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
		HashedString[] gINGER_SYMBOL_NAMES = MoleTuning.GINGER_SYMBOL_NAMES;
		foreach (HashedString hashedString in gINGER_SYMBOL_NAMES)
		{
			gameObject.GetComponent<KAnimControllerBase>().SetSymbolVisiblity(hashedString, is_visible: false);
		}
		return gameObject;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(CreateMole("Mole", STRINGS.CREATURES.SPECIES.MOLE.NAME, STRINGS.CREATURES.SPECIES.MOLE.DESC, "driller_kanim"), "MoleEgg", STRINGS.CREATURES.SPECIES.MOLE.EGG_NAME, STRINGS.CREATURES.SPECIES.MOLE.DESC, "egg_driller_kanim", MoleTuning.EGG_MASS, "MoleBaby", 60.000004f, 20f, eggSortOrder: EGG_SORT_ORDER, egg_chances: MoleTuning.EGG_CHANCES_BASE);
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
