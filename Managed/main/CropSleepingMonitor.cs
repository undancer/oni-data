using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class CropSleepingMonitor : GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public bool prefersDarkness = false;

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			if (prefersDarkness)
			{
				return new List<Descriptor>
				{
					new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_DARKNESS, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_DARKNESS, Descriptor.DescriptorType.Requirement)
				};
			}
			Attribute minLightLux = Db.Get().PlantAttributes.MinLightLux;
			int lux = Mathf.RoundToInt(minLightLux.Lookup(obj)?.GetTotalValue() ?? obj.GetComponent<Modifiers>().GetPreModifiedAttributeValue(minLightLux));
			return new List<Descriptor>
			{
				new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(lux)), UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(lux)), Descriptor.DescriptorType.Requirement)
			};
		}
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public bool IsSleeping()
		{
			BaseState currentState = GetCurrentState();
			return currentState == base.smi.sm.sleeping;
		}

		public bool IsCellSafe(int cell)
		{
			Attribute minLightLux = Db.Get().PlantAttributes.MinLightLux;
			AttributeInstance attributeInstance = minLightLux.Lookup(base.gameObject);
			int num = Grid.LightIntensity[cell];
			return base.def.prefersDarkness ? (num == 0) : ((float)num >= attributeInstance.GetTotalValue());
		}
	}

	public State sleeping;

	public State awake;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = awake;
		base.serializable = SerializeType.Never;
		root.Update("CropSleepingMonitor.root", delegate(Instance smi, float dt)
		{
			int cell = Grid.PosToCell(smi.master.gameObject);
			State state = (smi.IsCellSafe(cell) ? awake : sleeping);
			smi.GoTo(state);
		}, UpdateRate.SIM_1000ms);
		sleeping.TriggerOnEnter(GameHashes.CropSleep).ToggleStatusItem(Db.Get().CreatureStatusItems.CropSleeping, (Instance smi) => smi);
		awake.TriggerOnEnter(GameHashes.CropWakeUp);
	}
}
