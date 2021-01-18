using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/VerticalWindTunnelWorkable")]
public class VerticalWindTunnelWorkable : Workable, IWorkerPrioritizable
{
	public VerticalWindTunnel windTunnel;

	public HashedString overrideAnim;

	public string[] preAnims;

	public string loopAnim;

	public string[] pstAnims;

	private VerticalWindTunnelWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		AnimInfo anim = base.GetAnim(worker);
		anim.smi = new WindTunnelWorkerStateMachine.StatesInstance(worker, this);
		return anim;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		synchronizeAnims = false;
		showProgressBar = true;
		resetProgressOnStop = true;
		SetWorkTime(90f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		worker.GetComponent<Effects>().Add("VerticalWindTunnelFlying", should_save: false);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		worker.GetComponent<Effects>().Remove("VerticalWindTunnelFlying");
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		component.Add(windTunnel.trackingEffect, should_save: true);
		component.Add(windTunnel.specificEffect, should_save: true);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = windTunnel.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (component.HasEffect(windTunnel.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (component.HasEffect(windTunnel.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
