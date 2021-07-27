using TUNING;
using UnityEngine;

public abstract class LogicGateBaseConfig : IBuildingConfig
{
	protected abstract CellOffset[] InputPortOffsets { get; }

	protected abstract CellOffset[] OutputPortOffsets { get; }

	protected abstract CellOffset[] ControlPortOffsets { get; }

	protected BuildingDef CreateBuildingDef(string ID, string anim, int width = 2, int height = 2)
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, 10, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
		obj.ViewMode = OverlayModes.Logic.ID;
		obj.ObjectLayer = ObjectLayer.LogicGate;
		obj.SceneLayer = Grid.SceneLayer.LogicGates;
		obj.ThermalConductivity = 0.05f;
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.Entombable = false;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.PermittedRotations = PermittedRotations.R360;
		obj.DragBuild = true;
		LogicGateBase.uiSrcData = Assets.instance.logicModeUIData;
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return obj;
	}

	protected abstract LogicGateBase.Op GetLogicOp();

	protected abstract LogicGate.LogicGateDescriptions GetDescriptions();

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		MoveableLogicGateVisualizer moveableLogicGateVisualizer = go.AddComponent<MoveableLogicGateVisualizer>();
		moveableLogicGateVisualizer.op = GetLogicOp();
		moveableLogicGateVisualizer.inputPortOffsets = InputPortOffsets;
		moveableLogicGateVisualizer.outputPortOffsets = OutputPortOffsets;
		moveableLogicGateVisualizer.controlPortOffsets = ControlPortOffsets;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		LogicGateVisualizer logicGateVisualizer = go.AddComponent<LogicGateVisualizer>();
		logicGateVisualizer.op = GetLogicOp();
		logicGateVisualizer.inputPortOffsets = InputPortOffsets;
		logicGateVisualizer.outputPortOffsets = OutputPortOffsets;
		logicGateVisualizer.controlPortOffsets = ControlPortOffsets;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGate logicGate = go.AddComponent<LogicGate>();
		logicGate.op = GetLogicOp();
		logicGate.inputPortOffsets = InputPortOffsets;
		logicGate.outputPortOffsets = OutputPortOffsets;
		logicGate.controlPortOffsets = ControlPortOffsets;
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			game_object.GetComponent<LogicGate>().SetPortDescriptions(GetDescriptions());
		};
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
	}
}
