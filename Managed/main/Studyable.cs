using System;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Studyable")]
public class Studyable : Workable, ISidescreenButtonControl
{
	public string meterTrackerSymbol;

	public string meterAnim;

	private Chore chore;

	private const float STUDY_WORK_TIME = 3600f;

	[Serialize]
	private bool studied = false;

	[Serialize]
	private bool markedForStudy = false;

	private Guid statusItemGuid;

	private Guid additionalStatusItemGuid;

	private MeterController studiedIndicator;

	public bool Studied => studied;

	public bool Studying => chore != null && chore.InProgress();

	public string SidescreenTitleKey => "STRINGS.UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.TITLE";

	public string SidescreenStatusMessage
	{
		get
		{
			if (studied)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.STUDIED_STATUS;
			}
			if (markedForStudy)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.PENDING_STATUS;
			}
			return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.SEND_STATUS;
		}
	}

	public string SidescreenButtonText
	{
		get
		{
			if (studied)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.STUDIED_BUTTON;
			}
			if (markedForStudy)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.PENDING_BUTTON;
			}
			return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.SEND_BUTTON;
		}
	}

	public string SidescreenButtonTooltip
	{
		get
		{
			if (studied)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.STUDIED_STATUS;
			}
			if (markedForStudy)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.PENDING_STATUS;
			}
			return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.SEND_STATUS;
		}
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public bool SidescreenButtonInteractable()
	{
		return !studied;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_use_machine_kanim")
		};
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		workerStatusItem = Db.Get().DuplicantStatusItems.Studying;
		resetProgressOnStop = false;
		requiredSkillPerk = Db.Get().SkillPerks.CanStudyWorldObjects.Id;
		attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		SetWorkTime(3600f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		studiedIndicator = new MeterController(GetComponent<KBatchedAnimController>(), meterTrackerSymbol, meterAnim, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, meterTrackerSymbol);
		Refresh();
	}

	public void CancelChore()
	{
		if (chore != null)
		{
			chore.Cancel("Studyable.CancelChore");
			chore = null;
			Trigger(1488501379);
		}
	}

	public void Refresh()
	{
		if (KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		KSelectable component = GetComponent<KSelectable>();
		if (studied)
		{
			statusItemGuid = component.ReplaceStatusItem(statusItemGuid, Db.Get().MiscStatusItems.Studied);
			studiedIndicator.gameObject.SetActive(value: true);
			studiedIndicator.meterController.Play(meterAnim, KAnim.PlayMode.Loop);
			requiredSkillPerk = null;
			UpdateStatusItem();
			return;
		}
		if (markedForStudy)
		{
			if (chore == null)
			{
				chore = new WorkChore<Studyable>(Db.Get().ChoreTypes.Research, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
			}
			statusItemGuid = component.ReplaceStatusItem(statusItemGuid, Db.Get().MiscStatusItems.AwaitingStudy);
		}
		else
		{
			CancelChore();
			statusItemGuid = component.RemoveStatusItem(statusItemGuid);
		}
		studiedIndicator.gameObject.SetActive(value: false);
	}

	private void ToggleStudyChore()
	{
		if (DebugHandler.InstantBuildMode)
		{
			studied = true;
			if (chore != null)
			{
				chore.Cancel("debug");
				chore = null;
			}
			Trigger(-1436775550);
		}
		else
		{
			markedForStudy = !markedForStudy;
		}
		Refresh();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		studied = true;
		chore = null;
		Refresh();
		Trigger(-1436775550);
	}

	public void OnSidescreenButtonPressed()
	{
		ToggleStudyChore();
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}
}
