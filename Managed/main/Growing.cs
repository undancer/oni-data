using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Growing : StateMachineComponent<Growing.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Growing, object>.GameInstance
	{
		public AttributeModifier baseGrowingRate;

		public AttributeModifier wildGrowingRate;

		public AttributeModifier getOldRate;

		public StatesInstance(Growing master)
			: base(master)
		{
			baseGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.0016666667f, CREATURES.STATS.MATURITY.GROWING);
			wildGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.00041666668f, CREATURES.STATS.MATURITY.GROWINGWILD);
			getOldRate = new AttributeModifier(master.oldAge.deltaAttribute.Id, master.shouldGrowOld ? 1f : 0f);
		}

		public bool IsGrown()
		{
			return base.master.IsGrown();
		}

		public bool ReachedNextHarvest()
		{
			return base.master.ReachedNextHarvest();
		}

		public void ClampGrowthToHarvest()
		{
			base.master.ClampGrowthToHarvest();
		}

		public bool IsWilting()
		{
			if (base.master.wiltCondition != null)
			{
				return base.master.wiltCondition.IsWilting();
			}
			return false;
		}

		public bool IsSleeping()
		{
			return base.master.GetSMI<CropSleepingMonitor.Instance>()?.IsSleeping() ?? false;
		}

		public bool CanExitStalled()
		{
			if (!IsWilting())
			{
				return !IsSleeping();
			}
			return false;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Growing>
	{
		public class GrowingStates : State
		{
			public State wild;

			public State planted;
		}

		public class GrownStates : State
		{
			public State idle;

			public State try_self_harvest;
		}

		public GrowingStates growing;

		public State stalled;

		public GrownStates grown;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = growing;
			base.serializable = true;
			growing.EventTransition(GameHashes.Wilt, stalled, (StatesInstance smi) => smi.IsWilting()).EventTransition(GameHashes.CropSleep, stalled, (StatesInstance smi) => smi.IsSleeping()).EventTransition(GameHashes.PlanterStorage, growing.planted, (StatesInstance smi) => smi.master.rm.Replanted)
				.EventTransition(GameHashes.PlanterStorage, growing.wild, (StatesInstance smi) => !smi.master.rm.Replanted)
				.TriggerOnEnter(GameHashes.Grow)
				.Update("CheckGrown", delegate(StatesInstance smi, float dt)
				{
					if (smi.ReachedNextHarvest())
					{
						smi.GoTo(grown);
					}
				}, UpdateRate.SIM_4000ms)
				.ToggleStatusItem(Db.Get().CreatureStatusItems.Growing, (StatesInstance smi) => smi.master.GetComponent<Growing>())
				.Enter(delegate(StatesInstance smi)
				{
					State state = (smi.master.rm.Replanted ? growing.planted : growing.wild);
					smi.GoTo(state);
				});
			growing.wild.ToggleAttributeModifier("GrowingWild", (StatesInstance smi) => smi.wildGrowingRate);
			growing.planted.ToggleAttributeModifier("Growing", (StatesInstance smi) => smi.baseGrowingRate);
			stalled.EventTransition(GameHashes.WiltRecover, growing, (StatesInstance smi) => smi.CanExitStalled()).EventTransition(GameHashes.CropWakeUp, growing, (StatesInstance smi) => smi.CanExitStalled());
			grown.DefaultState(grown.idle).TriggerOnEnter(GameHashes.Grow).Update("CheckNotGrown", delegate(StatesInstance smi, float dt)
			{
				if (!smi.ReachedNextHarvest())
				{
					smi.GoTo(growing);
				}
			}, UpdateRate.SIM_4000ms)
				.ToggleAttributeModifier("GettingOld", (StatesInstance smi) => smi.getOldRate)
				.Enter(delegate(StatesInstance smi)
				{
					smi.ClampGrowthToHarvest();
				})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.oldAge.SetValue(0f);
				});
			grown.idle.Update("CheckNotGrown", delegate(StatesInstance smi, float dt)
			{
				if (smi.master.shouldGrowOld && smi.master.oldAge.value >= smi.master.oldAge.GetMax())
				{
					smi.GoTo(grown.try_self_harvest);
				}
			}, UpdateRate.SIM_4000ms);
			grown.try_self_harvest.Enter(delegate(StatesInstance smi)
			{
				Harvestable component = smi.master.GetComponent<Harvestable>();
				if ((bool)component && component.CanBeHarvested)
				{
					bool harvestWhenReady = component.harvestDesignatable.HarvestWhenReady;
					component.ForceCancelHarvest();
					component.Harvest();
					if (harvestWhenReady && component != null)
					{
						component.harvestDesignatable.SetHarvestWhenReady(state: true);
					}
				}
				smi.master.maturity.SetValue(0f);
				smi.master.oldAge.SetValue(0f);
			}).GoTo(grown.idle);
		}
	}

	public float growthTime;

	public bool shouldGrowOld = true;

	public float maxAge = 2400f;

	private AmountInstance maturity;

	private AmountInstance oldAge;

	private AttributeModifier baseMaturityMax;

	[MyCmpGet]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Modifiers modifiers;

	[MyCmpReq]
	private ReceptacleMonitor rm;

	private Crop _crop;

	private static readonly EventSystem.IntraObjectHandler<Growing> OnNewGameSpawnDelegate = new EventSystem.IntraObjectHandler<Growing>(delegate(Growing component, object data)
	{
		component.OnNewGameSpawn(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Growing> ResetGrowthDelegate = new EventSystem.IntraObjectHandler<Growing>(delegate(Growing component, object data)
	{
		component.ResetGrowth(data);
	});

	private Crop crop
	{
		get
		{
			if (_crop == null)
			{
				_crop = GetComponent<Crop>();
			}
			return _crop;
		}
	}

	protected override void OnPrefabInit()
	{
		Amounts amounts = base.gameObject.GetAmounts();
		maturity = amounts.Add(new AmountInstance(Db.Get().Amounts.Maturity, base.gameObject));
		baseMaturityMax = new AttributeModifier(maturity.maxAttribute.Id, growthTime / 600f);
		maturity.maxAttribute.Add(baseMaturityMax);
		oldAge = amounts.Add(new AmountInstance(Db.Get().Amounts.OldAge, base.gameObject));
		oldAge.maxAttribute.ClearModifiers();
		oldAge.maxAttribute.Add(new AttributeModifier(Db.Get().Amounts.OldAge.maxAttribute.Id, maxAge));
		base.OnPrefabInit();
		Subscribe(1119167081, OnNewGameSpawnDelegate);
		Subscribe(1272413801, ResetGrowthDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		base.gameObject.AddTag(GameTags.GrowingPlant);
	}

	private void OnNewGameSpawn(object data)
	{
		maturity.SetValue(maturity.maxAttribute.GetTotalValue() * Random.Range(0f, 1f));
	}

	public void OverrideMaturityLevel(float percent)
	{
		float value = maturity.GetMax() * percent;
		maturity.SetValue(value);
	}

	public bool ReachedNextHarvest()
	{
		return PercentOfCurrentHarvest() >= 1f;
	}

	public bool IsGrown()
	{
		return maturity.value == maturity.GetMax();
	}

	public bool CanGrow()
	{
		return !IsGrown();
	}

	public bool IsGrowing()
	{
		return maturity.GetDelta() > 0f;
	}

	public void ClampGrowthToHarvest()
	{
		maturity.value = maturity.GetMax();
	}

	public float PercentOfCurrentHarvest()
	{
		return maturity.value / maturity.GetMax();
	}

	public float TimeUntilNextHarvest()
	{
		return (maturity.GetMax() - maturity.value) / maturity.GetDelta();
	}

	public float DomesticGrowthTime()
	{
		return maturity.GetMax() / base.smi.baseGrowingRate.Value;
	}

	public float WildGrowthTime()
	{
		return maturity.GetMax() / base.smi.wildGrowingRate.Value;
	}

	public float PercentGrown()
	{
		return maturity.value / maturity.GetMax();
	}

	public void ResetGrowth(object data = null)
	{
		maturity.value = 0f;
	}

	public float PercentOldAge()
	{
		if (!shouldGrowOld)
		{
			return 0f;
		}
		return oldAge.value / oldAge.GetMax();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.GROWTHTIME_SIMPLE, GameUtil.GetFormattedCycles(growthTime, "")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.GROWTHTIME_SIMPLE, GameUtil.GetFormattedCycles(growthTime, "")), Descriptor.DescriptorType.Requirement)
		};
	}

	public void ConsumeMass(float mass_to_consume)
	{
		float value = maturity.value;
		mass_to_consume = Mathf.Min(mass_to_consume, value);
		maturity.value -= mass_to_consume;
		base.gameObject.Trigger(-1793167409);
	}
}
