using UnityEngine;

public class BalloonStandCellSensor : Sensor
{
	private MinionBrain brain;

	private Navigator navigator;

	private int cell;

	private int standCell;

	public BalloonStandCellSensor(Sensors sensors)
		: base(sensors)
	{
		navigator = GetComponent<Navigator>();
		brain = GetComponent<MinionBrain>();
	}

	public override void Update()
	{
		cell = Grid.InvalidCell;
		int num = int.MaxValue;
		ListPool<int[], BalloonStandCellSensor>.PooledList pooledList = ListPool<int[], BalloonStandCellSensor>.Allocate();
		int num2 = 50;
		foreach (int mingleCell in Game.Instance.mingleCellTracker.mingleCells)
		{
			if (!brain.IsCellClear(mingleCell))
			{
				continue;
			}
			int navigationCost = navigator.GetNavigationCost(mingleCell);
			if (navigationCost == -1)
			{
				continue;
			}
			if (mingleCell == Grid.InvalidCell || navigationCost < num)
			{
				cell = mingleCell;
				num = navigationCost;
			}
			if (navigationCost >= num2)
			{
				continue;
			}
			int num3 = Grid.CellRight(mingleCell);
			int num4 = Grid.CellRight(num3);
			int num5 = Grid.CellLeft(mingleCell);
			int num6 = Grid.CellLeft(num5);
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
			CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(num6);
			CavityInfo cavityForCell3 = Game.Instance.roomProber.GetCavityForCell(num4);
			if (cavityForCell != null)
			{
				if (cavityForCell3 != null && cavityForCell3.handle == cavityForCell.handle && navigator.NavGrid.NavTable.IsValid(num3) && navigator.NavGrid.NavTable.IsValid(num4))
				{
					pooledList.Add(new int[2]
					{
						mingleCell,
						num4
					});
				}
				if (cavityForCell2 != null && cavityForCell2.handle == cavityForCell.handle && navigator.NavGrid.NavTable.IsValid(num5) && navigator.NavGrid.NavTable.IsValid(num6))
				{
					pooledList.Add(new int[2]
					{
						mingleCell,
						num6
					});
				}
			}
		}
		if (pooledList.Count > 0)
		{
			int[] array = pooledList[Random.Range(0, pooledList.Count)];
			cell = array[0];
			standCell = array[1];
		}
		else if (Components.Telepads.Count > 0)
		{
			Telepad telepad = Components.Telepads.Items[0];
			if (telepad == null || !telepad.GetComponent<Operational>().IsOperational)
			{
				return;
			}
			int num7 = Grid.PosToCell(telepad.transform.GetPosition());
			num7 = Grid.CellLeft(num7);
			int num8 = Grid.CellRight(num7);
			int num9 = Grid.CellRight(num8);
			CavityInfo cavityForCell4 = Game.Instance.roomProber.GetCavityForCell(num7);
			CavityInfo cavityForCell5 = Game.Instance.roomProber.GetCavityForCell(num9);
			if (cavityForCell4 != null && cavityForCell5 != null && navigator.NavGrid.NavTable.IsValid(num7) && navigator.NavGrid.NavTable.IsValid(num8) && navigator.NavGrid.NavTable.IsValid(num9))
			{
				cell = num7;
				standCell = num9;
			}
		}
		pooledList.Recycle();
	}

	public int GetCell()
	{
		return cell;
	}

	public int GetStandCell()
	{
		return standCell;
	}
}
