using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class JetSuitConfig : IEquipmentConfig
{
	public const string ID = "Jet_Suit";

	public const string WORN_ID = "Worn_Jet_Suit";

	public static ComplexRecipe recipe;

	private const PathFinder.PotentialPath.Flags suit_flags = PathFinder.PotentialPath.Flags.HasJetPack;

	private AttributeModifier expertAthleticsModifier;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add(SimHashes.Steel.ToString(), 200f);
		dictionary.Add(SimHashes.Petroleum.ToString(), 25f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
		list.Add(new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
		list.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, TUNING.EQUIPMENT.SUITS.ATMOSUIT_DIGGING, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
		list.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME));
		expertAthleticsModifier = new AttributeModifier(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, -TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, Db.Get().Skills.Suits1.Name);
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Jet_Suit", TUNING.EQUIPMENT.SUITS.SLOT, SimHashes.Steel, TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS, "suit_jetpack_kanim", "", "body_jetpack_kanim", 6, list, null, IsBody: true, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, new Tag[2]
		{
			GameTags.Suit,
			GameTags.Clothes
		}, "JetSuit");
		equipmentDef.wornID = "Worn_Jet_Suit";
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC;
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("SoakingWet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("PoppedEarDrums"));
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			Ownables soleOwner2 = eq.assignee.GetSoleOwner();
			if (soleOwner2 != null)
			{
				GameObject targetGameObject2 = soleOwner2.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				Navigator component4 = targetGameObject2.GetComponent<Navigator>();
				if (component4 != null)
				{
					component4.SetFlags(PathFinder.PotentialPath.Flags.HasJetPack);
				}
				MinionResume component5 = targetGameObject2.GetComponent<MinionResume>();
				if (component5 != null && component5.HasPerk(Db.Get().SkillPerks.ExosuitExpertise.Id))
				{
					targetGameObject2.GetAttributes().Get(Db.Get().Attributes.Athletics).Add(expertAthleticsModifier);
				}
				KAnimControllerBase component6 = targetGameObject2.GetComponent<KAnimControllerBase>();
				if ((bool)component6)
				{
					component6.AddAnimOverrides(Assets.GetAnim("anim_loco_hover_kanim"));
				}
			}
		};
		equipmentDef.OnUnequipCallBack = delegate(Equippable eq)
		{
			if (eq.assignee != null)
			{
				Ownables soleOwner = eq.assignee.GetSoleOwner();
				if ((bool)soleOwner)
				{
					GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
					if ((bool)targetGameObject)
					{
						targetGameObject.GetAttributes()?.Get(Db.Get().Attributes.Athletics).Remove(expertAthleticsModifier);
						Navigator component = targetGameObject.GetComponent<Navigator>();
						if (component != null)
						{
							component.ClearFlags(PathFinder.PotentialPath.Flags.HasJetPack);
						}
						KAnimControllerBase component2 = targetGameObject.GetComponent<KAnimControllerBase>();
						if ((bool)component2)
						{
							component2.RemoveAnimOverrides(Assets.GetAnim("anim_loco_hover_kanim"));
						}
						Effects component3 = targetGameObject.GetComponent<Effects>();
						if (component3 != null && component3.HasEffect("SoiledSuit"))
						{
							component3.Remove("SoiledSuit");
						}
					}
					TagBits any_tags = new TagBits(eq.GetComponent<SuitTank>().elementTag);
					TagBits tagBits = default(TagBits);
					eq.GetComponent<Storage>().DropUnlessHasTags(any_tags, tagBits, tagBits, do_disease_transfer: true, dumpElements: true);
				}
			}
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Jet_Suit");
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Oxygen";
		suitTank.capacity = 75f;
		suitTank.elementTag = GameTags.Breathable;
		go.AddComponent<JetSuitTank>();
		HelmetController helmetController = go.AddComponent<HelmetController>();
		helmetController.has_jets = true;
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Clothes);
		component.AddTag(GameTags.PedestalDisplayable);
		component.AddTag(GameTags.AirtightSuit);
		Durability durability = go.AddComponent<Durability>();
		durability.wornEquipmentPrefabID = "Worn_Jet_Suit";
		durability.durabilityLossPerCycle = TUNING.EQUIPMENT.SUITS.ATMOSUIT_DECAY;
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		storage.showInUI = true;
		go.AddOrGet<AtmoSuit>();
		go.AddComponent<SuitDiseaseHandler>();
	}
}
