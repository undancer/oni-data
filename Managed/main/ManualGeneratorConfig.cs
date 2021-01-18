using TUNING;
using UnityEngine;

public class ManualGeneratorConfig : IBuildingConfig
{
	public const string ID = "ManualGenerator";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ManualGenerator", 2, 2, "generatormanual_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.NONE);
		buildingDef.GeneratorWattageRating = 400f;
		buildingDef.GeneratorBaseCapacity = 10000f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Breakable = true;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		Generator generator = go.AddOrGet<Generator>();
		generator.powerDistributionOrder = 10;
		ManualGenerator manualGenerator = go.AddOrGet<ManualGenerator>();
		manualGenerator.SetSliderValue(50f, 0);
		manualGenerator.workLayer = Grid.SceneLayer.BuildingFront;
		KBatchedAnimController kBatchedAnimController = go.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		kBatchedAnimController.initialAnim = "off";
		Tinkerable.MakePowerTinkerable(go);
	}
}
