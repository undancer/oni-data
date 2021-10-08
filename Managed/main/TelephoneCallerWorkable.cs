using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/TelephoneWorkable")]
public class TelephoneCallerWorkable : Workable, IWorkerPrioritizable
{
	[MyCmpReq]
	private Operational operational;

	public int basePriority;

	private Telephone telephone;

	private TelephoneCallerWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
		workingPstComplete = new HashedString[1] { "on_pst" };
		workAnims = new HashedString[6] { "on_pre", "on", "on_receiving", "on_pre_loop_receiving", "on_loop", "on_loop_pre" };
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_telephone_kanim") };
		showProgressBar = true;
		resetProgressOnStop = true;
		synchronizeAnims = true;
		SetWorkTime(40f);
		telephone = GetComponent<Telephone>();
	}

	protected override void OnStartWork(Worker worker)
	{
		operational.SetActive(value: true);
		telephone.isInUse = true;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (telephone.HasTag(GameTags.LongDistanceCall))
		{
			if (!string.IsNullOrEmpty(telephone.longDistanceEffect))
			{
				component.Add(telephone.longDistanceEffect, should_save: true);
			}
		}
		else if (telephone.wasAnswered)
		{
			if (!string.IsNullOrEmpty(telephone.chatEffect))
			{
				component.Add(telephone.chatEffect, should_save: true);
			}
		}
		else if (!string.IsNullOrEmpty(telephone.babbleEffect))
		{
			component.Add(telephone.babbleEffect, should_save: true);
		}
		if (!string.IsNullOrEmpty(telephone.trackingEffect))
		{
			component.Add(telephone.trackingEffect, should_save: true);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		operational.SetActive(value: false);
		telephone.HangUp();
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(telephone.trackingEffect) && component.HasEffect(telephone.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(telephone.chatEffect) && component.HasEffect(telephone.chatEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		if (!string.IsNullOrEmpty(telephone.babbleEffect) && component.HasEffect(telephone.babbleEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
