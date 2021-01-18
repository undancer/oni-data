using System;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

public class SteamTurbine : Generator
{
	public class States : GameStateMachine<States, Instance, SteamTurbine>
	{
		public class OperationalStates : State
		{
			public State idle;

			public State active;

			public State tooHot;
		}

		public State inoperational;

		public OperationalStates operational;

		private static readonly HashedString[] ACTIVE_ANIMS = new HashedString[2]
		{
			"working_pre",
			"working_loop"
		};

		private static readonly HashedString[] TOOHOT_ANIMS = new HashedString[1]
		{
			"working_pre"
		};

		public override void InitializeStates(out BaseState default_state)
		{
			InitializeStatusItems();
			default_state = operational;
			root.Update("UpdateBlocked", delegate(SteamTurbine.Instance smi, float dt)
			{
				smi.UpdateBlocked(dt);
			});
			inoperational.EventTransition(GameHashes.OperationalChanged, operational.active, (SteamTurbine.Instance smi) => smi.master.GetComponent<Operational>().IsOperational).QueueAnim("off");
			operational.DefaultState(operational.active).EventTransition(GameHashes.OperationalChanged, inoperational, (SteamTurbine.Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).Update("UpdateOperational", delegate(SteamTurbine.Instance smi, float dt)
			{
				smi.UpdateState(dt);
			})
				.Exit(delegate(SteamTurbine.Instance smi)
				{
					smi.DisableStatusItems();
				});
			operational.idle.QueueAnim("on");
			operational.active.Update("UpdateActive", delegate(SteamTurbine.Instance smi, float dt)
			{
				smi.master.Pump(dt);
			}).ToggleStatusItem((SteamTurbine.Instance smi) => activeStatusItem, (SteamTurbine.Instance smi) => smi.master).Enter(delegate(SteamTurbine.Instance smi)
			{
				smi.GetComponent<KAnimControllerBase>().Play(ACTIVE_ANIMS, KAnim.PlayMode.Loop);
				smi.GetComponent<Operational>().SetActive(value: true);
			})
				.Exit(delegate(SteamTurbine.Instance smi)
				{
					smi.master.GetComponent<Generator>().ResetJoules();
					smi.GetComponent<Operational>().SetActive(value: false);
				});
			operational.tooHot.Enter(delegate(SteamTurbine.Instance smi)
			{
				smi.GetComponent<KAnimControllerBase>().Play(TOOHOT_ANIMS, KAnim.PlayMode.Loop);
			});
		}
	}

	public class Instance : GameStateMachine<States, Instance, SteamTurbine, object>.GameInstance
	{
		public bool insufficientMass = false;

		public bool insufficientTemperature = false;

		public bool buildingTooHot = false;

		private Guid inputBlockedHandle = Guid.Empty;

		private Guid inputPartiallyBlockedHandle = Guid.Empty;

		private Guid insufficientMassHandle = Guid.Empty;

		private Guid insufficientTemperatureHandle = Guid.Empty;

		private Guid buildingTooHotHandle = Guid.Empty;

		private Guid activeWattageHandle = Guid.Empty;

		public Instance(SteamTurbine master)
			: base(master)
		{
		}

		public void UpdateBlocked(float dt)
		{
			base.master.BlockedInputs = 0;
			for (int i = 0; i < base.master.TotalInputs; i++)
			{
				int num = base.master.srcCells[i];
				Element element = Grid.Element[num];
				if (element.IsLiquid || element.IsSolid)
				{
					base.master.BlockedInputs++;
				}
			}
			KSelectable component = GetComponent<KSelectable>();
			inputBlockedHandle = UpdateStatusItem(inputBlockedStatusItem, base.master.BlockedInputs == base.master.TotalInputs, inputBlockedHandle, component);
			inputPartiallyBlockedHandle = UpdateStatusItem(inputPartiallyBlockedStatusItem, base.master.BlockedInputs > 0 && base.master.BlockedInputs < base.master.TotalInputs, inputPartiallyBlockedHandle, component);
		}

		public void UpdateState(float dt)
		{
			bool flag = CanSteamFlow(ref insufficientMass, ref insufficientTemperature);
			bool flag2 = IsTooHot(ref buildingTooHot);
			UpdateStatusItems();
			StateMachine.BaseState currentState = base.smi.GetCurrentState();
			if (flag2)
			{
				if (currentState != base.sm.operational.tooHot)
				{
					base.smi.GoTo(base.sm.operational.tooHot);
				}
			}
			else if (flag)
			{
				if (currentState != base.sm.operational.active)
				{
					base.smi.GoTo(base.sm.operational.active);
				}
			}
			else if (currentState != base.sm.operational.idle)
			{
				base.smi.GoTo(base.sm.operational.idle);
			}
		}

		private bool IsTooHot(ref bool building_too_hot)
		{
			building_too_hot = base.gameObject.GetComponent<PrimaryElement>().Temperature > base.smi.master.maxBuildingTemperature;
			return building_too_hot;
		}

		private bool CanSteamFlow(ref bool insufficient_mass, ref bool insufficient_temperature)
		{
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < base.master.srcCells.Length; i++)
			{
				int num3 = base.master.srcCells[i];
				float b = Grid.Mass[num3];
				if (Grid.Element[num3].id == base.master.srcElem)
				{
					num = Mathf.Max(num, b);
					float b2 = Grid.Temperature[num3];
					num2 = Mathf.Max(num2, b2);
				}
			}
			insufficient_mass = num < base.master.requiredMass;
			insufficient_temperature = num2 < base.master.minActiveTemperature;
			return !insufficient_mass && !insufficient_temperature;
		}

		public void UpdateStatusItems()
		{
			KSelectable component = GetComponent<KSelectable>();
			insufficientMassHandle = UpdateStatusItem(insufficientMassStatusItem, insufficientMass, insufficientMassHandle, component);
			insufficientTemperatureHandle = UpdateStatusItem(insufficientTemperatureStatusItem, insufficientTemperature, insufficientTemperatureHandle, component);
			buildingTooHotHandle = UpdateStatusItem(buildingTooHotItem, buildingTooHot, buildingTooHotHandle, component);
			StatusItem status_item = (base.master.operational.IsActive ? activeWattageStatusItem : Db.Get().BuildingStatusItems.GeneratorOffline);
			activeWattageHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, base.master);
		}

		private Guid UpdateStatusItem(StatusItem item, bool show, Guid current_handle, KSelectable ksel)
		{
			Guid result = current_handle;
			if (show != (current_handle != Guid.Empty))
			{
				result = ((!show) ? ksel.RemoveStatusItem(current_handle) : ksel.AddStatusItem(item, base.master));
			}
			return result;
		}

		public void DisableStatusItems()
		{
			KSelectable component = GetComponent<KSelectable>();
			component.RemoveStatusItem(buildingTooHotHandle);
			component.RemoveStatusItem(insufficientMassHandle);
			component.RemoveStatusItem(insufficientTemperatureHandle);
			component.RemoveStatusItem(activeWattageHandle);
		}
	}

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	public SimHashes srcElem;

	public SimHashes destElem;

	public float requiredMass = 0.001f;

	public float minActiveTemperature = 398.15f;

	public float idealSourceElementTemperature = 473.15f;

	public float maxBuildingTemperature = 373.15f;

	public float outputElementTemperature = 368.15f;

	public float minConvertMass;

	public float pumpKGRate;

	public float maxSelfHeat;

	public float wasteHeatToTurbinePercent;

	private static readonly HashedString TINT_SYMBOL = new HashedString("meter_fill");

	[Serialize]
	private float storedMass = 0f;

	[Serialize]
	private float storedTemperature = 0f;

	[Serialize]
	private byte diseaseIdx = byte.MaxValue;

	[Serialize]
	private int diseaseCount = 0;

	private static StatusItem inputBlockedStatusItem;

	private static StatusItem inputPartiallyBlockedStatusItem;

	private static StatusItem insufficientMassStatusItem;

	private static StatusItem insufficientTemperatureStatusItem;

	private static StatusItem activeWattageStatusItem;

	private static StatusItem buildingTooHotItem;

	private static StatusItem activeStatusItem;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)39;

	private MeterController meter;

	private HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.Handle simEmitCBHandle = HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.InvalidHandle;

	private Instance smi;

	private int[] srcCells;

	private Storage gasStorage;

	private Storage liquidStorage;

	private ElementConsumer consumer;

	private Guid statusHandle;

	private HandleVector<int>.Handle structureTemperature;

	private float lastSampleTime = -1f;

	public int BlockedInputs
	{
		get;
		private set;
	}

	public int TotalInputs => srcCells.Length;

	public float CurrentWattage => Game.Instance.accumulators.GetAverageRate(accumulator);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		accumulator = Game.Instance.accumulators.Add("Power", this);
		structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		simEmitCBHandle = Game.Instance.massEmitCallbackManager.Add(OnSimEmittedCallback, this, "SteamTurbineEmit");
		BuildingDef def = GetComponent<BuildingComplete>().Def;
		srcCells = new int[def.WidthInCells];
		int cell = Grid.PosToCell(this);
		for (int i = 0; i < def.WidthInCells; i++)
		{
			int x = i - (def.WidthInCells - 1) / 2;
			srcCells[i] = Grid.OffsetCell(cell, new CellOffset(x, -2));
		}
		smi = new Instance(this);
		smi.StartSM();
		CreateMeter();
	}

	private void CreateMeter()
	{
		meter = new MeterController(base.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_OL", "meter_frame", "meter_fill");
	}

	protected override void OnCleanUp()
	{
		if (smi != null)
		{
			smi.StopSM("cleanup");
		}
		Game.Instance.massEmitCallbackManager.Release(simEmitCBHandle, "SteamTurbine");
		simEmitCBHandle.Clear();
		base.OnCleanUp();
	}

	private void Pump(float dt)
	{
		float mass = pumpKGRate * dt / (float)srcCells.Length;
		int[] array = srcCells;
		foreach (int gameCell in array)
		{
			SimMessages.ConsumeMass(callbackIdx: Game.Instance.massConsumedCallbackManager.Add(OnSimConsumeCallback, this, "SteamTurbineConsume").index, gameCell: gameCell, element: srcElem, mass: mass, radius: 1);
		}
	}

	private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		((SteamTurbine)data).OnSimConsume(mass_cb_info);
	}

	private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
	{
		if (mass_cb_info.mass > 0f)
		{
			storedTemperature = SimUtil.CalculateFinalTemperature(storedMass, storedTemperature, mass_cb_info.mass, mass_cb_info.temperature);
			storedMass += mass_cb_info.mass;
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseIdx, diseaseCount, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount);
			diseaseIdx = diseaseInfo.idx;
			diseaseCount = diseaseInfo.count;
			if (storedMass > minConvertMass && simEmitCBHandle.IsValid())
			{
				Game.Instance.massEmitCallbackManager.GetItem(simEmitCBHandle);
				gasStorage.AddGasChunk(srcElem, storedMass, storedTemperature, diseaseIdx, diseaseCount, keep_zero_mass: true);
				storedMass = 0f;
				storedTemperature = 0f;
				diseaseIdx = byte.MaxValue;
				diseaseCount = 0;
			}
		}
	}

	private static void OnSimEmittedCallback(Sim.MassEmittedCallback info, object data)
	{
		((SteamTurbine)data).OnSimEmitted(info);
	}

	private void OnSimEmitted(Sim.MassEmittedCallback info)
	{
		if (info.suceeded != 1)
		{
			storedTemperature = SimUtil.CalculateFinalTemperature(storedMass, storedTemperature, info.mass, info.temperature);
			storedMass += info.mass;
			if (info.diseaseIdx != byte.MaxValue)
			{
				SimUtil.DiseaseInfo diseaseInfo = default(SimUtil.DiseaseInfo);
				diseaseInfo.idx = diseaseIdx;
				diseaseInfo.count = diseaseCount;
				SimUtil.DiseaseInfo a = diseaseInfo;
				diseaseInfo = default(SimUtil.DiseaseInfo);
				diseaseInfo.idx = info.diseaseIdx;
				diseaseInfo.count = info.diseaseCount;
				SimUtil.DiseaseInfo b = diseaseInfo;
				SimUtil.DiseaseInfo diseaseInfo2 = SimUtil.CalculateFinalDiseaseInfo(a, b);
				diseaseIdx = diseaseInfo2.idx;
				diseaseCount = diseaseInfo2.count;
			}
		}
	}

	public static void InitializeStatusItems()
	{
		activeStatusItem = new StatusItem("TURBINE_ACTIVE", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Good, allow_multiples: false, OverlayModes.None.ID);
		inputBlockedStatusItem = new StatusItem("TURBINE_BLOCKED_INPUT", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
		inputPartiallyBlockedStatusItem = new StatusItem("TURBINE_PARTIALLY_BLOCKED_INPUT", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
		inputPartiallyBlockedStatusItem.resolveStringCallback = ResolvePartialBlockedStatus;
		insufficientMassStatusItem = new StatusItem("TURBINE_INSUFFICIENT_MASS", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Power.ID);
		insufficientMassStatusItem.resolveStringCallback = ResolveStrings;
		buildingTooHotItem = new StatusItem("TURBINE_TOO_HOT", "BUILDING", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
		buildingTooHotItem.resolveTooltipCallback = ResolveStrings;
		insufficientTemperatureStatusItem = new StatusItem("TURBINE_INSUFFICIENT_TEMPERATURE", "BUILDING", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Power.ID);
		insufficientTemperatureStatusItem.resolveStringCallback = ResolveStrings;
		insufficientTemperatureStatusItem.resolveTooltipCallback = ResolveStrings;
		activeWattageStatusItem = new StatusItem("TURBINE_ACTIVE_WATTAGE", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
		activeWattageStatusItem.resolveStringCallback = ResolveWattageStatus;
	}

	private static string ResolveWattageStatus(string str, object data)
	{
		SteamTurbine steamTurbine = (SteamTurbine)data;
		float num = Game.Instance.accumulators.GetAverageRate(steamTurbine.accumulator) / steamTurbine.WattageRating;
		return str.Replace("{Wattage}", GameUtil.GetFormattedWattage(steamTurbine.CurrentWattage)).Replace("{Max_Wattage}", GameUtil.GetFormattedWattage(steamTurbine.WattageRating)).Replace("{Efficiency}", GameUtil.GetFormattedPercent(num * 100f))
			.Replace("{Src_Element}", ElementLoader.FindElementByHash(steamTurbine.srcElem).name);
	}

	private static string ResolvePartialBlockedStatus(string str, object data)
	{
		SteamTurbine steamTurbine = (SteamTurbine)data;
		return str.Replace("{Blocked}", steamTurbine.BlockedInputs.ToString()).Replace("{Total}", steamTurbine.TotalInputs.ToString());
	}

	private static string ResolveStrings(string str, object data)
	{
		SteamTurbine steamTurbine = (SteamTurbine)data;
		str = str.Replace("{Src_Element}", ElementLoader.FindElementByHash(steamTurbine.srcElem).name);
		str = str.Replace("{Dest_Element}", ElementLoader.FindElementByHash(steamTurbine.destElem).name);
		str = str.Replace("{Overheat_Temperature}", GameUtil.GetFormattedTemperature(steamTurbine.maxBuildingTemperature));
		str = str.Replace("{Active_Temperature}", GameUtil.GetFormattedTemperature(steamTurbine.minActiveTemperature));
		str = str.Replace("{Min_Mass}", GameUtil.GetFormattedMass(steamTurbine.requiredMass));
		return str;
	}

	public void SetStorage(Storage steamStorage, Storage waterStorage)
	{
		gasStorage = steamStorage;
		liquidStorage = waterStorage;
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		ushort circuitID = base.CircuitID;
		operational.SetFlag(Generator.wireConnectedFlag, circuitID != ushort.MaxValue);
		if (!operational.IsOperational)
		{
			return;
		}
		float value = 0f;
		if (gasStorage != null && gasStorage.items.Count > 0)
		{
			GameObject gameObject = gasStorage.FindFirst(ElementLoader.FindElementByHash(srcElem).tag);
			if (gameObject != null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				float num = 0.1f;
				if (component.Mass > num)
				{
					num = Mathf.Min(component.Mass, pumpKGRate * dt);
					float num2 = JoulesToGenerate(component);
					value = Mathf.Min(num2 * (num / pumpKGRate), base.WattageRating * dt);
					float num3 = HeatFromCoolingSteam(component);
					float num4 = num3 * (num / component.Mass);
					float num5 = num / component.Mass;
					int num6 = Mathf.RoundToInt((float)component.DiseaseCount * num5);
					component.Mass -= num;
					component.ModifyDiseaseCount(-num6, "SteamTurbine.EnergySim200ms");
					float display_dt = ((lastSampleTime > 0f) ? (Time.time - lastSampleTime) : 1f);
					lastSampleTime = Time.time;
					GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, num4 * wasteHeatToTurbinePercent, BUILDINGS.PREFABS.STEAMTURBINE2.HEAT_SOURCE, display_dt);
					liquidStorage.AddLiquid(destElem, num, outputElementTemperature, component.DiseaseIdx, num6, keep_zero_mass: true);
				}
			}
		}
		value = Mathf.Clamp(value, 0f, base.WattageRating);
		Game.Instance.accumulators.Accumulate(accumulator, value);
		if (value > 0f)
		{
			GenerateJoules(value);
		}
		meter.SetPositionPercent(Game.Instance.accumulators.GetAverageRate(accumulator) / base.WattageRating);
		meter.SetSymbolTint(TINT_SYMBOL, Color.Lerp(Color.red, Color.green, Game.Instance.accumulators.GetAverageRate(accumulator) / base.WattageRating));
	}

	public float HeatFromCoolingSteam(PrimaryElement steam)
	{
		float temperature = steam.Temperature;
		return 0f - GameUtil.CalculateEnergyDeltaForElement(steam, temperature, outputElementTemperature);
	}

	public float JoulesToGenerate(PrimaryElement steam)
	{
		float temperature = steam.Temperature;
		float num = (temperature - outputElementTemperature) / (idealSourceElementTemperature - outputElementTemperature);
		return base.WattageRating * (float)Math.Pow(num, 1.0);
	}
}
