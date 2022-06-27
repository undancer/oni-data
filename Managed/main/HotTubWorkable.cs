using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/HotTubWorkable")]
public class HotTubWorkable : Workable, IWorkerPrioritizable
{
	public HotTub hotTub;

	private bool faceLeft;

	private HotTubWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		synchronizeAnims = false;
		showProgressBar = true;
		resetProgressOnStop = true;
		faceTargetWhenWorking = true;
		SetWorkTime(90f);
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		AnimInfo anim = base.GetAnim(worker);
		anim.smi = new HotTubWorkerStateMachine.StatesInstance(worker);
		return anim;
	}

	protected override void OnStartWork(Worker worker)
	{
		faceLeft = Random.value > 0.5f;
		worker.GetComponent<Effects>().Add("HotTubRelaxing", should_save: false);
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetComponent<Effects>().Remove("HotTubRelaxing");
	}

	public override Vector3 GetFacingTarget()
	{
		return base.transform.GetPosition() + (faceLeft ? Vector3.left : Vector3.right);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(hotTub.trackingEffect))
		{
			component.Add(hotTub.trackingEffect, should_save: true);
		}
		if (!string.IsNullOrEmpty(hotTub.specificEffect))
		{
			component.Add(hotTub.specificEffect, should_save: true);
		}
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = hotTub.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(hotTub.trackingEffect) && component.HasEffect(hotTub.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(hotTub.specificEffect) && component.HasEffect(hotTub.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
