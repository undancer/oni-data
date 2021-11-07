using UnityEngine;

public class RadiationLight : StateMachineComponent<RadiationLight.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RadiationLight, object>.GameInstance
	{
		public StatesInstance(RadiationLight smi)
			: base(smi)
		{
			if (GetComponent<Rotatable>().IsRotated)
			{
				RadiationEmitter component = GetComponent<RadiationEmitter>();
				component.emitDirection = 180f;
				component.emissionOffset = Vector3.left;
			}
			ToggleEmitter(on: false);
			smi.meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target");
		}

		public void ToggleEmitter(bool on)
		{
			base.smi.master.operational.SetActive(on);
			base.smi.master.emitter.SetEmitting(on);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RadiationLight>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State on;
		}

		public State waiting;

		public ReadyStates ready;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = ready.idle;
			root.EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
			{
				smi.master.UpdateMeter();
			});
			waiting.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, ready.idle, (StatesInstance smi) => smi.master.operational.IsOperational);
			ready.EventTransition(GameHashes.OperationalChanged, waiting, (StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(ready.idle);
			ready.idle.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, ready.on, (StatesInstance smi) => smi.master.HasEnoughFuel());
			ready.on.PlayAnim("on").Enter(delegate(StatesInstance smi)
			{
				smi.ToggleEmitter(on: true);
			}).EventTransition(GameHashes.OnStorageChange, ready.idle, (StatesInstance smi) => !smi.master.HasEnoughFuel())
				.Exit(delegate(StatesInstance smi)
				{
					smi.ToggleEmitter(on: false);
				});
		}
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private RadiationEmitter emitter;

	[MyCmpGet]
	private ElementConverter elementConverter;

	private MeterController meter;

	public Tag elementToConsume;

	public float consumptionRate;

	public void UpdateMeter()
	{
		meter.SetPositionPercent(Mathf.Clamp01(storage.MassStored() / storage.capacityKg));
	}

	public bool HasEnoughFuel()
	{
		return elementConverter.HasEnoughMassToStartConverting();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		UpdateMeter();
	}
}
