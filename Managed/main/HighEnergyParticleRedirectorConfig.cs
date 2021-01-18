using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class HighEnergyParticleRedirectorConfig : IBuildingConfig
{
	public const string ID = "HighEnergyParticleRedirector";

	public const float TRAVEL_DELAY = 0.5f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("HighEnergyParticleRedirector", 1, 2, "orb_transporter_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = false;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.ViewMode = OverlayModes.Radiation.ID;
		buildingDef.UseHighEnergyParticleInputPort = true;
		buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 0);
		buildingDef.UseHighEnergyParticleOutputPort = true;
		buildingDef.HighEnergyParticleOutputOffset = new CellOffset(0, 1);
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(HighEnergyParticleRedirector.PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_PORT_INACTIVE)
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.RadiationIDs, "HighEnergyParticleRedirector");
		buildingDef.Deprecated = !Sim.IsRadiationEnabled();
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		Prioritizable.AddRef(go);
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.autoStore = true;
		highEnergyParticleStorage.showInUI = false;
		HighEnergyParticleRedirector highEnergyParticleRedirector = go.AddOrGet<HighEnergyParticleRedirector>();
		highEnergyParticleRedirector.directorDelay = 0.5f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
