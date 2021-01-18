using TUNING;
using UnityEngine;

public class StorageLockerConfig : IBuildingConfig
{
	public const string ID = "StorageLocker";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("StorageLocker", 1, 2, "storagelocker_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.Floodable = false;
		obj.AudioCategory = "Metal";
		obj.Overheatable = false;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		storage.allowItemRemoval = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
		go.AddOrGet<StorageLocker>();
		go.AddOrGet<UserNameable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<StorageController.Def>();
	}
}
