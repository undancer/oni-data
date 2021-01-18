using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class AquaSuitConfig : IEquipmentConfig
{
	public const string ID = "Aqua_Suit";

	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add(SimHashes.DirtyWater.ToString(), 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, TUNING.EQUIPMENT.SUITS.AQUASUIT_INSULATION, STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, TUNING.EQUIPMENT.SUITS.AQUASUIT_ATHLETICS, STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.MAX_UNDERWATER_TRAVELCOST, TUNING.EQUIPMENT.SUITS.AQUASUIT_UNDERWATER_TRAVELCOST, STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.NAME));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Aqua_Suit", TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Water, TUNING.EQUIPMENT.SUITS.AQUASUIT_MASS, "suit_water_slow_kanim", TUNING.EQUIPMENT.SUITS.SNAPON, "body_water_slow_kanim", 6, list, null, IsBody: false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, new Tag[1]
		{
			GameTags.Suit
		});
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.AQUA_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.underwaterSupport = true;
		suitTank.element = "Oxygen";
		suitTank.amount = 11f;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes);
	}
}
