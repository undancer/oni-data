using UnityEngine;

public class ModularLaunchpadPortSolidUnloaderConfig : IBuildingConfig
{
	public const string ID = "ModularLaunchpadPortSolidUnloader";

	public override BuildingDef CreateBuildingDef()
	{
		return BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort("ModularLaunchpadPortSolidUnloader", "conduit_port_solid_unloader_kanim", ConduitType.Solid, isLoader: false);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Solid, 20f, isLoader: false);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go, isLoader: false);
	}
}
