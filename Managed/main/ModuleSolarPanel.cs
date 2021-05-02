using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ModuleSolarPanel : Generator
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, ModuleSolarPanel, object>.GameInstance
	{
		public StatesInstance(ModuleSolarPanel master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ModuleSolarPanel>
	{
		public State idle;

		public State launch;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.EventTransition(GameHashes.DoLaunchRocket, launch).DoNothing();
			launch.EventTransition(GameHashes.RocketLanded, idle).PlayAnim("launch", KAnim.PlayMode.Loop);
		}
	}

	private MeterController meter;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private StatesInstance smi;

	private Guid statusHandle;

	private CellOffset[] solarCellOffsets = new CellOffset[3]
	{
		new CellOffset(-1, 0),
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	private static readonly EventSystem.IntraObjectHandler<ModuleSolarPanel> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<ModuleSolarPanel>(delegate(ModuleSolarPanel component, object data)
	{
		component.OnActiveChanged(data);
	});

	public float CurrentWattage => Game.Instance.accumulators.GetAverageRate(accumulator);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.IsVirtual = true;
	}

	protected override void OnSpawn()
	{
		CraftModuleInterface craftModuleInterface = (CraftModuleInterface)(base.VirtualCircuitKey = GetComponent<RocketModuleCluster>().CraftInterface);
		base.OnSpawn();
		Subscribe(824508782, OnActiveChangedDelegate);
		smi = new StatesInstance(this);
		smi.StartSM();
		accumulator = Game.Instance.accumulators.Add("Element", this);
		BuildingDef def = GetComponent<BuildingComplete>().Def;
		int num = Grid.PosToCell(this);
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
	}

	protected override void OnCleanUp()
	{
		smi.StopSM("cleanup");
		Game.Instance.accumulators.Remove(accumulator);
		base.OnCleanUp();
	}

	protected void OnActiveChanged(object data)
	{
		StatusItem status_item = (((Operational)data).IsActive ? Db.Get().BuildingStatusItems.Wattage : Db.Get().BuildingStatusItems.GeneratorOffline);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, this);
	}

	private void UpdateStatusItem()
	{
		selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.Wattage);
		if (statusHandle == Guid.Empty)
		{
			statusHandle = selectable.AddStatusItem(Db.Get().BuildingStatusItems.ModuleSolarPanelWattage, this);
		}
		else if (statusHandle != Guid.Empty)
		{
			GetComponent<KSelectable>().ReplaceStatusItem(statusHandle, Db.Get().BuildingStatusItems.ModuleSolarPanelWattage, this);
		}
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		ushort circuitID = base.CircuitID;
		operational.SetFlag(Generator.wireConnectedFlag, value: true);
		operational.SetFlag(Generator.generatorConnectedFlag, value: true);
		if (!operational.IsOperational)
		{
			return;
		}
		float num = 0f;
		if (Grid.IsValidCell(Grid.PosToCell(this)) && Grid.WorldIdx[Grid.PosToCell(this)] != ClusterManager.INVALID_WORLD_IDX)
		{
			CellOffset[] array = solarCellOffsets;
			foreach (CellOffset offset in array)
			{
				int num2 = Grid.LightIntensity[Grid.OffsetCell(Grid.PosToCell(this), offset)];
				num += (float)num2 * 0.00053f;
			}
		}
		else
		{
			num = 60f;
		}
		num = Mathf.Clamp(num, 0f, 60f);
		operational.SetActive(num > 0f);
		Game.Instance.accumulators.Accumulate(accumulator, num * dt);
		if (num > 0f)
		{
			num *= dt;
			num = Mathf.Max(num, 1f * dt);
			GenerateJoules(num);
		}
		meter.SetPositionPercent(Game.Instance.accumulators.GetAverageRate(accumulator) / 60f);
		UpdateStatusItem();
	}
}
