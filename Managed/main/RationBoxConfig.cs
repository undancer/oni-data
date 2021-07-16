using TUNING;
using UnityEngine;

public class RationBoxConfig : IBuildingConfig
{
	public const string ID = "RationBox";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("RationBox", 2, 2, "rationbox_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER0);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.AudioCategory = "Metal";
		SoundEventVolumeCache.instance.AddVolume("rationbox_kanim", "RationBox_open", NOISE_POLLUTION.NOISY.TIER1);
		SoundEventVolumeCache.instance.AddVolume("rationbox_kanim", "RationBox_close", NOISE_POLLUTION.NOISY.TIER1);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 150f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.FOOD;
		storage.allowItemRemoval = true;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<RationBox>();
		go.AddOrGet<UserNameable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<StorageController.Def>();
	}
}
