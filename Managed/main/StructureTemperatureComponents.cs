using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class StructureTemperatureComponents : KGameObjectSplitComponentManager<StructureTemperatureHeader, StructureTemperaturePayload>
{
	private const float MAX_PRESSURE = 1.5f;

	private static Dictionary<int, HandleVector<int>.Handle> handleInstanceMap = new Dictionary<int, HandleVector<int>.Handle>();

	private StatusItem operatingEnergyStatusItem;

	public HandleVector<int>.Handle Add(GameObject go)
	{
		StructureTemperaturePayload payload = new StructureTemperaturePayload(go);
		return Add(go, new StructureTemperatureHeader
		{
			dirty = false,
			simHandle = -1,
			isActiveBuilding = false
		}, ref payload);
	}

	public static void ClearInstanceMap()
	{
		handleInstanceMap.Clear();
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle handle)
	{
		InitializeStatusItem();
		base.OnPrefabInit(handle);
		GetData(handle, out var header, out var payload);
		payload.primaryElement.getTemperatureCallback = OnGetTemperature;
		payload.primaryElement.setTemperatureCallback = OnSetTemperature;
		header.isActiveBuilding = payload.building.Def.SelfHeatKilowattsWhenActive != 0f || payload.ExhaustKilowatts != 0f;
		SetHeader(handle, header);
	}

	private void InitializeStatusItem()
	{
		if (operatingEnergyStatusItem != null)
		{
			return;
		}
		operatingEnergyStatusItem = new StatusItem("OperatingEnergy", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
		operatingEnergyStatusItem.resolveStringCallback = delegate(string str, object ev_data)
		{
			int key = (int)ev_data;
			HandleVector<int>.Handle handle = handleInstanceMap[key];
			StructureTemperaturePayload payload = GetPayload(handle);
			if (str != (string)BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP)
			{
				try
				{
					str = string.Format(str, GameUtil.GetFormattedHeatEnergy(payload.TotalEnergyProducedKW * 1000f));
					return str;
				}
				catch (Exception obj)
				{
					Debug.LogWarning(obj);
					Debug.LogWarning(BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP);
					Debug.LogWarning(str);
					return str;
				}
			}
			string text = "";
			foreach (StructureTemperaturePayload.EnergySource item in payload.energySourcesKW)
			{
				text += string.Format(BUILDING.STATUSITEMS.OPERATINGENERGY.LINEITEM, item.source, GameUtil.GetFormattedHeatEnergy(item.value * 1000f, GameUtil.HeatEnergyFormatterUnit.DTU_S));
			}
			str = string.Format(str, GameUtil.GetFormattedHeatEnergy(payload.TotalEnergyProducedKW * 1000f, GameUtil.HeatEnergyFormatterUnit.DTU_S), text);
			return str;
		};
	}

	protected override void OnSpawn(HandleVector<int>.Handle handle)
	{
		GetData(handle, out var header, out var payload);
		if (payload.operational != null && header.isActiveBuilding)
		{
			payload.primaryElement.Subscribe(824508782, delegate
			{
				OnActiveChanged(handle);
			});
		}
		payload.maxTemperature = ((payload.overheatable != null) ? payload.overheatable.OverheatTemperature : 10000f);
		if (payload.maxTemperature <= 0f)
		{
			Debug.LogError("invalid max temperature");
		}
		SetPayload(handle, ref payload);
		SimRegister(handle, ref header, ref payload);
	}

	private static void OnActiveChanged(HandleVector<int>.Handle handle)
	{
		GameComps.StructureTemperatures.GetData(handle, out var header, out var payload);
		payload.primaryElement.InternalTemperature = payload.Temperature;
		header.dirty = true;
		GameComps.StructureTemperatures.SetHeader(handle, header);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle handle)
	{
		SimUnregister(handle);
		base.OnCleanUp(handle);
	}

	public override void Sim200ms(float dt)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		GetDataLists(out var headers, out var payloads);
		ListPool<int, StructureTemperatureComponents>.PooledList pooledList = ListPool<int, StructureTemperatureComponents>.Allocate();
		pooledList.Capacity = Math.Max(pooledList.Capacity, headers.Count);
		ListPool<int, StructureTemperatureComponents>.PooledList pooledList2 = ListPool<int, StructureTemperatureComponents>.Allocate();
		pooledList2.Capacity = Math.Max(pooledList2.Capacity, headers.Count);
		ListPool<int, StructureTemperatureComponents>.PooledList pooledList3 = ListPool<int, StructureTemperatureComponents>.Allocate();
		pooledList3.Capacity = Math.Max(pooledList3.Capacity, headers.Count);
		for (int i = 0; i != headers.Count; i++)
		{
			StructureTemperatureHeader value = headers[i];
			if (Sim.IsValidHandle(value.simHandle))
			{
				pooledList.Add(i);
				if (value.dirty)
				{
					pooledList2.Add(i);
					value.dirty = false;
					headers[i] = value;
				}
				if (value.isActiveBuilding)
				{
					pooledList3.Add(i);
				}
			}
		}
		foreach (int item in pooledList2)
		{
			StructureTemperaturePayload payload = payloads[item];
			UpdateSimState(ref payload);
		}
		foreach (int item2 in pooledList2)
		{
			if (payloads[item2].pendingEnergyModifications != 0f)
			{
				StructureTemperaturePayload value2 = payloads[item2];
				SimMessages.ModifyBuildingEnergy(value2.simHandleCopy, value2.pendingEnergyModifications, 0f, 10000f);
				value2.pendingEnergyModifications = 0f;
				payloads[item2] = value2;
			}
		}
		foreach (int item3 in pooledList3)
		{
			StructureTemperaturePayload value3 = payloads[item3];
			if (value3.operational == null || value3.operational.IsActive)
			{
				num++;
				if (!value3.isActiveStatusItemSet)
				{
					num3++;
					value3.primaryElement.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, operatingEnergyStatusItem, value3.simHandleCopy);
					value3.isActiveStatusItemSet = true;
				}
				value3.energySourcesKW = AccumulateProducedEnergyKW(value3.energySourcesKW, value3.OperatingKilowatts, BUILDING.STATUSITEMS.OPERATINGENERGY.OPERATING);
				if (value3.ExhaustKilowatts != 0f)
				{
					num2++;
					Extents extents = value3.GetExtents();
					int num4 = extents.width * extents.height;
					float num5 = value3.ExhaustKilowatts * dt / (float)num4;
					for (int j = 0; j < extents.height; j++)
					{
						int num6 = extents.y + j;
						for (int k = 0; k < extents.width; k++)
						{
							int num7 = extents.x + k;
							int num8 = num6 * Grid.WidthInCells + num7;
							float num9 = Mathf.Min(Grid.Mass[num8], 1.5f) / 1.5f;
							float kilojoules = num5 * num9;
							SimMessages.ModifyEnergy(num8, kilojoules, value3.maxTemperature, SimMessages.EnergySourceID.StructureTemperature);
						}
					}
					value3.energySourcesKW = AccumulateProducedEnergyKW(value3.energySourcesKW, value3.ExhaustKilowatts, BUILDING.STATUSITEMS.OPERATINGENERGY.EXHAUSTING);
				}
			}
			else if (value3.isActiveStatusItemSet)
			{
				num3++;
				value3.primaryElement.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, null);
				value3.isActiveStatusItemSet = false;
			}
			payloads[item3] = value3;
		}
		pooledList3.Recycle();
		pooledList2.Recycle();
		pooledList.Recycle();
	}

	private static void UpdateSimState(ref StructureTemperaturePayload payload)
	{
		DebugUtil.Assert(Sim.IsValidHandle(payload.simHandleCopy));
		float internalTemperature = payload.primaryElement.InternalTemperature;
		BuildingDef def = payload.building.Def;
		float mass = def.MassForTemperatureModification;
		float operatingKilowatts = payload.OperatingKilowatts;
		float overheat_temperature = ((payload.overheatable != null) ? payload.overheatable.OverheatTemperature : 10000f);
		if (!payload.enabled || payload.bypass)
		{
			mass = 0f;
		}
		Extents extents = payload.GetExtents();
		byte idx = payload.primaryElement.Element.idx;
		SimMessages.ModifyBuildingHeatExchange(payload.simHandleCopy, extents, mass, internalTemperature, def.ThermalConductivity, overheat_temperature, operatingKilowatts, idx);
	}

	private unsafe static float OnGetTemperature(PrimaryElement primary_element)
	{
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(primary_element.gameObject);
		StructureTemperaturePayload payload = GameComps.StructureTemperatures.GetPayload(handle);
		if (Sim.IsValidHandle(payload.simHandleCopy) && payload.enabled)
		{
			if (!payload.bypass)
			{
				int handleIndex = Sim.GetHandleIndex(payload.simHandleCopy);
				return Game.Instance.simData.buildingTemperatures[handleIndex].temperature;
			}
			int i = Grid.PosToCell(payload.primaryElement.transform.GetPosition());
			return Grid.Temperature[i];
		}
		return payload.primaryElement.InternalTemperature;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(primary_element.gameObject);
		GameComps.StructureTemperatures.GetData(handle, out var header, out var payload);
		payload.primaryElement.InternalTemperature = temperature;
		header.dirty = true;
		GameComps.StructureTemperatures.SetHeader(handle, header);
		if (!header.isActiveBuilding && Sim.IsValidHandle(payload.simHandleCopy))
		{
			UpdateSimState(ref payload);
			if (payload.pendingEnergyModifications != 0f)
			{
				SimMessages.ModifyBuildingEnergy(payload.simHandleCopy, payload.pendingEnergyModifications, 0f, 10000f);
				payload.pendingEnergyModifications = 0f;
				GameComps.StructureTemperatures.SetPayload(handle, ref payload);
			}
		}
	}

	public void ProduceEnergy(HandleVector<int>.Handle handle, float delta_kilojoules, string source, float display_dt)
	{
		StructureTemperaturePayload new_data = GetPayload(handle);
		if (Sim.IsValidHandle(new_data.simHandleCopy))
		{
			SimMessages.ModifyBuildingEnergy(new_data.simHandleCopy, delta_kilojoules, 0f, 10000f);
		}
		else
		{
			new_data.pendingEnergyModifications += delta_kilojoules;
			StructureTemperatureHeader header = GetHeader(handle);
			header.dirty = true;
			SetHeader(handle, header);
		}
		new_data.energySourcesKW = AccumulateProducedEnergyKW(new_data.energySourcesKW, delta_kilojoules / display_dt, source);
		SetPayload(handle, ref new_data);
	}

	private List<StructureTemperaturePayload.EnergySource> AccumulateProducedEnergyKW(List<StructureTemperaturePayload.EnergySource> sources, float kw, string source)
	{
		if (sources == null)
		{
			sources = new List<StructureTemperaturePayload.EnergySource>();
		}
		bool flag = false;
		for (int i = 0; i < sources.Count; i++)
		{
			if (sources[i].source == source)
			{
				sources[i].Accumulate(kw);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			sources.Add(new StructureTemperaturePayload.EnergySource(kw, source));
		}
		return sources;
	}

	public static void DoStateTransition(int sim_handle)
	{
		HandleVector<int>.Handle value = HandleVector<int>.InvalidHandle;
		if (handleInstanceMap.TryGetValue(sim_handle, out value))
		{
			DoMelt(GameComps.StructureTemperatures.GetPayload(value).primaryElement);
		}
	}

	public static void DoMelt(PrimaryElement primary_element)
	{
		Element element = primary_element.Element;
		if (element.highTempTransitionTarget != SimHashes.Unobtanium)
		{
			SimMessages.AddRemoveSubstance(Grid.PosToCell(primary_element.transform.GetPosition()), element.highTempTransitionTarget, CellEventLogger.Instance.OreMelted, primary_element.Mass, primary_element.Element.highTemp, primary_element.DiseaseIdx, primary_element.DiseaseCount);
			Util.KDestroyGameObject(primary_element.gameObject);
		}
	}

	public static void DoOverheat(int sim_handle)
	{
		HandleVector<int>.Handle value = HandleVector<int>.InvalidHandle;
		if (handleInstanceMap.TryGetValue(sim_handle, out value))
		{
			GameComps.StructureTemperatures.GetPayload(value).primaryElement.gameObject.Trigger(1832602615);
		}
	}

	public static void DoNoLongerOverheated(int sim_handle)
	{
		HandleVector<int>.Handle value = HandleVector<int>.InvalidHandle;
		if (handleInstanceMap.TryGetValue(sim_handle, out value))
		{
			GameComps.StructureTemperatures.GetPayload(value).primaryElement.gameObject.Trigger(171119937);
		}
	}

	public bool IsEnabled(HandleVector<int>.Handle handle)
	{
		return GetPayload(handle).enabled;
	}

	private void Enable(HandleVector<int>.Handle handle, bool isEnabled)
	{
		GetData(handle, out var header, out var payload);
		header.dirty = true;
		payload.enabled = isEnabled;
		SetData(handle, header, ref payload);
	}

	public void Enable(HandleVector<int>.Handle handle)
	{
		Enable(handle, isEnabled: true);
	}

	public void Disable(HandleVector<int>.Handle handle)
	{
		Enable(handle, isEnabled: false);
	}

	public bool IsBypassed(HandleVector<int>.Handle handle)
	{
		return GetPayload(handle).bypass;
	}

	private void Bypass(HandleVector<int>.Handle handle, bool bypass)
	{
		GetData(handle, out var header, out var payload);
		header.dirty = true;
		payload.bypass = bypass;
		SetData(handle, header, ref payload);
	}

	public void Bypass(HandleVector<int>.Handle handle)
	{
		Bypass(handle, bypass: true);
	}

	public void UnBypass(HandleVector<int>.Handle handle)
	{
		Bypass(handle, bypass: false);
	}

	protected void SimRegister(HandleVector<int>.Handle handle, ref StructureTemperatureHeader header, ref StructureTemperaturePayload payload)
	{
		if (payload.simHandleCopy != -1)
		{
			return;
		}
		PrimaryElement primaryElement = payload.primaryElement;
		if (!(primaryElement.Mass <= 0f) && !primaryElement.Element.IsTemperatureInsulated)
		{
			payload.simHandleCopy = -2;
			string dbg_name = primaryElement.name;
			HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle2 = Game.Instance.simComponentCallbackManager.Add(delegate(int sim_handle, object callback_data)
			{
				OnSimRegistered(handle, sim_handle, dbg_name);
			}, null, "StructureTemperature.SimRegister");
			BuildingDef def = primaryElement.GetComponent<Building>().Def;
			float internalTemperature = primaryElement.InternalTemperature;
			float massForTemperatureModification = def.MassForTemperatureModification;
			float operatingKilowatts = payload.OperatingKilowatts;
			SimMessages.AddBuildingHeatExchange(payload.GetExtents(), elem_idx: (byte)ElementLoader.elements.IndexOf(primaryElement.Element), mass: massForTemperatureModification, temperature: internalTemperature, thermal_conductivity: def.ThermalConductivity, operating_kw: operatingKilowatts, callbackIdx: handle2.index);
			header.simHandle = payload.simHandleCopy;
			SetData(handle, header, ref payload);
		}
	}

	private static void OnSimRegistered(HandleVector<int>.Handle handle, int sim_handle, string dbg_name)
	{
		if (GameComps.StructureTemperatures.IsValid(handle) && GameComps.StructureTemperatures.IsVersionValid(handle))
		{
			GameComps.StructureTemperatures.GetData(handle, out var header, out var payload);
			if (payload.simHandleCopy == -2)
			{
				handleInstanceMap[sim_handle] = handle;
				header.simHandle = sim_handle;
				payload.simHandleCopy = sim_handle;
				GameComps.StructureTemperatures.SetData(handle, header, ref payload);
				payload.primaryElement.Trigger(-1555603773);
			}
			else
			{
				SimMessages.RemoveBuildingHeatExchange(sim_handle);
			}
		}
	}

	protected unsafe void SimUnregister(HandleVector<int>.Handle handle)
	{
		if (!GameComps.StructureTemperatures.IsVersionValid(handle))
		{
			KCrashReporter.Assert(condition: false, "Handle version mismatch in StructureTemperature.SimUnregister");
		}
		else
		{
			if (KMonoBehaviour.isLoadingScene)
			{
				return;
			}
			GameComps.StructureTemperatures.GetData(handle, out var header, out var payload);
			if (payload.simHandleCopy != -1)
			{
				if (Sim.IsValidHandle(payload.simHandleCopy))
				{
					int handleIndex = Sim.GetHandleIndex(payload.simHandleCopy);
					payload.primaryElement.InternalTemperature = Game.Instance.simData.buildingTemperatures[handleIndex].temperature;
					SimMessages.RemoveBuildingHeatExchange(payload.simHandleCopy);
					handleInstanceMap.Remove(payload.simHandleCopy);
				}
				payload.simHandleCopy = -1;
				header.simHandle = -1;
				SetData(handle, header, ref payload);
			}
		}
	}
}
