using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/SodaFountainWorkable")]
public class SodaFountainWorkable : Workable, IWorkerPrioritizable
{
	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	private SodaFountain sodaFountain;

	private SodaFountainWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_sodamaker_kanim")
		};
		showProgressBar = true;
		resetProgressOnStop = true;
		synchronizeAnims = false;
		SetWorkTime(30f);
		sodaFountain = GetComponent<SodaFountain>();
	}

	protected override void OnStartWork(Worker worker)
	{
		operational.SetActive(value: true);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage component = GetComponent<Storage>();
		component.ConsumeAndGetDisease(GameTags.Water, sodaFountain.waterMassPerUse, out var amount_consumed, out var disease_info, out var aggregate_temperature);
		component.ConsumeAndGetDisease(sodaFountain.ingredientTag, sodaFountain.ingredientMassPerUse, out amount_consumed, out var disease_info2, out aggregate_temperature);
		GermExposureMonitor.Instance sMI = worker.GetSMI<GermExposureMonitor.Instance>();
		if (sMI != null)
		{
			sMI.TryInjectDisease(disease_info.idx, disease_info.count, GameTags.Water, Sickness.InfectionVector.Digestion);
			sMI.TryInjectDisease(disease_info2.idx, disease_info2.count, sodaFountain.ingredientTag, Sickness.InfectionVector.Digestion);
		}
		Effects component2 = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(sodaFountain.specificEffect))
		{
			component2.Add(sodaFountain.specificEffect, should_save: true);
		}
		if (!string.IsNullOrEmpty(sodaFountain.trackingEffect))
		{
			component2.Add(sodaFountain.trackingEffect, should_save: true);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		operational.SetActive(value: false);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(sodaFountain.trackingEffect) && component.HasEffect(sodaFountain.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(sodaFountain.specificEffect) && component.HasEffect(sodaFountain.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
