using TUNING;
using UnityEngine;

public class WarpConduitSenderConfig : IBuildingConfig
{
	public const string ID = "WarpConduitSender";

	private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(0, 1));

	private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(1, 1));

	private ConduitPortInfo solidInputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(2, 1));

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("WarpConduitSender", 4, 3, "warp_conduit_sender_kanim", 250, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.NONE);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ShowInBuildMenu = false;
		buildingDef.DefaultAnimState = "idle";
		buildingDef.CanMove = true;
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
		go.GetComponent<KPrefabID>().AddTag(GameTags.Gravitas);
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		WarpConduitSender warpConduitSender = go.AddOrGet<WarpConduitSender>();
		warpConduitSender.liquidPortInfo = liquidInputPort;
		warpConduitSender.gasPortInfo = gasInputPort;
		warpConduitSender.solidPortInfo = solidInputPort;
		warpConduitSender.gasStorage = go.AddComponent<Storage>();
		warpConduitSender.gasStorage.showInUI = false;
		warpConduitSender.gasStorage.capacityKg = 1f;
		warpConduitSender.liquidStorage = go.AddComponent<Storage>();
		warpConduitSender.liquidStorage.showInUI = false;
		warpConduitSender.liquidStorage.capacityKg = 10f;
		warpConduitSender.solidStorage = go.AddComponent<Storage>();
		warpConduitSender.solidStorage.showInUI = false;
		warpConduitSender.solidStorage.capacityKg = 100f;
		Activatable activatable = go.AddOrGet<Activatable>();
		activatable.synchronizeAnims = true;
		activatable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_warp_conduit_sender_kanim")
		};
		activatable.workAnims = new HashedString[2]
		{
			"sending_pre",
			"sending_loop"
		};
		activatable.workingPstComplete = new HashedString[1]
		{
			"sending_pst"
		};
		activatable.workingPstFailed = new HashedString[1]
		{
			"sending_pre"
		};
		activatable.SetWorkTime(30f);
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
