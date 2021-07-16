using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/VisibilityTester")]
public class VisibilityTester : KMonoBehaviour
{
	public static VisibilityTester Instance;

	public bool enableTesting;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	private void Update()
	{
		if (!(SelectTool.Instance == null) && !(SelectTool.Instance.selected == null) && enableTesting)
		{
			int cell = Grid.PosToCell(SelectTool.Instance.selected);
			int mouseCell = DebugHandler.GetMouseCell();
			string str = "";
			str = str + "Source Cell: " + cell + "\n";
			str = str + "Target Cell: " + mouseCell + "\n";
			str = str + "Visible: " + Grid.VisibilityTest(cell, mouseCell);
			for (int i = 0; i < 10000; i++)
			{
				Grid.VisibilityTest(cell, mouseCell);
			}
			DebugText.Instance.Draw(str, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
		}
	}
}
