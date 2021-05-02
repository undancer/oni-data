using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class IlluminationVulnerable : StateMachineComponent<IlluminationVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, IlluminationVulnerable, object>.GameInstance
	{
		public bool hasMaturity = false;

		public StatesInstance(IlluminationVulnerable master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, IlluminationVulnerable>
	{
		public BoolParameter illuminated;

		public State comfortable;

		public State too_dark;

		public State too_bright;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = comfortable;
			root.Update("Illumination", delegate(StatesInstance smi, float dt)
			{
				smi.master.GetAmounts().Get(Db.Get().Amounts.Illumination).SetValue(Grid.LightCount[Grid.PosToCell(smi.master.gameObject)]);
			}, UpdateRate.SIM_1000ms);
			comfortable.Update("Illumination.Comfortable", delegate(StatesInstance smi, float dt)
			{
				int cell3 = Grid.PosToCell(smi.master.gameObject);
				if (!smi.master.IsCellSafe(cell3))
				{
					State state = (smi.master.prefersDarkness ? too_bright : too_dark);
					smi.GoTo(state);
				}
			}, UpdateRate.SIM_1000ms).Enter(delegate(StatesInstance smi)
			{
				smi.master.Trigger(1113102781);
			});
			too_dark.TriggerOnEnter(GameHashes.IlluminationDiscomfort).Update("Illumination.too_dark", delegate(StatesInstance smi, float dt)
			{
				int cell2 = Grid.PosToCell(smi.master.gameObject);
				if (smi.master.IsCellSafe(cell2))
				{
					smi.GoTo(comfortable);
				}
			}, UpdateRate.SIM_1000ms);
			too_bright.TriggerOnEnter(GameHashes.IlluminationDiscomfort).Update("Illumination.too_bright", delegate(StatesInstance smi, float dt)
			{
				int cell = Grid.PosToCell(smi.master.gameObject);
				if (smi.master.IsCellSafe(cell))
				{
					smi.GoTo(comfortable);
				}
			}, UpdateRate.SIM_1000ms);
		}
	}

	private OccupyArea _occupyArea;

	private SchedulerHandle handle;

	public bool prefersDarkness = false;

	private AttributeInstance minLuxAttributeInstance;

	public int LightIntensityThreshold
	{
		get
		{
			if (minLuxAttributeInstance != null)
			{
				return Mathf.RoundToInt(minLuxAttributeInstance.GetTotalValue());
			}
			return Mathf.RoundToInt(GetComponent<Modifiers>().GetPreModifiedAttributeValue(Db.Get().PlantAttributes.MinLightLux));
		}
	}

	private OccupyArea occupyArea
	{
		get
		{
			if (_occupyArea == null)
			{
				_occupyArea = GetComponent<OccupyArea>();
			}
			return _occupyArea;
		}
	}

	WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[2]
	{
		WiltCondition.Condition.Darkness,
		WiltCondition.Condition.IlluminationComfort
	};

	public string WiltStateString
	{
		get
		{
			if (base.smi.IsInsideState(base.smi.sm.too_bright))
			{
				return Db.Get().CreatureStatusItems.Crop_Too_Bright.GetName(this);
			}
			if (base.smi.IsInsideState(base.smi.sm.too_dark))
			{
				return Db.Get().CreatureStatusItems.Crop_Too_Dark.GetName(this);
			}
			return "";
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.GetAmounts().Add(new AmountInstance(Db.Get().Amounts.Illumination, base.gameObject));
		minLuxAttributeInstance = base.gameObject.GetAttributes().Add(Db.Get().PlantAttributes.MinLightLux);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void SetPrefersDarkness(bool prefersDarkness = false)
	{
		this.prefersDarkness = prefersDarkness;
	}

	protected override void OnCleanUp()
	{
		handle.ClearScheduler();
		base.OnCleanUp();
	}

	public bool IsCellSafe(int cell)
	{
		if (prefersDarkness)
		{
			return Grid.LightIntensity[cell] == 0;
		}
		return Grid.LightIntensity[cell] > LightIntensityThreshold;
	}

	public bool IsComfortable()
	{
		return base.smi.IsInsideState(base.smi.sm.comfortable);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		if (prefersDarkness)
		{
			return new List<Descriptor>
			{
				new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_DARKNESS, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_DARKNESS, Descriptor.DescriptorType.Requirement)
			};
		}
		return new List<Descriptor>
		{
			new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(LightIntensityThreshold)), UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(LightIntensityThreshold)), Descriptor.DescriptorType.Requirement)
		};
	}
}
