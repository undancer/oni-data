using TUNING;
using UnityEngine;

public class LogicRibbonConfig : BaseLogicWireConfig
{
	public const string ID = "LogicRibbon";

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicRibbon", "logic_ribbon_kanim", 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		DoPostConfigureComplete(LogicWire.BitDepth.FourBit, go);
	}
}
