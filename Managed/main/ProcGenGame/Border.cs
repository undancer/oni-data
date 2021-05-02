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
			Vector2 vector = neighbors.n1.site.position - neighbors.n0.site.position;
			Vector2 vector2 = e0 - neighbors.n0.site.position;
			float num = vector.x * vector2.y - vector2.x * vector.y;
			if (num < 0f)
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

		public void ConvertToMap(Chunk world, TerrainCell.SetValuesFunction SetValues, float neighbour0Temperature, float neighbour1Temperature, float temperatureRange, SeededRandom rnd, int snapLastCells)
		{
			for (int i = 0; i < pathElements.Count; i++)
			{
				Vector2 vector = pathElements[i].e1 - pathElements[i].e0;
				Vector2 normalized = new Vector2(0f - vector.y, vector.x).normalized;
				float num = (neighbour0Temperature + neighbour1Temperature) / 2f;
				List<Vector2I> line = ProcGen.Util.GetLine(pathElements[i].e0, pathElements[i].e1);
				for (int j = 0; j < line.Count; j++)
				{
					int num2 = Grid.XYToCell(line[j].x, line[j].y);
					if (Grid.IsValidCell(num2))
					{
						SetCell(num2, num, SetValues, rnd);
					}
					for (float num3 = 0.5f; num3 <= width; num3 += 1f)
					{
						float num4 = Mathf.Clamp01((num3 - 0.5f) / (width - 0.5f));
						if (num3 + (float)snapLastCells > width)
						{
							num4 = 1f;
						}
						Vector2 vector2 = line[j] + normalized * num3;
						float defaultTemperature = num + (neighbour0Temperature - num) * num4;
						num2 = Grid.XYToCell((int)vector2.x, (int)vector2.y);
						if (Grid.IsValidCell(num2))
						{
							SetCell(num2, defaultTemperature, SetValues, rnd);
						}
						Vector2 vector3 = line[j] - normalized * num3;
						float defaultTemperature2 = num + (neighbour1Temperature - num) * num4;
						num2 = Grid.XYToCell((int)vector3.x, (int)vector3.y);
						if (Grid.IsValidCell(num2))
						{
							SetCell(num2, defaultTemperature2, SetValues, rnd);
						}
					}
				}
			}
		}
	}
}
