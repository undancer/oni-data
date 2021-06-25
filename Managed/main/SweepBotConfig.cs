using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class SweepBotConfig : IEntityConfig
{
	public const string ID = "SweepBot";

	public const string BASE_TRAIT_ID = "SweepBotBaseTrait";

	public const float STORAGE_CAPACITY = 500f;

	public const float BATTERY_CAPACITY = 9000f;

	public const float BATTERY_DEPLETION_RATE = 17.142857f;

	public const float MAX_SWEEP_AMOUNT = 10f;

	public const float MOP_SPEED = 10f;

	private string name = STRINGS.ROBOTS.MODELS.SWEEPBOT.NAME;

	private string desc = STRINGS.ROBOTS.MODELS.SWEEPBOT.DESC;

	public static float MASS = 25f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SweepBot", name, desc, MASS, decor: TUNING.BUILDINGS.DECOR.NONE, anim: Assets.GetAnim("sweep_bot_kanim"), initialAnim: "idle", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 1);
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.GetComponent<KBatchedAnimController>().isMovable = true;
		KPrefabID kPrefabID = gameObject.AddOrGet<KPrefabID>();
		kPrefabID.AddTag(GameTags.Creature);
		Pickupable pickupable = gameObject.AddComponent<Pickupable>();
		gameObject.AddOrGet<Clearable>().isClearable = false;
		Trait trait = Db.Get().CreateTrait("SweepBotBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.maxAttribute.Id, 9000f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.deltaAttribute.Id, -17.142857f, name));
		Modifiers modifiers = gameObject.AddOrGet<Modifiers>();
		bool flag = true;
		modifiers.initialTraits.Add("SweepBotBaseTrait");
		modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.InternalBattery.Id);
		gameObject.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity("snapto_pivot", is_visible: false);
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<Effects>();
		gameObject.AddOrGetDef<AnimInterruptMonitor.Def>();
		gameObject.AddOrGetDef<StorageUnloadMonitor.Def>();
		RobotBatteryMonitor.Def def = gameObject.AddOrGetDef<RobotBatteryMonitor.Def>();
		def.batteryAmountId = Db.Get().Amounts.InternalBattery.Id;
		def.canCharge = true;
		def.lowBatteryWarningPercent = 0.5f;
		gameObject.AddOrGetDef<SweetBotReactMonitor.Def>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<SweepBotTrappedMonitor.Def>();
		gameObject.AddOrGet<AnimEventHandler>();
		SnapOn snapOn = gameObject.AddOrGet<SnapOn>();
		snapOn.snapPoints = new List<SnapOn.SnapPoint>(new SnapOn.SnapPoint[1]
		{
			new SnapOn.SnapPoint
			{
				pointName = "carry",
				automatic = false,
				context = "",
				buildFile = null,
				overrideSymbol = "snapTo_ornament"
			}
		});
		SymbolOverrideControllerUtil.AddToPrefab(gameObject);
		Storage storage = gameObject.AddComponent<Storage>();
		Storage storage2 = gameObject.AddComponent<Storage>();
		storage2.capacityKg = 500f;
		OrnamentReceptacle ornamentReceptacle = gameObject.AddOrGet<OrnamentReceptacle>();
		ornamentReceptacle.AddDepositTag(GameTags.PedestalDisplayable);
		gameObject.AddOrGet<DecorProvider>();
		gameObject.AddOrGet<UserNameable>();
		gameObject.AddOrGet<CharacterOverlay>();
		gameObject.AddOrGet<ItemPedestal>();
		Navigator navigator = gameObject.AddOrGet<Navigator>();
		navigator.NavGridName = "WalkerBabyNavGrid";
		navigator.CurrentNavType = NavType.Floor;
		navigator.defaultSpeed = 1f;
		navigator.updateProber = true;
		navigator.maxProbingRadius = 32;
		navigator.sceneLayer = Grid.SceneLayer.Creatures;
		kPrefabID.AddTag(GameTags.Creatures.Walker);
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new FallStates.Def()).Add(new AnimInterruptStates.Def()).Add(new SweepBotTrappedStates.Def())
			.Add(new DeliverToSweepLockerStates.Def())
			.Add(new ReturnToChargeStationStates.Def())
			.Add(new SweepStates.Def())
			.Add(new IdleStates.Def());
		gameObject.AddOrGet<LoopingSounds>();
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Robots.Models.SweepBot, null);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		StorageUnloadMonitor.Instance sMI = inst.GetSMI<StorageUnloadMonitor.Instance>();
		sMI.sm.internalStorage.Set(inst.GetComponents<Storage>()[1], sMI);
		OrnamentReceptacle component = inst.GetComponent<OrnamentReceptacle>();
		inst.GetSMI<CreatureFallMonitor.Instance>().anim = "idle_loop";
	}
}
