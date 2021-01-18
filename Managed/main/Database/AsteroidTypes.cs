namespace Database
{
	public class AsteroidTypes : ResourceSet<AsteroidType>
	{
		public AsteroidType DefaultType;

		public AsteroidTypes(ResourceSet parent)
			: base("AsteroidTypes", parent)
		{
			DefaultType = Add(new AsteroidType("SandstoneStart", parent, "asteroid_sandstone_start_kanim"));
			Add(new AsteroidType("SwampStart", parent, "asteroid_swamp_start_kanim"));
			Add(new AsteroidType("IcePlanet", parent, "asteroid_ice_planet_kanim"));
			Add(new AsteroidType("SandstoneRocket", parent, "asteroid_sandstone_rocket_kanim"));
			Add(new AsteroidType("SwampRocket", parent, "asteroid_swamp_rocket_kanim"));
			Add(new AsteroidType("OilPlanet", parent, "asteroid_oil_planet_kanim"));
			Add(new AsteroidType("SandstoneTeleport", parent, "asteroid_swamp_oil_kanim"));
			Add(new AsteroidType("SwampTeleport", parent, "asteroid_swamp_teleport_kanim"));
			Add(new AsteroidType("NiobiumPlanet", parent, "asteroid_niobium_planet_kanim"));
			Add(new AsteroidType("MarshPlanet", parent, "asteroid_marshy_moonlet_kanim"));
		}

		public AsteroidType GetTypeOrDefault(string asteroidTypeId)
		{
			AsteroidType asteroidType = TryGet(asteroidTypeId);
			if (asteroidType == null)
			{
				if (!string.IsNullOrEmpty(asteroidTypeId))
				{
					Debug.LogWarning("Missing asteroid type " + asteroidTypeId + ". Using default");
				}
				asteroidType = DefaultType;
			}
			return asteroidType;
		}
	}
}
