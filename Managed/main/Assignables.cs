using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Assignables")]
public class Assignables : KMonoBehaviour
{
	protected List<AssignableSlotInstance> slots = new List<AssignableSlotInstance>();

	private static readonly EventSystem.IntraObjectHandler<Assignables> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler(GameTags.Dead, delegate(Assignables component, object data)
	{
		component.OnDeath(data);
	});

	public List<AssignableSlotInstance> Slots => slots;

	protected IAssignableIdentity GetAssignableIdentity()
	{
		MinionIdentity component = GetComponent<MinionIdentity>();
		if (component != null)
		{
			return component.assignableProxy.Get();
		}
		return GetComponent<MinionAssignablesProxy>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameUtil.SubscribeToTags(this, OnDeadTagAddedDelegate, triggerImmediately: true);
	}

	private void OnDeath(object data)
	{
		foreach (AssignableSlotInstance slot in slots)
		{
			slot.Unassign();
		}
	}

	public void Add(AssignableSlotInstance slot_instance)
	{
		slots.Add(slot_instance);
	}

	public Assignable GetAssignable(AssignableSlot slot)
	{
		return GetSlot(slot)?.assignable;
	}

	public AssignableSlotInstance GetSlot(AssignableSlot slot)
	{
		Debug.Assert(slots.Count > 0, "GetSlot called with no slots configured");
		if (slot == null)
		{
			return null;
		}
		foreach (AssignableSlotInstance slot2 in slots)
		{
			if (slot2.slot == slot)
			{
				return slot2;
			}
		}
		return null;
	}

	public Assignable AutoAssignSlot(AssignableSlot slot)
	{
		Assignable assignable = GetAssignable(slot);
		if (assignable != null)
		{
			return assignable;
		}
		GameObject targetGameObject = GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
		if (targetGameObject == null)
		{
			Debug.LogWarning("AutoAssignSlot failed, proxy game object was null.");
			return null;
		}
		Navigator component = targetGameObject.GetComponent<Navigator>();
		IAssignableIdentity assignableIdentity = GetAssignableIdentity();
		int num = int.MaxValue;
		foreach (Assignable item in Game.Instance.assignmentManager)
		{
			if (!(item == null) && !item.IsAssigned() && item.slot == slot && item.CanAutoAssignTo(assignableIdentity))
			{
				int navigationCost = item.GetNavigationCost(component);
				if (navigationCost != -1 && navigationCost < num)
				{
					num = navigationCost;
					assignable = item;
				}
			}
		}
		if (assignable != null)
		{
			assignable.Assign(assignableIdentity);
		}
		return assignable;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		foreach (AssignableSlotInstance slot in slots)
		{
			slot.Unassign();
		}
	}
}
