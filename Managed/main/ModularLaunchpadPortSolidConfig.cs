using UnityEngine;

public class ModularLaunchpadPortSolidConfig : IBuildingConfig
{
	public const string ID = "ModularLaunchpadPortSolid";

	public override BuildingDef CreateBuildingDef()
	{
		return BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort("ModularLaunchpadPortSolid", "conduit_port_solid_kanim", ConduitType.Solid);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Solid);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go);
	}
}
