using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class AtmoSuitConfig : IEquipmentConfig
{
	public const string ID = "Atmo_Suit";

	public static ComplexRecipe recipe;

	private const PathFinder.PotentialPath.Flags suit_flags = PathFinder.PotentialPath.Flags.HasAtmoSuit;

	private AttributeModifier expertAthleticsModifier;

	public EquipmentDef CreateEquipmentDef()
	{
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
		list.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, TUNING.EQUIPMENT.SUITS.ATMOSUIT_DIGGING, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
		list.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
		expertAthleticsModifier = new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, -TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, Db.Get().Skills.Suits1.Name);
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Atmo_Suit", TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Dirt, TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS, "suit_oxygen_kanim", "", "body_oxygen_kanim", 6, list, null, IsBody: true, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, new Tag[1]
		{
			GameTags.Suit
		});
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("SoakingWet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("PoppedEarDrums"));
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			Ownables soleOwner2 = eq.assignee.GetSoleOwner();
			if (soleOwner2 != null)
			{
				GameObject targetGameObject2 = soleOwner2.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				Navigator component3 = targetGameObject2.GetComponent<Navigator>();
				if (component3 != null)
				{
					component3.SetFlags(PathFinder.PotentialPath.Flags.HasAtmoSuit);
				}
				MinionResume component4 = targetGameObject2.GetComponent<MinionResume>();
				if (component4 != null && component4.HasPerk(Db.Get().SkillPerks.ExosuitExpertise.Id))
				{
					targetGameObject2.GetAttributes().Get(Db.Get().Attributes.Athletics).Add(expertAthleticsModifier);
				}
			}
		};
		equipmentDef.OnUnequipCallBack = delegate(Equippable eq)
		{
			if (eq.assignee != null)
			{
				Ownables soleOwner = eq.assignee.GetSoleOwner();
				if (soleOwner != null)
				{
					GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
					if ((bool)targetGameObject)
					{
						targetGameObject.GetAttributes()?.Get(Db.Get().Attributes.Athletics).Remove(expertAthleticsModifier);
						Navigator component = targetGameObject.GetComponent<Navigator>();
						if (component != null)
						{
							component.ClearFlags(PathFinder.PotentialPath.Flags.HasAtmoSuit);
						}
						Effects component2 = targetGameObject.GetComponent<Effects>();
						if (component2 != null && component2.HasEffect("SoiledSuit"))
						{
							component2.Remove("SoiledSuit");
						}
					}
					eq.GetComponent<Storage>().DropAll(eq.transform.GetPosition(), vent_gas: true, dump_liquid: true, default(Vector3), do_disease_transfer: false);
				}
			}
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Atmo_Suit");
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Oxygen";
		suitTank.capacity = 75f;
		go.AddComponent<HelmetController>();
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Clothes);
		component.AddTag(GameTags.PedestalDisplayable);
		component.AddTag(GameTags.AirtightSuit);
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		storage.showInUI = true;
		go.AddOrGet<AtmoSuit>();
		go.AddComponent<SuitDiseaseHandler>();
	}
}
