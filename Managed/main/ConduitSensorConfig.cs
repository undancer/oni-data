using System.Collections.Generic;
using TUNING;
using UnityEngine;

public abstract class ConduitSensorConfig : IBuildingConfig
{
	protected abstract ConduitType ConduitType
	{
		get;
	}

	protected BuildingDef CreateBuildingDef(string ID, string anim, float[] required_mass, string[] required_materials, List<LogicPorts.Port> output_ports)
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 1, anim, 30, 30f, required_mass, required_materials, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.ViewMode = OverlayModes.Logic.ID;
		obj.AudioCategory = "Metal";
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.AlwaysOperational = true;
		obj.LogicOutputPorts = output_ports;
		SoundEventVolumeCache.instance.AddVolume(anim, "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume(anim, "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
