using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class HighEnergyParticleRedirectorConfig : IBuildingConfig
{
	public const string ID = "HighEnergyParticleRedirector";

	public const float TRAVEL_DELAY = 0.5f;

	public const float REDIRECT_PARTICLE_COST = 0.1f;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("HighEnergyParticleRedirector", 1, 2, "orb_transporter_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER1);
		obj.Floodable = false;
		obj.AudioCategory = "Metal";
		obj.Overheatable = false;
		obj.PermittedRotations = PermittedRotations.R360;
		obj.ViewMode = OverlayModes.Radiation.ID;
		obj.UseHighEnergyParticleInputPort = true;
		obj.HighEnergyParticleInputOffset = new CellOffset(0, 0);
		obj.UseHighEnergyParticleOutputPort = true;
		obj.HighEnergyParticleOutputOffset = new CellOffset(0, 1);
		obj.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort(HighEnergyParticleRedirector.PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_PORT_INACTIVE) };
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.RadiationIDs, "HighEnergyParticleRedirector");
		obj.Deprecated = !Sim.IsRadiationEnabled();
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		Prioritizable.AddRef(go);
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.autoStore = true;
		highEnergyParticleStorage.showInUI = false;
		highEnergyParticleStorage.capacity = 501f;
		go.AddOrGet<HighEnergyParticleRedirector>().directorDelay = 0.5f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
