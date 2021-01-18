using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public abstract class BaseWireConfig : IBuildingConfig
{
	public abstract override BuildingDef CreateBuildingDef();

	public BuildingDef CreateBuildingDef(string id, string anim, float construction_time, float[] construction_mass, float insulation, EffectorValues decor, EffectorValues noise)
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 1, 1, anim, 10, construction_time, construction_mass, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, decor, noise);
		buildingDef.ThermalConductivity = insulation;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.ObjectLayer = ObjectLayer.Wire;
		buildingDef.TileLayer = ObjectLayer.WireTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementWire;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.SceneLayer = Grid.SceneLayer.Wires;
		buildingDef.isKAnimTile = true;
		buildingDef.isUtility = true;
		buildingDef.DragBuild = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, id);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<Wire>();
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Electrical;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		Constructable component = go.GetComponent<Constructable>();
		component.isDiggingRequired = false;
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Electrical;
	}

	protected void DoPostConfigureComplete(Wire.WattageRating rating, GameObject go)
	{
		Wire component = go.GetComponent<Wire>();
		component.MaxWattageRating = rating;
		float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(rating);
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.MAX_WATTAGE, GameUtil.GetFormattedWattage(maxWattageAsFloat)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MAX_WATTAGE));
		Building component2 = go.GetComponent<Building>();
		BuildingDef def = component2.Def;
		if (def.EffectDescription == null)
		{
			def.EffectDescription = new List<Descriptor>();
		}
		def.EffectDescription.Add(item);
	}
}
