using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class Skills : ResourceSet<Skill>
	{
		public Skill Mining1;

		public Skill Mining2;

		public Skill Mining3;

		public Skill Mining4;

		public Skill Building1;

		public Skill Building2;

		public Skill Building3;

		public Skill Farming1;

		public Skill Farming2;

		public Skill Farming3;

		public Skill Ranching1;

		public Skill Ranching2;

		public Skill Researching1;

		public Skill Researching2;

		public Skill Researching3;

		public Skill Researching4;

		public Skill AtomicResearch;

		public Skill SpaceResearch;

		public Skill Astronomy;

		public Skill RocketPiloting1;

		public Skill RocketPiloting2;

		public Skill Cooking1;

		public Skill Cooking2;

		public Skill Arting1;

		public Skill Arting2;

		public Skill Arting3;

		public Skill Hauling1;

		public Skill Hauling2;

		public Skill ThermalSuits;

		public Skill Suits1;

		public Skill Technicals1;

		public Skill Technicals2;

		public Skill Engineering1;

		public Skill Basekeeping1;

		public Skill Basekeeping2;

		public Skill Astronauting1;

		public Skill Astronauting2;

		public Skill Medicine1;

		public Skill Medicine2;

		public Skill Medicine3;

		public Skills(ResourceSet parent)
			: base("Skills", parent)
		{
			Mining1 = AddSkill(new Skill("Mining1", DUPLICANTS.ROLES.JUNIOR_MINER.NAME, DUPLICANTS.ROLES.JUNIOR_MINER.DESCRIPTION, "", 0, "hat_role_mining1", "skillbadge_role_mining1", Db.Get().SkillGroups.Mining.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseDigSpeedSmall,
				Db.Get().SkillPerks.CanDigVeryFirm
			}));
			Mining2 = AddSkill(new Skill("Mining2", DUPLICANTS.ROLES.MINER.NAME, DUPLICANTS.ROLES.MINER.DESCRIPTION, "", 1, "hat_role_mining2", "skillbadge_role_mining2", Db.Get().SkillGroups.Mining.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseDigSpeedMedium,
				Db.Get().SkillPerks.CanDigNearlyImpenetrable
			}, new List<string> { Mining1.Id }));
			Mining3 = AddSkill(new Skill("Mining3", DUPLICANTS.ROLES.SENIOR_MINER.NAME, DUPLICANTS.ROLES.SENIOR_MINER.DESCRIPTION, "", 2, "hat_role_mining3", "skillbadge_role_mining3", Db.Get().SkillGroups.Mining.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseDigSpeedLarge,
				Db.Get().SkillPerks.CanDigSuperDuperHard
			}, new List<string> { Mining2.Id }));
			Mining4 = AddSkill(new Skill("Mining4", DUPLICANTS.ROLES.MASTER_MINER.NAME, DUPLICANTS.ROLES.MASTER_MINER.DESCRIPTION, "EXPANSION1_ID", 3, "hat_role_mining4", "skillbadge_role_mining4", Db.Get().SkillGroups.Mining.Id, new List<SkillPerk> { Db.Get().SkillPerks.CanDigRadioactiveMaterials }, new List<string> { Mining3.Id }));
			Building1 = AddSkill(new Skill("Building1", DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME, DUPLICANTS.ROLES.JUNIOR_BUILDER.DESCRIPTION, "", 0, "hat_role_building1", "skillbadge_role_building1", Db.Get().SkillGroups.Building.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionSmall }));
			Building2 = AddSkill(new Skill("Building2", DUPLICANTS.ROLES.BUILDER.NAME, DUPLICANTS.ROLES.BUILDER.DESCRIPTION, "", 1, "hat_role_building2", "skillbadge_role_building2", Db.Get().SkillGroups.Building.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionMedium }, new List<string> { Building1.Id }));
			Building3 = AddSkill(new Skill("Building3", DUPLICANTS.ROLES.SENIOR_BUILDER.NAME, DUPLICANTS.ROLES.SENIOR_BUILDER.DESCRIPTION, "", 2, "hat_role_building3", "skillbadge_role_building3", Db.Get().SkillGroups.Building.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseConstructionLarge,
				Db.Get().SkillPerks.CanDemolish
			}, new List<string> { Building2.Id }));
			Farming1 = AddSkill(new Skill("Farming1", DUPLICANTS.ROLES.JUNIOR_FARMER.NAME, DUPLICANTS.ROLES.JUNIOR_FARMER.DESCRIPTION, "", 0, "hat_role_farming1", "skillbadge_role_farming1", Db.Get().SkillGroups.Farming.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseBotanySmall }));
			Farming2 = AddSkill(new Skill("Farming2", DUPLICANTS.ROLES.FARMER.NAME, DUPLICANTS.ROLES.FARMER.DESCRIPTION, "", 1, "hat_role_farming2", "skillbadge_role_farming2", Db.Get().SkillGroups.Farming.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseBotanyMedium,
				Db.Get().SkillPerks.CanFarmTinker
			}, new List<string> { Farming1.Id }));
			Farming3 = AddSkill(new Skill("Farming3", DUPLICANTS.ROLES.SENIOR_FARMER.NAME, DUPLICANTS.ROLES.SENIOR_FARMER.DESCRIPTION, "", 2, "hat_role_farming3", "skillbadge_role_farming3", Db.Get().SkillGroups.Farming.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseBotanyLarge }, new List<string> { Farming2.Id }));
			if (DlcManager.FeaturePlantMutationsEnabled())
			{
				Farming3.perks.Add(Db.Get().SkillPerks.CanIdentifyMutantSeeds);
			}
			Ranching1 = AddSkill(new Skill("Ranching1", DUPLICANTS.ROLES.RANCHER.NAME, DUPLICANTS.ROLES.RANCHER.DESCRIPTION, "", 1, "hat_role_rancher1", "skillbadge_role_rancher1", Db.Get().SkillGroups.Ranching.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanWrangleCreatures,
				Db.Get().SkillPerks.CanUseRanchStation,
				Db.Get().SkillPerks.IncreaseRanchingSmall
			}, new List<string> { Farming1.Id }));
			Ranching2 = AddSkill(new Skill("Ranching2", DUPLICANTS.ROLES.SENIOR_RANCHER.NAME, DUPLICANTS.ROLES.SENIOR_RANCHER.DESCRIPTION, "", 2, "hat_role_rancher2", "skillbadge_role_rancher2", Db.Get().SkillGroups.Ranching.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseRanchingMedium }, new List<string> { Ranching1.Id }));
			Researching1 = AddSkill(new Skill("Researching1", DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME, DUPLICANTS.ROLES.JUNIOR_RESEARCHER.DESCRIPTION, "", 0, "hat_role_research1", "skillbadge_role_research1", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningSmall,
				Db.Get().SkillPerks.AllowAdvancedResearch
			}));
			Researching2 = AddSkill(new Skill("Researching2", DUPLICANTS.ROLES.RESEARCHER.NAME, DUPLICANTS.ROLES.RESEARCHER.DESCRIPTION, "", 1, "hat_role_research2", "skillbadge_role_research2", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningMedium,
				Db.Get().SkillPerks.CanStudyWorldObjects
			}, new List<string> { Researching1.Id }));
			AtomicResearch = AddSkill(new Skill("AtomicResearch", DUPLICANTS.ROLES.NUCLEAR_RESEARCHER.NAME, DUPLICANTS.ROLES.NUCLEAR_RESEARCHER.DESCRIPTION, "EXPANSION1_ID", 2, "hat_role_research5", "skillbadge_role_research3", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningLarge,
				Db.Get().SkillPerks.AllowNuclearResearch
			}, new List<string> { Researching2.Id }));
			Researching4 = AddSkill(new Skill("Researching4", DUPLICANTS.ROLES.NUCLEAR_RESEARCHER.NAME, DUPLICANTS.ROLES.NUCLEAR_RESEARCHER.DESCRIPTION, "EXPANSION1_ID", 2, "hat_role_research4", "skillbadge_role_research3", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningLarge,
				Db.Get().SkillPerks.AllowNuclearResearch
			}, new List<string> { Researching2.Id }));
			Researching4.deprecated = true;
			Researching3 = AddSkill(new Skill("Researching3", DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME, DUPLICANTS.ROLES.SENIOR_RESEARCHER.DESCRIPTION, "", 2, "hat_role_research3", "skillbadge_role_research3", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningLarge,
				Db.Get().SkillPerks.AllowInterstellarResearch
			}, new List<string> { Researching2.Id }));
			Researching3.deprecated = DlcManager.IsExpansion1Active();
			Astronomy = AddSkill(new Skill("Astronomy", DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME, DUPLICANTS.ROLES.SENIOR_RESEARCHER.DESCRIPTION, "EXPANSION1_ID", 1, "hat_role_research3", "skillbadge_role_research2", Db.Get().SkillGroups.Research.Id, new List<SkillPerk> { Db.Get().SkillPerks.CanUseClusterTelescope }, new List<string> { Researching1.Id }));
			SpaceResearch = AddSkill(new Skill("SpaceResearch", DUPLICANTS.ROLES.SPACE_RESEARCHER.NAME, DUPLICANTS.ROLES.SPACE_RESEARCHER.DESCRIPTION, "EXPANSION1_ID", 2, "hat_role_research4", "skillbadge_role_research3", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningLargeSpace,
				Db.Get().SkillPerks.AllowOrbitalResearch
			}, new List<string> { Astronomy.Id }));
			if (DlcManager.IsExpansion1Active())
			{
				RocketPiloting1 = AddSkill(new Skill("RocketPiloting1", DUPLICANTS.ROLES.ROCKETPILOT.NAME, DUPLICANTS.ROLES.ROCKETPILOT.DESCRIPTION, "EXPANSION1_ID", 0, "hat_role_astronaut1", "skillbadge_role_rocketry1", Db.Get().SkillGroups.Rocketry.Id, new List<SkillPerk> { Db.Get().SkillPerks.CanUseRocketControlStation }, new List<string>()));
				RocketPiloting2 = AddSkill(new Skill("RocketPiloting2", DUPLICANTS.ROLES.SENIOR_ROCKETPILOT.NAME, DUPLICANTS.ROLES.SENIOR_ROCKETPILOT.DESCRIPTION, "EXPANSION1_ID", 2, "hat_role_astronaut2", "skillbadge_role_rocketry3", Db.Get().SkillGroups.Rocketry.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseRocketSpeedSmall }, new List<string> { RocketPiloting1.Id, Astronomy.Id }));
			}
			Cooking1 = AddSkill(new Skill("Cooking1", DUPLICANTS.ROLES.JUNIOR_COOK.NAME, DUPLICANTS.ROLES.JUNIOR_COOK.DESCRIPTION, "", 0, "hat_role_cooking1", "skillbadge_role_cooking1", Db.Get().SkillGroups.Cooking.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseCookingSmall,
				Db.Get().SkillPerks.CanElectricGrill
			}));
			Cooking2 = AddSkill(new Skill("Cooking2", DUPLICANTS.ROLES.COOK.NAME, DUPLICANTS.ROLES.COOK.DESCRIPTION, "", 1, "hat_role_cooking2", "skillbadge_role_cooking2", Db.Get().SkillGroups.Cooking.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseCookingMedium }, new List<string> { Cooking1.Id }));
			Arting1 = AddSkill(new Skill("Arting1", DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME, DUPLICANTS.ROLES.JUNIOR_ARTIST.DESCRIPTION, "", 0, "hat_role_art1", "skillbadge_role_art1", Db.Get().SkillGroups.Art.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanArt,
				Db.Get().SkillPerks.CanArtUgly,
				Db.Get().SkillPerks.IncreaseArtSmall
			}));
			Arting2 = AddSkill(new Skill("Arting2", DUPLICANTS.ROLES.ARTIST.NAME, DUPLICANTS.ROLES.ARTIST.DESCRIPTION, "", 1, "hat_role_art2", "skillbadge_role_art2", Db.Get().SkillGroups.Art.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanArtOkay,
				Db.Get().SkillPerks.IncreaseArtMedium,
				Db.Get().SkillPerks.CanClothingAlteration
			}, new List<string> { Arting1.Id }));
			if (DlcManager.FeatureClusterSpaceEnabled())
			{
				Arting2.perks.Add(Db.Get().SkillPerks.CanStudyArtifact);
			}
			Arting3 = AddSkill(new Skill("Arting3", DUPLICANTS.ROLES.MASTER_ARTIST.NAME, DUPLICANTS.ROLES.MASTER_ARTIST.DESCRIPTION, "", 2, "hat_role_art3", "skillbadge_role_art3", Db.Get().SkillGroups.Art.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanArtGreat,
				Db.Get().SkillPerks.IncreaseArtLarge
			}, new List<string> { Arting2.Id }));
			Hauling1 = AddSkill(new Skill("Hauling1", DUPLICANTS.ROLES.HAULER.NAME, DUPLICANTS.ROLES.HAULER.DESCRIPTION, "", 0, "hat_role_hauling1", "skillbadge_role_hauling1", Db.Get().SkillGroups.Hauling.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseStrengthGofer,
				Db.Get().SkillPerks.IncreaseCarryAmountSmall
			}));
			Hauling2 = AddSkill(new Skill("Hauling2", DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME, DUPLICANTS.ROLES.MATERIALS_MANAGER.DESCRIPTION, "", 1, "hat_role_hauling2", "skillbadge_role_hauling2", Db.Get().SkillGroups.Hauling.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseStrengthCourier,
				Db.Get().SkillPerks.IncreaseCarryAmountMedium
			}, new List<string> { Hauling1.Id }));
			if (DlcManager.IsExpansion1Active())
			{
				ThermalSuits = AddSkill(new Skill("ThermalSuits", DUPLICANTS.ROLES.SUIT_DURABILITY.NAME, DUPLICANTS.ROLES.SUIT_DURABILITY.DESCRIPTION, "EXPANSION1_ID", 1, "hat_role_suits1", "skillbadge_role_suits2", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk>
				{
					Db.Get().SkillPerks.IncreaseAthleticsLarge,
					Db.Get().SkillPerks.ExosuitDurability
				}, new List<string> { Hauling1.Id, RocketPiloting1.Id }));
			}
			else
			{
				ThermalSuits = AddSkill(new Skill("ThermalSuits", DUPLICANTS.ROLES.SUIT_DURABILITY.NAME, DUPLICANTS.ROLES.SUIT_DURABILITY.DESCRIPTION, "", 1, "hat_role_suits1", "skillbadge_role_suits2", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk>
				{
					Db.Get().SkillPerks.IncreaseAthleticsLarge,
					Db.Get().SkillPerks.ExosuitDurability
				}, new List<string> { Hauling1.Id }));
			}
			Suits1 = AddSkill(new Skill("Suits1", DUPLICANTS.ROLES.SUIT_EXPERT.NAME, DUPLICANTS.ROLES.SUIT_EXPERT.DESCRIPTION, "", 2, "hat_role_suits2", "skillbadge_role_suits3", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.ExosuitExpertise,
				Db.Get().SkillPerks.IncreaseAthleticsMedium
			}, new List<string> { ThermalSuits.Id }));
			Technicals1 = AddSkill(new Skill("Technicals1", DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME, DUPLICANTS.ROLES.MACHINE_TECHNICIAN.DESCRIPTION, "", 0, "hat_role_technicals1", "skillbadge_role_technicals1", Db.Get().SkillGroups.Technicals.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseMachinerySmall }));
			Technicals2 = AddSkill(new Skill("Technicals2", DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME, DUPLICANTS.ROLES.POWER_TECHNICIAN.DESCRIPTION, "", 1, "hat_role_technicals2", "skillbadge_role_technicals2", Db.Get().SkillGroups.Technicals.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseMachineryMedium,
				Db.Get().SkillPerks.CanPowerTinker
			}, new List<string> { Technicals1.Id }));
			Engineering1 = AddSkill(new Skill("Engineering1", DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.DESCRIPTION, "", 2, "hat_role_engineering1", "skillbadge_role_engineering1", Db.Get().SkillGroups.Technicals.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseMachineryLarge,
				Db.Get().SkillPerks.IncreaseConstructionMechatronics,
				Db.Get().SkillPerks.ConveyorBuild
			}, new List<string> { Technicals2.Id, Hauling2.Id }));
			Basekeeping1 = AddSkill(new Skill("Basekeeping1", DUPLICANTS.ROLES.HANDYMAN.NAME, DUPLICANTS.ROLES.HANDYMAN.DESCRIPTION, "", 0, "hat_role_basekeeping1", "skillbadge_role_basekeeping1", Db.Get().SkillGroups.Basekeeping.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseStrengthGroundskeeper }));
			Basekeeping2 = AddSkill(new Skill("Basekeeping2", DUPLICANTS.ROLES.PLUMBER.NAME, DUPLICANTS.ROLES.PLUMBER.DESCRIPTION, "", 1, "hat_role_basekeeping2", "skillbadge_role_basekeeping2", Db.Get().SkillGroups.Basekeeping.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseStrengthPlumber,
				Db.Get().SkillPerks.CanDoPlumbing
			}, new List<string> { Basekeeping1.Id }));
			if (DlcManager.IsExpansion1Active())
			{
				Astronauting1 = AddSkill(new Skill("Astronauting1", DUPLICANTS.ROLES.USELESSSKILL.NAME, DUPLICANTS.ROLES.USELESSSKILL.DESCRIPTION, "EXPANSION1_ID", 3, "hat_role_astronaut1", "skillbadge_role_astronaut1", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseAthleticsMedium }, new List<string> { Researching3.Id, Suits1.Id }));
				Astronauting1.deprecated = true;
				Astronauting2 = AddSkill(new Skill("Astronauting2", DUPLICANTS.ROLES.USELESSSKILL.NAME, DUPLICANTS.ROLES.USELESSSKILL.DESCRIPTION, "EXPANSION1_ID", 4, "hat_role_astronaut2", "skillbadge_role_astronaut2", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseAthleticsMedium }, new List<string> { Astronauting1.Id }));
				Astronauting2.deprecated = true;
			}
			else
			{
				Astronauting1 = AddSkill(new Skill("Astronauting1", DUPLICANTS.ROLES.ASTRONAUTTRAINEE.NAME, DUPLICANTS.ROLES.ASTRONAUTTRAINEE.DESCRIPTION, "", 3, "hat_role_astronaut1", "skillbadge_role_astronaut1", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk> { Db.Get().SkillPerks.CanUseRockets }, new List<string> { Researching3.Id, Suits1.Id }));
				Astronauting2 = AddSkill(new Skill("Astronauting2", DUPLICANTS.ROLES.ASTRONAUT.NAME, DUPLICANTS.ROLES.ASTRONAUT.DESCRIPTION, "", 4, "hat_role_astronaut2", "skillbadge_role_astronaut2", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk> { Db.Get().SkillPerks.FasterSpaceFlight }, new List<string> { Astronauting1.Id }));
			}
			Medicine1 = AddSkill(new Skill("Medicine1", DUPLICANTS.ROLES.JUNIOR_MEDIC.NAME, DUPLICANTS.ROLES.JUNIOR_MEDIC.DESCRIPTION, "", 0, "hat_role_medicalaid1", "skillbadge_role_medicalaid1", Db.Get().SkillGroups.MedicalAid.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanCompound,
				Db.Get().SkillPerks.IncreaseCaringSmall
			}));
			Medicine2 = AddSkill(new Skill("Medicine2", DUPLICANTS.ROLES.MEDIC.NAME, DUPLICANTS.ROLES.MEDIC.DESCRIPTION, "", 1, "hat_role_medicalaid2", "skillbadge_role_medicalaid2", Db.Get().SkillGroups.MedicalAid.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanDoctor,
				Db.Get().SkillPerks.IncreaseCaringMedium
			}, new List<string> { Medicine1.Id }));
			Medicine3 = AddSkill(new Skill("Medicine3", DUPLICANTS.ROLES.SENIOR_MEDIC.NAME, DUPLICANTS.ROLES.SENIOR_MEDIC.DESCRIPTION, "", 2, "hat_role_medicalaid3", "skillbadge_role_medicalaid3", Db.Get().SkillGroups.MedicalAid.Id, new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanAdvancedMedicine,
				Db.Get().SkillPerks.IncreaseCaringLarge
			}, new List<string> { Medicine2.Id }));
		}

		private Skill AddSkill(Skill skill)
		{
			if (DlcManager.IsContentActive(skill.dlcId))
			{
				return Add(skill);
			}
			return skill;
		}

		public List<Skill> GetSkillsWithPerk(string perk)
		{
			List<Skill> list = new List<Skill>();
			foreach (Skill resource in resources)
			{
				if (resource.GivesPerk(perk))
				{
					list.Add(resource);
				}
			}
			return list;
		}

		public List<Skill> GetSkillsWithPerk(SkillPerk perk)
		{
			List<Skill> list = new List<Skill>();
			foreach (Skill resource in resources)
			{
				if (resource.GivesPerk(perk))
				{
					list.Add(resource);
				}
			}
			return list;
		}

		public List<Skill> GetAllPriorSkills(Skill skill)
		{
			List<Skill> list = new List<Skill>();
			foreach (string priorSkill in skill.priorSkills)
			{
				Skill skill2 = Get(priorSkill);
				list.Add(skill2);
				list.AddRange(GetAllPriorSkills(skill2));
			}
			return list;
		}

		public List<Skill> GetTerminalSkills()
		{
			List<Skill> list = new List<Skill>();
			foreach (Skill resource in resources)
			{
				bool flag = true;
				foreach (Skill resource2 in resources)
				{
					if (resource2.priorSkills.Contains(resource.Id))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					list.Add(resource);
				}
			}
			return list;
		}
	}
}
