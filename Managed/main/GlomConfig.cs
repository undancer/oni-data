using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class GlomConfig : IEntityConfig
{
	public const string ID = "Glom";

	public const string BASE_TRAIT_ID = "GlomBaseTrait";

	public const SimHashes dirtyEmitElement = SimHashes.ContaminatedOxygen;

	public const float dirtyProbabilityPercent = 25f;

	public const float dirtyCellToTargetMass = 1f;

	public const float dirtyMassPerDirty = 0.2f;

	public const float dirtyMassReleaseOnDeath = 3f;

	public const string emitDisease = "SlimeLung";

	public const int emitDiseasePerKg = 1000;

	public GameObject CreatePrefab()
	{
		string text = STRINGS.CREATURES.SPECIES.GLOM.NAME;
		GameObject obj = EntityTemplates.CreatePlacedEntity("Glom", text, STRINGS.CREATURES.SPECIES.GLOM.DESC, 25f, decor: DECOR.BONUS.TIER0, anim: Assets.GetAnim("glom_kanim"), initialAnim: "idle_loop", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 1);
		Db.Get().CreateTrait("GlomBaseTrait", text, text, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true).Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, text));
		KPrefabID component = obj.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		EntityTemplates.ExtendEntityToBasicCreature(obj, FactionManager.FactionID.Pest, "GlomBaseTrait", "WalkerNavGrid1x1", NavType.Floor, 32, 2f, "", 0, drownVulnerable: true, entombVulnerable: true, 293.15f, 393.15f, 273.15f, 423.15f);
		obj.AddWeapon(1f, 1f);
		obj.AddOrGet<Trappable>();
		obj.AddOrGetDef<ThreatMonitor.Def>();
		obj.AddOrGetDef<CreatureFallMonitor.Def>();
		ElementDropperMonitor.Def def = obj.AddOrGetDef<ElementDropperMonitor.Def>();
		def.dirtyEmitElement = SimHashes.ContaminatedOxygen;
		def.dirtyProbabilityPercent = 25f;
		def.dirtyCellToTargetMass = 1f;
		def.dirtyMassPerDirty = 0.2f;
		def.dirtyMassReleaseOnDeath = 3f;
		def.emitDiseaseIdx = Db.Get().Diseases.GetIndex("SlimeLung");
		def.emitDiseasePerKg = 1000f;
		obj.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = 0;
		obj.AddOrGet<LoopingSounds>();
		obj.GetComponent<LoopingSounds>().updatePosition = true;
		obj.AddOrGet<DiseaseSourceVisualizer>().alwaysShowDisease = "SlimeLung";
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_movement_short", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_jump", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_land", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_expel", NOISE_POLLUTION.CREATURES.TIER4);
		EntityTemplates.CreateAndRegisterBaggedCreature(obj, must_stand_on_top_for_pickup: true, allow_mark_for_capture: false);
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new TrappedStates.Def()).Add(new BaggedStates.Def())
			.Add(new FallStates.Def())
			.Add(new StunnedStates.Def())
			.Add(new DrowningStates.Def())
			.Add(new DebugGoToStates.Def())
			.Add(new FleeStates.Def())
			.Add(new DropElementStates.Def())
			.Add(new IdleStates.Def());
		EntityTemplates.AddCreatureBrain(obj, chore_table, GameTags.Creatures.Species.GlomSpecies, null);
		return obj;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
