using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GantryConfig : IBuildingConfig
{
	public const string ID = "Gantry";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("Gantry", 6, 2, "gantry_kanim", 30, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, new string[1]
		{
			SimHashes.Steel.ToString()
		}, 3200f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.NONE, temperature_modification_mass_scale: 1f);
		obj.ObjectLayer = ObjectLayer.Gantry;
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		obj.PermittedRotations = PermittedRotations.FlipH;
		obj.Entombable = true;
		obj.IsFoundation = false;
		obj.RequiresPowerInput = true;
		obj.PowerInputOffset = new CellOffset(-2, 0);
		obj.EnergyConsumptionWhenActive = 1200f;
		obj.ExhaustKilowattsWhenActive = 1f;
		obj.SelfHeatKilowattsWhenActive = 1f;
		obj.OverheatTemperature = 2273.15f;
		obj.AudioCategory = "Metal";
		obj.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(Gantry.PORT_ID, new CellOffset(-1, 1), STRINGS.BUILDINGS.PREFABS.GANTRY.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.GANTRY.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.GANTRY.LOGIC_PORT_INACTIVE)
		};
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<Gantry>();
		Object.DestroyImmediate(go.GetComponent<LogicOperationalController>());
	}
}
