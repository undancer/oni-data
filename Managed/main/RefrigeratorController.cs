using System.Collections.Generic;
using UnityEngine;

public class RefrigeratorController : GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>
{
	public class Def : BaseDef
	{
		public float activeCoolingStartBuffer = 2f;

		public float activeCoolingStopBuffer = 0.1f;

		public float simulatedInternalTemperature = 274.15f;

		public float simulatedInternalHeatCapacity = 400f;

		public float simulatedThermalConductivity = 1000f;

		public float powerSaverEnergyUsage;
	}

	public class OperationalStates : State
	{
		public State cooling;

		public State steady;
	}

	public class StatesInstance : GameInstance, IGameObjectEffectDescriptor
	{
		[MyCmpReq]
		public Operational operational;

		[MyCmpReq]
		public Storage storage;

		private SimulatedTemperatureAdjuster temperatureAdjuster;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			temperatureAdjuster = new SimulatedTemperatureAdjuster(def.simulatedInternalTemperature, def.simulatedInternalHeatCapacity, def.simulatedThermalConductivity, storage);
		}

		protected override void OnCleanUp()
		{
			temperatureAdjuster.CleanUp();
			base.OnCleanUp();
		}

		public float GetSaverPower()
		{
			return base.def.powerSaverEnergyUsage;
		}

		public float GetNormalPower()
		{
			return GetComponent<EnergyConsumer>().WattsNeededWhenActive;
		}

		public void SetEnergySaver(bool energySaving)
		{
			EnergyConsumer component = GetComponent<EnergyConsumer>();
			if (energySaving)
			{
				component.BaseWattageRating = GetSaverPower();
			}
			else
			{
				component.BaseWattageRating = GetNormalPower();
			}
		}

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return SimulatedTemperatureAdjuster.GetDescriptors(base.def.simulatedInternalTemperature);
		}
	}

	public State inoperational;

	public OperationalStates operational;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = inoperational;
		inoperational.EventTransition(GameHashes.OperationalChanged, operational, IsOperational);
		operational.DefaultState(operational.steady).EventTransition(GameHashes.OperationalChanged, inoperational, GameStateMachine<RefrigeratorController, StatesInstance, IStateMachineTarget, Def>.Not(IsOperational)).Enter(delegate(StatesInstance smi)
		{
			smi.operational.SetActive(value: true);
		})
			.Exit(delegate(StatesInstance smi)
			{
				smi.operational.SetActive(value: false);
			});
		operational.cooling.UpdateTransition(operational.steady, AllFoodCool, UpdateRate.SIM_4000ms, load_balance: true).ToggleStatusItem(Db.Get().BuildingStatusItems.FridgeCooling, (StatesInstance smi) => smi, Db.Get().StatusItemCategories.Main);
		operational.steady.UpdateTransition(operational.cooling, AnyWarmFood, UpdateRate.SIM_4000ms, load_balance: true).ToggleStatusItem(Db.Get().BuildingStatusItems.FridgeSteady, (StatesInstance smi) => smi, Db.Get().StatusItemCategories.Main).Enter(delegate(StatesInstance smi)
		{
			smi.SetEnergySaver(energySaving: true);
		})
			.Exit(delegate(StatesInstance smi)
			{
				smi.SetEnergySaver(energySaving: false);
			});
	}

	private bool AllFoodCool(StatesInstance smi, float dt)
	{
		foreach (GameObject item in smi.storage.items)
		{
			if (!(item == null))
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (!(component == null) && component.Temperature >= smi.def.simulatedInternalTemperature + smi.def.activeCoolingStopBuffer)
				{
					return false;
				}
			}
		}
		return true;
	}

	private bool AnyWarmFood(StatesInstance smi, float dt)
	{
		foreach (GameObject item in smi.storage.items)
		{
			if (!(item == null))
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (!(component == null) && component.Temperature >= smi.def.simulatedInternalTemperature + smi.def.activeCoolingStartBuffer)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsOperational(StatesInstance smi)
	{
		return smi.operational.IsOperational;
	}
}
