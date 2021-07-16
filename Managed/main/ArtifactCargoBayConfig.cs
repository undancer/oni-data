using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ArtifactCargoBayConfig : IBuildingConfig
{
	public const string ID = "ArtifactCargoBay";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("ArtifactCargoBay", 3, 1, "artifact_transport_module_kanim", 1000, 60f, BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER1, MATERIALS.REFINED_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.Invincible = true;
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.RequiresPowerInput = false;
		obj.attachablePosition = new CellOffset(0, 0);
		obj.CanMove = true;
		obj.Cancellable = false;
		obj.ShowInBuildMenu = false;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 1), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MAJOR);
		go.AddOrGet<Storage>().SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>(new Storage.StoredItemModifier[2]
		{
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Preserve
		}));
		Prioritizable.AddRef(go);
		ArtifactModule artifactModule = go.AddOrGet<ArtifactModule>();
		artifactModule.AddDepositTag(GameTags.PedestalDisplayable);
		artifactModule.occupyingObjectRelativePosition = new Vector3(0f, 0.5f, -1f);
		go.AddOrGet<DecorProvider>();
		go.AddOrGet<ItemPedestal>();
		go.AddOrGetDef<ArtifactHarvestModule.Def>();
	}
}
