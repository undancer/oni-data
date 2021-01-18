using Klei.AI;
using UnityEngine;

public class Beefinery : StateMachineComponent<Beefinery.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Beefinery, object>.GameInstance
	{
		private RadiationEmitter emitter;

		public StatesInstance(Beefinery smi)
			: base(smi)
		{
			emitter = smi.GetComponent<RadiationEmitter>();
		}

		public void TurnOn()
		{
			emitter.emitRads = 300f;
			emitter.Refresh();
		}

		public void TurnOff()
		{
			emitter.emitRads = 100f;
			emitter.Refresh();
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Beefinery>
	{
		public class OnStates : State
		{
			public State pre;

			public State loop;

			public State pst;
		}

		public State off;

		public OnStates on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			off.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, on.pre, (StatesInstance smi) => smi.master.storage.FindFirst(SimHashes.Radium.CreateTag())).Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: false);
				smi.TurnOff();
			});
			on.EventTransition(GameHashes.OnStorageChange, on.pst, (StatesInstance smi) => !smi.master.storage.FindFirst(SimHashes.Radium.CreateTag())).Enter(delegate(StatesInstance smi)
			{
				smi.TurnOn();
				smi.GetComponent<Operational>().SetActive(value: true);
			});
			on.pre.PlayAnim("working_pre").OnAnimQueueComplete(on.loop);
			on.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).Update(EatOreFromStorage, UpdateRate.SIM_4000ms);
			on.pst.PlayAnim("working_pst").OnAnimQueueComplete(off);
		}

		public void EatOreFromStorage(StatesInstance smi, float dt)
		{
			GameObject gameObject = smi.master.GetComponent<Storage>().FindFirst(SimHashes.Radium.CreateTag());
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
			CreatureCalorieMonitor.Def def = smi.gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
			Diet.Info dietInfo = def.diet.GetDietInfo(component.PrefabTag);
			if (dietInfo != null)
			{
				AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(smi.gameObject);
				string properName = component.GetProperName();
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, properName, component.transform);
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

	public Storage storage;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
