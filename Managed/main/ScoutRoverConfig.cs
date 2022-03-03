using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class ScoutRoverConfig : IEntityConfig
{
	public struct LaserEffect
	{
		public string id;

		public string animFile;

		public string anim;

		public HashedString context;
	}

	public const string ID = "ScoutRover";

	public static string ROVER_BASE_TRAIT_ID = "ScoutRoverBaseTrait";

	public const int MAXIMUM_TECH_CONSTRUCTION_TIER = 2;

	public const float MASS = 100f;

	private const float WIDTH = 1f;

	private const float HEIGHT = 2f;

	public static GameObject CreateScout(string id, string name, string desc, string anim_file)
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity(id, name, desc, 100f, unitMass: true, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, SimHashes.Creature, new List<Tag> { GameTags.Experimental });
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.isMovable = true;
		gameObject.AddOrGet<Modifiers>();
		gameObject.AddOrGet<LoopingSounds>();
		KBoxCollider2D kBoxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
		kBoxCollider2D.size = new Vector2(1f, 2f);
		kBoxCollider2D.offset = new Vector2f(0f, 1f);
		Modifiers component2 = gameObject.GetComponent<Modifiers>();
		component2.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
		component2.initialAmounts.Add(Db.Get().Amounts.InternalChemicalBattery.Id);
		component2.initialAttributes.Add(Db.Get().Attributes.Construction.Id);
		component2.initialAttributes.Add(Db.Get().Attributes.Digging.Id);
		component2.initialAttributes.Add(Db.Get().Attributes.CarryAmount.Id);
		component2.initialAttributes.Add(Db.Get().Attributes.Machinery.Id);
		component2.initialAttributes.Add(Db.Get().Attributes.Athletics.Id);
		ChoreGroup[] disabled_chore_groups = new ChoreGroup[12]
		{
			Db.Get().ChoreGroups.Basekeeping,
			Db.Get().ChoreGroups.Cook,
			Db.Get().ChoreGroups.Art,
			Db.Get().ChoreGroups.Research,
			Db.Get().ChoreGroups.Farming,
			Db.Get().ChoreGroups.Ranching,
			Db.Get().ChoreGroups.MachineOperating,
			Db.Get().ChoreGroups.MedicalAid,
			Db.Get().ChoreGroups.Combat,
			Db.Get().ChoreGroups.LifeSupport,
			Db.Get().ChoreGroups.Recreation,
			Db.Get().ChoreGroups.Toggle
		};
		gameObject.AddOrGet<Traits>();
		Trait trait = Db.Get().CreateTrait(ROVER_BASE_TRAIT_ID, STRINGS.ROBOTS.MODELS.SCOUT.NAME, STRINGS.ROBOTS.MODELS.SCOUT.NAME, null, should_save: false, disabled_chore_groups, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, 200f, STRINGS.ROBOTS.MODELS.SCOUT.NAME));
		trait.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, TUNING.ROBOTS.SCOUTBOT.DIGGING, STRINGS.ROBOTS.MODELS.SCOUT.NAME));
		trait.Add(new AttributeModifier(Db.Get().Attributes.Construction.Id, TUNING.ROBOTS.SCOUTBOT.CONSTRUCTION, STRINGS.ROBOTS.MODELS.SCOUT.NAME));
		trait.Add(new AttributeModifier(Db.Get().Attributes.Athletics.Id, TUNING.ROBOTS.SCOUTBOT.ATHLETICS, STRINGS.ROBOTS.MODELS.SCOUT.NAME));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, TUNING.ROBOTS.SCOUTBOT.HIT_POINTS, STRINGS.ROBOTS.MODELS.SCOUT.NAME));
		trait.Add(new AttributeModifier(Db.Get().Amounts.InternalChemicalBattery.maxAttribute.Id, TUNING.ROBOTS.SCOUTBOT.BATTERY_CAPACITY, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.InternalChemicalBattery.deltaAttribute.Id, 0f - TUNING.ROBOTS.SCOUTBOT.BATTERY_DEPLETION_RATE, name));
		component2.initialTraits.Add(ROVER_BASE_TRAIT_ID);
		gameObject.AddOrGet<AttributeConverters>();
		GridVisibility gridVisibility = gameObject.AddOrGet<GridVisibility>();
		gridVisibility.radius = 30;
		gridVisibility.innerRadius = 20f;
		gameObject.AddOrGet<Worker>();
		gameObject.AddOrGet<Effects>();
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<AnimEventHandler>();
		gameObject.AddOrGet<Health>();
		MoverLayerOccupier moverLayerOccupier = gameObject.AddOrGet<MoverLayerOccupier>();
		moverLayerOccupier.objectLayers = new ObjectLayer[2]
		{
			ObjectLayer.Rover,
			ObjectLayer.Mover
		};
		moverLayerOccupier.cellOffsets = new CellOffset[2]
		{
			CellOffset.none,
			new CellOffset(0, 1)
		};
		RobotBatteryMonitor.Def def = gameObject.AddOrGetDef<RobotBatteryMonitor.Def>();
		def.batteryAmountId = Db.Get().Amounts.InternalChemicalBattery.Id;
		def.canCharge = false;
		def.lowBatteryWarningPercent = 0.2f;
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.fxPrefix = Storage.FXPrefix.PickedUp;
		storage.dropOnLoad = true;
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Seal
		});
		gameObject.AddOrGetDef<CreatureDebugGoToMonitor.Def>();
		Deconstructable deconstructable = gameObject.AddOrGet<Deconstructable>();
		deconstructable.enabled = false;
		deconstructable.audioSize = "medium";
		deconstructable.looseEntityDeconstructable = true;
		gameObject.AddOrGetDef<RobotAi.Def>();
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new RobotDeathStates.Def()).Add(new FallStates.Def()).Add(new DebugGoToStates.Def())
			.Add(new IdleStates.Def(), condition: true, Db.Get().ChoreTypes.Idle.priority);
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Robots.Models.ScoutRover, null);
		gameObject.AddOrGet<KPrefabID>().RemoveTag(GameTags.CreatureBrain);
		gameObject.AddOrGet<KPrefabID>().AddTag(GameTags.DupeBrain);
		Navigator navigator = gameObject.AddOrGet<Navigator>();
		string text = (navigator.NavGridName = "RobotNavGrid");
		navigator.CurrentNavType = NavType.Floor;
		navigator.defaultSpeed = 2f;
		navigator.updateProber = true;
		navigator.sceneLayer = Grid.SceneLayer.Creatures;
		gameObject.AddOrGet<Sensors>();
		gameObject.AddOrGet<Pickupable>().SetWorkTime(5f);
		gameObject.AddOrGet<SnapOn>();
		component.SetSymbolVisiblity("snapto_pivot", is_visible: false);
		component.SetSymbolVisiblity("snapto_radar", is_visible: false);
		return gameObject;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CreateScout("ScoutRover", STRINGS.ROBOTS.MODELS.SCOUT.NAME, STRINGS.ROBOTS.MODELS.SCOUT.DESC, "scout_bot_kanim");
		SetupLaserEffects(gameObject);
		return gameObject;
	}

	private void SetupLaserEffects(GameObject prefab)
	{
		GameObject gameObject = new GameObject("LaserEffect");
		gameObject.transform.parent = prefab.transform;
		KBatchedAnimEventToggler kBatchedAnimEventToggler = gameObject.AddComponent<KBatchedAnimEventToggler>();
		kBatchedAnimEventToggler.eventSource = prefab;
		kBatchedAnimEventToggler.enableEvent = "LaserOn";
		kBatchedAnimEventToggler.disableEvent = "LaserOff";
		kBatchedAnimEventToggler.entries = new List<KBatchedAnimEventToggler.Entry>();
		LaserEffect[] array = new LaserEffect[14];
		LaserEffect laserEffect = new LaserEffect
		{
			id = "DigEffect",
			animFile = "laser_kanim",
			anim = "idle",
			context = "dig"
		};
		array[0] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "BuildEffect",
			animFile = "construct_beam_kanim",
			anim = "loop",
			context = "build"
		};
		array[1] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "FetchLiquidEffect",
			animFile = "hose_fx_kanim",
			anim = "loop",
			context = "fetchliquid"
		};
		array[2] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "PaintEffect",
			animFile = "paint_beam_kanim",
			anim = "loop",
			context = "paint"
		};
		array[3] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "HarvestEffect",
			animFile = "plant_harvest_beam_kanim",
			anim = "loop",
			context = "harvest"
		};
		array[4] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "CaptureEffect",
			animFile = "net_gun_fx_kanim",
			anim = "loop",
			context = "capture"
		};
		array[5] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "AttackEffect",
			animFile = "attack_beam_fx_kanim",
			anim = "loop",
			context = "attack"
		};
		array[6] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "PickupEffect",
			animFile = "vacuum_fx_kanim",
			anim = "loop",
			context = "pickup"
		};
		array[7] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "StoreEffect",
			animFile = "vacuum_reverse_fx_kanim",
			anim = "loop",
			context = "store"
		};
		array[8] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "DisinfectEffect",
			animFile = "plant_spray_beam_kanim",
			anim = "loop",
			context = "disinfect"
		};
		array[9] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "TendEffect",
			animFile = "plant_tending_beam_fx_kanim",
			anim = "loop",
			context = "tend"
		};
		array[10] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "PowerTinkerEffect",
			animFile = "electrician_beam_fx_kanim",
			anim = "idle",
			context = "powertinker"
		};
		array[11] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "SpecialistDigEffect",
			animFile = "senior_miner_beam_fx_kanim",
			anim = "idle",
			context = "specialistdig"
		};
		array[12] = laserEffect;
		laserEffect = new LaserEffect
		{
			id = "DemolishEffect",
			animFile = "poi_demolish_fx_kanim",
			anim = "idle",
			context = "demolish"
		};
		array[13] = laserEffect;
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		LaserEffect[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			LaserEffect laserEffect2 = array2[i];
			GameObject gameObject2 = new GameObject(laserEffect2.id);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.AddOrGet<KPrefabID>().PrefabTag = new Tag(laserEffect2.id);
			KBatchedAnimTracker kBatchedAnimTracker = gameObject2.AddOrGet<KBatchedAnimTracker>();
			kBatchedAnimTracker.controller = component;
			kBatchedAnimTracker.symbol = new HashedString("snapto_radar");
			kBatchedAnimTracker.offset = new Vector3(40f, 0f, 0f);
			kBatchedAnimTracker.useTargetPoint = true;
			KBatchedAnimController kBatchedAnimController = gameObject2.AddOrGet<KBatchedAnimController>();
			kBatchedAnimController.AnimFiles = new KAnimFile[1] { Assets.GetAnim(laserEffect2.animFile) };
			KBatchedAnimEventToggler.Entry entry = default(KBatchedAnimEventToggler.Entry);
			entry.anim = laserEffect2.anim;
			entry.context = laserEffect2.context;
			entry.controller = kBatchedAnimController;
			KBatchedAnimEventToggler.Entry item = entry;
			kBatchedAnimEventToggler.entries.Add(item);
			gameObject2.AddOrGet<LoopingSounds>();
		}
	}

	public void OnPrefabInit(GameObject inst)
	{
		ChoreConsumer component = inst.GetComponent<ChoreConsumer>();
		if (component != null)
		{
			component.AddProvider(GlobalChoreProvider.Instance);
		}
		AmountInstance amountInstance = Db.Get().Amounts.InternalChemicalBattery.Lookup(inst);
		amountInstance.value = amountInstance.GetMax();
	}

	public void OnSpawn(GameObject inst)
	{
		Sensors component = inst.GetComponent<Sensors>();
		component.Add(new PathProberSensor(component));
		component.Add(new PickupableSensor(component));
		Navigator component2 = inst.GetComponent<Navigator>();
		component2.transitionDriver.overrideLayers.Add(new BipedTransitionLayer(component2, 3.325f, 2.5f));
		component2.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(component2));
		component2.transitionDriver.overrideLayers.Add(new LadderDiseaseTransitionLayer(component2));
		component2.transitionDriver.overrideLayers.Add(new SplashTransitionLayer(component2));
		component2.SetFlags(PathFinder.PotentialPath.Flags.None);
		component2.CurrentNavType = NavType.Floor;
		PathProber component3 = inst.GetComponent<PathProber>();
		if (component3 != null)
		{
			component3.SetGroupProber(MinionGroupProber.Get());
		}
		Effects effects = inst.GetComponent<Effects>();
		if (inst.transform.parent == null)
		{
			if (effects.HasEffect("ScoutBotCharging"))
			{
				effects.Remove("ScoutBotCharging");
			}
		}
		else if (!effects.HasEffect("ScoutBotCharging"))
		{
			effects.Add("ScoutBotCharging", should_save: false);
		}
		inst.Subscribe(856640610, delegate
		{
			if (inst.transform.parent == null)
			{
				if (effects.HasEffect("ScoutBotCharging"))
				{
					effects.Remove("ScoutBotCharging");
				}
			}
			else if (!effects.HasEffect("ScoutBotCharging"))
			{
				effects.Add("ScoutBotCharging", should_save: false);
			}
		});
	}
}
