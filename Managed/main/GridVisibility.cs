using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/GridVisibility")]
public class GridVisibility : KMonoBehaviour
{
	public int radius = 18;

	public float innerRadius = 16.5f;

	protected override void OnSpawn()
	{
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "GridVisibility.OnSpawn");
		OnCellChange();
	}

	private void OnCellChange()
	{
		if (base.gameObject.HasTag(GameTags.Dead))
		{
			return;
		}
		int num = Grid.PosToCell(this);
		if (Grid.IsValidCell(num))
		{
			if (!Grid.Revealed[num])
			{
				Grid.PosToXY(base.transform.GetPosition(), out var x, out var y);
				Reveal(x, y, radius, innerRadius);
				Grid.Revealed[num] = true;
			}
			FogOfWarMask.ClearMask(num);
		}
	}

	public static void Reveal(int baseX, int baseY, int radius, float innerRadius)
	{
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				int num = baseY + i;
				int num2 = baseX + j;
				if (num >= 0 && Grid.HeightInCells - 1 >= num && num2 >= 0 && Grid.WidthInCells - 1 >= num2)
				{
					int num3 = num * Grid.WidthInCells + num2;
					if (Grid.Visible[num3] < byte.MaxValue)
					{
						float num4 = Mathf.Lerp(1f, 0f, (new Vector2(j, i).magnitude - innerRadius) / ((float)radius - innerRadius));
						Grid.Reveal(num3, (byte)(255f * num4));
					}
				}
			}
		}
		int num5 = Mathf.CeilToInt(radius);
		Game.Instance.UpdateGameActiveRegion(baseX - num5, baseY - num5, baseX + num5, baseY + num5);
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
	}
}
