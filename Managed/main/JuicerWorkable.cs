using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/JuicerWorkable")]
public class JuicerWorkable : Workable, IWorkerPrioritizable
{
	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	private Juicer juicer;

	private JuicerWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_juicer_kanim")
		};
		showProgressBar = true;
		resetProgressOnStop = true;
		synchronizeAnims = false;
		SetWorkTime(30f);
		juicer = GetComponent<Juicer>();
	}

	protected override void OnStartWork(Worker worker)
	{
		operational.SetActive(value: true);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage component = GetComponent<Storage>();
		component.ConsumeAndGetDisease(GameTags.Water, juicer.waterMassPerUse, out var disease_info, out var aggregate_temperature);
		GermExposureMonitor.Instance sMI = worker.GetSMI<GermExposureMonitor.Instance>();
		for (int i = 0; i < juicer.ingredientTags.Length; i++)
		{
			component.ConsumeAndGetDisease(juicer.ingredientTags[i], juicer.ingredientMassesPerUse[i], out var disease_info2, out aggregate_temperature);
			sMI?.TryInjectDisease(disease_info2.idx, disease_info2.count, juicer.ingredientTags[i], Sickness.InfectionVector.Digestion);
		}
		sMI?.TryInjectDisease(disease_info.idx, disease_info.count, GameTags.Water, Sickness.InfectionVector.Digestion);
		Effects component2 = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(juicer.specificEffect))
		{
			component2.Add(juicer.specificEffect, should_save: true);
		}
		if (!string.IsNullOrEmpty(juicer.trackingEffect))
		{
			component2.Add(juicer.trackingEffect, should_save: true);
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
		if (!string.IsNullOrEmpty(juicer.trackingEffect) && component.HasEffect(juicer.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(juicer.specificEffect) && component.HasEffect(juicer.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
