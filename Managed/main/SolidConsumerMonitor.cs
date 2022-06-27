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
			float a = dietInfo.ConvertCaloriesToConsumptionMass(calories);
			Growing component2 = kPrefabID.GetComponent<Growing>();
			if (component2 != null)
			{
				BuddingTrunk component3 = kPrefabID.GetComponent<BuddingTrunk>();
				if ((bool)component3)
				{
					float maxBranchMaturity = component3.GetMaxBranchMaturity();
					a = Mathf.Min(a, maxBranchMaturity);
					component3.ConsumeMass(a);
				}
				else
				{
					float value = Db.Get().Amounts.Maturity.Lookup(component2.gameObject).value;
					a = Mathf.Min(a, value);
					component2.ConsumeMass(a);
				}
			}
			else
			{
				a = Mathf.Min(a, component.Mass);
				component.Mass -= a;
				Pickupable component4 = component.GetComponent<Pickupable>();
				if (component4.storage != null)
				{
					component4.storage.Trigger(-1452790913, base.gameObject);
					component4.storage.Trigger(-1697596308, base.gameObject);
				}
			}
			float calories2 = dietInfo.ConvertConsumptionMassToCalories(a);
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
		lookingforfood.TagTransition(GameTags.Creatures.Hungry, satisfied, on_remove: true).Update(FindFood, UpdateRate.SIM_4000ms, load_balance: true);
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
		ListPool<GameObject, SolidConsumerMonitor>.PooledList pooledList = ListPool<GameObject, SolidConsumerMonitor>.Allocate();
		Diet diet = smi.def.diet;
		int x = 0;
		int y = 0;
		Grid.PosToXY(smi.gameObject.transform.GetPosition(), out x, out y);
		x -= 8;
		y -= 8;
		ListPool<Storage, SolidConsumerMonitor>.PooledList pooledList2 = ListPool<Storage, SolidConsumerMonitor>.Allocate();
		if (!diet.eatsPlantsDirectly)
		{
			foreach (CreatureFeeder item in Components.CreatureFeeders.GetItems(smi.GetMyWorldId()))
			{
				Grid.PosToXY(item.transform.GetPosition(), out var x2, out var y2);
				if (x2 < x || x2 > x + 16 || y2 < y || y2 > y + 16)
				{
					continue;
				}
				item.GetComponents(pooledList2);
				foreach (Storage item2 in pooledList2)
				{
					if (item2 == null)
					{
						continue;
					}
					foreach (GameObject item3 in item2.items)
					{
						KPrefabID component = item3.GetComponent<KPrefabID>();
						component.UpdateTagBits();
						if (!component.HasAnyTags_AssumeLaundered(ref creatureMask) && diet.GetDietInfo(component.PrefabTag) != null)
						{
							pooledList.Add(item3);
						}
					}
				}
			}
		}
		pooledList2.Recycle();
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList3 = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		if (diet.eatsPlantsDirectly)
		{
			GameScenePartitioner.Instance.GatherEntries(x, y, 16, 16, GameScenePartitioner.Instance.plants, pooledList3);
			foreach (ScenePartitionerEntry item4 in pooledList3)
			{
				KPrefabID kPrefabID = (KPrefabID)item4.obj;
				kPrefabID.UpdateTagBits();
				if (kPrefabID.HasAnyTags_AssumeLaundered(ref creatureMask) || diet.GetDietInfo(kPrefabID.PrefabTag) == null)
				{
					continue;
				}
				if (kPrefabID.HasAnyTags_AssumeLaundered(ref plantMask))
				{
					float num = 0.25f;
					float num2 = 0f;
					BuddingTrunk component2 = kPrefabID.GetComponent<BuddingTrunk>();
					if ((bool)component2)
					{
						num2 = component2.GetMaxBranchMaturity();
					}
					else
					{
						AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(kPrefabID);
						if (amountInstance != null)
						{
							num2 = amountInstance.value / amountInstance.GetMax();
						}
					}
					if (num2 < num)
					{
						continue;
					}
				}
				pooledList.Add(kPrefabID.gameObject);
			}
		}
		else
		{
			GameScenePartitioner.Instance.GatherEntries(x, y, 16, 16, GameScenePartitioner.Instance.pickupablesLayer, pooledList3);
			foreach (ScenePartitionerEntry item5 in pooledList3)
			{
				Pickupable pickupable = (Pickupable)item5.obj;
				KPrefabID component3 = pickupable.GetComponent<KPrefabID>();
				component3.UpdateTagBits();
				if (!component3.HasAnyTags_AssumeLaundered(ref creatureMask) && diet.GetDietInfo(component3.PrefabTag) != null)
				{
					pooledList.Add(pickupable.gameObject);
				}
			}
		}
		pooledList3.Recycle();
		Navigator component4 = smi.GetComponent<Navigator>();
		DrowningMonitor component5 = smi.GetComponent<DrowningMonitor>();
		bool flag = component5 != null && component5.canDrownToDeath && !component5.livesUnderWater;
		smi.targetEdible = null;
		int num3 = -1;
		foreach (GameObject item6 in pooledList)
		{
			int cell = Grid.PosToCell(item6.transform.GetPosition());
			if (!flag || component5.IsCellSafe(cell))
			{
				int navigationCost = component4.GetNavigationCost(cell);
				if (navigationCost != -1 && (navigationCost < num3 || num3 == -1))
				{
					num3 = navigationCost;
					smi.targetEdible = item6.gameObject;
				}
			}
		}
		pooledList.Recycle();
	}
}
