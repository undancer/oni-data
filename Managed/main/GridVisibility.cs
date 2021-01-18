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
		base.gameObject.GetMyWorld().SetDiscovered();
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
		int num = Grid.WorldIdx[baseY * Grid.WidthInCells + baseX];
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				int num2 = baseY + i;
				int num3 = baseX + j;
				if (num2 >= 0 && Grid.HeightInCells - 1 >= num2 && num3 >= 0 && Grid.WidthInCells - 1 >= num3)
				{
					int num4 = num2 * Grid.WidthInCells + num3;
					byte b = Grid.Visible[num4];
					if (b < byte.MaxValue && num == Grid.WorldIdx[num4])
					{
						float num5 = Mathf.Lerp(1f, 0f, (new Vector2(j, i).magnitude - innerRadius) / ((float)radius - innerRadius));
						Grid.Reveal(num4, (byte)(255f * num5));
					}
				}
			}
		}
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
	}
}
