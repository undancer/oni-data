using STRINGS;
using UnityEngine;

public class NuclearWasteCometConfig : IEntityConfig
{
	public static string ID = "NuclearWasteComet";

	public static float MASS = 1f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, UI.SPACEDESTINATIONS.COMETS.NUCLEAR_WASTE.NAME);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<LoopingSounds>();
		Comet comet = gameObject.AddOrGet<Comet>();
		comet.massRange = new Vector2(MASS, MASS);
		comet.EXHAUST_ELEMENT = SimHashes.Fallout;
		comet.EXHAUST_RATE = MASS * 0.2f;
		comet.temperatureRange = new Vector2(473.15f, 573.15f);
		comet.entityDamage = 2;
		comet.totalTileDamage = 0.45f;
		comet.splashRadius = 0;
		comet.impactSound = "Meteor_Nuclear_Impact";
		comet.flyingSoundID = 3;
		comet.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
		comet.addTiles = 1;
		comet.diseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id);
		comet.addDiseaseCount = 1000000;
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.SetElement(SimHashes.Corium);
		primaryElement.Temperature = (comet.temperatureRange.x + comet.temperatureRange.y) / 2f;
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.AnimFiles = new KAnimFile[1]
		{
			Assets.GetAnim("nuclear_metldown_comet_fx_kanim")
		};
		kBatchedAnimController.isMovable = true;
		kBatchedAnimController.initialAnim = "fall_loop";
		kBatchedAnimController.initialMode = KAnim.PlayMode.Loop;
		kBatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		KCircleCollider2D kCircleCollider2D = gameObject.AddOrGet<KCircleCollider2D>();
		kCircleCollider2D.radius = 0.5f;
		gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
		gameObject.AddTag(GameTags.Comet);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
