using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BeeConfig : IEntityConfig
{
	public const string ID = "Bee";

	public const string BASE_TRAIT_ID = "BeeBaseTrait";

	public static GameObject CreateBee(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseBeeConfig.BaseBee(id, name, desc, anim_file, "BeeBaseTrait", DECOR.BONUS.TIER4, is_baby);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, BeeTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("BeeBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 5f, name));
		return gameObject;
	}

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		return CreateBee("Bee", STRINGS.CREATURES.SPECIES.BEE.NAME, STRINGS.CREATURES.SPECIES.BEE.DESC, "bee_kanim", is_baby: false);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BaseBeeConfig.SetupLoopingSounds(inst);
	}
}
