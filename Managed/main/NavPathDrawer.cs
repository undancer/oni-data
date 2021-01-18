using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NavPathDrawer")]
public class NavPathDrawer : KMonoBehaviour
{
	private PathFinder.Path path;

	public Material material;

	private Vector3 navigatorPos;

	private Navigator navigator;

	public static NavPathDrawer Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Shader shader = Shader.Find("Lines/Colored Blended");
		material = new Material(shader);
		Instance = this;
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	public void DrawPath(Vector3 navigator_pos, PathFinder.Path path)
	{
		navigatorPos = navigator_pos;
		navigatorPos.y += 0.5f;
		this.path = path;
	}

	public Navigator GetNavigator()
	{
		return navigator;
	}

	public void SetNavigator(Navigator navigator)
	{
		this.navigator = navigator;
	}

	public void ClearNavigator()
	{
		navigator = null;
	}

	private void DrawPath(PathFinder.Path path, Vector3 navigator_pos, Color color)
	{
		if (path.nodes == null || path.nodes.Count <= 1)
		{
			return;
		}
		GL.PushMatrix();
		material.SetPass(0);
		GL.Begin(1);
		GL.Color(color);
		GL.Vertex(navigator_pos);
		GL.Vertex(NavTypeHelper.GetNavPos(path.nodes[1].cell, path.nodes[1].navType));
		for (int i = 1; i < path.nodes.Count - 1; i++)
		{
			if (Grid.WorldIdx[path.nodes[i].cell] == ClusterManager.Instance.activeWorldId && Grid.WorldIdx[path.nodes[i + 1].cell] == ClusterManager.Instance.activeWorldId)
			{
				Vector3 navPos = NavTypeHelper.GetNavPos(path.nodes[i].cell, path.nodes[i].navType);
				Vector3 navPos2 = NavTypeHelper.GetNavPos(path.nodes[i + 1].cell, path.nodes[i + 1].navType);
				GL.Vertex(navPos);
				GL.Vertex(navPos2);
			}
		}
		GL.End();
		GL.PopMatrix();
	}

	private void OnPostRender()
	{
		DrawPath(path, navigatorPos, Color.white);
		path = default(PathFinder.Path);
		DebugDrawSelectedNavigator();
		if (navigator != null)
		{
			GL.PushMatrix();
			material.SetPass(0);
			GL.Begin(1);
			PathFinderQuery query = PathFinderQueries.drawNavGridQuery.Reset(null);
			navigator.RunQuery(query);
			GL.End();
			GL.PopMatrix();
		}
	}

	private void DebugDrawSelectedNavigator()
	{
		if (!DebugHandler.DebugPathFinding || SelectTool.Instance == null || SelectTool.Instance.selected == null)
		{
			return;
		}
		Navigator component = SelectTool.Instance.selected.GetComponent<Navigator>();
		if (!(component == null))
		{
			int mouseCell = DebugHandler.GetMouseCell();
			if (Grid.IsValidCell(mouseCell))
			{
				PathFinder.PotentialPath potential_path = new PathFinder.PotentialPath(Grid.PosToCell(component), component.CurrentNavType, component.flags);
				PathFinder.Path path = default(PathFinder.Path);
				PathFinder.UpdatePath(component.NavGrid, component.GetCurrentAbilities(), potential_path, PathFinderQueries.cellQuery.Reset(mouseCell), ref path);
				string text = "";
				text = text + "Source: " + Grid.PosToCell(component) + "\n";
				text = text + "Dest: " + mouseCell + "\n";
				text = text + "Cost: " + path.cost;
				DrawPath(path, component.GetComponent<KAnimControllerBase>().GetPivotSymbolPosition(), Color.green);
				DebugText.Instance.Draw(text, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
			}
		}
	}
}
