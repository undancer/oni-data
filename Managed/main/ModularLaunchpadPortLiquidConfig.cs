using UnityEngine;

public class ModularLaunchpadPortLiquidConfig : IBuildingConfig
{
	public const string ID = "ModularLaunchpadPortLiquid";

	public override BuildingDef CreateBuildingDef()
	{
		return BaseModularLaunchpadPortConfig.CreateBaseLaunchpadPort("ModularLaunchpadPortLiquid", "conduit_port_liquid_kanim", ConduitType.Liquid);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Liquid);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BaseModularLaunchpadPortConfig.DoPostConfigureComplete(go);
	}
}
