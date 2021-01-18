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
		public State waiting;

		public State hasfilter;

		public State converting;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = waiting;
			waiting.EventTransition(GameHashes.OnStorageChange, hasfilter, (StatesInstance smi) => smi.master.HasFilter() && smi.master.operational.IsOperational).EventTransition(GameHashes.OperationalChanged, hasfilter, (StatesInstance smi) => smi.master.HasFilter() && smi.master.operational.IsOperational);
			hasfilter.EventTransition(GameHashes.OnStorageChange, converting, (StatesInstance smi) => smi.master.IsConvertable()).EventTransition(GameHashes.OperationalChanged, waiting, (StatesInstance smi) => !smi.master.operational.IsOperational).Enter("EnableConsumption", delegate(StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(enabled: true);
			})
				.Exit("DisableConsumption", delegate(StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(enabled: false);
				});
			converting.Enter("SetActive(true)", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			}).Exit("SetActive(false)", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: false);
			}).Enter("EnableConsumption", delegate(StatesInstance smi)
			{
				smi.master.elementConsumer.EnableConsumption(enabled: true);
			})
				.Exit("DisableConsumption", delegate(StatesInstance smi)
				{
					smi.master.elementConsumer.EnableConsumption(enabled: false);
				})
				.EventTransition(GameHashes.OnStorageChange, waiting, (StatesInstance smi) => !smi.master.IsConvertable())
				.EventTransition(GameHashes.OperationalChanged, waiting, (StatesInstance smi) => !smi.master.operational.IsOperational);
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
