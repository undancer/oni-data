using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ScoutLanderConfig : IEntityConfig
{
	public const string ID = "ScoutLander";

	public const string PREVIEW_ID = "ScoutLander_Preview";

	public const float MASS = 400f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ScoutLander", STRINGS.BUILDINGS.PREFABS.SCOUTLANDER.NAME, STRINGS.BUILDINGS.PREFABS.SCOUTLANDER.DESC, 400f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("rocket_scout_cargo_lander_kanim"), initialAnim: "grounded", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 3);
		CargoLander.Def def = gameObject.AddOrGetDef<CargoLander.Def>();
		def.previewTag = "ScoutLander_Preview".ToTag();
		gameObject.AddOrGet<Prioritizable>();
		Prioritizable.AddRef(gameObject);
		gameObject.AddOrGet<Operational>();
		Storage storage = gameObject.AddComponent<Storage>();
		storage.showInUI = true;
		storage.allowItemRemoval = false;
		storage.capacityKg = 2000f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		Deconstructable deconstructable = gameObject.AddOrGet<Deconstructable>();
		deconstructable.audioSize = "large";
		gameObject.AddOrGet<Storable>();
		Placeable placeable = gameObject.AddOrGet<Placeable>();
		placeable.kAnimName = "rocket_scout_cargo_lander_kanim";
		placeable.animName = "place";
		placeable.placementRules = new List<Placeable.PlacementRules>
		{
			Placeable.PlacementRules.OnFoundation,
			Placeable.PlacementRules.VisibleToSpace,
			Placeable.PlacementRules.RestrictToWorld
		};
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterPreview("ScoutLander_Preview", Assets.GetAnim("rocket_scout_cargo_lander_kanim"), "place", ObjectLayer.Building, 3, 3);
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
