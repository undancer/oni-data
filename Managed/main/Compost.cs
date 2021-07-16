using System.Collections.Generic;
using UnityEngine;

public class Compost : StateMachineComponent<Compost.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Compost, object>.GameInstance
	{
		public StatesInstance(Compost master)
			: base(master)
		{
		}

		public bool CanStartConverting()
		{
			return base.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting();
		}

		public bool CanContinueConverting()
		{
			return base.master.GetComponent<ElementConverter>().CanConvertAtAll();
		}

		public bool IsEmpty()
		{
			return base.master.storage.IsEmpty();
		}

		public void ResetWorkable()
		{
			CompostWorkable component = base.master.GetComponent<CompostWorkable>();
			component.ShowProgressBar(show: false);
			component.WorkTimeRemaining = component.GetWorkTime();
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Compost>
	{
		public State empty;

		public State insufficientMass;

		public State disabled;

		public State disabledEmpty;

		public State inert;

		public State composting;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = empty;
			base.serializable = SerializeType.Both_DEPRECATED;
			empty.Enter("empty", delegate(StatesInstance smi)
			{
				smi.ResetWorkable();
			}).EventTransition(GameHashes.OnStorageChange, insufficientMass, (StatesInstance smi) => !smi.IsEmpty()).EventTransition(GameHashes.OperationalChanged, disabledEmpty, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational)
				.ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingWaste)
				.PlayAnim("off");
			insufficientMass.Enter("empty", delegate(StatesInstance smi)
			{
				smi.ResetWorkable();
			}).EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.IsEmpty()).EventTransition(GameHashes.OnStorageChange, inert, (StatesInstance smi) => smi.CanStartConverting())
				.ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingWaste)
				.PlayAnim("idle_half");
			inert.EventTransition(GameHashes.OperationalChanged, disabled, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational).PlayAnim("on").ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingCompostFlip)
				.ToggleChore(CreateFlipChore, composting);
			composting.Enter("Composting", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			}).EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => !smi.CanContinueConverting()).EventTransition(GameHashes.OperationalChanged, disabled, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational)
				.ScheduleGoTo((StatesInstance smi) => smi.master.flipInterval, inert)
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(value: false);
				});
			disabled.Enter("disabledEmpty", delegate(StatesInstance smi)
			{
				smi.ResetWorkable();
			}).PlayAnim("on").EventTransition(GameHashes.OperationalChanged, inert, (StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			disabledEmpty.Enter("disabledEmpty", delegate(StatesInstance smi)
			{
				smi.ResetWorkable();
			}).PlayAnim("off").EventTransition(GameHashes.OperationalChanged, empty, (StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
		}

		private Chore CreateFlipChore(StatesInstance smi)
		{
			return new WorkChore<CompostWorkable>(Db.Get().ChoreTypes.FlipCompost, smi.master);
		}
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private Storage storage;

	[SerializeField]
	public float flipInterval = 600f;

	[SerializeField]
	public float simulatedInternalTemperature = 323.15f;

	[SerializeField]
	public float simulatedInternalHeatCapacity = 400f;

	[SerializeField]
	public float simulatedThermalConductivity = 1000f;

	private SimulatedTemperatureAdjuster temperatureAdjuster;

	private static readonly EventSystem.IntraObjectHandler<Compost> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<Compost>(delegate(Compost component, object data)
	{
		component.OnStorageChanged(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1697596308, OnStorageChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<ManualDeliveryKG>().ShowStatusItem = false;
		temperatureAdjuster = new SimulatedTemperatureAdjuster(simulatedInternalTemperature, simulatedInternalHeatCapacity, simulatedThermalConductivity, GetComponent<Storage>());
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		temperatureAdjuster.CleanUp();
	}

	private void OnStorageChanged(object data)
	{
		_ = (GameObject)data == null;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return SimulatedTemperatureAdjuster.GetDescriptors(simulatedInternalTemperature);
	}
}
