using TUNING;
using UnityEngine;

public class SolidConduitBridgeConfig : IBuildingConfig
{
	public const string ID = "SolidConduitBridge";

	private const ConduitType CONDUIT_TYPE = ConduitType.Solid;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("SolidConduitBridge", 3, 1, "utilities_conveyorbridge_kanim", 10, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Conduit, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.ObjectLayer = ObjectLayer.SolidConduitConnection;
		obj.SceneLayer = Grid.SceneLayer.SolidConduitBridges;
		obj.InputConduitType = ConduitType.Solid;
		obj.OutputConduitType = ConduitType.Solid;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.SolidConveyor.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.PermittedRotations = PermittedRotations.R360;
		obj.UtilityInputOffset = new CellOffset(-1, 0);
		obj.UtilityOutputOffset = new CellOffset(1, 0);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidConduitBridge");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<SolidConduitBridge>();
	}
}
