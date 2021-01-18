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

	public const float BATTERY_DEPLETION_RATE = 40f;

	public const float BATTERY_CAPACITY = 21000f;

	public const float MAX_SWEEP_AMOUNT = 10f;

	public const float MOP_SPEED = 10f;

	private string name = ROBOTS.MODELS.SWEEPBOT.NAME;

	private string desc = ROBOTS.MODELS.SWEEPBOT.DESC;

	public static float MASS = 25f;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SweepBot", name, desc, MASS, decor: TUNING.BUILDINGS.DECOR.NONE, anim: Assets.GetAnim("sweep_bot_kanim"), initialAnim: "idle", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 1);
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.GetComponent<KBatchedAnimController>().isMovable = true;
		KPrefabID kPrefabID = gameObject.AddOrGet<KPrefabID>();
		kPrefabID.AddTag(GameTags.Creature);
		gameObject.AddComponent<Pickupable>();
		gameObject.AddOrGet<Clearable>().isClearable = false;
		Trait trait = Db.Get().CreateTrait("SweepBotBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.maxAttribute.Id, 21000f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.deltaAttribute.Id, -40f, name));
		Modifiers modifiers = gameObject.AddOrGet<Modifiers>();
		modifiers.initialTraits = new string[1]
		{
			"SweepBotBaseTrait"
		};
		modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.InternalBattery.Id);
		gameObject.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity("snapto_pivot", is_visible: false);
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<CharacterOverlay>();
		gameObject.AddOrGet<Effects>();
		gameObject.AddOrGetDef<AnimInterruptMonitor.Def>();
		gameObject.AddOrGetDef<StorageUnloadMonitor.Def>();
		gameObject.AddOrGetDef<RobotBatteryMonitor.Def>();
		gameObject.AddOrGetDef<SweetBotReactMonitor.Def>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<SweepBotTrappedMonitor.Def>();
		gameObject.AddOrGet<AnimEventHandler>();
		gameObject.AddOrGet<SnapOn>().snapPoints = new List<SnapOn.SnapPoint>(new SnapOn.SnapPoint[1]
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
		gameObject.AddComponent<Storage>();
		gameObject.AddComponent<Storage>().capacityKg = 500f;
		gameObject.AddOrGet<OrnamentReceptacle>().AddDepositTag(GameTags.PedestalDisplayable);
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
		inst.GetComponent<OrnamentReceptacle>();
		inst.GetSMI<CreatureFallMonitor.Instance>().anim = "idle_loop";
	}
}
