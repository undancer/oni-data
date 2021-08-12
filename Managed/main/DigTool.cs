using UnityEngine;

public class DigTool : DragTool
{
	public static DigTool Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (!Grid.Solid[cell])
		{
			foreach (Uprootable item in Components.Uprootables.Items)
			{
				if (Grid.PosToCell(item.gameObject) == cell)
				{
					item.MarkForUproot();
					break;
				}
				OccupyArea area = item.area;
				if (area != null && area.CheckIsOccupying(cell))
				{
					item.MarkForUproot();
				}
			}
		}
		if (DebugHandler.InstantBuildMode)
		{
			if (Grid.IsValidCell(cell) && Grid.Solid[cell] && !Grid.Foundation[cell])
			{
				WorldDamage.Instance.DestroyCell(cell);
			}
			return;
		}
		GameObject gameObject = PlaceDig(cell, distFromOrigin);
		if (gameObject != null)
		{
			Prioritizable component = gameObject.GetComponent<Prioritizable>();
			if (component != null)
			{
				component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
			}
		}
	}

	public static GameObject PlaceDig(int cell, int animationDelay = 0)
	{
		if (Grid.Solid[cell] && !Grid.Foundation[cell] && Grid.Objects[cell, 7] == null)
		{
			for (int i = 0; i < 42; i++)
			{
				if (Grid.Objects[cell, i] != null && Grid.Objects[cell, i].GetComponent<Constructable>() != null)
				{
					return null;
				}
			}
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")));
			gameObject.SetActive(value: true);
			Grid.Objects[cell, 7] = gameObject;
			Vector3 position = Grid.CellToPosCBC(cell, Instance.visualizerLayer);
			float num = -0.15f;
			position.z += num;
			gameObject.transform.SetPosition(position);
			gameObject.GetComponentInChildren<EasingAnimations>().PlayAnimation("ScaleUp", Mathf.Max(0f, (float)animationDelay * 0.02f));
			return gameObject;
		}
		if (Grid.Objects[cell, 7] != null)
		{
			return Grid.Objects[cell, 7];
		}
		return null;
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.Show();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(show: false);
	}
}
