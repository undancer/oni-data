using System;
using Klei;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;

public class ConduitDiseaseManager : KCompactedVector<ConduitDiseaseManager.Data>
{
	public struct Data
	{
		public byte diseaseIdx;

		public byte elemIdx;

		public int diseaseCount;

		public float accumulatedError;

		public float mass;

		public HandleVector<int>.Handle temperatureHandle;

		public ElemGrowthInfo growthInfo;

		public Data(HandleVector<int>.Handle temperature_handle, byte elem_idx, float mass, byte disease_idx, int disease_count)
		{
			diseaseIdx = disease_idx;
			elemIdx = elem_idx;
			this.mass = mass;
			diseaseCount = disease_count;
			accumulatedError = 0f;
			temperatureHandle = temperature_handle;
			growthInfo = GetGrowthInfo(disease_idx, elem_idx);
		}
	}

	private ConduitTemperatureManager temperatureManager;

	private static ElemGrowthInfo GetGrowthInfo(byte disease_idx, byte elem_idx)
	{
		if (disease_idx != byte.MaxValue)
		{
			return Db.Get().Diseases[disease_idx].elemGrowthInfo[elem_idx];
		}
		return Disease.DEFAULT_GROWTH_INFO;
	}

	public ConduitDiseaseManager(ConduitTemperatureManager temperature_manager)
		: base(0)
	{
		temperatureManager = temperature_manager;
	}

	public HandleVector<int>.Handle Allocate(HandleVector<int>.Handle temperature_handle, ref ConduitFlow.ConduitContents contents)
	{
		byte elem_idx = (byte)ElementLoader.GetElementIndex(contents.element);
		Data initial_data = new Data(temperature_handle, elem_idx, contents.mass, contents.diseaseIdx, contents.diseaseCount);
		return Allocate(initial_data);
	}

	public void SetData(HandleVector<int>.Handle handle, ref ConduitFlow.ConduitContents contents)
	{
		Data new_data = GetData(handle);
		new_data.diseaseCount = contents.diseaseCount;
		if (contents.diseaseIdx != new_data.diseaseIdx)
		{
			new_data.diseaseIdx = contents.diseaseIdx;
			byte elem_idx = (byte)ElementLoader.GetElementIndex(contents.element);
			new_data.growthInfo = GetGrowthInfo(contents.diseaseIdx, elem_idx);
		}
		SetData(handle, new_data);
	}

	public void Sim200ms(float dt)
	{
		using (new KProfiler.Region("ConduitDiseaseManager.SimUpdate"))
		{
			for (int i = 0; i < data.Count; i++)
			{
				Data value = data[i];
				if (value.diseaseIdx != byte.MaxValue)
				{
					float accumulatedError = value.accumulatedError;
					accumulatedError += value.growthInfo.CalculateDiseaseCountDelta(value.diseaseCount, value.mass, dt);
					Disease disease = Db.Get().Diseases[value.diseaseIdx];
					float num = Disease.HalfLifeToGrowthRate(Disease.CalculateRangeHalfLife(temperatureManager.GetTemperature(value.temperatureHandle), ref disease.temperatureRange, ref disease.temperatureHalfLives), dt);
					accumulatedError += (float)value.diseaseCount * num - (float)value.diseaseCount;
					int num2 = (int)accumulatedError;
					value.accumulatedError = accumulatedError - (float)num2;
					value.diseaseCount += num2;
					if (value.diseaseCount <= 0)
					{
						value.diseaseCount = 0;
						value.diseaseIdx = byte.MaxValue;
						value.accumulatedError = 0f;
					}
					data[i] = value;
				}
			}
		}
	}

	public void ModifyDiseaseCount(HandleVector<int>.Handle h, int disease_count_delta)
	{
		Data new_data = GetData(h);
		new_data.diseaseCount = Math.Max(0, new_data.diseaseCount + disease_count_delta);
		if (new_data.diseaseCount == 0)
		{
			new_data.diseaseIdx = byte.MaxValue;
		}
		SetData(h, new_data);
	}

	public void AddDisease(HandleVector<int>.Handle h, byte disease_idx, int disease_count)
	{
		Data new_data = GetData(h);
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, disease_count, new_data.diseaseIdx, new_data.diseaseCount);
		new_data.diseaseIdx = diseaseInfo.idx;
		new_data.diseaseCount = diseaseInfo.count;
		SetData(h, new_data);
	}
}
