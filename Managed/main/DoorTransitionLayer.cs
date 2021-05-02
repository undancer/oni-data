using System.Collections.Generic;
using UnityEngine;

public class DoorTransitionLayer : TransitionDriver.OverrideLayer
{
	private List<INavDoor> doors = new List<INavDoor>();

	private INavDoor targetDoor;

	public DoorTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private bool AreAllDoorsOpen()
	{
		foreach (INavDoor door in doors)
		{
			if (door != null && !door.IsOpen())
			{
				return false;
			}
		}
		return true;
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		int cell = Grid.PosToCell(navigator);
		int cell2 = Grid.OffsetCell(cell, transition.x, transition.y);
		AddDoor(cell2);
		if (navigator.CurrentNavType != NavType.Tube)
		{
			AddDoor(Grid.CellAbove(cell2));
		}
		for (int i = 0; i < transition.navGridTransition.voidOffsets.Length; i++)
		{
			int cell3 = Grid.OffsetCell(cell, transition.navGridTransition.voidOffsets[i]);
			AddDoor(cell3);
		}
		if (doors.Count > 0 && !AreAllDoorsOpen())
		{
			transition.anim = navigator.NavGrid.GetIdleAnim(navigator.CurrentNavType);
			transition.isLooping = false;
			transition.end = transition.start;
			transition.speed = 1f;
			transition.animSpeed = 1f;
			transition.x = 0;
			transition.y = 0;
			transition.isCompleteCB = () => AreAllDoorsOpen();
		}
		foreach (INavDoor door in doors)
		{
			door.Open();
		}
	}

	public override void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.UpdateTransition(navigator, transition);
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		foreach (INavDoor door in doors)
		{
			if (!door.IsNullOrDestroyed())
			{
				door.Close();
			}
		}
		doors.Clear();
	}

	private void AddDoor(int cell)
	{
		INavDoor door = GetDoor(cell);
		if (!door.IsNullOrDestroyed() && !doors.Contains(door))
		{
			doors.Add(door);
		}
	}

	private INavDoor GetDoor(int cell)
	{
		if (!Grid.HasDoor[cell])
		{
			return null;
		}
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null)
		{
			INavDoor navDoor = gameObject.GetComponent<INavDoor>();
			if (navDoor == null)
			{
				navDoor = gameObject.GetSMI<INavDoor>();
			}
			if (navDoor?.isSpawned ?? false)
			{
				return navDoor;
			}
		}
		return null;
	}
}
