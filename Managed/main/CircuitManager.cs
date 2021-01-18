using System.Collections.Generic;
using System.Collections.ObjectModel;
using STRINGS;
using UnityEngine;

public class CircuitManager
{
	private struct CircuitInfo
	{
		public List<Generator> generators;

		public List<IEnergyConsumer> consumers;

		public List<Battery> batteries;

		public List<Battery> inputTransformers;

		public List<Generator> outputTransformers;

		public List<WireUtilityNetworkLink>[] bridgeGroups;

		public float minBatteryPercentFull;

		public float wattsUsed;
	}

	public enum ConnectionStatus
	{
		NotConnected,
		Unpowered,
		Powered
	}

	public const ushort INVALID_ID = ushort.MaxValue;

	private const int SimUpdateSortKey = 1000;

	private const float MIN_POWERED_THRESHOLD = 0.01f;

	private bool dirty = true;

	private HashSet<Generator> generators = new HashSet<Generator>();

	private HashSet<IEnergyConsumer> consumers = new HashSet<IEnergyConsumer>();

	private HashSet<WireUtilityNetworkLink> bridges = new HashSet<WireUtilityNetworkLink>();

	private float elapsedTime;

	private List<CircuitInfo> circuitInfo = new List<CircuitInfo>();

	private List<IEnergyConsumer> consumersShadow = new List<IEnergyConsumer>();

	private List<Generator> activeGenerators = new List<Generator>();

	public void Connect(Generator generator)
	{
		if (!Game.IsQuitting())
		{
			generators.Add(generator);
			dirty = true;
		}
	}

	public void Disconnect(Generator generator)
	{
		if (!Game.IsQuitting())
		{
			generators.Remove(generator);
			dirty = true;
		}
	}

	public void Connect(IEnergyConsumer consumer)
	{
		if (!Game.IsQuitting())
		{
			consumers.Add(consumer);
			dirty = true;
		}
	}

	public void Disconnect(IEnergyConsumer consumer)
	{
		if (!Game.IsQuitting())
		{
			consumers.Remove(consumer);
			dirty = true;
		}
	}

	public void Connect(WireUtilityNetworkLink bridge)
	{
		bridges.Add(bridge);
		dirty = true;
	}

	public void Disconnect(WireUtilityNetworkLink bridge)
	{
		bridges.Remove(bridge);
		dirty = true;
	}

	public float GetPowerDraw(ushort circuitID, Generator generator)
	{
		if (circuitID < circuitInfo.Count)
		{
			CircuitInfo value = circuitInfo[circuitID];
			circuitInfo[circuitID] = value;
			circuitInfo[circuitID] = value;
		}
		return 0f;
	}

	public ushort GetCircuitID(int cell)
	{
		return (ushort)(Game.Instance.electricalConduitSystem.GetNetworkForCell(cell)?.id ?? 65535);
	}

	public void Sim200msFirst(float dt)
	{
		Refresh(dt);
	}

	public void RenderEveryTick(float dt)
	{
		Refresh(dt);
	}

	private void Refresh(float dt)
	{
		UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem = Game.Instance.electricalConduitSystem;
		if (!electricalConduitSystem.IsDirty && !dirty)
		{
			return;
		}
		electricalConduitSystem.Update();
		IList<UtilityNetwork> networks = electricalConduitSystem.GetNetworks();
		while (this.circuitInfo.Count < networks.Count)
		{
			CircuitInfo circuitInfo = default(CircuitInfo);
			circuitInfo.generators = new List<Generator>();
			circuitInfo.consumers = new List<IEnergyConsumer>();
			circuitInfo.batteries = new List<Battery>();
			circuitInfo.inputTransformers = new List<Battery>();
			circuitInfo.outputTransformers = new List<Generator>();
			CircuitInfo item = circuitInfo;
			item.bridgeGroups = new List<WireUtilityNetworkLink>[5];
			for (int i = 0; i < item.bridgeGroups.Length; i++)
			{
				item.bridgeGroups[i] = new List<WireUtilityNetworkLink>();
			}
			this.circuitInfo.Add(item);
		}
		Rebuild();
	}

	public void Rebuild()
	{
		for (int i = 0; i < circuitInfo.Count; i++)
		{
			CircuitInfo value = circuitInfo[i];
			value.generators.Clear();
			value.consumers.Clear();
			value.batteries.Clear();
			value.inputTransformers.Clear();
			value.outputTransformers.Clear();
			value.minBatteryPercentFull = 1f;
			for (int j = 0; j < value.bridgeGroups.Length; j++)
			{
				value.bridgeGroups[j].Clear();
			}
			circuitInfo[i] = value;
		}
		consumersShadow.AddRange(consumers);
		List<IEnergyConsumer>.Enumerator enumerator = consumersShadow.GetEnumerator();
		while (enumerator.MoveNext())
		{
			IEnergyConsumer current = enumerator.Current;
			int powerCell = current.PowerCell;
			ushort circuitID = GetCircuitID(powerCell);
			if (circuitID == ushort.MaxValue)
			{
				continue;
			}
			Battery battery = current as Battery;
			if (battery != null)
			{
				Operational component = battery.GetComponent<Operational>();
				if (component == null || component.IsOperational)
				{
					CircuitInfo value2 = circuitInfo[circuitID];
					if (battery.powerTransformer != null)
					{
						value2.inputTransformers.Add(battery);
					}
					else
					{
						value2.batteries.Add(battery);
						value2.minBatteryPercentFull = Mathf.Min(circuitInfo[circuitID].minBatteryPercentFull, battery.PercentFull);
					}
					circuitInfo[circuitID] = value2;
				}
			}
			else
			{
				circuitInfo[circuitID].consumers.Add(current);
			}
		}
		consumersShadow.Clear();
		for (int k = 0; k < circuitInfo.Count; k++)
		{
			circuitInfo[k].consumers.Sort((IEnergyConsumer a, IEnergyConsumer b) => a.WattsNeededWhenActive.CompareTo(b.WattsNeededWhenActive));
		}
		HashSet<Generator>.Enumerator enumerator2 = generators.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			Generator current2 = enumerator2.Current;
			int powerCell2 = current2.PowerCell;
			ushort circuitID2 = GetCircuitID(powerCell2);
			if (circuitID2 != ushort.MaxValue)
			{
				if (current2.GetType() == typeof(PowerTransformer))
				{
					circuitInfo[circuitID2].outputTransformers.Add(current2);
				}
				else
				{
					circuitInfo[circuitID2].generators.Add(current2);
				}
			}
		}
		HashSet<WireUtilityNetworkLink>.Enumerator enumerator3 = bridges.GetEnumerator();
		while (enumerator3.MoveNext())
		{
			WireUtilityNetworkLink current3 = enumerator3.Current;
			current3.GetCells(out var linked_cell, out var _);
			ushort circuitID3 = GetCircuitID(linked_cell);
			if (circuitID3 != ushort.MaxValue)
			{
				Wire.WattageRating maxWattageRating = current3.GetMaxWattageRating();
				circuitInfo[circuitID3].bridgeGroups[(int)maxWattageRating].Add(current3);
			}
		}
		dirty = false;
	}

	private float GetBatteryJoulesAvailable(List<Battery> batteries, out int num_powered)
	{
		float result = 0f;
		num_powered = 0;
		for (int i = 0; i < batteries.Count; i++)
		{
			if (batteries[i].JoulesAvailable > 0f)
			{
				result = batteries[i].JoulesAvailable;
				num_powered = batteries.Count - i;
				break;
			}
		}
		return result;
	}

	public void Sim200msLast(float dt)
	{
		elapsedTime += dt;
		if (elapsedTime < 0.2f)
		{
			return;
		}
		elapsedTime -= 0.2f;
		for (int i = 0; i < circuitInfo.Count; i++)
		{
			CircuitInfo value = circuitInfo[i];
			value.wattsUsed = 0f;
			activeGenerators.Clear();
			List<Generator> list = value.generators;
			List<IEnergyConsumer> list2 = value.consumers;
			List<Battery> batteries = value.batteries;
			List<Generator> outputTransformers = value.outputTransformers;
			batteries.Sort((Battery a, Battery b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
			bool flag = false;
			bool flag2 = list.Count > 0;
			for (int j = 0; j < list.Count; j++)
			{
				Generator generator = list[j];
				if (generator.JoulesAvailable > 0f)
				{
					flag = true;
					activeGenerators.Add(generator);
				}
			}
			activeGenerators.Sort((Generator a, Generator b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
			if (!flag)
			{
				for (int k = 0; k < outputTransformers.Count; k++)
				{
					if (outputTransformers[k].JoulesAvailable > 0f)
					{
						flag = true;
					}
				}
			}
			float num = 1f;
			for (int l = 0; l < batteries.Count; l++)
			{
				Battery battery = batteries[l];
				if (battery.JoulesAvailable > 0f)
				{
					flag = true;
				}
				num = Mathf.Min(num, battery.PercentFull);
			}
			for (int m = 0; m < value.inputTransformers.Count; m++)
			{
				Battery battery2 = value.inputTransformers[m];
				num = Mathf.Min(num, battery2.PercentFull);
			}
			value.minBatteryPercentFull = num;
			if (flag)
			{
				for (int n = 0; n < list2.Count; n++)
				{
					IEnergyConsumer energyConsumer = list2[n];
					float num2 = energyConsumer.WattsUsed * 0.2f;
					if (num2 > 0f)
					{
						bool flag3 = false;
						for (int num3 = 0; num3 < activeGenerators.Count; num3++)
						{
							Generator g = activeGenerators[num3];
							num2 = PowerFromGenerator(num2, g, energyConsumer);
							if (num2 <= 0f)
							{
								flag3 = true;
								break;
							}
						}
						if (!flag3)
						{
							for (int num4 = 0; num4 < outputTransformers.Count; num4++)
							{
								Generator g2 = outputTransformers[num4];
								num2 = PowerFromGenerator(num2, g2, energyConsumer);
								if (num2 <= 0f)
								{
									flag3 = true;
									break;
								}
							}
						}
						if (!flag3)
						{
							num2 = PowerFromBatteries(num2, batteries, energyConsumer);
							flag3 = num2 <= 0.01f;
						}
						if (flag3)
						{
							value.wattsUsed += energyConsumer.WattsUsed;
						}
						else
						{
							value.wattsUsed += energyConsumer.WattsUsed - num2 / 0.2f;
						}
						energyConsumer.SetConnectionStatus((!flag3) ? ConnectionStatus.Unpowered : ConnectionStatus.Powered);
					}
					else
					{
						energyConsumer.SetConnectionStatus((!flag) ? ConnectionStatus.Unpowered : ConnectionStatus.Powered);
					}
				}
			}
			else if (flag2)
			{
				for (int num5 = 0; num5 < list2.Count; num5++)
				{
					list2[num5].SetConnectionStatus(ConnectionStatus.Unpowered);
				}
			}
			else
			{
				for (int num6 = 0; num6 < list2.Count; num6++)
				{
					list2[num6].SetConnectionStatus(ConnectionStatus.NotConnected);
				}
			}
			circuitInfo[i] = value;
		}
		for (int num7 = 0; num7 < circuitInfo.Count; num7++)
		{
			CircuitInfo value2 = circuitInfo[num7];
			value2.batteries.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
			value2.inputTransformers.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
			value2.generators.Sort((Generator a, Generator b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
			float joules_used = 0f;
			ChargeTransformers(value2.inputTransformers, value2.generators, ref joules_used);
			ChargeTransformers(value2.inputTransformers, value2.outputTransformers, ref joules_used);
			float joules_used2 = 0f;
			ChargeBatteries(value2.batteries, value2.generators, ref joules_used2);
			ChargeBatteries(value2.batteries, value2.outputTransformers, ref joules_used2);
			value2.minBatteryPercentFull = 1f;
			for (int num8 = 0; num8 < value2.batteries.Count; num8++)
			{
				float percentFull = value2.batteries[num8].PercentFull;
				if (percentFull < value2.minBatteryPercentFull)
				{
					value2.minBatteryPercentFull = percentFull;
				}
			}
			for (int num9 = 0; num9 < value2.inputTransformers.Count; num9++)
			{
				float percentFull2 = value2.inputTransformers[num9].PercentFull;
				if (percentFull2 < value2.minBatteryPercentFull)
				{
					value2.minBatteryPercentFull = percentFull2;
				}
			}
			value2.wattsUsed += joules_used / 0.2f;
			circuitInfo[num7] = value2;
		}
		for (int num10 = 0; num10 < circuitInfo.Count; num10++)
		{
			CircuitInfo value3 = circuitInfo[num10];
			value3.batteries.Sort((Battery a, Battery b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
			float joules_used3 = 0f;
			ChargeTransformers(value3.inputTransformers, value3.batteries, ref joules_used3);
			value3.wattsUsed += joules_used3 / 0.2f;
			circuitInfo[num10] = value3;
		}
		for (int num11 = 0; num11 < circuitInfo.Count; num11++)
		{
			CircuitInfo value4 = circuitInfo[num11];
			bool is_connected_to_something_useful = value4.generators.Count + value4.consumers.Count + value4.outputTransformers.Count > 0;
			UpdateBatteryConnectionStatus(value4.batteries, is_connected_to_something_useful, num11);
			bool flag4 = value4.generators.Count > 0 || value4.outputTransformers.Count > 0;
			if (!flag4)
			{
				foreach (Battery battery3 in value4.batteries)
				{
					if (battery3.JoulesAvailable > 0f)
					{
						flag4 = true;
						break;
					}
				}
			}
			UpdateBatteryConnectionStatus(value4.inputTransformers, flag4, num11);
			circuitInfo[num11] = value4;
			for (int num12 = 0; num12 < value4.generators.Count; num12++)
			{
				Generator generator2 = value4.generators[num12];
				ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, 0f - generator2.JoulesAvailable, StringFormatter.Replace(BUILDINGS.PREFABS.GENERATOR.OVERPRODUCTION, "{Generator}", generator2.gameObject.GetProperName()));
			}
		}
		for (int num13 = 0; num13 < circuitInfo.Count; num13++)
		{
			CheckCircuitOverloaded(0.2f, num13, circuitInfo[num13].wattsUsed);
		}
	}

	private float PowerFromBatteries(float joules_needed, List<Battery> batteries, IEnergyConsumer c)
	{
		int num_powered;
		do
		{
			float num = GetBatteryJoulesAvailable(batteries, out num_powered) * (float)num_powered;
			float num2 = ((num < joules_needed) ? num : joules_needed);
			joules_needed -= num2;
			ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, 0f - num2, c.Name);
			float joules = num2 / (float)num_powered;
			for (int i = batteries.Count - num_powered; i < batteries.Count; i++)
			{
				batteries[i].ConsumeEnergy(joules);
			}
		}
		while (joules_needed >= 0.01f && num_powered > 0);
		return joules_needed;
	}

	private float PowerFromGenerator(float joules_needed, Generator g, IEnergyConsumer c)
	{
		float num = Mathf.Min(g.JoulesAvailable, joules_needed);
		joules_needed -= num;
		g.ApplyDeltaJoules(0f - num);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, 0f - num, c.Name);
		return joules_needed;
	}

	private void ChargeBatteries(List<Battery> sink_batteries, List<Generator> source_generators, ref float joules_used)
	{
		if (sink_batteries.Count == 0)
		{
			return;
		}
		foreach (Generator source_generator in source_generators)
		{
			for (bool flag = true; flag && source_generator.JoulesAvailable >= 1f; flag = ChargeBatteriesFromGenerator(sink_batteries, source_generator, ref joules_used))
			{
			}
		}
	}

	private bool ChargeBatteriesFromGenerator(List<Battery> sink_batteries, Generator source_generator, ref float joules_used)
	{
		float num = source_generator.JoulesAvailable;
		float num2 = 0f;
		for (int i = 0; i < sink_batteries.Count; i++)
		{
			Battery battery = sink_batteries[i];
			if (battery != null && source_generator != null && battery.gameObject != source_generator.gameObject)
			{
				float num3 = battery.Capacity - battery.JoulesAvailable;
				if (num3 > 0f)
				{
					float num4 = Mathf.Min(num3, num / (float)(sink_batteries.Count - i));
					battery.AddEnergy(num4);
					num -= num4;
					num2 += num4;
				}
			}
		}
		if (num2 > 0f)
		{
			source_generator.ApplyDeltaJoules(0f - num2);
			joules_used += num2;
			return true;
		}
		return false;
	}

	private void UpdateBatteryConnectionStatus(List<Battery> batteries, bool is_connected_to_something_useful, int circuit_id)
	{
		foreach (Battery battery in batteries)
		{
			if (!(battery == null))
			{
				if (battery.powerTransformer == null)
				{
					battery.SetConnectionStatus(is_connected_to_something_useful ? ConnectionStatus.Powered : ConnectionStatus.NotConnected);
				}
				else if (GetCircuitID(battery.PowerCell) == circuit_id)
				{
					battery.SetConnectionStatus((!is_connected_to_something_useful) ? ConnectionStatus.Unpowered : ConnectionStatus.Powered);
				}
			}
		}
	}

	private void ChargeTransformer<T>(Battery sink_transformer, List<T> source_energy_producers, ref float joules_used) where T : IEnergyProducer
	{
		if (source_energy_producers.Count <= 0)
		{
			return;
		}
		float num = Mathf.Min(sink_transformer.Capacity - sink_transformer.JoulesAvailable, sink_transformer.ChargeCapacity);
		if (num <= 0f)
		{
			return;
		}
		float num2 = num;
		float num3 = 0f;
		for (int i = 0; i < source_energy_producers.Count; i++)
		{
			T val = source_energy_producers[i];
			if (val.JoulesAvailable > 0f)
			{
				float num4 = Mathf.Min(val.JoulesAvailable, num2 / (float)(source_energy_producers.Count - i));
				val.ConsumeEnergy(num4);
				num2 -= num4;
				num3 += num4;
			}
		}
		sink_transformer.AddEnergy(num3);
		joules_used += num3;
	}

	private void ChargeTransformers<T>(List<Battery> sink_transformers, List<T> source_energy_producers, ref float joules_used) where T : IEnergyProducer
	{
		foreach (Battery sink_transformer in sink_transformers)
		{
			ChargeTransformer(sink_transformer, source_energy_producers, ref joules_used);
		}
	}

	private void CheckCircuitOverloaded(float dt, int id, float watts_used)
	{
		UtilityNetwork networkByID = Game.Instance.electricalConduitSystem.GetNetworkByID(id);
		if (networkByID != null)
		{
			((ElectricalUtilityNetwork)networkByID)?.UpdateOverloadTime(dt, watts_used, circuitInfo[id].bridgeGroups);
		}
	}

	public float GetWattsUsedByCircuit(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return -1f;
		}
		return circuitInfo[circuitID].wattsUsed;
	}

	public float GetWattsNeededWhenActive(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return -1f;
		}
		float num = 0f;
		foreach (IEnergyConsumer consumer in circuitInfo[circuitID].consumers)
		{
			num += consumer.WattsNeededWhenActive;
		}
		foreach (Battery inputTransformer in circuitInfo[circuitID].inputTransformers)
		{
			num += inputTransformer.WattsNeededWhenActive;
		}
		return num;
	}

	public float GetWattsGeneratedByCircuit(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return -1f;
		}
		float num = 0f;
		foreach (Generator generator in circuitInfo[circuitID].generators)
		{
			if (!(generator == null) && generator.GetComponent<Operational>().IsActive)
			{
				num += generator.WattageRating;
			}
		}
		return num;
	}

	public float GetPotentialWattsGeneratedByCircuit(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return -1f;
		}
		float num = 0f;
		foreach (Generator generator in circuitInfo[circuitID].generators)
		{
			num += generator.WattageRating;
		}
		return num;
	}

	public bool HasPowerSource(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return false;
		}
		List<Generator> list = circuitInfo[circuitID].generators;
		List<Battery> batteries = circuitInfo[circuitID].batteries;
		if (list.Count <= 0 || !(list.Find(FindActiveGenerator) != null))
		{
			if (batteries.Count > 0)
			{
				return batteries.Find(FindActiveBattery) != null;
			}
			return false;
		}
		return true;
	}

	private bool FindActiveGenerator(Generator g)
	{
		Operational component = g.GetComponent<Operational>();
		ManualGenerator component2 = g.GetComponent<ManualGenerator>();
		if (component2 == null)
		{
			return component.IsActive;
		}
		if (component.IsOperational)
		{
			return component2.IsPowered;
		}
		return false;
	}

	private bool FindActiveBattery(Battery b)
	{
		if (b.GetComponent<Operational>().IsOperational)
		{
			return b.PercentFull > 0f;
		}
		return false;
	}

	public float GetJoulesAvailableOnCircuit(ushort circuitID)
	{
		int num_powered;
		return GetBatteryJoulesAvailable(GetBatteriesOnCircuit(circuitID), out num_powered) * (float)num_powered;
	}

	public ReadOnlyCollection<Generator> GetGeneratorsOnCircuit(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return null;
		}
		return circuitInfo[circuitID].generators.AsReadOnly();
	}

	public ReadOnlyCollection<IEnergyConsumer> GetConsumersOnCircuit(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return null;
		}
		return circuitInfo[circuitID].consumers.AsReadOnly();
	}

	public ReadOnlyCollection<Battery> GetTransformersOnCircuit(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return null;
		}
		return circuitInfo[circuitID].inputTransformers.AsReadOnly();
	}

	public List<Battery> GetBatteriesOnCircuit(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return null;
		}
		return circuitInfo[circuitID].batteries;
	}

	public float GetMinBatteryPercentFullOnCircuit(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return 0f;
		}
		return circuitInfo[circuitID].minBatteryPercentFull;
	}

	public bool HasBatteries(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return false;
		}
		return circuitInfo[circuitID].batteries.Count + circuitInfo[circuitID].inputTransformers.Count > 0;
	}

	public bool HasGenerators(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return false;
		}
		return circuitInfo[circuitID].generators.Count + circuitInfo[circuitID].outputTransformers.Count > 0;
	}

	public bool HasGenerators()
	{
		return generators.Count > 0;
	}

	public bool HasConsumers(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return false;
		}
		return circuitInfo[circuitID].consumers.Count > 0;
	}

	public float GetMaxSafeWattageForCircuit(ushort circuitID)
	{
		if (circuitID == ushort.MaxValue)
		{
			return 0f;
		}
		return (Game.Instance.electricalConduitSystem.GetNetworkByID(circuitID) as ElectricalUtilityNetwork)?.GetMaxSafeWattage() ?? 0f;
	}
}
