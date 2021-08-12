using KSerialization;
using UnityEngine;

public class HeatCompressor : StateMachineComponent<HeatCompressor.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, HeatCompressor, object>.GameInstance
	{
		[Serialize]
		public float heatRemovalTimer;

		public StatesInstance(HeatCompressor master)
			: base(master)
		{
		}

		public void UpdateMeter()
		{
			float remainingCharge = GetRemainingCharge();
			base.master.meter.SetPositionPercent(remainingCharge);
		}

		public float GetRemainingCharge()
		{
			PrimaryElement primaryElement = base.smi.master.heatCubeStorage.FindFirstWithMass(GameTags.IndustrialIngredient);
			float result = 1f;
			if (primaryElement != null)
			{
				result = Mathf.Clamp01(primaryElement.GetComponent<PrimaryElement>().Temperature / base.smi.master.MAX_CUBE_TEMPERATURE);
			}
			return result;
		}

		public bool CanWork()
		{
			if (GetRemainingCharge() < 1f)
			{
				return base.smi.master.heatCubeStorage.items.Count > 0;
			}
			return false;
		}

		public void StartNewHeatRemoval()
		{
			heatRemovalTimer = base.smi.master.heatRemovalTime;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, HeatCompressor>
	{
		public State active;

		public State inactive;

		public State dropCube;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = inactive;
			base.serializable = SerializeType.Both_DEPRECATED;
			root.EventTransition(GameHashes.OperationalChanged, inactive, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			inactive.Enter(delegate(StatesInstance smi)
			{
				smi.UpdateMeter();
			}).PlayAnim("idle").Transition(dropCube, (StatesInstance smi) => smi.GetRemainingCharge() >= 1f)
				.Transition(active, (StatesInstance smi) => smi.GetComponent<Operational>().IsOperational && smi.CanWork());
			active.Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: true);
				smi.StartNewHeatRemoval();
			}).PlayAnim("working_loop", KAnim.PlayMode.Loop).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.time_active += dt;
				smi.UpdateMeter();
				smi.master.CompressHeat(smi, dt);
			})
				.Transition(dropCube, (StatesInstance smi) => smi.GetRemainingCharge() >= 1f)
				.Transition(inactive, (StatesInstance smi) => !smi.CanWork())
				.Exit(delegate(StatesInstance smi)
				{
					smi.GetComponent<Operational>().SetActive(value: false);
				});
			dropCube.Enter(delegate(StatesInstance smi)
			{
				smi.master.EjectHeatCube();
				smi.GoTo(inactive);
			});
		}
	}

	[MyCmpReq]
	private Operational operational;

	private MeterController meter;

	public Storage inputStorage;

	public Storage outputStorage;

	public Storage heatCubeStorage;

	public float heatRemovalRate = 100f;

	public float heatRemovalTime = 100f;

	[Serialize]
	public float energyCompressed;

	public float heat_sink_active_time = 9000f;

	[Serialize]
	public float time_active;

	public float MAX_CUBE_TEMPERATURE = 3000f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		meter.gameObject.GetComponent<KBatchedAnimController>().SetDirty();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("HeatCube"), base.transform.GetPosition());
		gameObject.SetActive(value: true);
		heatCubeStorage.Store(gameObject, hide_popups: true);
		base.smi.StartSM();
	}

	public void SetStorage(Storage inputStorage, Storage outputStorage, Storage heatCubeStorage)
	{
		this.inputStorage = inputStorage;
		this.outputStorage = outputStorage;
		this.heatCubeStorage = heatCubeStorage;
	}

	public void CompressHeat(StatesInstance smi, float dt)
	{
		smi.heatRemovalTimer -= dt;
		float num = heatRemovalRate * dt / (float)inputStorage.items.Count;
		foreach (GameObject item in inputStorage.items)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			float lowTemp = component.Element.lowTemp;
			GameUtil.DeltaThermalEnergy(component, 0f - num, lowTemp);
			energyCompressed += num;
		}
		if (smi.heatRemovalTimer <= 0f)
		{
			for (int num2 = inputStorage.items.Count; num2 > 0; num2--)
			{
				GameObject gameObject = inputStorage.items[num2 - 1];
				if ((bool)gameObject)
				{
					inputStorage.Transfer(gameObject, outputStorage, block_events: false, hide_popups: true);
				}
			}
			smi.StartNewHeatRemoval();
		}
		foreach (GameObject item2 in heatCubeStorage.items)
		{
			GameUtil.DeltaThermalEnergy(item2.GetComponent<PrimaryElement>(), energyCompressed / (float)heatCubeStorage.items.Count, 100000f);
		}
		energyCompressed = 0f;
	}

	public void EjectHeatCube()
	{
		heatCubeStorage.DropAll(base.transform.GetPosition());
	}
}
