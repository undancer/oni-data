using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class RailGunConfig : IBuildingConfig
{
	public const string ID = "RailGun";

	public const float BASE_PARTICLE_COST = 10f;

	public const float HEX_PARTICLE_COST = 10f;

	public const float HEP_CAPACITY = 210f;

	public const float TAKEOFF_VELOCITY = 35f;

	public const int MAINTENANCE_AFTER_NUM_PAYLOADS = 6;

	public const int MAINTENANCE_COOLDOWN = 30;

	public const float CAPACITY = 1200f;

	private ConduitPortInfo solidInputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(-1, 0));

	private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 0));

	private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(1, 0));

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RailGun", 5, 6, "rail_gun_kanim", 250, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: TUNING.BUILDINGS.DECOR.NONE);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = 400f;
		buildingDef.DefaultAnimState = "off";
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(-2, 0);
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.UseHighEnergyParticleInputPort = true;
		buildingDef.HighEnergyParticleInputOffset = new CellOffset(-2, 1);
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(RailGun.PORT_ID, new CellOffset(-2, 2), STRINGS.BUILDINGS.PREFABS.RAILGUN.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.RAILGUN.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.RAILGUN.LOGIC_PORT_INACTIVE)
		};
		return buildingDef;
	}

	private void AttachPorts(GameObject go)
	{
		ConduitSecondaryInput conduitSecondaryInput = go.AddComponent<ConduitSecondaryInput>();
		conduitSecondaryInput.portInfo = liquidInputPort;
		ConduitSecondaryInput conduitSecondaryInput2 = go.AddComponent<ConduitSecondaryInput>();
		conduitSecondaryInput2.portInfo = gasInputPort;
		ConduitSecondaryInput conduitSecondaryInput3 = go.AddComponent<ConduitSecondaryInput>();
		conduitSecondaryInput3.portInfo = solidInputPort;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		RailGun railGun = go.AddOrGet<RailGun>();
		railGun.FreeStartHex = true;
		railGun.FreeDestinationHex = true;
		go.AddOrGet<LoopingSounds>();
		ClusterDestinationSelector clusterDestinationSelector = go.AddOrGet<ClusterDestinationSelector>();
		clusterDestinationSelector.assignable = true;
		clusterDestinationSelector.requireAsteroidDestination = true;
		railGun.liquidPortInfo = liquidInputPort;
		railGun.gasPortInfo = gasInputPort;
		railGun.solidPortInfo = solidInputPort;
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.capacity = 210f;
		highEnergyParticleStorage.autoStore = true;
		highEnergyParticleStorage.showInUI = false;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		List<Tag> list = new List<Tag>();
		list.AddRange(STORAGEFILTERS.NOT_EDIBLE_SOLIDS);
		list.AddRange(STORAGEFILTERS.GASES);
		list.AddRange(STORAGEFILTERS.FOOD);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.storageFilters = list;
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage.capacityKg = 1200f;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		AttachPorts(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		AttachPorts(go);
	}
}
