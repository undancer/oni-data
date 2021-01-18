using STRINGS;
using TUNING;
using UnityEngine;

public class PioneerLanderConfig : IEntityConfig
{
	public const string ID = "PioneerLander";

	public const string PREVIEW_ID = "PioneerLander_Preview";

	public const float MASS = 400f;

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PioneerLander", STRINGS.BUILDINGS.PREFABS.PIONEERLANDER.NAME, STRINGS.BUILDINGS.PREFABS.PIONEERLANDER.DESC, 400f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("rocket_pioneer_cargo_lander_kanim"), initialAnim: "grounded", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 3);
		CargoLander.Def def = gameObject.AddOrGetDef<CargoLander.Def>();
		def.previewTag = "PioneerLander_Preview".ToTag();
		gameObject.AddOrGet<MinionStorage>();
		gameObject.AddOrGet<Prioritizable>();
		Prioritizable.AddRef(gameObject);
		gameObject.AddOrGet<Operational>();
		Deconstructable deconstructable = gameObject.AddOrGet<Deconstructable>();
		deconstructable.audioSize = "large";
		gameObject.AddOrGet<Storable>();
		Placeable placeable = gameObject.AddOrGet<Placeable>();
		placeable.previewTag = "PioneerLander_Preview".ToTag();
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterPreview("PioneerLander_Preview", Assets.GetAnim("rocket_pioneer_cargo_lander_kanim"), "place", ObjectLayer.Building, 3, 3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.ApplyToCells = false;
		component.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
