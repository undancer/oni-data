using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ElementGrowthMonitor : GameStateMachine<ElementGrowthMonitor, ElementGrowthMonitor.Instance, IStateMachineTarget, ElementGrowthMonitor.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public int levelCount;

		public float defaultGrowthRate;

		public Tag itemDroppedOnShear;

		public float dropMass;

		public float minTemperature;

		public float maxTemperature;

		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.ElementGrowth.Id);
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			return new List<Descriptor>
			{
				new Descriptor(UI.BUILDINGEFFECTS.SCALE_GROWTH_TEMP.Replace("{Item}", itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(dropMass)).Replace("{Time}", GameUtil.GetFormattedCycles(1f / defaultGrowthRate))
					.Replace("{TempMin}", GameUtil.GetFormattedTemperature(minTemperature))
					.Replace("{TempMax}", GameUtil.GetFormattedTemperature(maxTemperature)), UI.BUILDINGEFFECTS.TOOLTIPS.SCALE_GROWTH_TEMP.Replace("{Item}", itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(dropMass)).Replace("{Time}", GameUtil.GetFormattedCycles(1f / defaultGrowthRate))
					.Replace("{TempMin}", GameUtil.GetFormattedTemperature(minTemperature))
					.Replace("{TempMax}", GameUtil.GetFormattedTemperature(maxTemperature)))
			};
		}
	}

	public class GrowingState : State
	{
		public State growing;

		public State stunted;
	}

	public new class Instance : GameInstance, IShearable
	{
		public AmountInstance elementGrowth;

		public AttributeModifier growingGrowthModifier;

		public AttributeModifier stuntedGrowthModifier;

		public int currentGrowthLevel = -1;

		[Serialize]
		public SimHashes lastConsumedElement;

		[Serialize]
		public float lastConsumedTemperature;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			elementGrowth = Db.Get().Amounts.ElementGrowth.Lookup(base.gameObject);
			elementGrowth.value = elementGrowth.GetMax();
			growingGrowthModifier = new AttributeModifier(elementGrowth.amount.deltaAttribute.Id, def.defaultGrowthRate * 100f, CREATURES.MODIFIERS.ELEMENT_GROWTH_RATE.NAME);
			stuntedGrowthModifier = new AttributeModifier(elementGrowth.amount.deltaAttribute.Id, def.defaultGrowthRate * 20f, CREATURES.MODIFIERS.ELEMENT_GROWTH_RATE.NAME);
		}

		public void OnEatSolidComplete(object data)
		{
			KPrefabID kPrefabID = (KPrefabID)data;
			if (!(kPrefabID == null))
			{
				PrimaryElement component = kPrefabID.GetComponent<PrimaryElement>();
				lastConsumedElement = component.ElementID;
				lastConsumedTemperature = component.Temperature;
			}
		}

		public bool IsFullyGrown()
		{
			return currentGrowthLevel == base.def.levelCount;
		}

		public void Shear()
		{
			PrimaryElement component = base.smi.GetComponent<PrimaryElement>();
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(base.def.itemDroppedOnShear));
			gameObject.transform.SetPosition(Grid.CellToPosCCC(Grid.CellLeft(Grid.PosToCell(this)), Grid.SceneLayer.Ore));
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			component2.Temperature = component.Temperature;
			component2.Mass = base.def.dropMass;
			component2.AddDisease(component.DiseaseIdx, component.DiseaseCount, "Shearing");
			gameObject.SetActive(value: true);
			Vector2 initial_velocity = new Vector2(Random.Range(-1f, 1f) * 1f, Random.value * 2f + 2f);
			if (GameComps.Fallers.Has(gameObject))
			{
				GameComps.Fallers.Remove(gameObject);
			}
			GameComps.Fallers.Add(gameObject, initial_velocity);
			elementGrowth.value = 0f;
			UpdateGrowth(this, 0f);
		}
	}

	public Tag[] HungryTags = new Tag[1] { GameTags.Creatures.Hungry };

	public State halted;

	public GrowingState growing;

	public State fullyGrown;

	private static HashedString[] GROWTH_SYMBOL_NAMES = new HashedString[5] { "del_ginger1", "del_ginger2", "del_ginger3", "del_ginger4", "del_ginger5" };

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = growing;
		root.Enter(delegate(Instance smi)
		{
			UpdateGrowth(smi, 0f);
		}).Update(UpdateGrowth, UpdateRate.SIM_1000ms).EventHandler(GameHashes.EatSolidComplete, delegate(Instance smi, object data)
		{
			smi.OnEatSolidComplete(data);
		});
		growing.DefaultState(growing.growing).Transition(fullyGrown, IsFullyGrown, UpdateRate.SIM_1000ms).TagTransition(HungryTags, halted);
		growing.growing.Transition(growing.stunted, GameStateMachine<ElementGrowthMonitor, Instance, IStateMachineTarget, Def>.Not(IsConsumedInTemperatureRange), UpdateRate.SIM_1000ms).ToggleStatusItem(Db.Get().CreatureStatusItems.ElementGrowthGrowing).Enter(ApplyModifier)
			.Exit(RemoveModifier);
		growing.stunted.Transition(growing.growing, IsConsumedInTemperatureRange, UpdateRate.SIM_1000ms).ToggleStatusItem(Db.Get().CreatureStatusItems.ElementGrowthStunted).Enter(ApplyModifier)
			.Exit(RemoveModifier);
		halted.TagTransition(HungryTags, growing, on_remove: true).ToggleStatusItem(Db.Get().CreatureStatusItems.ElementGrowthHalted);
		fullyGrown.ToggleStatusItem(Db.Get().CreatureStatusItems.ElementGrowthComplete).ToggleBehaviour(GameTags.Creatures.ScalesGrown, (Instance smi) => smi.HasTag(GameTags.Creatures.CanMolt)).Transition(growing, GameStateMachine<ElementGrowthMonitor, Instance, IStateMachineTarget, Def>.Not(IsFullyGrown), UpdateRate.SIM_1000ms);
	}

	private static bool IsConsumedInTemperatureRange(Instance smi)
	{
		if (smi.lastConsumedTemperature != 0f)
		{
			if (smi.lastConsumedTemperature >= smi.def.minTemperature)
			{
				return smi.lastConsumedTemperature <= smi.def.maxTemperature;
			}
			return false;
		}
		return true;
	}

	private static bool IsFullyGrown(Instance smi)
	{
		return smi.elementGrowth.value >= smi.elementGrowth.GetMax();
	}

	private static void ApplyModifier(Instance smi)
	{
		if (smi.IsInsideState(smi.sm.growing.growing))
		{
			smi.elementGrowth.deltaAttribute.Add(smi.growingGrowthModifier);
		}
		else if (smi.IsInsideState(smi.sm.growing.stunted))
		{
			smi.elementGrowth.deltaAttribute.Add(smi.stuntedGrowthModifier);
		}
	}

	private static void RemoveModifier(Instance smi)
	{
		smi.elementGrowth.deltaAttribute.Remove(smi.growingGrowthModifier);
		smi.elementGrowth.deltaAttribute.Remove(smi.stuntedGrowthModifier);
	}

	private static void UpdateGrowth(Instance smi, float dt)
	{
		int num = (int)((float)smi.def.levelCount * smi.elementGrowth.value / 100f);
		if (smi.currentGrowthLevel != num)
		{
			KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
			for (int i = 0; i < GROWTH_SYMBOL_NAMES.Length; i++)
			{
				bool is_visible = i == num - 1;
				component.SetSymbolVisiblity(GROWTH_SYMBOL_NAMES[i], is_visible);
			}
			smi.currentGrowthLevel = num;
		}
	}
}
