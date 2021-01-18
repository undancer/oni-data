using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;

public class RailGunPayloadOpenerConfig : IBuildingConfig
{
	public const string ID = "RailGunPayloadOpener";

	private ConduitPortInfo liquidOutputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 0));

	private ConduitPortInfo gasOutputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(1, 0));

	private ConduitPortInfo solidOutputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(-1, 0));

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RailGunPayloadOpener", 3, 3, "railgun_emptier_kanim", 250, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.NONE);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.DefaultAnimState = "on";
		return buildingDef;
	}

	private void AttachPorts(GameObject go)
	{
		ConduitSecondaryOutput conduitSecondaryOutput = go.AddComponent<ConduitSecondaryOutput>();
		conduitSecondaryOutput.portInfo = liquidOutputPort;
		ConduitSecondaryOutput conduitSecondaryOutput2 = go.AddComponent<ConduitSecondaryOutput>();
		conduitSecondaryOutput2.portInfo = gasOutputPort;
		ConduitSecondaryOutput conduitSecondaryOutput3 = go.AddComponent<ConduitSecondaryOutput>();
		conduitSecondaryOutput3.portInfo = solidOutputPort;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		RailGunPayloadOpener railGunPayloadOpener = go.AddOrGet<RailGunPayloadOpener>();
		railGunPayloadOpener.liquidPortInfo = liquidOutputPort;
		railGunPayloadOpener.gasPortInfo = gasOutputPort;
		railGunPayloadOpener.solidPortInfo = solidOutputPort;
		railGunPayloadOpener.payloadStorage = go.AddComponent<Storage>();
		railGunPayloadOpener.payloadStorage.showInUI = false;
		railGunPayloadOpener.payloadStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		railGunPayloadOpener.payloadStorage.storageFilters = new List<Tag>
		{
			GameTags.RailGunPayloadEmptyable
		};
		railGunPayloadOpener.resourceStorage = go.AddComponent<Storage>();
		railGunPayloadOpener.resourceStorage.showInUI = true;
		railGunPayloadOpener.resourceStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		List<Tag> first = STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Concat(STORAGEFILTERS.GASES).ToList();
		first = first.Concat(STORAGEFILTERS.LIQUIDS).ToList();
		railGunPayloadOpener.resourceStorage.storageFilters = first;
		railGunPayloadOpener.resourceStorage.capacityKg = 20000f;
		go.AddComponent<ConduitSecondaryOutput>();
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(railGunPayloadOpener.payloadStorage);
		manualDeliveryKG.requestedItemTag = GameTags.RailGunPayloadEmptyable;
		manualDeliveryKG.capacity = 2f;
		manualDeliveryKG.refillMass = 1f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		manualDeliveryKG.operationalRequirement = FetchOrder2.OperationalRequirement.None;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		go.AddOrGet<BuildingCellVisualizer>();
		AttachPorts(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.AddOrGet<BuildingCellVisualizer>();
		AttachPorts(go);
	}
}
