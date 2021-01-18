using TUNING;
using UnityEngine;

public class CompostConfig : IBuildingConfig
{
	public const string ID = "Compost";

	public static readonly Tag COMPOST_TAG = GameTags.Compostable;

	public const float SAND_INPUT_PER_SECOND = 0.1f;

	public const float FERTILIZER_OUTPUT_PER_SECOND = 0.1f;

	public const float FERTILIZER_OUTPUT_TEMP = 348.15f;

	public const float INPUT_CAPACITY = 300f;

	private const SimHashes OUTPUT_ELEMENT = SimHashes.Dirt;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Compost", 2, 2, "compost_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER3);
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		SoundEventVolumeCache.instance.AddVolume("anim_interacts_compost_kanim", "Compost_shovel_in", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("anim_interacts_compost_kanim", "Compost_shovel_out", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 2000f;
		Compost compost = go.AddOrGet<Compost>();
		compost.simulatedInternalTemperature = 348.15f;
		CompostWorkable compostWorkable = go.AddOrGet<CompostWorkable>();
		compostWorkable.workTime = 20f;
		compostWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_compost_kanim")
		};
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(COMPOST_TAG, 0.1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.1f, SimHashes.Dirt, 348.15f, useEntityTemperature: false, storeOutput: true)
		};
		ElementDropper elementDropper = go.AddComponent<ElementDropper>();
		elementDropper.emitMass = 10f;
		elementDropper.emitTag = SimHashes.Dirt.CreateTag();
		elementDropper.emitOffset = new Vector3(0.5f, 1f, 0f);
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = COMPOST_TAG;
		manualDeliveryKG.capacity = 300f;
		manualDeliveryKG.refillMass = 60f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
		Prioritizable.AddRef(go);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
