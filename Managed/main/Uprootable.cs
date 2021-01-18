using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Uprootable")]
public class Uprootable : Workable
{
	[Serialize]
	protected bool isMarkedForUproot;

	protected bool uprootComplete;

	[MyCmpReq]
	private Prioritizable prioritizable;

	[Serialize]
	protected bool canBeUprooted = true;

	public bool deselectOnUproot = true;

	protected Chore chore;

	private string buttonLabel;

	private string buttonTooltip;

	private string cancelButtonLabel;

	private string cancelButtonTooltip;

	private StatusItem pendingStatusItem;

	public OccupyArea area;

	private Storage planterStorage;

	public bool showUserMenuButtons = true;

	public HandleVector<int>.Handle partitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<Uprootable> OnPlanterStorageDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.OnPlanterStorage(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Uprootable> ForceCancelUprootDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.ForceCancelUproot(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Uprootable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Uprootable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public bool IsMarkedForUproot => isMarkedForUproot;

	public Storage GetPlanterStorage => planterStorage;

	protected Uprootable()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		buttonLabel = UI.USERMENUACTIONS.UPROOT.NAME;
		buttonTooltip = UI.USERMENUACTIONS.UPROOT.TOOLTIP;
		cancelButtonLabel = UI.USERMENUACTIONS.CANCELUPROOT.NAME;
		cancelButtonTooltip = UI.USERMENUACTIONS.CANCELUPROOT.TOOLTIP;
		pendingStatusItem = Db.Get().MiscStatusItems.PendingUproot;
		workerStatusItem = Db.Get().DuplicantStatusItems.Uprooting;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		pendingStatusItem = Db.Get().MiscStatusItems.PendingUproot;
		workerStatusItem = Db.Get().DuplicantStatusItems.Uprooting;
		attributeConverter = Db.Get().AttributeConverters.HarvestSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Farming.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		multitoolContext = "harvest";
		multitoolHitEffectTag = "fx_harvest_splash";
		Subscribe(1309017699, OnPlanterStorageDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(2127324410, ForceCancelUprootDelegate);
		SetWorkTime(12.5f);
		Subscribe(2127324410, OnCancelDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		faceTargetWhenWorking = true;
		Components.Uprootables.Add(this);
		area = GetComponent<OccupyArea>();
		Prioritizable.AddRef(base.gameObject);
		base.gameObject.AddTag(GameTags.Plant);
		Extents extents = new Extents(Grid.PosToCell(base.gameObject), base.gameObject.GetComponent<OccupyArea>().OccupiedCellsOffsets);
		partitionerEntry = GameScenePartitioner.Instance.Add(base.gameObject.name, base.gameObject.GetComponent<KPrefabID>(), extents, GameScenePartitioner.Instance.plants, null);
		if (isMarkedForUproot)
		{
			MarkForUproot();
		}
	}

	private void OnPlanterStorage(object data)
	{
		planterStorage = (Storage)data;
		Prioritizable component = GetComponent<Prioritizable>();
		if (component != null)
		{
			component.showIcon = planterStorage == null;
		}
	}

	public bool IsInPlanterBox()
	{
		return planterStorage != null;
	}

	public void Uproot()
	{
		isMarkedForUproot = false;
		chore = null;
		uprootComplete = true;
		Trigger(-216549700, this);
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingUproot);
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.Operating);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public void SetCanBeUprooted(bool state)
	{
		canBeUprooted = state;
		if (canBeUprooted)
		{
			SetUprootedComplete(state: false);
		}
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public void SetUprootedComplete(bool state)
	{
		uprootComplete = state;
	}

	public void MarkForUproot(bool instantOnDebug = true)
	{
		if (canBeUprooted)
		{
			if (DebugHandler.InstantBuildMode && instantOnDebug)
			{
				Uproot();
			}
			else if (chore == null)
			{
				chore = new WorkChore<Uprootable>(Db.Get().ChoreTypes.Uproot, this);
				GetComponent<KSelectable>().AddStatusItem(pendingStatusItem, this);
			}
			isMarkedForUproot = true;
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Uproot();
	}

	private void OnCancel(object data)
	{
		if (chore != null)
		{
			chore.Cancel("Cancel uproot");
			chore = null;
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingUproot);
		}
		isMarkedForUproot = false;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public bool HasChore()
	{
		if (chore == null)
		{
			return false;
		}
		return true;
	}

	private void OnClickUproot()
	{
		MarkForUproot();
	}

	protected void OnClickCancelUproot()
	{
		OnCancel(null);
	}

	public virtual void ForceCancelUproot(object data = null)
	{
		OnCancel(null);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!showUserMenuButtons)
		{
			return;
		}
		if (uprootComplete)
		{
			if (deselectOnUproot)
			{
				KSelectable component = GetComponent<KSelectable>();
				if (component != null && SelectTool.Instance.selected == component)
				{
					SelectTool.Instance.Select(null);
				}
			}
		}
		else if (canBeUprooted)
		{
			KIconButtonMenu.ButtonInfo button = ((chore != null) ? new KIconButtonMenu.ButtonInfo("action_uproot", cancelButtonLabel, OnClickCancelUproot, Action.NumActions, null, null, null, cancelButtonTooltip) : new KIconButtonMenu.ButtonInfo("action_uproot", buttonLabel, OnClickUproot, Action.NumActions, null, null, null, buttonTooltip));
			Game.Instance.userMenu.AddButton(base.gameObject, button);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		Components.Uprootables.Remove(this);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingUproot);
	}
}
