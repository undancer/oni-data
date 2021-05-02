using UnityEngine;

public class ModularLaunchpadPortGasUnloaderConfig : IBuildingConfig
{
	public const string ID = "ModularLaunchpadPortGasUnloader";

	public override BuildingDef CreateBuildingDef()
	{
		return BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort("ModularLaunchpadPortGasUnloader", "conduit_port_gas_unloader_kanim", ConduitType.Gas, isLoader: false);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Gas, 1f, isLoader: false);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go, isLoader: false);
	}
}
