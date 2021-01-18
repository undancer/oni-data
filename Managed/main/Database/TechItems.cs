using System;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class TechItems : ResourceSet<TechItem>
	{
		public const string AUTOMATION_OVERLAY_ID = "AutomationOverlay";

		public TechItem automationOverlay;

		public const string SUITS_OVERLAY_ID = "SuitsOverlay";

		public TechItem suitsOverlay;

		public const string JET_SUIT_ID = "JetSuit";

		public TechItem jetSuit;

		public const string ATMO_SUIT_ID = "AtmoSuit";

		public TechItem atmoSuit;

		public const string LEAD_SUIT_ID = "LeadSuit";

		public TechItem leadSuit;

		public const string BETA_RESEARCH_POINT_ID = "BetaResearchPoint";

		public TechItem betaResearchPoint;

		public const string GAMMA_RESEARCH_POINT_ID = "GammaResearchPoint";

		public TechItem gammaResearchPoint;

		public const string DELTA_RESEARCH_POINT_ID = "DeltaResearchPoint";

		public TechItem deltaResearchPoint;

		public const string CONVEYOR_OVERLAY_ID = "ConveyorOverlay";

		public TechItem conveyorOverlay;

		public TechItems(ResourceSet parent)
			: base("TechItems", parent)
		{
		}

		public void Init()
		{
			automationOverlay = AddTechItem("AutomationOverlay", RESEARCH.OTHER_TECH_ITEMS.AUTOMATION_OVERLAY.NAME, RESEARCH.OTHER_TECH_ITEMS.AUTOMATION_OVERLAY.DESC, GetSpriteFnBuilder("overlay_logic"), "");
			suitsOverlay = AddTechItem("SuitsOverlay", RESEARCH.OTHER_TECH_ITEMS.SUITS_OVERLAY.NAME, RESEARCH.OTHER_TECH_ITEMS.SUITS_OVERLAY.DESC, GetSpriteFnBuilder("overlay_suit"), "");
			betaResearchPoint = AddTechItem("BetaResearchPoint", RESEARCH.OTHER_TECH_ITEMS.BETA_RESEARCH_POINT.NAME, RESEARCH.OTHER_TECH_ITEMS.BETA_RESEARCH_POINT.DESC, GetSpriteFnBuilder("research_type_beta_icon"), "");
			gammaResearchPoint = AddTechItem("GammaResearchPoint", RESEARCH.OTHER_TECH_ITEMS.GAMMA_RESEARCH_POINT.NAME, RESEARCH.OTHER_TECH_ITEMS.GAMMA_RESEARCH_POINT.DESC, GetSpriteFnBuilder("research_type_gamma_icon"), "");
			conveyorOverlay = AddTechItem("ConveyorOverlay", RESEARCH.OTHER_TECH_ITEMS.CONVEYOR_OVERLAY.NAME, RESEARCH.OTHER_TECH_ITEMS.CONVEYOR_OVERLAY.DESC, GetSpriteFnBuilder("overlay_conveyor"), "");
			jetSuit = AddTechItem("JetSuit", RESEARCH.OTHER_TECH_ITEMS.JET_SUIT.NAME, RESEARCH.OTHER_TECH_ITEMS.JET_SUIT.DESC, GetPrefabSpriteFnBuilder("Jet_Suit".ToTag()), "");
			atmoSuit = AddTechItem("AtmoSuit", RESEARCH.OTHER_TECH_ITEMS.ATMO_SUIT.NAME, RESEARCH.OTHER_TECH_ITEMS.ATMO_SUIT.DESC, GetPrefabSpriteFnBuilder("Atmo_Suit".ToTag()), "");
			if (DlcManager.IsExpansion1Active())
			{
				deltaResearchPoint = AddTechItem("DeltaResearchPoint", RESEARCH.OTHER_TECH_ITEMS.DELTA_RESEARCH_POINT.NAME, RESEARCH.OTHER_TECH_ITEMS.DELTA_RESEARCH_POINT.DESC, GetSpriteFnBuilder("research_type_delta_icon"), "EXPANSION1_ID");
				leadSuit = AddTechItem("LeadSuit", RESEARCH.OTHER_TECH_ITEMS.LEAD_SUIT.NAME, RESEARCH.OTHER_TECH_ITEMS.LEAD_SUIT.DESC, GetPrefabSpriteFnBuilder("Lead_Suit".ToTag()), "EXPANSION1_ID");
			}
		}

		private Func<string, bool, Sprite> GetSpriteFnBuilder(string spriteName)
		{
			return (string anim, bool centered) => Assets.GetSprite(spriteName);
		}

		private Func<string, bool, Sprite> GetPrefabSpriteFnBuilder(Tag prefabTag)
		{
			return (string anim, bool centered) => Def.GetUISprite(prefabTag).first;
		}

		public TechItem AddTechItem(string id, string name, string description, Func<string, bool, Sprite> getUISprite, string DLCId)
		{
			if (TryGet(id) != null)
			{
				DebugUtil.LogWarningArgs("Tried adding a tech item called", id, name, "but it was already added!");
				return Get(id);
			}
			Tech techFromItemID = GetTechFromItemID(id);
			if (techFromItemID == null)
			{
				return null;
			}
			TechItem techItem = new TechItem(id, this, name, description, getUISprite, techFromItemID.Id, DLCId);
			Add(techItem);
			techFromItemID.unlockedItems.Add(techItem);
			return techItem;
		}

		public bool IsTechItemComplete(string id)
		{
			bool result = true;
			foreach (TechItem resource in resources)
			{
				if (resource.Id == id)
				{
					result = resource.IsComplete();
					break;
				}
			}
			return result;
		}

		private Tech GetTechFromItemID(string itemId)
		{
			if (Db.Get().Techs == null)
			{
				return null;
			}
			return Db.Get().Techs.TryGetTechForTechItem(itemId);
		}

		public int GetTechTierForItem(string itemId)
		{
			Tech techFromItemID = GetTechFromItemID(itemId);
			if (techFromItemID != null)
			{
				return Techs.GetTier(techFromItemID);
			}
			return 0;
		}
	}
}
