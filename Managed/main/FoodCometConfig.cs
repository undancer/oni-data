using System;
using STRINGS;
using UnityEngine;

public class FoodCometConfig : IEntityConfig
{
	public static string ID = "FoodComet";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, UI.SPACEDESTINATIONS.COMETS.FOODCOMET.NAME);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<LoopingSounds>();
		Comet comet = gameObject.AddOrGet<Comet>();
		comet.massRange = new Vector2(0.2f, 0.5f);
		comet.temperatureRange = new Vector2(298.15f, 303.15f);
		comet.entityDamage = 0;
		comet.totalTileDamage = 0f;
		comet.splashRadius = 0;
		comet.impactSound = "Meteor_Small_Impact";
		comet.flyingSoundID = 0;
		comet.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
		comet.canHitDuplicants = true;
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.SetElement(SimHashes.Creature);
		primaryElement.Temperature = (comet.temperatureRange.x + comet.temperatureRange.y) / 2f;
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.AnimFiles = new KAnimFile[1]
		{
			Assets.GetAnim("meteor_sand_kanim")
		};
		kBatchedAnimController.isMovable = true;
		kBatchedAnimController.initialAnim = "fall_loop";
		kBatchedAnimController.initialMode = KAnim.PlayMode.Loop;
		kBatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		KCircleCollider2D kCircleCollider2D = gameObject.AddOrGet<KCircleCollider2D>();
		kCircleCollider2D.radius = 0.5f;
		gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
		comet.EXHAUST_ELEMENT = SimHashes.Void;
		gameObject.AddTag(GameTags.Comet);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
		Comet component = go.GetComponent<Comet>();
		component.OnImpact = (System.Action)Delegate.Combine(component.OnImpact, (System.Action)delegate
		{
			int num = 10;
			while (num > 0)
			{
				num--;
				Vector3 vector = go.transform.position + new Vector3(UnityEngine.Random.Range(-2, 3), UnityEngine.Random.Range(-2, 3), 0f);
				if (!Grid.Solid[Grid.PosToCell(vector)])
				{
					GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("FoodSplat"), vector);
					gameObject.SetActive(value: true);
					gameObject.transform.Rotate(0f, 0f, UnityEngine.Random.Range(-90, 90));
					num = 0;
				}
			}
		});
	}

	public void OnSpawn(GameObject go)
	{
	}
}
