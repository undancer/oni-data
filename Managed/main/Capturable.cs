using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Capturable")]
public class Capturable : Workable, IGameObjectEffectDescriptor
{
	[MyCmpAdd]
	private Baggable baggable;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	public bool allowCapture = true;

	[Serialize]
	private bool markedForCapture;

	private Chore chore;

	private static readonly EventSystem.IntraObjectHandler<Capturable> OnDeathDelegate = new EventSystem.IntraObjectHandler<Capturable>(delegate(Capturable component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Capturable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Capturable>(delegate(Capturable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Capturable> OnTagsChangedDelegate = new EventSystem.IntraObjectHandler<Capturable>(delegate(Capturable component, object data)
	{
		component.OnTagsChanged(data);
	});

	public bool IsMarkedForCapture => markedForCapture;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Capturables.Add(this);
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		requiredSkillPerk = Db.Get().SkillPerks.CanWrangleCreatures.Id;
		resetProgressOnStop = true;
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		multitoolContext = "capture";
		multitoolHitEffectTag = "fx_capture_splash";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(1623392196, OnDeathDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-1582839653, OnTagsChangedDelegate);
		if (markedForCapture)
		{
			Prioritizable.AddRef(base.gameObject);
		}
		UpdateStatusItem();
		UpdateChore();
		SetWorkTime(10f);
	}

	protected override void OnCleanUp()
	{
		Components.Capturables.Remove(this);
		base.OnCleanUp();
	}

	public override Vector3 GetTargetPoint()
	{
		Vector3 result = base.transform.GetPosition();
		KBoxCollider2D component = GetComponent<KBoxCollider2D>();
		if (component != null)
		{
			result = component.bounds.center;
		}
		result.z = 0f;
		return result;
	}

	private void OnDeath(object data)
	{
		allowCapture = false;
		markedForCapture = false;
		UpdateChore();
	}

	private void OnTagsChanged(object data)
	{
		MarkForCapture(markedForCapture);
	}

	public void MarkForCapture(bool mark)
	{
		PrioritySetting priority = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);
		MarkForCapture(mark, priority);
	}

	public void MarkForCapture(bool mark, PrioritySetting priority)
	{
		mark = mark && IsCapturable();
		if (markedForCapture && !mark)
		{
			Prioritizable.RemoveRef(base.gameObject);
		}
		else if (!markedForCapture && mark)
		{
			Prioritizable.AddRef(base.gameObject);
			Prioritizable component = GetComponent<Prioritizable>();
			if ((bool)component)
			{
				component.SetMasterPriority(priority);
			}
		}
		markedForCapture = mark;
		UpdateStatusItem();
		UpdateChore();
	}

	public bool IsCapturable()
	{
		if (!allowCapture)
		{
			return false;
		}
		if (base.gameObject.HasTag(GameTags.Trapped))
		{
			return false;
		}
		if (base.gameObject.HasTag(GameTags.Stored))
		{
			return false;
		}
		if (base.gameObject.HasTag(GameTags.Creatures.Bagged))
		{
			return false;
		}
		return true;
	}

	private void OnRefreshUserMenu(object data)
	{
		if (IsCapturable())
		{
			KIconButtonMenu.ButtonInfo button = ((!markedForCapture) ? new KIconButtonMenu.ButtonInfo("action_capture", UI.USERMENUACTIONS.CAPTURE.NAME, delegate
			{
				MarkForCapture(mark: true);
			}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CAPTURE.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_capture", UI.USERMENUACTIONS.CANCELCAPTURE.NAME, delegate
			{
				MarkForCapture(mark: false);
			}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCELCAPTURE.TOOLTIP));
			Game.Instance.userMenu.AddButton(base.gameObject, button);
		}
	}

	private void UpdateStatusItem()
	{
		shouldShowSkillPerkStatusItem = markedForCapture;
		base.UpdateStatusItem();
		if (markedForCapture)
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OrderCapture, this);
		}
		else
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.OrderCapture);
		}
	}

	private void UpdateChore()
	{
		if (markedForCapture && chore == null)
		{
			chore = new WorkChore<Capturable>(Db.Get().ChoreTypes.Capture, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: true);
		}
		else if (!markedForCapture && chore != null)
		{
			chore.Cancel("not marked for capture");
			chore = null;
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Stunned);
	}

	protected override void OnStopWork(Worker worker)
	{
		GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.Stunned);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		int num = this.NaturalBuildingCell();
		if (Grid.Solid[num])
		{
			int num2 = Grid.CellAbove(num);
			if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
			{
				num = num2;
			}
		}
		MarkForCapture(mark: false);
		baggable.SetWrangled();
		baggable.transform.SetPosition(Grid.CellToPosCCC(num, Grid.SceneLayer.Ore));
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		if (allowCapture)
		{
			descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_WRANGLE, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_WRANGLE));
		}
		return descriptors;
	}
}
