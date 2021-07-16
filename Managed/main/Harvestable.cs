using KSerialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Harvestable")]
public class Harvestable : Workable
{
	public HarvestDesignatable harvestDesignatable;

	[Serialize]
	protected bool canBeHarvested;

	protected Chore chore;

	private static readonly EventSystem.IntraObjectHandler<Harvestable> ForceCancelHarvestDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.ForceCancelHarvest(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Harvestable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.OnCancel(data);
	});

	public Worker completed_by
	{
		get;
		protected set;
	}

	public bool CanBeHarvested => canBeHarvested;

	protected Harvestable()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Harvesting;
		multitoolContext = "harvest";
		multitoolHitEffectTag = "fx_harvest_splash";
	}

	protected override void OnSpawn()
	{
		harvestDesignatable = GetComponent<HarvestDesignatable>();
		Subscribe(2127324410, ForceCancelHarvestDelegate);
		SetWorkTime(10f);
		Subscribe(2127324410, OnCancelDelegate);
		faceTargetWhenWorking = true;
		Components.Harvestables.Add(this);
		attributeConverter = Db.Get().AttributeConverters.HarvestSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Farming.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
	}

	public void OnUprooted(object data)
	{
		if (canBeHarvested)
		{
			Harvest();
		}
	}

	public void Harvest()
	{
		harvestDesignatable.MarkedForHarvest = false;
		chore = null;
		Trigger(1272413801, this);
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
		component.RemoveStatusItem(Db.Get().MiscStatusItems.Operating);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public void OnMarkedForHarvest()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (chore == null)
		{
			chore = new WorkChore<Harvestable>(Db.Get().ChoreTypes.Harvest, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: true);
			component.AddStatusItem(Db.Get().MiscStatusItems.PendingHarvest, this);
		}
		component.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest);
	}

	public void SetCanBeHarvested(bool state)
	{
		canBeHarvested = state;
		KSelectable component = GetComponent<KSelectable>();
		if (canBeHarvested)
		{
			component.AddStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest);
			if (harvestDesignatable.HarvestWhenReady)
			{
				harvestDesignatable.MarkForHarvest();
			}
			else if (harvestDesignatable.InPlanterBox)
			{
				component.AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, this);
			}
		}
		else
		{
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest);
			component.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest);
		}
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		completed_by = worker;
		Harvest();
	}

	protected virtual void OnCancel(object data)
	{
		if (chore != null)
		{
			chore.Cancel("Cancel harvest");
			chore = null;
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
			harvestDesignatable.SetHarvestWhenReady(state: false);
		}
		harvestDesignatable.MarkedForHarvest = false;
	}

	public bool HasChore()
	{
		if (chore == null)
		{
			return false;
		}
		return true;
	}

	public virtual void ForceCancelHarvest(object data = null)
	{
		OnCancel(null);
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Harvestables.Remove(this);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
	}
}
