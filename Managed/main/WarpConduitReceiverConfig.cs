using TUNING;
using UnityEngine;

public class WarpConduitReceiverConfig : IBuildingConfig
{
	public const string ID = "WarpConduitReceiver";

	private ConduitPortInfo liquidOutputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 1));

	private ConduitPortInfo gasOutputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-1, 1));

	private ConduitPortInfo solidOutputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(1, 1));

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("WarpConduitReceiver", 4, 3, "warp_conduit_receiver_kanim", 250, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.NONE);
		obj.DefaultAnimState = "off";
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.ShowInBuildMenu = false;
		obj.Disinfectable = false;
		obj.Invincible = true;
		obj.Repairable = false;
		return obj;
	}

	private void AttachPorts(GameObject go)
	{
		go.AddComponent<ConduitSecondaryOutput>().portInfo = liquidOutputPort;
		go.AddComponent<ConduitSecondaryOutput>().portInfo = gasOutputPort;
		go.AddComponent<ConduitSecondaryOutput>().portInfo = solidOutputPort;
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
		go.GetComponent<Deconstructable>().SetAllowDeconstruction(allow: false);
		go.GetComponent<Activatable>().requiredSkillPerk = Db.Get().SkillPerks.CanStudyWorldObjects.Id;
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
