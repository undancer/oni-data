using TUNING;
using UnityEngine;

public class PressureSwitchGasConfig : IBuildingConfig
{
	public static string ID = "PressureSwitchGas";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "switchgaspressure_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		obj.Deprecated = true;
		obj.Overheatable = false;
		obj.Floodable = true;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.SceneLayer = Grid.SceneLayer.Building;
		SoundEventVolumeCache.instance.AddVolume("switchgaspressure_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchgaspressure_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		PressureSwitch pressureSwitch = go.AddOrGet<PressureSwitch>();
		pressureSwitch.objectLayer = ObjectLayer.Wire;
		pressureSwitch.rangeMin = 0f;
		pressureSwitch.rangeMax = 2f;
		pressureSwitch.Threshold = 1f;
		pressureSwitch.ActivateAboveThreshold = false;
		pressureSwitch.manuallyControlled = false;
		pressureSwitch.desiredState = Element.State.Gas;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddComponent<BuildingCellVisualizer>();
	}
}
