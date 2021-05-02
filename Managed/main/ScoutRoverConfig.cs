using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class ScoutRoverConfig : IEntityConfig
{
	public const string ID = "ScoutRover";

	public static string ROVER_BASE_TRAIT_ID = "ScoutRoverBaseTrait";

	public const int MAXIMUM_TECH_CONSTRUCTION_TIER = 1;

	public const float MASS = 100f;

	private const float WIDTH = 1f;

	private const float HEIGHT = 2f;

	public static GameObject CreateScout(string id, string name, string desc, string anim_file)
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity(id, name, desc, 100f, unitMass: true, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures);
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
		Traits traits = gameObject.AddOrGet<Traits>();
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
		gameObject.AddOrGet<AttributeLevels>();
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
		gameObject.AddOrGetDef<RobotAi.Def>();
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new RobotDeathStates.Def()).Add(new FallStates.Def()).Add(new DebugGoToStates.Def())
			.Add(new IdleStates.Def(), condition: true, Db.Get().ChoreTypes.Idle.priority);
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Robots.Models.ScoutRover, null);
		Navigator navigator = gameObject.AddOrGet<Navigator>();
		string text = (navigator.NavGridName = "RobotNavGrid");
		navigator.CurrentNavType = NavType.Floor;
		navigator.defaultSpeed = 2f;
		navigator.updateProber = true;
		navigator.sceneLayer = Grid.SceneLayer.Creatures;
		gameObject.AddOrGet<Sensors>();
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		pickupable.SetWorkTime(5f);
		SnapOn snapOn = gameObject.AddOrGet<SnapOn>();
		snapOn.snapPoints = new List<SnapOn.SnapPoint>(new SnapOn.SnapPoint[0]);
		component.SetSymbolVisiblity("snapto_pivot", is_visible: false);
		return gameObject;
	}

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		return CreateScout("ScoutRover", STRINGS.ROBOTS.MODELS.SCOUT.NAME, STRINGS.ROBOTS.MODELS.SCOUT.DESC, "scout_bot_kanim");
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
