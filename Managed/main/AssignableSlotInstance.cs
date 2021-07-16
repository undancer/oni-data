using UnityEngine;

public abstract class AssignableSlotInstance
{
	public AssignableSlot slot;

	public Assignable assignable;

	private bool unassigning;

	public Assignables assignables
	{
		get;
		private set;
	}

	public GameObject gameObject => assignables.gameObject;

	public AssignableSlotInstance(Assignables assignables, AssignableSlot slot)
	{
		this.slot = slot;
		this.assignables = assignables;
	}

	public void Assign(Assignable assignable)
	{
		if (!(this.assignable == assignable))
		{
			Unassign(trigger_event: false);
			this.assignable = assignable;
			assignables.Trigger(-1585839766, this);
		}
	}

	public virtual void Unassign(bool trigger_event = true)
	{
		if (!unassigning && IsAssigned())
		{
			unassigning = true;
			assignable.Unassign();
			if (trigger_event)
			{
				assignables.Trigger(-1585839766, this);
			}
			assignable = null;
			unassigning = false;
		}
	}

	public bool IsAssigned()
	{
		return assignable != null;
	}

	public bool IsUnassigning()
	{
		return unassigning;
	}
}
