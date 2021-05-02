using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GantryConfig : IBuildingConfig
{
	public const string ID = "Gantry";

	private static readonly CellOffset[] SOLID_OFFSETS = new CellOffset[2]
	{
		new CellOffset(-2, 1),
		new CellOffset(-1, 1)
	};

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Gantry", 6, 2, "gantry_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, new string[1]
		{
			SimHashes.Steel.ToString()
		}, 3200f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.NONE, temperature_modification_mass_scale: 1f);
		buildingDef.ObjectLayer = ObjectLayer.Gantry;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.Entombable = true;
		buildingDef.IsFoundation = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(-2, 0);
		buildingDef.EnergyConsumptionWhenActive = 1200f;
		buildingDef.ExhaustKilowattsWhenActive = 1f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.AudioCategory = "Metal";
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(Gantry.PORT_ID, new CellOffset(-1, 1), STRINGS.BUILDINGS.PREFABS.GANTRY.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.GANTRY.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.GANTRY.LOGIC_PORT_INACTIVE)
		};
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<Gantry>();
		MakeBaseSolid.Def def = go.AddOrGetDef<MakeBaseSolid.Def>();
		def.solidOffsets = SOLID_OFFSETS;
		FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
		fakeFloorAdder.floorOffsets = new CellOffset[4]
		{
			new CellOffset(0, 1),
			new CellOffset(1, 1),
			new CellOffset(2, 1),
			new CellOffset(3, 1)
		};
		fakeFloorAdder.initiallyActive = false;
		Object.DestroyImmediate(go.GetComponent<LogicOperationalController>());
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		MakeBaseSolid.Def def2 = go.AddOrGetDef<MakeBaseSolid.Def>();
		def2.solidOffsets = SOLID_OFFSETS;
	}
}
