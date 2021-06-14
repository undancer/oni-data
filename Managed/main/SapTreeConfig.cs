using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SapTreeConfig : IEntityConfig
{
	public const string ID = "SapTree";

	public static readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER5;

	private const int WIDTH = 5;

	private const int HEIGHT = 5;

	private const int ATTACK_RADIUS = 2;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SapTree", STRINGS.CREATURES.SPECIES.SAPTREE.NAME, STRINGS.CREATURES.SPECIES.SAPTREE.DESC, 1f, decor: POSITIVE_DECOR_EFFECT, anim: Assets.GetAnim("gravitas_sap_tree_kanim"), initialAnim: "idle", sceneLayer: Grid.SceneLayer.BuildingFront, width: 5, height: 5, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Decoration
		});
		SapTree.Def def = gameObject.AddOrGetDef<SapTree.Def>();
		def.foodSenseArea = new Vector2I(5, 1);
		def.massEatRate = 5f;
		def.kcalorieToKGConversionRatio = 0.0005f;
		def.stomachSize = 50f;
		def.oozeRate = 2f;
		def.oozeOffsets = new List<Vector3>
		{
			new Vector3(-2f, 2f),
			new Vector3(2f, 1f)
		};
		def.attackSenseArea = new Vector2I(5, 5);
		def.attackCooldown = 5f;
		gameObject.AddOrGet<Storage>();
		FactionAlignment factionAlignment = gameObject.AddOrGet<FactionAlignment>();
		factionAlignment.Alignment = FactionManager.FactionID.Hostile;
		factionAlignment.canBePlayerTargeted = false;
		gameObject.AddOrGet<RangedAttackable>();
		gameObject.AddWeapon(5f, 5f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.AreaOfEffect, 1, 2f);
		gameObject.AddOrGet<WiltCondition>();
		TemperatureVulnerable temperatureVulnerable = gameObject.AddOrGet<TemperatureVulnerable>();
		temperatureVulnerable.Configure(173.15f, 0f, 373.15f, 1023.15f);
		gameObject.AddOrGet<EntombVulnerable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
