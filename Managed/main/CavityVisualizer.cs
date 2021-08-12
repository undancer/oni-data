using System.Collections.Generic;
using ProcGenGame;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CavityVisualizer")]
public class CavityVisualizer : KMonoBehaviour
{
	public static CavityVisualizer Instance;

	public List<int> cavityCells = new List<int>();

	public List<int> spawnCells = new List<int>();

	public bool drawCavity = true;

	public bool drawSpawnCells = true;

	protected override void OnPrefabInit()
	{
		Debug.Assert(Instance == null);
		Instance = this;
		base.OnPrefabInit();
		foreach (TerrainCell key in MobSpawning.NaturalCavities.Keys)
		{
			foreach (HashSet<int> item in MobSpawning.NaturalCavities[key])
			{
				foreach (int item2 in item)
				{
					cavityCells.Add(item2);
				}
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (drawCavity)
		{
			Color[] array = new Color[2]
			{
				Color.blue,
				Color.yellow
			};
			int num = 0;
			foreach (TerrainCell key in MobSpawning.NaturalCavities.Keys)
			{
				Gizmos.color = array[num % array.Length];
				Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.125f);
				num++;
				foreach (HashSet<int> item in MobSpawning.NaturalCavities[key])
				{
					foreach (int item2 in item)
					{
						Gizmos.DrawCube(Grid.CellToPos(item2) + (Vector3.right / 2f + Vector3.up / 2f), Vector3.one);
					}
				}
			}
		}
		if (spawnCells == null || !drawSpawnCells)
		{
			return;
		}
		Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
		foreach (int spawnCell in spawnCells)
		{
			Gizmos.DrawCube(Grid.CellToPos(spawnCell) + (Vector3.right / 2f + Vector3.up / 2f), Vector3.one);
		}
	}
}
