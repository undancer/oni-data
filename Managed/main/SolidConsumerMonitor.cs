using System.Diagnostics;
using Klei.AI;
using UnityEngine;

public class SolidConsumerMonitor : GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>
{
	public class Def : BaseDef
	{
		public Diet diet;
	}

	public new class Instance : GameInstance
	{
		public GameObject targetEdible;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void OnEatSolidComplete(object data)
		{
			KPrefabID kPrefabID = data as KPrefabID;
			if (kPrefabID == null)
			{
				return;
			}
			PrimaryElement component = kPrefabID.GetComponent<PrimaryElement>();
			if (component == null)
			{
				return;
			}
			Diet.Info dietInfo = base.def.diet.GetDietInfo(kPrefabID.PrefabTag);
			if (dietInfo == null)
			{
				return;
			}
			AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(base.smi.gameObject);
			string properName = kPrefabID.GetProperName();
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, properName, kPrefabID.transform);
			float calories = amountInstance.GetMax() - amountInstance.value;
			float num = dietInfo.ConvertCaloriesToConsumptionMass(calories);
			Growing component2 = kPrefabID.GetComponent<Growing>();
			if (component2 != null)
			{
				BuddingTrunk component3 = kPrefabID.GetComponent<BuddingTrunk>();
				if ((bool)component3)
				{
					component3.ConsumeMass(num);
				}
				else
				{
					component2.ConsumeMass(num);
				}
			}
			else
			{
				num = Mathf.Min(num, component.Mass);
				component.Mass -= num;
				Pickupable component4 = component.GetComponent<Pickupable>();
				if (component4.storage != null)
				{
					component4.storage.Trigger(-1452790913, base.gameObject);
					component4.storage.Trigger(-1697596308, base.gameObject);
				}
			}
			float calories2 = dietInfo.ConvertConsumptionMassToCalories(num);
			CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = default(CreatureCalorieMonitor.CaloriesConsumedEvent);
			caloriesConsumedEvent.tag = kPrefabID.PrefabTag;
			caloriesConsumedEvent.calories = calories2;
			CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent2 = caloriesConsumedEvent;
			Trigger(-2038961714, caloriesConsumedEvent2);
			targetEdible = null;
		}
	}

	private State satisfied;

	private State lookingforfood;

	private static TagBits plantMask = new TagBits(GameTags.GrowingPlant);

	private static TagBits creatureMask = new TagBits(new Tag[2]
	{
		GameTags.Creatures.ReservedByCreature,
		GameTags.CreatureBrain
	});

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.EventHandler(GameHashes.EatSolidComplete, delegate(Instance smi, object data)
		{
			smi.OnEatSolidComplete(data);
		}).ToggleBehaviour(GameTags.Creatures.WantsToEat, (Instance smi) => smi.targetEdible != null && !smi.targetEdible.HasTag(GameTags.Creatures.ReservedByCreature));
		satisfied.TagTransition(GameTags.Creatures.Hungry, lookingforfood);
		lookingforfood.TagTransition(GameTags.Creatures.Hungry, satisfied, on_remove: true).Update(FindFood, UpdateRate.SIM_1000ms, load_balance: true);
	}

	[Conditional("DETAILED_SOLID_CONSUMER_MONITOR_PROFILE")]
	private static void BeginDetailedSample(string region_name)
	{
	}

	[Conditional("DETAILED_SOLID_CONSUMER_MONITOR_PROFILE")]
	private static void EndDetailedSample(string region_name)
	{
	}

	private static void FindFood(Instance smi, float dt)
	{
		ListPool<KMonoBehaviour, SolidConsumerMonitor>.PooledList pooledList = ListPool<KMonoBehaviour, SolidConsumerMonitor>.Allocate();
		ListPool<Storage, SolidConsumerMonitor>.PooledList pooledList2 = ListPool<Storage, SolidConsumerMonitor>.Allocate();
		foreach (CreatureFeeder item in Components.CreatureFeeders.Items)
		{
			item.GetComponents(pooledList2);
			foreach (Storage item2 in pooledList2)
			{
				if (item2 == null)
				{
					continue;
				}
				foreach (GameObject item3 in item2.items)
				{
					pooledList.Add((item3 != null) ? item3.GetComponent<KMonoBehaviour>() : null);
				}
			}
		}
		pooledList2.Recycle();
		int x = 0;
		int y = 0;
		Grid.PosToXY(smi.gameObject.transform.GetPosition(), out x, out y);
		x -= 8;
		y -= 8;
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList3 = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(x, y, 16, 16, GameScenePartitioner.Instance.pickupablesLayer, pooledList3);
		GameScenePartitioner.Instance.GatherEntries(x, y, 16, 16, GameScenePartitioner.Instance.plants, pooledList3);
		foreach (ScenePartitionerEntry item4 in pooledList3)
		{
			pooledList.Add(item4.obj as KMonoBehaviour);
		}
		pooledList3.Recycle();
		Diet diet = smi.def.diet;
		for (int i = 0; i != pooledList.Count; i++)
		{
			KMonoBehaviour kMonoBehaviour = pooledList[i];
			if (kMonoBehaviour == null)
			{
				continue;
			}
			KPrefabID component = kMonoBehaviour.GetComponent<KPrefabID>();
			component.UpdateTagBits();
			if (component.HasAnyTags_AssumeLaundered(ref creatureMask) || diet.GetDietInfo(component.PrefabTag) == null)
			{
				pooledList[i] = null;
			}
			else
			{
				if (!component.HasAnyTags_AssumeLaundered(ref plantMask))
				{
					continue;
				}
				float num = 0.25f;
				float num2 = 0f;
				BuddingTrunk component2 = component.GetComponent<BuddingTrunk>();
				if ((bool)component2)
				{
					num2 = component2.GetMaxBranchMaturity();
				}
				else
				{
					AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(component);
					if (amountInstance != null)
					{
						num2 = amountInstance.value / amountInstance.GetMax();
					}
				}
				if (num2 < num)
				{
					pooledList[i] = null;
				}
			}
		}
		Navigator component3 = smi.GetComponent<Navigator>();
		DrowningMonitor component4 = smi.GetComponent<DrowningMonitor>();
		bool flag = component4 != null && component4.canDrownToDeath && !component4.livesUnderWater;
		smi.targetEdible = null;
		int num3 = -1;
		foreach (KMonoBehaviour item5 in pooledList)
		{
			if (item5 == null)
			{
				continue;
			}
			int cell = Grid.PosToCell(item5.gameObject.transform.GetPosition());
			if (!flag || component4.IsCellSafe(cell))
			{
				int navigationCost = component3.GetNavigationCost(cell);
				if (navigationCost != -1 && (navigationCost < num3 || num3 == -1))
				{
					num3 = navigationCost;
					smi.targetEdible = item5.gameObject;
				}
			}
		}
		pooledList.Recycle();
	}
}
