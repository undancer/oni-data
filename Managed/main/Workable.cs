using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Workable")]
public class Workable : KMonoBehaviour, ISaveLoadable, IApproachable
{
	public enum WorkableEvent
	{
		WorkStarted,
		WorkCompleted,
		WorkStopped
	}

	public struct AnimInfo
	{
		public KAnimFile[] overrideAnims;

		public StateMachine.Instance smi;
	}

	public float workTime;

	public Vector3 AnimOffset = Vector3.zero;

	protected bool showProgressBar = true;

	public bool alwaysShowProgressBar;

	protected bool lightEfficiencyBonus = true;

	protected StatusItem lightEfficiencyBonusStatusItem;

	protected Guid lightEfficiencyBonusStatusItemHandle;

	public bool currentlyLit;

	protected StatusItem workerStatusItem;

	protected StatusItem workingStatusItem;

	protected Guid workStatusItemHandle;

	protected OffsetTracker offsetTracker;

	[SerializeField]
	protected string attributeConverterId;

	protected AttributeConverter attributeConverter;

	protected float minimumAttributeMultiplier = 0.5f;

	public bool resetProgressOnStop;

	protected bool shouldTransferDiseaseWithWorker = true;

	[SerializeField]
	protected float attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;

	[SerializeField]
	protected string skillExperienceSkillGroup;

	[SerializeField]
	protected float skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;

	public bool triggerWorkReactions = true;

	public ReportManager.ReportType reportType = ReportManager.ReportType.WorkTime;

	[SerializeField]
	[Tooltip("What layer does the dupe switch to when interacting with the building")]
	public Grid.SceneLayer workLayer = Grid.SceneLayer.Move;

	[SerializeField]
	[Serialize]
	protected float workTimeRemaining = float.PositiveInfinity;

	[SerializeField]
	public KAnimFile[] overrideAnims;

	[SerializeField]
	protected HashedString multitoolContext;

	[SerializeField]
	protected Tag multitoolHitEffectTag;

	[SerializeField]
	[Tooltip("Whether to user the KAnimSynchronizer or not")]
	public bool synchronizeAnims = true;

	[SerializeField]
	[Tooltip("Whether to display number of uses in the details panel")]
	public bool trackUses;

	[Serialize]
	protected int numberOfUses;

	public Action<WorkableEvent> OnWorkableEventCB;

	private int skillsUpdateHandle = -1;

	private int minionUpdateHandle = -1;

	public string requiredSkillPerk;

	[SerializeField]
	protected bool shouldShowSkillPerkStatusItem = true;

	[SerializeField]
	public bool requireMinionToWork;

	protected StatusItem readyForSkillWorkStatusItem;

	public HashedString[] workAnims = new HashedString[2] { "working_pre", "working_loop" };

	public HashedString[] workingPstComplete = new HashedString[1] { "working_pst" };

	public HashedString[] workingPstFailed = new HashedString[1] { "working_pst" };

	public KAnim.PlayMode workAnimPlayMode;

	public bool faceTargetWhenWorking;

	protected ProgressBar progressBar;

	public Worker worker { get; protected set; }

	public float WorkTimeRemaining
	{
		get
		{
			return workTimeRemaining;
		}
		set
		{
			workTimeRemaining = value;
		}
	}

	public bool preferUnreservedCell { get; set; }

	public virtual float GetWorkTime()
	{
		return workTime;
	}

	public Worker GetWorker()
	{
		return worker;
	}

	public virtual float GetPercentComplete()
	{
		if (!(workTimeRemaining <= workTime))
		{
			return -1f;
		}
		return 1f - workTimeRemaining / workTime;
	}

	public void ConfigureMultitoolContext(HashedString context, Tag hitEffectTag)
	{
		multitoolContext = context;
		multitoolHitEffectTag = hitEffectTag;
	}

	public virtual AnimInfo GetAnim(Worker worker)
	{
		AnimInfo result = default(AnimInfo);
		if (overrideAnims != null && overrideAnims.Length != 0)
		{
			result.overrideAnims = overrideAnims;
		}
		if (multitoolContext.IsValid && multitoolHitEffectTag.IsValid)
		{
			result.smi = new MultitoolController.Instance(this, worker, multitoolContext, Assets.GetPrefab(multitoolHitEffectTag));
		}
		return result;
	}

	public virtual HashedString[] GetWorkAnims(Worker worker)
	{
		return workAnims;
	}

	public virtual KAnim.PlayMode GetWorkAnimPlayMode()
	{
		return workAnimPlayMode;
	}

	public virtual HashedString[] GetWorkPstAnims(Worker worker, bool successfully_completed)
	{
		if (successfully_completed)
		{
			return workingPstComplete;
		}
		return workingPstFailed;
	}

	public virtual Vector3 GetWorkOffset()
	{
		return Vector3.zero;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().MiscStatusItems.Using;
		workingStatusItem = Db.Get().MiscStatusItems.Operating;
		readyForSkillWorkStatusItem = Db.Get().BuildingStatusItems.RequiresSkillPerk;
		workTime = GetWorkTime();
		workTimeRemaining = Mathf.Min(workTimeRemaining, workTime);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (shouldShowSkillPerkStatusItem && !string.IsNullOrEmpty(requiredSkillPerk))
		{
			if (skillsUpdateHandle != -1)
			{
				Game.Instance.Unsubscribe(skillsUpdateHandle);
			}
			skillsUpdateHandle = Game.Instance.Subscribe(-1523247426, UpdateStatusItem);
		}
		if (requireMinionToWork && minionUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(minionUpdateHandle);
		}
		minionUpdateHandle = Game.Instance.Subscribe(586301400, UpdateStatusItem);
		GetComponent<KPrefabID>().AddTag(GameTags.HasChores);
		lightEfficiencyBonusStatusItem = Db.Get().DuplicantStatusItems.LightWorkEfficiencyBonus;
		ShowProgressBar(alwaysShowProgressBar && workTimeRemaining < GetWorkTime());
		UpdateStatusItem();
	}

	protected virtual void UpdateStatusItem(object data = null)
	{
		KSelectable component = GetComponent<KSelectable>();
		if (component == null)
		{
			return;
		}
		component.RemoveStatusItem(workStatusItemHandle);
		if (worker == null)
		{
			if (requireMinionToWork && Components.LiveMinionIdentities.GetWorldItems(this.GetMyWorldId()).Count == 0)
			{
				workStatusItemHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.WorkRequiresMinion);
			}
			else if (shouldShowSkillPerkStatusItem && !string.IsNullOrEmpty(requiredSkillPerk))
			{
				if (!MinionResume.AnyMinionHasPerk(requiredSkillPerk, this.GetMyWorldId()))
				{
					StatusItem status_item = (DlcManager.FeatureClusterSpaceEnabled() ? Db.Get().BuildingStatusItems.ClusterColonyLacksRequiredSkillPerk : Db.Get().BuildingStatusItems.ColonyLacksRequiredSkillPerk);
					workStatusItemHandle = component.AddStatusItem(status_item, requiredSkillPerk);
				}
				else
				{
					workStatusItemHandle = component.AddStatusItem(readyForSkillWorkStatusItem, requiredSkillPerk);
				}
			}
		}
		else if (workingStatusItem != null)
		{
			workStatusItemHandle = component.AddStatusItem(workingStatusItem, this);
		}
	}

	protected override void OnLoadLevel()
	{
		overrideAnims = null;
		base.OnLoadLevel();
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	public void StartWork(Worker worker_to_start)
	{
		Debug.Assert(worker_to_start != null, "How did we get a null worker?");
		worker = worker_to_start;
		UpdateStatusItem();
		if (showProgressBar)
		{
			ShowProgressBar(show: true);
		}
		OnStartWork(worker);
		if (worker != null)
		{
			string conversationTopic = GetConversationTopic();
			if (conversationTopic != null)
			{
				worker.Trigger(937885943, conversationTopic);
			}
		}
		if (OnWorkableEventCB != null)
		{
			OnWorkableEventCB(WorkableEvent.WorkStarted);
		}
		numberOfUses++;
		if (worker != null)
		{
			if (base.gameObject.GetComponent<KSelectable>() != null && base.gameObject.GetComponent<KSelectable>().IsSelected && worker.gameObject.GetComponent<LoopingSounds>() != null)
			{
				worker.gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(selected: true);
			}
			else if (worker.gameObject.GetComponent<KSelectable>() != null && worker.gameObject.GetComponent<KSelectable>().IsSelected && base.gameObject.GetComponent<LoopingSounds>() != null)
			{
				base.gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(selected: true);
			}
		}
		base.gameObject.Trigger(853695848, this);
	}

	public bool WorkTick(Worker worker, float dt)
	{
		bool flag = false;
		if (dt > 0f)
		{
			workTimeRemaining -= dt;
			flag = OnWorkTick(worker, dt);
		}
		if (!flag)
		{
			return workTimeRemaining < 0f;
		}
		return true;
	}

	public virtual float GetEfficiencyMultiplier(Worker worker)
	{
		float num = 1f;
		if (attributeConverter != null)
		{
			AttributeConverterInstance converter = worker.GetComponent<AttributeConverters>().GetConverter(attributeConverter.Id);
			num += converter.Evaluate();
		}
		if (lightEfficiencyBonus)
		{
			int num2 = Grid.PosToCell(worker.gameObject);
			if (Grid.IsValidCell(num2))
			{
				if (Grid.LightIntensity[num2] > 0)
				{
					currentlyLit = true;
					num += DUPLICANTSTATS.LIGHT.LIGHT_WORK_EFFICIENCY_BONUS;
					if (lightEfficiencyBonusStatusItemHandle == Guid.Empty)
					{
						lightEfficiencyBonusStatusItemHandle = worker.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.LightWorkEfficiencyBonus, this);
					}
				}
				else
				{
					currentlyLit = false;
					if (lightEfficiencyBonusStatusItemHandle != Guid.Empty)
					{
						worker.GetComponent<KSelectable>().RemoveStatusItem(lightEfficiencyBonusStatusItemHandle);
					}
				}
			}
		}
		return Mathf.Max(num, minimumAttributeMultiplier);
	}

	public virtual Klei.AI.Attribute GetWorkAttribute()
	{
		if (attributeConverter != null)
		{
			return attributeConverter.attribute;
		}
		return null;
	}

	public virtual string GetConversationTopic()
	{
		KPrefabID component = GetComponent<KPrefabID>();
		if (!component.HasTag(GameTags.NotConversationTopic))
		{
			return component.PrefabTag.Name;
		}
		return null;
	}

	public float GetAttributeExperienceMultiplier()
	{
		return attributeExperienceMultiplier;
	}

	public string GetSkillExperienceSkillGroup()
	{
		return skillExperienceSkillGroup;
	}

	public float GetSkillExperienceMultiplier()
	{
		return skillExperienceMultiplier;
	}

	protected virtual bool OnWorkTick(Worker worker, float dt)
	{
		return false;
	}

	public void StopWork(Worker workerToStop, bool aborted)
	{
		if (worker == workerToStop && aborted)
		{
			OnAbortWork(workerToStop);
		}
		if (shouldTransferDiseaseWithWorker)
		{
			TransferDiseaseWithWorker(workerToStop);
		}
		if (OnWorkableEventCB != null)
		{
			OnWorkableEventCB(WorkableEvent.WorkStopped);
		}
		OnStopWork(workerToStop);
		if (resetProgressOnStop)
		{
			workTimeRemaining = GetWorkTime();
		}
		ShowProgressBar(alwaysShowProgressBar && workTimeRemaining < GetWorkTime());
		if (lightEfficiencyBonusStatusItemHandle != Guid.Empty)
		{
			lightEfficiencyBonusStatusItemHandle = workerToStop.GetComponent<KSelectable>().RemoveStatusItem(lightEfficiencyBonusStatusItemHandle);
		}
		if (base.gameObject.GetComponent<KSelectable>() != null && !base.gameObject.GetComponent<KSelectable>().IsSelected && base.gameObject.GetComponent<LoopingSounds>() != null)
		{
			base.gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(selected: false);
		}
		else if (workerToStop.gameObject.GetComponent<KSelectable>() != null && !workerToStop.gameObject.GetComponent<KSelectable>().IsSelected && workerToStop.gameObject.GetComponent<LoopingSounds>() != null)
		{
			workerToStop.gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(selected: false);
		}
		worker = null;
		base.gameObject.Trigger(679550494, this);
		UpdateStatusItem();
	}

	public virtual StatusItem GetWorkerStatusItem()
	{
		return workerStatusItem;
	}

	public void SetWorkerStatusItem(StatusItem item)
	{
		workerStatusItem = item;
	}

	public void CompleteWork(Worker worker)
	{
		if (shouldTransferDiseaseWithWorker)
		{
			TransferDiseaseWithWorker(worker);
		}
		OnCompleteWork(worker);
		if (OnWorkableEventCB != null)
		{
			OnWorkableEventCB(WorkableEvent.WorkCompleted);
		}
		if (OnWorkableEventCB != null)
		{
			OnWorkableEventCB(WorkableEvent.WorkStopped);
		}
		workTimeRemaining = GetWorkTime();
		ShowProgressBar(show: false);
		base.gameObject.Trigger(-2011693419, this);
	}

	public void SetReportType(ReportManager.ReportType report_type)
	{
		reportType = report_type;
	}

	public ReportManager.ReportType GetReportType()
	{
		return reportType;
	}

	protected virtual void OnStartWork(Worker worker)
	{
	}

	protected virtual void OnStopWork(Worker worker)
	{
	}

	protected virtual void OnCompleteWork(Worker worker)
	{
	}

	protected virtual void OnAbortWork(Worker worker)
	{
	}

	public virtual void OnPendingCompleteWork(Worker worker)
	{
	}

	public void SetOffsets(CellOffset[] offsets)
	{
		if (offsetTracker != null)
		{
			offsetTracker.Clear();
		}
		offsetTracker = new StandardOffsetTracker(offsets);
	}

	public void SetOffsetTable(CellOffset[][] offset_table)
	{
		if (offsetTracker != null)
		{
			offsetTracker.Clear();
		}
		offsetTracker = new OffsetTableTracker(offset_table, this);
	}

	public virtual CellOffset[] GetOffsets(int cell)
	{
		if (offsetTracker == null)
		{
			offsetTracker = new StandardOffsetTracker(new CellOffset[1]);
		}
		return offsetTracker.GetOffsets(cell);
	}

	public CellOffset[] GetOffsets()
	{
		return GetOffsets(Grid.PosToCell(this));
	}

	public void SetWorkTime(float work_time)
	{
		workTime = work_time;
		workTimeRemaining = work_time;
	}

	public bool ShouldFaceTargetWhenWorking()
	{
		return faceTargetWhenWorking;
	}

	public virtual Vector3 GetFacingTarget()
	{
		return base.transform.GetPosition();
	}

	public void ShowProgressBar(bool show)
	{
		if (show)
		{
			if (progressBar == null)
			{
				progressBar = ProgressBar.CreateProgressBar(base.gameObject, GetPercentComplete);
			}
			progressBar.gameObject.SetActive(value: true);
		}
		else if (progressBar != null)
		{
			progressBar.gameObject.DeleteObject();
			progressBar = null;
		}
	}

	protected override void OnCleanUp()
	{
		ShowProgressBar(show: false);
		if (offsetTracker != null)
		{
			offsetTracker.Clear();
		}
		if (skillsUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(skillsUpdateHandle);
		}
		if (minionUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(minionUpdateHandle);
		}
		base.OnCleanUp();
		OnWorkableEventCB = null;
	}

	public virtual Vector3 GetTargetPoint()
	{
		Vector3 result = base.transform.GetPosition();
		float y = result.y + 0.65f;
		KBoxCollider2D component = GetComponent<KBoxCollider2D>();
		if (component != null)
		{
			result = component.bounds.center;
		}
		result.y = y;
		result.z = 0f;
		return result;
	}

	public int GetNavigationCost(Navigator navigator, int cell)
	{
		return navigator.GetNavigationCost(cell, GetOffsets(cell));
	}

	public int GetNavigationCost(Navigator navigator)
	{
		return GetNavigationCost(navigator, Grid.PosToCell(this));
	}

	private void TransferDiseaseWithWorker(Worker worker)
	{
		if (!(this == null) && !(worker == null))
		{
			TransferDiseaseWithWorker(base.gameObject, worker.gameObject);
		}
	}

	public static void TransferDiseaseWithWorker(GameObject workable, GameObject worker)
	{
		if (workable == null || worker == null)
		{
			return;
		}
		PrimaryElement component = workable.GetComponent<PrimaryElement>();
		if (component == null)
		{
			return;
		}
		PrimaryElement component2 = worker.GetComponent<PrimaryElement>();
		if (!(component2 == null))
		{
			SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
			invalid.idx = component2.DiseaseIdx;
			invalid.count = (int)((float)component2.DiseaseCount * 0.33f);
			SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid;
			invalid2.idx = component.DiseaseIdx;
			invalid2.count = (int)((float)component.DiseaseCount * 0.33f);
			component2.ModifyDiseaseCount(-invalid.count, "Workable.TransferDiseaseWithWorker");
			component.ModifyDiseaseCount(-invalid2.count, "Workable.TransferDiseaseWithWorker");
			if (invalid.count > 0)
			{
				component.AddDisease(invalid.idx, invalid.count, "Workable.TransferDiseaseWithWorker");
			}
			if (invalid2.count > 0)
			{
				component2.AddDisease(invalid2.idx, invalid2.count, "Workable.TransferDiseaseWithWorker");
			}
		}
	}

	public virtual bool InstantlyFinish(Worker worker)
	{
		float num = worker.workable.WorkTimeRemaining;
		if (!float.IsInfinity(num))
		{
			worker.Work(num);
			return true;
		}
		DebugUtil.DevAssert(test: false, ToString() + " was asked to instantly finish but it has infinite work time! Override InstantlyFinish in your workable!");
		return false;
	}

	public virtual List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (trackUses)
		{
			Descriptor item = new Descriptor(string.Format(BUILDING.DETAILS.USE_COUNT, numberOfUses), string.Format(BUILDING.DETAILS.USE_COUNT_TOOLTIP, numberOfUses), Descriptor.DescriptorType.Detail);
			list.Add(item);
		}
		return list;
	}

	[ContextMenu("Refresh Reachability")]
	public void RefreshReachability()
	{
		if (offsetTracker != null)
		{
			offsetTracker.ForceRefresh();
		}
	}
}
