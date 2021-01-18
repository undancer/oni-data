using System.Collections.Generic;
using UnityEngine;

public struct StructureTemperaturePayload
{
	public class EnergySource
	{
		public string source;

		public RunningAverage kw_accumulator;

		public float value => kw_accumulator.AverageValue;

		public EnergySource(float kj, string source)
		{
			this.source = source;
			kw_accumulator = new RunningAverage(float.MinValue, float.MaxValue, Mathf.RoundToInt(186f));
		}

		public void Accumulate(float value)
		{
			kw_accumulator.AddSample(value);
		}
	}

	public int simHandleCopy;

	public bool enabled;

	public bool bypass;

	public bool isActiveStatusItemSet;

	public bool overrideExtents;

	private PrimaryElement primaryElementBacking;

	public Overheatable overheatable;

	public Building building;

	public Operational operational;

	public List<EnergySource> energySourcesKW;

	public float pendingEnergyModifications;

	public float maxTemperature;

	public Extents overriddenExtents;

	public PrimaryElement primaryElement
	{
		get
		{
			return primaryElementBacking;
		}
		set
		{
			if (primaryElementBacking != value)
			{
				primaryElementBacking = value;
				overheatable = primaryElementBacking.GetComponent<Overheatable>();
			}
		}
	}

	public float TotalEnergyProducedKW
	{
		get
		{
			if (energySourcesKW == null || energySourcesKW.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < energySourcesKW.Count; i++)
			{
				num += energySourcesKW[i].value;
			}
			return num;
		}
	}

	public float Temperature => primaryElement.Temperature;

	public float ExhaustKilowatts => building.Def.ExhaustKilowattsWhenActive;

	public float OperatingKilowatts => (operational != null && operational.IsActive) ? building.Def.SelfHeatKilowattsWhenActive : 0f;

	public StructureTemperaturePayload(GameObject go)
	{
		simHandleCopy = -1;
		enabled = true;
		bypass = false;
		overrideExtents = false;
		overriddenExtents = default(Extents);
		primaryElementBacking = go.GetComponent<PrimaryElement>();
		overheatable = ((primaryElementBacking != null) ? primaryElementBacking.GetComponent<Overheatable>() : null);
		building = go.GetComponent<Building>();
		operational = go.GetComponent<Operational>();
		pendingEnergyModifications = 0f;
		maxTemperature = 10000f;
		energySourcesKW = null;
		isActiveStatusItemSet = false;
	}

	public void OverrideExtents(Extents newExtents)
	{
		overrideExtents = true;
		overriddenExtents = newExtents;
	}

	public Extents GetExtents()
	{
		return overrideExtents ? overriddenExtents : building.GetExtents();
	}
}
