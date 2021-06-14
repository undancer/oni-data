using STRINGS;
using TUNING;
using UnityEngine;

public class ShockwormConfig : IEntityConfig
{
	public const string ID = "ShockWorm";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ShockWorm", STRINGS.CREATURES.SPECIES.SHOCKWORM.NAME, STRINGS.CREATURES.SPECIES.SHOCKWORM.DESC, 50f, decor: DECOR.BONUS.TIER0, anim: Assets.GetAnim("shockworm_kanim"), initialAnim: "idle", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 2);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Hostile, null, "FlyerNavGrid1x2", NavType.Hover, 32, 2f, "Meat", 3, drownVulnerable: true, entombVulnerable: true, lethalLowTemperature: TUNING.CREATURES.TEMPERATURE.FREEZING_2, warningLowTemperature: TUNING.CREATURES.TEMPERATURE.FREEZING_1, warningHighTemperature: TUNING.CREATURES.TEMPERATURE.HOT_1, lethalHighTemperature: TUNING.CREATURES.TEMPERATURE.HOT_2);
		gameObject.AddOrGet<LoopingSounds>();
		Weapon weapon = gameObject.AddWeapon(3f, 6f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.AreaOfEffect, 10, 4f);
		weapon.AddEffect();
		SoundEventVolumeCache.instance.AddVolume("shockworm_kanim", "Shockworm_attack_arc", NOISE_POLLUTION.CREATURES.TIER6);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
