using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FactionAlignment")]
public class FactionAlignment : KMonoBehaviour
{
	[SerializeField]
	public bool canBePlayerTargeted = true;

	[SerializeField]
	public bool updatePrioritizable = true;

	[Serialize]
	private bool alignmentActive = true;

	public FactionManager.FactionID Alignment;

	[Serialize]
	private bool targeted;

	[Serialize]
	private bool targetable = true;

	private static readonly EventSystem.IntraObjectHandler<FactionAlignment> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler(GameTags.Dead, delegate(FactionAlignment component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<FactionAlignment> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<FactionAlignment>(delegate(FactionAlignment component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<FactionAlignment> SetPlayerTargetedFalseDelegate = new EventSystem.IntraObjectHandler<FactionAlignment>(delegate(FactionAlignment component, object data)
	{
		component.SetPlayerTargeted(state: false);
	});

	[MyCmpAdd]
	public Health health { get; private set; }

	public AttackableBase attackable { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		health = GetComponent<Health>();
		attackable = GetComponent<AttackableBase>();
		Components.FactionAlignments.Add(this);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(2127324410, SetPlayerTargetedFalseDelegate);
		if (alignmentActive)
		{
			FactionManager.Instance.GetFaction(Alignment).Members.Add(this);
		}
		GameUtil.SubscribeToTags(this, OnDeadTagAddedDelegate, triggerImmediately: true);
		UpdateStatusItem();
	}

	protected override void OnPrefabInit()
	{
	}

	private void OnDeath(object data)
	{
		SetAlignmentActive(active: false);
	}

	public void SetAlignmentActive(bool active)
	{
		SetPlayerTargetable(active);
		alignmentActive = active;
		if (active)
		{
			FactionManager.Instance.GetFaction(Alignment).Members.Add(this);
		}
		else
		{
			FactionManager.Instance.GetFaction(Alignment).Members.Remove(this);
		}
	}

	public bool IsAlignmentActive()
	{
		return FactionManager.Instance.GetFaction(Alignment).Members.Contains(this);
	}

	public bool IsPlayerTargeted()
	{
		return targeted;
	}

	public void SetPlayerTargetable(bool state)
	{
		targetable = state && canBePlayerTargeted;
		if (!state)
		{
			SetPlayerTargeted(state: false);
		}
	}

	public void SetPlayerTargeted(bool state)
	{
		targeted = canBePlayerTargeted && state && targetable;
		UpdateStatusItem();
	}

	private void UpdateStatusItem()
	{
		TogglePrioritizable(targeted);
		if (targeted)
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OrderAttack);
		}
		else
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.OrderAttack);
		}
	}

	private void TogglePrioritizable(bool enable)
	{
		Prioritizable component = GetComponent<Prioritizable>();
		if (!(component == null) && updatePrioritizable)
		{
			if (enable)
			{
				Prioritizable.AddRef(base.gameObject);
			}
			else if (component.IsPrioritizable())
			{
				Prioritizable.RemoveRef(base.gameObject);
			}
		}
	}

	public void SwitchAlignment(FactionManager.FactionID newAlignment)
	{
		SetAlignmentActive(active: false);
		Alignment = newAlignment;
		SetAlignmentActive(active: true);
	}

	protected override void OnCleanUp()
	{
		Components.FactionAlignments.Remove(this);
		FactionManager.Instance.GetFaction(Alignment).Members.Remove(this);
		base.OnCleanUp();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (Alignment != 0 && canBePlayerTargeted && IsAlignmentActive())
		{
			KIconButtonMenu.ButtonInfo button = ((!targeted) ? new KIconButtonMenu.ButtonInfo("action_attack", UI.USERMENUACTIONS.ATTACK.NAME, delegate
			{
				SetPlayerTargeted(state: true);
			}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.ATTACK.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_attack", UI.USERMENUACTIONS.CANCELATTACK.NAME, delegate
			{
				SetPlayerTargeted(state: false);
			}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCELATTACK.TOOLTIP));
			Game.Instance.userMenu.AddButton(base.gameObject, button);
		}
	}
}
