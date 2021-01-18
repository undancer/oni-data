using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SteamTurbineConfig2 : IBuildingConfig
{
	public const string ID = "SteamTurbine2";

	public static float MAX_WATTAGE = 850f;

	private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};

	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[2]
		{
			"RefinedMetal",
			"Plastic"
		};
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0]
		}, construction_materials: array, melting_point: 1600f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, id: "SteamTurbine2", width: 5, height: 3, anim: "steamturbine2_kanim", hitpoints: 30, construction_time: 60f, decor: BUILDINGS.DECOR.NONE, temperature_modification_mass_scale: 1f);
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityOutputOffset = new CellOffset(2, 2);
		buildingDef.GeneratorWattageRating = MAX_WATTAGE;
		buildingDef.GeneratorBaseCapacity = MAX_WATTAGE;
		buildingDef.Entombable = true;
		buildingDef.IsFoundation = false;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.OverheatTemperature = 1273.15f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		Constructable component = go.GetComponent<Constructable>();
		component.requiredSkillPerk = Db.Get().SkillPerks.CanPowerTinker.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Storage storage = go.AddComponent<Storage>();
		storage.showDescriptor = false;
		storage.showInUI = false;
		storage.storageFilters = STORAGEFILTERS.LIQUIDS;
		storage.SetDefaultStoredItemModifiers(StoredItemModifiers);
		storage.capacityKg = 10f;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.showDescriptor = false;
		storage2.showInUI = false;
		storage2.storageFilters = STORAGEFILTERS.GASES;
		storage2.SetDefaultStoredItemModifiers(StoredItemModifiers);
		SteamTurbine steamTurbine = go.AddOrGet<SteamTurbine>();
		steamTurbine.srcElem = SimHashes.Steam;
		steamTurbine.destElem = SimHashes.Water;
		steamTurbine.pumpKGRate = 2f;
		steamTurbine.maxSelfHeat = 64f;
		steamTurbine.wasteHeatToTurbinePercent = 0.1f;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.elementFilter = new SimHashes[1]
		{
			SimHashes.Water
		};
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.storage = storage;
		conduitDispenser.alwaysDispense = true;
		go.AddOrGet<LogicOperationalController>();
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(game_object);
			StructureTemperaturePayload new_data = GameComps.StructureTemperatures.GetPayload(handle);
			Extents extents = game_object.GetComponent<Building>().GetExtents();
			Extents newExtents = new Extents(extents.x, extents.y - 1, extents.width, extents.height + 1);
			new_data.OverrideExtents(newExtents);
			GameComps.StructureTemperatures.SetPayload(handle, ref new_data);
			Storage[] components = game_object.GetComponents<Storage>();
			game_object.GetComponent<SteamTurbine>().SetStorage(components[1], components[0]);
		};
		Tinkerable.MakePowerTinkerable(go);
	}
}
