using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class AirFilter : StateMachineComponent<AirFilter.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, AirFilter, object>.GameInstance
	{
		public StatesInstance(AirFilter smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, AirFilter>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State converting;
		}

		public ReadyStates hasFilter;

		public State waiting;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = waiting;
			waiting.EventTransition(GameHashes.OnStorageChange, hasFilter, (StatesInstance smi) => smi.master.HasFilter() && smi.master.operational.IsOperational).EventTransition(GameHashes.OperationalChanged, hasFilter, (StatesInstance smi) => smi.master.HasFilter() && smi.master.operational.IsOperational);
			hasFilter.EventTransition(GameHashes.OperationalChanged, waiting, (StatesInstance smi) => !smi.master.operational.IsOperational).Enter("EnableConsumption", delegate(StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(enabled: true);
			}).Exit("DisableConsumption", delegate(StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(enabled: false);
			})
				.DefaultState(hasFilter.idle);
			hasFilter.idle.EventTransition(GameHashes.OnStorageChange, hasFilter.converting, (StatesInstance smi) => smi.master.IsConvertable());
			hasFilter.converting.Enter("SetActive(true)", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			}).Exit("SetActive(false)", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: false);
			}).EventTransition(GameHashes.OnStorageChange, hasFilter.idle, (StatesInstance smi) => !smi.master.IsConvertable());
		}
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private ElementConverter elementConverter;

	[MyCmpGet]
	private ElementConsumer elementConsumer;

	public Tag filterTag;

	public bool HasFilter()
	{
		return elementConverter.HasEnoughMass(filterTag);
	}

	public bool IsConvertable()
	{
		return elementConverter.HasEnoughMassToStartConverting();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return null;
	}
}
