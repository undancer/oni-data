using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;

public class RailGunConfig : IBuildingConfig
{
	public const string ID = "RailGun";

	public const float PARTICLES_PER_LAUNCH = 200f;

	public const float HEP_CAPACITY = 2000f;

	private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(-2, 0));

	private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-1, 0));

	private ConduitPortInfo solidInputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(0, 0));

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RailGun", 5, 6, "rail_gun_kanim", 250, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.NONE);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = 400f;
		buildingDef.ShowInBuildMenu = false;
		buildingDef.DefaultAnimState = "off";
		buildingDef.UseHighEnergyParticleInputPort = true;
		buildingDef.HighEnergyParticleInputOffset = new CellOffset(-2, 1);
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
		go.AddOrGet<HighEnergyParticleStorage>();
		ClusterDestinationSelector clusterDestinationSelector = go.AddOrGet<ClusterDestinationSelector>();
		clusterDestinationSelector.assignable = true;
		clusterDestinationSelector.requireAsteroidDestination = true;
		railGun.liquidPortInfo = liquidInputPort;
		railGun.gasPortInfo = gasInputPort;
		railGun.solidPortInfo = solidInputPort;
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.capacity = 2000f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		List<Tag> first = STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Concat(STORAGEFILTERS.GASES).ToList();
		first = (storage.storageFilters = first.Concat(STORAGEFILTERS.LIQUIDS).ToList());
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage.capacityKg = 20000f;
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
