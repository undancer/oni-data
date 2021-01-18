using System.Collections.Generic;
using UnityEngine;

public static class CreatureHelpers
{
	public static bool isClear(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		if (Grid.Solid[cell])
		{
			return false;
		}
		if (Grid.IsSubstantialLiquid(cell, 0.9f) || (Grid.IsValidCell(Grid.CellBelow(cell)) && Grid.IsLiquid(cell) && Grid.IsLiquid(Grid.CellBelow(cell))))
		{
			return false;
		}
		return true;
	}

	public static int FindNearbyBreathableCell(int currentLocation, SimHashes breathableElement)
	{
		return currentLocation;
	}

	public static bool cellsAreClear(int[] cells)
	{
		for (int i = 0; i < cells.Length; i++)
		{
			if (!Grid.IsValidCell(cells[i]))
			{
				return false;
			}
			if (!isClear(cells[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static Vector3 PositionOfCurrentCell(Vector3 transformPosition)
	{
		return Grid.CellToPos(Grid.PosToCell(transformPosition));
	}

	public static Vector3 CenterPositionOfCell(int cell)
	{
		return Grid.CellToPos(cell) + new Vector3(0.5f, 0.5f, -2f);
	}

	public static void DeselectCreature(GameObject creature)
	{
		KSelectable component = creature.GetComponent<KSelectable>();
		if (component != null && SelectTool.Instance.selected == component)
		{
			SelectTool.Instance.Select(null);
		}
	}

	public static bool isSwimmable(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		if (Grid.Solid[cell])
		{
			return false;
		}
		if (!Grid.IsSubstantialLiquid(cell))
		{
			return false;
		}
		return true;
	}

	public static bool isSolidGround(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		if (Grid.Solid[cell])
		{
			return true;
		}
		return false;
	}

	public static void FlipAnim(KAnimControllerBase anim, Vector3 heading)
	{
		if (heading.x < 0f)
		{
			anim.FlipX = true;
		}
		else if (heading.x > 0f)
		{
			anim.FlipX = false;
		}
	}

	public static void FlipAnim(KBatchedAnimController anim, Vector3 heading)
	{
		if (heading.x < 0f)
		{
			anim.FlipX = true;
		}
		else if (heading.x > 0f)
		{
			anim.FlipX = false;
		}
	}

	public static Vector3 GetWalkMoveTarget(Transform transform, Vector2 Heading)
	{
		int cell = Grid.PosToCell(transform.GetPosition());
		if (Heading.x == 1f)
		{
			if (isClear(Grid.CellRight(cell)) && isClear(Grid.CellDownRight(cell)) && isClear(Grid.CellRight(Grid.CellRight(cell))) && !isClear(Grid.PosToCell(transform.GetPosition() + Vector3.right * 2f + Vector3.down)))
			{
				return transform.GetPosition() + Vector3.right * 2f;
			}
			if (cellsAreClear(new int[2]
			{
				Grid.CellRight(cell),
				Grid.CellDownRight(cell)
			}) && !isClear(Grid.CellBelow(Grid.CellDownRight(cell))))
			{
				return transform.GetPosition() + Vector3.right + Vector3.down;
			}
			if (cellsAreClear(new int[3]
			{
				Grid.OffsetCell(cell, 1, 0),
				Grid.OffsetCell(cell, 1, -1),
				Grid.OffsetCell(cell, 1, -2)
			}) && !isClear(Grid.OffsetCell(cell, 1, -3)))
			{
				return transform.GetPosition() + Vector3.right + Vector3.down + Vector3.down;
			}
			if (cellsAreClear(new int[4]
			{
				Grid.OffsetCell(cell, 1, 0),
				Grid.OffsetCell(cell, 1, -1),
				Grid.OffsetCell(cell, 1, -2),
				Grid.OffsetCell(cell, 1, -3)
			}))
			{
				return transform.GetPosition();
			}
			if (isClear(Grid.CellRight(cell)))
			{
				return transform.GetPosition() + Vector3.right;
			}
			if (isClear(Grid.CellUpRight(cell)) && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellRight(cell)])
			{
				return transform.GetPosition() + Vector3.up + Vector3.right;
			}
			if (!Grid.Solid[Grid.CellAbove(cell)] && !Grid.Solid[Grid.CellAbove(Grid.CellAbove(cell))] && Grid.Solid[Grid.CellAbove(Grid.CellRight(cell))] && isClear(Grid.CellRight(Grid.CellAbove(Grid.CellAbove(cell)))))
			{
				return transform.GetPosition() + Vector3.up + Vector3.up + Vector3.right;
			}
		}
		if (Heading.x == -1f)
		{
			if (isClear(Grid.CellLeft(cell)) && isClear(Grid.CellDownLeft(cell)) && isClear(Grid.CellLeft(Grid.CellLeft(cell))) && !isClear(Grid.PosToCell(transform.GetPosition() + Vector3.left * 2f + Vector3.down)))
			{
				return transform.GetPosition() + Vector3.left * 2f;
			}
			if (cellsAreClear(new int[2]
			{
				Grid.CellLeft(cell),
				Grid.CellDownLeft(cell)
			}) && !isClear(Grid.CellBelow(Grid.CellDownLeft(cell))))
			{
				return transform.GetPosition() + Vector3.left + Vector3.down;
			}
			if (cellsAreClear(new int[3]
			{
				Grid.OffsetCell(cell, -1, 0),
				Grid.OffsetCell(cell, -1, -1),
				Grid.OffsetCell(cell, -1, -2)
			}) && !isClear(Grid.OffsetCell(cell, -1, -3)))
			{
				return transform.GetPosition() + Vector3.left + Vector3.down + Vector3.down;
			}
			if (cellsAreClear(new int[4]
			{
				Grid.OffsetCell(cell, -1, 0),
				Grid.OffsetCell(cell, -1, -1),
				Grid.OffsetCell(cell, -1, -2),
				Grid.OffsetCell(cell, -1, -3)
			}))
			{
				return transform.GetPosition();
			}
			if (isClear(Grid.CellLeft(Grid.PosToCell(transform.GetPosition()))))
			{
				return transform.GetPosition() + Vector3.left;
			}
			if (isClear(Grid.CellUpLeft(cell)) && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellLeft(cell)])
			{
				return transform.GetPosition() + Vector3.up + Vector3.left;
			}
			if (!Grid.Solid[Grid.CellAbove(cell)] && !Grid.Solid[Grid.CellAbove(Grid.CellAbove(cell))] && Grid.Solid[Grid.CellAbove(Grid.CellLeft(cell))] && isClear(Grid.CellLeft(Grid.CellAbove(Grid.CellAbove(cell)))))
			{
				return transform.GetPosition() + Vector3.up + Vector3.up + Vector3.left;
			}
		}
		return transform.GetPosition();
	}

	public static bool CrewNearby(Transform transform, int range = 6)
	{
		int cell = Grid.PosToCell(transform.gameObject);
		for (int i = 1; i < range; i++)
		{
			int cell2 = Grid.OffsetCell(cell, i, 0);
			int cell3 = Grid.OffsetCell(cell, -i, 0);
			if (Grid.Objects[cell2, 0] != null)
			{
				return true;
			}
			if (Grid.Objects[cell3, 0] != null)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckHorizontalClear(Vector3 startPosition, Vector3 endPosition)
	{
		int cell = Grid.PosToCell(startPosition);
		int num = 1;
		if (endPosition.x < startPosition.x)
		{
			num = -1;
		}
		float num2 = Mathf.Abs(endPosition.x - startPosition.x);
		for (int i = 0; (float)i < num2; i++)
		{
			int i2 = Grid.OffsetCell(cell, i * num, 0);
			if (Grid.Solid[i2])
			{
				return false;
			}
		}
		return true;
	}

	public static GameObject GetFleeTargetLocatorObject(GameObject self, GameObject threat)
	{
		if (threat == null)
		{
			Debug.LogWarning(self.name + " is trying to flee, bus has no threats");
			return null;
		}
		int num = Grid.PosToCell(threat);
		int num2 = Grid.PosToCell(self);
		Navigator nav = self.GetComponent<Navigator>();
		if (nav == null)
		{
			Debug.LogWarning(self.name + " is trying to flee, bus has no navigator component attached.");
			return null;
		}
		HashSet<int> hashSet = GameUtil.FloodCollectCells(Grid.PosToCell(self), (int cell) => CanFleeTo(cell, nav));
		int num3 = -1;
		int num4 = -1;
		foreach (int item in hashSet)
		{
			if (nav.CanReach(item) && item != num2)
			{
				int num5 = -1;
				num5 += Grid.GetCellDistance(item, num);
				if (isInFavoredFleeDirection(item, num, self))
				{
					num5 += 2;
				}
				if (num5 > num4)
				{
					num4 = num5;
					num3 = item;
				}
			}
		}
		if (num3 != -1)
		{
			return ChoreHelpers.CreateLocator("GoToLocator", Grid.CellToPos(num3));
		}
		return null;
	}

	private static bool isInFavoredFleeDirection(int targetFleeCell, int threatCell, GameObject self)
	{
		bool flag = ((Grid.CellToPos(threatCell).x < self.transform.GetPosition().x) ? true : false);
		bool flag2 = ((Grid.CellToPos(threatCell).x < Grid.CellToPos(targetFleeCell).x) ? true : false);
		return flag == flag2;
	}

	private static bool CanFleeTo(int cell, Navigator nav)
	{
		return nav.CanReach(cell) || nav.CanReach(Grid.OffsetCell(cell, -1, -1)) || nav.CanReach(Grid.OffsetCell(cell, 1, -1)) || nav.CanReach(Grid.OffsetCell(cell, -1, 1)) || nav.CanReach(Grid.OffsetCell(cell, 1, 1));
	}
}
