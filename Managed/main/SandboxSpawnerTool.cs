using System.Collections.Generic;
using UnityEngine;

public class SandboxSpawnerTool : InterfaceTool
{
	protected Color radiusIndicatorColor = new Color(0.5f, 0.7f, 0.5f, 0.2f);

	private int currentCell;

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		colors.Add(new ToolMenu.CellColorData(currentCell, radiusIndicatorColor));
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		currentCell = Grid.PosToCell(cursorPos);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		Place(Grid.PosToCell(cursor_pos));
	}

	private void Place(int cell)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			return;
		}
		string stringSetting = SandboxToolParameterMenu.instance.settings.GetStringSetting("SandboxTools.SelectedEntity");
		GameObject prefab = Assets.GetPrefab(stringSetting);
		if (stringSetting == MinionConfig.ID)
		{
			SpawnMinion();
		}
		else if (prefab.GetComponent<Building>() != null)
		{
			BuildingDef def = prefab.GetComponent<Building>().Def;
			def.Build(cell, Orientation.Neutral, null, def.DefaultElements(), 298.15f);
		}
		else
		{
			GameObject gameObject = GameUtil.KInstantiate(prefab, Grid.CellToPosCBC(currentCell, Grid.SceneLayer.Creatures), Grid.SceneLayer.Creatures);
			gameObject.SetActive(value: true);
			if (gameObject.GetComponent<MutantPlant>() != null)
			{
				gameObject.GetComponent<MutantPlant>().SetSubSpecies(0);
			}
		}
		UISounds.PlaySound(UISounds.Sound.ClickObject);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.entitySelector.row.SetActive(value: true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: false);
	}

	private void SpawnMinion()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID));
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 position = Grid.CellToPosCBC(currentCell, Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(position);
		gameObject.SetActive(value: true);
		MinionStartingStats minionStartingStats = new MinionStartingStats(is_starter_minion: false);
		minionStartingStats.Apply(gameObject);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.SandboxCopyElement))
		{
			int cell = Grid.PosToCell(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
			List<ObjectLayer> list = new List<ObjectLayer>();
			list.Add(ObjectLayer.Pickupables);
			list.Add(ObjectLayer.Plants);
			list.Add(ObjectLayer.Minion);
			list.Add(ObjectLayer.Building);
			if (Grid.IsValidCell(cell))
			{
				foreach (ObjectLayer item in list)
				{
					GameObject gameObject = Grid.Objects[cell, (int)item];
					if ((bool)gameObject)
					{
						SandboxToolParameterMenu.instance.settings.SetStringSetting("SandboxTools.SelectedEntity", gameObject.PrefabID().ToString());
						break;
					}
				}
			}
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}
}
