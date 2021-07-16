using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DebugCellDrawer")]
public class DebugCellDrawer : KMonoBehaviour
{
	public List<int> cells;

	private void Update()
	{
		for (int i = 0; i < cells.Count; i++)
		{
			if (cells[i] != PathFinder.InvalidCell)
			{
				DebugExtension.DebugPoint(Grid.CellToPosCCF(cells[i], Grid.SceneLayer.Background));
			}
		}
	}
}
