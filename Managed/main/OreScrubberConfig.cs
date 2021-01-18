using TUNING;
using UnityEngine;

public class OreScrubberConfig : IBuildingConfig
{
	public const string ID = "OreScrubber";

	private const float MASS_PER_USE = 0.07f;

	private const int DISEASE_REMOVAL_COUNT = 480000;

	private const SimHashes CONSUMED_ELEMENT = SimHashes.ChlorineGas;

	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[1]
		{
			"Metal"
		};
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(construction_mass: new float[1]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0]
		}, construction_materials: array, melting_point: 1600f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, id: "OreScrubber", width: 3, height: 3, anim: "orescrubber_kanim", hitpoints: 30, construction_time: 30f, decor: BUILDINGS.DECOR.BONUS.TIER1);
		obj.UtilityInputOffset = new CellOffset(1, 1);
		obj.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		obj.InputConduitType = ConduitType.Gas;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		OreScrubber oreScrubber = go.AddOrGet<OreScrubber>();
		oreScrubber.massConsumedPerUse = 0.07f;
		oreScrubber.consumedElement = SimHashes.ChlorineGas;
		oreScrubber.diseaseRemovalCount = 480000;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityKG = 10f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.ChlorineGas).tag;
		go.AddOrGet<DirectionControl>();
		OreScrubber.Work work = go.AddOrGet<OreScrubber.Work>();
		work.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_ore_scrubber_kanim")
		};
		work.workTime = 10.200001f;
		work.trackUses = true;
		work.workLayer = Grid.SceneLayer.BuildingUse;
		go.AddOrGet<Storage>().SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<RequireInputs>().requireConduitHasMass = false;
	}
}
