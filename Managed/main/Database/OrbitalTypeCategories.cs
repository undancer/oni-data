namespace Database
{
	public class OrbitalTypeCategories : ResourceSet<OrbitalData>
	{
		public OrbitalData backgroundEarth;

		public OrbitalData frozenOre;

		public OrbitalData heliumCloud;

		public OrbitalData iceCloud;

		public OrbitalData iceRock;

		public OrbitalData purpleGas;

		public OrbitalData radioactiveGas;

		public OrbitalData rocky;

		public OrbitalData gravitas;

		public OrbitalData orbit;

		public OrbitalData landed;

		public OrbitalTypeCategories(ResourceSet parent)
			: base("OrbitalTypeCategories", parent)
		{
			backgroundEarth = new OrbitalData("backgroundEarth", this, "earth_kanim", "", OrbitalData.OrbitalType.world, 1f, 0.5f, 0.95f, 10f, 10f);
			frozenOre = new OrbitalData("frozenOre", this, "starmap_frozen_ore_kanim", "", OrbitalData.OrbitalType.poi, 1f, 0.5f, 0.5f, -350f, 350f, 1f);
			heliumCloud = new OrbitalData("heliumCloud", this, "starmap_helium_cloud_kanim");
			iceCloud = new OrbitalData("iceCloud", this, "starmap_ice_cloud_kanim");
			iceRock = new OrbitalData("iceRock", this, "starmap_ice_kanim");
			purpleGas = new OrbitalData("purpleGas", this, "starmap_purple_gas_kanim");
			radioactiveGas = new OrbitalData("radioactiveGas", this, "starmap_radioactive_gas_kanim");
			rocky = new OrbitalData("rocky", this, "starmap_rocky_kanim");
			gravitas = new OrbitalData("gravitas", this, "starmap_space_junk_kanim");
			orbit = new OrbitalData("orbit", this, "starmap_orbit_kanim", "", OrbitalData.OrbitalType.inOrbit, 1f, 0.25f, 0.5f, -350f, 350f, 1.05f, rotatesBehind: false, 0.05f, 4f);
			landed = new OrbitalData("landed", this, "starmap_landed_surface_kanim", "", OrbitalData.OrbitalType.landed, 0f, 0.5f, 0.35f, -350f, 350f, 1.05f, rotatesBehind: false, 0.05f, 4f);
		}
	}
}
