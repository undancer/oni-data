using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SapTree : GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>
{
	public class Def : BaseDef
	{
		public Vector2I foodSenseArea;

		public float massEatRate;

		public float kcalorieToKGConversionRatio;

		public float stomachSize;

		public float oozeRate;

		public List<Vector3> oozeOffsets;

		public Vector2I attackSenseArea;

		public float attackCooldown;
	}

	public class AliveStates : PlantAliveSubState
	{
		public NormalStates normal;

		public WiltingState wilting;
	}

	public class NormalStates : State
	{
		public State idle;

		public State eating;

		public State eating_pst;

		public State oozing;

		public State oozing_pst;

		public State attacking_pre;

		public State attacking;

		public State attacking_cooldown;

		public State attacking_done;
	}

	public class WiltingState : State
	{
		public State wilting_pre;

		public State wilting;

		public State wilting_pst;
	}

	public class StatesInstance : GameInstance
	{
		[MyCmpReq]
		public WiltCondition wiltCondition;

		[MyCmpReq]
		public EntombVulnerable entombVulnerable;

		[MyCmpReq]
		private Storage storage;

		[MyCmpReq]
		private Weapon weapon;

		private HandleVector<int>.Handle partitionerEntry;

		private Extents feedExtents;

		private Extents attackExtents;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			Vector2I vector2I = Grid.PosToXY(base.gameObject.transform.GetPosition());
			Vector2I vector2I2 = new Vector2I(vector2I.x - def.attackSenseArea.x / 2, vector2I.y);
			attackExtents = new Extents(vector2I2.x, vector2I2.y, def.attackSenseArea.x, def.attackSenseArea.y);
			partitionerEntry = GameScenePartitioner.Instance.Add("SapTreeAttacker", this, attackExtents, GameScenePartitioner.Instance.objectLayers[0], OnMinionChanged);
			Vector2I vector2I3 = new Vector2I(vector2I.x - def.foodSenseArea.x / 2, vector2I.y);
			feedExtents = new Extents(vector2I3.x, vector2I3.y, def.foodSenseArea.x, def.foodSenseArea.y);
		}

		protected override void OnCleanUp()
		{
			GameScenePartitioner.Instance.Free(ref partitionerEntry);
		}

		public void EatFoodItem(float dt)
		{
			Pickupable pickupable = base.sm.foodItem.Get(this).GetComponent<Pickupable>().Take(base.def.massEatRate * dt);
			if (pickupable != null)
			{
				float mass = pickupable.GetComponent<Edible>().Calories * 0.001f * base.def.kcalorieToKGConversionRatio;
				Util.KDestroyGameObject(pickupable.gameObject);
				PrimaryElement component = GetComponent<PrimaryElement>();
				storage.AddLiquid(SimHashes.Resin, mass, component.Temperature, byte.MaxValue, 0, keep_zero_mass: true, do_disease_transfer: false);
				base.sm.storedSap.Set(storage.GetMassAvailable(SimHashes.Resin.CreateTag()), this);
			}
		}

		public void Ooze(float dt)
		{
			float num = Mathf.Min(base.sm.storedSap.Get(this), dt * base.def.oozeRate);
			if (!(num <= 0f))
			{
				int index = Mathf.FloorToInt(GameClock.Instance.GetTime() % (float)base.def.oozeOffsets.Count);
				storage.DropSome(SimHashes.Resin.CreateTag(), num, ventGas: false, dumpLiquid: true, base.def.oozeOffsets[index]);
				base.sm.storedSap.Set(storage.GetMassAvailable(SimHashes.Resin.CreateTag()), this);
			}
		}

		public void CheckForFood()
		{
			ListPool<ScenePartitionerEntry, SapTree>.PooledList pooledList = ListPool<ScenePartitionerEntry, SapTree>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(feedExtents, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
			foreach (ScenePartitionerEntry item in pooledList)
			{
				Pickupable pickupable = item.obj as Pickupable;
				if (pickupable.GetComponent<Edible>() != null)
				{
					base.sm.foodItem.Set(pickupable.gameObject, this);
					return;
				}
			}
			base.sm.foodItem.Set(null, this);
		}

		public bool DoAttack()
		{
			int num = weapon.AttackArea(base.transform.GetPosition());
			base.sm.hasNearbyEnemy.Set(num > 0, this);
			return true;
		}

		private void OnMinionChanged(object obj)
		{
			if (obj as GameObject != null)
			{
				base.sm.hasNearbyEnemy.Set(value: true, this);
			}
		}
	}

	public AliveStates alive;

	public State dead;

	private TargetParameter foodItem;

	private BoolParameter hasNearbyEnemy;

	private FloatParameter storedSap;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = alive;
		base.serializable = SerializeType.ParamsOnly;
		dead.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).ToggleTag(GameTags.PreventEmittingDisease).Enter(delegate(StatesInstance smi)
		{
			GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.PlantDeathId), smi.master.transform.GetPosition(), Grid.SceneLayer.FXFront).SetActive(value: true);
			smi.master.Trigger(1623392196);
			smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
		});
		alive.InitializeStates(masterTarget, dead).DefaultState(alive.normal);
		alive.normal.DefaultState(alive.normal.idle).EventTransition(GameHashes.Wilt, alive.wilting, (StatesInstance smi) => smi.wiltCondition.IsWilting()).Update(delegate(StatesInstance smi, float dt)
		{
			smi.CheckForFood();
		}, UpdateRate.SIM_1000ms);
		alive.normal.idle.PlayAnim("idle", KAnim.PlayMode.Loop).ToggleStatusItem(CREATURES.STATUSITEMS.IDLE.NAME, CREATURES.STATUSITEMS.IDLE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).ParamTransition(hasNearbyEnemy, alive.normal.attacking_pre, GameStateMachine<SapTree, StatesInstance, IStateMachineTarget, Def>.IsTrue)
			.ParamTransition(storedSap, alive.normal.oozing, (StatesInstance smi, float p) => p >= smi.def.stomachSize)
			.ParamTransition(foodItem, alive.normal.eating, GameStateMachine<SapTree, StatesInstance, IStateMachineTarget, Def>.IsNotNull);
		alive.normal.eating.PlayAnim("eat_pre", KAnim.PlayMode.Once).QueueAnim("eat_loop", loop: true).Update(delegate(StatesInstance smi, float dt)
		{
			smi.EatFoodItem(dt);
		}, UpdateRate.SIM_1000ms)
			.ParamTransition(foodItem, alive.normal.eating_pst, GameStateMachine<SapTree, StatesInstance, IStateMachineTarget, Def>.IsNull)
			.ParamTransition(storedSap, alive.normal.eating_pst, (StatesInstance smi, float p) => p >= smi.def.stomachSize);
		alive.normal.eating_pst.PlayAnim("eat_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(alive.normal.idle);
		alive.normal.oozing.PlayAnim("ooze_pre", KAnim.PlayMode.Once).QueueAnim("ooze_loop", loop: true).Update(delegate(StatesInstance smi, float dt)
		{
			smi.Ooze(dt);
		})
			.ParamTransition(storedSap, alive.normal.oozing_pst, (StatesInstance smi, float p) => p <= 0f)
			.ParamTransition(hasNearbyEnemy, alive.normal.oozing_pst, GameStateMachine<SapTree, StatesInstance, IStateMachineTarget, Def>.IsTrue);
		alive.normal.oozing_pst.PlayAnim("ooze_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(alive.normal.idle);
		alive.normal.attacking_pre.PlayAnim("attacking_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(alive.normal.attacking);
		alive.normal.attacking.PlayAnim("attacking_loop", KAnim.PlayMode.Once).Enter(delegate(StatesInstance smi)
		{
			smi.DoAttack();
		}).OnAnimQueueComplete(alive.normal.attacking_cooldown);
		alive.normal.attacking_cooldown.PlayAnim("attacking_pst", KAnim.PlayMode.Once).QueueAnim("attack_cooldown", loop: true).ParamTransition(hasNearbyEnemy, alive.normal.attacking_done, GameStateMachine<SapTree, StatesInstance, IStateMachineTarget, Def>.IsFalse)
			.ScheduleGoTo((StatesInstance smi) => smi.def.attackCooldown, alive.normal.attacking);
		alive.normal.attacking_done.PlayAnim("attack_to_idle", KAnim.PlayMode.Once).OnAnimQueueComplete(alive.normal.idle);
		alive.wilting.PlayAnim("withered", KAnim.PlayMode.Loop).EventTransition(GameHashes.WiltRecover, alive.normal).ToggleTag(GameTags.PreventEmittingDisease);
	}
}
