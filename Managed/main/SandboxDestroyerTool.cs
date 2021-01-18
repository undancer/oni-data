using System.Collections.Generic;
using UnityEngine;

public class SandboxDestroyerTool : BrushTool
{
	public static SandboxDestroyerTool instance;

	protected HashSet<int> recentlyAffectedCells = new HashSet<int>();

	protected Color recentlyAffectedCellColor = new Color(1f, 1f, 1f, 0.1f);

	private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
		affectFoundation = true;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(value: true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: false);
	}

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		foreach (int recentlyAffectedCell in recentlyAffectedCells)
		{
			colors.Add(new ToolMenu.CellColorData(recentlyAffectedCell, recentlyAffectedCellColor));
		}
		foreach (int item in cellsInRadius)
		{
			colors.Add(new ToolMenu.CellColorData(item, radiusIndicatorColor));
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
	}

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		recentlyAffectedCells.Add(cell);
		Game.CallbackInfo item = new Game.CallbackInfo(delegate
		{
			recentlyAffectedCells.Remove(cell);
		});
		int index = Game.Instance.callbackManager.Add(item).index;
		int gameCell = cell;
		CellElementEvent sandBoxTool = CellEventLogger.Instance.SandBoxTool;
		int callbackIdx = index;
		SimMessages.ReplaceElement(gameCell, SimHashes.Vacuum, sandBoxTool, 0f, 0f, Db.Get().Diseases.GetIndex(Db.Get().Diseases.Get(settings.GetStringSetting("SandboxTools.SelectedDisease")).id), 0, callbackIdx);
		HashSetPool<GameObject, SandboxDestroyerTool>.PooledHashSet pooledHashSet = HashSetPool<GameObject, SandboxDestroyerTool>.Allocate();
		foreach (Pickupable item2 in Components.Pickupables.Items)
		{
			if (Grid.PosToCell(item2) == cell)
			{
				pooledHashSet.Add(item2.gameObject);
			}
		}
		foreach (BuildingComplete item3 in Components.BuildingCompletes.Items)
		{
			if (Grid.PosToCell(item3) == cell)
			{
				pooledHashSet.Add(item3.gameObject);
			}
		}
		if (Grid.Objects[cell, 1] != null)
		{
			pooledHashSet.Add(Grid.Objects[cell, 1]);
		}
		foreach (Crop item4 in Components.Crops.Items)
		{
			if (Grid.PosToCell(item4) == cell)
			{
				pooledHashSet.Add(item4.gameObject);
			}
		}
		foreach (Health item5 in Components.Health.Items)
		{
			if (Grid.PosToCell(item5) == cell)
			{
				pooledHashSet.Add(item5.gameObject);
			}
		}
		foreach (GameObject item6 in pooledHashSet)
		{
			Util.KDestroyGameObject(item6);
		}
		pooledHashSet.Recycle();
	}
}
