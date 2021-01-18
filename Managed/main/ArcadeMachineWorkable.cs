using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ArcadeMachineWorkable")]
public class ArcadeMachineWorkable : Workable, IWorkerPrioritizable
{
	public ArcadeMachine owner;

	public int basePriority = RELAXATION.PRIORITY.TIER3;

	private static string specificEffect = "PlayedArcade";

	private static string trackingEffect = "RecentlyPlayedArcade";

	private ArcadeMachineWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		synchronizeAnims = false;
		showProgressBar = true;
		resetProgressOnStop = true;
		SetWorkTime(15f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		worker.GetComponent<Effects>().Add("ArcadePlaying", should_save: false);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		worker.GetComponent<Effects>().Remove("ArcadePlaying");
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(trackingEffect))
		{
			component.Add(trackingEffect, should_save: true);
		}
		if (!string.IsNullOrEmpty(specificEffect))
		{
			component.Add(specificEffect, should_save: true);
		}
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(trackingEffect) && component.HasEffect(trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(specificEffect) && component.HasEffect(specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
