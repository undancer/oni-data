using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/AirConditioner")]
public class AirConditioner : KMonoBehaviour, ISaveLoadable, IGameObjectEffectDescriptor, ISim200ms
{
	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	protected Storage storage;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpReq]
	private ConduitConsumer consumer;

	[MyCmpReq]
	private BuildingComplete building;

	[MyCmpGet]
	private OccupyArea occupyArea;

	private HandleVector<int>.Handle structureTemperature;

	public float temperatureDelta = -14f;

	public float maxEnvironmentDelta = -50f;

	private float lowTempLag;

	private bool showingLowTemp = false;

	public bool isLiquidConditioner;

	private bool showingHotEnv = false;

	private Guid statusHandle;

	[Serialize]
	private float targetTemperature;

	private int cooledAirOutputCell = -1;

	private static readonly EventSystem.IntraObjectHandler<AirConditioner> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<AirConditioner>(delegate(AirConditioner component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<AirConditioner> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<AirConditioner>(delegate(AirConditioner component, object data)
	{
		component.OnActiveChanged(data);
	});

	private float lastSampleTime = -1f;

	private float envTemp;

	private int cellCount;

	private static readonly Func<int, object, bool> UpdateStateCbDelegate = (int cell, object data) => UpdateStateCb(cell, data);

	public float lastEnvTemp
	{
		get;
		private set;
	}

	public float lastGasTemp
	{
		get;
		private set;
	}

	public float TargetTemperature => targetTemperature;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-592767678, OnOperationalChangedDelegate);
		Subscribe(824508782, OnActiveChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("InsulationTutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Insulation);
		});
		structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		cooledAirOutputCell = building.GetUtilityOutputCell();
	}

	public void Sim200ms(float dt)
	{
		if (operational != null && !operational.IsOperational)
		{
			operational.SetActive(value: false);
		}
		else
		{
			UpdateState(dt);
		}
	}

	private static bool UpdateStateCb(int cell, object data)
	{
		AirConditioner airConditioner = data as AirConditioner;
		airConditioner.cellCount++;
		airConditioner.envTemp += Grid.Temperature[cell];
		return true;
	}

	private void UpdateState(float dt)
	{
		bool value = consumer.IsSatisfied;
		envTemp = 0f;
		cellCount = 0;
		if (occupyArea != null && base.gameObject != null)
		{
			occupyArea.TestArea(Grid.PosToCell(base.gameObject), this, UpdateStateCbDelegate);
			envTemp /= cellCount;
		}
		lastEnvTemp = envTemp;
		List<GameObject> items = storage.items;
		for (int i = 0; i < items.Count; i++)
		{
			PrimaryElement component = items[i].GetComponent<PrimaryElement>();
			if (component.Mass > 0f && (!isLiquidConditioner || !component.Element.IsGas) && (isLiquidConditioner || !component.Element.IsLiquid))
			{
				value = true;
				lastGasTemp = component.Temperature;
				float num = component.Temperature + temperatureDelta;
				if (num < 1f)
				{
					num = 1f;
					lowTempLag = Mathf.Min(lowTempLag + dt / 5f, 1f);
				}
				else
				{
					lowTempLag = Mathf.Min(lowTempLag - dt / 5f, 0f);
				}
				ConduitFlow conduitFlow = (isLiquidConditioner ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow);
				float num2 = conduitFlow.AddElement(cooledAirOutputCell, component.ElementID, component.Mass, num, component.DiseaseIdx, component.DiseaseCount);
				component.KeepZeroMassObject = true;
				float num3 = num2 / component.Mass;
				int num4 = (int)((float)component.DiseaseCount * num3);
				component.Mass -= num2;
				component.ModifyDiseaseCount(-num4, "AirConditioner.UpdateState");
				float num5 = num - component.Temperature;
				float num6 = num5 * component.Element.specificHeatCapacity * num2;
				float display_dt = ((lastSampleTime > 0f) ? (Time.time - lastSampleTime) : 1f);
				lastSampleTime = Time.time;
				GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, 0f - num6, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, display_dt);
				break;
			}
		}
		if (Time.time - lastSampleTime > 2f)
		{
			GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, 0f, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, Time.time - lastSampleTime);
			lastSampleTime = Time.time;
		}
		operational.SetActive(value);
		UpdateStatus();
	}

	private void OnOperationalChanged(object data)
	{
		if (operational.IsOperational)
		{
			UpdateState(0f);
		}
	}

	private void OnActiveChanged(object data)
	{
		UpdateStatus();
	}

	private void UpdateStatus()
	{
		if (operational.IsActive)
		{
			if (lowTempLag >= 1f && !showingLowTemp)
			{
				statusHandle = (isLiquidConditioner ? selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CoolingStalledColdLiquid, this) : selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CoolingStalledColdGas, this));
				showingLowTemp = true;
				showingHotEnv = false;
			}
			else if (lowTempLag <= 0f && (showingHotEnv || showingLowTemp))
			{
				statusHandle = selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Cooling);
				showingLowTemp = false;
				showingHotEnv = false;
			}
			else if (statusHandle == Guid.Empty)
			{
				statusHandle = selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Cooling);
				showingLowTemp = false;
				showingHotEnv = false;
			}
		}
		else
		{
			statusHandle = selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, null);
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		string formattedTemperature = GameUtil.GetFormattedTemperature(temperatureDelta, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative);
		Element element = ElementLoader.FindElementByName(isLiquidConditioner ? "Water" : "Oxygen");
		float num = ((!isLiquidConditioner) ? Mathf.Abs(temperatureDelta * element.specificHeatCapacity * 1000f) : Mathf.Abs(temperatureDelta * element.specificHeatCapacity * 10000f));
		float dtu = num * 1f;
		Descriptor item = default(Descriptor);
		string txt = string.Format(isLiquidConditioner ? UI.BUILDINGEFFECTS.HEATGENERATED_LIQUIDCONDITIONER : UI.BUILDINGEFFECTS.HEATGENERATED_AIRCONDITIONER, GameUtil.GetFormattedHeatEnergy(dtu), GameUtil.GetFormattedTemperature(Mathf.Abs(temperatureDelta), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative));
		string tooltip = string.Format(isLiquidConditioner ? UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED_LIQUIDCONDITIONER : UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED_AIRCONDITIONER, GameUtil.GetFormattedHeatEnergy(dtu), GameUtil.GetFormattedTemperature(Mathf.Abs(temperatureDelta), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative));
		item.SetupDescriptor(txt, tooltip);
		list.Add(item);
		Descriptor item2 = default(Descriptor);
		item2.SetupDescriptor(string.Format(isLiquidConditioner ? UI.BUILDINGEFFECTS.LIQUIDCOOLING : UI.BUILDINGEFFECTS.GASCOOLING, formattedTemperature), string.Format(isLiquidConditioner ? UI.BUILDINGEFFECTS.TOOLTIPS.LIQUIDCOOLING : UI.BUILDINGEFFECTS.TOOLTIPS.GASCOOLING, formattedTemperature));
		list.Add(item2);
		return list;
	}
}
