using System.Collections.Generic;
using ProcGen;
using UnityEngine;

namespace ProcGenGame
{
	public class Border : Path
	{
		public Neighbors neighbors;

		public List<WeightedSimHash> element;

		public float width;

		public Border(Neighbors neighbors, Vector2 e0, Vector2 e1)
		{
			this.neighbors = neighbors;
			Vector2 a = e1 - e0;
			Vector2 normalized = new Vector2(0f - a.y, a.x).normalized;
			Vector2 a2 = e0 + a / 2f;
			Vector2 point = a2 + normalized;
			if (neighbors.n0.poly.Contains(point))
			{
				AddSegment(e0, e1);
			}
			else
			{
				AddSegment(e1, e0);
			}
		}

		private void SetCell(int gridCell, float defaultTemperature, TerrainCell.SetValuesFunction SetValues, SeededRandom rnd)
		{
			WeightedSimHash weightedSimHash = WeightedRandom.Choose(element, rnd);
			TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(weightedSimHash.element, weightedSimHash.overrides);
			if (!elementOverride.overrideTemperature)
			{
				elementOverride.pdelement.temperature = defaultTemperature;
			}
			SetValues(gridCell, elementOverride.element, elementOverride.pdelement, elementOverride.dc);
		}

		public void ConvertToMap(Chunk world, TerrainCell.SetValuesFunction SetValues, float neighbour0Temperature, float neighbour1Temperature, float midTemp, SeededRandom rnd, int snapLastCells)
		{
			for (int i = 0; i < pathElements.Count; i++)
			{
				Vector2 vector = pathElements[i].e1 - pathElements[i].e0;
				Vector2 normalized = new Vector2(0f - vector.y, vector.x).normalized;
				List<Vector2I> line = ProcGen.Util.GetLine(pathElements[i].e0, pathElements[i].e1);
				for (int j = 0; j < line.Count; j++)
				{
					int num = Grid.XYToCell(line[j].x, line[j].y);
					if (Grid.IsValidCell(num))
					{
						SetCell(num, midTemp, SetValues, rnd);
					}
					for (float num2 = 0.5f; num2 <= width; num2 += 1f)
					{
						float num3 = Mathf.Clamp01((num2 - 0.5f) / (width - 0.5f));
						if (num2 + (float)snapLastCells > width)
						{
							num3 = 1f;
						}
						Vector2 vector2 = line[j] + normalized * num2;
						float defaultTemperature = midTemp + (neighbour0Temperature - midTemp) * num3;
						num = Grid.XYToCell((int)vector2.x, (int)vector2.y);
						if (Grid.IsValidCell(num))
						{
							SetCell(num, defaultTemperature, SetValues, rnd);
						}
						Vector2 vector3 = line[j] - normalized * num2;
						float defaultTemperature2 = midTemp + (neighbour1Temperature - midTemp) * num3;
						num = Grid.XYToCell((int)vector3.x, (int)vector3.y);
						if (Grid.IsValidCell(num))
						{
							SetCell(num, defaultTemperature2, SetValues, rnd);
						}
					}
				}
			}
		}
	}
}
