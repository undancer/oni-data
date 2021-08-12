using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ScaleGrowthMonitor : GameStateMachine<ScaleGrowthMonitor, ScaleGrowthMonitor.Instance, IStateMachineTarget, ScaleGrowthMonitor.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public int levelCount;

		public float defaultGrowthRate;

		public SimHashes targetAtmosphere;

		public Tag itemDroppedOnShear;

		public float dropMass;

		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.ScaleGrowth.Id);
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			List<Descriptor> list = new List<Descriptor>();
			if (targetAtmosphere == (SimHashes)0)
			{
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.SCALE_GROWTH.Replace("{Item}", itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(dropMass)).Replace("{Time}", GameUtil.GetFormattedCycles(1f / defaultGrowthRate)), UI.BUILDINGEFFECTS.TOOLTIPS.SCALE_GROWTH.Replace("{Item}", itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(dropMass)).Replace("{Time}", GameUtil.GetFormattedCycles(1f / defaultGrowthRate))));
			}
			else
			{
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.SCALE_GROWTH_ATMO.Replace("{Item}", itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(dropMass)).Replace("{Time}", GameUtil.GetFormattedCycles(1f / defaultGrowthRate))
					.Replace("{Atmosphere}", targetAtmosphere.CreateTag().ProperName()), UI.BUILDINGEFFECTS.TOOLTIPS.SCALE_GROWTH_ATMO.Replace("{Item}", itemDroppedOnShear.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(dropMass)).Replace("{Time}", GameUtil.GetFormattedCycles(1f / defaultGrowthRate))
					.Replace("{Atmosphere}", targetAtmosphere.CreateTag().ProperName())));
			}
			return list;
		}
	}

	public class GrowingState : State
	{
		public State growing;

		public State stunted;
	}

	public new class Instance : GameInstance
	{
		public AmountInstance scaleGrowth;

		public AttributeModifier scaleGrowthModifier;

		public int currentScaleLevel = -1;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			scaleGrowth = Db.Get().Amounts.ScaleGrowth.Lookup(base.gameObject);
			scaleGrowth.value = scaleGrowth.GetMax();
			scaleGrowthModifier = new AttributeModifier(scaleGrowth.amount.deltaAttribute.Id, def.defaultGrowthRate * 100f, CREATURES.MODIFIERS.SCALE_GROWTH_RATE.NAME);
		}

		public bool IsFullyGrown()
		{
			return currentScaleLevel == base.def.levelCount;
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
			scaleGrowth.value = 0f;
			UpdateScales(this, 0f);
		}
	}

	public GrowingState growing;

	public State fullyGrown;

	private AttributeModifier scaleGrowthModifier;

	private static HashedString[] SCALE_SYMBOL_NAMES = new HashedString[5] { "scale_0", "scale_1", "scale_2", "scale_3", "scale_4" };

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = growing;
		root.Enter(delegate(Instance smi)
		{
			UpdateScales(smi, 0f);
		}).Update(UpdateScales, UpdateRate.SIM_1000ms);
		growing.DefaultState(growing.growing).Transition(fullyGrown, AreScalesFullyGrown, UpdateRate.SIM_1000ms);
		growing.growing.Transition(growing.stunted, GameStateMachine<ScaleGrowthMonitor, Instance, IStateMachineTarget, Def>.Not(IsInCorrectAtmosphere), UpdateRate.SIM_1000ms).Enter(ApplyModifier).Exit(RemoveModifier);
		growing.stunted.Transition(growing.growing, IsInCorrectAtmosphere, UpdateRate.SIM_1000ms).ToggleStatusItem(CREATURES.STATUSITEMS.STUNTED_SCALE_GROWTH.NAME, CREATURES.STATUSITEMS.STUNTED_SCALE_GROWTH.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		fullyGrown.ToggleTag(GameTags.Creatures.ScalesGrown).ToggleBehaviour(GameTags.Creatures.ScalesGrown, (Instance smi) => smi.HasTag(GameTags.Creatures.CanMolt)).Transition(growing, GameStateMachine<ScaleGrowthMonitor, Instance, IStateMachineTarget, Def>.Not(AreScalesFullyGrown), UpdateRate.SIM_1000ms);
	}

	private static bool IsInCorrectAtmosphere(Instance smi)
	{
		int num = Grid.PosToCell(smi);
		return Grid.Element[num].id == smi.def.targetAtmosphere;
	}

	private static bool AreScalesFullyGrown(Instance smi)
	{
		return smi.scaleGrowth.value >= smi.scaleGrowth.GetMax();
	}

	private static void ApplyModifier(Instance smi)
	{
		smi.scaleGrowth.deltaAttribute.Add(smi.scaleGrowthModifier);
	}

	private static void RemoveModifier(Instance smi)
	{
		smi.scaleGrowth.deltaAttribute.Remove(smi.scaleGrowthModifier);
	}

	private static void UpdateScales(Instance smi, float dt)
	{
		int num = (int)((float)smi.def.levelCount * smi.scaleGrowth.value / 100f);
		if (smi.currentScaleLevel != num)
		{
			KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
			for (int i = 0; i < SCALE_SYMBOL_NAMES.Length; i++)
			{
				bool is_visible = i <= num - 1;
				component.SetSymbolVisiblity(SCALE_SYMBOL_NAMES[i], is_visible);
			}
			smi.currentScaleLevel = num;
		}
	}
}
