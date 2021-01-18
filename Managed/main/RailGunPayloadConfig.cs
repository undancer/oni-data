using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RailGunPayloadConfig : IEntityConfig
{
	public const string ID = "RailGunPayload";

	public const float MASS = 200f;

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("RailGunPayload", ITEMS.RAILGUNPAYLOAD.NAME, ITEMS.RAILGUNPAYLOAD.DESC, 200f, unitMass: true, Assets.GetAnim("railgun_capsule_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 1f, 1f, isPickupable: true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.IgnoreMaterialCategory,
			GameTags.Experimental
		});
		gameObject.AddOrGetDef<RailGunPayload.Def>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(gameObject);
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.allowItemRemoval = false;
		storage.capacityKg = 200f;
		DropAllWorkable dropAllWorkable = gameObject.AddOrGet<DropAllWorkable>();
		dropAllWorkable.dropWorkTime = 30f;
		dropAllWorkable.choreTypeID = Db.Get().ChoreTypes.Fetch.Id;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
