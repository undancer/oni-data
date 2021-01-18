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
					"a",
					"b",
					"c",
					"d",
					"e",
					"f",
					"g",
					"h",
					"rocket",
					"paperplane",
					"plant",
					"plantpot",
					"mushroom",
					"mermaid",
					"spacepet",
					"spacepet2",
					"spacepet3",
					"spacepet4",
					"spacepet5",
					"unicorn"
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

		public static readonly List<System.Action> TRAIT_CREATORS = new List<System.Action>
		{
			CreateAttributeEffectTrait("None", DUPLICANTS.CONGENITALTRAITS.NONE.NAME, DUPLICANTS.CONGENITALTRAITS.NONE.DESC, "", NO_ATTRIBUTE_BONUS),
			CreateComponentTrait<Stinky>("Stinky", DUPLICANTS.CONGENITALTRAITS.STINKY.NAME, DUPLICANTS.CONGENITALTRAITS.STINKY.DESC),
			CreateAttributeEffectTrait("Ellie", DUPLICANTS.CONGENITALTRAITS.ELLIE.NAME, DUPLICANTS.CONGENITALTRAITS.ELLIE.DESC, "AirConsumptionRate", -0.044999998f, "DecorExpectation", -5f),
			CreateDisabledTaskTrait("Joshua", DUPLICANTS.CONGENITALTRAITS.JOSHUA.NAME, DUPLICANTS.CONGENITALTRAITS.JOSHUA.DESC, "Combat", is_valid_starter_trait: true),
			CreateComponentTrait<Stinky>("Liam", DUPLICANTS.CONGENITALTRAITS.LIAM.NAME, DUPLICANTS.CONGENITALTRAITS.LIAM.DESC),
			CreateDisabledTaskTrait("CantResearch", DUPLICANTS.TRAITS.CANTRESEARCH.NAME, DUPLICANTS.TRAITS.CANTRESEARCH.DESC, "Research", is_valid_starter_trait: true),
			CreateDisabledTaskTrait("CantBuild", DUPLICANTS.TRAITS.CANTBUILD.NAME, DUPLICANTS.TRAITS.CANTBUILD.DESC, "Build", is_valid_starter_trait: false),
			CreateDisabledTaskTrait("CantCook", DUPLICANTS.TRAITS.CANTCOOK.NAME, DUPLICANTS.TRAITS.CANTCOOK.DESC, "Cook", is_valid_starter_trait: true),
			CreateDisabledTaskTrait("CantDig", DUPLICANTS.TRAITS.CANTDIG.NAME, DUPLICANTS.TRAITS.CANTDIG.DESC, "Dig", is_valid_starter_trait: false),
			CreateDisabledTaskTrait("Hemophobia", DUPLICANTS.TRAITS.HEMOPHOBIA.NAME, DUPLICANTS.TRAITS.HEMOPHOBIA.DESC, "MedicalAid", is_valid_starter_trait: true),
			CreateDisabledTaskTrait("ScaredyCat", DUPLICANTS.TRAITS.SCAREDYCAT.NAME, DUPLICANTS.TRAITS.SCAREDYCAT.DESC, "Combat", is_valid_starter_trait: true),
			CreateNamedTrait("Allergies", DUPLICANTS.TRAITS.ALLERGIES.NAME, DUPLICANTS.TRAITS.ALLERGIES.DESC),
			CreateAttributeEffectTrait("MouthBreather", DUPLICANTS.TRAITS.MOUTHBREATHER.NAME, DUPLICANTS.TRAITS.MOUTHBREATHER.DESC, "AirConsumptionRate", 0.1f),
			CreateAttributeEffectTrait("CalorieBurner", DUPLICANTS.TRAITS.CALORIEBURNER.NAME, DUPLICANTS.TRAITS.CALORIEBURNER.DESC, "CaloriesDelta", -833.3333f),
			CreateAttributeEffectTrait("SmallBladder", DUPLICANTS.TRAITS.SMALLBLADDER.NAME, DUPLICANTS.TRAITS.SMALLBLADDER.DESC, "BladderDelta", 0.00027777778f),
			CreateAttributeEffectTrait("Anemic", DUPLICANTS.TRAITS.ANEMIC.NAME, DUPLICANTS.TRAITS.ANEMIC.DESC, "Athletics", HORRIBLE_ATTRIBUTE_PENALTY),
			CreateAttributeEffectTrait("SlowLearner", DUPLICANTS.TRAITS.SLOWLEARNER.NAME, DUPLICANTS.TRAITS.SLOWLEARNER.DESC, "Learning", BAD_ATTRIBUTE_PENALTY),
			CreateAttributeEffectTrait("NoodleArms", DUPLICANTS.TRAITS.NOODLEARMS.NAME, DUPLICANTS.TRAITS.NOODLEARMS.DESC, "Strength", BAD_ATTRIBUTE_PENALTY),
			CreateAttributeEffectTrait("InteriorDecorator", DUPLICANTS.TRAITS.INTERIORDECORATOR.NAME, DUPLICANTS.TRAITS.INTERIORDECORATOR.DESC, "Art", GOOD_ATTRIBUTE_BONUS, "DecorExpectation", -5f, positiveTrait: true),
			CreateAttributeEffectTrait("SimpleTastes", DUPLICANTS.TRAITS.SIMPLETASTES.NAME, DUPLICANTS.TRAITS.SIMPLETASTES.DESC, "FoodExpectation", 1f, positiveTrait: true),
			CreateAttributeEffectTrait("Foodie", DUPLICANTS.TRAITS.FOODIE.NAME, DUPLICANTS.TRAITS.FOODIE.DESC, "Cooking", GOOD_ATTRIBUTE_BONUS, "FoodExpectation", -1f, positiveTrait: true),
			CreateAttributeEffectTrait("Regeneration", DUPLICANTS.TRAITS.REGENERATION.NAME, DUPLICANTS.TRAITS.REGENERATION.DESC, "HitPointsDelta", 71f / (678f * (float)Math.PI)),
			CreateAttributeEffectTrait("DeeperDiversLungs", DUPLICANTS.TRAITS.DEEPERDIVERSLUNGS.NAME, DUPLICANTS.TRAITS.DEEPERDIVERSLUNGS.DESC, "AirConsumptionRate", -0.05f),
			CreateAttributeEffectTrait("SunnyDisposition", DUPLICANTS.TRAITS.SUNNYDISPOSITION.NAME, DUPLICANTS.TRAITS.SUNNYDISPOSITION.DESC, "StressDelta", -71f / (678f * (float)Math.PI), positiveTrait: false, delegate(GameObject go)
			{
				go.GetComponent<KBatchedAnimController>().AddAnimOverrides(Assets.GetAnim("anim_loco_happy_kanim"));
			}),
			CreateAttributeEffectTrait("RockCrusher", DUPLICANTS.TRAITS.ROCKCRUSHER.NAME, DUPLICANTS.TRAITS.ROCKCRUSHER.DESC, "Strength", 10f),
			CreateTrait("Uncultured", DUPLICANTS.TRAITS.UNCULTURED.NAME, DUPLICANTS.TRAITS.UNCULTURED.DESC, "DecorExpectation", 20f, new string[1]
			{
				"Art"
			}, positiveTrait: true),
			CreateNamedTrait("Archaeologist", DUPLICANTS.TRAITS.ARCHAEOLOGIST.NAME, DUPLICANTS.TRAITS.ARCHAEOLOGIST.DESC),
			CreateAttributeEffectTrait("WeakImmuneSystem", DUPLICANTS.TRAITS.WEAKIMMUNESYSTEM.NAME, DUPLICANTS.TRAITS.WEAKIMMUNESYSTEM.DESC, "GermResistance", -1f),
			CreateAttributeEffectTrait("IrritableBowel", DUPLICANTS.TRAITS.IRRITABLEBOWEL.NAME, DUPLICANTS.TRAITS.IRRITABLEBOWEL.DESC, "ToiletEfficiency", -0.5f),
			CreateComponentTrait<Flatulence>("Flatulence", DUPLICANTS.TRAITS.FLATULENCE.NAME, DUPLICANTS.TRAITS.FLATULENCE.DESC),
			CreateComponentTrait<Snorer>("Snorer", DUPLICANTS.TRAITS.SNORER.NAME, DUPLICANTS.TRAITS.SNORER.DESC),
			CreateComponentTrait<Narcolepsy>("Narcolepsy", DUPLICANTS.TRAITS.NARCOLEPSY.NAME, DUPLICANTS.TRAITS.NARCOLEPSY.DESC),
			CreateAttributeEffectTrait("Twinkletoes", DUPLICANTS.TRAITS.TWINKLETOES.NAME, DUPLICANTS.TRAITS.TWINKLETOES.DESC, "Athletics", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			CreateAttributeEffectTrait("Greasemonkey", DUPLICANTS.TRAITS.GREASEMONKEY.NAME, DUPLICANTS.TRAITS.GREASEMONKEY.DESC, "Machinery", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			CreateAttributeEffectTrait("MoleHands", DUPLICANTS.TRAITS.MOLEHANDS.NAME, DUPLICANTS.TRAITS.MOLEHANDS.DESC, "Digging", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			CreateAttributeEffectTrait("FastLearner", DUPLICANTS.TRAITS.FASTLEARNER.NAME, DUPLICANTS.TRAITS.FASTLEARNER.DESC, "Learning", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			CreateAttributeEffectTrait("DiversLung", DUPLICANTS.TRAITS.DIVERSLUNG.NAME, DUPLICANTS.TRAITS.DIVERSLUNG.DESC, "AirConsumptionRate", -0.025f, positiveTrait: true),
			CreateAttributeEffectTrait("StrongArm", DUPLICANTS.TRAITS.STRONGARM.NAME, DUPLICANTS.TRAITS.STRONGARM.DESC, "Strength", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			CreateNamedTrait("IronGut", DUPLICANTS.TRAITS.IRONGUT.NAME, DUPLICANTS.TRAITS.IRONGUT.DESC, positiveTrait: true),
			CreateAttributeEffectTrait("StrongImmuneSystem", DUPLICANTS.TRAITS.STRONGIMMUNESYSTEM.NAME, DUPLICANTS.TRAITS.STRONGIMMUNESYSTEM.DESC, "GermResistance", 1f, positiveTrait: true),
			CreateAttributeEffectTrait("BedsideManner", DUPLICANTS.TRAITS.BEDSIDEMANNER.NAME, DUPLICANTS.TRAITS.BEDSIDEMANNER.DESC, "Caring", GOOD_ATTRIBUTE_BONUS, positiveTrait: true),
			CreateTrait("Aggressive", DUPLICANTS.TRAITS.AGGRESSIVE.NAME, DUPLICANTS.TRAITS.AGGRESSIVE.DESC, OnAddAggressive, null, positiveTrait: false, () => DUPLICANTS.TRAITS.AGGRESSIVE.NOREPAIR),
			CreateTrait("UglyCrier", DUPLICANTS.TRAITS.UGLYCRIER.NAME, DUPLICANTS.TRAITS.UGLYCRIER.DESC, OnAddUglyCrier),
			CreateTrait("BingeEater", DUPLICANTS.TRAITS.BINGEEATER.NAME, DUPLICANTS.TRAITS.BINGEEATER.DESC, OnAddBingeEater),
			CreateTrait("StressVomiter", DUPLICANTS.TRAITS.STRESSVOMITER.NAME, DUPLICANTS.TRAITS.STRESSVOMITER.DESC, OnAddStressVomiter),
			CreateTrait("BalloonArtist", DUPLICANTS.TRAITS.BALLOONARTIST.NAME, DUPLICANTS.TRAITS.BALLOONARTIST.DESC, OnAddBalloonArtist),
			CreateTrait("SparkleStreaker", DUPLICANTS.TRAITS.SPARKLESTREAKER.NAME, DUPLICANTS.TRAITS.SPARKLESTREAKER.DESC, OnAddSparkleStreaker),
			CreateTrait("StickerBomber", DUPLICANTS.TRAITS.STICKERBOMBER.NAME, DUPLICANTS.TRAITS.STICKERBOMBER.DESC, OnAddStickerBomber),
			CreateTrait("SuperProductive", DUPLICANTS.TRAITS.SUPERPRODUCTIVE.NAME, DUPLICANTS.TRAITS.SUPERPRODUCTIVE.DESC, OnAddSuperProductive),
			CreateComponentTrait<EarlyBird>("EarlyBird", DUPLICANTS.TRAITS.EARLYBIRD.NAME, DUPLICANTS.TRAITS.EARLYBIRD.DESC, positiveTrait: true, () => string.Format(DUPLICANTS.TRAITS.EARLYBIRD.EXTENDED_DESC, GameUtil.AddPositiveSign(EARLYBIRD_MODIFIER.ToString(), positive: true))),
			CreateComponentTrait<NightOwl>("NightOwl", DUPLICANTS.TRAITS.NIGHTOWL.NAME, DUPLICANTS.TRAITS.NIGHTOWL.DESC, positiveTrait: true, () => string.Format(DUPLICANTS.TRAITS.NIGHTOWL.EXTENDED_DESC, GameUtil.AddPositiveSign(NIGHTOWL_MODIFIER.ToString(), positive: true))),
			CreateComponentTrait<Claustrophobic>("Claustrophobic", DUPLICANTS.TRAITS.NEEDS.CLAUSTROPHOBIC.NAME, DUPLICANTS.TRAITS.NEEDS.CLAUSTROPHOBIC.DESC),
			CreateComponentTrait<PrefersWarmer>("PrefersWarmer", DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.NAME, DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.DESC),
			CreateComponentTrait<PrefersColder>("PrefersColder", DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.NAME, DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.DESC),
			CreateComponentTrait<SensitiveFeet>("SensitiveFeet", DUPLICANTS.TRAITS.NEEDS.SENSITIVEFEET.NAME, DUPLICANTS.TRAITS.NEEDS.SENSITIVEFEET.DESC),
			CreateComponentTrait<Fashionable>("Fashionable", DUPLICANTS.TRAITS.NEEDS.FASHIONABLE.NAME, DUPLICANTS.TRAITS.NEEDS.FASHIONABLE.DESC),
			CreateComponentTrait<Climacophobic>("Climacophobic", DUPLICANTS.TRAITS.NEEDS.CLIMACOPHOBIC.NAME, DUPLICANTS.TRAITS.NEEDS.CLIMACOPHOBIC.DESC),
			CreateComponentTrait<SolitarySleeper>("SolitarySleeper", DUPLICANTS.TRAITS.NEEDS.SOLITARYSLEEPER.NAME, DUPLICANTS.TRAITS.NEEDS.SOLITARYSLEEPER.DESC),
			CreateComponentTrait<Workaholic>("Workaholic", DUPLICANTS.TRAITS.NEEDS.WORKAHOLIC.NAME, DUPLICANTS.TRAITS.NEEDS.WORKAHOLIC.DESC)
		};

		private static System.Action CreateDisabledTaskTrait(string id, string name, string desc, string disabled_chore_group, bool is_valid_starter_trait)
		{
			return delegate
			{
				ChoreGroup[] disabled_chore_groups = new ChoreGroup[1]
				{
					Db.Get().ChoreGroups.Get(disabled_chore_group)
				};
				Db.Get().CreateTrait(id, name, desc, null, should_save: true, disabled_chore_groups, positive_trait: false, is_valid_starter_trait);
			};
		}

		private static System.Action CreateTrait(string id, string name, string desc, string attributeId, float delta, string[] chore_groups, bool positiveTrait = false)
		{
			return delegate
			{
				List<ChoreGroup> list = new List<ChoreGroup>();
				string[] array = chore_groups;
				foreach (string id2 in array)
				{
					list.Add(Db.Get().ChoreGroups.Get(id2));
				}
				Db.Get().CreateTrait(id, name, desc, null, should_save: true, list.ToArray(), positiveTrait, is_valid_starter_trait: true).Add(new AttributeModifier(attributeId, delta, name));
			};
		}

		private static System.Action CreateAttributeEffectTrait(string id, string name, string desc, string attributeId, float delta, string attributeId2, float delta2, bool positiveTrait = false)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait: true);
				trait.Add(new AttributeModifier(attributeId, delta, name));
				trait.Add(new AttributeModifier(attributeId2, delta2, name));
			};
		}

		private static System.Action CreateAttributeEffectTrait(string id, string name, string desc, string attributeId, float delta, bool positiveTrait = false, Action<GameObject> on_add = null)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait: true);
				trait.Add(new AttributeModifier(attributeId, delta, name));
				trait.OnAddTrait = on_add;
			};
		}

		private static System.Action CreateEffectModifierTrait(string id, string name, string desc, string[] ignoredEffects, bool positiveTrait = false)
		{
			return delegate
			{
				Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait: true).AddIgnoredEffects(ignoredEffects);
			};
		}

		private static System.Action CreateNamedTrait(string id, string name, string desc, bool positiveTrait = false)
		{
			return delegate
			{
				Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait: true);
			};
		}

		private static System.Action CreateTrait(string id, string name, string desc, Action<GameObject> on_add, ChoreGroup[] disabled_chore_groups = null, bool positiveTrait = false, Func<string> extendedDescFn = null)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, disabled_chore_groups, positiveTrait, is_valid_starter_trait: true);
				trait.OnAddTrait = on_add;
				if (extendedDescFn != null)
				{
					trait.ExtendedTooltip = (Func<string>)Delegate.Combine(trait.ExtendedTooltip, extendedDescFn);
				}
			};
		}

		private static System.Action CreateComponentTrait<T>(string id, string name, string desc, bool positiveTrait = false, Func<string> extendedDescFn = null) where T : KMonoBehaviour
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, should_save: true, null, positiveTrait, is_valid_starter_trait: true);
				trait.OnAddTrait = delegate(GameObject go)
				{
					go.FindOrAddUnityComponent<T>();
				};
				if (extendedDescFn != null)
				{
					trait.ExtendedTooltip = (Func<string>)Delegate.Combine(trait.ExtendedTooltip, extendedDescFn);
				}
			};
		}

		private static void OnAddStressVomiter(GameObject go)
		{
			Notification notification = new Notification(DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => string.Concat(DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_TOOLTIP, notificationList.ReduceMessages(countNames: false)));
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalVomiter", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_VOMITER.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_VOMITER.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_interrupt_vomiter_kanim", new HashedString[1]
			{
				"interrupt_vomiter"
			}, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new VomitChore(Db.Get().ChoreTypes.StressVomit, chore_provider, Db.Get().DuplicantStatusItems.Vomiting, notification), "anim_loco_vomiter_kanim").StartSM();
		}

		private static void OnAddAggressive(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalAggresive", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_AGGRESIVE.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_AGGRESIVE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_interrupt_destructive_kanim", new HashedString[1]
			{
				"interrupt_destruct"
			}, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new AggressiveChore(chore_provider), "anim_loco_destructive_kanim").StartSM();
		}

		private static void OnAddUglyCrier(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalUglyCrier", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_UGLY_CRIER.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_UGLY_CRIER.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_cry_kanim", new HashedString[3]
			{
				"working_pre",
				"working_loop",
				"working_pst"
			}, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new UglyCryChore(Db.Get().ChoreTypes.UglyCry, chore_provider), "anim_loco_cry_kanim").StartSM();
		}

		private static void OnAddBingeEater(GameObject go)
		{
			StatusItem tierOneBehaviourStatusItem = new StatusItem("StressSignalBingeEater", DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_BINGE_EAT.NAME, DUPLICANTS.STATUSITEMS.STRESS_SIGNAL_BINGE_EAT.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			new StressBehaviourMonitor.Instance(go.GetComponent<KMonoBehaviour>(), (ChoreProvider chore_provider) => new StressEmoteChore(chore_provider, Db.Get().ChoreTypes.StressEmote, "anim_interrupt_binge_eat_kanim", new HashedString[1]
			{
				"interrupt_binge_eat"
			}, KAnim.PlayMode.Once, () => tierOneBehaviourStatusItem), (ChoreProvider chore_provider) => new BingeEatChore(chore_provider), "anim_loco_binge_eat_kanim", 8f).StartSM();
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
