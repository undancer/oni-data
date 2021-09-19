using TUNING;
using UnityEngine;

public class HEPBridgeTileConfig : IBuildingConfig
{
	public const string ID = "HEPBridgeTile";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("HEPBridgeTile", 2, 1, "radbolt_joint_plate_kanim", 100, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.PLASTICS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER5);
		obj.Overheatable = false;
		obj.UseStructureTemperature = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.AudioCategory = "Plastic";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.InitialOrientation = Orientation.R180;
		obj.ForegroundLayer = Grid.SceneLayer.TileMain;
		obj.PermittedRotations = PermittedRotations.R360;
		obj.ViewMode = OverlayModes.Radiation.ID;
		obj.UseHighEnergyParticleInputPort = true;
		obj.HighEnergyParticleInputOffset = new CellOffset(1, 0);
		obj.UseHighEnergyParticleOutputPort = true;
		obj.HighEnergyParticleOutputOffset = new CellOffset(0, 0);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.RadiationIDs, "HEPBridgeTile");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
		go.AddOrGet<TileTemperature>();
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.autoStore = true;
		highEnergyParticleStorage.showInUI = false;
		highEnergyParticleStorage.capacity = 501f;
		HighEnergyParticleRedirector highEnergyParticleRedirector = go.AddOrGet<HighEnergyParticleRedirector>();
		highEnergyParticleRedirector.directorDelay = 0.5f;
		highEnergyParticleRedirector.directionControllable = false;
		highEnergyParticleRedirector.Direction = EightDirection.Right;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		go.AddOrGet<HEPBridgeTileVisualizer>();
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.HEPPassThrough);
		go.AddOrGet<BuildingCellVisualizer>();
		go.AddOrGetDef<MakeBaseSolid.Def>().solidOffsets = new CellOffset[1]
		{
			new CellOffset(0, 0)
		};
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject inst)
		{
			Rotatable component = inst.GetComponent<Rotatable>();
			HighEnergyParticleRedirector component2 = inst.GetComponent<HighEnergyParticleRedirector>();
			switch (component.Orientation)
			{
			case Orientation.Neutral:
				component2.Direction = EightDirection.Left;
				break;
			case Orientation.R90:
				component2.Direction = EightDirection.Up;
				break;
			case Orientation.R180:
				component2.Direction = EightDirection.Right;
				break;
			case Orientation.R270:
				component2.Direction = EightDirection.Down;
				break;
			}
		};
	}
}
