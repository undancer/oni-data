using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class TemperatureSuitConfig : IEquipmentConfig
{
	public const string ID = "Temperature_Suit";

	public EquipmentDef CreateEquipmentDef()
	{
		new Dictionary<string, float>().Add(SimHashes.Ice.ToString(), 300f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_INSULATION, STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.NAME));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_ATHLETICS, STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.NAME));
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Temperature_Suit", TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Water, TUNING.EQUIPMENT.SUITS.TEMPERATURESUIT_MASS, TUNING.EQUIPMENT.SUITS.ANIM, TUNING.EQUIPMENT.SUITS.SNAPON, "body_oxygen_kanim", 6, list, null, IsBody: false, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, new Tag[1]
		{
			GameTags.Suit
		});
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.TEMPERATURE_SUIT.RECIPE_DESC;
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Water";
		suitTank.amount = 100f;
		go.GetComponent<KPrefabID>().AddTag(GameTags.PedestalDisplayable);
	}
}
