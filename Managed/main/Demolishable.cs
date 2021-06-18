using KSerialization;
using STRINGS;
using TUNING;

public class Demolishable : Workable
{
	public Chore chore = null;

	public bool allowDemolition = true;

	[Serialize]
	private bool isMarkedForDemolition;

	private bool destroyed = false;

	private static readonly EventSystem.IntraObjectHandler<Demolishable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Demolishable>(delegate(Demolishable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Demolishable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Demolishable>(delegate(Demolishable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Demolishable> OnDeconstructDelegate = new EventSystem.IntraObjectHandler<Demolishable>(delegate(Demolishable component, object data)
	{
		component.OnDemolish(data);
	});

	public bool HasBeenDestroyed => destroyed;

	private CellOffset[] placementOffsets
	{
		get
		{
			Building component = GetComponent<Building>();
			if (component != null)
			{
				return component.Def.PlacementOffsets;
			}
			OccupyArea component2 = GetComponent<OccupyArea>();
			if (component2 != null)
			{
				return component2.OccupiedCellsOffsets;
			}
			Debug.Assert(condition: false, "Ack! We put a Demolishable on something that's neither a Building nor OccupyArea!", this);
			return null;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		requiredSkillPerk = Db.Get().SkillPerks.CanDemolish.Id;
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		workerStatusItem = Db.Get().DuplicantStatusItems.Deconstructing;
		attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		minimumAttributeMultiplier = 0.75f;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		multitoolContext = "demolish";
		multitoolHitEffectTag = EffectConfigs.DemolishSplashId;
		workingPstComplete = null;
		workingPstFailed = null;
		CellOffset[][] table = OffsetGroups.InvertedStandardTable;
		CellOffset[] filter = null;
		Building component = GetComponent<Building>();
		if (component != null && component.Def.IsTilePiece)
		{
			table = OffsetGroups.InvertedStandardTableWithCorners;
			filter = component.Def.ConstructionOffsetFilter;
			SetWorkTime(component.Def.ConstructionTime * 0.5f);
		}
		else
		{
			SetWorkTime(30f);
		}
		CellOffset[][] offsetTable = OffsetGroups.BuildReachabilityTable(placementOffsets, table, filter);
		SetOffsetTable(offsetTable);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-111137758, OnRefreshUserMenuDelegate);
		Subscribe(2127324410, OnCancelDelegate);
		Subscribe(-790448070, OnDeconstructDelegate);
		if (isMarkedForDemolition)
		{
			QueueDemolition();
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("DeconstructBar");
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDemolition);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		TriggerDestroy();
	}

	private void TriggerDestroy()
	{
		if (!(this == null) && !destroyed)
		{
			destroyed = true;
			isMarkedForDemolition = false;
			base.gameObject.DeleteObject();
		}
	}

	private void QueueDemolition()
	{
		if (DebugHandler.InstantBuildMode)
		{
			OnCompleteWork(null);
			return;
		}
		if (chore == null)
		{
			Prioritizable.AddRef(base.gameObject);
			requiredSkillPerk = Db.Get().SkillPerks.CanDemolish.Id;
			chore = new WorkChore<Demolishable>(Db.Get().ChoreTypes.Demolish, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false, Assets.GetAnim("anim_interacts_clothingfactory_kanim"), is_preemptable: true, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.basic, 5, ignore_building_assignment: true);
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingDemolition, this);
			isMarkedForDemolition = true;
			Trigger(2108245096, "Demolish");
		}
		UpdateStatusItem();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (allowDemolition)
		{
			KIconButtonMenu.ButtonInfo button = ((chore == null) ? new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.DEMOLISH.NAME, OnDemolish, Action.NumActions, null, null, null, UI.USERMENUACTIONS.DEMOLISH.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.DEMOLISH.NAME_OFF, OnDemolish, Action.NumActions, null, null, null, UI.USERMENUACTIONS.DEMOLISH.TOOLTIP_OFF));
			Game.Instance.userMenu.AddButton(base.gameObject, button, 0f);
		}
	}

	public void CancelDemolition()
	{
		if (chore != null)
		{
			chore.Cancel("Cancelled demolition");
			chore = null;
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDemolition);
			ShowProgressBar(show: false);
			isMarkedForDemolition = false;
			Prioritizable.RemoveRef(base.gameObject);
		}
		UpdateStatusItem();
	}

	private void OnCancel(object data)
	{
		CancelDemolition();
	}

	private void OnDemolish(object data)
	{
		if (allowDemolition || DebugHandler.InstantBuildMode)
		{
			QueueDemolition();
		}
	}

	private void OnDemolish()
	{
		if (chore == null)
		{
			QueueDemolition();
		}
		else
		{
			CancelDemolition();
		}
	}

	protected override void UpdateStatusItem(object data = null)
	{
		shouldShowSkillPerkStatusItem = isMarkedForDemolition;
		base.UpdateStatusItem(data);
	}
}
