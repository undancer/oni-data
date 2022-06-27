using System;
using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Worker")]
public class Worker : KMonoBehaviour
{
	public enum State
	{
		Idle,
		Working,
		PendingCompletion,
		Completing
	}

	public class StartWorkInfo
	{
		public Workable workable { get; set; }

		public StartWorkInfo(Workable workable)
		{
			this.workable = workable;
		}
	}

	public enum WorkResult
	{
		Success,
		InProgress,
		Failed
	}

	private const float EARLIEST_REACT_TIME = 1f;

	[MyCmpGet]
	private Facing facing;

	[MyCmpGet]
	private MinionResume resume;

	private float workPendingCompletionTime;

	private int onWorkChoreDisabledHandle;

	public object workCompleteData;

	private Workable.AnimInfo animInfo;

	private KAnimSynchronizer kanimSynchronizer;

	private StatusItemGroup.Entry previousStatusItem;

	private StateMachine.Instance smi;

	private bool successFullyCompleted;

	private Vector3 workAnimOffset = Vector3.zero;

	public bool usesMultiTool = true;

	private static readonly EventSystem.IntraObjectHandler<Worker> OnChoreInterruptDelegate = new EventSystem.IntraObjectHandler<Worker>(delegate(Worker component, object data)
	{
		component.OnChoreInterrupt(data);
	});

	private Reactable passerbyReactable;

	public State state { get; private set; }

	public StartWorkInfo startWorkInfo { get; private set; }

	public Workable workable
	{
		get
		{
			if (startWorkInfo != null)
			{
				return startWorkInfo.workable;
			}
			return null;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		state = State.Idle;
		Subscribe(1485595942, OnChoreInterruptDelegate);
	}

	private string GetWorkableDebugString()
	{
		if (workable == null)
		{
			return "Null";
		}
		return workable.name;
	}

	public void CompleteWork()
	{
		successFullyCompleted = false;
		state = State.Idle;
		if (workable != null)
		{
			if (workable.triggerWorkReactions && workable.GetWorkTime() > 30f)
			{
				string conversationTopic = workable.GetConversationTopic();
				if (!conversationTopic.IsNullOrWhiteSpace())
				{
					CreateCompletionReactable(conversationTopic);
				}
			}
			DetachAnimOverrides();
			workable.CompleteWork(this);
			if (workable.worker != null && !(workable is Constructable) && !(workable is Deconstructable) && !(workable is Repairable) && !(workable is Disinfectable))
			{
				BonusEvent.GameplayEventData gameplayEventData = new BonusEvent.GameplayEventData();
				gameplayEventData.workable = workable;
				gameplayEventData.worker = workable.worker;
				gameplayEventData.building = workable.GetComponent<BuildingComplete>();
				gameplayEventData.eventTrigger = GameHashes.UseBuilding;
				GameplayEventManager.Instance.Trigger(1175726587, gameplayEventData);
			}
		}
		InternalStopWork(workable, is_aborted: false);
	}

	public WorkResult Work(float dt)
	{
		if (state == State.PendingCompletion)
		{
			bool flag = Time.time - workPendingCompletionTime > 10f;
			if (GetComponent<KAnimControllerBase>().IsStopped() || flag)
			{
				Navigator component = GetComponent<Navigator>();
				if (component != null)
				{
					NavGrid.NavTypeData navTypeData = component.NavGrid.GetNavTypeData(component.CurrentNavType);
					if (navTypeData.idleAnim.IsValid)
					{
						GetComponent<KAnimControllerBase>().Play(navTypeData.idleAnim);
					}
				}
				if (successFullyCompleted)
				{
					CompleteWork();
					return WorkResult.Success;
				}
				StopWork();
				return WorkResult.Failed;
			}
			return WorkResult.InProgress;
		}
		if (state == State.Completing)
		{
			if (successFullyCompleted)
			{
				CompleteWork();
				return WorkResult.Success;
			}
			StopWork();
			return WorkResult.Failed;
		}
		if (workable != null)
		{
			if ((bool)facing)
			{
				if (workable.ShouldFaceTargetWhenWorking())
				{
					facing.Face(workable.GetFacingTarget());
				}
				else
				{
					Rotatable component2 = workable.GetComponent<Rotatable>();
					bool flag2 = component2 != null && component2.GetOrientation() == Orientation.FlipH;
					Vector3 position = facing.transform.GetPosition();
					position += (flag2 ? Vector3.left : Vector3.right);
					facing.Face(position);
				}
			}
			if (dt > 0f && Game.Instance.FastWorkersModeActive)
			{
				dt = Mathf.Min(workable.WorkTimeRemaining + 0.01f, 5f);
			}
			Klei.AI.Attribute workAttribute = workable.GetWorkAttribute();
			AttributeLevels component3 = GetComponent<AttributeLevels>();
			if (workAttribute != null && workAttribute.IsTrainable && component3 != null)
			{
				float attributeExperienceMultiplier = workable.GetAttributeExperienceMultiplier();
				component3.AddExperience(workAttribute.Id, dt, attributeExperienceMultiplier);
			}
			string skillExperienceSkillGroup = workable.GetSkillExperienceSkillGroup();
			if (resume != null && skillExperienceSkillGroup != null)
			{
				float skillExperienceMultiplier = workable.GetSkillExperienceMultiplier();
				resume.AddExperienceWithAptitude(skillExperienceSkillGroup, dt, skillExperienceMultiplier);
			}
			float efficiencyMultiplier = workable.GetEfficiencyMultiplier(this);
			float dt2 = dt * efficiencyMultiplier * 1f;
			if (workable.WorkTick(this, dt2) && state == State.Working)
			{
				successFullyCompleted = true;
				StartPlayingPostAnim();
				workable.OnPendingCompleteWork(this);
			}
		}
		return WorkResult.InProgress;
	}

	private void StartPlayingPostAnim()
	{
		if (workable != null && !workable.alwaysShowProgressBar)
		{
			workable.ShowProgressBar(show: false);
		}
		GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption);
		state = State.PendingCompletion;
		workPendingCompletionTime = Time.time;
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		HashedString[] workPstAnims = workable.GetWorkPstAnims(this, successFullyCompleted);
		if (smi == null)
		{
			if (workPstAnims != null && workPstAnims.Length != 0)
			{
				if (workable != null && workable.synchronizeAnims)
				{
					KAnimControllerBase component2 = workable.GetComponent<KAnimControllerBase>();
					if (component2 != null)
					{
						component2.Play(workPstAnims);
					}
				}
				else
				{
					component.Play(workPstAnims);
				}
			}
			else
			{
				state = State.Completing;
			}
		}
		Trigger(-1142962013, this);
	}

	private void InternalStopWork(Workable target_workable, bool is_aborted)
	{
		state = State.Idle;
		base.gameObject.RemoveTag(GameTags.PerformingWorkRequest);
		GetComponent<KAnimControllerBase>().Offset -= workAnimOffset;
		workAnimOffset = Vector3.zero;
		GetComponent<KPrefabID>().RemoveTag(GameTags.PreventChoreInterruption);
		DetachAnimOverrides();
		ClearPasserbyReactable();
		AnimEventHandler component = GetComponent<AnimEventHandler>();
		if ((bool)component)
		{
			component.ClearContext();
		}
		if (previousStatusItem.item != null)
		{
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, previousStatusItem.item, previousStatusItem.data);
		}
		if (target_workable != null)
		{
			target_workable.Unsubscribe(onWorkChoreDisabledHandle);
			target_workable.StopWork(this, is_aborted);
		}
		if (smi != null)
		{
			smi.StopSM("stopping work");
			smi = null;
		}
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
		base.transform.SetPosition(position);
		startWorkInfo = null;
	}

	private void OnChoreInterrupt(object data)
	{
		if (state == State.Working)
		{
			successFullyCompleted = false;
			StartPlayingPostAnim();
		}
	}

	private void OnWorkChoreDisabled(object data)
	{
		string text = data as string;
		ChoreConsumer component = GetComponent<ChoreConsumer>();
		if (component != null && component.choreDriver != null)
		{
			component.choreDriver.GetCurrentChore().Fail((text != null) ? text : "WorkChoreDisabled");
		}
	}

	public void StopWork()
	{
		if (state == State.PendingCompletion || state == State.Completing)
		{
			state = State.Idle;
			if (successFullyCompleted)
			{
				CompleteWork();
			}
			else
			{
				InternalStopWork(workable, is_aborted: true);
			}
		}
		else
		{
			if (state != State.Working)
			{
				return;
			}
			if (workable != null && workable.synchronizeAnims)
			{
				KBatchedAnimController component = workable.GetComponent<KBatchedAnimController>();
				if (component != null)
				{
					HashedString[] workPstAnims = workable.GetWorkPstAnims(this, successfully_completed: false);
					if (workPstAnims != null && workPstAnims.Length != 0)
					{
						component.Play(workPstAnims);
						component.SetPositionPercent(1f);
					}
				}
			}
			InternalStopWork(workable, is_aborted: true);
		}
	}

	public void StartWork(StartWorkInfo start_work_info)
	{
		startWorkInfo = start_work_info;
		Game.Instance.StartedWork();
		if (state != 0)
		{
			string text = "";
			if (workable != null)
			{
				text = workable.name;
			}
			Debug.LogError(base.name + "." + text + ".state should be idle but instead it's:" + state);
		}
		string text2 = workable.GetType().Name;
		try
		{
			base.gameObject.AddTag(GameTags.PerformingWorkRequest);
			state = State.Working;
			base.gameObject.Trigger(1568504979, this);
			if (workable != null)
			{
				animInfo = workable.GetAnim(this);
				if (animInfo.smi != null)
				{
					smi = animInfo.smi;
					smi.StartSM();
				}
				Vector3 position = base.transform.GetPosition();
				position.z = Grid.GetLayerZ(workable.workLayer);
				base.transform.SetPosition(position);
				KAnimControllerBase component = GetComponent<KAnimControllerBase>();
				if (animInfo.smi == null)
				{
					AttachOverrideAnims(component);
				}
				HashedString[] workAnims = workable.GetWorkAnims(this);
				KAnim.PlayMode workAnimPlayMode = workable.GetWorkAnimPlayMode();
				Vector3 vector = (workAnimOffset = workable.GetWorkOffset());
				component.Offset += vector;
				if (usesMultiTool && animInfo.smi == null && workAnims != null && resume != null)
				{
					if (workable.synchronizeAnims)
					{
						KAnimControllerBase component2 = workable.GetComponent<KAnimControllerBase>();
						if (component2 != null)
						{
							kanimSynchronizer = component2.GetSynchronizer();
							if (kanimSynchronizer != null)
							{
								kanimSynchronizer.Add(component);
							}
						}
						component2.Play(workAnims, workAnimPlayMode);
					}
					else
					{
						component.Play(workAnims, workAnimPlayMode);
					}
				}
			}
			workable.StartWork(this);
			if (workable == null)
			{
				Debug.LogWarning("Stopped work as soon as I started. This is usually a sign that a chore is open when it shouldn't be or that it's preconditions are wrong.");
				return;
			}
			onWorkChoreDisabledHandle = workable.Subscribe(2108245096, OnWorkChoreDisabled);
			if (workable.triggerWorkReactions && workable.WorkTimeRemaining > 10f)
			{
				CreatePasserbyReactable();
			}
			KSelectable component3 = GetComponent<KSelectable>();
			previousStatusItem = component3.GetStatusItem(Db.Get().StatusItemCategories.Main);
			component3.SetStatusItem(Db.Get().StatusItemCategories.Main, workable.GetWorkerStatusItem(), workable);
		}
		catch (Exception ex)
		{
			string text3 = "Exception in: Worker.StartWork(" + text2 + ")";
			DebugUtil.LogErrorArgs(this, text3 + "\n" + ex.ToString());
			throw;
		}
	}

	private void Update()
	{
		if (state == State.Working)
		{
			ForceSyncAnims();
		}
	}

	private void ForceSyncAnims()
	{
		if (Time.deltaTime > 0f && kanimSynchronizer != null)
		{
			kanimSynchronizer.SyncTime();
		}
	}

	public bool InstantlyFinish()
	{
		if (workable != null)
		{
			return workable.InstantlyFinish(this);
		}
		return false;
	}

	private void AttachOverrideAnims(KAnimControllerBase worker_controller)
	{
		if (animInfo.overrideAnims != null && animInfo.overrideAnims.Length != 0)
		{
			for (int i = 0; i < animInfo.overrideAnims.Length; i++)
			{
				worker_controller.AddAnimOverrides(animInfo.overrideAnims[i]);
			}
		}
	}

	private void DetachAnimOverrides()
	{
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		if (kanimSynchronizer != null)
		{
			kanimSynchronizer.Remove(component);
			kanimSynchronizer = null;
		}
		if (animInfo.overrideAnims != null)
		{
			for (int i = 0; i < animInfo.overrideAnims.Length; i++)
			{
				component.RemoveAnimOverrides(animInfo.overrideAnims[i]);
			}
			animInfo.overrideAnims = null;
		}
	}

	private void CreateCompletionReactable(string topic)
	{
		if (!(GameClock.Instance.GetTime() / 600f < 1f))
		{
			EmoteReactable emoteReactable = OneshotReactableLocator.CreateOneshotReactable(base.gameObject, 3f, "WorkCompleteAcknowledgement", Db.Get().ChoreTypes.Emote, "anim_clapcheer_kanim", 9, 5, 100f);
			emoteReactable.AddStep(new EmoteReactable.EmoteStep
			{
				anim = "clapcheer_pre",
				startcb = GetReactionEffect
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "clapcheer_loop"
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "clapcheer_pst",
				finishcb = delegate(GameObject r)
				{
					r.Trigger(937885943, topic);
				}
			})
				.AddPrecondition(ReactorIsOnFloor);
			Tuple<Sprite, Color> tuple = null;
			tuple = Def.GetUISprite(topic, "ui", centered: true);
			if (tuple != null)
			{
				Thought thought = new Thought("Completion_" + topic, null, tuple.first, "mode_satisfaction", "conversation_short", "bubble_conversation", SpeechMonitor.PREFIX_HAPPY, "", show_immediately: true);
				emoteReactable.AddThought(thought);
			}
		}
	}

	public void CreatePasserbyReactable()
	{
		if (!(GameClock.Instance.GetTime() / 600f < 1f) && passerbyReactable == null)
		{
			passerbyReactable = new EmoteReactable(base.gameObject, "WorkPasserbyAcknowledgement", Db.Get().ChoreTypes.Emote, "anim_react_thumbsup_kanim", 5, 5, 30f, 720f * TuningData<DupeGreetingManager.Tuning>.Get().greetingDelayMultiplier).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "react",
				startcb = GetReactionEffect
			}).AddThought(Db.Get().Thoughts.Encourage).AddPrecondition(ReactorIsOnFloor)
				.AddPrecondition(ReactorIsFacingMe)
				.AddPrecondition(ReactorIsntPartying);
		}
	}

	private void GetReactionEffect(GameObject reactor)
	{
		GetComponent<Effects>().Add("WorkEncouraged", should_save: true);
	}

	private bool ReactorIsOnFloor(GameObject reactor, Navigator.ActiveTransition transition)
	{
		return transition.end == NavType.Floor;
	}

	private bool ReactorIsFacingMe(GameObject reactor, Navigator.ActiveTransition transition)
	{
		Facing component = reactor.GetComponent<Facing>();
		return base.transform.GetPosition().x < reactor.transform.GetPosition().x == component.GetFacing();
	}

	private bool ReactorIsntPartying(GameObject reactor, Navigator.ActiveTransition transition)
	{
		ChoreConsumer component = reactor.GetComponent<ChoreConsumer>();
		if (component.choreDriver.HasChore())
		{
			return component.choreDriver.GetCurrentChore().choreType != Db.Get().ChoreTypes.Party;
		}
		return false;
	}

	public void ClearPasserbyReactable()
	{
		if (passerbyReactable != null)
		{
			passerbyReactable.Cleanup();
			passerbyReactable = null;
		}
	}
}
