using Klei.AI;
using TUNING;
using UnityEngine;

public class PartyPointWorkable : Workable, IWorkerPrioritizable
{
	private enum ActivityType
	{
		Talk,
		Dance,
		LENGTH
	}

	private GameObject lastTalker;

	public int basePriority;

	public string specificEffect;

	public KAnimFile[][] workerOverrideAnims;

	private ActivityType activity;

	private PartyPointWorkable()
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
		workAnimPlayMode = KAnim.PlayMode.Loop;
		faceTargetWhenWorking = true;
		workerStatusItem = Db.Get().DuplicantStatusItems.Socializing;
		synchronizeAnims = false;
		showProgressBar = false;
		resetProgressOnStop = true;
		lightEfficiencyBonus = false;
		float num = Random.Range(0f, 100f);
		if (num > 80f)
		{
			activity = ActivityType.Dance;
		}
		else
		{
			activity = ActivityType.Talk;
		}
		switch (activity)
		{
		case ActivityType.Talk:
			workAnims = new HashedString[1]
			{
				"idle"
			};
			workerOverrideAnims = new KAnimFile[1][]
			{
				new KAnimFile[1]
				{
					Assets.GetAnim("anim_generic_convo_kanim")
				}
			};
			break;
		case ActivityType.Dance:
			workAnims = new HashedString[1]
			{
				"working_loop"
			};
			workerOverrideAnims = new KAnimFile[3][]
			{
				new KAnimFile[1]
				{
					Assets.GetAnim("anim_interacts_phonobox_danceone_kanim")
				},
				new KAnimFile[1]
				{
					Assets.GetAnim("anim_interacts_phonobox_dancetwo_kanim")
				},
				new KAnimFile[1]
				{
					Assets.GetAnim("anim_interacts_phonobox_dancethree_kanim")
				}
			};
			break;
		}
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		int num = Random.Range(0, workerOverrideAnims.Length);
		overrideAnims = workerOverrideAnims[num];
		return base.GetAnim(worker);
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
		return false;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.AddTag(GameTags.AlwaysConverse);
		worker.Subscribe(-594200555, OnStartedTalking);
		worker.Subscribe(25860745, OnStoppedTalking);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.RemoveTag(GameTags.AlwaysConverse);
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
			if (activity == ActivityType.Talk)
			{
				KBatchedAnimController component = base.worker.GetComponent<KBatchedAnimController>();
				string anim = startedTalkingEvent.anim;
				anim += Random.Range(1, 9);
				component.Play(anim);
				component.Queue("idle", KAnim.PlayMode.Loop);
			}
		}
		else
		{
			if (activity == ActivityType.Talk)
			{
				Facing component2 = base.worker.GetComponent<Facing>();
				component2.Face(talker.transform.GetPosition());
			}
			lastTalker = talker;
		}
	}

	private void OnStoppedTalking(object data)
	{
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		if (!string.IsNullOrEmpty(specificEffect))
		{
			Effects component = worker.GetComponent<Effects>();
			if (component.HasEffect(specificEffect))
			{
				priority = RELAXATION.PRIORITY.RECENTLY_USED;
			}
		}
		return true;
	}
}
