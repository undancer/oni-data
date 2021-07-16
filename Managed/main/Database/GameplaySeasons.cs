using Klei.AI;

namespace Database
{
	public class GameplaySeasons : ResourceSet<GameplaySeason>
	{
		public GameplaySeason MeteorShowers;

		public GameplaySeason GassyMooteorShowers;

		public GameplaySeason TemporalTearMeteorShowers;

		public GameplaySeason NaturalRandomEvents;

		public GameplaySeason DupeRandomEvents;

		public GameplaySeason PrickleCropSeason;

		public GameplaySeason BonusEvents;

		public GameplaySeason RegolithMoonMeteorShowers;

		public GameplaySeasons(ResourceSet parent)
			: base("GameplaySeasons", parent)
		{
			MeteorShowers = Add(new GameplaySeason("MeteorShowers", GameplaySeason.Type.World, "", 14f, synchronizedToPeriod: false, -1f, startActive: true).AddEvent(Db.Get().GameplayEvents.MeteorShowerIronEvent).AddEvent(Db.Get().GameplayEvents.MeteorShowerGoldEvent).AddEvent(Db.Get().GameplayEvents.MeteorShowerCopperEvent));
			RegolithMoonMeteorShowers = Add(new GameplaySeason("RegolithMoonMeteorShowers", GameplaySeason.Type.World, "EXPANSION1_ID", 20f, synchronizedToPeriod: false, -1f, startActive: true).AddEvent(Db.Get().GameplayEvents.MeteorShowerDustEvent));
			TemporalTearMeteorShowers = Add(new GameplaySeason("TemporalTearMeteorShowers", GameplaySeason.Type.World, "EXPANSION1_ID", 1f, synchronizedToPeriod: false, 0f).AddEvent(Db.Get().GameplayEvents.MeteorShowerFullereneEvent));
			GassyMooteorShowers = Add(new GameplaySeason("GassyMooteorShowers", GameplaySeason.Type.World, "EXPANSION1_ID", 20f, synchronizedToPeriod: false, -1f, startActive: true).AddEvent(Db.Get().GameplayEvents.GassyMooteorEvent));
		}
	}
}
