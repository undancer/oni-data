using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

namespace TUNING
{
	public class TRAITS
	{
		public class JOY_REACTIONS
		{
			public class SUPER_PRODUCTIVE
			{
				public static float INSTANT_SUCCESS_CHANCE = 10f;
			}

			public class BALLOON_ARTIST
			{
				public static float MINIMUM_BALLOON_MOVESPEED = 5f;

				public static int NUM_BALLOONS_TO_GIVE = 4;
			}

			public class STICKER_BOMBER
			{
				public static float TIME_PER_STICKER_BOMB = 150f;

				public static float STICKER_DURATION = 12000f;

				public static List<string> STICKER_ANIMS = new List<string>
				{
					"a", "b", "c", "d", "e", "f", "g", "h", "rocket", "paperplane",
					"plant", "plantpot", "mushroom", "mermaid", "spacepet", "spacepet2", "spacepet3", "spacepet4", "spacepet5", "unicorn"
				};
			}

			public static float MIN_MORALE_EXCESS = 8f;

			public static float MAX_MORALE_EXCESS = 20f;

			public static float MIN_REACTION_CHANCE = 2f;

			public static float MAX_REACTION_CHANCE = 5f;

			public static float JOY_REACTION_DURATION = 570f;
		}

		public static float EARLYBIRD_MODIFIER = 2f;

		public static int EARLYBIRD_SCHEDULEBLOCK = 5;

		public static float NIGHTOWL_MODIFIER = 3f;

		public const float FLATULENCE_EMIT_MASS = 0.1f;

		public static float FLATULENCE_EMIT_INTERVAL_MIN = 10f;

		public static float FLATULENCE_EMIT_INTERVAL_MAX = 40f;

		public static float STINKY_EMIT_INTERVAL_MIN = 10f;

		public static float STINKY_EMIT_INTERVAL_MAX = 30f;

		public static float NARCOLEPSY_INTERVAL_MIN = 300f;

		public static float NARCOLEPSY_INTERVAL_MAX = 600f;

		public static float NARCOLEPSY_SLEEPDURATION_MIN = 15f;

		public static float NARCOLEPSY_SLEEPDURATION_MAX = 30f;

		public const float INTERRUPTED_SLEEP_STRESS_DELTA = 10f;

		public const float INTERRUPTED_SLEEP_ATHLETICS_DELTA = -2f;

		public static int NO_ATTRIBUTE_BONUS = 0;

		public static int GOOD_ATTRIBUTE_BONUS = 3;

		public static int GREAT_ATTRIBUTE_BONUS = 5;

		public static int BAD_ATTRIBUTE_PENALTY = -3;

		public static int HORRIBLE_ATTRIBUTE_PENALTY = -5;

		public static float GLOWSTICK_RADIATION_RESISTANCE = 0.33f;

		public static float RADIATION_EATER_RECOVERY = -0.25f;

		public static float RADS_TO_CALS = 3333f;

		public static readonly List<System.Action> TRAIT_CREATORS = new List<System.Action>
		{
			TraitUtil.CreateAttributeEffectTrait("None", DUPLICANTS.CONGENITALTRAITS.NONE.NAME, DUPLICANTS.CONGENITALTRAITS.NONE.DESC, "", NO_ATTRIBUTE_BONUS),
			TraitUtil.CreateComponentTrait<Stinky>("Stinky", DUPLICANTS.CONGENITALTRAITS.STINKY.NAME, DUPLICANTS.CONGENITALTRAITS.STINKY.DESC),
			TraitUtil.CreateAttributeEffectTrait("Ellie", DUPLICANTS.CONGENITALTRAITS.ELLIE.NAME, DUPLICANTS.CONGENITALTRAITS.ELLIE.DESC, "AirConsumptionRate", -0.044999998f, "DecorExpectation", -5f),
			TraitUtil.CreateDisabledTaskTrait("Joshua", DUPLICANTS.CONGENITALTRAITS.JOSHUA.NAME, DUPLICANTS.CONGENITALTRAITS.JOSHUA.DESC, "Combat", is_valid_starter_trait: true),
			TraitUtil.CreateComponentTrait<Stinky>("Liam", DUPLICANTS.CONGENITALTRAITS.LIAM.NAME, DUPLICANTS.CONGENITALTRAITS.LIAM.DESC),
			TraitUtil.CreateNamedTrait("AncientKnowledge", DUPLICANTS.TRAITS.ANCIENTKNOWLEDGE.NAME, DUPLICANTS.TRAITS.ANCIENTKNOWLEDGE.DESC, positiveTrait: true),
			TraitUtil.CreateDisabledTaskTrait("CantResearch", DUPLICANTS.TRAITS.CANTRESEARCH.NAME, DUPLICANTS.TRAITS.CANTRESEARCH.DESC, "Research", is_valid_starter_trait: true),
			TraitUtil.CreateDisabledTaskTrait("CantBuild", DUPLICANTS.TRAITS.CANTBUILD.NAME, DUPLICANTS.TRAITS.CANTBUILD.DESC, "Build", is_valid_starter_trait: false),
			TraitUtil.CreateDisabledTaskTrait("CantCook", DUPLICANTS.TRAITS.CANTCOOK.NAME, DUPLICANTS.TRAITS.CANTCOOK.DESC, "Cook", is_valid_starter_trait: true),
			TraitUtil.CreateDisabledTaskTrait("CantDig", DUPLICANTS.TRAITS.CANTDIG.NAME, DUPLICANTS.TRAITS.CANTDIG.DESC, "Dig", is_valid_starter_trait: false),
			TraitUtil.CreateDisabledTaskTrait("Hemophobia", DUPLICANTS.TRAITS.HEMOPHOBIA.NAME, DUPLICANTS.TRAITS.HEMOPHOBIA.DESC, "MedicalAid", is_valid_starter_trait: true),
			TraitUtil.CreateDisabledTaskTrait("ScaredyCat", DUPLICANTS.TRAITS.SCAREDYCAT.NAME, DUPLICANTS.TRAITS.SCAREDYCAT.DESC, "Combat", is_valid_starter_trait: true),
			TraitUtil.CreateNamedTrait("Allergies", DUPLICANTS.TRAITS.ALLERGIES.NAME, DUPLICANTS.TRAITS.ALLERGIES.DESC),
			TraitUtil.CreateNamedTrait("NightLight", DUPLICANTS.TRAITS.NIGHTLIGHT.NAME, DUPLICANTS.TRAITS.NIGHTLIGHT.DESC),
			TraitUtil.CreateAttributeEffectTrait("MouthBreather", DUPLICANTS.TRAITS.MOUTHBREATHER.NAME, DUPLICANTS.TRAITS.MOUTHBREATHER.DESC, "AirConsumptionRate", 0.1f),
			TraitUtil.CreateAttributeEffectTrait("CalorieBurner", DUPLICANTS.TRAITS.CALORIEBURNER.NAME, DUPLICANTS.TRAITS.CALORIEBURNER.DESC, "CaloriesDelta", -833.3333f),
			TraitUtil.CreateAttributeEffectTrait("SmallBladder", DUPLICANTS.TRAITS.SMALLBLADDER.NAME, DUPLICANTS.TRAITS.SMALLBLADDER.DESC, "BladderDelta", 0.00027777778f),
			TraitUtil.CreateAttributeEffectTrait("Anemic", DUPLICANTS.TRAITS.ANEMIC.NAME, DUPLICANTS.TRAITS.ANEMIC.DESC, "Athletics", HORRIBLE_ATTRIBUTE_PENALTY),
			TraitUtil.CreateAttributeEffectTrait("SlowLearner", DUPLICANTS.TRAITS.SLOWLEARNER.NAME, DUPLICANTS.TRAITS.SLOWLEARNER.DESC, "Learning", BAD_ATTRIBUTE_PENALTY),
			TraitUtil.CreateAttributeEffectTrait("NoodleArms", DUPLICANTS.TRAITS.NOODLEARMS.NAME, DUPLICANTS.TRAITS.NOODLEARMS.DESC, "Strength", BAD_ATTRIBUTE_PENALTY),
			TraitUtil.CreateAttributeEffectTrait("ConstructionDown", DUPLICANTS.TRAITS.CONSTRUCTIONDOWN.NAME, DUPLICANTS.TRAITS.CONSTRUCTIONDOWN.DESC, "Construction", BAD_ATTRIBUTE_PENALTY),
			TraitUtil.CreateAttributeEffectTrait("RanchingDown", DUPLICANTS.TRAITS.RANCHINGDOWN.NAME, DUPLICANTS.TRAITS.RANCHINGDOWN.DESC, "Ranching", BAD_ATTRIBUTE_PENALTY),
			TraitUtil.CreateAttributeEffectTrait("DiggingDown", DUPLICANTS.TRAITS.DIGGINGDOWN.NAME, DUPLICANTS.TRAITS.DIGGINGDOWN.DESC, "Digging", BAD_ATTRIBUTE_PENALTY),
			TraitUtil.CreateAttributeEffectTrait("MachineryDown", DUPLICANTS.TRAITS.MACHINERYDOWN.NAME, DUPLICANTS.TRAITS.MACHINERYDOWN.DESC, "Machinery", BAD_ATTRIBUTE_PENALTY),
			TraitUtil.CreateAttributeEffectTrait("CookingDown", DUPLICANTS.TRAITS.COOKINGDOWN.NAME, DUPLICANTS.TRAITS.COOKINGDOWN.DESC, "Cooking", BAD_ATTRIBUTE_PENALTY, "FoodExpectation", 1f),
			TraitUtil.CreateAttributeEffectTrait("ArtDown", DUPLICANTS.TRAITS.ARTDOWN.NAME, DUPLICANTS.TRAITS.ARTDOWN.DESC, "Art", BAD_ATTRIBUTE_PENALTY, "DecorExpectation", 5f),
			TraitUtil.CreateAttributeEffectTrait("CaringDown", DUPLICANTS.TRAITS.CARINGDOWN.NAME, DUPLICANTS.TRAITS.CARINGDOWN.DESC, "Caring", BAD_ATTRIBUTE_PENALTY),
			TraitUtil.CreateAttributeEffectTrait("BotanistDown", DUPLICANTS.TRAITS.BOTANISTDOWN.NAME, DUPLICANTS.TRAITS.BOTANISTDOWN.DESC, "Botanist", BAD_ATTRIBUTE_PENALTY),
			TraitUtil.CreateAttributeEffectTrait("DecorDown", DUPLICANTS.TRAITS.DECORDOWN.NAME, DUPLICANTS.TRAITS.DECORDOWN.DESC, "Decor", BUILDINGS.DECOR.PENALTY.TIER2.amount),
			TraitUtil.CreateAttributeEffectTrait("Regeneration", DUPLICANTS.TRAITS.REGENERATION.NAME, DUPLICANTS.TRAITS.REGENERATION.DESC, "HitPointsDelta", 71f / (678f * (float)Math.PI)),
			TraitUtil.CreateAttributeEffectTrait("DeeperDiversLungs", DUPLICANTS.TRAITS.DEEPERDIVERSLUNGS.NAME, DUPLICANTS.TRAITS.DEEPERDIVERSLUNGS.DESC, "AirConsumptionRate", -0.05f),
			TraitUtil.CreateAttributeEffectTrait("SunnyDisposition", DUPLICANTS.TRAITS.SUNNYDISPOSITION.NAME, DUPLICANTS.TRAITS.SUNNYDISPOSITION.DESC, "StressDelta", -71f / (678f * (float)Math.PI), positiveTrait: false, delegate(GameObject go)
			{
				go.GetComponent<KBatchedAnimController>().AddAnimOverrides(Assets.GetAnim("anim_loco_happy_kanim"));
			}),
			TraitUtil.CreateAttributeEffectTrait("RockCrusher", DUPLICANTS.TRAITS.ROCKCRUSHER.NAME, DUPLICANTS.TRAITS.ROCKCRUSHER.DESC, "Strength", 10f),
			TraitUtil.CreateTrait("Uncultured", DUPLICANTS.TRAITS.UNCULTURED.NAME, DUPLICANTS.TRAITS.UNCULTURED.DESC, "DecorExpectation", 20f, new string[1] { "Art" }, positiveTrait: true),
			TraitUtil.CreateNamedTrait("Archaeologist", DUPLICANTS.TRAITS.ARCHAEOLOGIST.NAME, DUPLICANTS.TRAITS.ARCHAEOLOGIST.DESC),
			TraitUtil.CreateAttributeEffectTrait("WeakImmuneSystem", DUPLICANTS.TRAITS.WEAKIMMUNESYSTEM.NAME, DUPLICANTS.TRAITS.WEAKIMMUNESYSTEM.DESC, "GermResistance", -1f),
			TraitUtil.CreateAttributeEffectTrait("IrritableBowel", DUPLICANTS.TRAITS.IRRITABLEBOWEL.NAME, DUPLICANTS.TRAITS.IRRITABLEBOWEL.DESC, "ToiletEfficiency", -0.5f),
			TraitUtil.CreateComponentTrait<Flatulence>("Flatulence", DUPLICANTS.TRAITS.FLATULENCE.NAME, DUPLICANTS.TRAITS.FLATULENCE.DESC),
			TraitUtil.CreateComponentTrait<Snorer>("Snorer", DUPLICANTS.TRAITS.SNORER.NAME, DUPLICANTS.TRAITS.SNORER.DESC),
			TraitUtil.CreateComponentTrait<Narcolepsy>("Narcolepsy", DUPLICANTS.TRAITS.NARCOLEPSY.NAME, DUPLICANTS.TRAITS.NARCOLEPSY.DESC),
			TraitUtil.CreateComponentTrait<Thriver>("Thriver", DUPLICANTS.TRAITS.THRIVER.NAME, DUPLICANTS.TRAITS.THRIVER.DESC, positiveTrait: true),
			TraitUtil.CreateComponentTrait<Loner>("Loner", DUPLICANTS.TRAITS.LONER.NAME, DUPLICANTS.TRAITS.LONER.DESC, positiveTrait: true),
			TraitUtil.CreateComponentTrait<StarryEyed>("StarryEyed", DUPLICANTS.TRAITS.STARRYEYED.NAME, DUPLICANTS.TRAITS.STARRYEYED.DESC, positiveTrait: true),
			TraitUtil.CreateComponentTrait<GlowStick>("GlowStick", DUPLICANTS.TRAITS.GLOWSTICK.NAME, DUPLICANTS.TRAITS.GLOWSTICK.DESC, positiveTrait: true),
			TraitUtil.CreateComponentTrait<RadiationEater>("RadiationEater", DUPLICANTS.TRAITS.RADIATIONEATER.NAME, DUPLICANTS.TRAITS.RADIATIONEATER.DESC, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("Twinkletoes", DUPLICANTS.TRAITS.TWINKLETOES.NAME, DUPLICANTS.TRAITS.TWINKLETOES.DESC, "Athletics", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("Greasemonkey", DUPLICANTS.TRAITS.GREASEMONKEY.NAME, DUPLICANTS.TRAITS.GREASEMONKEY.DESC, "Machinery", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("MoleHands", DUPLICANTS.TRAITS.MOLEHANDS.NAME, DUPLICANTS.TRAITS.MOLEHANDS.DESC, "Digging", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("FastLearner", DUPLICANTS.TRAITS.FASTLEARNER.NAME, DUPLICANTS.TRAITS.FASTLEARNER.DESC, "Learning", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("DiversLung", DUPLICANTS.TRAITS.DIVERSLUNG.NAME, DUPLICANTS.TRAITS.DIVERSLUNG.DESC, "AirConsumptionRate", -0.025f, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("StrongArm", DUPLICANTS.TRAITS.STRONGARM.NAME, DUPLICANTS.TRAITS.STRONGARM.DESC, "Strength", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("DecorUp", DUPLICANTS.TRAITS.DECORUP.NAME, DUPLICANTS.TRAITS.DECORUP.DESC, "Decor", BUILDINGS.DECOR.BONUS.TIER4.amount, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("GreenThumb", DUPLICANTS.TRAITS.GREENTHUMB.NAME, DUPLICANTS.TRAITS.GREENTHUMB.DESC, "Botanist", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("InteriorDecorator", DUPLICANTS.TRAITS.INTERIORDECORATOR.NAME, DUPLICANTS.TRAITS.INTERIORDECORATOR.DESC, "Art", GOOD_ATTRIBUTE_BONUS, "DecorExpectation", -5f, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("SimpleTastes", DUPLICANTS.TRAITS.SIMPLETASTES.NAME, DUPLICANTS.TRAITS.SIMPLETASTES.DESC, "FoodExpectation", 1f, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("Foodie", DUPLICANTS.TRAITS.FOODIE.NAME, DUPLICANTS.TRAITS.FOODIE.DESC, "Cooking", GOOD_ATTRIBUTE_BONUS, "FoodExpectation", -1f, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("BedsideManner", DUPLICANTS.TRAITS.BEDSIDEMANNER.NAME, DUPLICANTS.TRAITS.BEDSIDEMANNER.DESC, "Caring", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("ConstructionUp", DUPLICANTS.TRAITS.CONSTRUCTIONUP.NAME, DUPLICANTS.TRAITS.CONSTRUCTIONUP.DESC, "Construction", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("RanchingUp", DUPLICANTS.TRAITS.RANCHINGUP.NAME, DUPLICANTS.TRAITS.RANCHINGUP.DESC, "Ranching", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Mining1", DUPLICANTS.TRAITS.GRANTSKILL_MINING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_MINING1.DESC, "Mining1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Mining2", DUPLICANTS.TRAITS.GRANTSKILL_MINING2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_MINING2.DESC, "Mining2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Mining3", DUPLICANTS.TRAITS.GRANTSKILL_MINING3.NAME, DUPLICANTS.TRAITS.GRANTSKILL_MINING3.DESC, "Mining3"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Mining4", DUPLICANTS.TRAITS.GRANTSKILL_MINING4.NAME, DUPLICANTS.TRAITS.GRANTSKILL_MINING4.DESC, "Mining4"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Building1", DUPLICANTS.TRAITS.GRANTSKILL_BUILDING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_BUILDING1.DESC, "Building1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Building2", DUPLICANTS.TRAITS.GRANTSKILL_BUILDING2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_BUILDING2.DESC, "Building2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Building3", DUPLICANTS.TRAITS.GRANTSKILL_BUILDING3.NAME, DUPLICANTS.TRAITS.GRANTSKILL_BUILDING3.DESC, "Building3"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Farming1", DUPLICANTS.TRAITS.GRANTSKILL_FARMING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_FARMING1.DESC, "Farming1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Farming2", DUPLICANTS.TRAITS.GRANTSKILL_FARMING2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_FARMING2.DESC, "Farming2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Farming3", DUPLICANTS.TRAITS.GRANTSKILL_FARMING3.NAME, DUPLICANTS.TRAITS.GRANTSKILL_FARMING3.DESC, "Farming3"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Ranching1", DUPLICANTS.TRAITS.GRANTSKILL_RANCHING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_RANCHING1.DESC, "Ranching1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Ranching2", DUPLICANTS.TRAITS.GRANTSKILL_RANCHING2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_RANCHING2.DESC, "Ranching2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Researching1", DUPLICANTS.TRAITS.GRANTSKILL_RESEARCHING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_RESEARCHING1.DESC, "Researching1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Researching2", DUPLICANTS.TRAITS.GRANTSKILL_RESEARCHING2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_RESEARCHING2.DESC, "Researching2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Researching3", DUPLICANTS.TRAITS.GRANTSKILL_RESEARCHING3.NAME, DUPLICANTS.TRAITS.GRANTSKILL_RESEARCHING3.DESC, "Researching3"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Researching4", DUPLICANTS.TRAITS.GRANTSKILL_RESEARCHING4.NAME, DUPLICANTS.TRAITS.GRANTSKILL_RESEARCHING4.DESC, "Researching4"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Cooking1", DUPLICANTS.TRAITS.GRANTSKILL_COOKING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_COOKING1.DESC, "Cooking1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Cooking2", DUPLICANTS.TRAITS.GRANTSKILL_COOKING2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_COOKING2.DESC, "Cooking2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Arting1", DUPLICANTS.TRAITS.GRANTSKILL_ARTING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_ARTING1.DESC, "Arting1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Arting2", DUPLICANTS.TRAITS.GRANTSKILL_ARTING2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_ARTING2.DESC, "Arting2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Arting3", DUPLICANTS.TRAITS.GRANTSKILL_ARTING3.NAME, DUPLICANTS.TRAITS.GRANTSKILL_ARTING3.DESC, "Arting3"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Hauling1", DUPLICANTS.TRAITS.GRANTSKILL_HAULING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_HAULING1.DESC, "Hauling1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Hauling2", DUPLICANTS.TRAITS.GRANTSKILL_HAULING2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_HAULING2.DESC, "Hauling2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Suits1", DUPLICANTS.TRAITS.GRANTSKILL_SUITS1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_SUITS1.DESC, "Suits1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Technicals1", DUPLICANTS.TRAITS.GRANTSKILL_TECHNICALS1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_TECHNICALS1.DESC, "Technicals1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Technicals2", DUPLICANTS.TRAITS.GRANTSKILL_TECHNICALS2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_TECHNICALS2.DESC, "Technicals2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Engineering1", DUPLICANTS.TRAITS.GRANTSKILL_ENGINEERING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_ENGINEERING1.DESC, "Engineering1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Basekeeping1", DUPLICANTS.TRAITS.GRANTSKILL_BASEKEEPING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_BASEKEEPING1.DESC, "Basekeeping1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Basekeeping2", DUPLICANTS.TRAITS.GRANTSKILL_BASEKEEPING2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_BASEKEEPING2.DESC, "Basekeeping2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Astronauting1", DUPLICANTS.TRAITS.GRANTSKILL_ASTRONAUTING1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_ASTRONAUTING1.DESC, "Astronauting1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Astronauting2", DUPLICANTS.TRAITS.GRANTSKILL_ASTRONAUTING2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_ASTRONAUTING2.DESC, "Astronauting2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Medicine1", DUPLICANTS.TRAITS.GRANTSKILL_MEDICINE1.NAME, DUPLICANTS.TRAITS.GRANTSKILL_MEDICINE1.DESC, "Medicine1"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Medicine2", DUPLICANTS.TRAITS.GRANTSKILL_MEDICINE2.NAME, DUPLICANTS.TRAITS.GRANTSKILL_MEDICINE2.DESC, "Medicine2"),
			TraitUtil.CreateSkillGrantingTrait("GrantSkill_Medicine3", DUPLICANTS.TRAITS.GRANTSKILL_MEDICINE3.NAME, DUPLICANTS.TRAITS.GRANTSKILL_MEDICINE3.DESC, "Medicine3"),
			TraitUtil.CreateNamedTrait("IronGut", DUPLICANTS.TRAITS.IRONGUT.NAME, DUPLICANTS.TRAITS.IRONGUT.DESC, positiveTrait: true),
			TraitUtil.CreateAttributeEffectTrait("StrongImmuneSystem", DUPLICANTS.TRAITS.STRONGIMMUNESYSTEM.NAME, DUPLICANTS.TRAITS.STRONGIMMUNESYSTEM.DESC, "GermResistance", 1f, positiveTrait: true),
			TraitUtil.CreateTrait("Aggressive", DUPLICANTS.TRAITS.AGGRESSIVE.NAME, DUPLICANTS.TRAITS.AGGRESSIVE.DESC, OnAddAggressive, null, positiveTrait: false, () => DUPLICANTS.TRAITS.AGGRESSIVE.NOREPAIR),
			TraitUtil.CreateTrait("UglyCrier", DUPLICANTS.TRAITS.UGLYCRIER.NAME, DUPLICANTS.TRAITS.UGLYCRIER.DESC, OnAddUglyCrier),
			TraitUtil.CreateTrait("BingeEater", DUPLICANTS.TRAITS.BINGEEATER.NAME, DUPLICANTS.TRAITS.BINGEEATER.DESC, OnAddBingeEater),
			TraitUtil.CreateTrait("StressVomiter", DUPLICANTS.TRAITS.STRESSVOMITER.NAME, DUPLICANTS.TRAITS.STRESSVOMITER.DESC, OnAddStressVomiter),
			TraitUtil.CreateTrait("BalloonArtist", DUPLICANTS.TRAITS.BALLOONARTIST.NAME, DUPLICANTS.TRAITS.BALLOONARTIST.DESC, OnAddBalloonArtist),
			TraitUtil.CreateTrait("SparkleStreaker", DUPLICANTS.TRAITS.SPARKLESTREAKER.NAME, DUPLICANTS.TRAITS.SPARKLESTREAKER.DESC, OnAddSparkleStreaker),
			TraitUtil.CreateTrait("StickerBomber", DUPLICANTS.TRAITS.STICKERBOMBER.NAME, DUPLICANTS.TRAITS.STICKERBOMBER.DESC, OnAddStickerBomber),
			TraitUtil.CreateTrait("SuperProductive", DUPLICANTS.TRAITS.SUPERPRODUCTIVE.NAME, DUPLICANTS.TRAITS.SUPERPRODUCTIVE.DESC, OnAddSuperProductive),
			TraitUtil.CreateComponentTrait<EarlyBird>("EarlyBird", DUPLICANTS.TRAITS.EARLYBIRD.NAME, DUPLICANTS.TRAITS.EARLYBIRD.DESC, positiveTrait: true, () => string.Format(DUPLICANTS.TRAITS.EARLYBIRD.EXTENDED_DESC, GameUtil.AddPositiveSign(EARLYBIRD_MODIFIER.ToString(), positive: true))),
			TraitUtil.CreateComponentTrait<NightOwl>("NightOwl", DUPLICANTS.TRAITS.NIGHTOWL.NAME, DUPLICANTS.TRAITS.NIGHTOWL.DESC, positiveTrait: true, () => string.Format(DUPLICANTS.TRAITS.NIGHTOWL.EXTENDED_DESC, GameUtil.AddPositiveSign(NIGHTOWL_MODIFIER.ToString(), positive: true))),
			TraitUtil.CreateComponentTrait<Claustrophobic>("Claustrophobic", DUPLICANTS.TRAITS.NEEDS.CLAUSTROPHOBIC.NAME, DUPLICANTS.TRAITS.NEEDS.CLAUSTROPHOBIC.DESC),
			TraitUtil.CreateComponentTrait<PrefersWarmer>("PrefersWarmer", DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.NAME, DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.DESC),
			TraitUtil.CreateComponentTrait<PrefersColder>("PrefersColder", DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.NAME, DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.DESC),
			TraitUtil.CreateComponentTrait<SensitiveFeet>("SensitiveFeet", DUPLICANTS.TRAITS.NEEDS.SENSITIVEFEET.NAME, DUPLICANTS.TRAITS.NEEDS.SENSITIVEFEET.DESC),
			TraitUtil.CreateComponentTrait<Fashionable>("Fashionable", DUPLICANTS.TRAITS.NEEDS.FASHIONABLE.NAME, DUPLICANTS.TRAITS.NEEDS.FASHIONABLE.DESC),
			TraitUtil.CreateComponentTrait<Climacophobic>("Climacophobic", DUPLICANTS.TRAITS.NEEDS.CLIMACOPHOBIC.NAME, DUPLICANTS.TRAITS.NEEDS.CLIMACOPHOBIC.DESC),
			TraitUtil.CreateComponentTrait<SolitarySleeper>("SolitarySleeper", DUPLICANTS.TRAITS.NEEDS.SOLITARYSLEEPER.NAME, DUPLICANTS.TRAITS.NEEDS.SOLITARYSLEEPER.DESC),
			TraitUtil.CreateComponentTrait<Workaholic>("Workaholic", DUPLICANTS.TRAITS.NEEDS.WORKAHOLIC.NAME, DUPLICANTS.TRAITS.NEEDS.WORKAHOLIC.DESC)
		};

		private static void OnAddStressVomiter(GameObject go)
		{
			Notification notification = new Notification(DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => string.Concat(DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_TOOLTIP, notificationList.ReduceMessages(countNames: false)));
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalVomiter", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_VOMITER.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_VOMITER.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_interrupt_vomiter_kanim", new HashedString[1] { "interrupt_vomiter" }, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new VomitChore(Db.Get().ChoreTypes.StressVomit, chore_provider, Db.Get().DuplicantStatusItems.Vomiting, notification), "anim_loco_vomiter_kanim").StartSM();
		}

		private static void OnAddAggressive(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalAggresive", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_AGGRESIVE.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_AGGRESIVE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_interrupt_destructive_kanim", new HashedString[1] { "interrupt_destruct" }, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new AggressiveChore(chore_provider), "anim_loco_destructive_kanim").StartSM();
		}

		private static void OnAddUglyCrier(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalUglyCrier", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_UGLY_CRIER.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_UGLY_CRIER.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_cry_kanim", new HashedString[3] { "working_pre", "working_loop", "working_pst" }, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new UglyCryChore(Db.Get().ChoreTypes.UglyCry, chore_provider), "anim_loco_cry_kanim").StartSM();
		}

		private static void OnAddBingeEater(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalBingeEater", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_BINGE_EAT.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_BINGE_EAT.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_interrupt_binge_eat_kanim", new HashedString[1] { "interrupt_binge_eat" }, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new BingeEatChore(chore_provider), "anim_loco_binge_eat_kanim", 8f).StartSM();
		}

		private static void OnAddBalloonArtist(GameObject go)
		{
			new BalloonArtist.Instance(go.GetComponent<KMonoBehaviour>()).StartSM();
			new JoyBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), "anim_loco_happy_balloon_stickers_kanim", null, Db.Get().Expressions.Balloon).StartSM();
		}

		private static void OnAddSparkleStreaker(GameObject go)
		{
			new SparkleStreaker.Instance(go.GetComponent<KMonoBehaviour>()).StartSM();
			new JoyBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), "anim_loco_sparkle_kanim", null, Db.Get().Expressions.Sparkle).StartSM();
		}

		private static void OnAddStickerBomber(GameObject go)
		{
			new StickerBomber.Instance(go.GetComponent<KMonoBehaviour>()).StartSM();
			new JoyBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), "anim_loco_stickers", null, Db.Get().Expressions.Sticker).StartSM();
		}

		private static void OnAddSuperProductive(GameObject go)
		{
			new SuperProductive.Instance(go.GetComponent<KMonoBehaviour>()).StartSM();
			new JoyBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), "anim_loco_productive_kanim", "anim_loco_walk_productive_kanim", Db.Get().Expressions.Productive).StartSM();
		}
	}
}
