using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PioneerLanderConfig : IEntityConfig
{
	public const string ID = "PioneerLander";

	public const string PREVIEW_ID = "PioneerLander_Preview";

	public const float MASS = 400f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("PioneerLander", STRINGS.BUILDINGS.PREFABS.PIONEERLANDER.NAME, STRINGS.BUILDINGS.PREFABS.PIONEERLANDER.DESC, 400f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("rocket_pioneer_cargo_lander_kanim"), initialAnim: "grounded", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 3);
		obj.AddOrGetDef<CargoLander.Def>().previewTag = "PioneerLander_Preview".ToTag();
		obj.AddOrGet<MinionStorage>();
		obj.AddOrGet<Prioritizable>();
		Prioritizable.AddRef(obj);
		obj.AddOrGet<Operational>();
		obj.AddOrGet<Deconstructable>().audioSize = "large";
		obj.AddOrGet<Storable>();
		Placeable placeable = obj.AddOrGet<Placeable>();
		placeable.kAnimName = "rocket_pioneer_cargo_lander_kanim";
		placeable.animName = "place";
		placeable.placementRules = new List<Placeable.PlacementRules>
		{
			Placeable.PlacementRules.OnFoundation,
			Placeable.PlacementRules.VisibleToSpace,
			Placeable.PlacementRules.RestrictToWorld
		};
		EntityTemplates.CreateAndRegisterPreview("PioneerLander_Preview", Assets.GetAnim("rocket_pioneer_cargo_lander_kanim"), "place", ObjectLayer.Building, 3, 3);
		return obj;
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
