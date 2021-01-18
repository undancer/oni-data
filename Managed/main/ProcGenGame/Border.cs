using System.Collections.Generic;
using ProcGen;
using UnityEngine;

namespace ProcGenGame
{
	public class Border : Path, SymbolicMapElement
	{
		public Neighbors neighbors;

		public List<WeightedSimHash> element;

		public float width;

		public Border(Neighbors neighbors, Vector2 e0, Vector2 e1)
		{
			this.neighbors = neighbors;
			AddSegment(e0, e1);
		}

		public Border(TerrainCell a, TerrainCell b, Vector2 e0, Vector2 e1)
		{
			Debug.Assert(a != null && b != null, "NULL neighbor for Border");
			neighbors.n0 = a;
			neighbors.n1 = b;
			AddSegment(e0, e1);
		}

		public void ConvertToMap(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			Sim.DiseaseCell invalid = Sim.DiseaseCell.Invalid;
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
						Element element = ElementLoader.FindElementByName(WeightedRandom.Choose(this.element, rnd).element);
						Sim.PhysicsData defaultValues = element.defaultValues;
						defaultValues.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
						SetValues(num, element, defaultValues, invalid);
					}
					for (float num2 = 0.5f; num2 <= width; num2 += 1f)
					{
						Vector2 vector2 = line[j] + normalized * num2;
						num = Grid.XYToCell((int)vector2.x, (int)vector2.y);
						if (Grid.IsValidCell(num))
						{
							Element element2 = ElementLoader.FindElementByName(WeightedRandom.Choose(this.element, rnd).element);
							Sim.PhysicsData defaultValues2 = element2.defaultValues;
							defaultValues2.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
							SetValues(num, element2, defaultValues2, invalid);
						}
						Vector2 vector3 = line[j] - normalized * num2;
						num = Grid.XYToCell((int)vector3.x, (int)vector3.y);
						if (Grid.IsValidCell(num))
						{
							Element element3 = ElementLoader.FindElementByName(WeightedRandom.Choose(this.element, rnd).element);
							Sim.PhysicsData defaultValues3 = element3.defaultValues;
							defaultValues3.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
							SetValues(num, element3, defaultValues3, invalid);
						}
					}
				}
			}
		}
	}
}
