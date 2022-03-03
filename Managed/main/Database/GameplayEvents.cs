using Klei.AI;
using STRINGS;

namespace Database
{
	public class GameplayEvents : ResourceSet<GameplayEvent>
	{
		public GameplayEvent HatchSpawnEvent;

		public GameplayEvent PartyEvent;

		public GameplayEvent EclipseEvent;

		public GameplayEvent SatelliteCrashEvent;

		public GameplayEvent FoodFightEvent;

		public GameplayEvent MeteorShowerIronEvent;

		public GameplayEvent MeteorShowerGoldEvent;

		public GameplayEvent MeteorShowerCopperEvent;

		public GameplayEvent MeteorShowerFullereneEvent;

		public GameplayEvent MeteorShowerDustEvent;

		public GameplayEvent GassyMooteorEvent;

		public GameplayEvent PrickleFlowerBlightEvent;

		public GameplayEvent BonusDream1;

		public GameplayEvent BonusDream2;

		public GameplayEvent BonusDream3;

		public GameplayEvent BonusDream4;

		public GameplayEvent BonusToilet1;

		public GameplayEvent BonusToilet2;

		public GameplayEvent BonusToilet3;

		public GameplayEvent BonusToilet4;

		public GameplayEvent BonusResearch;

		public GameplayEvent BonusDigging1;

		public GameplayEvent BonusStorage;

		public GameplayEvent BonusBuilder;

		public GameplayEvent BonusOxygen;

		public GameplayEvent BonusAlgae;

		public GameplayEvent BonusGenerator;

		public GameplayEvent BonusDoor;

		public GameplayEvent BonusHitTheBooks;

		public GameplayEvent BonusLitWorkspace;

		public GameplayEvent BonusTalker;

		public GameplayEvent CryoFriend;

		public GameplayEvent WarpWorldReveal;

		public GameplayEvent ArtifactReveal;

		public GameplayEvents(ResourceSet parent)
			: base("GameplayEvents", parent)
		{
			HatchSpawnEvent = Add(new CreatureSpawnEvent());
			PartyEvent = Add(new PartyEvent());
			EclipseEvent = Add(new EclipseEvent());
			SatelliteCrashEvent = Add(new SatelliteCrashEvent());
			FoodFightEvent = Add(new FoodFightEvent());
			MeteorShowerIronEvent = Add(new MeteorShowerEvent("MeteorShowerIronEvent", 6000f, 1.25f, secondsBombardmentOn: new MathUtil.MinMax(100f, 400f), secondsBombardmentOff: new MathUtil.MinMax(300f, 1200f)).AddMeteor(IronCometConfig.ID, 1f).AddMeteor(RockCometConfig.ID, 2f).AddMeteor(DustCometConfig.ID, 5f));
			MeteorShowerGoldEvent = Add(new MeteorShowerEvent("MeteorShowerGoldEvent", 3000f, 0.4f, secondsBombardmentOn: new MathUtil.MinMax(50f, 100f), secondsBombardmentOff: new MathUtil.MinMax(800f, 1200f)).AddMeteor(GoldCometConfig.ID, 2f).AddMeteor(RockCometConfig.ID, 0.5f).AddMeteor(DustCometConfig.ID, 5f));
			MeteorShowerCopperEvent = Add(new MeteorShowerEvent("MeteorShowerCopperEvent", 4200f, 5.5f, secondsBombardmentOn: new MathUtil.MinMax(100f, 400f), secondsBombardmentOff: new MathUtil.MinMax(300f, 1200f)).AddMeteor(CopperCometConfig.ID, 1f).AddMeteor(RockCometConfig.ID, 1f));
			MeteorShowerFullereneEvent = Add(new MeteorShowerEvent("MeteorShowerFullereneEvent", 30f, 0.66f, secondsBombardmentOn: new MathUtil.MinMax(80f, 80f), secondsBombardmentOff: new MathUtil.MinMax(1f, 1f)).AddMeteor(FullereneCometConfig.ID, 6f).AddMeteor(RockCometConfig.ID, 1f).AddMeteor(DustCometConfig.ID, 1f));
			MeteorShowerDustEvent = Add(new MeteorShowerEvent("MeteorShowerDustEvent", 9000f, 2f, secondsBombardmentOn: new MathUtil.MinMax(100f, 400f), secondsBombardmentOff: new MathUtil.MinMax(300f, 1200f)).AddMeteor(RockCometConfig.ID, 1f).AddMeteor(DustCometConfig.ID, 5f));
			GassyMooteorEvent = Add(new MeteorShowerEvent("GassyMooteorEvent", 15f, 5f, secondsBombardmentOn: new MathUtil.MinMax(15f, 15f), secondsBombardmentOff: new MathUtil.MinMax(1f, 1f)).AddMeteor(GassyMooCometConfig.ID, 1f));
			PrickleFlowerBlightEvent = Add(new PlantBlightEvent("PrickleFlowerBlightEvent", "PrickleFlower", 3600f, 30f));
			CryoFriend = Add(new SimpleEvent("CryoFriend", GAMEPLAY_EVENTS.EVENT_TYPES.CRYOFRIEND.NAME, GAMEPLAY_EVENTS.EVENT_TYPES.CRYOFRIEND.DESCRIPTION, GAMEPLAY_EVENTS.EVENT_TYPES.CRYOFRIEND.BUTTON).SetVisuals(null, "cryofriend_kanim"));
			WarpWorldReveal = Add(new SimpleEvent("WarpWorldReveal", GAMEPLAY_EVENTS.EVENT_TYPES.WARPWORLDREVEAL.NAME, GAMEPLAY_EVENTS.EVENT_TYPES.WARPWORLDREVEAL.DESCRIPTION, GAMEPLAY_EVENTS.EVENT_TYPES.WARPWORLDREVEAL.BUTTON).SetVisuals(null, "warpworldreveal_kanim"));
			ArtifactReveal = Add(new SimpleEvent("ArtifactReveal", GAMEPLAY_EVENTS.EVENT_TYPES.ARTIFACT_REVEAL.NAME, GAMEPLAY_EVENTS.EVENT_TYPES.ARTIFACT_REVEAL.DESCRIPTION, GAMEPLAY_EVENTS.EVENT_TYPES.ARTIFACT_REVEAL.BUTTON).SetVisuals("event_bg_01", "analyzeartifact_kanim"));
			BonusEvents();
		}

		private void BonusEvents()
		{
			GameplayEventMinionFilters instance = GameplayEventMinionFilters.Instance;
			GameplayEventPreconditions instance2 = GameplayEventPreconditions.Instance;
			Skills skills = Db.Get().Skills;
			RoomTypes roomTypes = Db.Get().RoomTypes;
			BonusDream1 = Add(new BonusEvent("BonusDream1").TriggerOnUseBuilding(1, BedConfig.ID, LuxuryBedConfig.ID).SetRoomConstraints(false, roomTypes.Barracks).AddPrecondition(instance2.BuildingExists(BedConfig.ID, 2))
				.AddPriorityBoost(instance2.BuildingExists(BedConfig.ID, 5), 1)
				.AddPriorityBoost(instance2.BuildingExists(LuxuryBedConfig.ID), 5)
				.TrySpawnEventOnSuccess("BonusDream2"));
			BonusDream2 = Add(new BonusEvent("BonusDream2", null, 1, preSelectMinion: false, 10).TriggerOnUseBuilding(10, BedConfig.ID, LuxuryBedConfig.ID).AddPrecondition(instance2.PastEventCountAndNotActive(BonusDream1)).AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Barracks), instance2.RoomBuilt(roomTypes.Bedroom)))
				.AddPriorityBoost(instance2.BuildingExists(LuxuryBedConfig.ID), 5)
				.TrySpawnEventOnSuccess("BonusDream3"));
			BonusDream3 = Add(new BonusEvent("BonusDream3", null, 1, preSelectMinion: false, 20).TriggerOnUseBuilding(10, BedConfig.ID, LuxuryBedConfig.ID).AddPrecondition(instance2.PastEventCountAndNotActive(BonusDream2)).AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Barracks), instance2.RoomBuilt(roomTypes.Bedroom)))
				.TrySpawnEventOnSuccess("BonusDream4"));
			BonusDream4 = Add(new BonusEvent("BonusDream4", null, 1, preSelectMinion: false, 30).TriggerOnUseBuilding(10, LuxuryBedConfig.ID).AddPrecondition(instance2.PastEventCountAndNotActive(BonusDream2)).AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Barracks), instance2.RoomBuilt(roomTypes.Bedroom))));
			BonusToilet1 = Add(new BonusEvent("BonusToilet1").TriggerOnUseBuilding(1, "Outhouse", "FlushToilet").AddPrecondition(instance2.Or(instance2.BuildingExists("Outhouse", 2), instance2.BuildingExists("FlushToilet"))).AddPrecondition(instance2.Or(instance2.BuildingExists("WashBasin", 2), instance2.BuildingExists("WashSink")))
				.AddPriorityBoost(instance2.BuildingExists("FlushToilet"), 1)
				.TrySpawnEventOnSuccess("BonusToilet2"));
			BonusToilet2 = Add(new BonusEvent("BonusToilet2", null, 1, preSelectMinion: false, 10).TriggerOnUseBuilding(5, "FlushToilet").AddPrecondition(instance2.BuildingExists("FlushToilet")).AddPrecondition(instance2.PastEventCountAndNotActive(BonusToilet1))
				.AddPriorityBoost(instance2.BuildingExists("FlushToilet", 2), 5)
				.TrySpawnEventOnSuccess("BonusToilet3"));
			BonusToilet3 = Add(new BonusEvent("BonusToilet3", null, 1, preSelectMinion: false, 20).TriggerOnUseBuilding(5, "FlushToilet").SetRoomConstraints(false, roomTypes.Latrine, roomTypes.PlumbedBathroom).AddPrecondition(instance2.PastEventCountAndNotActive(BonusToilet2))
				.AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Latrine), instance2.RoomBuilt(roomTypes.PlumbedBathroom)))
				.AddPriorityBoost(instance2.BuildingExists("FlushToilet", 2), 10)
				.TrySpawnEventOnSuccess("BonusToilet4"));
			BonusToilet4 = Add(new BonusEvent("BonusToilet4", null, 1, preSelectMinion: false, 30).TriggerOnUseBuilding(5, "FlushToilet").SetRoomConstraints(false, roomTypes.PlumbedBathroom).AddPrecondition(instance2.PastEventCountAndNotActive(BonusToilet3))
				.AddPrecondition(instance2.RoomBuilt(roomTypes.PlumbedBathroom)));
			BonusResearch = Add(new BonusEvent("BonusResearch").AddPrecondition(instance2.BuildingExists("ResearchCenter")).AddPrecondition(instance2.ResearchCompleted("FarmingTech")).AddMinionFilter(instance.HasSkillAptitude(skills.Researching1)));
			BonusDigging1 = Add(new BonusEvent("BonusDigging1", null, 1, preSelectMinion: true).TriggerOnWorkableComplete(30, typeof(Diggable)).AddMinionFilter(instance.Or(instance.HasChoreGroupPriorityOrHigher(Db.Get().ChoreGroups.Dig, 4), instance.HasSkillAptitude(skills.Mining1))).AddPriorityBoost(instance2.MinionsWithChoreGroupPriorityOrGreater(Db.Get().ChoreGroups.Dig, 1, 4), 1));
			BonusStorage = Add(new BonusEvent("BonusStorage", null, 1, preSelectMinion: true).TriggerOnUseBuilding(10, "StorageLocker").AddMinionFilter(instance.Or(instance.HasChoreGroupPriorityOrHigher(Db.Get().ChoreGroups.Hauling, 4), instance.HasSkillAptitude(skills.Hauling1))).AddPrecondition(instance2.BuildingExists("StorageLocker")));
			BonusBuilder = Add(new BonusEvent("BonusBuilder", null, 1, preSelectMinion: true).TriggerOnNewBuilding(10).AddMinionFilter(instance.Or(instance.HasChoreGroupPriorityOrHigher(Db.Get().ChoreGroups.Build, 4), instance.HasSkillAptitude(skills.Building1))));
			BonusOxygen = Add(new BonusEvent("BonusOxygen").TriggerOnUseBuilding(1, "MineralDeoxidizer").AddPrecondition(instance2.BuildingExists("MineralDeoxidizer")).AddPrecondition(instance2.Not(instance2.PastEventCount("BonusAlgae"))));
			BonusAlgae = Add(new BonusEvent("BonusAlgae", "BonusOxygen").TriggerOnUseBuilding(1, "AlgaeHabitat").AddPrecondition(instance2.BuildingExists("AlgaeHabitat")).AddPrecondition(instance2.Not(instance2.PastEventCount("BonusOxygen"))));
			BonusGenerator = Add(new BonusEvent("BonusGenerator").TriggerOnUseBuilding(1, "ManualGenerator").AddPrecondition(instance2.BuildingExists("ManualGenerator")));
			BonusDoor = Add(new BonusEvent("BonusDoor").TriggerOnUseBuilding(1, "Door").SetExtraCondition((BonusEvent.GameplayEventData data) => data.building.GetComponent<Door>().RequestedState == Door.ControlState.Locked).AddPrecondition(instance2.RoomBuilt(roomTypes.Barracks)));
			BonusHitTheBooks = Add(new BonusEvent("BonusHitTheBooks", null, 1, preSelectMinion: true).TriggerOnWorkableComplete(1, typeof(ResearchCenter), typeof(NuclearResearchCenterWorkable)).AddPrecondition(instance2.BuildingExists("ResearchCenter")).AddMinionFilter(instance.HasSkillAptitude(skills.Researching1)));
			BonusLitWorkspace = Add(new BonusEvent("BonusLitWorkspace").TriggerOnWorkableComplete(1).SetExtraCondition((BonusEvent.GameplayEventData data) => data.workable.currentlyLit).AddPrecondition(instance2.CycleRestriction(10f)));
			BonusTalker = Add(new BonusEvent("BonusTalker", null, 1, preSelectMinion: true).TriggerOnWorkableComplete(3, typeof(SocialGatheringPointWorkable)).SetExtraCondition((BonusEvent.GameplayEventData data) => (data.workable as SocialGatheringPointWorkable).timesConversed > 0).AddPrecondition(instance2.CycleRestriction(10f)));
		}

		private void VerifyEvents()
		{
			foreach (GameplayEvent resource in resources)
			{
				_ = resource.popupAnimFileName == null;
				if (resource is BonusEvent)
				{
					VerifyBonusEvent(resource as BonusEvent);
				}
			}
		}

		private void VerifyBonusEvent(BonusEvent e)
		{
			if (!Strings.TryGet("STRINGS.GAMEPLAY_EVENTS.BONUS." + e.Id.ToUpper() + ".NAME", out var result))
			{
				DebugUtil.DevLogError("Event [" + e.Id + "]: STRINGS.GAMEPLAY_EVENTS.BONUS." + e.Id.ToUpper() + " is missing");
			}
			Effect effect = Db.Get().effects.TryGet(e.effect);
			if (effect == null)
			{
				DebugUtil.DevLogError("Effect " + e.effect + "[" + e.Id + "]: Missing from spreadsheet");
				return;
			}
			if (!Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".NAME", out result))
			{
				DebugUtil.DevLogError("Effect " + e.effect + "[" + e.Id + "]: STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".NAME is missing");
			}
			if (!Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".TOOLTIP", out result))
			{
				DebugUtil.DevLogError("Effect " + e.effect + "[" + e.Id + "]: STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".TOOLTIP is missing");
			}
		}
	}
}
