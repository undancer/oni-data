using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class BeehiveCalorieMonitor : GameStateMachine<BeehiveCalorieMonitor, BeehiveCalorieMonitor.Instance, IStateMachineTarget, BeehiveCalorieMonitor.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public Diet diet;

		public float minPoopSizeInCalories = 100f;

		public float minimumTimeBeforePooping = 10f;

		public bool storePoop = true;

		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Calories.Id);
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_HEADER, UI.BUILDINGEFFECTS.TOOLTIPS.DIET_HEADER));
			float calorie_loss_per_second = 0f;
			foreach (AttributeModifier selfModifier in Db.Get().traits.Get(obj.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
			{
				if (selfModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
				{
					calorie_loss_per_second = selfModifier.Value;
				}
			}
			string newValue = string.Join(", ", diet.consumedTags.Select((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray());
			string newValue2 = string.Join("\n", diet.consumedTags.Select((KeyValuePair<Tag, float> t) => UI.BUILDINGEFFECTS.DIET_CONSUMED_ITEM.text.Replace("{Food}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass((0f - calorie_loss_per_second) / t.Value, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.Kilogram))).ToArray());
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_CONSUMED.text.Replace("{Foodlist}", newValue), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_CONSUMED.text.Replace("{Foodlist}", newValue2)));
			string newValue3 = string.Join(", ", diet.producedTags.Select((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray());
			string newValue4 = string.Join("\n", diet.producedTags.Select((KeyValuePair<Tag, float> t) => UI.BUILDINGEFFECTS.DIET_PRODUCED_ITEM.text.Replace("{Item}", t.Key.ProperName()).Replace("{Percent}", GameUtil.GetFormattedPercent(t.Value * 100f))).ToArray());
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_PRODUCED.text.Replace("{Items}", newValue3), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_PRODUCED.text.Replace("{Items}", newValue4)));
			return list;
		}
	}

	public new class Instance : GameInstance
	{
		public const float HUNGRY_RATIO = 0.9f;

		public AmountInstance calories;

		[Serialize]
		public CreatureCalorieMonitor.Stomach stomach;

		public float lastMealOrPoopTime;

		public AttributeInstance metabolism;

		public AttributeModifier deltaCalorieMetabolismModifier;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			calories = Db.Get().Amounts.Calories.Lookup(base.gameObject);
			calories.value = calories.GetMax() * 0.9f;
			stomach = new CreatureCalorieMonitor.Stomach(def.diet, master.gameObject, def.minPoopSizeInCalories, def.storePoop);
			metabolism = base.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Metabolism);
			deltaCalorieMetabolismModifier = new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, 1f, DUPLICANTS.MODIFIERS.METABOLISM_CALORIE_MODIFIER.NAME, is_multiplier: true, uiOnly: false, is_readonly: false);
			calories.deltaAttribute.Add(deltaCalorieMetabolismModifier);
		}

		public void OnCaloriesConsumed(object data)
		{
			CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = (CreatureCalorieMonitor.CaloriesConsumedEvent)data;
			calories.value += caloriesConsumedEvent.calories;
			stomach.Consume(caloriesConsumedEvent.tag, caloriesConsumedEvent.calories);
			lastMealOrPoopTime = Time.time;
		}

		public void Poop()
		{
			lastMealOrPoopTime = Time.time;
			stomach.Poop();
		}

		public float GetCalories0to1()
		{
			return calories.value / calories.GetMax();
		}

		public bool IsHungry()
		{
			return GetCalories0to1() < 0.9f;
		}
	}

	public State normal;

	public State hungry;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = normal;
		base.serializable = SerializeType.Both_DEPRECATED;
		root.EventHandler(GameHashes.CaloriesConsumed, delegate(Instance smi, object data)
		{
			smi.OnCaloriesConsumed(data);
		}).ToggleBehaviour(GameTags.Creatures.Poop, ReadyToPoop, delegate(Instance smi)
		{
			smi.Poop();
		}).Update(UpdateMetabolismCalorieModifier);
		normal.Transition(hungry, (Instance smi) => smi.IsHungry(), UpdateRate.SIM_1000ms);
		hungry.ToggleTag(GameTags.Creatures.Hungry).EventTransition(GameHashes.CaloriesConsumed, normal, (Instance smi) => !smi.IsHungry()).ToggleStatusItem(Db.Get().CreatureStatusItems.HiveHungry)
			.Transition(normal, (Instance smi) => !smi.IsHungry(), UpdateRate.SIM_1000ms);
	}

	private static bool ReadyToPoop(Instance smi)
	{
		if (!smi.stomach.IsReadyToPoop())
		{
			return false;
		}
		if (Time.time - smi.lastMealOrPoopTime < smi.def.minimumTimeBeforePooping)
		{
			return false;
		}
		return true;
	}

	private static void UpdateMetabolismCalorieModifier(Instance smi, float dt)
	{
		smi.deltaCalorieMetabolismModifier.SetValue(1f - smi.metabolism.GetTotalValue() / 100f);
	}
}
