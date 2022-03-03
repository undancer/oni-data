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
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			Grid.CellToXY(cell, out var x, out var y);
			GameScenePartitioner.Instance.GatherEntries(x, y, 1, 1, GameScenePartitioner.Instance.plants, pooledList);
			if (pooledList.Count > 0)
			{
				Uprootable component = (pooledList[0].obj as Component).GetComponent<Uprootable>();
				if (component != null)
				{
					component.MarkForUproot();
				}
			}
			pooledList.Recycle();
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
			Prioritizable component2 = gameObject.GetComponent<Prioritizable>();
			if (component2 != null)
			{
				component2.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
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
