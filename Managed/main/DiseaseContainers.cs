using System;
using System.Collections.Generic;
using Database;
using Klei;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using UnityEngine;

public class DiseaseContainers : KGameObjectSplitComponentManager<DiseaseHeader, DiseaseContainer>
{
	public HandleVector<int>.Handle Add(GameObject go, byte disease_idx, int disease_count)
	{
		DiseaseHeader diseaseHeader = default(DiseaseHeader);
		diseaseHeader.diseaseIdx = disease_idx;
		diseaseHeader.diseaseCount = disease_count;
		diseaseHeader.primaryElement = go.GetComponent<PrimaryElement>();
		DiseaseHeader header = diseaseHeader;
		DiseaseContainer payload = new DiseaseContainer(go, header.primaryElement.Element.idx);
		if (disease_idx != byte.MaxValue)
		{
			EvaluateGrowthConstants(header, ref payload);
		}
		return Add(go, header, ref payload);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		AutoDisinfectable autoDisinfectable = GetPayload(h).autoDisinfectable;
		if (autoDisinfectable != null)
		{
			AutoDisinfectableManager.Instance.RemoveAutoDisinfectable(autoDisinfectable);
		}
		base.OnCleanUp(h);
	}

	public override void Sim200ms(float dt)
	{
		ListPool<int, DiseaseContainers>.PooledList pooledList = ListPool<int, DiseaseContainers>.Allocate();
		pooledList.Capacity = Math.Max(pooledList.Capacity, headers.Count);
		for (int i = 0; i < headers.Count; i++)
		{
			DiseaseHeader diseaseHeader = headers[i];
			if (diseaseHeader.diseaseIdx != byte.MaxValue && diseaseHeader.primaryElement != null)
			{
				pooledList.Add(i);
			}
		}
		bool radiation_enabled = Sim.IsRadiationEnabled();
		foreach (int item in pooledList)
		{
			DiseaseContainer container = payloads[item];
			DiseaseHeader diseaseHeader2 = headers[item];
			Disease disease = Db.Get().Diseases[diseaseHeader2.diseaseIdx];
			float num = CalculateDelta(diseaseHeader2, ref container, disease, dt, radiation_enabled);
			num += container.accumulatedError;
			int num2 = (int)num;
			container.accumulatedError = num - (float)num2;
			bool num3 = diseaseHeader2.diseaseCount > container.overpopulationCount;
			bool flag = diseaseHeader2.diseaseCount + num2 > container.overpopulationCount;
			if (num3 != flag)
			{
				EvaluateGrowthConstants(diseaseHeader2, ref container);
			}
			diseaseHeader2.diseaseCount += num2;
			if (diseaseHeader2.diseaseCount <= 0)
			{
				container.accumulatedError = 0f;
				diseaseHeader2.diseaseCount = 0;
				diseaseHeader2.diseaseIdx = byte.MaxValue;
			}
			headers[item] = diseaseHeader2;
			payloads[item] = container;
		}
		pooledList.Recycle();
	}

	private static float CalculateDelta(DiseaseHeader header, ref DiseaseContainer container, Disease disease, float dt, bool radiation_enabled)
	{
		return CalculateDelta(header.diseaseCount, container.elemIdx, header.primaryElement.Mass, Grid.PosToCell(header.primaryElement.transform.GetPosition()), header.primaryElement.Temperature, container.instanceGrowthRate, disease, dt, radiation_enabled);
	}

	public static float CalculateDelta(int disease_count, int element_idx, float mass, int environment_cell, float temperature, float tags_multiplier_base, Disease disease, float dt, bool radiation_enabled)
	{
		float num = 0f;
		ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[element_idx];
		num += elemGrowthInfo.CalculateDiseaseCountDelta(disease_count, mass, dt);
		float num2 = Disease.HalfLifeToGrowthRate(Disease.CalculateRangeHalfLife(temperature, ref disease.temperatureRange, ref disease.temperatureHalfLives), dt);
		num += (float)disease_count * num2 - (float)disease_count;
		float num3 = Mathf.Pow(tags_multiplier_base, dt);
		num += (float)disease_count * num3 - (float)disease_count;
		if (Grid.IsValidCell(environment_cell))
		{
			byte b = Grid.ElementIdx[environment_cell];
			ElemExposureInfo elemExposureInfo = disease.elemExposureInfo[b];
			num += elemExposureInfo.CalculateExposureDiseaseCountDelta(disease_count, dt);
			if (radiation_enabled)
			{
				float num4 = Grid.Radiation[environment_cell];
				if (num4 > 0f)
				{
					num -= num4 * disease.radiationKillRate;
				}
			}
		}
		return num;
	}

	public int ModifyDiseaseCount(HandleVector<int>.Handle h, int disease_count_delta)
	{
		DiseaseHeader header = GetHeader(h);
		header.diseaseCount = Math.Max(0, header.diseaseCount + disease_count_delta);
		if (header.diseaseCount == 0)
		{
			header.diseaseIdx = byte.MaxValue;
			DiseaseContainer new_data = GetPayload(h);
			new_data.accumulatedError = 0f;
			SetPayload(h, ref new_data);
		}
		SetHeader(h, header);
		return header.diseaseCount;
	}

	public int AddDisease(HandleVector<int>.Handle h, byte disease_idx, int disease_count)
	{
		GetData(h, out var header, out var payload);
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, disease_count, header.diseaseIdx, header.diseaseCount);
		bool num = header.diseaseIdx != diseaseInfo.idx;
		header.diseaseIdx = diseaseInfo.idx;
		header.diseaseCount = diseaseInfo.count;
		if (num && diseaseInfo.idx != byte.MaxValue)
		{
			EvaluateGrowthConstants(header, ref payload);
			SetData(h, header, ref payload);
		}
		else
		{
			SetHeader(h, header);
		}
		if (num)
		{
			header.primaryElement.Trigger(-283306403);
		}
		return header.diseaseCount;
	}

	private void GetVisualDiseaseIdxAndCount(DiseaseHeader header, ref DiseaseContainer payload, out int disease_idx, out int disease_count)
	{
		if (payload.visualDiseaseProvider == null)
		{
			disease_idx = header.diseaseIdx;
			disease_count = header.diseaseCount;
			return;
		}
		disease_idx = 255;
		disease_count = 0;
		HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(payload.visualDiseaseProvider);
		if (handle != HandleVector<int>.InvalidHandle)
		{
			DiseaseHeader header2 = GameComps.DiseaseContainers.GetHeader(handle);
			disease_idx = header2.diseaseIdx;
			disease_count = header2.diseaseCount;
		}
	}

	public void UpdateOverlayColours()
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Diseases diseases = Db.Get().Diseases;
		Color32 color = new Color32(0, 0, 0, byte.MaxValue);
		for (int i = 0; i < headers.Count; i++)
		{
			DiseaseContainer payload = payloads[i];
			DiseaseHeader header = headers[i];
			KBatchedAnimController controller = payload.controller;
			if (!(controller != null))
			{
				continue;
			}
			Color32 color2 = color;
			Vector3 position = controller.transform.GetPosition();
			if (visibleArea.Min <= position && position <= visibleArea.Max)
			{
				int num = 0;
				int disease_idx = 255;
				int disease_count = 0;
				GetVisualDiseaseIdxAndCount(header, ref payload, out disease_idx, out disease_count);
				if (disease_idx != 255)
				{
					color2 = GlobalAssets.Instance.colorSet.GetColorByName(diseases[disease_idx].overlayColourName);
					num = disease_count;
				}
				if (payload.isContainer)
				{
					List<GameObject> items = header.primaryElement.GetComponent<Storage>().items;
					for (int j = 0; j < items.Count; j++)
					{
						GameObject gameObject = items[j];
						if (!(gameObject != null))
						{
							continue;
						}
						HandleVector<int>.Handle handle = GetHandle(gameObject);
						if (handle.IsValid())
						{
							DiseaseHeader header2 = GetHeader(handle);
							if (header2.diseaseCount > num && header2.diseaseIdx != byte.MaxValue)
							{
								num = header2.diseaseCount;
								color2 = GlobalAssets.Instance.colorSet.GetColorByName(diseases[header2.diseaseIdx].overlayColourName);
							}
						}
					}
				}
				color2.a = SimUtil.DiseaseCountToAlpha254(num);
				if (payload.conduitType != 0)
				{
					ConduitFlow flowManager = Conduit.GetFlowManager(payload.conduitType);
					int cell = Grid.PosToCell(position);
					ConduitFlow.ConduitContents contents = flowManager.GetContents(cell);
					if (contents.diseaseIdx != byte.MaxValue && contents.diseaseCount > num)
					{
						num = contents.diseaseCount;
						color2 = GlobalAssets.Instance.colorSet.GetColorByName(diseases[contents.diseaseIdx].overlayColourName);
						color2.a = byte.MaxValue;
					}
				}
			}
			controller.OverlayColour = color2;
		}
	}

	private void EvaluateGrowthConstants(DiseaseHeader header, ref DiseaseContainer container)
	{
		Disease disease = Db.Get().Diseases[header.diseaseIdx];
		KPrefabID component = header.primaryElement.GetComponent<KPrefabID>();
		ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[header.diseaseIdx];
		container.overpopulationCount = (int)(elemGrowthInfo.maxCountPerKG * header.primaryElement.Mass);
		container.instanceGrowthRate = disease.GetGrowthRateForTags(component.Tags, header.diseaseCount > container.overpopulationCount);
	}

	public override void Clear()
	{
		base.Clear();
		for (int i = 0; i < payloads.Count; i++)
		{
			payloads[i].Clear();
		}
		headers.Clear();
		payloads.Clear();
		handles.Clear();
	}
}
