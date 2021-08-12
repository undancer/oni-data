using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RadiationGridEmitter
{
	private static int MAX_EMIT_DISTANCE = 128;

	public int originCell = -1;

	public int intensity = 1;

	public int projectionCount = 20;

	public int direction;

	public int angle = 360;

	public bool enabled;

	private HashSet<int> scanCells = new HashSet<int>();

	public RadiationGridEmitter(int originCell, int intensity)
	{
		this.originCell = originCell;
		this.intensity = intensity;
	}

	public void Emit()
	{
		scanCells.Clear();
		Vector2 vector = Grid.CellToPosCCC(originCell, Grid.SceneLayer.Building);
		for (float num = (float)direction - (float)angle / 2f; num < (float)direction + (float)angle / 2f; num += (float)(angle / projectionCount))
		{
			float num2 = UnityEngine.Random.Range((float)(-angle / projectionCount) / 2f, (float)(angle / projectionCount) / 2f);
			Vector2 vector2 = new Vector2(Mathf.Cos((num + num2) * (float)Math.PI / 180f), Mathf.Sin((num + num2) * (float)Math.PI / 180f));
			int num3 = 3;
			float num4 = intensity / 4;
			Vector2 vector3 = vector2;
			float num5 = 0f;
			while ((double)num4 > 0.01 && num5 < (float)MAX_EMIT_DISTANCE)
			{
				num5 += 1f / (float)num3;
				int num6 = Grid.PosToCell(vector + vector3 * num5);
				if (!Grid.IsValidCell(num6))
				{
					break;
				}
				if (!scanCells.Contains(num6))
				{
					SimMessages.ModifyRadiationOnCell(num6, Mathf.RoundToInt(num4));
					scanCells.Add(num6);
				}
				num4 *= Mathf.Max(0f, 1f - Mathf.Pow(Grid.Mass[num6], 1.25f) * Grid.Element[num6].molarMass / 1000000f);
				num4 *= UnityEngine.Random.Range(0.96f, 0.98f);
			}
		}
	}

	private int CalculateFalloff(float falloffRate, int cell, int origin)
	{
		return Mathf.Max(1, Mathf.RoundToInt(falloffRate * (float)Mathf.Max(Grid.GetCellDistance(origin, cell), 1)));
	}
}
