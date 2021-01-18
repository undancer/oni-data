using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/SocialGatheringPointWorkable")]
public class SocialGatheringPointWorkable : Workable, IWorkerPrioritizable
{
	private GameObject lastTalker;

	public int basePriority;

	public string specificEffect;

	private SocialGatheringPointWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_generic_convo_kanim")
		};
		workAnims = new HashedString[1]
		{
			"idle"
		};
		faceTargetWhenWorking = true;
		workerStatusItem = Db.Get().DuplicantStatusItems.Socializing;
		synchronizeAnims = false;
		showProgressBar = false;
		resetProgressOnStop = true;
		lightEfficiencyBonus = false;
	}

	public override Vector3 GetFacingTarget()
	{
		if (lastTalker != null)
		{
			return lastTalker.transform.GetPosition();
		}
		return base.GetFacingTarget();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (!worker.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Recreation))
		{
			Effects component = worker.GetComponent<Effects>();
			if (string.IsNullOrEmpty(specificEffect) || component.HasEffect(specificEffect))
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		worker.GetComponent<KPrefabID>().AddTag(GameTags.AlwaysConverse);
		worker.Subscribe(-594200555, OnStartedTalking);
		worker.Subscribe(25860745, OnStoppedTalking);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		worker.GetComponent<KPrefabID>().RemoveTag(GameTags.AlwaysConverse);
		worker.Unsubscribe(-594200555, OnStartedTalking);
		worker.Unsubscribe(25860745, OnStoppedTalking);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(specificEffect))
		{
			component.Add(specificEffect, should_save: true);
		}
	}

	private void OnStartedTalking(object data)
	{
		ConversationManager.StartedTalkingEvent startedTalkingEvent = (ConversationManager.StartedTalkingEvent)data;
		GameObject talker = startedTalkingEvent.talker;
		if (talker == base.worker.gameObject)
		{
			KBatchedAnimController component = base.worker.GetComponent<KBatchedAnimController>();
			string anim = startedTalkingEvent.anim;
			anim += Random.Range(1, 9);
			component.Play(anim);
			component.Queue("idle", KAnim.PlayMode.Loop);
		}
		else
		{
			base.worker.GetComponent<Facing>().Face(talker.transform.GetPosition());
			lastTalker = talker;
		}
	}

	private void OnStoppedTalking(object data)
	{
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		if (!string.IsNullOrEmpty(specificEffect) && worker.GetComponent<Effects>().HasEffect(specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
