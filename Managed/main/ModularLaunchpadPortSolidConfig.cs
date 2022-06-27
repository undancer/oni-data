using UnityEngine;

public class ModularLaunchpadPortSolidConfig : IBuildingConfig
{
	public const string ID = "ModularLaunchpadPortSolid";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort("ModularLaunchpadPortSolid", "conduit_port_solid_loader_kanim", ConduitType.Solid, isLoader: true, 2, 2);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Solid, 20f, isLoader: true);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go, isLoader: true);
	}
}
