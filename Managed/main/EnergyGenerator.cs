using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EnergyGenerator : Generator, IGameObjectEffectDescriptor, ISingleSliderControl, ISliderControl
{
	[Serializable]
	[DebuggerDisplay("{tag} -{consumptionRate} kg/s")]
	public struct InputItem
	{
		public Tag tag;

		public float consumptionRate;

		public float maxStoredMass;

		public InputItem(Tag tag, float consumption_rate, float max_stored_mass)
		{
			this.tag = tag;
			consumptionRate = consumption_rate;
			maxStoredMass = max_stored_mass;
		}
	}

	[Serializable]
	[DebuggerDisplay("{element} {creationRate} kg/s")]
	public struct OutputItem
	{
		public SimHashes element;

		public float creationRate;

		public bool store;

		public CellOffset emitOffset;

		public float minTemperature;

		public OutputItem(SimHashes element, float creation_rate, bool store, float min_temperature = 0f)
			: this(element, creation_rate, store, CellOffset.none, min_temperature)
		{
		}

		public OutputItem(SimHashes element, float creation_rate, bool store, CellOffset emit_offset, float min_temperature = 0f)
		{
			this.element = element;
			creationRate = creation_rate;
			this.store = store;
			emitOffset = emit_offset;
			minTemperature = min_temperature;
		}
	}

	[Serializable]
	public struct Formula
	{
		public InputItem[] inputs;

		public OutputItem[] outputs;

		public Tag meterTag;
	}

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ManualDeliveryKG delivery;

	[SerializeField]
	[Serialize]
	private float batteryRefillPercent = 0.5f;

	public bool ignoreBatteryRefillPercent;

	public bool hasMeter = true;

	private static StatusItem batteriesSufficientlyFull;

	public Meter.Offset meterOffset;

	[SerializeField]
	public Formula formula;

	private MeterController meter;

	private static readonly EventSystem.IntraObjectHandler<EnergyGenerator> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<EnergyGenerator>(delegate(EnergyGenerator component, object data)
	{
		component.OnActiveChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<EnergyGenerator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<EnergyGenerator>(delegate(EnergyGenerator component, object data)
	{
		component.OnCopySettings(data);
	});

	public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.MANUALDELIVERYGENERATORSIDESCREEN.TITLE";

	public string SliderUnits => UI.UNITSUFFIXES.PERCENT;

	public static StatusItem BatteriesSufficientlyFull => batteriesSufficientlyFull;

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 100f;
	}

	public float GetSliderValue(int index)
	{
		return batteryRefillPercent * 100f;
	}

	public void SetSliderValue(float value, int index)
	{
		batteryRefillPercent = value / 100f;
	}

	string ISliderControl.GetSliderTooltip()
	{
		ManualDeliveryKG component = GetComponent<ManualDeliveryKG>();
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.MANUALDELIVERYGENERATORSIDESCREEN.TOOLTIP"), component.requestedItemTag.ProperName(), batteryRefillPercent * 100f);
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.MANUALDELIVERYGENERATORSIDESCREEN.TOOLTIP";
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		EnsureStatusItemAvailable();
		Subscribe(824508782, OnActiveChangedDelegate);
		if (!ignoreBatteryRefillPercent)
		{
			base.gameObject.AddOrGet<CopyBuildingSettings>();
			Subscribe(-905833192, OnCopySettingsDelegate);
		}
	}

	private void OnCopySettings(object data)
	{
		EnergyGenerator component = ((GameObject)data).GetComponent<EnergyGenerator>();
		if (component != null)
		{
			batteryRefillPercent = component.batteryRefillPercent;
		}
	}

	protected void OnActiveChanged(object data)
	{
		StatusItem status_item = (((Operational)data).IsActive ? Db.Get().BuildingStatusItems.Wattage : Db.Get().BuildingStatusItems.GeneratorOffline);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (hasMeter)
		{
			meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", meterOffset, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		}
	}

	private bool IsConvertible(float dt)
	{
		bool flag = true;
		InputItem[] inputs = formula.inputs;
		for (int i = 0; i < inputs.Length; i++)
		{
			InputItem inputItem = inputs[i];
			float massAvailable = storage.GetMassAvailable(inputItem.tag);
			float num = inputItem.consumptionRate * dt;
			flag = flag && massAvailable >= num;
			if (!flag)
			{
				break;
			}
		}
		return flag;
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		if (hasMeter)
		{
			InputItem inputItem = formula.inputs[0];
			float positionPercent = storage.GetMassAvailable(inputItem.tag) / inputItem.maxStoredMass;
			meter.SetPositionPercent(positionPercent);
		}
		ushort circuitID = base.CircuitID;
		operational.SetFlag(Generator.wireConnectedFlag, circuitID != ushort.MaxValue);
		bool value = false;
		if (operational.IsOperational)
		{
			bool flag = false;
			List<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID);
			if (!ignoreBatteryRefillPercent && batteriesOnCircuit.Count > 0)
			{
				foreach (Battery item in batteriesOnCircuit)
				{
					if (batteryRefillPercent <= 0f && item.PercentFull <= 0f)
					{
						flag = true;
						break;
					}
					if (item.PercentFull < batteryRefillPercent)
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (!ignoreBatteryRefillPercent)
			{
				selectable.ToggleStatusItem(batteriesSufficientlyFull, !flag);
			}
			if (delivery != null)
			{
				delivery.Pause(!flag, "Circuit has sufficient energy");
			}
			if (formula.inputs != null)
			{
				bool flag2 = IsConvertible(dt);
				selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedResourceMass, !flag2, formula);
				if (flag2)
				{
					InputItem[] inputs = formula.inputs;
					for (int i = 0; i < inputs.Length; i++)
					{
						InputItem inputItem2 = inputs[i];
						float amount = inputItem2.consumptionRate * dt;
						storage.ConsumeIgnoringDisease(inputItem2.tag, amount);
					}
					PrimaryElement component = GetComponent<PrimaryElement>();
					OutputItem[] outputs = formula.outputs;
					foreach (OutputItem output in outputs)
					{
						Emit(output, dt, component);
					}
					GenerateJoules(base.WattageRating * dt);
					selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, this);
					value = true;
				}
			}
		}
		operational.SetActive(value);
	}

	public List<Descriptor> RequirementDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		if (formula.inputs == null || formula.inputs.Length == 0)
		{
			return list;
		}
		for (int i = 0; i < formula.inputs.Length; i++)
		{
			InputItem inputItem = formula.inputs[i];
			string arg = inputItem.tag.ProperName();
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, arg, GameUtil.GetFormattedMass(inputItem.consumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, arg, GameUtil.GetFormattedMass(inputItem.consumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
			list.Add(item);
		}
		return list;
	}

	public List<Descriptor> EffectDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		if (formula.outputs == null || formula.outputs.Length == 0)
		{
			return list;
		}
		for (int i = 0; i < formula.outputs.Length; i++)
		{
			OutputItem outputItem = formula.outputs[i];
			string arg = ElementLoader.FindElementByHash(outputItem.element).tag.ProperName();
			Descriptor item = default(Descriptor);
			if (outputItem.minTemperature > 0f)
			{
				item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_MINORENTITYTEMP, arg, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond), GameUtil.GetFormattedTemperature(outputItem.minTemperature)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_MINORENTITYTEMP, arg, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond), GameUtil.GetFormattedTemperature(outputItem.minTemperature)));
			}
			else
			{
				item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_ENTITYTEMP, arg, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_ENTITYTEMP, arg, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond)));
			}
			list.Add(item);
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor item in RequirementDescriptors())
		{
			list.Add(item);
		}
		foreach (Descriptor item2 in EffectDescriptors())
		{
			list.Add(item2);
		}
		return list;
	}

	public static void EnsureStatusItemAvailable()
	{
		if (batteriesSufficientlyFull == null)
		{
			batteriesSufficientlyFull = new StatusItem("BatteriesSufficientlyFull", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
		}
	}

	public static Formula CreateSimpleFormula(Tag input_element, float input_mass_rate, float max_stored_input_mass, SimHashes output_element = SimHashes.Void, float output_mass_rate = 0f, bool store_output_mass = true, CellOffset output_offset = default(CellOffset), float min_output_temperature = 0f)
	{
		Formula result = default(Formula);
		result.inputs = new InputItem[1]
		{
			new InputItem(input_element, input_mass_rate, max_stored_input_mass)
		};
		if (output_element != SimHashes.Void)
		{
			result.outputs = new OutputItem[1]
			{
				new OutputItem(output_element, output_mass_rate, store_output_mass, output_offset, min_output_temperature)
			};
		}
		else
		{
			result.outputs = null;
		}
		return result;
	}

	private void Emit(OutputItem output, float dt, PrimaryElement root_pe)
	{
		Element element = ElementLoader.FindElementByHash(output.element);
		float num = output.creationRate * dt;
		if (output.store)
		{
			if (element.IsGas)
			{
				storage.AddGasChunk(output.element, num, root_pe.Temperature, byte.MaxValue, 0, keep_zero_mass: true);
				return;
			}
			if (element.IsLiquid)
			{
				storage.AddLiquid(output.element, num, root_pe.Temperature, byte.MaxValue, 0, keep_zero_mass: true);
				return;
			}
			GameObject go = element.substance.SpawnResource(base.transform.GetPosition(), num, root_pe.Temperature, byte.MaxValue, 0);
			storage.Store(go, hide_popups: true);
			return;
		}
		int num2 = Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), output.emitOffset);
		float temperature = Mathf.Max(root_pe.Temperature, output.minTemperature);
		if (element.IsGas)
		{
			SimMessages.ModifyMass(num2, num, byte.MaxValue, 0, CellEventLogger.Instance.EnergyGeneratorModifyMass, temperature, output.element);
		}
		else if (element.IsLiquid)
		{
			int elementIndex = ElementLoader.GetElementIndex(output.element);
			FallingWater.instance.AddParticle(num2, (byte)elementIndex, num, temperature, byte.MaxValue, 0, skip_sound: true);
		}
		else
		{
			element.substance.SpawnResource(Grid.CellToPosCCC(num2, Grid.SceneLayer.Front), num, temperature, byte.MaxValue, 0, prevent_merge: true);
		}
	}
}
