using UnityEngine;

public class WorkableReactable : Reactable
{
	public enum AllowedDirection
	{
		Any,
		Left,
		Right
	}

	protected Workable workable;

	private Worker worker;

	public AllowedDirection allowedDirection;

	public WorkableReactable(Workable workable, HashedString id, ChoreType chore_type, AllowedDirection allowed_direction = AllowedDirection.Any)
		: base(workable.gameObject, id, chore_type, 1, 1)
	{
		this.workable = workable;
		allowedDirection = allowed_direction;
	}

	public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
	{
		if (workable == null)
		{
			return false;
		}
		if (reactor != null)
		{
			return false;
		}
		Brain component = new_reactor.GetComponent<Brain>();
		if (component == null)
		{
			return false;
		}
		if (!component.IsRunning())
		{
			return false;
		}
		Navigator component2 = new_reactor.GetComponent<Navigator>();
		if (component2 == null)
		{
			return false;
		}
		if (!component2.IsMoving())
		{
			return false;
		}
		if (allowedDirection == AllowedDirection.Any)
		{
			return true;
		}
		Facing component3 = new_reactor.GetComponent<Facing>();
		if (component3 == null)
		{
			return false;
		}
		bool facing = component3.GetFacing();
		if (facing && allowedDirection == AllowedDirection.Right)
		{
			return false;
		}
		if (!facing && allowedDirection == AllowedDirection.Left)
		{
			return false;
		}
		return true;
	}

	protected override void InternalBegin()
	{
		worker = reactor.GetComponent<Worker>();
		worker.StartWork(new Worker.StartWorkInfo(workable));
	}

	public override void Update(float dt)
	{
		if (worker.workable == null)
		{
			End();
		}
		else if (worker.Work(dt) != Worker.WorkResult.InProgress)
		{
			End();
		}
	}

	protected override void InternalEnd()
	{
		if (worker != null)
		{
			worker.StopWork();
		}
	}

	protected override void InternalCleanup()
	{
	}
}
