using TUNING;
using UnityEngine;

public class LogicWireConfig : BaseLogicWireConfig
{
	public const string ID = "LogicWire";

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("LogicWire", "logic_wires_kanim", 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER_TINY, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		DoPostConfigureComplete(LogicWire.BitDepth.OneBit, go);
	}
}
