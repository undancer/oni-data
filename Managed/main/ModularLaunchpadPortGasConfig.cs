using UnityEngine;

public class ModularLaunchpadPortGasConfig : IBuildingConfig
{
	public const string ID = "ModularLaunchpadPortGas";

	public override BuildingDef CreateBuildingDef()
	{
		return BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort("ModularLaunchpadPortGas", "conduit_port_gas_kanim", ConduitType.Gas);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Gas);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go);
	}
}
