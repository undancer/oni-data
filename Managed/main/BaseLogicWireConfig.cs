using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public abstract class BaseLogicWireConfig : IBuildingConfig
{
	public abstract override BuildingDef CreateBuildingDef();

	public BuildingDef CreateBuildingDef(string id, string anim, float construction_time, float[] construction_mass, EffectorValues decor, EffectorValues noise)
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 1, 1, anim, 10, construction_time, construction_mass, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, decor, noise);
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.ObjectLayer = ObjectLayer.LogicWire;
		buildingDef.TileLayer = ObjectLayer.LogicWireTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementLogicWire;
		buildingDef.SceneLayer = Grid.SceneLayer.LogicWires;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.isKAnimTile = true;
		buildingDef.isUtility = true;
		buildingDef.DragBuild = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, id);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LogicWire>();
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Logic;
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		Constructable component = go.GetComponent<Constructable>();
		component.isDiggingRequired = false;
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Logic;
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
	}

	protected void DoPostConfigureComplete(LogicWire.BitDepth rating, GameObject go)
	{
		LogicWire component = go.GetComponent<LogicWire>();
		component.MaxBitDepth = rating;
		int bitDepthAsInt = LogicWire.GetBitDepthAsInt(rating);
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.MAX_BITS, bitDepthAsInt), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MAX_BITS));
		Building component2 = go.GetComponent<Building>();
		BuildingDef def = component2.Def;
		if (def.EffectDescription == null)
		{
			def.EffectDescription = new List<Descriptor>();
		}
		def.EffectDescription.Add(item);
	}
}
