using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Electrolyzer : StateMachineComponent<Electrolyzer.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Electrolyzer, object>.GameInstance
	{
		public StatesInstance(Electrolyzer smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Electrolyzer>
	{
		public State disabled;

		public State waiting;

		public State converting;

		public State overpressure;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disabled;
			root.EventTransition(GameHashes.OperationalChanged, disabled, (StatesInstance smi) => !smi.master.operational.IsOperational).EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
			{
				smi.master.UpdateMeter();
			});
			disabled.EventTransition(GameHashes.OperationalChanged, waiting, (StatesInstance smi) => smi.master.operational.IsOperational);
			waiting.Enter("Waiting", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: false);
			}).EventTransition(GameHashes.OnStorageChange, converting, (StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			converting.Enter("Ready", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			}).Transition(waiting, (StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll()).Transition(overpressure, (StatesInstance smi) => !smi.master.RoomForPressure);
			overpressure.Enter("OverPressure", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: false);
			}).ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk).Transition(converting, (StatesInstance smi) => smi.master.RoomForPressure);
		}
	}

	[SerializeField]
	public float maxMass = 2.5f;

	[SerializeField]
	public bool hasMeter = true;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ElementConverter emitter;

	[MyCmpReq]
	private Operational operational;

	private MeterController meter;

	private bool RoomForPressure
	{
		get
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			cell = Grid.CellAbove(cell);
			return !GameUtil.FloodFillCheck(OverPressure, this, cell, 3, stop_at_solid: true, stop_at_liquid: true);
		}
	}

	protected override void OnSpawn()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if (hasMeter)
		{
			meter = new MeterController(component, "U2H_meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new Vector3(-0.4f, 0.5f, -0.1f), "U2H_meter_target", "U2H_meter_tank", "U2H_meter_waterbody", "U2H_meter_level");
		}
		base.smi.StartSM();
		UpdateMeter();
		Tutorial.Instance.oxygenGenerators.Add(base.gameObject);
	}

	protected override void OnCleanUp()
	{
		Tutorial.Instance.oxygenGenerators.Remove(base.gameObject);
		base.OnCleanUp();
	}

	public void UpdateMeter()
	{
		if (hasMeter)
		{
			float positionPercent = Mathf.Clamp01(storage.MassStored() / storage.capacityKg);
			meter.SetPositionPercent(positionPercent);
		}
	}

	private static bool OverPressure(int cell, Electrolyzer electrolyzer)
	{
		return Grid.Mass[cell] > electrolyzer.maxMass;
	}
}
