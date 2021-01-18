using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class PowerControlStationConfig : IBuildingConfig
{
	public const string ID = "PowerControlStation";

	public static Tag MATERIAL_FOR_TINKER = GameTags.RefinedMetal;

	public static Tag TINKER_TOOLS = PowerStationToolsConfig.tag;

	public const float MASS_PER_TINKER = 5f;

	public static string ROLE_PERK = "CanPowerTinker";

	public const float OUTPUT_TEMPERATURE = 308.15f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("PowerControlStation", 2, 4, "electricianworkdesk_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.NONE);
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.PowerStation);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 50f;
		storage.showInUI = true;
		storage.storageFilters = new List<Tag>
		{
			MATERIAL_FOR_TINKER
		};
		TinkerStation tinkerStation = go.AddOrGet<TinkerStation>();
		tinkerStation.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_electricianworkdesk_kanim")
		};
		tinkerStation.inputMaterial = MATERIAL_FOR_TINKER;
		tinkerStation.massPerTinker = 5f;
		tinkerStation.outputPrefab = TINKER_TOOLS;
		tinkerStation.outputTemperature = 308.15f;
		tinkerStation.requiredSkillPerk = ROLE_PERK;
		tinkerStation.choreType = Db.Get().ChoreTypes.PowerFabricate.IdHash;
		tinkerStation.useFilteredStorage = true;
		tinkerStation.fetchChoreType = Db.Get().ChoreTypes.PowerFetch.IdHash;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.PowerPlant.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			TinkerStation component = game_object.GetComponent<TinkerStation>();
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		};
	}
}
