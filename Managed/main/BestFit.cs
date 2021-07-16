using System;
using System.Collections.Generic;
using ProcGen;
using TUNING;

public class BestFit
{
	private struct Rect
	{
		private int x;

		private int y;

		private int width;

		private int height;

		public int X1 => x;

		public int X2 => x + width + 2;

		public int Y1 => y;

		public int Y2 => y + height + 2;

		public Rect(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
	}

	public static Vector2I BestFitWorlds(List<WorldPlacement> worldsToArrange, bool ignoreBestFitY = false)
	{
		List<Rect> list = new List<Rect>();
		Vector2I result = default(Vector2I);
		worldsToArrange.Sort((WorldPlacement a, WorldPlacement b) => b.height.CompareTo(a.height));
		int height = worldsToArrange[0].height;
		foreach (WorldPlacement item in worldsToArrange)
		{
			Vector2I position = default(Vector2I);
			while (!UnoccupiedSpace(new Rect(position.x, position.y, item.width, item.height), list))
			{
				if (ignoreBestFitY)
				{
					position.x++;
				}
				else if (position.y + item.height >= height + 32)
				{
					position.y = 0;
					position.x++;
				}
				else
				{
					position.y++;
				}
			}
			result.x = Math.Max(item.width + position.x, result.x);
			result.y = Math.Max(item.height + position.y, result.y);
			list.Add(new Rect(position.x, position.y, item.width, item.height));
			item.SetPosition(position);
		}
		result.x += 136;
		result.y = Math.Max(result.y, 136);
		return result;
	}

	private static bool UnoccupiedSpace(Rect RectA, List<Rect> placed)
	{
		foreach (Rect item in placed)
		{
			if (RectA.X1 < item.X2 && RectA.X2 > item.X1 && RectA.Y1 < item.Y2 && RectA.Y2 > item.Y1)
			{
				return false;
			}
		}
		return true;
	}

	public static Vector2I GetGridOffset(IList<WorldContainer> existingWorlds, Vector2I newWorldSize, out Vector2I newWorldOffset)
	{
		List<Rect> list = new List<Rect>();
		foreach (WorldContainer existingWorld in existingWorlds)
		{
			list.Add(new Rect(existingWorld.WorldOffset.x, existingWorld.WorldOffset.y, existingWorld.WorldSize.x, existingWorld.WorldSize.y));
		}
		Vector2I result = new Vector2I(Grid.WidthInCells, 0);
		int widthInCells = Grid.WidthInCells;
		Vector2I vector2I = default(Vector2I);
		while (!UnoccupiedSpace(new Rect(vector2I.x, vector2I.y, newWorldSize.x, newWorldSize.y), list))
		{
			if (vector2I.x + newWorldSize.x >= widthInCells)
			{
				vector2I.x = 0;
				vector2I.y++;
			}
			else
			{
				vector2I.x++;
			}
		}
		Debug.Assert(vector2I.x + newWorldSize.x <= Grid.WidthInCells, "BestFit is trying to expand the grid width, this is unsupported and will break the SIM.");
		result.y = Math.Max(newWorldSize.y + vector2I.y, Grid.HeightInCells);
		newWorldOffset = vector2I;
		return result;
	}

	public static int CountRocketInteriors(IList<WorldContainer> existingWorlds)
	{
		int num = 0;
		List<Rect> list = new List<Rect>();
		foreach (WorldContainer existingWorld in existingWorlds)
		{
			list.Add(new Rect(existingWorld.WorldOffset.x, existingWorld.WorldOffset.y, existingWorld.WorldSize.x, existingWorld.WorldSize.y));
		}
		Vector2I rOCKET_INTERIOR_SIZE = ROCKETRY.ROCKET_INTERIOR_SIZE;
		Vector2I newWorldOffset;
		while (PlaceWorld(list, rOCKET_INTERIOR_SIZE, out newWorldOffset))
		{
			num++;
			list.Add(new Rect(newWorldOffset.x, newWorldOffset.y, rOCKET_INTERIOR_SIZE.x, rOCKET_INTERIOR_SIZE.y));
		}
		return num;
	}

	private static bool PlaceWorld(List<Rect> placedWorlds, Vector2I newWorldSize, out Vector2I newWorldOffset)
	{
		Vector2I vector2I = new Vector2I(Grid.WidthInCells, 0);
		int widthInCells = Grid.WidthInCells;
		Vector2I vector2I2 = default(Vector2I);
		while (!UnoccupiedSpace(new Rect(vector2I2.x, vector2I2.y, newWorldSize.x, newWorldSize.y), placedWorlds))
		{
			if (vector2I2.x + newWorldSize.x >= widthInCells)
			{
				vector2I2.x = 0;
				vector2I2.y++;
			}
			else
			{
				vector2I2.x++;
			}
		}
		vector2I.y = Math.Max(newWorldSize.y + vector2I2.y, Grid.HeightInCells);
		newWorldOffset = vector2I2;
		if (vector2I2.x + newWorldSize.x <= Grid.WidthInCells)
		{
			return vector2I2.y + newWorldSize.y <= Grid.HeightInCells;
		}
		return false;
	}
}
