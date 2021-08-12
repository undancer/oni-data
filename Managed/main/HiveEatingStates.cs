using Klei.AI;
using UnityEngine;

public class HiveEatingStates : GameStateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>
{
	public class Def : BaseDef
	{
		public Tag consumedOre;

		public Def(Tag consumedOre)
		{
			this.consumedOre = consumedOre;
		}
	}

	public class EatingStates : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public new class Instance : GameInstance
	{
		[MyCmpReq]
		public Storage storage;

		[MyCmpReq]
		private RadiationEmitter emitter;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToEat);
		}

		public void TurnOn()
		{
			emitter.emitRads = 60f * emitter.emitRate;
			emitter.Refresh();
		}

		public void TurnOff()
		{
			emitter.emitRads = 0f;
			emitter.Refresh();
		}

		public void EatOreFromStorage(Instance smi, float dt)
		{
			GameObject gameObject = smi.storage.FindFirst(smi.def.consumedOre);
			if (!gameObject)
			{
				return;
			}
			float num = 0.25f;
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (component == null)
			{
				return;
			}
			PrimaryElement component2 = component.GetComponent<PrimaryElement>();
			if (component2 == null)
			{
				return;
			}
			Diet.Info dietInfo = smi.gameObject.AddOrGetDef<BeehiveCalorieMonitor.Def>().diet.GetDietInfo(component.PrefabTag);
			if (dietInfo != null)
			{
				AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(smi.gameObject);
				float calories = amountInstance.GetMax() - amountInstance.value;
				float num2 = dietInfo.ConvertCaloriesToConsumptionMass(calories);
				float num3 = num * dt;
				if (num2 < num3)
				{
					num3 = num2;
				}
				num3 = Mathf.Min(num3, component2.Mass);
				component2.Mass -= num3;
				Pickupable component3 = component2.GetComponent<Pickupable>();
				if (component3.storage != null)
				{
					component3.storage.Trigger(-1452790913, smi.gameObject);
					component3.storage.Trigger(-1697596308, smi.gameObject);
				}
				float calories2 = dietInfo.ConvertConsumptionMassToCalories(num3);
				CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = default(CreatureCalorieMonitor.CaloriesConsumedEvent);
				caloriesConsumedEvent.tag = component.PrefabTag;
				caloriesConsumedEvent.calories = calories2;
				CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent2 = caloriesConsumedEvent;
				smi.gameObject.Trigger(-2038961714, caloriesConsumedEvent2);
			}
		}
	}

	public EatingStates eating;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = eating;
		base.serializable = SerializeType.ParamsOnly;
		eating.DefaultState(eating.pre).Enter(delegate(Instance smi)
		{
			smi.TurnOn();
		}).Exit(delegate(Instance smi)
		{
			smi.TurnOff();
		});
		eating.pre.PlayAnim("eating_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(eating.loop);
		eating.loop.PlayAnim("eating_loop", KAnim.PlayMode.Loop).Update(delegate(Instance smi, float dt)
		{
			smi.EatOreFromStorage(smi, dt);
		}, UpdateRate.SIM_4000ms).EventTransition(GameHashes.OnStorageChange, eating.pst, (Instance smi) => !smi.storage.FindFirst(smi.def.consumedOre));
		eating.pst.PlayAnim("eating_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToEat);
	}
}
