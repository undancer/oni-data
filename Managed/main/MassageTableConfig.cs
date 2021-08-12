using TUNING;
using UnityEngine;

public class MassageTableConfig : IBuildingConfig
{
	public const string ID = "MassageTable";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("MassageTable", 2, 2, "masseur_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER0, decor: BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.PowerInputOffset = new CellOffset(0, 0);
		obj.Overheatable = true;
		obj.EnergyConsumptionWhenActive = 240f;
		obj.ExhaustKilowattsWhenActive = 0.125f;
		obj.SelfHeatKilowattsWhenActive = 0.5f;
		obj.AudioCategory = "Metal";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.MassageTable);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.DeStressingBuilding);
		MassageTable massageTable = go.AddOrGet<MassageTable>();
		massageTable.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_masseur_kanim") };
		massageTable.stressModificationValue = -30f;
		massageTable.roomStressModificationValue = -60f;
		massageTable.workLayer = Grid.SceneLayer.BuildingFront;
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.MassageTable.Id;
		ownable.canBePublic = true;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.MassageClinic.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		go.AddOrGet<CopyBuildingSettings>();
	}
}
