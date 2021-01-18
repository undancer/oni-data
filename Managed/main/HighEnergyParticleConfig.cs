using STRINGS;
using UnityEngine;

public class HighEnergyParticleConfig : IEntityConfig
{
	public const int PARTICLE_SPEED = 8;

	public const float PARTICLE_COLLISION_SIZE = 0.2f;

	public const int PER_CELL_FALLOFF = 50;

	public const float RADIOACTIVE_RATIO = 0.5f;

	public const short EMITTER_RADIUS = 2;

	public const float EMITTER_RATE = 0.2f;

	public const float EMITTER_SPEED = 1f;

	public const float BLACKHOLE_EMIT_DURRATION = 1f;

	public const short BLACKHOLE_EMIT_RADIUS = 6;

	public const float EXPLOSION_EMIT_DURRATION = 1f;

	public const short EXPLOSION_EMIT_RADIUS = 6;

	public const string ID = "HighEnergyParticle";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity("HighEnergyParticle", ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, ITEMS.RADIATION.HIGHENERGYPARITCLE.DESC, 1f, unitMass: false, Assets.GetAnim("spark_radial_high_energy_particles_kanim"), "travel_pre", Grid.SceneLayer.FXFront2);
		EntityTemplates.AddCollision(gameObject, EntityTemplates.CollisionShape.CIRCLE, 0.2f, 0.2f);
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		RadiationEmitter radiationEmitter = gameObject.AddOrGet<RadiationEmitter>();
		radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.PulsingAveraged;
		radiationEmitter.emitRadiusX = 2;
		radiationEmitter.emitRadiusY = 2;
		radiationEmitter.emitRate = 0.2f;
		radiationEmitter.emitSpeed = 1f;
		HighEnergyParticle highEnergyParticle = gameObject.AddComponent<HighEnergyParticle>();
		highEnergyParticle.speed = 8f;
		highEnergyParticle.perCellFalloff = 50f;
		highEnergyParticle.radioactiveRatio = 0.5f;
		highEnergyParticle.explodeEmitDurration = 1f;
		highEnergyParticle.explodeEmitRadius = 6;
		highEnergyParticle.blackholeEmitDurration = 1f;
		highEnergyParticle.blackholeEmitRadius = 6;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
