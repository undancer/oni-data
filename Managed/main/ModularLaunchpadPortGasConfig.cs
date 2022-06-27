using UnityEngine;

public class ModularLaunchpadPortGasConfig : IBuildingConfig
{
	public const string ID = "ModularLaunchpadPortGas";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		return BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort("ModularLaunchpadPortGas", "conduit_port_gas_loader_kanim", ConduitType.Gas, isLoader: true, 2, 2);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Gas, 1f, isLoader: true);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go, isLoader: true);
	}
}
