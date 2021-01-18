using TUNING;
using UnityEngine;

public class WarpConduitReceiverConfig : IBuildingConfig
{
	public const string ID = "WarpConduitReceiver";

	private ConduitPortInfo liquidOutputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 1));

	private ConduitPortInfo gasOutputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-1, 1));

	private ConduitPortInfo solidOutputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(1, 1));

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("WarpConduitReceiver", 4, 3, "warp_conduit_receiver_kanim", 250, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.NONE);
		buildingDef.DefaultAnimState = "off";
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ShowInBuildMenu = false;
		buildingDef.Disinfectable = false;
		buildingDef.Invincible = true;
		buildingDef.Repairable = false;
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
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		WarpConduitReceiver warpConduitReceiver = go.AddOrGet<WarpConduitReceiver>();
		warpConduitReceiver.liquidPortInfo = liquidOutputPort;
		warpConduitReceiver.gasPortInfo = gasOutputPort;
		warpConduitReceiver.solidPortInfo = solidOutputPort;
		Activatable activatable = go.AddOrGet<Activatable>();
		activatable.synchronizeAnims = true;
		activatable.workAnims = new HashedString[2]
		{
			"touchpanel_interact_pre",
			"touchpanel_interact_loop"
		};
		activatable.workingPstComplete = new HashedString[1]
		{
			"touchpanel_interact_pst"
		};
		activatable.workingPstFailed = new HashedString[1]
		{
			"touchpanel_interact_pst"
		};
		activatable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_warp_conduit_receiver_kanim")
		};
		activatable.SetWorkTime(30f);
		go.AddComponent<ConduitSecondaryOutput>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.Gravitas);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<BuildingCellVisualizer>();
		Deconstructable component = go.GetComponent<Deconstructable>();
		component.SetAllowDeconstruction(allow: false);
		Activatable component2 = go.GetComponent<Activatable>();
		component2.requiredSkillPerk = Db.Get().SkillPerks.CanStudyWorldObjects.Id;
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
