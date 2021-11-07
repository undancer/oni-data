using System.Collections.Generic;
using STRINGS;
using TUNING;

namespace Database
{
	public class ColonyAchievements : ResourceSet<ColonyAchievement>
	{
		public ColonyAchievement Thriving;

		public ColonyAchievement ReachedDistantPlanet;

		public ColonyAchievement CollectedArtifacts;

		public ColonyAchievement Survived100Cycles;

		public ColonyAchievement ReachedSpace;

		public ColonyAchievement CompleteSkillBranch;

		public ColonyAchievement CompleteResearchTree;

		public ColonyAchievement Clothe8Dupes;

		public ColonyAchievement Build4NatureReserves;

		public ColonyAchievement Minimum20LivingDupes;

		public ColonyAchievement TameAGassyMoo;

		public ColonyAchievement CoolBuildingTo6K;

		public ColonyAchievement EatkCalFromMeatByCycle100;

		public ColonyAchievement NoFarmTilesAndKCal;

		public ColonyAchievement Generate240000kJClean;

		public ColonyAchievement BuildOutsideStartBiome;

		public ColonyAchievement Travel10000InTubes;

		public ColonyAchievement VarietyOfRooms;

		public ColonyAchievement TameAllBasicCritters;

		public ColonyAchievement SurviveOneYear;

		public ColonyAchievement ExploreOilBiome;

		public ColonyAchievement EatCookedFood;

		public ColonyAchievement BasicPumping;

		public ColonyAchievement BasicComforts;

		public ColonyAchievement PlumbedWashrooms;

		public ColonyAchievement AutomateABuilding;

		public ColonyAchievement MasterpiecePainting;

		public ColonyAchievement InspectPOI;

		public ColonyAchievement HatchACritter;

		public ColonyAchievement CuredDisease;

		public ColonyAchievement GeneratorTuneup;

		public ColonyAchievement ClearFOW;

		public ColonyAchievement HatchRefinement;

		public ColonyAchievement BunkerDoorDefense;

		public ColonyAchievement IdleDuplicants;

		public ColonyAchievement ExosuitCycles;

		public ColonyAchievement FirstTeleport;

		public ColonyAchievement SoftLaunch;

		public ColonyAchievement GMOOK;

		public ColonyAchievement MineTheGap;

		public ColonyAchievement LandedOnAllWorlds;

		public ColonyAchievement RadicalTrip;

		public ColonyAchievement SweeterThanHoney;

		public ColonyAchievement SurviveInARocket;

		public ColonyAchievement RunAReactor;

		public ColonyAchievements(ResourceSet parent)
			: base("ColonyAchievements", parent)
		{
			Thriving = Add(new ColonyAchievement("Thriving", "WINCONDITION_STAY", COLONY_ACHIEVEMENTS.THRIVING.NAME, COLONY_ACHIEVEMENTS.THRIVING.DESCRIPTION, isVictoryCondition: true, new List<ColonyAchievementRequirement>
			{
				new CycleNumber(200),
				new MinimumMorale(),
				new NumberOfDupes(12),
				new MonumentBuilt()
			}, COLONY_ACHIEVEMENTS.THRIVING.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.THRIVING.MESSAGE_BODY, "victoryShorts/Stay", "victoryLoops/Stay_loop", ThrivingSequence.Start, AudioMixerSnapshots.Get().VictoryNISGenericSnapshot, "home_sweet_home"));
			ReachedDistantPlanet = (DlcManager.IsExpansion1Active() ? Add(new ColonyAchievement("ReachedDistantPlanet", "WINCONDITION_LEAVE", COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.NAME, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.DESCRIPTION, isVictoryCondition: true, new List<ColonyAchievementRequirement>
			{
				new EstablishColonies(),
				new OpenTemporalTear(),
				new SentCraftIntoTemporalTear()
			}, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_TITLE_DLC1, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_BODY_DLC1, "victoryShorts/Leave", "victoryLoops/Leave_loop", EnterTemporalTearSequence.Start, AudioMixerSnapshots.Get().VictoryNISRocketSnapshot, "rocket")) : Add(new ColonyAchievement("ReachedDistantPlanet", "WINCONDITION_LEAVE", COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.NAME, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.DESCRIPTION, isVictoryCondition: true, new List<ColonyAchievementRequirement>
			{
				new ReachedSpace(Db.Get().SpaceDestinationTypes.Wormhole)
			}, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_BODY, "victoryShorts/Leave", "victoryLoops/Leave_loop", ReachedDistantPlanetSequence.Start, AudioMixerSnapshots.Get().VictoryNISRocketSnapshot, "rocket")));
			if (DlcManager.IsExpansion1Active())
			{
				CollectedArtifacts = new ColonyAchievement("CollectedArtifacts", "WINCONDITION_ARTIFACTS", COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.NAME, COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.DESCRIPTION, isVictoryCondition: true, new List<ColonyAchievementRequirement>
				{
					new CollectedArtifacts(),
					new CollectedSpaceArtifacts()
				}, COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.MESSAGE_BODY, "victoryShorts/Artifact", "victoryLoops/Artifact_loop", ArtifactSequence.Start, AudioMixerSnapshots.Get().VictoryNISGenericSnapshot, "cosmic_archaeology");
				Add(CollectedArtifacts);
			}
			Survived100Cycles = Add(new ColonyAchievement("Survived100Cycles", "SURVIVE_HUNDRED_CYCLES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_HUNDRED_CYCLES, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_HUNDRED_CYCLES_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new CycleNumber()
			}, "", "", "", "", null, "", "Turn_of_the_Century"));
			ReachedSpace = (DlcManager.IsExpansion1Active() ? Add(new ColonyAchievement("ReachedSpace", "REACH_SPACE_ANY_DESTINATION", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new LaunchedCraft()
			}, "", "", "", "", null, "", "space_race")) : Add(new ColonyAchievement("ReachedSpace", "REACH_SPACE_ANY_DESTINATION", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new ReachedSpace()
			}, "", "", "", "", null, "", "space_race")));
			CompleteSkillBranch = Add(new ColonyAchievement("CompleteSkillBranch", "COMPLETED_SKILL_BRANCH", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_SKILL_BRANCH, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_SKILL_BRANCH_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new SkillBranchComplete(Db.Get().Skills.GetTerminalSkills())
			}, "", "", "", "", null, "", "CompleteSkillBranch"));
			CompleteResearchTree = Add(new ColonyAchievement("CompleteResearchTree", "COMPLETED_RESEARCH", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_RESEARCH, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_RESEARCH_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new ResearchComplete()
			}, "", "", "", "", null, "", "honorary_doctorate"));
			Clothe8Dupes = Add(new ColonyAchievement("Clothe8Dupes", "EQUIP_EIGHT_DUPES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_N_DUPES, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_N_DUPES_DESCRIPTION, 8), isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new EquipNDupes(Db.Get().AssignableSlots.Outfit, 8)
			}, "", "", "", "", null, "", "and_nowhere_to_go"));
			TameAllBasicCritters = Add(new ColonyAchievement("TameAllBasicCritters", "TAME_BASIC_CRITTERS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_BASIC_CRITTERS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_BASIC_CRITTERS_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new CritterTypesWithTraits(new List<Tag> { "Drecko", "Hatch", "LightBug", "Mole", "Oilfloater", "Pacu", "Puft", "Moo", "Crab", "Squirrel" })
			}, "", "", "", "", null, "", "Animal_friends"));
			Build4NatureReserves = Add(new ColonyAchievement("Build4NatureReserves", "BUILD_NATURE_RESERVES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_NATURE_RESERVES, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_NATURE_RESERVES_DESCRIPTION, Db.Get().RoomTypes.NatureReserve.Name, 4), isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new BuildNRoomTypes(Db.Get().RoomTypes.NatureReserve, 4)
			}, "", "", "", "", null, "", "Some_Reservations"));
			Minimum20LivingDupes = Add(new ColonyAchievement("Minimum20LivingDupes", "TWENTY_DUPES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TWENTY_DUPES, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TWENTY_DUPES_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new NumberOfDupes(20)
			}, "", "", "", "", null, "", "no_place_like_clone"));
			TameAGassyMoo = Add(new ColonyAchievement("TameAGassyMoo", "TAME_GASSYMOO", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_GASSYMOO, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_GASSYMOO_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new CritterTypesWithTraits(new List<Tag> { "Moo" })
			}, "", "", "", "", null, "", "moovin_on_up"));
			CoolBuildingTo6K = Add(new ColonyAchievement("CoolBuildingTo6K", "SIXKELVIN_BUILDING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SIXKELVIN_BUILDING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SIXKELVIN_BUILDING_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new CoolBuildingToXKelvin(6)
			}, "", "", "", "", null, "", "not_0k"));
			EatkCalFromMeatByCycle100 = Add(new ColonyAchievement("EatkCalFromMeatByCycle100", "EAT_MEAT", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_MEAT, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_MEAT_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new BeforeCycleNumber(),
				new EatXCaloriesFromY(400000, new List<string>
				{
					FOOD.FOOD_TYPES.MEAT.Id,
					FOOD.FOOD_TYPES.FISH_MEAT.Id,
					FOOD.FOOD_TYPES.COOKED_MEAT.Id,
					FOOD.FOOD_TYPES.COOKED_FISH.Id,
					FOOD.FOOD_TYPES.SURF_AND_TURF.Id,
					FOOD.FOOD_TYPES.BURGER.Id
				})
			}, "", "", "", "", null, "", "Carnivore"));
			NoFarmTilesAndKCal = Add(new ColonyAchievement("NoFarmTilesAndKCal", "NO_PLANTERBOX", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.NO_PLANTERBOX, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.NO_PLANTERBOX_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new NoFarmables(),
				new EatXCalories(400000)
			}, "", "", "", "", null, "", "Locavore"));
			Generate240000kJClean = Add(new ColonyAchievement("Generate240000kJClean", "CLEAN_ENERGY", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAN_ENERGY, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAN_ENERGY_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new ProduceXEngeryWithoutUsingYList(240000f, new List<Tag> { "MethaneGenerator", "PetroleumGenerator", "WoodGasGenerator", "Generator" })
			}, "", "", "", "", null, "", "sustainably_sustaining"));
			BuildOutsideStartBiome = Add(new ColonyAchievement("BuildOutsideStartBiome", "BUILD_OUTSIDE_BIOME", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_OUTSIDE_BIOME, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_OUTSIDE_BIOME_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new BuildOutsideStartBiome()
			}, "", "", "", "", null, "", "build_outside"));
			Travel10000InTubes = Add(new ColonyAchievement("Travel10000InTubes", "TUBE_TRAVEL_DISTANCE", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TUBE_TRAVEL_DISTANCE, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TUBE_TRAVEL_DISTANCE_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new TravelXUsingTransitTubes(NavType.Tube, 10000)
			}, "", "", "", "", null, "", "Totally-Tubular"));
			VarietyOfRooms = Add(new ColonyAchievement("VarietyOfRooms", "VARIETY_OF_ROOMS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.VARIETY_OF_ROOMS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.VARIETY_OF_ROOMS_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new BuildRoomType(Db.Get().RoomTypes.NatureReserve),
				new BuildRoomType(Db.Get().RoomTypes.Hospital),
				new BuildRoomType(Db.Get().RoomTypes.RecRoom),
				new BuildRoomType(Db.Get().RoomTypes.GreatHall),
				new BuildRoomType(Db.Get().RoomTypes.Bedroom),
				new BuildRoomType(Db.Get().RoomTypes.PlumbedBathroom),
				new BuildRoomType(Db.Get().RoomTypes.Farm),
				new BuildRoomType(Db.Get().RoomTypes.CreaturePen)
			}, "", "", "", "", null, "", "Get-a-Room"));
			SurviveOneYear = Add(new ColonyAchievement("SurviveOneYear", "SURVIVE_ONE_YEAR", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_ONE_YEAR, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_ONE_YEAR_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new FractionalCycleNumber(365.25f)
			}, "", "", "", "", null, "", "One_year"));
			ExploreOilBiome = Add(new ColonyAchievement("ExploreOilBiome", "EXPLORE_OIL_BIOME", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EXPLORE_OIL_BIOME, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EXPLORE_OIL_BIOME_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new ExploreOilFieldSubZone()
			}, "", "", "", "", null, "", "enter_oil_biome"));
			EatCookedFood = Add(new ColonyAchievement("EatCookedFood", "COOKED_FOOD", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COOKED_FOOD, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COOKED_FOOD_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new EatXKCalProducedByY(1, new List<Tag> { "GourmetCookingStation", "CookingStation" })
			}, "", "", "", "", null, "", "its_not_raw"));
			BasicPumping = Add(new ColonyAchievement("BasicPumping", "BASIC_PUMPING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_PUMPING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_PUMPING_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new VentXKG(SimHashes.Oxygen, 1000f)
			}, "", "", "", "", null, "", "BasicPumping"));
			BasicComforts = Add(new ColonyAchievement("BasicComforts", "BASIC_COMFORTS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_COMFORTS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_COMFORTS_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new AtLeastOneBuildingForEachDupe(new List<Tag> { "FlushToilet", "Outhouse" }),
				new AtLeastOneBuildingForEachDupe(new List<Tag>
				{
					BedConfig.ID,
					LuxuryBedConfig.ID
				})
			}, "", "", "", "", null, "", "1bed_1toilet"));
			PlumbedWashrooms = Add(new ColonyAchievement("PlumbedWashrooms", "PLUMBED_WASHROOMS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.PLUMBED_WASHROOMS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.PLUMBED_WASHROOMS_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new UpgradeAllBasicBuildings("Outhouse", "FlushToilet"),
				new UpgradeAllBasicBuildings("WashBasin", "WashSink")
			}, "", "", "", "", null, "", "royal_flush"));
			AutomateABuilding = Add(new ColonyAchievement("AutomateABuilding", "AUTOMATE_A_BUILDING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.AUTOMATE_A_BUILDING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.AUTOMATE_A_BUILDING_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new AutomateABuilding()
			}, "", "", "", "", null, "", "red_light_green_light"));
			MasterpiecePainting = Add(new ColonyAchievement("MasterpiecePainting", "MASTERPIECE_PAINTING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MASTERPIECE_PAINTING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MASTERPIECE_PAINTING_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new CreateMasterPainting()
			}, "", "", "", "", null, "", "art_underground"));
			InspectPOI = Add(new ColonyAchievement("InspectPOI", "INSPECT_POI", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.INSPECT_POI, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.INSPECT_POI_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new ActivateLorePOI()
			}, "", "", "", "", null, "", "ghosts_of_gravitas"));
			HatchACritter = Add(new ColonyAchievement("HatchACritter", "HATCH_A_CRITTER", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_A_CRITTER, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_A_CRITTER_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new CritterTypeExists(new List<Tag>
				{
					"DreckoPlasticBaby", "HatchHardBaby", "HatchMetalBaby", "HatchVeggieBaby", "LightBugBlackBaby", "LightBugBlueBaby", "LightBugCrystalBaby", "LightBugOrangeBaby", "LightBugPinkBaby", "LightBugPurpleBaby",
					"OilfloaterDecorBaby", "OilfloaterHighTempBaby", "PacuCleanerBaby", "PacuTropicalBaby", "PuftBleachstoneBaby", "PuftOxyliteBaby"
				})
			}, "", "", "", "", null, "", "good_egg"));
			CuredDisease = Add(new ColonyAchievement("CuredDisease", "CURED_DISEASE", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CURED_DISEASE, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CURED_DISEASE_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new CureDisease()
			}, "", "", "", "", null, "", "medic"));
			GeneratorTuneup = Add(new ColonyAchievement("GeneratorTuneup", "GENERATOR_TUNEUP", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GENERATOR_TUNEUP, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GENERATOR_TUNEUP_DESCRIPTION, 100), isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new TuneUpGenerator(100f)
			}, "", "", "", "", null, "", "tune_up_for_what"));
			ClearFOW = Add(new ColonyAchievement("ClearFOW", "CLEAR_FOW", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAR_FOW, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAR_FOW_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new RevealAsteriod(0.8f)
			}, "", "", "", "", null, "", "pulling_back_the_veil"));
			HatchRefinement = Add(new ColonyAchievement("HatchRefinement", "HATCH_REFINEMENT", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_REFINEMENT, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_REFINEMENT_DESCRIPTION, GameUtil.GetFormattedMass(10000f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne)), isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new CreaturePoopKGProduction("HatchMetal", 10000f)
			}, "", "", "", "", null, "", "down_the_hatch"));
			BunkerDoorDefense = Add(new ColonyAchievement("BunkerDoorDefense", "BUNKER_DOOR_DEFENSE", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUNKER_DOOR_DEFENSE, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUNKER_DOOR_DEFENSE_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new BlockedCometWithBunkerDoor()
			}, "", "", "", "", null, "", "Immovable_Object"));
			IdleDuplicants = Add(new ColonyAchievement("IdleDuplicants", "IDLE_DUPLICANTS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.IDLE_DUPLICANTS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.IDLE_DUPLICANTS_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new DupesVsSolidTransferArmFetch(0.51f, 5)
			}, "", "", "", "", null, "", "easy_livin"));
			ExosuitCycles = Add(new ColonyAchievement("ExosuitCycles", "EXOSUIT_CYCLES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EXOSUIT_CYCLES, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EXOSUIT_CYCLES_DESCRIPTION, 10), isVictoryCondition: false, new List<ColonyAchievementRequirement>
			{
				new DupesCompleteChoreInExoSuitForCycles(10)
			}, "", "", "", "", null, "", "job_suitability"));
			if (DlcManager.IsExpansion1Active())
			{
				FirstTeleport = Add(new ColonyAchievement("FirstTeleport", "FIRST_TELEPORT", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.FIRST_TELEPORT, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.FIRST_TELEPORT_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
				{
					new TeleportDuplicant(),
					new DefrostDuplicant()
				}, "", "", "", "", null, "", "first_teleport_of_call"));
				SoftLaunch = Add(new ColonyAchievement("SoftLaunch", "SOFT_LAUNCH", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SOFT_LAUNCH, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SOFT_LAUNCH_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
				{
					new BuildALaunchPad()
				}, "", "", "", "", null, "", "soft_launch"));
				GMOOK = Add(new ColonyAchievement("GMOOK", "GMO_OK", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GMO_OK, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GMO_OK_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
				{
					new AnalyzeSeed(BasicFabricMaterialPlantConfig.ID),
					new AnalyzeSeed("BasicSingleHarvestPlant"),
					new AnalyzeSeed("GasGrass"),
					new AnalyzeSeed("MushroomPlant"),
					new AnalyzeSeed("PrickleFlower"),
					new AnalyzeSeed("SaltPlant"),
					new AnalyzeSeed(SeaLettuceConfig.ID),
					new AnalyzeSeed("SpiceVine"),
					new AnalyzeSeed("SwampHarvestPlant"),
					new AnalyzeSeed(SwampLilyConfig.ID),
					new AnalyzeSeed("WormPlant"),
					new AnalyzeSeed("ColdWheat"),
					new AnalyzeSeed("BeanPlant")
				}, "", "", "", "", null, "", "gmo_ok"));
				MineTheGap = Add(new ColonyAchievement("MineTheGap", "MINE_THE_GAP", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MINE_THE_GAP, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MINE_THE_GAP_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
				{
					new HarvestAmountFromSpacePOI(1000000f)
				}, "", "", "", "", null, "", "mine_the_gap"));
				LandedOnAllWorlds = Add(new ColonyAchievement("LandedOnAllWorlds", "LANDED_ON_ALL_WORLDS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.LAND_ON_ALL_WORLDS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.LAND_ON_ALL_WORLDS_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
				{
					new LandOnAllWorlds()
				}, "", "", "", "", null, "", "land_on_all_worlds"));
				RadicalTrip = Add(new ColonyAchievement("RadicalTrip", "RADICAL_TRIP", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.RADICAL_TRIP, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.RADICAL_TRIP_DESCRIPTION, 10), isVictoryCondition: false, new List<ColonyAchievementRequirement>
				{
					new RadBoltTravelDistance(10000)
				}, "", "", "", "", null, "", "radical_trip"));
				SweeterThanHoney = Add(new ColonyAchievement("SweeterThanHoney", "SWEETER_THAN_HONEY", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SWEETER_THAN_HONEY, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SWEETER_THAN_HONEY_DESCRIPTION, isVictoryCondition: false, new List<ColonyAchievementRequirement>
				{
					new HarvestAHiveWithoutBeingStung()
				}, "", "", "", "", null, "", "sweeter_than_honey"));
				SurviveInARocket = Add(new ColonyAchievement("SurviveInARocket", "SURVIVE_IN_A_ROCKET", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_IN_A_ROCKET, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_IN_A_ROCKET_DESCRIPTION, 10, 25), isVictoryCondition: false, new List<ColonyAchievementRequirement>
				{
					new SurviveARocketWithMinimumMorale(25f, 10)
				}, "", "", "", "", null, "", "survive_a_rocket"));
				RunAReactor = Add(new ColonyAchievement("RunAReactor", "REACTOR_USAGE", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACTOR_USAGE, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACTOR_USAGE_DESCRIPTION, 5), isVictoryCondition: false, new List<ColonyAchievementRequirement>
				{
					new RunReactorForXDays(5)
				}, "", "", "", "", null, "", "thats_rad"));
			}
		}
	}
}
