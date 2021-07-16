using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class RadiationVulnerable : GameStateMachine<RadiationVulnerable, RadiationVulnerable.StatesInstance, IStateMachineTarget, RadiationVulnerable.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			Modifiers component = go.GetComponent<Modifiers>();
			float preModifiedAttributeValue = component.GetPreModifiedAttributeValue(Db.Get().PlantAttributes.MinRadiationThreshold);
			string preModifiedAttributeFormattedValue = component.GetPreModifiedAttributeFormattedValue(Db.Get().PlantAttributes.MinRadiationThreshold);
			string preModifiedAttributeFormattedValue2 = component.GetPreModifiedAttributeFormattedValue(Db.Get().PlantAttributes.MaxRadiationThreshold);
			MutantPlant component2 = go.GetComponent<MutantPlant>();
			bool flag = component2 != null && component2.IsOriginal;
			if (preModifiedAttributeValue <= 0f)
			{
				return new List<Descriptor>
				{
					new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_NO_MIN_RADIATION.Replace("{MaxRads}", preModifiedAttributeFormattedValue2), UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_NO_MIN_RADIATION.Replace("{MaxRads}", preModifiedAttributeFormattedValue2) + (flag ? UI.GAMEOBJECTEFFECTS.TOOLTIPS.MUTANT_SEED_TOOLTIP.ToString() : ""), Descriptor.DescriptorType.Requirement)
				};
			}
			return new List<Descriptor>
			{
				new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_RADIATION.Replace("{MinRads}", preModifiedAttributeFormattedValue).Replace("{MaxRads}", preModifiedAttributeFormattedValue2), UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_RADIATION.Replace("{MinRads}", preModifiedAttributeFormattedValue).Replace("{MaxRads}", preModifiedAttributeFormattedValue2) + (flag ? UI.GAMEOBJECTEFFECTS.TOOLTIPS.MUTANT_SEED_TOOLTIP.ToString() : ""), Descriptor.DescriptorType.Requirement)
			};
		}
	}

	public class StatesInstance : GameInstance, IWiltCause
	{
		private AttributeInstance minRadiationAttributeInstance;

		private AttributeInstance maxRadiationAttributeInstance;

		public WiltCondition.Condition[] Conditions => new WiltCondition.Condition[1]
		{
			WiltCondition.Condition.Radiation
		};

		public string WiltStateString
		{
			get
			{
				if (base.smi.IsInsideState(base.smi.sm.too_dark))
				{
					return Db.Get().CreatureStatusItems.Crop_Too_NonRadiated.GetName(this);
				}
				if (base.smi.IsInsideState(base.smi.sm.too_bright))
				{
					return Db.Get().CreatureStatusItems.Crop_Too_Radiated.GetName(this);
				}
				return "";
			}
		}

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			minRadiationAttributeInstance = Db.Get().PlantAttributes.MinRadiationThreshold.Lookup(base.gameObject);
			maxRadiationAttributeInstance = Db.Get().PlantAttributes.MaxRadiationThreshold.Lookup(base.gameObject);
		}

		public int GetRadiationThresholdCrossed()
		{
			int num = Grid.PosToCell(base.master.gameObject);
			if (!Grid.IsValidCell(num))
			{
				return 0;
			}
			if (Grid.Radiation[num] < minRadiationAttributeInstance.GetTotalValue())
			{
				return -1;
			}
			if (Grid.Radiation[num] <= maxRadiationAttributeInstance.GetTotalValue())
			{
				return 0;
			}
			return 1;
		}
	}

	public State comfortable;

	public State too_dark;

	public State too_bright;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = comfortable;
		comfortable.Transition(too_dark, (StatesInstance smi) => smi.GetRadiationThresholdCrossed() == -1, UpdateRate.SIM_1000ms).Transition(too_bright, (StatesInstance smi) => smi.GetRadiationThresholdCrossed() == 1, UpdateRate.SIM_1000ms).TriggerOnEnter(GameHashes.RadiationComfort);
		too_dark.Transition(comfortable, (StatesInstance smi) => smi.GetRadiationThresholdCrossed() != -1, UpdateRate.SIM_1000ms).TriggerOnEnter(GameHashes.RadiationDiscomfort);
		too_bright.Transition(comfortable, (StatesInstance smi) => smi.GetRadiationThresholdCrossed() != 1, UpdateRate.SIM_1000ms).TriggerOnEnter(GameHashes.RadiationDiscomfort);
	}
}
