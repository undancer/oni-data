using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/AutoDisinfectable")]
public class AutoDisinfectable : Workable
{
	private Chore chore;

	private const float MAX_WORK_TIME = 10f;

	private float diseasePerSecond;

	[MyCmpGet]
	private PrimaryElement primaryElement;

	[Serialize]
	private bool enableAutoDisinfect = true;

	private static readonly EventSystem.IntraObjectHandler<AutoDisinfectable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<AutoDisinfectable>(delegate(AutoDisinfectable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		workerStatusItem = Db.Get().DuplicantStatusItems.Disinfecting;
		resetProgressOnStop = true;
		multitoolContext = "disinfect";
		multitoolHitEffectTag = "fx_disinfect_splash";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		SetWorkTime(10f);
		shouldTransferDiseaseWithWorker = false;
	}

	public void CancelChore()
	{
		if (chore != null)
		{
			chore.Cancel("AutoDisinfectable.CancelChore");
			chore = null;
		}
	}

	public void RefreshChore()
	{
		if (KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		if (!enableAutoDisinfect || !SaveGame.Instance.enableAutoDisinfect)
		{
			if (chore != null)
			{
				chore.Cancel("Autodisinfect Disabled");
				chore = null;
			}
		}
		else if (chore == null || !(chore.driver != null))
		{
			int diseaseCount = primaryElement.DiseaseCount;
			if (chore == null && diseaseCount > SaveGame.Instance.minGermCountForDisinfect)
			{
				chore = new WorkChore<AutoDisinfectable>(Db.Get().ChoreTypes.Disinfect, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.basic, 5, ignore_building_assignment: true);
			}
			else if (diseaseCount < SaveGame.Instance.minGermCountForDisinfect && chore != null)
			{
				chore.Cancel("AutoDisinfectable.Update");
				chore = null;
			}
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		diseasePerSecond = (float)GetComponent<PrimaryElement>().DiseaseCount / 10f;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		base.OnWorkTick(worker, dt);
		PrimaryElement component = GetComponent<PrimaryElement>();
		component.AddDisease(component.DiseaseIdx, -(int)(diseasePerSecond * dt + 0.5f), "Disinfectable.OnWorkTick");
		return false;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		PrimaryElement component = GetComponent<PrimaryElement>();
		component.AddDisease(component.DiseaseIdx, -component.DiseaseCount, "Disinfectable.OnCompleteWork");
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForDisinfection, this);
		chore = null;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	private void EnableAutoDisinfect()
	{
		enableAutoDisinfect = true;
		RefreshChore();
	}

	private void DisableAutoDisinfect()
	{
		enableAutoDisinfect = false;
		RefreshChore();
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo = null;
		buttonInfo = (enableAutoDisinfect ? new KIconButtonMenu.ButtonInfo("action_disinfect", STRINGS.BUILDINGS.AUTODISINFECTABLE.DISABLE_AUTODISINFECT.NAME, DisableAutoDisinfect, Action.NumActions, null, null, null, STRINGS.BUILDINGS.AUTODISINFECTABLE.DISABLE_AUTODISINFECT.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_disinfect", STRINGS.BUILDINGS.AUTODISINFECTABLE.ENABLE_AUTODISINFECT.NAME, EnableAutoDisinfect, Action.NumActions, null, null, null, STRINGS.BUILDINGS.AUTODISINFECTABLE.ENABLE_AUTODISINFECT.TOOLTIP));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 10f);
	}
}
