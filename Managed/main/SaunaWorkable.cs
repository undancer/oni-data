using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/SaunaWorkable")]
public class SaunaWorkable : Workable, IWorkerPrioritizable
{
	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	private Sauna sauna;

	private SaunaWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_sauna_kanim")
		};
		showProgressBar = true;
		resetProgressOnStop = true;
		synchronizeAnims = true;
		workLayer = Grid.SceneLayer.BuildingUse;
		SetWorkTime(30f);
		sauna = GetComponent<Sauna>();
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		operational.SetActive(value: true);
		worker.GetComponent<Effects>().Add("SaunaRelaxing", should_save: false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(sauna.specificEffect))
		{
			component.Add(sauna.specificEffect, should_save: true);
		}
		if (!string.IsNullOrEmpty(sauna.trackingEffect))
		{
			component.Add(sauna.trackingEffect, should_save: true);
		}
		operational.SetActive(value: false);
	}

	protected override void OnStopWork(Worker worker)
	{
		operational.SetActive(value: false);
		worker.GetComponent<Effects>().Remove("SaunaRelaxing");
		Storage component = GetComponent<Storage>();
		component.ConsumeAndGetDisease(SimHashes.Steam.CreateTag(), sauna.steamPerUseKG, out var disease_info, out var _);
		component.AddLiquid(SimHashes.Water, sauna.steamPerUseKG, sauna.waterOutputTemp, disease_info.idx, disease_info.count, keep_zero_mass: true, do_disease_transfer: false);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(sauna.trackingEffect) && component.HasEffect(sauna.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(sauna.specificEffect) && component.HasEffect(sauna.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
