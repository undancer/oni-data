using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class CreatureCalorieMonitor : GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>
{
	public struct CaloriesConsumedEvent
	{
		public Tag tag;

		public float calories;
	}

	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public Diet diet;

		public float minPoopSizeInCalories = 100f;

		public float minimumTimeBeforePooping = 10f;

		public float deathTimer = 6000f;

		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Calories.Id);
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_HEADER, UI.BUILDINGEFFECTS.TOOLTIPS.DIET_HEADER));
			float dailyPlantGrowthConsumption = 1f;
			if (diet.consumedTags.Count > 0)
			{
				float calorie_loss_per_second = 0f;
				foreach (AttributeModifier selfModifier in Db.Get().traits.Get(obj.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
				{
					if (selfModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
					{
						calorie_loss_per_second = selfModifier.Value;
					}
				}
				string newValue = string.Join(", ", diet.consumedTags.Select((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray());
				string text = "";
				text = ((!diet.eatsPlantsDirectly) ? string.Join("\n", diet.consumedTags.Select((KeyValuePair<Tag, float> t) => UI.BUILDINGEFFECTS.DIET_CONSUMED_ITEM.text.Replace("{Food}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass((0f - calorie_loss_per_second) / t.Value, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.Kilogram))).ToArray()) : string.Join("\n", diet.consumedTags.Select(delegate(KeyValuePair<Tag, float> t)
				{
					dailyPlantGrowthConsumption = (0f - calorie_loss_per_second) / t.Value;
					GameObject prefab = Assets.GetPrefab(t.Key.ToString());
					Crop crop = prefab.GetComponent<Crop>();
					float num = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == crop.cropId).cropDuration / 600f;
					float num2 = 1f / num;
					return UI.BUILDINGEFFECTS.DIET_CONSUMED_ITEM.text.Replace("{Food}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedPlantGrowth((0f - calorie_loss_per_second) / t.Value * num2 * 100f, GameUtil.TimeSlice.PerCycle));
				}).ToArray()));
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_CONSUMED.text.Replace("{Foodlist}", newValue), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_CONSUMED.text.Replace("{Foodlist}", text)));
			}
			if (diet.producedTags.Count > 0)
			{
				string newValue2 = string.Join(", ", diet.producedTags.Select((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray());
				string text2 = "";
				text2 = ((!diet.eatsPlantsDirectly) ? string.Join("\n", diet.producedTags.Select((KeyValuePair<Tag, float> t) => UI.BUILDINGEFFECTS.DIET_PRODUCED_ITEM.text.Replace("{Item}", t.Key.ProperName()).Replace("{Percent}", GameUtil.GetFormattedPercent(t.Value * 100f))).ToArray()) : string.Join("\n", diet.producedTags.Select((KeyValuePair<Tag, float> t) => UI.BUILDINGEFFECTS.DIET_PRODUCED_ITEM_FROM_PLANT.text.Replace("{Item}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(t.Value * dailyPlantGrowthConsumption, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.Kilogram))).ToArray()));
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_PRODUCED.text.Replace("{Items}", newValue2), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_PRODUCED.text.Replace("{Items}", text2)));
			}
			return list;
		}
	}

	public class HungryStates : State
	{
		public class OutOfCaloriesState : State
		{
			public State wild;

			public State tame;

			public State starvedtodeath;
		}

		public State hungry;

		public OutOfCaloriesState outofcalories;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public class Stomach
	{
		[Serializable]
		public struct CaloriesConsumedEntry
		{
			public Tag tag;

			public float calories;
		}

		[Serialize]
		private List<CaloriesConsumedEntry> caloriesConsumed = new List<CaloriesConsumedEntry>();

		private float minPoopSizeInCalories;

		private GameObject owner;

		public Diet diet
		{
			get;
			private set;
		}

		public Stomach(Diet diet, GameObject owner, float min_poop_size_in_calories)
		{
			this.diet = diet;
			this.owner = owner;
			minPoopSizeInCalories = min_poop_size_in_calories;
		}

		public void Poop()
		{
			float num = 0f;
			Tag tag = Tag.Invalid;
			byte disease_idx = byte.MaxValue;
			int num2 = 0;
			bool flag = false;
			for (int i = 0; i < caloriesConsumed.Count; i++)
			{
				CaloriesConsumedEntry value = caloriesConsumed[i];
				if (!(value.calories <= 0f))
				{
					Diet.Info dietInfo = diet.GetDietInfo(value.tag);
					if (dietInfo != null && (!(tag != Tag.Invalid) || !(tag != dietInfo.producedElement)))
					{
						num += dietInfo.ConvertConsumptionMassToProducedMass(dietInfo.ConvertCaloriesToConsumptionMass(value.calories));
						tag = dietInfo.producedElement;
						disease_idx = dietInfo.diseaseIdx;
						num2 = (int)(dietInfo.diseasePerKgProduced * num);
						value.calories = 0f;
						caloriesConsumed[i] = value;
						flag = flag || dietInfo.produceSolidTile;
					}
				}
			}
			if (num <= 0f || tag == Tag.Invalid)
			{
				return;
			}
			Element element = ElementLoader.GetElement(tag);
			Debug.Assert(element != null, "TODO: implement non-element tag spawning");
			int num3 = Grid.PosToCell(owner.transform.GetPosition());
			float temperature = owner.GetComponent<PrimaryElement>().Temperature;
			if (element.IsLiquid)
			{
				FallingWater.instance.AddParticle(num3, element.idx, num, temperature, disease_idx, num2, skip_sound: true);
			}
			else if (element.IsGas)
			{
				SimMessages.AddRemoveSubstance(num3, element.idx, CellEventLogger.Instance.ElementConsumerSimUpdate, num, temperature, disease_idx, num2);
			}
			else if (flag)
			{
				int num4 = owner.GetComponent<Facing>().GetFrontCell();
				if (!Grid.IsValidCell(num4))
				{
					Debug.LogWarningFormat("{0} attemping to Poop {1} on invalid cell {2} from cell {3}", owner, element.name, num4, num3);
					num4 = num3;
				}
				SimMessages.AddRemoveSubstance(num4, element.idx, CellEventLogger.Instance.ElementConsumerSimUpdate, num, temperature, disease_idx, num2);
			}
			else
			{
				element.substance.SpawnResource(Grid.CellToPosCCC(num3, Grid.SceneLayer.Ore), num, temperature, disease_idx, num2);
			}
			KPrefabID component = owner.GetComponent<KPrefabID>();
			if (!Game.Instance.savedInfo.creaturePoopAmount.ContainsKey(component.PrefabTag))
			{
				Game.Instance.savedInfo.creaturePoopAmount.Add(component.PrefabTag, 0f);
			}
			Game.Instance.savedInfo.creaturePoopAmount[component.PrefabTag] += num;
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, element.name, owner.transform);
		}

		public List<CaloriesConsumedEntry> GetCalorieEntries()
		{
			return caloriesConsumed;
		}

		public float GetTotalConsumedCalories()
		{
			float num = 0f;
			foreach (CaloriesConsumedEntry item in caloriesConsumed)
			{
				if (!(item.calories <= 0f))
				{
					Diet.Info dietInfo = diet.GetDietInfo(item.tag);
					if (dietInfo != null && !(dietInfo.producedElement == Tag.Invalid))
					{
						num += item.calories;
					}
				}
			}
			return num;
		}

		public float GetFullness()
		{
			return GetTotalConsumedCalories() / minPoopSizeInCalories;
		}

		public bool IsReadyToPoop()
		{
			float totalConsumedCalories = GetTotalConsumedCalories();
			if (totalConsumedCalories > 0f)
			{
				return totalConsumedCalories >= minPoopSizeInCalories;
			}
			return false;
		}

		public void Consume(Tag tag, float calories)
		{
			for (int i = 0; i < caloriesConsumed.Count; i++)
			{
				CaloriesConsumedEntry value = caloriesConsumed[i];
				if (value.tag == tag)
				{
					value.calories += calories;
					caloriesConsumed[i] = value;
					return;
				}
			}
			CaloriesConsumedEntry item = default(CaloriesConsumedEntry);
			item.tag = tag;
			item.calories = calories;
			caloriesConsumed.Add(item);
		}

		public Tag GetNextPoopEntry()
		{
			for (int i = 0; i < caloriesConsumed.Count; i++)
			{
				CaloriesConsumedEntry caloriesConsumedEntry = caloriesConsumed[i];
				if (!(caloriesConsumedEntry.calories <= 0f))
				{
					Diet.Info dietInfo = diet.GetDietInfo(caloriesConsumedEntry.tag);
					if (dietInfo != null && !(dietInfo.producedElement == Tag.Invalid))
					{
						return dietInfo.producedElement;
					}
				}
			}
			return Tag.Invalid;
		}
	}

	public new class Instance : GameInstance
	{
		public const float HUNGRY_RATIO = 0.9f;

		public AmountInstance calories;

		[Serialize]
		public Stomach stomach;

		public float lastMealOrPoopTime;

		public AttributeInstance metabolism;

		public AttributeModifier deltaCalorieMetabolismModifier;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			calories = Db.Get().Amounts.Calories.Lookup(base.gameObject);
			calories.value = calories.GetMax() * 0.9f;
			stomach = new Stomach(def.diet, master.gameObject, def.minPoopSizeInCalories);
			metabolism = base.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Metabolism);
			deltaCalorieMetabolismModifier = new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, 1f, DUPLICANTS.MODIFIERS.METABOLISM_CALORIE_MODIFIER.NAME, is_multiplier: true, uiOnly: false, is_readonly: false);
			calories.deltaAttribute.Add(deltaCalorieMetabolismModifier);
		}

		public void OnCaloriesConsumed(object data)
		{
			CaloriesConsumedEvent caloriesConsumedEvent = (CaloriesConsumedEvent)data;
			calories.value += caloriesConsumedEvent.calories;
			stomach.Consume(caloriesConsumedEvent.tag, caloriesConsumedEvent.calories);
			lastMealOrPoopTime = Time.time;
		}

		public float GetDeathTimeRemaining()
		{
			return base.smi.def.deathTimer - (GameClock.Instance.GetTime() - base.sm.starvationStartTime.Get(base.smi));
		}

		public void Poop()
		{
			lastMealOrPoopTime = Time.time;
			stomach.Poop();
		}

		private float GetCalories0to1()
		{
			return calories.value / calories.GetMax();
		}

		public bool IsHungry()
		{
			return GetCalories0to1() < 0.9f;
		}

		public bool IsOutOfCalories()
		{
			return GetCalories0to1() <= 0f;
		}
	}

	public State normal;

	private HungryStates hungry;

	private Effect outOfCaloriesTame;

	public FloatParameter starvationStartTime;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = normal;
		base.serializable = true;
		root.EventHandler(GameHashes.CaloriesConsumed, delegate(Instance smi, object data)
		{
			smi.OnCaloriesConsumed(data);
		}).ToggleBehaviour(GameTags.Creatures.Poop, ReadyToPoop, delegate(Instance smi)
		{
			smi.Poop();
		}).Update(UpdateMetabolismCalorieModifier);
		normal.Transition(hungry, (Instance smi) => smi.IsHungry(), UpdateRate.SIM_1000ms);
		hungry.DefaultState(hungry.hungry).ToggleTag(GameTags.Creatures.Hungry).EventTransition(GameHashes.CaloriesConsumed, normal, (Instance smi) => !smi.IsHungry());
		hungry.hungry.Transition(normal, (Instance smi) => !smi.IsHungry(), UpdateRate.SIM_1000ms).Transition(hungry.outofcalories, (Instance smi) => smi.IsOutOfCalories(), UpdateRate.SIM_1000ms).ToggleStatusItem(Db.Get().CreatureStatusItems.Hungry);
		hungry.outofcalories.DefaultState(hungry.outofcalories.wild).Transition(hungry.hungry, (Instance smi) => !smi.IsOutOfCalories(), UpdateRate.SIM_1000ms);
		hungry.outofcalories.wild.TagTransition(GameTags.Creatures.Wild, hungry.outofcalories.tame, on_remove: true).ToggleStatusItem(Db.Get().CreatureStatusItems.Hungry);
		hungry.outofcalories.tame.Enter("StarvationStartTime", StarvationStartTime).Exit("ClearStarvationTime", delegate(Instance smi)
		{
			starvationStartTime.Set(0f, smi);
		}).Transition(hungry.outofcalories.starvedtodeath, (Instance smi) => smi.GetDeathTimeRemaining() <= 0f, UpdateRate.SIM_1000ms)
			.TagTransition(GameTags.Creatures.Wild, hungry.outofcalories.wild)
			.ToggleStatusItem(STRINGS.CREATURES.STATUSITEMS.STARVING.NAME, STRINGS.CREATURES.STATUSITEMS.STARVING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, default(HashedString), 129022, (string str, Instance smi) => str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedCycles(smi.GetDeathTimeRemaining())))
			.ToggleNotification((Instance smi) => new Notification(STRINGS.CREATURES.STATUSITEMS.STARVING.NOTIFICATION_NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notifications, object data) => string.Concat(STRINGS.CREATURES.STATUSITEMS.STARVING.NOTIFICATION_TOOLTIP, notifications.ReduceMessages(countNames: false))))
			.ToggleEffect((Instance smi) => outOfCaloriesTame);
		hungry.outofcalories.starvedtodeath.Enter(delegate(Instance smi)
		{
			smi.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Starvation);
		});
		outOfCaloriesTame = new Effect("OutOfCaloriesTame", STRINGS.CREATURES.MODIFIERS.OUT_OF_CALORIES.NAME, STRINGS.CREATURES.MODIFIERS.OUT_OF_CALORIES.TOOLTIP, 0f, show_in_ui: false, trigger_floating_text: false, is_bad: false);
		outOfCaloriesTame.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, STRINGS.CREATURES.MODIFIERS.OUT_OF_CALORIES.NAME));
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

	private static void StarvationStartTime(Instance smi)
	{
		if (smi.sm.starvationStartTime.Get(smi) == 0f)
		{
			smi.sm.starvationStartTime.Set(GameClock.Instance.GetTime(), smi);
		}
	}
}
