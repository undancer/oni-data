using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class LeadSuitConfig : IEquipmentConfig
{
	public const string ID = "Lead_Suit";

	public const string WORN_ID = "Worn_Lead_Suit";

	public static ComplexRecipe recipe;

	private const PathFinder.PotentialPath.Flags suit_flags = PathFinder.PotentialPath.Flags.HasAtmoSuit;

	private AttributeModifier expertAthleticsModifier;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public EquipmentDef CreateEquipmentDef()
	{
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, TUNING.EQUIPMENT.SUITS.LEADSUIT_ATHLETICS, STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME));
		list.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, TUNING.EQUIPMENT.SUITS.LEADSUIT_SCALDING, STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME));
		list.Add(new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, TUNING.EQUIPMENT.SUITS.LEADSUIT_RADIATION_SHIELDING, STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME));
		list.Add(new AttributeModifier(Db.Get().Attributes.Strength.Id, TUNING.EQUIPMENT.SUITS.LEADSUIT_STRENGTH, STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, TUNING.EQUIPMENT.SUITS.LEADSUIT_INSULATION, STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, TUNING.EQUIPMENT.SUITS.LEADSUIT_THERMAL_CONDUCTIVITY_BARRIER, STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.NAME));
		expertAthleticsModifier = new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, -TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, Db.Get().Skills.Suits1.Name);
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Lead_Suit", TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Dirt, TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS, "suit_leadsuit_kanim", "", "body_leadsuit_kanim", 6, list, null, IsBody: true, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, new Tag[2]
		{
			GameTags.Suit,
			GameTags.Clothes
		});
		equipmentDef.wornID = "Worn_Lead_Suit";
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
					TagBits any_tags = new TagBits(eq.GetComponent<SuitTank>().elementTag);
					TagBits tagBits = default(TagBits);
					eq.GetComponent<Storage>().DropUnlessHasTags(any_tags, tagBits, tagBits, do_disease_transfer: true, dumpElements: true);
				}
			}
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Lead_Suit");
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Oxygen";
		suitTank.capacity = 40f;
		suitTank.elementTag = GameTags.Breathable;
		go.AddComponent<LeadSuitTank>().batteryDuration = 200f;
		go.AddComponent<HelmetController>();
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Clothes);
		component.AddTag(GameTags.PedestalDisplayable);
		component.AddTag(GameTags.AirtightSuit);
		Durability durability = go.AddComponent<Durability>();
		durability.wornEquipmentPrefabID = "Worn_Lead_Suit";
		durability.durabilityLossPerCycle = TUNING.EQUIPMENT.SUITS.ATMOSUIT_DECAY;
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		storage.showInUI = true;
		go.AddOrGet<AtmoSuit>();
		go.AddComponent<SuitDiseaseHandler>();
	}
}
