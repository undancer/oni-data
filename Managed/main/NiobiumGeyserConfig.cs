using STRINGS;
using UnityEngine;

public class NiobiumGeyserConfig : IEntityConfig
{
	public const string ID = "NiobiumGeyser";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GeyserConfigurator.GeyserType geyserType = new GeyserConfigurator.GeyserType("molten_niobium", SimHashes.MoltenNiobium, 3500f, 800f, 1600f, 150f, 6000f, 12000f, 0.005f, 0.01f);
		return GeyserGenericConfig.CreateGeyser("NiobiumGeyser", "geyser_molten_niobium_kanim", 3, 3, CREATURES.SPECIES.GEYSER.MOLTEN_NIOBIUM.NAME, CREATURES.SPECIES.GEYSER.MOLTEN_NIOBIUM.DESC, geyserType.idHash, geyserType.geyserTemperature);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
