using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class OxygenMaskStationConfig : IBuildingConfig
{
	public const string ID = "OxygenMaskStation";

	public const float MATERIAL_PER_MASK = 15f;

	public const float OXYGEN_PER_MASK = 20f;

	public const int MASKS_PER_REFILL = 3;

	public const float WORK_TIME = 5f;

	public ChoreType fetchChoreType = Db.Get().ChoreTypes.Fetch;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string[] rAW_MINERALS = MATERIALS.RAW_MINERALS;
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, construction_materials: rAW_MINERALS, melting_point: 1600f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER0, id: "OxygenMaskStation", width: 2, height: 3, anim: "oxygen_mask_station_kanim", hitpoints: 30, construction_time: 30f, decor: BUILDINGS.DECOR.BONUS.TIER1);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 60f;
		obj.ExhaustKilowattsWhenActive = 0.5f;
		obj.SelfHeatKilowattsWhenActive = 1f;
		obj.PermittedRotations = PermittedRotations.FlipH;
		obj.PreventIdleTraversalPastBuilding = true;
		obj.Deprecated = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddComponent<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		storage.storageFilters = new List<Tag> { GameTags.Metal };
		storage.capacityKg = 45f;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage2.showInUI = true;
		storage2.storageFilters = new List<Tag> { GameTags.Breathable };
		MaskStation maskStation = go.AddOrGet<MaskStation>();
		maskStation.materialConsumedPerMask = 15f;
		maskStation.oxygenConsumedPerMask = 20f;
		maskStation.maxUses = 3;
		maskStation.materialTag = GameTags.Metal;
		maskStation.oxygenTag = GameTags.Breathable;
		maskStation.choreTypeID = fetchChoreType.Id;
		maskStation.PathFlag = PathFinder.PotentialPath.Flags.HasOxygenMask;
		maskStation.materialStorage = storage;
		maskStation.oxygenStorage = storage2;
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.Oxygen;
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.consumptionRate = 0.5f;
		elementConsumer.storeOnConsume = true;
		elementConsumer.showInStatusPanel = false;
		elementConsumer.consumptionRadius = 2;
		elementConsumer.storage = storage2;
		ElementConsumer elementConsumer2 = go.AddComponent<ElementConsumer>();
		elementConsumer2.elementToConsume = SimHashes.ContaminatedOxygen;
		elementConsumer2.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer2.consumptionRate = 0.5f;
		elementConsumer2.storeOnConsume = true;
		elementConsumer2.showInStatusPanel = false;
		elementConsumer2.consumptionRadius = 2;
		elementConsumer2.storage = storage2;
		Prioritizable.AddRef(go);
		go.AddOrGet<LoopingSounds>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
